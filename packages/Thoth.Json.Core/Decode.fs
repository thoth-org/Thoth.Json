namespace Thoth.Json.Core

open System
open System.Globalization

[<RequireQualifiedAccess>]
module Decode =

    module Helpers =

        /// <summary>Put a path segment in front of a failure's own path.</summary>
        let prependPath
            (path: string)
            (err: DecoderError<'JsonValue>)
            : DecoderError<'JsonValue>
            =
            let (oldPath, reason) = err
            (path + oldPath, reason)

        /// <summary>Put a path segment in front of a failed result's path.</summary>
        let inline prependPathToResult<'T, 'JsonValue>
            (path: string)
            (res: Result<'T, DecoderError<'JsonValue>>)
            =
            res |> Result.mapError (prependPath path)

    let private genericMsg
        (helpers: IDecoderHelpers<'JsonValue>)
        msg
        value
        newLine
        =
        try
            "Expecting "
            + msg
            + " but instead got:"
            + (if newLine then
                   "\n"
               else
                   " ")
            + (helpers.anyToString value)
        with _ ->
            "Expecting "
            + msg
            + " but decoder failed. Couldn't report given value due to circular structure."
            + (if newLine then
                   "\n"
               else
                   " ")

    /// <summary>Format a decoding failure as the message the entry points return.</summary>
    let rec errorToString
        (helpers: IDecoderHelpers<'JsonValue>)
        (path: string, error)
        =
        let reason =
            match error with
            | BadPrimitive(msg, value) -> genericMsg helpers msg value false
            | BadType(msg, value) -> genericMsg helpers msg value true
            | BadPrimitiveExtra(msg, value, reason) ->
                genericMsg helpers msg value false + "\nReason: " + reason
            | BadField(msg, value) -> genericMsg helpers msg value true
            | BadPath(msg, value, fieldName) ->
                genericMsg helpers msg value true
                + ("\nNode `" + fieldName + "` is unknown.")
            | TooSmallArray(msg, value) ->
                "Expecting " + msg + ".\n" + (helpers.anyToString value)
            | BadOneOf(errors) ->
                let messages =
                    errors
                    |> List.map (fun error ->
                        Helpers.prependPath path error |> errorToString helpers
                    )

                "The following errors were found:\n\n"
                + String.concat "\n\n" messages
            | FailMessage msg ->
                "The following `failure` occurred with the decoder: " + msg

        match error with
        | BadOneOf _ ->
            // Don't need to show the path here because each error case will show it's own path
            reason
        | _ -> "Error at: `" + path + "`\n" + reason

    // Low-level API
    module Advanced =

        /// <summary>
        /// Run a decoder against a JSON value of the runtime's own type, giving the formatted
        /// message on failure.
        /// </summary>
        /// <remarks>
        /// This is what a runtime package builds its <c>Decode.fromValue</c> on.
        /// </remarks>
        let fromValue
            (helpers: IDecoderHelpers<'JsonValue>)
            (decoder: Decoder<'T>)
            =
            fun value ->
                match decoder.Decode(helpers, value) with
                | Ok success -> Ok success
                | Error error -> Error(errorToString helpers error)

    /// <summary>
    /// Decode a JSON string into an F# string.
    /// </summary>
    ///
    /// <example>
    /// <code lang="fsharp">
    /// Decode.fromString Decode.string "Hello" == Ok "Hello"
    /// Decode.fromString Decode.string "42" == Error ...
    /// Decode.fromString Decode.string "true" == Error ...
    /// Decode.fromString Decode.string "{ \"foo\": 42 }" == Error ...
    /// </code>
    /// </example>
    let string: Decoder<string> =
        { new Decoder<string> with
            member _.Decode(helpers, value) =
                if helpers.isString value then
                    Ok(helpers.asString value)
                else
                    Error("", BadPrimitive("a string", value))
        }

    /// <summary>
    /// Decode a JSON string into an F# char.
    /// </summary>
    ///
    /// <example>
    /// <code lang="fsharp">
    /// Decode.fromString Decode.char "a" == Ok 'a'
    /// Decode.fromString Decode.char "ab" == Error ...
    /// Decode.fromString Decode.char "true" == Error ...
    /// Decode.fromString Decode.char "42" == Error ...
    /// </code>
    /// </example>
    let char: Decoder<char> =
        { new Decoder<char> with
            member _.Decode(helpers, value) =
                if helpers.isString value then
                    let str = helpers.asString value

                    if str.Length = 1 then
                        Ok(str.[0])
                    else
                        ("", BadPrimitive("a single character string", value))
                        |> Error
                else
                    ("", BadPrimitive("a char", value)) |> Error
        }

    /// <summary>
    /// Decode a JSON string into an F# Guid.
    /// </summary>
    ///
    /// <example>
    /// <code lang="fsharp">
    /// Decode.fromString Decode.guid "58bd4436-7583-40e2-bd3c-aa3c5d0b4286" == Ok (System.Guid "58bd4436-7583-40e2-bd3c-aa3c5d0b4286")
    /// Decode.fromString Decode.guid "58bd4436-aa3c5d0b4286" == Error ...
    /// </code>
    /// </example>
    let guid: Decoder<System.Guid> =
        { new Decoder<System.Guid> with
            member _.Decode(helpers, value) =
                if helpers.isString value then
                    match System.Guid.TryParse(helpers.asString value) with
                    | true, x -> Ok x
                    | _ -> ("", BadPrimitive("a guid", value)) |> Error
                else
                    ("", BadPrimitive("a guid", value)) |> Error
        }

    /// <summary>
    /// Decode a JSON string into a System.Uri.
    /// </summary>
    ///
    /// <remarks>
    /// The string is parsed with <c>UriKind.RelativeOrAbsolute</c>, so both
    /// absolute (e.g. <c>https://example.com</c>) and relative (e.g.
    /// <c>/path?query=1</c>) URIs are accepted.
    /// </remarks>
    ///
    /// <example>
    /// <code lang="fsharp">
    /// Decode.fromString Decode.uri "\"https://example.com\"" == Ok (System.Uri "https://example.com")
    /// </code>
    /// </example>
    let uri: Decoder<System.Uri> =
        { new Decoder<System.Uri> with
            member _.Decode(helpers, value) =
                if helpers.isString value then
                    match
                        System.Uri.TryCreate(
                            helpers.asString value,
                            System.UriKind.RelativeOrAbsolute
                        )
                    with
                    | true, uri -> Ok uri
                    | _ -> ("", BadPrimitive("a URI", value)) |> Error
                else
                    ("", BadPrimitive("a URI", value)) |> Error
        }

    /// <summary>
    /// Decode a JSON null value into an F# unit.
    /// </summary>
    ///
    /// <example>
    /// <code lang="fsharp">
    /// Decode.fromString Decode.unit "null" == Ok ()
    /// Decode.fromString Decode.unit "42" == Error ...
    /// </code>
    /// </example>
    let unit: Decoder<unit> =
        { new Decoder<unit> with
            member _.Decode(helpers, value) =
                if helpers.isNullValue value then
                    Ok()
                else
                    ("", BadPrimitive("null", value)) |> Error
        }

    let inline private integral
        (name: string)
        (tryParse: (string -> bool * 'T))
        (min: unit -> 'T)
        (max: unit -> 'T)
        (conv: float -> 'T)
        : Decoder<'T>
        =

        { new Decoder<'T> with
            member _.Decode(helpers, value) =
                if helpers.isNumber value then
                    if helpers.isIntegralValue value then
                        let floatValue: float = helpers.asFloat value

                        if
                            (float (min ())) <= floatValue
                            && floatValue <= (float (max ()))
                        then
                            Ok(conv floatValue)
                        else
                            ("",
                             BadPrimitiveExtra(
                                 name,
                                 value,
                                 "Value was either too large or too small for "
                                 + name
                             ))
                            |> Error
                    else
                        ("",
                         BadPrimitiveExtra(
                             name,
                             value,
                             "Value is not an integral value"
                         ))
                        |> Error
                elif helpers.isString value then
                    match tryParse (helpers.asString value) with
                    | true, x -> Ok x
                    | _ -> ("", BadPrimitive(name, value)) |> Error
                else
                    ("", BadPrimitive(name, value)) |> Error
        }

    let inline private bigIntegral
        (name: string)
        (tryParse: string -> bool * 'T)
        : Decoder<'T>
        =
        { new Decoder<'T> with
            member _.Decode(helpers, value) =
                if helpers.isNumber value then
                    let rawText = helpers.numberToString value

                    match tryParse rawText with
                    | true, x -> Ok x
                    | _ -> ("", BadPrimitive(name, value)) |> Error
                elif helpers.isString value then
                    match tryParse (helpers.asString value) with
                    | true, x -> Ok x
                    | _ -> ("", BadPrimitive(name, value)) |> Error
                else
                    ("", BadPrimitive(name, value)) |> Error
        }

    /// <summary>Decode a JSON number or string into an sbyte.</summary>
    let sbyte: Decoder<sbyte> =
        integral
            "a sbyte"
            System.SByte.TryParse
            (fun () -> System.SByte.MinValue)
            (fun () -> System.SByte.MaxValue)
            sbyte

    /// Alias to Decode.uint8
    let byte: Decoder<byte> =
        integral
            "a byte"
            System.Byte.TryParse
            (fun () -> System.Byte.MinValue)
            (fun () -> System.Byte.MaxValue)
            byte

    /// <summary>Decode a JSON number or string into an int16.</summary>
    let int16: Decoder<int16> =
        integral
            "an int16"
            System.Int16.TryParse
            (fun () -> System.Int16.MinValue)
            (fun () -> System.Int16.MaxValue)
            int16

    /// <summary>Decode a JSON number or string into a uint16.</summary>
    let uint16: Decoder<uint16> =
        integral
            "an uint16"
            System.UInt16.TryParse
            (fun () -> System.UInt16.MinValue)
            (fun () -> System.UInt16.MaxValue)
            uint16

    /// <summary>Decode a JSON number or string into an int.</summary>
    let int: Decoder<int> =
        integral
            "an int"
            System.Int32.TryParse
            (fun () -> System.Int32.MinValue)
            (fun () -> System.Int32.MaxValue)
            int

    /// <summary>Decode a JSON number or string into a uint32.</summary>
    let uint32: Decoder<uint32> =
        integral
            "an uint32"
            System.UInt32.TryParse
            (fun () -> System.UInt32.MinValue)
            (fun () -> System.UInt32.MaxValue)
            uint32

    /// <summary>Decode a JSON string or number into an int64.</summary>
    let int64: Decoder<int64> =
        let tryParse (text: string) =
#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_PYTHON
            System.Int64.TryParse(text)
#else
            System.Int64.TryParse(
                text,
                NumberStyles.Integer ||| NumberStyles.AllowExponent,
                CultureInfo.InvariantCulture
            )
#endif

        bigIntegral "an int64" tryParse

    /// <summary>Decode a JSON string or number into a uint64.</summary>
    let uint64: Decoder<uint64> =
        let tryParse (text: string) =
#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_PYTHON
            System.UInt64.TryParse(text)
#else
            System.UInt64.TryParse(
                text,
                NumberStyles.Integer ||| NumberStyles.AllowExponent,
                CultureInfo.InvariantCulture
            )
#endif

        bigIntegral "an uint64" tryParse

    /// <summary>Decode a JSON string or number into a bigint.</summary>
    let bigint: Decoder<bigint> =
        { new Decoder<bigint> with
            member _.Decode(helpers, value) =
                if helpers.isNumber value then
                    helpers.asInt value |> bigint |> Ok
                elif helpers.isString value then
                    let parseResult =
#if FABLE_COMPILER
                        bigint.TryParse(helpers.asString value)
#else
                        bigint.TryParse(
                            helpers.asString value,
                            NumberStyles.Any,
                            CultureInfo.InvariantCulture
                        )
#endif

                    match parseResult with
                    | true, x -> Ok x
                    | _ -> ("", BadPrimitive("a bigint", value)) |> Error
                else
                    ("", BadPrimitive("a bigint", value)) |> Error
        }

    /// <summary>Decode a JSON boolean.</summary>
    let bool: Decoder<bool> =
        { new Decoder<bool> with
            member _.Decode(helpers, value) =
                if helpers.isBoolean value then
                    Ok(helpers.asBoolean value)
                else
                    ("", BadPrimitive("a boolean", value)) |> Error
        }

    /// <summary>Decode a JSON number into a float.</summary>
    let float: Decoder<float> =
        { new Decoder<float> with
            member _.Decode(helpers, value) =
                let inline decodeString (rawString: string) =
                    match rawString with
                    // Double.TryParse does not read all three of these on every runtime.
                    | "NaN" -> Ok Double.NaN
                    | "Infinity" -> Ok Double.PositiveInfinity
                    | "-Infinity" -> Ok Double.NegativeInfinity
                    | _ ->
                        match
#if FABLE_COMPILER_PYTHON || FABLE_COMPILER_JAVASCRIPT
                            Double.TryParse(rawString)
#else
                            Double.TryParse(
                                rawString,
                                NumberStyles.Float,
                                CultureInfo.InvariantCulture
                            )
#endif
                        with
                        | true, f -> Ok f
                        | false, _ ->
                            ("", BadPrimitive("a float", value)) |> Error

                if helpers.isNumber value then
                    decodeString (helpers.anyToString value)
                elif helpers.isString value then
                    decodeString (helpers.asString value)
                else
                    ("", BadPrimitive("a float", value)) |> Error
        }


    /// <summary>Decode a JSON number into a float32.</summary>
    let float32: Decoder<float32> =
        { new Decoder<float32> with
            member _.Decode(helpers, value) =
                if helpers.isNumber value then
                    Ok(helpers.asFloat32 value)
                elif helpers.isString value then
                    match helpers.asString value with
                    | "NaN" -> Ok System.Single.NaN
                    | "Infinity" -> Ok System.Single.PositiveInfinity
                    | "-Infinity" -> Ok System.Single.NegativeInfinity
                    | _ -> ("", BadPrimitive("a float32", value)) |> Error
                else
                    ("", BadPrimitive("a float32", value)) |> Error
        }

    /// <summary>Decode a JSON string or number into a decimal.</summary>
    let decimal: Decoder<decimal> =
        { new Decoder<decimal> with
            member _.Decode(helpers, value) =
                if helpers.isNumber value then
                    helpers.asFloat value |> decimal |> Ok
                elif helpers.isString value then
                    match
                        System.Decimal.TryParse(
                            helpers.asString value,
                            NumberStyles.Number,
                            CultureInfo.InvariantCulture
                        )
                    with
                    | true, x -> Ok x
                    | _ -> ("", BadPrimitive("a decimal", value)) |> Error
                else
                    ("", BadPrimitive("a decimal", value)) |> Error
        }

#if !FABLE_COMPILER_PYTHON
    /// Decode a System.DateTime value using Sytem.DateTime.TryParse, then convert it to UTC.
    let datetimeUtc: Decoder<System.DateTime> =
        { new Decoder<System.DateTime> with
            member _.Decode(helpers, value) =
                if helpers.isString value then
                    match System.DateTime.TryParse(helpers.asString value) with
                    | true, datetime -> datetime.ToUniversalTime() |> Ok
                    | _ -> ("", BadPrimitive("a datetime", value)) |> Error
                else
                    ("", BadPrimitive("a datetime", value)) |> Error
        }
#endif

    /// Decode a System.DateTime with DateTime.TryParse; uses default System.DateTimeStyles.
    let datetimeLocal: Decoder<System.DateTime> =
        { new Decoder<System.DateTime> with
            member _.Decode(helpers, value) =
                if helpers.isString value then
                    match System.DateTime.TryParse(helpers.asString value) with
                    | true, datetime -> Ok datetime
                    | _ -> ("", BadPrimitive("a datetime", value)) |> Error
                else
                    ("", BadPrimitive("a datetime", value)) |> Error
        }

#if !FABLE_COMPILER_PYTHON
    /// <summary>Decode a JSON string into a DateTimeOffset.</summary>
    let datetimeOffset: Decoder<System.DateTimeOffset> =
        { new Decoder<System.DateTimeOffset> with
            member _.Decode(helpers, value) =
                if helpers.isString value then
                    match
                        System.DateTimeOffset.TryParse(helpers.asString value)
                    with
                    | true, datetimeOffset -> Ok datetimeOffset
                    | _ ->
                        ("", BadPrimitive("a datetimeoffset", value)) |> Error
                else
                    ("", BadPrimitive("a datetime", value)) |> Error
        }
#endif

    /// <summary>Decode a JSON string into a TimeSpan.</summary>
    let timespan: Decoder<System.TimeSpan> =
        { new Decoder<System.TimeSpan> with
            member _.Decode(helpers, value) =
                if helpers.isString value then
                    match System.TimeSpan.TryParse(helpers.asString value) with
                    | true, timespan -> Ok timespan
                    | _ -> ("", BadPrimitive("a timespan", value)) |> Error
                else
                    ("", BadPrimitive("a timespan", value)) |> Error
        }

    /////////////////////////
    // Object primitives ///
    ///////////////////////

    let private decodeMaybeNull
        (helpers: IDecoderHelpers<'JsonValue>)
        (path: string)
        (decoder: Decoder<'value>)
        (value: 'JsonValue)
        =
        // The decoder may be an option decoder so give it an opportunity to check null values

        // We catch the null value case first to avoid executing the decoder logic
        // Indeed, if the decoder logic try to access the value to do something with it,
        // it can throw an exception about the value being null
        if helpers.isNullValue value then
            Ok None
        else
            match decoder.Decode(helpers, value) with
            | Ok v -> Ok(Some v)
            | Error er -> er |> Helpers.prependPath path |> Error

    /// <summary>
    /// Decode the value under a property, giving <c>None</c> when the property is missing or
    /// <c>null</c>.
    /// </summary>
    /// <remarks>
    /// Fails when the property is there but the decoder rejects its value.
    /// </remarks>
    let optional
        (fieldName: string)
        (decoder: Decoder<'value>)
        : Decoder<'value option>
        =
        { new Decoder<'value option> with
            member _.Decode(helpers, value) =
                if helpers.isObject value then

                    if helpers.hasProperty fieldName value then
                        let fieldValue = helpers.getProperty (fieldName, value)

                        decodeMaybeNull
                            helpers
                            ("." + fieldName)
                            decoder
                            fieldValue
                    else
                        Ok None
                else
                    Error("", BadType("an object", value))
        }

    let private badPathError fieldNames currentPath value =
        let currentPath =
            defaultArg currentPath (fieldNames |> String.concat ".")

        let msg = "an object with path `" + (String.concat "." fieldNames) + "`"

        Error(
            "." + currentPath,
            BadPath(
                msg,
                value,
                List.tryLast fieldNames |> Option.defaultValue ""
            )
        )

    /// <summary>Decode with two decoders and combine their results.</summary>
    let map2
        (ctor: 'a -> 'b -> 'Output)
        (d1: Decoder<'a>)
        (d2: Decoder<'b>)
        : Decoder<'Output>
        =
        { new Decoder<'Output> with
            member _.Decode(helpers, value) =
                match d1.Decode(helpers, value), d2.Decode(helpers, value) with
                | Ok v1, Ok v2 -> Ok(ctor v1 v2)
                | Error er, _ -> Error er
                | _, Error er -> Error er
        }

    // let custom =
    //     map2 (|>)

    /// <summary>
    /// Decode the value at a path of property names, giving <c>None</c> when the path is missing or
    /// holds <c>null</c>.
    /// </summary>
    let optionalAt
        (fieldNames: string list)
        (decoder: Decoder<'value>)
        : Decoder<'value option>
        =
        { new Decoder<'value option> with
            member _.Decode(helpers, firstValue) =
                (("", firstValue, None), fieldNames)
                ||> List.fold (fun (curPath, curValue, res) field ->
                    match res with
                    | Some _ -> curPath, curValue, res
                    | None ->
                        if helpers.isNullValue curValue then
                            curPath, curValue, Some(Ok None)
                        elif helpers.isObject curValue then
                            if helpers.hasProperty field curValue then
                                let curValue =
                                    helpers.getProperty (field, curValue)

                                curPath + "." + field, curValue, None
                            else
                                curPath, curValue, Some(Ok None)
                        else
                            let res =
                                Error(curPath, BadType("an object", curValue))

                            curPath, curValue, Some res
                )
                |> function
                    | _, _, Some res -> res
                    | lastPath, lastValue, None ->
                        if helpers.isNullValue lastValue then
                            Ok None
                        else
                            decodeMaybeNull helpers lastPath decoder lastValue
        }

    /// <summary>Decode the value under a property.</summary>
    let field (fieldName: string) (decoder: Decoder<'value>) : Decoder<'value> =
        { new Decoder<'value> with
            member _.Decode(helpers, value) =
                if helpers.isObject value then
                    if helpers.hasProperty fieldName value then
                        let fieldValue = helpers.getProperty (fieldName, value)

                        decoder.Decode(helpers, fieldValue)
                        |> Helpers.prependPathToResult ("." + fieldName)
                    else
                        Error(
                            "",
                            BadField(
                                "an object with a field named `"
                                + fieldName
                                + "`",
                                value
                            )
                        )
                else
                    Error("", BadType("an object", value))
        }

    /// <summary>Decode the value at a path of property names.</summary>
    let at
        (fieldNames: string list)
        (decoder: Decoder<'value>)
        : Decoder<'value>
        =
        { new Decoder<'value> with
            member _.Decode(helpers, firstValue) =
                (("", firstValue, None), fieldNames)
                ||> List.fold (fun (curPath, curValue, res) field ->
                    match res with
                    | Some _ -> curPath, curValue, res
                    | None ->
                        if helpers.isNullValue curValue then
                            let res =
                                badPathError
                                    fieldNames
                                    (Some curPath)
                                    firstValue

                            curPath, curValue, Some res
                        elif helpers.isObject curValue then
                            if helpers.hasProperty field curValue then
                                let curValue =
                                    helpers.getProperty (field, curValue)

                                curPath + "." + field, curValue, None
                            else
                                let res =
                                    badPathError fieldNames None firstValue

                                curPath, curValue, Some res
                        else
                            let res =
                                Error(curPath, BadType("an object", curValue))

                            curPath, curValue, Some res
                )
                |> function
                    | _, _, Some res -> res
                    | lastPath, lastValue, None ->
                        decoder.Decode(helpers, lastValue)
                        |> Helpers.prependPathToResult lastPath
        }

    /// <summary>Decode the element of an array at the given position.</summary>
    let index
        (requestedIndex: int)
        (decoder: Decoder<'value>)
        : Decoder<'value>
        =
        { new Decoder<'value> with
            member _.Decode(helpers, value) =
                if helpers.isArray value then
                    let vArray = helpers.asArray value

                    let path = ".[" + (Operators.string requestedIndex) + "]"

                    if requestedIndex < vArray.Length then
                        decoder.Decode(helpers, vArray.[requestedIndex])
                        |> Helpers.prependPathToResult path
                    else
                        let msg =
                            "a longer array. Need index `"
                            + (Operators.string requestedIndex)
                            + "` but there are only `"
                            + (Operators.string vArray.Length)
                            + "` entries"

                        (path, TooSmallArray(msg, value)) |> Error
                else
                    ("", BadPrimitive("an array", value)) |> Error
        }

    //////////////////////
    // Data structure ///
    ////////////////////

    /// <summary>Decode a JSON array into a list.</summary>
    let list (decoder: Decoder<'value>) : Decoder<'value list> =
        { new Decoder<'value list> with
            member _.Decode(helpers, value) =
                if helpers.isArray value then
                    let tokens = helpers.asArray value
                    let mutable i = 0
                    let mutable result = []
                    let mutable error: DecoderError<_> option = None

                    while i < tokens.Length && error.IsNone do
                        let value = tokens.[i]

                        match decoder.Decode(helpers, value) with
                        | Ok value -> result <- result @ [ value ]
                        | Error er ->
                            let x =
                                Some(
                                    er
                                    |> Helpers.prependPath (
                                        ".[" + (i.ToString()) + "]"
                                    )
                                )

                            error <- x

                        i <- i + 1

                    if error.IsNone then
                        Ok result
                    else
                        Error error.Value
                else
                    ("", BadPrimitive("a list", value)) |> Error
        }

    /// <summary>Decode a JSON array into a ResizeArray.</summary>
    let resizeArray (decoder: Decoder<'value>) : Decoder<'value ResizeArray> =
        { new Decoder<'value ResizeArray> with
            member _.Decode(helpers, value) =
                if helpers.isArray value then
                    let tokens = helpers.asArray value
                    let mutable i = 0
                    let result = ResizeArray tokens.Length
                    let mutable error: DecoderError<_> option = None

                    while i < tokens.Length && error.IsNone do
                        let value = tokens.[i]

                        match decoder.Decode(helpers, value) with
                        | Ok value ->
                            // Setting the value via the index fails with a runtime error
                            // but because we iterate over the tokens in order, adding the value
                            // should keep the order
                            result.Add value
                        | Error er ->
                            error <-
                                Some(
                                    er
                                    |> Helpers.prependPath (
                                        ".[" + (i.ToString()) + "]"
                                    )
                                )

                        i <- i + 1

                    if error.IsNone then
                        Ok(ResizeArray result)
                    else
                        Error error.Value
                else
                    ("", BadPrimitive("a ResizeArray", value)) |> Error
        }

    /// <summary>Decode a JSON array into a sequence.</summary>
    let seq (decoder: Decoder<'value>) : Decoder<'value seq> =
        { new Decoder<'value seq> with
            member _.Decode(helpers, value) =
                if helpers.isArray value then
                    let mutable i = -1
                    let tokens = helpers.asArray value

                    (Ok(seq []), tokens)
                    ||> Array.fold (fun acc value ->
                        i <- i + 1

                        match acc with
                        | Error _ -> acc
                        | Ok acc ->
                            match decoder.Decode(helpers, value) with
                            | Error er ->
                                Error(
                                    er
                                    |> Helpers.prependPath (
                                        ".[" + (i.ToString()) + "]"
                                    )
                                )
                            | Ok value -> Ok(Seq.append [ value ] acc)
                    )
                    |> Result.map Seq.rev
                else
                    ("", BadPrimitive("a seq", value)) |> Error
        }

    /// <summary>Decode a JSON array into an array.</summary>
    let array (decoder: Decoder<'value>) : Decoder<'value array> =
        { new Decoder<'value array> with
            member _.Decode(helpers, value) =
                if helpers.isArray value then
                    let mutable i = -1
                    let tokens = helpers.asArray value
                    let arr = Array.zeroCreate tokens.Length

                    (Ok arr, tokens)
                    ||> Array.fold (fun acc value ->
                        i <- i + 1

                        match acc with
                        | Error _ -> acc
                        | Ok acc ->
                            match decoder.Decode(helpers, value) with
                            | Error er ->
                                Error(
                                    er
                                    |> Helpers.prependPath (
                                        ".[" + (i.ToString()) + "]"
                                    )
                                )
                            | Ok value ->
                                acc.[i] <- value
                                Ok acc
                    )
                else
                    ("", BadPrimitive("an array", value)) |> Error
        }


    /// <summary>The property names of an object.</summary>
    let keys: Decoder<string list> =
        { new Decoder<string list> with
            member _.Decode(helpers, value) =
                if helpers.isObject value then
                    helpers.getProperties value |> List.ofSeq |> Ok
                else
                    ("", BadPrimitive("an object", value)) |> Error
        }


    /// <summary>Decode every property of an object, giving its name paired with its decoded value.</summary>
    let keyValuePairs
        (decoder: Decoder<'value>)
        : Decoder<(string * 'value) list>
        =
        { new Decoder<(string * 'value) list> with
            member _.Decode(helpers, value) =
                match keys.Decode(helpers, value) with
                | Ok objectKeys ->
                    (Ok [], objectKeys)
                    ||> List.fold (fun acc prop ->
                        match acc with
                        | Error _ -> acc
                        | Ok acc ->
                            let fieldValue = helpers.getProperty (prop, value)

                            match decoder.Decode(helpers, fieldValue) with
                            | Error er ->
                                Error(Helpers.prependPath ("." + prop) er)
                            | Ok value -> (prop, value) :: acc |> Ok
                    )
                    |> Result.map List.rev
                | Error e -> Error e
        }

    //////////////////////////////
    // Inconsistent Structure ///
    ////////////////////////////

    /// <summary>
    /// Decode with the first decoder of the list that succeeds.
    /// </summary>
    /// <remarks>
    /// The decoders are tried in order. If all of them fail, the failure reports every one.
    /// </remarks>
    let oneOf (decoders: Decoder<'value> list) : Decoder<'value> =
        { new Decoder<'value> with
            member _.Decode(helpers, value) =
                let rec runner
                    (decoders: Decoder<'value> list)
                    (errors: DecoderError<'JsonValue> list)
                    =
                    match decoders with
                    | head :: tail ->
                        match head.Decode(helpers, value) with
                        | Ok v -> Ok v
                        | Error error ->
                            runner tail (List.append errors [ error ])
                    | [] -> ("", BadOneOf errors) |> Error

                runner decoders []
        }

    //////////////////////
    // Fancy decoding ///
    ////////////////////

    /// <summary>Decode <c>null</c> into the given value, and fail on anything else.</summary>
    let nil (output: 'a) : Decoder<'a> =
        { new Decoder<'a> with
            member _.Decode(helpers, value) =
                if helpers.isNullValue value then
                    Ok output
                else
                    ("", BadPrimitive("null", value)) |> Error
        }

    type private ValueDecoder() =
        interface Decoder<Json> with
            member this.Decode
                (helpers: IDecoderHelpers<'JsonValue>, value: 'JsonValue)
                : Result<Json, DecoderError<'JsonValue>>
                =
                let decoder = this :> Decoder<_>

                if helpers.isBoolean value then
                    helpers.asBoolean value |> Json.Boolean |> Ok
                elif helpers.isNullValue value then
                    Json.Null |> Ok
                elif helpers.isString value then
                    helpers.asString value |> Json.String |> Ok
                elif helpers.isNumber value then
                    helpers.asFloat value |> Json.Number |> Ok
                elif helpers.isArray value then
                    let tokens = helpers.asArray value
                    let result = Array.zeroCreate (Array.length tokens)

                    let mutable i = 0
                    let mutable error: DecoderError<_> option = None

                    while i < tokens.Length && error.IsNone do
                        let value = tokens.[i]

                        match decoder.Decode(helpers, value) with
                        | Ok value -> result[i] <- value
                        | Error er ->
                            let x = Some(er |> Helpers.prependPath $".[%i{i}]")

                            error <- x

                        i <- i + 1

                    if error.IsNone then
                        result |> Seq.toList |> Json.Array |> Ok
                    else
                        Error error.Value
                elif helpers.isObject value then
                    let props = helpers.getProperties value |> Seq.toArray
                    let result = Array.zeroCreate (Array.length props)

                    let mutable i = 0
                    let mutable error: DecoderError<_> option = None

                    while i < props.Length && error.IsNone do
                        let key = props[i]
                        let value = helpers.getProperty (key, value)

                        match decoder.Decode(helpers, value) with
                        | Ok value -> result[i] <- key, value
                        | Error er ->
                            let x = Some(er |> Helpers.prependPath ("." + key))

                            error <- x

                        i <- i + 1

                    if error.IsNone then
                        result |> Seq.toList |> Json.Object |> Ok
                    else
                        Error error.Value
                else
                    Error("", BadPrimitive("any", value))

    /// <summary>Decode any JSON into a <see cref="T:Thoth.Json.Core.Json"/> tree.</summary>
    let value: Decoder<Json> = ValueDecoder()

    /// <summary>Always succeed with the given value, whatever the JSON.</summary>
    let succeed (output: 'a) : Decoder<'a> =
        { new Decoder<'a> with
            member _.Decode(_, _) = Ok output
        }

    /// <summary>Always fail with the given message.</summary>
    let fail (msg: string) : Decoder<'a> =
        { new Decoder<'a> with
            member _.Decode(_, _) = Error("", FailMessage msg)
        }

    /// <summary>Use the result of a decoder to choose the next one.</summary>
    let andThen (cb: 'a -> Decoder<'b>) (decoder: Decoder<'a>) : Decoder<'b> =
        { new Decoder<'b> with
            member _.Decode(helpers, value) =
                match decoder.Decode(helpers, value) with
                | Error er -> Error er
                | Ok result -> (cb result).Decode(helpers, value)
        }

    /// <summary>Run every decoder against the same value, and collect their results.</summary>
    let all (decoders: Decoder<'a> list) : Decoder<'a list> =
        { new Decoder<'a list> with
            member _.Decode(helpers, value) =
                let rec runner (decoders: Decoder<'a> list) (values: 'a list) =
                    match decoders with
                    | decoder :: tail ->
                        match decoder.Decode(helpers, value) with
                        | Ok value -> runner tail (List.append values [ value ])
                        | Error error -> Error error
                    | [] -> Ok values

                runner decoders []
        }

    [<NoComparison>]
    type private LazyDecoder<'t>(x: Lazy<Decoder<'t>>) =
        struct
        end

        interface Decoder<'t> with
            member this.Decode<'json>
                (helpers: IDecoderHelpers<'json>, json: 'json)
                =
                let decoder = x.Force()
                decoder.Decode(helpers, json)

    /// <summary>Defer the construction of a decoder until it is first used, for mutually recursive types.</summary>
    let lazily (x: Lazy<Decoder<'t>>) : Decoder<'t> = LazyDecoder(x) :> _

    /////////////////////
    // Map functions ///
    ///////////////////

    /// <summary>Transform the result of a decoder.</summary>
    let map (ctor: 'a -> 'Output) (d1: Decoder<'a>) : Decoder<'Output> =
        { new Decoder<'Output> with
            member _.Decode(helpers, value) =
                match d1.Decode(helpers, value) with
                | Ok v1 -> Ok(ctor v1)
                | Error er -> Error er
        }

    /// <summary>Decode with three decoders and combine their results.</summary>
    let map3
        (ctor: 'a -> 'b -> 'c -> 'Output)
        (d1: Decoder<'a>)
        (d2: Decoder<'b>)
        (d3: Decoder<'c>)
        : Decoder<'Output>
        =
        { new Decoder<'Output> with
            member _.Decode(helpers, value) =
                match
                    d1.Decode(helpers, value),
                    d2.Decode(helpers, value),
                    d3.Decode(helpers, value)
                with
                | Ok v1, Ok v2, Ok v3 -> Ok(ctor v1 v2 v3)
                | Error er, _, _ -> Error er
                | _, Error er, _ -> Error er
                | _, _, Error er -> Error er
        }

    /// <summary>Decode with four decoders and combine their results.</summary>
    let map4
        (ctor: 'a -> 'b -> 'c -> 'd -> 'Output)
        (d1: Decoder<'a>)
        (d2: Decoder<'b>)
        (d3: Decoder<'c>)
        (d4: Decoder<'d>)
        : Decoder<'Output>
        =
        { new Decoder<'Output> with
            member _.Decode(helpers, value) =
                match
                    d1.Decode(helpers, value),
                    d2.Decode(helpers, value),
                    d3.Decode(helpers, value),
                    d4.Decode(helpers, value)
                with
                | Ok v1, Ok v2, Ok v3, Ok v4 -> Ok(ctor v1 v2 v3 v4)
                | Error er, _, _, _ -> Error er
                | _, Error er, _, _ -> Error er
                | _, _, Error er, _ -> Error er
                | _, _, _, Error er -> Error er
        }

    /// <summary>Decode with five decoders and combine their results.</summary>
    let map5
        (ctor: 'a -> 'b -> 'c -> 'd -> 'e -> 'Output)
        (d1: Decoder<'a>)
        (d2: Decoder<'b>)
        (d3: Decoder<'c>)
        (d4: Decoder<'d>)
        (d5: Decoder<'e>)
        : Decoder<'Output>
        =
        { new Decoder<'Output> with
            member _.Decode(helpers, value) =
                match
                    d1.Decode(helpers, value),
                    d2.Decode(helpers, value),
                    d3.Decode(helpers, value),
                    d4.Decode(helpers, value),
                    d5.Decode(helpers, value)
                with
                | Ok v1, Ok v2, Ok v3, Ok v4, Ok v5 -> Ok(ctor v1 v2 v3 v4 v5)
                | Error er, _, _, _, _ -> Error er
                | _, Error er, _, _, _ -> Error er
                | _, _, Error er, _, _ -> Error er
                | _, _, _, Error er, _ -> Error er
                | _, _, _, _, Error er -> Error er
        }

    /// <summary>Decode with six decoders and combine their results.</summary>
    let map6
        (ctor: 'a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'Output)
        (d1: Decoder<'a>)
        (d2: Decoder<'b>)
        (d3: Decoder<'c>)
        (d4: Decoder<'d>)
        (d5: Decoder<'e>)
        (d6: Decoder<'f>)
        : Decoder<'Output>
        =
        { new Decoder<'Output> with
            member _.Decode(helpers, value) =
                match
                    d1.Decode(helpers, value),
                    d2.Decode(helpers, value),
                    d3.Decode(helpers, value),
                    d4.Decode(helpers, value),
                    d5.Decode(helpers, value),
                    d6.Decode(helpers, value)
                with
                | Ok v1, Ok v2, Ok v3, Ok v4, Ok v5, Ok v6 ->
                    Ok(ctor v1 v2 v3 v4 v5 v6)
                | Error er, _, _, _, _, _ -> Error er
                | _, Error er, _, _, _, _ -> Error er
                | _, _, Error er, _, _, _ -> Error er
                | _, _, _, Error er, _, _ -> Error er
                | _, _, _, _, Error er, _ -> Error er
                | _, _, _, _, _, Error er -> Error er
        }

    /// <summary>Decode with seven decoders and combine their results.</summary>
    let map7
        (ctor: 'a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g -> 'Output)
        (d1: Decoder<'a>)
        (d2: Decoder<'b>)
        (d3: Decoder<'c>)
        (d4: Decoder<'d>)
        (d5: Decoder<'e>)
        (d6: Decoder<'f>)
        (d7: Decoder<'g>)
        : Decoder<'Output>
        =
        { new Decoder<'Output> with
            member _.Decode(helpers, value) =
                match
                    d1.Decode(helpers, value),
                    d2.Decode(helpers, value),
                    d3.Decode(helpers, value),
                    d4.Decode(helpers, value),
                    d5.Decode(helpers, value),
                    d6.Decode(helpers, value),
                    d7.Decode(helpers, value)
                with
                | Ok v1, Ok v2, Ok v3, Ok v4, Ok v5, Ok v6, Ok v7 ->
                    Ok(ctor v1 v2 v3 v4 v5 v6 v7)
                | Error er, _, _, _, _, _, _ -> Error er
                | _, Error er, _, _, _, _, _ -> Error er
                | _, _, Error er, _, _, _, _ -> Error er
                | _, _, _, Error er, _, _, _ -> Error er
                | _, _, _, _, Error er, _, _ -> Error er
                | _, _, _, _, _, Error er, _ -> Error er
                | _, _, _, _, _, _, Error er -> Error er
        }

    /// <summary>Decode with eight decoders and combine their results.</summary>
    let map8
        (ctor: 'a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g -> 'h -> 'Output)
        (d1: Decoder<'a>)
        (d2: Decoder<'b>)
        (d3: Decoder<'c>)
        (d4: Decoder<'d>)
        (d5: Decoder<'e>)
        (d6: Decoder<'f>)
        (d7: Decoder<'g>)
        (d8: Decoder<'h>)
        : Decoder<'Output>
        =
        { new Decoder<'Output> with
            member _.Decode(helpers, value) =
                match
                    d1.Decode(helpers, value),
                    d2.Decode(helpers, value),
                    d3.Decode(helpers, value),
                    d4.Decode(helpers, value),
                    d5.Decode(helpers, value),
                    d6.Decode(helpers, value),
                    d7.Decode(helpers, value),
                    d8.Decode(helpers, value)
                with
                | Ok v1, Ok v2, Ok v3, Ok v4, Ok v5, Ok v6, Ok v7, Ok v8 ->
                    Ok(ctor v1 v2 v3 v4 v5 v6 v7 v8)
                | Error er, _, _, _, _, _, _, _ -> Error er
                | _, Error er, _, _, _, _, _, _ -> Error er
                | _, _, Error er, _, _, _, _, _ -> Error er
                | _, _, _, Error er, _, _, _, _ -> Error er
                | _, _, _, _, Error er, _, _, _ -> Error er
                | _, _, _, _, _, Error er, _, _ -> Error er
                | _, _, _, _, _, _, Error er, _ -> Error er
                | _, _, _, _, _, _, _, Error er -> Error er
        }

    ///////////////////////
    //  Option decoders  //
    ///////////////////////

    /// <summary>
    /// Decode <c>null</c> into <c>None</c>, and anything else with the given decoder.
    /// </summary>
    /// <remarks>
    /// Lossy: a nested option does not round-trip. Use <see cref="losslessOption"/> when you need
    /// the distinction.
    /// </remarks>
    let lossyOption (decoder: Decoder<'value>) : Decoder<'value option> =
        { new Decoder<'value option> with
            member _.Decode(helpers, value) =
                if helpers.isNullValue value then
                    Ok None
                else
                    decoder.Decode(helpers, value) |> Result.map Some
        }

    /// <summary>
    /// Decode the object written by <see cref="M:Thoth.Json.Core.Encode.losslessOption"/> into an
    /// option.
    /// </summary>
    /// <remarks>
    /// Round-trips at any nesting depth, at the cost of a heavier representation.
    /// </remarks>
    let losslessOption (decoder: Decoder<'value>) : Decoder<'value option> =
        field "$type" string
        |> andThen (fun typeName ->
            match typeName with
            | "option" ->
                field "$case" string
                |> andThen (fun state ->
                    match state with
                    | "none" -> succeed None
                    | "some" -> field "$value" decoder |> map Some
                    | _ ->
                        fail (
                            "Expecting a state field with value 'none' or 'some' but got "
                            + state
                        )
                )
            | _ -> fail ("Expecting an Option type but got " + typeName)
        )

    //////////////////////
    // Object builder ///
    ////////////////////

    /// <summary>
    /// Apply one decoded value to a decoded function, for building a value field by field.
    /// </summary>
    /// <example>
    /// <code lang="fsharp">
    /// type Point =
    ///     {
    ///         X : float
    ///         Y : float
    ///     }
    ///
    /// module Point =
    ///     let create x y = { X = x; Y = y }
    ///
    ///     let decode =
    ///         Decode.succeed create
    ///             |> Decode.andMap (Decode.field "x" Decode.float)
    ///             |> Decode.andMap (Decode.field "y" Decode.float)
    /// </code>
    /// </example>
    let andMap<'a, 'b> : 'a Decoder -> ('a -> 'b) Decoder -> 'b Decoder =
        map2 (|>)

    type IRequiredGetter =
        abstract Field: string -> Decoder<'a> -> 'a
        abstract At: List<string> -> Decoder<'a> -> 'a
        abstract Raw: Decoder<'a> -> 'a

    type IOptionalGetter =
        abstract Field: string -> Decoder<'a> -> 'a option
        abstract At: List<string> -> Decoder<'a> -> 'a option
        abstract Raw: Decoder<'a> -> 'a option

    type IGetters =
        abstract Required: IRequiredGetter
        abstract Optional: IOptionalGetter

    let private unwrapWith
        (errors: ResizeArray<DecoderError<'JsonValue>>)
        (helpers: IDecoderHelpers<'JsonValue>)
        (decoder: Decoder<'T>)
        (value: 'JsonValue)
        : 'T
        =
        match decoder.Decode(helpers, value) with
        | Ok v -> v
        | Error er ->
            errors.Add(er)
            Unchecked.defaultof<'T>


    type Getters<'JsonValue, 'T>
        (helpers: IDecoderHelpers<'JsonValue>, value: 'JsonValue)
        =
        let mutable errors = ResizeArray<DecoderError<'JsonValue>>()

        let required =
            { new IRequiredGetter with
                member __.Field (fieldName: string) (decoder: Decoder<_>) =
                    unwrapWith errors helpers (field fieldName decoder) value

                member __.At (fieldNames: string list) (decoder: Decoder<_>) =
                    unwrapWith errors helpers (at fieldNames decoder) value

                member __.Raw(decoder: Decoder<_>) =
                    unwrapWith errors helpers decoder value
            }

        let optional =
            { new IOptionalGetter with
                member __.Field (fieldName: string) (decoder: Decoder<_>) =
                    unwrapWith errors helpers (optional fieldName decoder) value

                member __.At (fieldNames: string list) (decoder: Decoder<_>) =
                    unwrapWith
                        errors
                        helpers
                        (optionalAt fieldNames decoder)
                        value

                member __.Raw(decoder: Decoder<_>) =
                    match decoder.Decode(helpers, value) with
                    | Ok v -> Some v
                    | Error((_, reason) as error) ->
                        match reason with
                        | BadPrimitive(_, v)
                        | BadPrimitiveExtra(_, v, _)
                        | BadType(_, v) ->
                            if helpers.isNullValue v then
                                None
                            else
                                errors.Add(error)
                                Unchecked.defaultof<_>
                        | BadField _
                        | BadPath _ -> None
                        | TooSmallArray _
                        | FailMessage _
                        | BadOneOf _ ->
                            errors.Add(error)
                            Unchecked.defaultof<_>
            }

        member __.Errors: _ list = Seq.toList errors

        interface IGetters with
            member __.Required = required
            member __.Optional = optional

    /// <summary>
    /// Decode an object, reading each property through <c>get.Required</c> or <c>get.Optional</c>.
    /// </summary>
    /// <example>
    /// <code lang="fsharp">
    /// let pointDecoder =
    ///     Decode.object (fun get ->
    ///         {
    ///             X = get.Required.Field "x" Decode.int
    ///             Y = get.Required.Field "y" Decode.int
    ///         }
    ///     )
    /// </code>
    /// </example>
    let object (builder: IGetters -> 'value) : Decoder<'value> =
        { new Decoder<'value> with
            member _.Decode(helpers, value) =
                let getters = Getters(helpers, value)
                let result = builder getters

                match getters.Errors with
                | [] -> Ok result
                | fst :: _ as errors ->
                    if errors.Length > 1 then
                        ("", BadOneOf errors) |> Error
                    else
                        Error fst
        }

    ///////////////////////
    // Tuples decoders ///
    ////////////////////

    /// <summary>Decode a JSON array of 2 elements into a tuple.</summary>
    let tuple2
        (decoder1: Decoder<'T1>)
        (decoder2: Decoder<'T2>)
        : Decoder<'T1 * 'T2>
        =
        index 0 decoder1
        |> andThen (fun v1 ->
            index 1 decoder2 |> andThen (fun v2 -> succeed (v1, v2))
        )

    /// <summary>Decode a JSON array of 3 elements into a tuple.</summary>
    let tuple3
        (decoder1: Decoder<'T1>)
        (decoder2: Decoder<'T2>)
        (decoder3: Decoder<'T3>)
        : Decoder<'T1 * 'T2 * 'T3>
        =
        index 0 decoder1
        |> andThen (fun v1 ->
            index 1 decoder2
            |> andThen (fun v2 ->
                index 2 decoder3 |> andThen (fun v3 -> succeed (v1, v2, v3))
            )
        )

    /// <summary>Decode a JSON array of 4 elements into a tuple.</summary>
    let tuple4
        (decoder1: Decoder<'T1>)
        (decoder2: Decoder<'T2>)
        (decoder3: Decoder<'T3>)
        (decoder4: Decoder<'T4>)
        : Decoder<'T1 * 'T2 * 'T3 * 'T4>
        =
        index 0 decoder1
        |> andThen (fun v1 ->
            index 1 decoder2
            |> andThen (fun v2 ->
                index 2 decoder3
                |> andThen (fun v3 ->
                    index 3 decoder4
                    |> andThen (fun v4 -> succeed (v1, v2, v3, v4))
                )
            )
        )

    /// <summary>Decode a JSON array of 5 elements into a tuple.</summary>
    let tuple5
        (decoder1: Decoder<'T1>)
        (decoder2: Decoder<'T2>)
        (decoder3: Decoder<'T3>)
        (decoder4: Decoder<'T4>)
        (decoder5: Decoder<'T5>)
        : Decoder<'T1 * 'T2 * 'T3 * 'T4 * 'T5>
        =
        index 0 decoder1
        |> andThen (fun v1 ->
            index 1 decoder2
            |> andThen (fun v2 ->
                index 2 decoder3
                |> andThen (fun v3 ->
                    index 3 decoder4
                    |> andThen (fun v4 ->
                        index 4 decoder5
                        |> andThen (fun v5 -> succeed (v1, v2, v3, v4, v5))
                    )
                )
            )
        )

    /// <summary>Decode a JSON array of 6 elements into a tuple.</summary>
    let tuple6
        (decoder1: Decoder<'T1>)
        (decoder2: Decoder<'T2>)
        (decoder3: Decoder<'T3>)
        (decoder4: Decoder<'T4>)
        (decoder5: Decoder<'T5>)
        (decoder6: Decoder<'T6>)
        : Decoder<'T1 * 'T2 * 'T3 * 'T4 * 'T5 * 'T6>
        =
        index 0 decoder1
        |> andThen (fun v1 ->
            index 1 decoder2
            |> andThen (fun v2 ->
                index 2 decoder3
                |> andThen (fun v3 ->
                    index 3 decoder4
                    |> andThen (fun v4 ->
                        index 4 decoder5
                        |> andThen (fun v5 ->
                            index 5 decoder6
                            |> andThen (fun v6 ->
                                succeed (v1, v2, v3, v4, v5, v6)
                            )
                        )
                    )
                )
            )
        )

    /// <summary>Decode a JSON array of 7 elements into a tuple.</summary>
    let tuple7
        (decoder1: Decoder<'T1>)
        (decoder2: Decoder<'T2>)
        (decoder3: Decoder<'T3>)
        (decoder4: Decoder<'T4>)
        (decoder5: Decoder<'T5>)
        (decoder6: Decoder<'T6>)
        (decoder7: Decoder<'T7>)
        : Decoder<'T1 * 'T2 * 'T3 * 'T4 * 'T5 * 'T6 * 'T7>
        =
        index 0 decoder1
        |> andThen (fun v1 ->
            index 1 decoder2
            |> andThen (fun v2 ->
                index 2 decoder3
                |> andThen (fun v3 ->
                    index 3 decoder4
                    |> andThen (fun v4 ->
                        index 4 decoder5
                        |> andThen (fun v5 ->
                            index 5 decoder6
                            |> andThen (fun v6 ->
                                index 6 decoder7
                                |> andThen (fun v7 ->
                                    succeed (v1, v2, v3, v4, v5, v6, v7)
                                )
                            )
                        )
                    )
                )
            )
        )

    /// <summary>Decode a JSON array of 8 elements into a tuple.</summary>
    let tuple8
        (decoder1: Decoder<'T1>)
        (decoder2: Decoder<'T2>)
        (decoder3: Decoder<'T3>)
        (decoder4: Decoder<'T4>)
        (decoder5: Decoder<'T5>)
        (decoder6: Decoder<'T6>)
        (decoder7: Decoder<'T7>)
        (decoder8: Decoder<'T8>)
        : Decoder<'T1 * 'T2 * 'T3 * 'T4 * 'T5 * 'T6 * 'T7 * 'T8>
        =
        index 0 decoder1
        |> andThen (fun v1 ->
            index 1 decoder2
            |> andThen (fun v2 ->
                index 2 decoder3
                |> andThen (fun v3 ->
                    index 3 decoder4
                    |> andThen (fun v4 ->
                        index 4 decoder5
                        |> andThen (fun v5 ->
                            index 5 decoder6
                            |> andThen (fun v6 ->
                                index 6 decoder7
                                |> andThen (fun v7 ->
                                    index 7 decoder8
                                    |> andThen (fun v8 ->
                                        succeed (
                                            v1,
                                            v2,
                                            v3,
                                            v4,
                                            v5,
                                            v6,
                                            v7,
                                            v8
                                        )
                                    )
                                )
                            )
                        )
                    )
                )
            )
        )

    ///////////
    // Map ///
    /////////

    /// <summary>Decode an object into a map keyed by the property names.</summary>
    let dict (decoder: Decoder<'value>) : Decoder<Map<string, 'value>> =
        map Map.ofList (keyValuePairs decoder)

    /// <summary>Decode an array of <c>[ key, value ]</c> pairs into a map. Use it when the key is not a string.</summary>
    let map'
        (keyDecoder: Decoder<'key>)
        (valueDecoder: Decoder<'value>)
        : Decoder<Map<'key, 'value>>
        =
        map Map.ofSeq (array (tuple2 keyDecoder valueDecoder))

    type private FixDecoder<'a>(make: Decoder<'a> -> Decoder<'a>) as this =
        let self = make this

        interface Decoder<'a> with
            member this.Decode
                (helpers: IDecoderHelpers<'JsonValue>, value: 'JsonValue)
                =
                self.Decode(helpers, value)

    /// <summary>
    /// Allow to build a decoder that can call itself
    /// </summary>
    /// <example>
    /// <code lang="fsharp">
    /// type Tree =
    ///     | Empty
    ///     | Branch of Tree * int * Tree
    ///
    /// module Tree =
    ///
    ///     let decode =
    ///         Decode.fix
    ///             (fun self ->
    ///                 Decode.oneOf
    ///                     [
    ///                         Decode.unit
    ///                             |> Decode.map (fun () -> Tree.Empty)
    ///
    ///                         Decode.tuple3 self Decode.int self
    ///                             |> Decode.map Tree.Branch
    ///                     ])
    /// </code>
    /// </example>
    let fix (make: Decoder<'a> -> Decoder<'a>) : Decoder<'a> =
        FixDecoder<'a>(make)

    //////////////////
    // requireSome ///
    /////////////////

    /// <summary>Turn <c>None</c> into a failure carrying the given message.</summary>
    let requireSome
        (errorMessage: string)
        (decoder: Decoder<'a option>)
        : Decoder<'a>
        =
        decoder
        |> andThen (
            function
            | Some value -> succeed value
            | None -> fail errorMessage
        )

    /// <summary>Turn a decoder of an option into one which fails on <c>None</c>.</summary>
    let notNone (decoder: Decoder<'a option>) : Decoder<'a> =
        decoder
        |> andThen (
            function
            | Some value -> succeed value
            | None -> fail "Expecting a value but instead got: None"
        )

    ////////////
    // Enum ///
    /////////

#if !FABLE_REPL_LIB
    module Enum =

        let inline private enumOfValue value =
            if Enum.IsDefined(typeof<'TEnum>, value) then
                LanguagePrimitives.EnumOfValue<_, 'TEnum> value |> succeed
            else
                { new Decoder<_> with
                    member _.Decode<'JsonValue>(_, value: 'JsonValue) =
                        ("",
                         BadPrimitiveExtra(
                             typeof<'TEnum>.FullName,
                             value,
                             "Unknown value provided for the enum"
                         ))
                        |> Error
                }

        /// <summary>Decode a JSON number into an enum with an underlying byte.</summary>
        let inline byte<'TEnum when 'TEnum: enum<byte>> : Decoder<'TEnum> =
            byte |> andThen enumOfValue

        /// <summary>Decode a JSON number into an enum with an underlying sbyte.</summary>
        let inline sbyte<'TEnum when 'TEnum: enum<sbyte>> : Decoder<'TEnum> =
            sbyte |> andThen enumOfValue

        /// <summary>Decode a JSON number into an enum with an underlying int16.</summary>
        let inline int16<'TEnum when 'TEnum: enum<int16>> : Decoder<'TEnum> =
            int16 |> andThen enumOfValue

        /// <summary>Decode a JSON number into an enum with an underlying uint16.</summary>
        let inline uint16<'TEnum when 'TEnum: enum<uint16>> : Decoder<'TEnum> =
            uint16 |> andThen enumOfValue

        /// <summary>Decode a JSON number into an enum with an underlying int.</summary>
        let inline int<'TEnum when 'TEnum: enum<int>> : Decoder<'TEnum> =
            int |> andThen enumOfValue

        /// <summary>Decode a JSON number into an enum with an underlying uint32.</summary>
        let inline uint32<'TEnum when 'TEnum: enum<uint32>> : Decoder<'TEnum> =
            uint32 |> andThen enumOfValue
#endif

    /// <summary>The decoder half of a codec.</summary>
    let codec (c: Codec<'t>) : Decoder<'t> = c.Decoder
