---
title: Codecs
---

`Codec.Auto.generateCodec` generates a [codec](../codec/introduction.md) from an F# type.

It takes the same `caseStrategy`, `extra`, `skipNullField` and `losslessOption` arguments as the
encoder and the decoder, and passes each to both halves. See [Configuration](configuration.md).

```fsharp live
open Thoth.Json.Core
open Thoth.Json.Core.Auto
open Thoth.Json.JavaScript

type FooBar =
    {
        Foo: int
        Bar: bool
        Baz: string list
    }

module FooBar =

    let codec: Codec<FooBar> = Codec.Auto.generateCodec (CamelCase)

let json =
    {
        Foo = 123
        Bar = true
        Baz = [ "abc" ]
    }
    |> Encode.codec FooBar.codec
    |> Encode.toString 4

printfn "%s" json

Decode.fromString FooBar.codec json |> Docs.print
```

## Round-trip

A generated codec round-trips the types the auto API supports, with two things to know.

### Nested options

Options are erased by default, so `Some None` and `None` both encode to `null` and both decode to
`None`. See [Option](json-representation.md#option).

Pass `losslessOption` to keep the distinction.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.Core.Auto
open Thoth.Json.JavaScript

type Response =
    {
        Data: int option option
    }

let codec: Codec<Response> = Codec.Auto.generateCodec (losslessOption = true)

let json =
    {
        Data = Some None
    }
    |> Encode.codec codec
    |> Encode.toString 0

printfn "%s" json

Decode.fromString codec json |> Docs.print
```

Read what the flag costs on the decoding side under
[losslessOption](configuration.md#losslessoption) before turning it on.

### DateTime

A `DateTime` is decoded as UTC, so only a value whose `Kind` is already `Utc` comes back as it went
in. See [DateTime](json-representation.md#datetime).
