namespace Thoth.Json.JavaScript

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

            member _.isUndefined jsonValue = jsTypeof jsonValue = "undefined"

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

            member _.getObjectKeys jsonValue =
                upcast JS.Constructors.Object.keys (jsonValue)

            member _.getField(fieldName: string, jsonValue: obj) =
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

    let fromString (decoder: Decoder<'T>) =
        fun value ->
            try
                let json = JS.JSON.parse value
                Decode.fromValue helpers "$" decoder json
            with ex when Helpers.isSyntaxError ex ->
                Error(
                    "Given an invalid JSON: "
                    + ex.Message
                )
