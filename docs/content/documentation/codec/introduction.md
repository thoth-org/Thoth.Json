---
title: Introduction
---

A codec holds an encoder and a decoder for the same type.

```fs
type Codec<'T> =
    {
        Encoder: Encoder<'T>
        Decoder: Decoder<'T>
    }
```

Describing a type once, instead of twice, keeps the two directions in step. Rename a field and both
sides change together.

A codec round-trips: decoding what it encoded gives the value back, any number of times. Every
combinator below keeps that property, except `Codec.oneOf` and `Codec.nil`, which say so.

Everything the manual API offers has a codec counterpart. Where you would write two coders, write
one codec and take it apart when you need one half.

## Primitives

`Codec` provides a codec for each primitive type: `Codec.string`, `Codec.int`, `Codec.bool`,
`Codec.guid`, `Codec.decimal`, and so on. They match the
[representation](../manual/json-representation.md) of the manual API.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

Encode.codec Codec.int 42 |> Encode.toString 0 |> printfn "%s"

Decode.fromString Codec.int "42" |> Docs.print
```

## Collections

`Codec.list`, `Codec.array`, `Codec.seq`, `Codec.resizeArray` and `Codec.tuple2` to `Codec.tuple8`
lift a codec into a collection.

`Codec.dict` represents a `Map<string, 'T>` as a JSON object. `Codec.map'` takes a codec for the key
as well, and represents the map as an array of pairs.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

let codec = Codec.list (Codec.tuple2 Codec.string Codec.int)

let json =
    [
        "one", 1
        "two", 2
    ]
    |> Encode.codec codec
    |> Encode.toString 0

printfn "%s" json

Decode.fromString codec json |> Docs.print
```

## Option

`Codec.lossyOption` writes `Some x` as `x` and `None` as `null`. `Codec.losslessOption` writes an
object carrying the case, which round-trips nested options. See
[Option](../manual/json-representation.md#option).

## Mapping to another type

`Codec.map` turns a `Codec<'T>` into a `Codec<'U>`. It takes the two conversions, one for each
direction.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

type Email = Email of string

module Email =

    let codec: Codec<Email> =
        Codec.string |> Codec.map Email (fun (Email value) -> value)

Encode.codec Email.codec (Email "maxime@mail.com")
|> Encode.toString 0
|> printfn "%s"

Decode.fromString Email.codec "\"maxime@mail.com\"" |> Docs.print
```

## Reading several representations

`Codec.oneOf` decodes with the first codec of the list that succeeds, and always encodes with the
first one.

Use it to accept an old representation while writing the new one.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

let codec =
    Codec.oneOf
        [
            Codec.int
            Codec.string |> Codec.map int string
        ]

Decode.fromString codec "42" |> Docs.print

Decode.fromString codec "\"42\"" |> Docs.print

Encode.codec codec 42 |> Encode.toString 0 |> printfn "%s"
```

## Taking a codec apart

`Codec.encoder` and `Codec.decoder` return one half of a codec. `Encode.codec` and `Decode.codec` do
the same, and read better in a pipeline.

The runtime packages accept a codec wherever they accept a decoder, so `Decode.fromString` takes
either.

## Building one from existing coders

`Codec.create` pairs an encoder and a decoder you already have.

```fs
let codec = Codec.create User.encoder User.decoder
```

Nothing checks that the two agree. Prefer the [object](objects.md) and [union](unions.md) builders,
which derive both sides from one description.
