(***
layout: nacara/layouts/docs.njk
title: Extra coders
***)


(*** hide ***)

#I "../../../src/bin/Debug/netstandard2.0"
#r "Thoth.Json.dll"
#r "nuget: Fable.Core"

open Thoth.Json
open Fable.Core

(**
## Concept

Extra coders is a mechanism allowing you to extend the default set of coders provided by Thoth.Json.

If an auto coders doesn't know how to handle a type, you will see an error like this:

```
Cannot generate auto decoder for Tests.Types.BaseClass. Please pass an extra decoder.
```

This is because the auto coder doesn't know how to handle the type `Tests.Types.BaseClass` and you need to provide an extra coder for it.

## Ready to use extra coders

Thoth.Json have the following extra coders ready to use:

- `withInt64`
- `withUInt64`
- `withDecimal`
- `withBigInt`

You can use them like this:

*)

let myExtra =
    Extra.empty
    |> Extra.withInt64
    |> Extra.withUInt64
    |> Extra.withDecimal
    |> Extra.withBigInt

Decode.Auto.fromString<uint64> ("123", extra = myExtra)
// Result: Ok 123UL

(**
There types are not supported by default because they have a important impact on
the bundle size of your application.

So we decided to let you choose if you want to use them or not, to not penalize
the application that don't need them.

## Support for custom types

You can also create your own extra coders. You can use the `withCustom` function

*)


type Point(x: int, y: int) =

    member __.x = x

    member __.y = y

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
                "x", Encode.int point.x
                "y", Encode.int point.y
            ]

let myExtra2 = Extra.empty |> Extra.withCustom Point.encoder Point.decoder

Encode.Auto.toString (4, [ Point(1, 2), Point(3, 4) ], extra = myExtra2)

(**
## Override the default behavior

:::warning
Be really careful when overriding the default coders
:::

You can also use the `withCustom` function to override the default.

For example, if you want to represents the `int` using an object with a property `value` and a `type`:

```json
{
    "type": "int",
    "value": 42
}
```

You can do it like this:
*)


module IntAsRecord =

    let encode (value: int) =
        Encode.object
            [
                "type", Encode.string "int"
                "value", Encode.int value
            ]

    let decode: Decoder<int> =
        Decode.field "type" Decode.string
        |> Decode.andThen (fun typ ->
            if typ = "int" then
                Decode.field "value" Decode.int
            else
                Decode.fail "Invalid type"
        )

let overrideDefaults =
    Extra.empty |> Extra.withCustom IntAsRecord.encode IntAsRecord.decode

Encode.Auto.toString (4, 42, extra = overrideDefaults)
