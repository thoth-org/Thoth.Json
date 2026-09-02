---
title: Installation
---

Thoth.Json is split into a core package holding the API, and one package per runtime.

## Core

`Thoth.Json.Core` contains `Decode`, `Encode`, `Codec` and the computation expressions. It doesn't
read or write JSON on its own.

```bash frame=terminal
dotnet add package Thoth.Json.Core
```

## Runtime

Add the package matching the runtime your code targets. It supplies `Decode.fromString` and
`Encode.toString`.

| Package | Runtime |
|---|---|
| `Thoth.Json.JavaScript` | Fable, compiling to JavaScript |
| `Thoth.Json.Python` | Fable, compiling to Python |
| `Thoth.Json.Newtonsoft` | .NET, on top of Newtonsoft.Json |
| `Thoth.Json.System.Text.Json` | .NET, on top of System.Text.Json |

```bash frame=terminal
dotnet add package Thoth.Json.JavaScript
```

A project can reference more than one runtime package. This is what an isomorphic application does:
the Fable code opens `Thoth.Json.JavaScript`, the server code opens one of the .NET packages, and
both use the same coders.

## Auto API

`Thoth.Json.Core.Auto` generates coders from your F# types using reflection. It's a separate package
because reflection has a cost on bundle size.

```bash frame=terminal
dotnet add package Thoth.Json.Core.Auto
```

## Sharing code between Fable and .NET

Coders are written against `Thoth.Json.Core`, which compiles everywhere. Only the runtime package
differs, so a compiler directive is needed for its `open` alone.

```fs
open Thoth.Json.Core

#if FABLE_COMPILER_JAVASCRIPT
open Thoth.Json.JavaScript
#endif

#if FABLE_COMPILER_PYTHON
open Thoth.Json.Python
#endif

#if !FABLE_COMPILER
open Thoth.Json.Newtonsoft
#endif

// Here you can write your code as usual
```

## Migrating from Thoth.Json 10 and Thoth.Json.Net

`Thoth.Json` and `Thoth.Json.Net` are the previous generation of the library. Their documentation
lives under the **Legacy** version, in the picker at the top of this page.

The API is the same in spirit. The differences you'll meet when moving over:

- `Thoth.Json` and `Thoth.Json.Net` become `Thoth.Json.Core` plus a runtime package.
- `Decoder<'T>` is an interface instead of a function. A decoder written as
  `fun path value -> ...` becomes an object expression, see
  [Override defaults](manual/override-defaults.md).
- An encoder returns `IEncodable` instead of `JsonValue`.
- `Extra` moves to `Thoth.Json.Core.Auto`.
