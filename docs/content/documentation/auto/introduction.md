---
title: Introduction
---

The auto API generates coders from your F# types at runtime, using reflection. The JSON structure is
still checked, and the errors are the same as with a hand-written coder.

It's handy when using F# on both server and client, or when your JSON is a one-to-one mapping of
your F# types.

Auto coders live in their own package.

```bash
dotnet add package Thoth.Json.Core.Auto
```

```fsharp live
open Thoth.Json.Core
open Thoth.Json.Core.Auto
open Thoth.Json.JavaScript

type User =
    {
        Name: string
        Age: int
    }

let userJson =
    """
{
    "Name": "Geralt de Riv",
    "Age": 92
}
    """

userJson
|> Decode.fromString (Decode.Auto.generateDecoder<User> ())
|> Docs.print
```

## Generating a coder

`Decode.Auto.generateDecoder` and `Encode.Auto.generateEncoder` return an ordinary
`Decoder<'T>` and `Encoder<'T>`. They compose with everything in the manual API.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.Core.Auto
open Thoth.Json.JavaScript

type User =
    {
        Name: string
        Age: int
    }

{
    Name = "Geralt de Riv"
    Age = 92
}
|> Encode.Auto.generateEncoder<User> ()
|> Encode.toString 4
|> printfn "%s"
```

`Codec.Auto.generateCodec` returns both halves at once, see [Codecs](codecs.md).

:::warning
Generating a coder walks the type each time. Generate it once and reuse it, or use the
[cached](caching.md) variants.
:::

## Under Fable

The generate functions are `inline` so that Fable can resolve the generic parameter. A helper of
your own wrapping them must be `inline` too.

```fs
let inline decode<'T> (json: string) =
    Decode.fromString (Decode.Auto.generateDecoder<'T> ()) json
```
