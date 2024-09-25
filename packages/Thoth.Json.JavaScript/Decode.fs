namespace Thoth.Json.JavaScript

#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT || !FABLE_COMPILER

open Fable.Core
open Fable.Core.JsInterop
open Thoth.Json.Core

[<RequireQualifiedAccess>]
module Decode =

    let helpers =
        { new IDecoderHelpers<obj> with
            member _.isString jsonValue = jsonValue :? string
            member _.isNumber jsonValue = jsTypeof jsonValue = "number"
            member _.isBoolean jsonValue = jsonValue :? bool
            member _.isNullValue jsonValue = isNull jsonValue

            member _.isArray jsonValue =
                JS.Constructors.Array.isArray (jsonValue)

            member _.isObject jsonValue =
                emitJsStatement
                    jsonValue
                    """
return $0 === null ? false : (Object.getPrototypeOf($0 || false) === Object.prototype)
                """

            member _.hasProperty fieldName jsonValue =
                emitJsStatement
                    (jsonValue, fieldName)
                    """
return $0.hasOwnProperty($1);
                    """

            member _.isIntegralValue jsonValue =
                emitJsStatement
                    jsonValue
                    """
return isFinite($0) && Math.floor($0) === $0
                    """

            member _.asString jsonValue = unbox jsonValue
            member _.asBoolean jsonValue = unbox jsonValue
            member _.asArray jsonValue = unbox jsonValue
            member _.asFloat jsonValue = unbox jsonValue
            member _.asFloat32 jsonValue = unbox jsonValue
            member _.asInt jsonValue = unbox jsonValue

            member _.getProperties jsonValue =
                upcast JS.Constructors.Object.keys (jsonValue)

            member _.getProperty(fieldName: string, jsonValue: obj) =
                jsonValue?(fieldName)

            member _.anyToString jsonValue =
                emitJsStatement
                    jsonValue
                    """
return JSON.stringify($0, null, 4) + ''
                    """
        }

    module Helpers =

        [<Emit("$0 instanceof SyntaxError")>]
        let isSyntaxError (_: obj) : bool = jsNative

    let fromValue (decoder: Decoder<'T>) =
        Decode.Advanced.fromValue helpers decoder

    let fromString (decoder: Decoder<'T>) =
        fun value ->
            try
                let json = JS.JSON.parse value

                match decoder.Decode(helpers, json) with
                | Ok success -> Ok success
                | Error error ->
                    let finalError = error |> Decode.Helpers.prependPath "$"
                    Error(Decode.errorToString helpers finalError)

            with ex when Helpers.isSyntaxError ex ->
                Error("Given an invalid JSON: " + ex.Message)

    let unsafeFromString (decoder: Decoder<'T>) =
        fun value ->
            match fromString decoder value with
            | Ok x -> x
            | Error msg -> failwith msg

#endif
