---
title: Extra coders
---

## Concept

Extra coders extend the set of types the auto API knows how to handle.

When an auto coder meets a type it doesn't know, it fails with:

```text
Cannot generate auto decoder for 'Tests.Types.BaseClass'. Please pass an extra decoder.
```

Pass an `ExtraCoders` holding a coder for that type.

## Ready to use extra coders

Thoth.Json ships four of them:

- `withInt64`
- `withUInt64`
- `withDecimal`
- `withBigInt`

```fsharp live
open Thoth.Json.Core
open Thoth.Json.Core.Auto
open Thoth.Json.JavaScript

let myExtra =
    Extra.empty
    |> Extra.withInt64
    |> Extra.withUInt64
    |> Extra.withDecimal
    |> Extra.withBigInt

"\"123\""
|> Decode.fromString (Decode.Auto.generateDecoder<uint64> (extra = myExtra))
|> Docs.print
```

These types are not supported by default because they have an important impact on the bundle size of
your application. Adding them is your choice, so applications that don't need them aren't penalised.

## Support for custom types

Use `withCustom` to add a coder of your own.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.Core.Auto
open Thoth.Json.JavaScript

type Point(x: int, y: int) =

    member _.X = x

    member _.Y = y

module Point =

    let decoder: Decoder<Point> =
        Decode.object (fun get ->
            Point(
                get.Required.Field "x" Decode.int,
                get.Required.Field "y" Decode.int
            )
        )

    let encoder (point: Point) =
        Encode.object
            [
                "x", Encode.int point.X
                "y", Encode.int point.Y
            ]

let myExtra = Extra.empty |> Extra.withCustom Point.encoder Point.decoder

[
    Point(1, 2)
    Point(3, 4)
]
|> Encode.Auto.generateEncoder<Point list> (extra = myExtra)
|> Encode.toString 4
|> printfn "%s"
```

## Override the default behaviour

:::warning
Be really careful when overriding the default coders.
:::

`withCustom` also replaces the coder of a type Thoth.Json already supports.

For example, to represent an `int` as an object with a `value` and a `type`:

```json
{
    "type": "int",
    "value": 42
}
```

```fsharp live
open Thoth.Json.Core
open Thoth.Json.Core.Auto
open Thoth.Json.JavaScript

module IntAsRecord =

    let encoder (value: int) =
        Encode.object
            [
                "type", Encode.string "int"
                "value", Encode.int value
            ]

    let decoder: Decoder<int> =
        Decode.field "type" Decode.string
        |> Decode.andThen (fun typ ->
            if typ = "int" then
                Decode.field "value" Decode.int
            else
                Decode.fail "Invalid type"
        )

let overrideDefaults =
    Extra.empty |> Extra.withCustom IntAsRecord.encoder IntAsRecord.decoder

42
|> Encode.Auto.generateEncoder<int> (extra = overrideDefaults)
|> Encode.toString 4
|> printfn "%s"
```

The override applies wherever the type appears, including inside records, lists and unions.
