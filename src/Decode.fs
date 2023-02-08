
namespace Thoth.Json
open System.Text.RegularExpressions

[<RequireQualifiedAccess>]
module Decode =

    open System.Globalization
    open Fable.Core
    open Fable.Core.JsInterop

    module Helpers =
        [<Emit("typeof $0")>]
        let jsTypeof (_ : JsonValue) : string = jsNative

        [<Emit("$0 instanceof SyntaxError")>]
        let isSyntaxError (_ : JsonValue) : bool = jsNative

        let inline getField (fieldName: string) (o: JsonValue) = o?(fieldName)
        let inline isString (o: JsonValue) : bool = o :? string

        let inline isBoolean (o: JsonValue) : bool = o :? bool

        let inline isNumber (o: JsonValue) : bool = jsTypeof o = "number"

        let inline isArray (o: JsonValue) : bool = JS.Constructors.Array.isArray(o)

        [<Emit("$0 === null ? false : (Object.getPrototypeOf($0 || false) === Object.prototype)")>]
        let isObject (_ : JsonValue) : bool = jsNative

        let inline isNaN (o: JsonValue) : bool = JS.Constructors.Number.isNaN(!!o)

        let inline isNullValue (o: JsonValue): bool = isNull o

        /// is the value an integer? This returns false for 1.1, NaN, Infinite, ...
        [<Emit("isFinite($0) && Math.floor($0) === $0")>]
        let isIntegralValue (_: JsonValue) : bool = jsNative

        [<Emit("$1 <= $0 && $0 < $2")>]
        let isBetweenInclusive(_v: JsonValue, _min: obj, _max: obj) = jsNative

        [<Emit("isFinite($0) && !($0 % 1)")>]
        let isIntFinite (_: JsonValue) : bool = jsNative

        let isUndefined (o: JsonValue): bool = jsTypeof o = "undefined"

        [<Emit("JSON.stringify($0, null, 4) + ''")>]
        let anyToString (_: JsonValue) : string = jsNative

        let inline isFunction (o: JsonValue) : bool = jsTypeof o = "function"

        let inline objectKeys (o: JsonValue) : string seq = upcast JS.Constructors.Object.keys(o)
        let inline asBool (o: JsonValue): bool = unbox o
        let inline asInt (o: JsonValue): int = unbox o
        let inline asFloat (o: JsonValue): float = unbox o
        let inline asFloat32 (o: JsonValue): float32 = unbox o
        let inline asString (o: JsonValue): string = unbox o
        let inline asArray (o: JsonValue): JsonValue[] = unbox o

    let private genericMsg msg value newLine =
        try
            "Expecting "
                + msg
                + " but instead got:"
                + (if newLine then "\n" else " ")
                + (Helpers.anyToString value)
        with
            | _ ->
                "Expecting "
                + msg
                + " but decoder failed. Couldn't report given value due to circular structure."
                + (if newLine then "\n" else " ")

    let private errorToString (path : string, error) =
        let reason =
            match error with
            | BadPrimitive (msg, value) ->
                genericMsg msg value false
            | BadType (msg, value) ->
                genericMsg msg value true
            | BadPrimitiveExtra (msg, value, reason) ->
                genericMsg msg value false + "\nReason: " + reason
            | BadField (msg, value) ->
                genericMsg msg value true
            | BadPath (msg, value, fieldName) ->
                genericMsg msg value true + ("\nNode `" + fieldName + "` is unkown.")
            | TooSmallArray (msg, value) ->
                "Expecting " + msg + ".\n" + (Helpers.anyToString value)
            | BadOneOf messages ->
                "The following errors were found:\n\n" + String.concat "\n\n" messages
            | FailMessage msg ->
                "The following `failure` occurred with the decoder: " + msg

        match error with
        | BadOneOf _ ->
            // Don't need to show the path here because each error case will show it's own path
            reason
        | _ ->
            "Error at: `" + path + "`\n" + reason

    ///////////////
    // Runners ///
    /////////////

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
    let fromValue (path : string) (decoder : Decoder<'T>) =
        fun value ->
            match decoder path value with
            | Ok success ->
                Ok success
            | Error error ->
                Error (errorToString error)

    /// <summary>
    /// Parse the provided string in as JSON and then run the decoder against it.
    /// </summary>
    /// <param name="decoder">Decoder to apply</param>
    /// <param name="value">JSON string to decode</param>
    /// <returns>
    /// Returns <c>Ok</c> if the decoder succeeds, otherwise <c>Error</c> with the error message.
    /// </returns>
    let fromString (decoder : Decoder<'T>) =
        fun value ->
            try
               let json = JS.JSON.parse value
               fromValue "$" decoder json
            with
                | ex when Helpers.isSyntaxError ex ->
                    Error("Given an invalid JSON: " + ex.Message)

    /// <summary>
    /// Parse the provided string in as JSON and then run the decoder against it.
    /// </summary>
    /// <param name="decoder">Decoder to apply</param>
    /// <param name="value">JSON string to decode</param>
    /// <returns>
    /// Return the decoded value if the decoder succeeds, otherwise throws an exception.
    /// </returns>
    let unsafeFromString (decoder : Decoder<'T>) =
        fun value ->
            match fromString decoder value with
            | Ok x -> x
            | Error msg -> failwith msg

    [<System.Obsolete("Please use fromValue instead")>]
    let decodeValue (path : string) (decoder : Decoder<'T>) = fromValue path decoder

    [<System.Obsolete("Please use fromString instead")>]
    let decodeString (decoder : Decoder<'T>) = fromString decoder

    //////////////////
    // Primitives ///
    ////////////////

    let string : Decoder<string> =
        fun path value ->
            if Helpers.isString value then
                Ok(Helpers.asString value)
            else
                (path, BadPrimitive("a string", value)) |> Error

    let char : Decoder<char> =
        fun path value ->
            if Helpers.isString value then
                let str = Helpers.asString value
                if str.Length = 1 then
                    Ok(str.[0])
                else
                    (path, BadPrimitive("a single character string", value)) |> Error
            else
                (path, BadPrimitive("a char", value)) |> Error

    let guid : Decoder<System.Guid> =
        fun path value ->
            if Helpers.isString value then
                match System.Guid.TryParse (Helpers.asString value) with
                | true, x -> Ok x
                | _ -> (path, BadPrimitive("a guid", value)) |> Error
            else (path, BadPrimitive("a guid", value)) |> Error

    let unit : Decoder<unit> =
        fun path value ->
            if Helpers.isNullValue value then
                Ok ()
            else
                (path, BadPrimitive("null", value)) |> Error

    let inline private integral
                    (name : string)
                    (tryParse : (string -> bool * 'T))
                    (min : unit->'T)
                    (max : unit->'T)
                    (conv : float -> 'T) : Decoder< 'T > =

        fun path value ->
            if Helpers.isNumber value then
                let value : float = unbox value
                if Helpers.isIntegralValue value then
                    if (float(min())) <= value && value <= (float(max())) then
                        Ok(conv value)
                    else
                        (path, BadPrimitiveExtra(name, value, "Value was either too large or too small for " + name)) |> Error
                else
                    (path, BadPrimitiveExtra(name, value, "Value is not an integral value")) |> Error
            elif Helpers.isString value then
                match tryParse (Helpers.asString value) with
                | true, x -> Ok x
                | _ -> (path, BadPrimitive(name, value)) |> Error
            else
                (path, BadPrimitive(name, value)) |> Error

    let sbyte : Decoder<sbyte> =
        integral
            "a sbyte"
            System.SByte.TryParse
            (fun () -> System.SByte.MinValue)
            (fun () -> System.SByte.MaxValue)
            sbyte

    /// Alias to Decode.uint8
    let byte : Decoder<byte> =
        integral
            "a byte"
            System.Byte.TryParse
            (fun () -> System.Byte.MinValue)
            (fun () -> System.Byte.MaxValue)
            byte

    let int16 : Decoder<int16> =
        integral
            "an int16"
            System.Int16.TryParse
            (fun () -> System.Int16.MinValue)
            (fun () -> System.Int16.MaxValue)
            int16

    let uint16 : Decoder<uint16> =
        integral
            "an uint16"
            System.UInt16.TryParse
            (fun () -> System.UInt16.MinValue)
            (fun () -> System.UInt16.MaxValue)
            uint16

    let int : Decoder<int> =
        integral
            "an int"
            System.Int32.TryParse
            (fun () -> System.Int32.MinValue)
            (fun () -> System.Int32.MaxValue)
            int

    let uint32 : Decoder<uint32> =
        integral
            "an uint32"
            System.UInt32.TryParse
            (fun () -> System.UInt32.MinValue)
            (fun () -> System.UInt32.MaxValue)
            uint32

    let int64 : Decoder<int64> =
        integral
            "an int64"
            System.Int64.TryParse
            (fun () -> System.Int64.MinValue)
            (fun () -> System.Int64.MaxValue)
            int64

    let uint64 : Decoder<uint64> =
        integral
            "an uint64"
            System.UInt64.TryParse
            (fun () -> System.UInt64.MinValue)
            (fun () -> System.UInt64.MaxValue)
            uint64

    let bigint : Decoder<bigint> =
        fun path value ->
            if Helpers.isNumber value then
                Helpers.asInt value |> bigint |> Ok
            elif Helpers.isString value then
                // TODO: BigInt.TryParse has been added in Fable 2.1.4
                // Don't use it for now to support lower Fable versions
                try
                    bigint.Parse (Helpers.asString value) |> Ok
                with _ ->
                    (path, BadPrimitive("a bigint", value)) |> Error
            else
                (path, BadPrimitive("a bigint", value)) |> Error

    let bool : Decoder<bool> =
        fun path value ->
            if Helpers.isBoolean value then
                Ok(Helpers.asBool value)
            else
                (path, BadPrimitive("a boolean", value)) |> Error

    let float : Decoder<float> =
        fun path value ->
            if Helpers.isNumber value then
                Ok(Helpers.asFloat value)
            else
                (path, BadPrimitive("a float", value)) |> Error

    let float32 : Decoder<float32> =
        fun path value ->
            if Helpers.isNumber value then
                Ok(Helpers.asFloat32 value)
            else
                (path, BadPrimitive("a float32", value)) |> Error

    let decimal : Decoder<decimal> =
        fun path value ->
            if Helpers.isNumber value then
                Helpers.asFloat value |> decimal |> Ok
            elif Helpers.isString value then
                match System.Decimal.TryParse (Helpers.asString value) with
                | true, x -> Ok x
                | _ -> (path, BadPrimitive("a decimal", value)) |> Error
            else
                (path, BadPrimitive("a decimal", value)) |> Error

    [<System.Obsolete("Please use datetimeUtc instead.")>]
    let datetime : Decoder<System.DateTime> =
        fun path value ->
            if Helpers.isString value then
                match System.DateTime.TryParse (Helpers.asString value) with
                | true, x -> x.ToUniversalTime() |> Ok
                | _ -> (path, BadPrimitive("a datetime", value)) |> Error
            else
                (path, BadPrimitive("a datetime", value)) |> Error

    /// Decode a System.DateTime value using Sytem.DateTime.TryParse, then convert it to UTC.
    let datetimeUtc : Decoder<System.DateTime> =
        fun path value ->
            if Helpers.isString value then
                match System.DateTime.TryParse (Helpers.asString value) with
                | true, x -> x.ToUniversalTime() |> Ok
                | _ -> (path, BadPrimitive("a datetime", value)) |> Error
            else
                (path, BadPrimitive("a datetime", value)) |> Error

    /// Decode a System.DateTime with DateTime.TryParse; uses default System.DateTimeStyles.
    let datetimeLocal : Decoder<System.DateTime> =
        fun path value ->
            if Helpers.isString value then
                match System.DateTime.TryParse (Helpers.asString value) with
                | true, x -> x |> Ok
                | _ -> (path, BadPrimitive("a datetime", value)) |> Error
            else
                (path, BadPrimitive("a datetime", value)) |> Error

    let datetimeOffset : Decoder<System.DateTimeOffset> =
        fun path value ->
            if Helpers.isString value then
                match System.DateTimeOffset.TryParse(Helpers.asString value) with
                | true, x -> Ok x
                | _ -> (path, BadPrimitive("a datetimeoffset", value)) |> Error
            else
                (path, BadPrimitive("a datetime", value)) |> Error

    let timespan : Decoder<System.TimeSpan> =
        fun path value ->
            if Helpers.isString value then
                match System.TimeSpan.TryParse(Helpers.asString value) with
                | true, x -> Ok x
                | _ -> (path, BadPrimitive("a timespan", value)) |> Error
            else
                (path, BadPrimitive("a timespan", value)) |> Error

    /////////////////////////
    // Object primitives ///
    ///////////////////////

    let private decodeMaybeNull path (decoder : Decoder<'T>) value =
        // The decoder may be an option decoder so give it an opportunity to check null values

        // We catch the null value case first to avoid executing the decoder logic
        // Indeed, if the decoder logic try to access the value to do something with it,
        // it can throw an exception about the value being null
        if Helpers.isNullValue value then
            Ok None
        else
            match decoder path value with
            | Ok v -> Ok(Some v)
            | Error er -> Error er

    let optional (fieldName : string) (decoder : Decoder<'value>) : Decoder<'value option> =
        fun path value ->
            if Helpers.isObject value then
                let fieldValue = Helpers.getField fieldName value
                if Helpers.isUndefined fieldValue then Ok None
                else decodeMaybeNull (path + "." + fieldName) decoder fieldValue
            else
                Error(path, BadType("an object", value))

    let private badPathError fieldNames currentPath value =
        let currentPath = defaultArg currentPath ("$"::fieldNames |> String.concat ".")
        let msg = "an object with path `" + (String.concat "." fieldNames) + "`"
        Error(currentPath, BadPath (msg, value, List.tryLast fieldNames |> Option.defaultValue ""))

    let optionalAt (fieldNames : string list) (decoder : Decoder<'value>) : Decoder<'value option> =
        fun firstPath firstValue ->
            ((firstPath, firstValue, None), fieldNames)
            ||> List.fold (fun (curPath, curValue, res) field ->
                match res with
                | Some _ -> curPath, curValue, res
                | None ->
                    if Helpers.isNullValue curValue then
                        curPath, curValue, Some (Ok None)
                    elif Helpers.isObject curValue then
                        let curValue = Helpers.getField field curValue
                        curPath + "." + field, curValue, None
                    else
                        let res = Error(curPath, BadType("an object", curValue))
                        curPath, curValue, Some res)
            |> function
                | _, _, Some res -> res
                | lastPath, lastValue, None ->
                    if Helpers.isUndefined lastValue then Ok None
                    else decodeMaybeNull lastPath decoder lastValue

    let field (fieldName: string) (decoder : Decoder<'value>) : Decoder<'value> =
        fun path value ->
            if Helpers.isObject value then
                let fieldValue = Helpers.getField fieldName value
                if Helpers.isUndefined fieldValue then
                    Error(path, BadField ("an object with a field named `" + fieldName + "`", value))
                else
                    decoder (path + "." + fieldName) fieldValue
            else
                Error(path, BadType("an object", value))

    let at (fieldNames: string list) (decoder : Decoder<'value>) : Decoder<'value> =
        fun firstPath firstValue ->
            ((firstPath, firstValue, None), fieldNames)
            ||> List.fold (fun (curPath, curValue, res) field ->
                match res with
                | Some _ -> curPath, curValue, res
                | None ->
                    if Helpers.isNullValue curValue then
                        let res = badPathError fieldNames (Some curPath) firstValue
                        curPath, curValue, Some res
                    elif Helpers.isObject curValue then
                        let curValue = Helpers.getField field curValue
                        if Helpers.isUndefined curValue then
                            let res = badPathError fieldNames None firstValue
                            curPath, curValue, Some res
                        else
                            curPath + "." + field, curValue, None
                    else
                        let res = Error(curPath, BadType("an object", curValue))
                        curPath, curValue, Some res)
            |> function
                | _, _, Some res -> res
                | lastPath, lastValue, None ->
                    decoder lastPath lastValue

    let index (requestedIndex: int) (decoder : Decoder<'value>) : Decoder<'value> =
        fun path value ->
            let currentPath = path + ".[" + (Operators.string requestedIndex) + "]"
            if Helpers.isArray value then
                let vArray = Helpers.asArray value
                if requestedIndex < vArray.Length then
                    decoder currentPath (vArray.[requestedIndex])
                else
                    let msg =
                        "a longer array. Need index `"
                            + (requestedIndex.ToString())
                            + "` but there are only `"
                            + (vArray.Length.ToString())
                            + "` entries"

                    (currentPath, TooSmallArray(msg, value))
                    |> Error
            else
                (currentPath, BadPrimitive("an array", value))
                |> Error

    let option (decoder : Decoder<'value>) : Decoder<'value option> =
        fun path value ->
            if Helpers.isNullValue value then Ok None
            else decoder path value |> Result.map Some

    //////////////////////
    // Data structure ///
    ////////////////////

    let list (decoder : Decoder<'value>) : Decoder<'value list> =
        fun path value ->
            if Helpers.isArray value then
                let mutable i = -1
                let tokens = Helpers.asArray value
                (Ok [], tokens) ||> Array.fold (fun acc value ->
                    i <- i + 1
                    match acc with
                    | Error _ -> acc
                    | Ok acc ->
                        match decoder (path + ".[" + (i.ToString()) + "]") value with
                        | Error er -> Error er
                        | Ok value -> Ok (value::acc))
                |> Result.map List.rev
            else
                (path, BadPrimitive ("a list", value))
                |> Error

    let seq (decoder : Decoder<'value>) : Decoder<'value seq> =
        fun path value ->
            if Helpers.isArray value then
                let mutable i = -1
                let tokens = Helpers.asArray value
                (Ok (seq []), tokens) ||> Array.fold (fun acc value ->
                    i <- i + 1
                    match acc with
                    | Error _ -> acc
                    | Ok acc ->
                        match decoder (path + ".[" + (i.ToString()) + "]") value with
                        | Error er -> Error er
                        | Ok value -> Ok (Seq.append [value] acc))
                |> Result.map Seq.rev
            else
                (path, BadPrimitive ("a seq", value))
                |> Error

    let array (decoder : Decoder<'value>) : Decoder<'value array> =
        fun path value ->
            if Helpers.isArray value then
                let mutable i = -1
                let tokens = Helpers.asArray value
                let arr = Array.zeroCreate tokens.Length
                (Ok arr, tokens) ||> Array.fold (fun acc value ->
                    i <- i + 1
                    match acc with
                    | Error _ -> acc
                    | Ok acc ->
                        match decoder (path + ".[" + (i.ToString()) + "]") value with
                        | Error er -> Error er
                        | Ok value -> acc.[i] <- value; Ok acc)
            else
                (path, BadPrimitive ("an array", value))
                |> Error

    let keys: Decoder<string list> =
        fun path value ->
            if Helpers.isObject value then
                Helpers.objectKeys value |> List.ofSeq |> Ok
            else
                (path, BadPrimitive ("an object", value))
                |> Error

    let keyValuePairs (decoder : Decoder<'value>) : Decoder<(string * 'value) list> =
        fun path value ->
            match keys path value with
            | Ok objectKeys ->
                (Ok [], objectKeys) ||> List.fold (fun acc prop ->
                    match acc with
                    | Error _ -> acc
                    | Ok acc ->
                        match Helpers.getField prop value |> decoder path with
                        | Error er -> Error er
                        | Ok value -> (prop, value)::acc |> Ok)
                |> Result.map List.rev
            | Error e -> Error e

    //////////////////////////////
    // Inconsistent Structure ///
    ////////////////////////////

    let oneOf (decoders : Decoder<'value> list) : Decoder<'value> =
        fun path value ->
            let rec runner (decoders : Decoder<'value> list) (errors : string list) =
                match decoders with
                | head::tail ->
                    match fromValue path head value with
                    | Ok v ->
                        Ok v
                    | Error error -> runner tail (errors @ [error])
                | [] -> (path, BadOneOf errors) |> Error

            runner decoders []

    //////////////////////
    // Fancy decoding ///
    ////////////////////

    let nil (output : 'a) : Decoder<'a> =
        fun path value ->
            if Helpers.isNullValue value then
                Ok output
            else
                (path, BadPrimitive("null", value)) |> Error

    let value _ v = Ok v

    let succeed (output : 'a) : Decoder<'a> =
        fun _ _ ->
            Ok output

    let fail (msg: string) : Decoder<'a> =
        fun path _ ->
            (path, FailMessage msg) |> Error

    let andThen (cb: 'a -> Decoder<'b>) (decoder : Decoder<'a>) : Decoder<'b> =
        fun path value ->
            match decoder path value with
            | Error error -> Error error
            | Ok result -> cb result path value

    let all (decoders: Decoder<'a> list): Decoder<'a list> =
        fun path value ->
            let rec runner (decoders: Decoder<'a> list) (values: 'a list) =
                match decoders with
                | decoder :: tail ->
                    match decoder path value with
                    | Ok value -> runner tail (values @ [ value ])
                    | Error error -> Error error
                | [] -> Ok values

            runner decoders []

    /////////////////////
    // Map functions ///
    ///////////////////

    let map
        (ctor : 'a -> 'value)
        (d1 : Decoder<'a>) : Decoder<'value> =
        fun path value ->
            match d1 path value with
            | Ok v1 -> Ok (ctor v1)
            | Error er -> Error er

    let map2
        (ctor : 'a -> 'b -> 'value)
        (d1 : Decoder<'a>)
        (d2 : Decoder<'b>) : Decoder<'value> =
        fun path value ->
            match d1 path value, d2 path value with
            | Ok v1, Ok v2 -> Ok (ctor v1 v2)
            | Error er,_ -> Error er
            | _,Error er -> Error er

    let map3
        (ctor : 'a -> 'b -> 'c -> 'value)
        (d1 : Decoder<'a>)
        (d2 : Decoder<'b>)
        (d3 : Decoder<'c>) : Decoder<'value> =
        fun path value ->
            match d1 path value, d2 path value, d3 path value with
            | Ok v1, Ok v2, Ok v3 -> Ok (ctor v1 v2 v3)
            | Error er,_,_ -> Error er
            | _,Error er,_ -> Error er
            | _,_,Error er -> Error er

    let map4
        (ctor : 'a -> 'b -> 'c -> 'd -> 'value)
        (d1 : Decoder<'a>)
        (d2 : Decoder<'b>)
        (d3 : Decoder<'c>)
        (d4 : Decoder<'d>) : Decoder<'value> =
        fun path value ->
            match d1 path value, d2 path value, d3 path value, d4 path value with
            | Ok v1, Ok v2, Ok v3, Ok v4 -> Ok (ctor v1 v2 v3 v4)
            | Error er,_,_,_ -> Error er
            | _,Error er,_,_ -> Error er
            | _,_,Error er,_ -> Error er
            | _,_,_,Error er -> Error er

    let map5
        (ctor : 'a -> 'b -> 'c -> 'd -> 'e -> 'value)
        (d1 : Decoder<'a>)
        (d2 : Decoder<'b>)
        (d3 : Decoder<'c>)
        (d4 : Decoder<'d>)
        (d5 : Decoder<'e>) : Decoder<'value> =
        fun path value ->
            match d1 path value, d2 path value, d3 path value, d4 path value, d5 path value with
            | Ok v1, Ok v2, Ok v3, Ok v4, Ok v5 -> Ok (ctor v1 v2 v3 v4 v5)
            | Error er,_,_,_,_ -> Error er
            | _,Error er,_,_,_ -> Error er
            | _,_,Error er,_,_ -> Error er
            | _,_,_,Error er,_ -> Error er
            | _,_,_,_,Error er -> Error er

    let map6
        (ctor : 'a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'value)
        (d1 : Decoder<'a>)
        (d2 : Decoder<'b>)
        (d3 : Decoder<'c>)
        (d4 : Decoder<'d>)
        (d5 : Decoder<'e>)
        (d6 : Decoder<'f>) : Decoder<'value> =
        fun path value ->
            match d1 path value, d2 path value, d3 path value, d4 path value, d5 path value, d6 path value with
            | Ok v1, Ok v2, Ok v3, Ok v4, Ok v5, Ok v6 -> Ok (ctor v1 v2 v3 v4 v5 v6)
            | Error er,_,_,_,_,_ -> Error er
            | _,Error er,_,_,_,_ -> Error er
            | _,_,Error er,_,_,_ -> Error er
            | _,_,_,Error er,_,_ -> Error er
            | _,_,_,_,Error er,_ -> Error er
            | _,_,_,_,_,Error er -> Error er

    let map7
        (ctor : 'a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g -> 'value)
        (d1 : Decoder<'a>)
        (d2 : Decoder<'b>)
        (d3 : Decoder<'c>)
        (d4 : Decoder<'d>)
        (d5 : Decoder<'e>)
        (d6 : Decoder<'f>)
        (d7 : Decoder<'g>) : Decoder<'value> =
        fun path value ->
            match d1 path value, d2 path value, d3 path value, d4 path value, d5 path value, d6 path value, d7 path value with
            | Ok v1, Ok v2, Ok v3, Ok v4, Ok v5, Ok v6, Ok v7 -> Ok (ctor v1 v2 v3 v4 v5 v6 v7)
            | Error er,_,_,_,_,_,_ -> Error er
            | _,Error er,_,_,_,_,_ -> Error er
            | _,_,Error er,_,_,_,_ -> Error er
            | _,_,_,Error er,_,_,_ -> Error er
            | _,_,_,_,Error er,_,_ -> Error er
            | _,_,_,_,_,Error er,_ -> Error er
            | _,_,_,_,_,_,Error er -> Error er

    let map8
        (ctor : 'a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g -> 'h -> 'value)
        (d1 : Decoder<'a>)
        (d2 : Decoder<'b>)
        (d3 : Decoder<'c>)
        (d4 : Decoder<'d>)
        (d5 : Decoder<'e>)
        (d6 : Decoder<'f>)
        (d7 : Decoder<'g>)
        (d8 : Decoder<'h>) : Decoder<'value> =
        fun path value ->
            match d1 path value, d2 path value, d3 path value, d4 path value, d5 path value, d6 path value, d7 path value, d8 path value with
            | Ok v1, Ok v2, Ok v3, Ok v4, Ok v5, Ok v6, Ok v7, Ok v8 -> Ok (ctor v1 v2 v3 v4 v5 v6 v7 v8)
            | Error er,_,_,_,_,_,_,_ -> Error er
            | _,Error er,_,_,_,_,_,_ -> Error er
            | _,_,Error er,_,_,_,_,_ -> Error er
            | _,_,_,Error er,_,_,_,_ -> Error er
            | _,_,_,_,Error er,_,_,_ -> Error er
            | _,_,_,_,_,Error er,_,_ -> Error er
            | _,_,_,_,_,_,Error er,_ -> Error er
            | _,_,_,_,_,_,_,Error er -> Error er

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
        abstract Field : string -> Decoder<'a> -> 'a
        abstract At : List<string> -> Decoder<'a> -> 'a
        abstract Raw : Decoder<'a> -> 'a

    type IOptionalGetter =
        abstract Field : string -> Decoder<'a> -> 'a option
        abstract At : List<string> -> Decoder<'a> -> 'a option
        abstract Raw : Decoder<'a> -> 'a option

    type IGetters =
        abstract Required: IRequiredGetter
        abstract Optional: IOptionalGetter

    let private unwrapWith (errors: ResizeArray<DecoderError>) path (decoder: Decoder<'T>) value: 'T =
        match decoder path value with
        | Ok v -> v
        | Error er -> errors.Add(er); Unchecked.defaultof<'T>

    type Getters<'T>(path: string, v: 'T) =
        let mutable errors = ResizeArray<DecoderError>()
        let required =
            { new IRequiredGetter with
                member __.Field (fieldName : string) (decoder : Decoder<_>) =
                    unwrapWith errors path (field fieldName decoder) v
                member __.At (fieldNames : string list) (decoder : Decoder<_>) =
                    unwrapWith errors path (at fieldNames decoder) v
                member __.Raw (decoder: Decoder<_>) =
                    unwrapWith errors path decoder v }
        let optional =
            { new IOptionalGetter with
                member __.Field (fieldName : string) (decoder : Decoder<_>) =
                    unwrapWith errors path (optional fieldName decoder) v
                member __.At (fieldNames : string list) (decoder : Decoder<_>) =
                    unwrapWith errors path (optionalAt fieldNames decoder) v
                member __.Raw (decoder: Decoder<_>) =
                    match decoder path v with
                    | Ok v -> Some v
                    | Error((_, reason) as error) ->
                        match reason with
                        | BadPrimitive(_,v)
                        | BadPrimitiveExtra(_,v,_)
                        | BadType(_,v) ->
                            if Helpers.isNullValue v then None
                            else errors.Add(error); Unchecked.defaultof<_>
                        | BadField _
                        | BadPath _ -> None
                        | TooSmallArray _
                        | FailMessage _
                        | BadOneOf _ -> errors.Add(error); Unchecked.defaultof<_> }
        member __.Errors: _ list = Seq.toList errors
        interface IGetters with
            member __.Required = required
            member __.Optional = optional

    let object (builder: IGetters -> 'value) : Decoder<'value> =
        fun path v ->
            let getters = Getters(path, v)
            let result = builder getters
            match getters.Errors with
            | [] -> Ok result
            | fst::_ as errors ->
                if errors.Length > 1 then
                    let errors = List.map errorToString errors
                    (path, BadOneOf errors) |> Error
                else
                    Error fst

    ///////////////////////
    // Tuples decoders ///
    ////////////////////

    let tuple2 (decoder1: Decoder<'T1>) (decoder2: Decoder<'T2>) : Decoder<'T1 * 'T2> =
        index 0 decoder1
        |> andThen (fun v1 ->
            index 1 decoder2
            |> andThen (fun v2 ->
                succeed (v1, v2)
            )
        )

    let tuple3 (decoder1: Decoder<'T1>)
               (decoder2: Decoder<'T2>)
               (decoder3: Decoder<'T3>) : Decoder<'T1 * 'T2 * 'T3> =
        index 0 decoder1
        |> andThen (fun v1 ->
            index 1 decoder2
            |> andThen (fun v2 ->
                index 2 decoder3
                |> andThen (fun v3 ->
                    succeed (v1, v2, v3)
                )
            )
        )

    let tuple4 (decoder1: Decoder<'T1>)
               (decoder2: Decoder<'T2>)
               (decoder3: Decoder<'T3>)
               (decoder4: Decoder<'T4>) : Decoder<'T1 * 'T2 * 'T3 * 'T4> =
        index 0 decoder1
        |> andThen (fun v1 ->
            index 1 decoder2
            |> andThen (fun v2 ->
                index 2 decoder3
                |> andThen (fun v3 ->
                    index 3 decoder4
                    |> andThen (fun v4 ->
                        succeed (v1, v2, v3, v4)
                    )
                )
            )
        )

    let tuple5 (decoder1: Decoder<'T1>)
               (decoder2: Decoder<'T2>)
               (decoder3: Decoder<'T3>)
               (decoder4: Decoder<'T4>)
               (decoder5: Decoder<'T5>) : Decoder<'T1 * 'T2 * 'T3 * 'T4 * 'T5> =
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
                            succeed (v1, v2, v3, v4, v5)
                        )
                    )
                )
            )
        )

    let tuple6 (decoder1: Decoder<'T1>)
               (decoder2: Decoder<'T2>)
               (decoder3: Decoder<'T3>)
               (decoder4: Decoder<'T4>)
               (decoder5: Decoder<'T5>)
               (decoder6: Decoder<'T6>) : Decoder<'T1 * 'T2 * 'T3 * 'T4 * 'T5 * 'T6> =
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

    let tuple7 (decoder1: Decoder<'T1>)
               (decoder2: Decoder<'T2>)
               (decoder3: Decoder<'T3>)
               (decoder4: Decoder<'T4>)
               (decoder5: Decoder<'T5>)
               (decoder6: Decoder<'T6>)
               (decoder7: Decoder<'T7>) : Decoder<'T1 * 'T2 * 'T3 * 'T4 * 'T5 * 'T6 * 'T7> =
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

    let tuple8 (decoder1: Decoder<'T1>)
               (decoder2: Decoder<'T2>)
               (decoder3: Decoder<'T3>)
               (decoder4: Decoder<'T4>)
               (decoder5: Decoder<'T5>)
               (decoder6: Decoder<'T6>)
               (decoder7: Decoder<'T7>)
               (decoder8: Decoder<'T8>) : Decoder<'T1 * 'T2 * 'T3 * 'T4 * 'T5 * 'T6 * 'T7 * 'T8> =
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
                                        succeed (v1, v2, v3, v4, v5, v6, v7, v8)
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

    let dict (decoder : Decoder<'value>) : Decoder<Map<string, 'value>> =
        map Map.ofList (keyValuePairs decoder)

    let map' (keyDecoder : Decoder<'key>) (valueDecoder : Decoder<'value>) : Decoder<Map<'key, 'value>> =
        map Map.ofSeq (array (tuple2 keyDecoder valueDecoder))

    ////////////
    // Enum ///
    /////////

    #if !FABLE_REPL_LIB
    module Enum =

        let inline byte<'TEnum when 'TEnum : enum<byte>> : Decoder<'TEnum> =
            byte
            |> andThen (fun value ->
                LanguagePrimitives.EnumOfValue<byte, 'TEnum> value
                |> succeed
            )

        let inline sbyte<'TEnum when 'TEnum : enum<sbyte>> : Decoder<'TEnum> =
            sbyte
            |> andThen (fun value ->
                LanguagePrimitives.EnumOfValue<sbyte, 'TEnum> value
                |> succeed
            )

        let inline int16<'TEnum when 'TEnum : enum<int16>> : Decoder<'TEnum> =
            int16
            |> andThen (fun value ->
                LanguagePrimitives.EnumOfValue<int16, 'TEnum> value
                |> succeed
            )

        let inline uint16<'TEnum when 'TEnum : enum<uint16>> : Decoder<'TEnum> =
            uint16
            |> andThen (fun value ->
                LanguagePrimitives.EnumOfValue<uint16, 'TEnum> value
                |> succeed
            )

        let inline int<'TEnum when 'TEnum : enum<int>> : Decoder<'TEnum> =
            int
            |> andThen (fun value ->
                LanguagePrimitives.EnumOfValue<int, 'TEnum> value
                |> succeed
            )

        let inline uint32<'TEnum when 'TEnum : enum<uint32>> : Decoder<'TEnum> =
            uint32
            |> andThen (fun value ->
                LanguagePrimitives.EnumOfValue<uint32, 'TEnum> value
                |> succeed
            )
    #endif

    //////////////////
    // Reflection ///
    ////////////////

    open FSharp.Reflection

    // As generics are erased by Fable, let's just do an unsafe cast for performance
    let inline boxDecoder (d: Decoder<'T>): BoxedDecoder =
        !!d // d >> Result.map box

    let inline unboxDecoder (d: BoxedDecoder): Decoder<'T> =
        !!d // d >> Result.map unbox

    // This is used to force Fable use a generic comparer for map keys
    let private toMap<'key, 'value when 'key: comparison> (xs: ('key*'value) seq) = Map.ofSeq xs
    let private toSet<'key when 'key: comparison> (xs: 'key seq) = Set.ofSeq xs

    let private autoObject (decoderInfos: (string * BoxedDecoder)[]) (path : string) (value: JsonValue) =
        if not (Helpers.isObject value) then
            (path, BadPrimitive ("an object", value)) |> Error
        else
            (decoderInfos, Ok []) ||> Array.foldBack (fun (name, decoder) acc ->
                match acc with
                | Error _ -> acc
                | Ok result ->
                    Helpers.getField name value
                    |> decoder (path + "." + name)
                    |> Result.map (fun v -> v::result))

    let inline private enumDecoder<'UnderlineType when 'UnderlineType : equality>
        (decoder : Decoder<'UnderlineType>)
        (toString : 'UnderlineType -> string)
        (t: System.Type) =

            fun path value ->
                match decoder path value with
                | Ok enumValue ->
                    System.Enum.GetValues(t)
                    |> Seq.cast<'UnderlineType>
                    |> Seq.contains enumValue
                    |> function
                    | true ->
                        System.Enum.Parse(t, toString enumValue)
                        |> Ok
                    | false ->
                        (path, BadPrimitiveExtra(t.FullName, value, "Unkown value provided for the enum"))
                        |> Error
                | Error msg ->
                    Error msg

    let private autoObject2 (keyDecoder: BoxedDecoder) (valueDecoder: BoxedDecoder) (path : string) (value: JsonValue) =
        if not (Helpers.isObject value) then
            (path, BadPrimitive ("an object", value)) |> Error
        else
            (Ok [], Helpers.objectKeys(value)) ||> Seq.fold (fun acc name ->
                match acc with
                | Error _ -> acc
                | Ok acc ->
                    match keyDecoder path name with
                    | Error er -> Error er
                    | Ok k ->
                        Helpers.getField name value
                        |> valueDecoder (path + "." + name)
                        |> function
                            | Error er -> Error er
                            | Ok v -> (k,v)::acc |> Ok)

    let private mixedArray offset (decoders: BoxedDecoder[]) (path: string) (values: JsonValue[]): Result<JsonValue list, DecoderError> =
        let expectedLength = decoders.Length + offset
        if expectedLength <> values.Length then
            (path, sprintf "Expected array of length %i but got %i" expectedLength values.Length
            |> FailMessage) |> Error
        else
            let mutable result = Ok []
            for i = offset to values.Length - 1 do
                match result with
                | Error _ -> ()
                | Ok acc ->
                    let path = sprintf "%s[%i]" path i
                    let decoder = decoders.[i - offset]
                    let value = values.[i]
                    result <- decoder path value |> Result.map (fun v -> v::acc)
            result
            |> Result.map List.rev

    let rec private makeUnion extra caseStrategy t name (path : string) (values: JsonValue[]) =
        let uci =
            FSharpType.GetUnionCases(t, allowAccessToPrivateRepresentation=true)
            |> Array.tryFind (fun x -> x.Name = name)
        match uci with
        | None -> (path, FailMessage("Cannot find case " + name + " in " + t.FullName)) |> Error
        | Some uci ->
            let decoders = uci.GetFields() |> Array.map (fun fi -> autoDecoder extra caseStrategy false fi.PropertyType)
            let values =
                if decoders.Length = 0 && values.Length <= 1 // First item is the case name
                then Ok []
                else mixedArray 1 decoders path values
            values |> Result.map (fun values -> FSharpValue.MakeUnion(uci, List.toArray values, allowAccessToPrivateRepresentation=true))

    and private autoDecodeRecordsAndUnions extra (caseStrategy : CaseStrategy) (isOptional : bool) (t: System.Type) : BoxedDecoder =
        // Add the decoder to extra in case one of the fields is recursive
        let decoderRef = ref Unchecked.defaultof<_>
        let extra =
            // As of 3.7.17 Fable assigns empty name to anonymous record, we shouldn't add them to the map to avoid conflicts.
            // Anonymous records cannot be recursive anyways, see #144
            match t.FullName with
            | "" -> extra
            | fullName -> extra |> Map.add fullName decoderRef
        let decoder =
            if FSharpType.IsRecord(t, allowAccessToPrivateRepresentation=true) then
                let decoders =
                    FSharpType.GetRecordFields(t, allowAccessToPrivateRepresentation=true)
                    |> Array.map (fun fi ->
                        let name = Util.Casing.convert caseStrategy fi.Name
                        name, autoDecoder extra caseStrategy false fi.PropertyType)
                fun path value ->
                    autoObject decoders path value
                    |> Result.map (fun xs -> FSharpValue.MakeRecord(t, List.toArray xs, allowAccessToPrivateRepresentation=true))

            elif FSharpType.IsUnion(t, allowAccessToPrivateRepresentation=true) then
                fun path (value: JsonValue) ->
                    if Helpers.isString(value) then
                        let name = Helpers.asString value
                        makeUnion extra caseStrategy t name path [||]
                    elif Helpers.isArray(value) then
                        let values = Helpers.asArray value
                        string (path + "[0]") values.[0]
                        |> Result.bind (fun name -> makeUnion extra caseStrategy t name path values)
                    else (path, BadPrimitive("a string or array", value)) |> Error

            else
                if isOptional then
                    // The error will only happen at runtime if the value is not null
                    // See https://github.com/MangelMaxime/Thoth/pull/84#issuecomment-444837773
                    boxDecoder(fun path value -> Error(path, BadType("an extra coder for " + t.FullName, value)))
                else
                    // Don't use failwithf here, for some reason F#/Fable compiles it as a function
                    // when the return type is a function too, so it doesn't fail immediately
                    sprintf """Cannot generate auto decoder for %s. Please pass an extra decoder.

Documentation available at: https://thoth-org.github.io/Thoth.Json/documentation/auto/extra-coders.html#ready-to-use-extra-coders""" t.FullName |> failwith
        decoderRef.Value <- decoder
        decoder

    and private autoDecoder (extra: Map<string, ref<BoxedDecoder>>) caseStrategy (isOptional : bool) (t: System.Type) : BoxedDecoder =
      let fullname = t.FullName
      match Map.tryFind fullname extra with
      | Some decoderRef -> fun path value -> decoderRef.contents path value
      | None ->
        if t.IsArray then
            let decoder = t.GetElementType() |> autoDecoder extra caseStrategy false
            array decoder |> boxDecoder
        elif t.IsEnum then
            let enumType = System.Enum.GetUnderlyingType(t).FullName
            if enumType = typeof<sbyte>.FullName then
                enumDecoder<sbyte> sbyte Operators.string t |> boxDecoder
            elif enumType = typeof<byte>.FullName then
                enumDecoder<byte> byte Operators.string t |> boxDecoder
            elif enumType = typeof<int16>.FullName then
                enumDecoder<int16> int16 Operators.string t |> boxDecoder
            elif enumType = typeof<uint16>.FullName then
                enumDecoder<uint16> uint16 Operators.string t |> boxDecoder
            elif enumType = typeof<int>.FullName then
                enumDecoder<int> int Operators.string t |> boxDecoder
            elif enumType = typeof<uint32>.FullName then
                enumDecoder<uint32> uint32 Operators.string t |> boxDecoder
            else
                failwithf
                    """Cannot generate auto decoder for %s.
Thoth.Json.Net only support the following enum types:
- sbyte
- byte
- int16
- uint16
- int
- uint32
If you can't use one of these types, please pass an extra decoder.
                    """ t.FullName
        elif t.IsGenericType then
            if FSharpType.IsTuple(t) then
                let decoders = FSharpType.GetTupleElements(t) |> Array.map (autoDecoder extra caseStrategy false)
                fun path value ->
                    if Helpers.isArray value then
                        mixedArray 0 decoders path (Helpers.asArray value)
                        |> Result.map (fun xs -> FSharpValue.MakeTuple(List.toArray xs, t))
                    else (path, BadPrimitive ("an array", value)) |> Error
            else
                let fullname = t.GetGenericTypeDefinition().FullName
                if fullname = typedefof<obj option>.FullName then
                    t.GenericTypeArguments.[0] |> (autoDecoder extra caseStrategy true) |> option |> boxDecoder
                elif fullname = typedefof<obj list>.FullName then
                    t.GenericTypeArguments.[0] |> (autoDecoder extra caseStrategy false) |> list |> boxDecoder
                elif fullname = typedefof<obj seq>.FullName then
                    t.GenericTypeArguments.[0] |> (autoDecoder extra caseStrategy false) |> seq |> boxDecoder
                elif fullname = typedefof< Map<System.IComparable, obj> >.FullName then
                    let keyDecoder = t.GenericTypeArguments.[0] |> autoDecoder extra caseStrategy false
                    let valueDecoder = t.GenericTypeArguments.[1] |> autoDecoder extra caseStrategy false
                    oneOf [
                        autoObject2 keyDecoder valueDecoder
                        list (tuple2 keyDecoder valueDecoder)
                    ] |> map (fun ar -> toMap (unbox ar) |> box)
                elif fullname = typedefof< Set<string> >.FullName then
                    let decoder = t.GenericTypeArguments.[0] |> autoDecoder extra caseStrategy false
                    fun path value ->
                        match array decoder path value with
                        | Error er -> Error er
                        | Ok ar -> toSet (unbox ar) |> box |> Ok
                else
                    autoDecodeRecordsAndUnions extra caseStrategy isOptional t
        else
            if fullname = typeof<bool>.FullName then
                boxDecoder bool
            elif fullname = typedefof<unit>.FullName then
                boxDecoder unit
            elif fullname = typeof<string>.FullName then
                boxDecoder string
            elif fullname = typeof<char>.FullName then
                boxDecoder char
            elif fullname = typeof<sbyte>.FullName then
                boxDecoder sbyte
            elif fullname = typeof<byte>.FullName then
                boxDecoder byte
            elif fullname = typeof<int16>.FullName then
                boxDecoder int16
            elif fullname = typeof<uint16>.FullName then
                boxDecoder uint16
            elif fullname = typeof<int>.FullName then
                boxDecoder int
            elif fullname = typeof<uint32>.FullName then
                boxDecoder uint32
            elif fullname = typeof<float>.FullName then
                boxDecoder float
            elif fullname = typeof<float32>.FullName then
                boxDecoder float32
            // These number types require extra libraries in Fable. To prevent penalizing
            // all users, extra decoders (withInt64, etc) must be passed when they're needed.

            // elif fullname = typeof<int64>.FullName then
            //     boxDecoder int64
            // elif fullname = typeof<uint64>.FullName then
            //     boxDecoder uint64
            // elif fullname = typeof<bigint>.FullName then
            //     boxDecoder bigint
            // elif fullname = typeof<decimal>.FullName then
            //     boxDecoder decimal
            elif fullname = typeof<System.DateTime>.FullName then
                boxDecoder datetimeUtc
            elif fullname = typeof<System.DateTimeOffset>.FullName then
                boxDecoder datetimeOffset
            elif fullname = typeof<System.TimeSpan>.FullName then
                boxDecoder timespan
            elif fullname = typeof<System.Guid>.FullName then
                boxDecoder guid
            elif fullname = typeof<obj>.FullName then
                fun _ v -> Ok v
            else autoDecodeRecordsAndUnions extra caseStrategy isOptional t

    let private makeExtra (extra: ExtraCoders option) =
        match extra with
        | None -> Map.empty
        | Some e -> Map.map (fun _ (_,dec) -> ref dec) e.Coders

    type Auto =
        static member generateBoxedDecoderCached(t: System.Type, ?caseStrategy : CaseStrategy, ?extra: ExtraCoders): BoxedDecoder =
            let caseStrategy = defaultArg caseStrategy PascalCase

            let key =
                t.FullName
                |> (+) (Operators.string caseStrategy)
                |> (+) (extra |> Option.map (fun e -> e.Hash) |> Option.defaultValue "")

            Util.CachedDecoders.GetOrAdd(key, fun _ -> autoDecoder (makeExtra extra) caseStrategy false t)

        static member inline generateDecoderCached<'T>(?caseStrategy : CaseStrategy, ?extra: ExtraCoders): Decoder<'T> =
            Auto.generateBoxedDecoderCached(typeof<'T>, ?caseStrategy=caseStrategy, ?extra=extra) |> unboxDecoder

        static member generateBoxedDecoder(t: System.Type, ?caseStrategy : CaseStrategy, ?extra: ExtraCoders): BoxedDecoder =
            let caseStrategy = defaultArg caseStrategy PascalCase
            autoDecoder (makeExtra extra) caseStrategy false t

        static member inline generateDecoder<'T>(?caseStrategy : CaseStrategy, ?extra: ExtraCoders): Decoder<'T> =
            Auto.generateBoxedDecoder(typeof<'T>, ?caseStrategy=caseStrategy, ?extra=extra) |> unboxDecoder

        static member inline fromString<'T>(json: string, ?caseStrategy : CaseStrategy, ?extra: ExtraCoders): Result<'T, string> =
            let decoder = Auto.generateDecoder<'T>(?caseStrategy=caseStrategy, ?extra=extra)
            fromString decoder json

        static member inline unsafeFromString<'T>(json: string, ?caseStrategy : CaseStrategy, ?extra: ExtraCoders): 'T =
            let decoder = Auto.generateDecoder<'T>(?caseStrategy=caseStrategy, ?extra=extra)
            match fromString decoder json with
            | Ok x -> x
            | Error msg -> failwith msg
