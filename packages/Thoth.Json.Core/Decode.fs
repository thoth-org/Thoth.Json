namespace Thoth.Json.Core

open Fable.Core
open System.Globalization

[<RequireQualifiedAccess>]
module Decode =

    module Helpers =

        let prependPath
            (path: string)
            (err: DecoderError<'JsonValue>)
            : DecoderError<'JsonValue>
            =
            let (oldPath, reason) = err
            (path + oldPath, reason)

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
                + ("\nNode `" + fieldName + "` is unkown.")
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

    let int16: Decoder<int16> =
        integral
            "an int16"
            System.Int16.TryParse
            (fun () -> System.Int16.MinValue)
            (fun () -> System.Int16.MaxValue)
            int16

    let uint16: Decoder<uint16> =
        integral
            "an uint16"
            System.UInt16.TryParse
            (fun () -> System.UInt16.MinValue)
            (fun () -> System.UInt16.MaxValue)
            uint16

    let int: Decoder<int> =
        integral
            "an int"
            System.Int32.TryParse
            (fun () -> System.Int32.MinValue)
            (fun () -> System.Int32.MaxValue)
            int

    let uint32: Decoder<uint32> =
        integral
            "an uint32"
            System.UInt32.TryParse
            (fun () -> System.UInt32.MinValue)
            (fun () -> System.UInt32.MaxValue)
            uint32

    let int64: Decoder<int64> =
        integral
            "an int64"
            System.Int64.TryParse
            (fun () -> System.Int64.MinValue)
            (fun () -> System.Int64.MaxValue)
            int64

    let uint64: Decoder<uint64> =
        integral
            "an uint64"
            System.UInt64.TryParse
            (fun () -> System.UInt64.MinValue)
            (fun () -> System.UInt64.MaxValue)
            uint64

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

    let bool: Decoder<bool> =
        { new Decoder<bool> with
            member _.Decode(helpers, value) =
                if helpers.isBoolean value then
                    Ok(helpers.asBoolean value)
                else
                    ("", BadPrimitive("a boolean", value)) |> Error
        }

    let float: Decoder<float> =
        { new Decoder<float> with
            member _.Decode(helpers, value) =
                if helpers.isNumber value then
                    Ok(helpers.asFloat value)
                else
                    ("", BadPrimitive("a float", value)) |> Error
        }


    let float32: Decoder<float32> =
        { new Decoder<float32> with
            member _.Decode(helpers, value) =
                if helpers.isNumber value then
                    Ok(helpers.asFloat32 value)
                else
                    ("", BadPrimitive("a float32", value)) |> Error
        }

    let decimal: Decoder<decimal> =
        { new Decoder<decimal> with
            member _.Decode(helpers, value) =
                if helpers.isNumber value then
                    helpers.asFloat value |> decimal |> Ok
                elif helpers.isString value then
                    match System.Decimal.TryParse(helpers.asString value) with
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


    let option (decoder: Decoder<'value>) : Decoder<'value option> =
        { new Decoder<'value option> with
            member _.Decode(helpers, value) =
                if helpers.isNullValue value then
                    Ok None
                else
                    decoder.Decode(helpers, value) |> Result.map Some
        }

    /// <summary>
    /// Runs the decoder against the given JSON value.
    ///
    /// If the decoder fails, it reports the error prefixed with the given path.
    ///
    /// </summary>
    /// <example>
    /// <code lang="fsharp">
    /// module Decode =
    ///     let fromRootValue (decoder : Decoder&lt;'T&gt;) =
    ///         Decode.fromValue "$" decoder
    /// </code>
    /// </example>
    /// <param name="path">Path used to report the error</param>
    /// <param name="decoder">Decoder to apply</param>
    /// <param name="value">JSON value to decoder</param>
    /// <returns>
    /// Returns <c>Ok</c> if the decoder succeeds, otherwise <c>Error</c> with the error message.
    /// </returns>
    let fromValue
        (helpers: IDecoderHelpers<'JsonValue>)
        (decoder: Decoder<'T>)
        =
        fun value ->
            match decoder.Decode(helpers, value) with
            | Ok success -> Ok success
            | Error error -> Error(errorToString helpers error)

    //////////////////////
    // Data structure ///
    ////////////////////

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


    let keys: Decoder<string list> =
        { new Decoder<string list> with
            member _.Decode(helpers, value) =
                if helpers.isObject value then
                    helpers.getProperties value |> List.ofSeq |> Ok
                else
                    ("", BadPrimitive("an object", value)) |> Error
        }


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
                            | Error er -> Error er
                            | Ok value -> (prop, value) :: acc |> Ok
                    )
                    |> Result.map List.rev
                | Error e -> Error e
        }

    //////////////////////////////
    // Inconsistent Structure ///
    ////////////////////////////

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

    let nil (output: 'a) : Decoder<'a> =
        { new Decoder<'a> with
            member _.Decode(helpers, value) =
                if helpers.isNullValue value then
                    Ok output
                else
                    ("", BadPrimitive("null", value)) |> Error
        }

    let value _ v = Ok v

    let succeed (output: 'a) : Decoder<'a> =
        { new Decoder<'a> with
            member _.Decode(_, _) = Ok output
        }

    let fail (msg: string) : Decoder<'a> =
        { new Decoder<'a> with
            member _.Decode(_, _) = Error("", FailMessage msg)
        }

    let andThen (cb: 'a -> Decoder<'b>) (decoder: Decoder<'a>) : Decoder<'b> =
        { new Decoder<'b> with
            member _.Decode(helpers, value) =
                match decoder.Decode(helpers, value) with
                | Error er -> Error er
                | Ok result -> (cb result).Decode(helpers, value)
        }

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

    /////////////////////
    // Map functions ///
    ///////////////////

    let map (ctor: 'a -> 'Output) (d1: Decoder<'a>) : Decoder<'Output> =
        { new Decoder<'Output> with
            member _.Decode(helpers, value) =
                match d1.Decode(helpers, value) with
                | Ok v1 -> Ok(ctor v1)
                | Error er -> Error er
        }

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

    //////////////////////
    // Object builder ///
    ////////////////////

    /// <summary>
    /// Allow to incrementally apply a decoder, for building large objects.
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

    let tuple2
        (decoder1: Decoder<'T1>)
        (decoder2: Decoder<'T2>)
        : Decoder<'T1 * 'T2>
        =
        index 0 decoder1
        |> andThen (fun v1 ->
            index 1 decoder2 |> andThen (fun v2 -> succeed (v1, v2))
        )

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

    let dict (decoder: Decoder<'value>) : Decoder<Map<string, 'value>> =
        map Map.ofList (keyValuePairs decoder)

    let map'
        (keyDecoder: Decoder<'key>)
        (valueDecoder: Decoder<'value>)
        : Decoder<Map<'key, 'value>>
        =
        map Map.ofSeq (array (tuple2 keyDecoder valueDecoder))

////////////
// Enum ///
/////////

#if !FABLE_REPL_LIB
    module Enum =

        let inline byte<'TEnum when 'TEnum: enum<byte>> : Decoder<'TEnum> =
            byte
            |> andThen (fun value ->
                LanguagePrimitives.EnumOfValue<byte, 'TEnum> value |> succeed
            )

        let inline sbyte<'TEnum when 'TEnum: enum<sbyte>> : Decoder<'TEnum> =
            sbyte
            |> andThen (fun value ->
                LanguagePrimitives.EnumOfValue<sbyte, 'TEnum> value |> succeed
            )

        let inline int16<'TEnum when 'TEnum: enum<int16>> : Decoder<'TEnum> =
            int16
            |> andThen (fun value ->
                LanguagePrimitives.EnumOfValue<int16, 'TEnum> value |> succeed
            )

        let inline uint16<'TEnum when 'TEnum: enum<uint16>> : Decoder<'TEnum> =
            uint16
            |> andThen (fun value ->
                LanguagePrimitives.EnumOfValue<uint16, 'TEnum> value |> succeed
            )

        let inline int<'TEnum when 'TEnum: enum<int>> : Decoder<'TEnum> =
            int
            |> andThen (fun value ->
                LanguagePrimitives.EnumOfValue<int, 'TEnum> value |> succeed
            )

        let inline uint32<'TEnum when 'TEnum: enum<uint32>> : Decoder<'TEnum> =
            uint32
            |> andThen (fun value ->
                LanguagePrimitives.EnumOfValue<uint32, 'TEnum> value |> succeed
            )
#endif
