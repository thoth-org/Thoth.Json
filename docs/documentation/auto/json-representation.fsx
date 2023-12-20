(***
layout: nacara/layouts/docs.njk
title: JSON representation
toc:
    to: 3
***)

(*** hide ***)

#I "../../../src/bin/Debug/netstandard2.0"
#r "Thoth.Json.dll"
#r "nuget: Fable.Core"

open Thoth.Json
open Fable.Core

(**

## Configuration

When using auto API, you have a very limited control over the JSON representation.

### Case strategy

You can decide which case strategy you want to use:

- PascalCase (default)
- CamelCase
- SnakeCase

*)

type Device =
    {
        EquipmentId: int
        SerialNumber: string
    }

let myDevice =
    {
        EquipmentId = 5862
        SerialNumber = "1452-48-4298"
    }

Encode.Auto.toString (4, myDevice)

// Returns:
// {
//     "EquipmentId": "5862",
//     "SerialNumber": "1452-48-4298"
// }

Encode.Auto.toString (4, myDevice, caseStrategy = CaseStrategy.CamelCase)

// Returns:
// {
//     "equipmentId": "5862",
//     "serialNumber": "1452-48-4298"
// }

Encode.Auto.toString (4, myDevice, caseStrategy = CaseStrategy.SnakeCase)

// Returns:
// {
//     "equipment_id": "5862",
//     "serial_number": "1452-48-4298"
// }

(**

When decoding the case strategy is also respected.

*)

Decode.Auto.fromString<Device> (
    """
{
    "equipment_id": "5862",
    "serial_number": "1452-48-4298"
}
    """,
    caseStrategy = CaseStrategy.SnakeCase
)

// Returns:
// Ok { EquipmentId = 5862; SerialNumber = "1452-48-4298" }

Decode.Auto.fromString<Device> (
    """
{
    "EquipmentId": "5862",
    "SerialNumber": "1452-48-4298"
}
    """,
    caseStrategy = CaseStrategy.SnakeCase
)

// Returns:
// Error
// Error at: `$.serial_number`
// Expecting a string but instead got: undefined

(**

### Skip null fields

By default, optional fields are not included in the JSON representation.

*)

type Response<'T> =
    {
        Code: int
        Data: 'T option
    }

let response =
    {
        Code = 200
        Data = None
    }

Encode.Auto.toString (4, response)

// Returns:
// {
//     "Code": "200"
// }

(**

If you want to include the `Data` field, you need to set `skipNullField = false`

*)

Encode.Auto.toString (4, response, skipNullField = false)

// Returns:
// {
//     "Code": "200",
//     "Data": null
// }

(**

### Extra coders

The auto APIs, accept an `ExtraCoders` objects which is used to extends or override
the supported types.

In order to minimize the impact on the bundle size, Thoth.Json don't include
out of the box support for the following types:

- `int64`
- `uint64`
- `decimal`
- `bigint`

If you want to use them, you can add them to the `ExtraCoders` object.

*)

let myExtra =
    Extra.empty
    |> Extra.withInt64
    |> Extra.withUInt64
    |> Extra.withDecimal
    |> Extra.withBigInt

Encode.Auto.toString (4, 86UL, extra = myExtra)

// Returns:
// "86"

(**

## Primitives

By default, primitives are represented the same way as they are when using the Manual API.

See [Manual API - JSON representation - Numbers](/Thoth.Json/documentation/manual/json-representation.html#numbers) for more information.

If the default is not what you want, you can override them by using the `extra` argument.

**)

let customIntEncoder (value: int) =
    Encode.object
        [
            "type", Encode.string "customInt"
            "value", Encode.int value
        ]

let customIntDecoder =
    Decode.field "type" Decode.string
    |> Decode.andThen (
        function
        | "customInt" -> Decode.field "value" Decode.int

        | _ -> Decode.fail "Invalid type for customInt"
    )

let extra = Extra.empty |> Extra.withCustom customIntEncoder customIntDecoder

Encode.Auto.toString (4, 42, extra = extra)

// Returns:
// {
//     "type": "customInt",
//     "value": 42
// }

(**

## Records

Records are represented as JSON objects.

*)

type User =
    {
        Name: string
        Age: int
    }

Encode.Auto.toString (
    4,
    {
        Name = "Geralt de Riv"
        Age = 92
    }
)

// Returns:
// {
//     "Name": "Geralt de Riv",
//     "Age": 92
// }

(**

## Tuple with no arguments

Tuple without arguments are represented as a string containing the case name:

:::info
The case name respect the `CompiledName` attributes if provided.
:::

*)

[<RequireQualifiedAccess>]
type Language =
    | FSharp
    | [<CompiledName("C#")>] CSharp

Encode.Auto.toString (4, Language.FSharp)

// Returns: "FSharp"

Encode.Auto.toString (4, Language.CSharp)

// Returns: "C#"

(**

## Tuple with arguments

Tuples are represented using JSON arrays where the first elements is the name
of the case followed by as much elements as the tuple arguments.

*)

type MenuElement =
    | Label of label: string
    | ExternalLink of label: string * url: string

Encode.Auto.toString (4, Label "Introduction")

// Returns:
// [
//     "Label",
//     "Introduction"
// ]

Encode.Auto.toString (4, ExternalLink(label = "Fable", url = "http://fable.io"))

// Returns:
// [
//     "ExternalLink",
//     "Fable",
//     "http://fable.io"
// ]

(**

## Option type

In current version of Thoth.Json the option type are erased.

This means that:

- `Some 42` is encoded as `42`.
- `None` is encoded as `null`.

:::warning
This means that nested option types support is limited.

If you have a nested option like `(int option) option`.

Then you can't differentiate if `42` is for `Some 42` and `Some (Some 42)`.

The same goes for `null` and `Some None` or `None `.
:::

## Class

Classes support need to be added case by case via the `extra` argument.

This is because Fable offer a limited reflection API and classes are not supported.

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

// Returns:
// [
//     {
//         "x": 1,
//         "y": 2
//     },
//     {
//         "x": 3,
//         "y": 4
//     }
// ]
