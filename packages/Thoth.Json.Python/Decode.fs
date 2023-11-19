namespace Thoth.Json.Python

open Fable.Core
open Fable.Core.PyInterop
open Thoth.Json.Core
open Fable.Python

[<AutoOpen>]
module Interop =

    [<Erase>]
    type IExports =
        abstract Number: obj

    [<ImportAll("numbers")>]
    let numbers: IExports = nativeOnly

    [<Global; Emit("list")>]
    let pyList : obj = nativeOnly


[<RequireQualifiedAccess>]
module Decode =

    let helpers =
        { new IDecoderHelpers<obj> with
            member _.isString jsonValue = jsonValue :? string
            member _.isNumber jsonValue = pyInstanceof jsonValue numbers.Number
            member _.isBoolean jsonValue = jsonValue :? bool
            member _.isNullValue jsonValue = isNull jsonValue

            member _.isArray jsonValue =
                pyInstanceof jsonValue pyList

            member _.isObject jsonValue =
//                 emitJsStatement
//                     jsonValue
//                     """
// return $0 === null ? false : (Object.getPrototypeOf($0 || false) === Object.prototype)
//                 """
                false

            member _.isUndefined jsonValue = isNull jsonValue

            member _.isIntegralValue jsonValue =
                false

            member _.asString jsonValue = unbox jsonValue
            member _.asBoolean jsonValue = unbox jsonValue
            member _.asArray jsonValue = unbox jsonValue
            member _.asFloat jsonValue = unbox jsonValue
            member _.asFloat32 jsonValue = unbox jsonValue
            member _.asInt jsonValue = unbox jsonValue

            member _.getObjectKeys jsonValue =
                upcast JS.Constructors.Object.keys (jsonValue)

            member _.getField(fieldName: string, jsonValue: obj) =
                jsonValue?(fieldName)

            member _.anyToString jsonValue =
                ""
        }

    let fromString (decoder: Decoder<'T>) =
        fun value ->
            try
                let json = Fable.Python.Json.json.loads value
                Decode.fromValue helpers "$" decoder json
            // with ex when Helpers.isSyntaxError ex
            with ex -> // TODO: Capture only the exact exception
                Error(
                    "Given an invalid JSON: "
                    + ex.Message
                )
