---
title: Unions
---

The `variantCodec` computation expression builds a codec for a discriminated union.

Each `Codec.case` names a tag, gives the case constructor, and the codec for the case fields. The
`return` maps a value back to the case that was declared for it.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

type Shape =
    | Square of width: int
    | Rectangle of width: int * height: int
    | Circle of radius: int

module Shape =

    let codec: Codec<Shape> =
        variantCodec {
            let! square = Codec.case "square" Square Codec.int

            and! rectangle =
                Codec.case
                    "rectangle"
                    Rectangle
                    (Codec.tuple2 Codec.int Codec.int)

            and! circle = Codec.case "circle" Circle Codec.int

            return
                function
                | Square width -> square width
                | Rectangle(width, height) -> rectangle (width, height)
                | Circle radius -> circle radius
        }

let json = Rectangle(7, 2) |> Encode.codec Shape.codec |> Encode.toString 0

printfn "%s" json

Decode.fromString Shape.codec json |> Docs.print
```

A case with several fields takes a `Codec.tuple2` to `Codec.tuple8`, in the order the fields are
declared.

## The two representations

### One property per case

`variantCodec` writes an object with a single property, named after the tag.

```json
{ "square": 4 }

{ "rectangle": [ 7, 2 ] }
```

Decoding fails unless the object carries exactly one recognised tag.

### A tag and a value property

`variantCodecWithTag` writes the tag and the value under property names you choose.

```json
{ "type": "square", "value": 4 }

{ "type": "rectangle", "value": [ 7, 2 ] }
```

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

type Shape =
    | Square of width: int
    | Circle of radius: int

module Shape =

    let codec: Codec<Shape> =
        variantCodecWithTag "type" "value" {
            let! square = Codec.case "square" Square Codec.int
            and! circle = Codec.case "circle" Circle Codec.int

            return
                function
                | Square width -> square width
                | Circle radius -> circle radius
        }

let json = Circle 3 |> Encode.codec Shape.codec |> Encode.toString 0

printfn "%s" json

Decode.fromString Shape.codec json |> Docs.print
```

## Cases without fields

Give the case `Codec.unit`, and a constructor ignoring its argument. The value is written as `null`.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

type Color =
    | Red
    | Rgb of int * int * int

module Color =

    let codec: Codec<Color> =
        variantCodec {
            let! red = Codec.case "red" (fun () -> Red) Codec.unit

            and! rgb =
                Codec.case
                    "rgb"
                    Rgb
                    (Codec.tuple3 Codec.int Codec.int Codec.int)

            return
                function
                | Red -> red ()
                | Rgb(r, g, b) -> rgb (r, g, b)
        }

Red |> Encode.codec Color.codec |> Encode.toString 0 |> printfn "%s"

Rgb(255, 0, 0) |> Encode.codec Color.codec |> Encode.toString 0 |> printfn "%s"

Decode.fromString Color.codec """{ "red": null }""" |> Docs.print
```

## Enums

`Codec.Enum.int` and its siblings represent an enum as the number it compiles to. There is one per
underlying type: `byte`, `sbyte`, `int16`, `uint16`, `int` and `uint32`.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

type Rating =
    | One = 1
    | Two = 2
    | Three = 3

let codec: Codec<Rating> = Codec.Enum.int

Encode.codec codec Rating.Two |> Encode.toString 0 |> printfn "%s"

Decode.fromString codec "2" |> Docs.print
```
