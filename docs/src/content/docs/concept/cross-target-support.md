---
title: Cross-target support
---

Thoth.Json runs on .NET and on the runtimes supported by [Fable](https://fable.io/).

Decoders and encoders are written once against `Thoth.Json.Core`, which knows nothing about how JSON
is represented at runtime. Reading and writing the actual JSON is the job of a runtime package, and
that is the only part of your code which depends on the target you are compiling for.

## Packages

| Package                       | Runtime                                |
| ----------------------------- | -------------------------------------- |
| `Thoth.Json.Core`             | Any, this is where you write your coders |
| `Thoth.Json.JavaScript`       | JavaScript, via Fable                  |
| `Thoth.Json.Python`           | Python, via Fable                      |
| `Thoth.Json.Newtonsoft`       | .NET, using Newtonsoft.Json            |
| `Thoth.Json.System.Text.Json` | .NET, using System.Text.Json           |

Only the project which actually reads or writes JSON needs a runtime package. A project defining
coders can reference `Thoth.Json.Core` alone and leave the choice to the projects depending on it.

On .NET, the choice between `Thoth.Json.Newtonsoft` and `Thoth.Json.System.Text.Json` is yours, both
expose the same API.

:::note
`Thoth.Json.Core.Auto`, which generates coders from your types, is portable too. Like
`Thoth.Json.Core`, it still needs a runtime package to read and write JSON.
:::

## Writing coders

Coders only need `Thoth.Json.Core`, so the code below compiles for every target:

```fs
open Thoth.Json.Core

type User =
    {
        Name: string
        Age: int
    }

module User =

    let decoder: Decoder<User> =
        Decode.object (fun get ->
            {
                Name = get.Required.Field "name" Decode.string
                Age = get.Required.Field "age" Decode.int
            }
        )

    let encoder: Encoder<User> =
        fun user ->
            Encode.object [
                "name", Encode.string user.Name
                "age", Encode.int user.Age
            ]
```

## Running them

The runtime package provides the entry points, `Decode.fromString` and `Encode.toString`:

```fs
open Thoth.Json.Core
open Thoth.Json.Newtonsoft

let user = """{ "name": "Maxime", "age": 30 }""" |> Decode.fromString User.decoder
// Ok { Name = "Maxime"; Age = 30 }

let json = { Name = "Maxime"; Age = 30 } |> User.encoder |> Encode.toString 4
```

Targeting JavaScript instead means changing the `open`, nothing else:

```fs
open Thoth.Json.Core
open Thoth.Json.JavaScript
```

:::note
Opening both `Thoth.Json.Core` and a runtime package is expected. The first one brings the coders
such as `Decode.string`, the second one the entry points such as `Decode.fromString`.
:::

## Sharing code between targets

Because coders are portable, the recommended approach is to keep them in a project which only
references `Thoth.Json.Core`. Each application referencing that project then picks the runtime
package matching what it compiles to, and nothing in your domain code has to know about it.

A server and a client sharing their coders look like this:

```text
Shared      → Thoth.Json.Core                (types and coders)
Server      → Shared + Thoth.Json.System.Text.Json
Client      → Shared + Thoth.Json.JavaScript
```

`Shared` is compiled twice, once for .NET and once by Fable, and reads the same in both cases.

When a single file has to serve several targets, use the Fable compiler directives to select the
runtime package:

```fs
open Thoth.Json.Core

#if FABLE_COMPILER_JAVASCRIPT
open Thoth.Json.JavaScript
#elif FABLE_COMPILER_PYTHON
open Thoth.Json.Python
#else
open Thoth.Json.Newtonsoft
#endif

// From here, the code is the same for all the targets
```

## Previous versions

Before `Thoth.Json.Core`, .NET and Fable were supported by two separate libraries, `Thoth.Json.Net`
and `Thoth.Json`, sharing your code between them required compiler directives. They are documented in
the [legacy documentation](/Thoth.Json/legacy/).
