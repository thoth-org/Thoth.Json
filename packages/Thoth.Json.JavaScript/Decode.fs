namespace Thoth.Json.JavaScript

open Fable.Core
open Fable.Core.JsInterop
open Thoth.Json.Core

/// <summary>Controls how a JSON string is parsed before the decoder runs.</summary>
type DecodeOptions =
    {
        /// <summary>
        /// Keep the text of number literals, so <c>10.0</c> is not read as an integer and a large
        /// integer keeps its digits. Costs a <c>JSON.parse</c> reviver call per value.
        /// </summary>
        ExactNumbers: bool
    }

[<RequireQualifiedAccess>]
module DecodeOptions =

    /// <summary>The options <c>Decode.fromString</c> uses.</summary>
    let defaults =
        {
            ExactNumbers = false
        }

[<RequireQualifiedAccess>]
module Decode =

    // Keyed by the boxed number so that nothing is written onto Number itself, which keeps the
    // emitted code valid under TypeScript.
    let private numberSources: obj = emitJsExpr () "new WeakMap()"

    /// <summary>
    /// Reads a <c>obj</c>, so a decoder written against Thoth.Json.Core runs on JavaScript.
    /// </summary>
    let helpers =
        { new IDecoderHelpers<obj> with
            member _.isString jsonValue = jsonValue :? string

            member _.isNumber jsonValue =
                emitJsStatement
                    jsonValue
                    """
return typeof $0 === "number" || $0 instanceof Number
                    """

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
                    (numberSources, jsonValue)
                    """
const source = $0.get($1);
return typeof source === "string"
    ? source.indexOf(".") === -1
    : (isFinite($1) && Math.floor($1) === $1)
                    """

            member _.asString jsonValue = unbox jsonValue
            member _.asBoolean jsonValue = unbox jsonValue
            member _.asArray jsonValue = unbox jsonValue
            member _.asFloat jsonValue = emitJsExpr jsonValue "Number($0)"

            member _.asFloat32 jsonValue = emitJsExpr jsonValue "Number($0)"

            member _.asInt jsonValue = emitJsExpr jsonValue "Number($0)"

            member _.getProperties jsonValue =
                upcast JS.Constructors.Object.keys (jsonValue)

            member _.getProperty(fieldName: string, jsonValue: obj) =
                jsonValue?(fieldName)

            member _.anyToString jsonValue =
                emitJsStatement
                    (numberSources, jsonValue)
                    """
const source = $0.get($1);
return typeof source === "string"
    ? source
    : JSON.stringify($1, null, 4) + ''
                    """

            member _.numberToString jsonValue =
                emitJsStatement
                    (numberSources, jsonValue)
                    """
const source = $0.get($1);
return typeof source === "string" ? source : String($1)
                    """
        }

    let private numberLiteralReviverFunc =
        System.Func<string, obj, obj, obj>(fun _key value context ->
            emitJsExpr
                (numberSources, value, context)
                """
($2 && typeof $1 === "number" && $2.source !== String($1))
    ? (function () {
        const boxed = new Number($1);
        $0.set(boxed, $2.source);
        return boxed;
      })()
    : $1
                """
        )

    /// <summary>
    /// The <c>JSON.parse</c> reviver behind <c>DecodeOptions.ExactNumbers</c>. Pass it to your own
    /// <c>JSON.parse</c> call to decode the result with <c>Decode.fromValue</c> and keep the same
    /// behaviour.
    /// </summary>
    /// <remarks>
    /// Typed as <c>obj</c> because TypeScript's own <c>JSON.parse</c> declaration still takes a
    /// two argument reviver, so a typed value would not compile against it.
    /// </remarks>
    let numberLiteralReviver: obj = box numberLiteralReviverFunc

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
    /// Parse a JSON string with the given options and run the decoder against it.
    /// </summary>
    /// <returns>
    /// <c>Ok</c> with the decoded value, or <c>Error</c> with the formatted message.
    /// </returns>
    /// <example>
    /// <code lang="fsharp">
    /// let options = { DecodeOptions.defaults with ExactNumbers = true }
    ///
    /// json |> Decode.fromStringWithOptions(options, decoder)
    /// </code>
    /// </example>
    static member fromStringWithOptions
        (options: DecodeOptions, decoder: Decoder<'T>)
        =
        fun value ->
            try
                let json =
                    if options.ExactNumbers then
                        emitJsExpr
                            (value, Decode.numberLiteralReviver)
                            "JSON.parse($0, $1)"
                    else
                        JS.JSON.parse value

                match decoder.Decode(Decode.helpers, json) with
                | Ok success -> Ok success
                | Error error ->
                    let finalError = error |> Decode.Helpers.prependPath "$"
                    Error(Decode.errorToString Decode.helpers finalError)

            with ex when Decode.Interop.isSyntaxError ex ->
                Error("Given an invalid JSON: " + ex.Message)

    /// <summary>
    /// Parse a JSON string with the given options and run the decoder half of a codec against it.
    /// </summary>
    static member fromStringWithOptions
        (options: DecodeOptions, codec: Codec<'T>)
        =
        Decode.fromStringWithOptions (options, Decode.codec codec)

    /// <summary>
    /// Parse a JSON string and run the decoder against it.
    /// </summary>
    /// <returns>
    /// <c>Ok</c> with the decoded value, or <c>Error</c> with the formatted message.
    /// </returns>
    static member fromString(decoder: Decoder<'T>) =
        Decode.fromStringWithOptions (DecodeOptions.defaults, decoder)

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
        codec |> Decode.codec |> Decode.unsafeFromString

    /// <summary>
    /// Parse a JSON string with the given options and run the decoder, raising an exception
    /// carrying the message on failure.
    /// </summary>
    static member unsafeFromStringWithOptions
        (options: DecodeOptions, decoder: Decoder<'T>)
        =
        fun value ->
            match Decode.fromStringWithOptions (options, decoder) value with
            | Ok x -> x
            | Error msg -> failwith msg

    /// <summary>
    /// Parse a JSON string with the given options and run the decoder half of a codec, raising an
    /// exception carrying the message on failure.
    /// </summary>
    static member unsafeFromStringWithOptions
        (options: DecodeOptions, codec: Codec<'T>)
        =
        Decode.unsafeFromStringWithOptions (options, Decode.codec codec)
