---
title: Cache
---

Auto coders are generated at runtime by walking the type. To avoid generating them again every time
you need one, use the helpers with the `Cached` suffix.

The cache is keyed on the type and on the arguments you pass, so a type generated with two different
case strategies is cached twice.

The easiest way to use the cache is to add helpers to your code and use them in the rest of your
application.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.Core.Auto
open Thoth.Json.JavaScript

// Build your extra coders if needed
let myExtraCoders = Extra.empty

// The helpers must be inlined to resolve generic parameters in Fable
let inline myDecoder<'T> =
    Decode.Auto.generateDecoderCached<'T> (
        caseStrategy = CamelCase,
        extra = myExtraCoders
    )

let inline myEncoder<'T> =
    Encode.Auto.generateEncoderCached<'T> (
        caseStrategy = CamelCase,
        extra = myExtraCoders
    )

type User =
    {
        Name: string
        Age: int
    }

let userJson =
    """
{
    "name": "Geralt de Riv",
    "age": 92
}
    """

userJson |> Decode.fromString myDecoder<User> |> Docs.print
```

:::warning
Build your `ExtraCoders` once and share it. `Extra.withCustom` stamps a new identity on the result,
so an `ExtraCoders` rebuilt on every call misses the cache every time.
:::

On .NET the cache is held per thread, so no locking is involved.
