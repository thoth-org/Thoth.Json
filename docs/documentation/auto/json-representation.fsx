(**
---
layout: standard
title: JSON representation
---
*)

(*** hide ***)

#I "../../../src/bin/Debug/netstandard2.0"
#r "Thoth.Json.dll"
#r "nuget: Fable.Core"

open Thoth.Json
open Fable.Core

(**

When using auto API, you have a very limited control over the JSON representation.

## Case strategy

You can decide which case strategy you want to use:

- PascalCase (default)
- CamelCase
- SnakeCase

*)

type Device =
    {
        EquipmentId : int
        SerialNumber : string
    }

let myDevice =
    {
        EquipmentId = 5862
        SerialNumber = "1452-48-4298"
    }

Encode.Auto.toString(4, myDevice)

// Returns:
// {
//     "EquipmentId": "5862",
//     "SerialNumber": "1452-48-4298"
// }

Encode.Auto.toString(4, myDevice, caseStrategy = CaseStrategy.CamelCase)

// Returns:
// {
//     "equipmentId": "5862",
//     "serialNumber": "1452-48-4298"
// }

Encode.Auto.toString(4, myDevice, caseStrategy = CaseStrategy.SnakeCase)

// Returns:
// {
//     "equipment_id": "5862",
//     "serial_number": "1452-48-4298"
// }

(**

When decoding the case strategy is also respected.

*)

Decode.Auto.fromString<Device>(
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

Decode.Auto.fromString<Device>(
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

## Primitives

Primitives are represented with their corresponding equivalent in JSON.

There is a special case for the numbers see [Manual API - JSON representation - Numbers](/Thoth.Json/documentation/manual/json-representation.html#numbers).

## Records

Records are represented as JSON objects.

*)

type User =
    {
        Name : string
        Age : int
    }

Encode.Auto.toString(
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

Encode.Auto.toString(4, Language.FSharp)

// Returns: "FSharp"

Encode.Auto.toString(4, Language.CSharp)

// Returns: "C#"

(**

## Tuple with arguments

Tuples are represented using JSON arrays where the first elements is the name
of the case followed by as much elements as the tuple arguments.

*)

type MenuElement =
    | Label of label : string
    | ExternalLink of label : string * url : string

Encode.Auto.toString(4, Label "Introduction")

// Returns:
// [
//     "Label",
//     "Introduction"
// ]

Encode.Auto.toString(4,
    ExternalLink (
        label = "Fable",
        url = "http://fable.io"
    )
)

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

*)
