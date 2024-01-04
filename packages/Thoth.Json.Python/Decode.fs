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
    let pyList: obj = nativeOnly

    [<Global; Emit("dict")>]
    let pyDict: obj = nativeOnly

    [<Global; Emit("int")>]
    let pyInt: obj = nativeOnly

    [<Global; Emit("bool")>]
    let pyBool: obj = nativeOnly

[<RequireQualifiedAccess>]
module Decode =

    let helpers =
        { new IDecoderHelpers<obj> with
            member _.isString jsonValue = jsonValue :? string

            member _.isNumber jsonValue =
                pyInstanceof jsonValue numbers.Number
                // In Python, bool is a subclass of int so we need to check
                // that the value is not a bool
                && not (pyInstanceof jsonValue pyBool)

            member _.isBoolean jsonValue = jsonValue :? bool
            member _.isNullValue jsonValue = isNull jsonValue

            member _.isArray jsonValue = pyInstanceof jsonValue pyList

            member _.isObject jsonValue = pyInstanceof jsonValue pyDict

            member _.hasProperty fieldName jsonValue =
                emitPyStatement (jsonValue, fieldName) "return $1 in $0"

            member _.isIntegralValue jsonValue = pyInstanceof jsonValue pyInt

            member _.asString jsonValue = unbox jsonValue
            member _.asBoolean jsonValue = unbox jsonValue
            member _.asArray jsonValue = unbox jsonValue
            member _.asFloat jsonValue = unbox jsonValue
            member _.asFloat32 jsonValue = unbox jsonValue
            member _.asInt jsonValue = unbox jsonValue

            member _.getProperties jsonValue =
                emitPyStatement jsonValue "return $0.keys()"

            member _.getProperty(fieldName: string, jsonValue: obj) =
                jsonValue?(fieldName)

            member _.anyToString jsonValue =
                Python.Json.json.dumps (jsonValue, indent = 4)
        }

    let fromString (decoder: Decoder<'T>) =
        fun value ->
            try
                let json = Fable.Python.Json.json.loads value

                match decoder.Decode(helpers, json) with
                | Ok success -> Ok success
                | Error error ->
                    let finalError = error |> Decode.Helpers.prependPath "$"
                    Error(Decode.errorToString helpers finalError)

            with :? Python.Json.JSONDecodeError as ex ->
                Error("Given an invalid JSON: " + ex.Message)

    let unsafeFromString (decoder: Decoder<'T>) =
        fun value ->
            match fromString decoder value with
            | Ok x -> x
            | Error e -> failwith e
