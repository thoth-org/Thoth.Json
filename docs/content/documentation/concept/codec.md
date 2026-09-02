---
title: Codec
---

A codec is an encoder and a decoder for the same type, held together.

```fs
type Codec<'T> =
    {
        Encoder: Encoder<'T>
        Decoder: Decoder<'T>
    }
```

Writing the two halves separately means a field renamed on one side and forgotten on the other
compiles. A codec describes each field once, so both directions are always built from the same
description.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

type Point =
    {
        X: int
        Y: int
    }

module Point =

    let codec: Codec<Point> =
        objectCodec {
            let! x = Codec.field "x" _.X Codec.int
            and! y = Codec.field "y" _.Y Codec.int

            return
                {
                    X = x
                    Y = y
                }
        }

let json =
    {
        X = 1
        Y = 2
    }
    |> Encode.codec Point.codec
    |> Encode.toString 4

printfn "%s" json

Decode.fromString Point.codec json |> Docs.print
```

`Codec.encoder` and `Codec.decoder` take a codec apart when you need one half of it. `Encode.codec`
and `Decode.codec` do the same, in a shape that composes with the rest of each module.

The [Codec API](../codec/introduction.md) section covers building them.
