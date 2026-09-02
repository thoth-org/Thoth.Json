---
title: Decoder
---

A decoder describes the JSON you expect and produces an F# value from it.

`Decode.fromString` runs a decoder against a JSON string and returns a `Result`.

**Example of a successful decoder:**

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

Decode.fromString Decode.int "1" |> Docs.print
```

**Example of a failed decoder:**

In case of an error, a decoder returns a helpful error explaining:

1. Where the error happened in the JSON using the JSONPath syntax.

    You can use tools like [JSONPath Online Evaluator](http://jsonpath.com/) to explore your JSON

    <span/>

2. What was expected

3. What was received

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

Decode.fromString Decode.int "\"maxime\"" |> Docs.print
```

## Where decoders come from

`Thoth.Json.Core` gives you a decoder for each primitive type, and combinators to build decoders for
your own types. See [Composition](../manual/composition.md).

## Running a decoder

The runtime package provides three entry points.

### Decode.fromString

Parses the string and runs the decoder. Returns `Result<'T, string>`, where the error is the
formatted message shown above.

### Decode.unsafeFromString

The same, but raises an exception carrying the error message instead of returning a `Result`.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

printfn "%i" (Decode.unsafeFromString Decode.int "1")
```

### Decode.fromValue

Runs the decoder against a value already parsed by the runtime, without going through a string.
Its argument type is the one of the underlying library: `obj` for JavaScript, `JsonElement` for
System.Text.Json, `JToken` for Newtonsoft.
