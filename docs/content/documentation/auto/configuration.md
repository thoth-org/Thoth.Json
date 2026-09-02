---
title: Configuration
---

The generate functions take optional arguments controlling the JSON they produce and accept. The
same arguments exist on `Decode.Auto`, `Encode.Auto` and `Codec.Auto`, minus the ones which only
make sense in one direction.

## caseStrategy

**default:** the names as declared in F#

Renames record fields and union cases.

| Strategy | `EquipmentId` becomes |
|---|---|
| `PascalCase` | `EquipmentId` |
| `CamelCase` | `equipmentId` |
| `SnakeCase` | `equipment_id` |
| `ScreamingSnakeCase` | `EQUIPMENT_ID` |
| `DotNetPascalCase` | `EquipmentID` |
| `DotNetCamelCase` | `equipmentID` |

The two `DotNet` strategies keep the acronyms `ID` and `IP` in capitals, the way .NET names its own
types.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.Core.Auto
open Thoth.Json.JavaScript

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

myDevice
|> Encode.Auto.generateEncoder<Device> ()
|> Encode.toString 4
|> printfn "%s"

myDevice
|> Encode.Auto.generateEncoder<Device> (caseStrategy = CamelCase)
|> Encode.toString 4
|> printfn "%s"

myDevice
|> Encode.Auto.generateEncoder<Device> (caseStrategy = SnakeCase)
|> Encode.toString 4
|> printfn "%s"
```

The decoder respects the strategy as well, so pass the same one on both sides.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.Core.Auto
open Thoth.Json.JavaScript

type Device =
    {
        EquipmentId: int
        SerialNumber: string
    }

"""
{
    "equipment_id": 5862,
    "serial_number": "1452-48-4298"
}
"""
|> Decode.fromString (Decode.Auto.generateDecoder<Device> (caseStrategy = SnakeCase))
|> Docs.print

"""
{
    "EquipmentId": 5862,
    "SerialNumber": "1452-48-4298"
}
"""
|> Decode.fromString (Decode.Auto.generateDecoder<Device> (caseStrategy = SnakeCase))
|> Docs.print
```

## skipNullField

**default:** `true`

Encoding only. When `true`, a field holding `None` is left out of the object.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.Core.Auto
open Thoth.Json.JavaScript

type Response =
    {
        Code: int
        Data: string option
    }

let response =
    {
        Code = 200
        Data = None
    }

response
|> Encode.Auto.generateEncoder<Response> ()
|> Encode.toString 4
|> printfn "%s"

response
|> Encode.Auto.generateEncoder<Response> (skipNullField = false)
|> Encode.toString 4
|> printfn "%s"
```

## losslessOption

**default:** `false`

Writes options as an object carrying the case, so nested options round-trip. See
[Option](../manual/json-representation.md#option).

```fsharp live
open Thoth.Json.Core
open Thoth.Json.Core.Auto
open Thoth.Json.JavaScript

type Response =
    {
        Data: string option
    }

{
    Data = Some "hello"
}
|> Encode.Auto.generateEncoder<Response> (losslessOption = true)
|> Encode.toString 0
|> printfn "%s"
```

Pass it to the decoder as well, or the object won't be read back.

:::warning
The decoder reads an option field leniently when this is on: a value it can't read becomes `None`
instead of an error. An `int option` field holding `"thirty"` decodes to `None`, and so does a
document written without the flag.
:::

## extra

**default:** `Extra.empty`

Adds coders for types the auto API doesn't handle, and overrides the ones it does. See
[Extra coders](extra-coders.md).
