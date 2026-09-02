namespace Thoth.Json.JavaScript

open Fable.Core
open Fable.Core.JsInterop
open Thoth.Json.Core

[<RequireQualifiedAccess>]
module Decode =

    /// <summary>
    /// Reads a <c>obj</c>, so a decoder written against Thoth.Json.Core runs on JavaScript.
    /// </summary>
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

            member _.numberToString jsonValue =
                emitJsStatement
                    jsonValue
                    """
return String($0)
                    """
        }

    module Interop =

        [<Emit("$0 instanceof SyntaxError")>]
        let isSyntaxError (_: obj) : bool = jsNative

/// <summary>
/// Runs a decoder against JavaScript.
/// </summary>
type Decode =

    /// <summary>
    /// Run a decoder against a <c>obj</c> which is already parsed.
    /// </summary>
    static member fromValue(decoder: Decoder<'T>) =
        Decode.Advanced.fromValue Decode.helpers decoder

    /// <summary>
    /// Run the decoder half of a codec against a <c>obj</c> which is already parsed.
    /// </summary>
    static member fromValue(codec: Codec<'T>) =
        codec |> Decode.codec |> Decode.fromValue

    /// <summary>
    /// Parse a JSON string and run the decoder against it.
    /// </summary>
    /// <returns>
    /// <c>Ok</c> with the decoded value, or <c>Error</c> with the formatted message.
    /// </returns>
    static member fromString(decoder: Decoder<'T>) =
        fun value ->
            try
                let json = JS.JSON.parse value

                match decoder.Decode(Decode.helpers, json) with
                | Ok success -> Ok success
                | Error error ->
                    let finalError = error |> Decode.Helpers.prependPath "$"
                    Error(Decode.errorToString Decode.helpers finalError)

            with ex when Decode.Interop.isSyntaxError ex ->
                Error("Given an invalid JSON: " + ex.Message)

    /// <summary>
    /// Parse a JSON string and run the decoder half of a codec against it.
    /// </summary>
    static member fromString(codec: Codec<'T>) =
        codec |> Decode.codec |> Decode.fromString

    /// <summary>
    /// Parse a JSON string and run the decoder, raising an exception carrying the message on
    /// failure.
    /// </summary>
    static member unsafeFromString(decoder: Decoder<'T>) =
        fun value ->
            match Decode.fromString decoder value with
            | Ok x -> x
            | Error msg -> failwith msg

    /// <summary>
    /// Parse a JSON string and run the decoder half of a codec, raising an exception carrying the
    /// message on failure.
    /// </summary>
    static member unsafeFromString(codec: Codec<'T>) =
        codec |> Decode.codec |> Decode.fromString
