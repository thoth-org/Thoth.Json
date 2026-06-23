namespace Thoth.Json.Python

open Fable.Core
open Fable.Core.PyInterop
open Thoth.Json.Core
open Fable.Python
open Fable.Python.Fable.Types

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
                (pyInstanceof jsonValue numbers.Number
                 // In Python, bool is a subclass of int so we need to check
                 // that the value is not a bool
                 && not (pyInstanceof jsonValue pyBool))
                || isNumericType jsonValue

            member _.isBoolean jsonValue = jsonValue :? bool
            member _.isNullValue jsonValue = isNull jsonValue

            member _.isArray jsonValue = pyInstanceof jsonValue pyList

            member _.isObject jsonValue = pyInstanceof jsonValue pyDict

            member _.hasProperty fieldName jsonValue =
                emitPyStatement (jsonValue, fieldName) "return $1 in $0"

            member _.isIntegralValue jsonValue =
                pyInstanceof jsonValue pyInt || isIntegralType jsonValue

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
                Fable.Python.Json.Json.dumps (jsonValue, indent = 4)

            member _.numberToString jsonValue =
                if pyInstanceof jsonValue pyInt || isIntegralType jsonValue then
                    emitPyStatement jsonValue "return str(int($0))"
                else
                    string (unbox<float> jsonValue)
        }

type Decode =

    static member fromValue(decoder: Decoder<'T>) =
        Decode.Advanced.fromValue Decode.helpers decoder

    static member fromValue(codec: Codec<'T>) =
        codec |> Decode.codec |> Decode.fromValue

    static member fromString(decoder: Decoder<'T>) =
        fun value ->
            try
                let json = Fable.Python.Json.json.loads value

                match decoder.Decode(Decode.helpers, json) with
                | Ok success -> Ok success
                | Error error ->
                    let finalError = error |> Decode.Helpers.prependPath "$"
                    Error(Decode.errorToString Decode.helpers finalError)

            with :? Python.Json.JSONDecodeError as ex ->
                Error("Given an invalid JSON: " + ex.Message)

    static member fromString(codec: Codec<'T>) =
        codec |> Decode.codec |> Decode.fromString

    static member unsafeFromString(decoder: Decoder<'T>) =
        fun value ->
            match Decode.fromString decoder value with
            | Ok x -> x
            | Error e -> failwith e

    static member unsafeFromString(codec: Codec<'T>) =
        codec |> Decode.codec |> Decode.unsafeFromString
