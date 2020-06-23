#if THOTH_JSON_FABLE
namespace Thoth.Json.Fable
#endif

#if THOTH_JSON_NEWTONSOFT
namespace Thoth.Json.Newtonsoft
#endif

[<RequireQualifiedAccess>]
module Decode =

    //////////////////
    // Primitives ///
    ////////////////

    let string : Decoder<string> =
        fun value ->
            if Helpers.isString value then
                Ok(Helpers.asString value)
            else
                ("", BadPrimitive("a string", value)) |> Error

    let guid : Decoder<System.Guid> =
        fun value ->
            if Helpers.isString value then
                match System.Guid.TryParse (Helpers.asString value) with
                | true, x -> Ok x
                | _ -> ("", BadPrimitive("a guid", value)) |> Error
            else ("", BadPrimitive("a guid", value)) |> Error

    let unit : Decoder<unit> =
        fun value ->
            if Helpers.isNullValue value then
                Ok ()
            else
                ("", BadPrimitive("null", value)) |> Error

    let inline private integral
                    (name : string)
                    (tryParse : (string -> bool * 'T))
                    (min : 'T)
                    (max : 'T)
                    (conv : float -> 'T) : Decoder<'T > =

        fun value ->
            if Helpers.isNumber value then
                if Helpers.isIntegralValue value then
                    let fValue = Helpers.asFloat value
                    if (float min) <= fValue && fValue <= (float max) then
                        Ok(conv fValue)
                    else
                        ("", BadPrimitiveExtra(name, value, "Value was either too large or too small for " + name)) |> Error
                else
                    ("", BadPrimitiveExtra(name, value, "Value is not an integral value")) |> Error
            elif Helpers.isString value then
                match tryParse (Helpers.asString value) with
                | true, x -> Ok x
                | _ -> ("", BadPrimitive(name, value)) |> Error
            else
                ("", BadPrimitive(name, value)) |> Error

    let sbyte<'JsonValue> : Decoder<sbyte> =
        integral
            "a sbyte"
            System.SByte.TryParse
            System.SByte.MinValue
            System.SByte.MaxValue
            sbyte

    /// Alias to Decode.uint8
    let byte<'JsonValue> : Decoder<byte> =
        integral
            "a byte"
            System.Byte.TryParse
            System.Byte.MinValue
            System.Byte.MaxValue
            byte

    let int16<'JsonValue> : Decoder<int16> =
        integral
            "an int16"
            System.Int16.TryParse
            System.Int16.MinValue
            System.Int16.MaxValue
            int16

    let uint16<'JsonValue> : Decoder<uint16> =
        integral
            "an uint16"
            System.UInt16.TryParse
            System.UInt16.MinValue
            System.UInt16.MaxValue
            uint16

    let int<'JsonValue> : Decoder<int> =
        integral
            "an int"
            System.Int32.TryParse
            System.Int32.MinValue
            System.Int32.MaxValue
            int

    let uint32<'JsonValue> : Decoder<uint32> =
        integral
            "an uint32"
            System.UInt32.TryParse
            System.UInt32.MinValue
            System.UInt32.MaxValue
            uint32

    let int64<'JsonValue> : Decoder<int64> =
        integral
            "an int64"
            System.Int64.TryParse
            System.Int64.MinValue
            System.Int64.MaxValue
            int64

    let uint64<'JsonValue> : Decoder<uint64> =
        integral
            "an uint64"
            System.UInt64.TryParse
            System.UInt64.MinValue
            System.UInt64.MaxValue
            uint64

    let bigint : Decoder<bigint> =
        fun value ->
            if Helpers.isNumber value then
                Helpers.asInt value |> bigint |> Ok
            elif Helpers.isString value then
                // TODO: BigInt.TryParse has been added in Fable 2.1.4
                // Don't use it for now to support lower Fable versions
                try
                    bigint.Parse (Helpers.asString value) |> Ok
                with _ ->
                    ("", BadPrimitive("a bigint", value)) |> Error
            else
                ("", BadPrimitive("a bigint", value)) |> Error

    let bool : Decoder<bool> =
        fun value ->
            if Helpers.isBoolean value then
                Ok(Helpers.asBool value)
            else
                ("", BadPrimitive("a boolean", value)) |> Error

    let float : Decoder<float> =
        fun value ->
            if Helpers.isNumber value then
                Ok(Helpers.asFloat value)
            else
                ("", BadPrimitive("a float", value)) |> Error

    let float32 : Decoder<float32> =
        fun value ->
            if Helpers.isNumber value then
                Ok(Helpers.asFloat32 value)
            else
                ("", BadPrimitive("a float32", value)) |> Error

    let decimal : Decoder<decimal> =
        fun value ->
            if Helpers.isNumber value then
                Helpers.asFloat value |> decimal |> Ok
            elif Helpers.isString value then
                match System.Decimal.TryParse (Helpers.asString value) with
                | true, x -> Ok x
                | _ -> ("", BadPrimitive("a decimal", value)) |> Error
            else
                ("", BadPrimitive("a decimal", value)) |> Error

    let datetime : Decoder<System.DateTime> =
        fun value ->
            if Helpers.isString value then
                match System.DateTime.TryParse (Helpers.asString value) with
                | true, x -> x.ToUniversalTime() |> Ok
                | _ -> ("", BadPrimitive("a datetime", value)) |> Error
            else
                ("", BadPrimitive("a datetime", value)) |> Error

    let datetimeOffset : Decoder<System.DateTimeOffset> =
        fun value ->
            if Helpers.isString value then
                match System.DateTimeOffset.TryParse(Helpers.asString value) with
                | true, x -> Ok x
                | _ -> ("", BadPrimitive("a datetimeoffset", value)) |> Error
            else
                ("", BadPrimitive("a datetime", value)) |> Error

    let timespan : Decoder<System.TimeSpan> =
        fun value ->
            if Helpers.isString value then
                match System.TimeSpan.TryParse(Helpers.asString value) with
                | true, x -> Ok x
                | _ -> ("", BadPrimitive("a timespan", value)) |> Error
            else
                ("", BadPrimitive("a timespan", value)) |> Error

    /////////////////////////
    // Object primitives ///
    ///////////////////////

    let optional (fieldName : string) (decoder : Decoder<'value>) : Decoder<'value option> =
        fun value ->
            if Helpers.isObject value then
                let fieldValue = Helpers.getField fieldName value
                if Helpers.isUndefined fieldValue then Ok None
                else
                    // The decoder may be an option decoder so give it an opportunity to check null values
                    match decoder fieldValue with
                    | Ok v -> Ok (Some v)
                    | Error _ when Helpers.isNullValue fieldValue -> Ok None
                    | Error er ->
                        Error(er |> DecoderError.prependPath ("." + fieldName))
            else
                Error("", BadType("an object", value))

    let private badPathError fieldNames currentPath value =
        // IMPORTANT: The empty string is normal, this is to prefix by "." the generated path
        let currentPath = defaultArg currentPath (""::fieldNames |> String.concat ".")
        let msg = "an object with path `" + (String.concat "." fieldNames) + "`"
        Error(currentPath, BadPath (msg, value, List.tryLast fieldNames |> Option.defaultValue ""))

    let optionalAt (fieldNames : string list) (decoder : Decoder<'value>) : Decoder<'value option> =
        fun firstValue ->
            (("", firstValue, None), fieldNames)
            ||> List.fold (fun (curPath, curValue, res) field ->
                match res with
                | Some _ -> curPath, curValue, res
                | None ->
                    if Helpers.isNullValue curValue then
                        let res = badPathError fieldNames (Some curPath) firstValue
                        curPath, curValue, Some res
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
                    else
                        // The decoder may be an option decoder so give it an opportunity to check null values
                        match decoder lastValue with
                        | Ok v -> Ok (Some v)
                        | Error _ when Helpers.isNullValue lastValue -> Ok None
                        | Error er ->
                            Error(er |> DecoderError.prependPath lastPath)

    let field (fieldName: string) (decoder : Decoder<'value>) : Decoder<'value> =
        fun value ->
            if Helpers.isObject value then
                let fieldValue = Helpers.getField fieldName value
                if Helpers.isUndefined fieldValue then
                    Error("", BadField ("an object with a field named `" + fieldName + "`", value))
                else
                    match decoder fieldValue with
                    | Ok _ as ok -> ok
                    | Error er ->
                        Error(er |> DecoderError.prependPath ("." + fieldName))
            else
                Error("", BadType("an object", value))

    let at (fieldNames: string list) (decoder : Decoder<'value>) : Decoder<'value> =
        fun firstValue ->
            (("", firstValue, None), fieldNames)
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
                    match decoder lastValue with
                    | Ok _ as ok -> ok
                    | Error er ->
                        Error(er |> DecoderError.prependPath lastPath)

    let index (requestedIndex: int) (decoder : Decoder<'value>) : Decoder<'value> =
        fun value ->
            if Helpers.isArray value then
                let vArray = Helpers.asArray value
                if requestedIndex < vArray.Length then
                    decoder (vArray.[requestedIndex])
                    |> DecoderError.prependPathToResult (".[" + (Operators.string requestedIndex) + "]")
                else
                    let msg =
                        "a longer array. Need index `"
                            + (requestedIndex.ToString())
                            + "` but there are only `"
                            + (vArray.Length.ToString())
                            + "` entries"

                    ("", TooSmallArray(msg, value))
                    |> Error
                    |> DecoderError.prependPathToResult (".[" + (Operators.string requestedIndex) + "]")
            else
                ("", BadPrimitive("an array", value))
                |> Error
                |> DecoderError.prependPathToResult (".[" + (Operators.string requestedIndex) + "]")

    let option (decoder : Decoder<'value>) : Decoder<'value option> =
        fun value ->
            if Helpers.isNullValue value then Ok None
            else decoder value |> Result.map Some

    //////////////////////
    // Data structure ///
    ////////////////////

    let private arrayWith expectedMsg (mapping: 'value[] -> 'result) (decoder : Decoder<'value>) : Decoder<'result> =
        fun value ->
            if Helpers.isArray value then
                let mutable i = -1
                let tokens = Helpers.asArray value
                let result = Array.zeroCreate tokens.Length
                let mutable error : DecoderError option = None

                while i < tokens.Length - 1 && error.IsNone do
                    i <- i + 1
                    match decoder tokens.[i] with
                    | Ok value ->
                        result.[i] <- value

                    | Error err ->
                        error <- Some (err |> DecoderError.prependPath (".[" + (i.ToString()) + "]"))

                if error.IsNone then
                    Ok (result |> mapping)
                else
                    Error error.Value

            else
                ("", BadPrimitive (expectedMsg, value))
                |> Error

    let list (decoder : Decoder<'value>) : Decoder<'value list> =
        arrayWith "a list" List.ofArray decoder

    let seq (decoder : Decoder<'value>) : Decoder<'value seq> =
        arrayWith "a seq" Seq.ofArray decoder

    let array (decoder : Decoder<'value>) : Decoder<'value array> =
        arrayWith "an array" id decoder

    let keys: Decoder<string list> =
        fun value ->
            if Helpers.isObject value then
                Helpers.objectKeys value |> List.ofSeq |> Ok
            else
                ("", BadPrimitive ("an object", value))
                |> Error

    let keyValuePairs (decoder : Decoder<'value>) : Decoder<(string * 'value) list> =
        fun value ->
            match keys value with
            | Ok objectKeys ->
                (Ok [], objectKeys) ||> List.fold (fun acc prop ->
                    match acc with
                    | Error _ -> acc
                    | Ok acc ->
                        match Helpers.getField prop value |> decoder with
                        | Error er -> Error (er |> DecoderError.prependPath ( "." + prop))
                        | Ok value -> (prop, value)::acc |> Ok)
                |> Result.map List.rev
            | Error e -> Error e

    //////////////////////////////
    // Inconsistent Structure ///
    ////////////////////////////

    let oneOf (decoders : Decoder<'value> list) : Decoder<'value> =
        fun value ->
            let rec runner (decoders : Decoder<'value> list) (errors : DecoderError list) =
                match decoders with
                | head::tail ->
                    match head value with
                    | Ok v ->
                        Ok v
                    | Error error ->
                        runner tail (errors @ [error])
                | [] -> ("", BadOneOf errors) |> Error

            runner decoders []

    //////////////////////
    // Fancy decoding ///
    ////////////////////

    let nil (output : 'a) : Decoder<'a> =
        fun value ->
            if Helpers.isNullValue value then
                Ok output
            else
                ("", BadPrimitive("null", value)) |> Error

    let value _ v = Ok v

    let succeed (output : 'a) : Decoder<'a> =
        fun _ ->
            Ok output

    let fail (msg: string) : Decoder<'a> =
        fun _ ->
            ("", FailMessage msg) |> Error

    let andThen<'JsonValue, 'a, 'b> (cb: 'a -> Decoder<'b>) (decoder : Decoder<'a>) : Decoder<'b> =
        fun value ->
            match decoder value with
            | Error error -> Error error
            | Ok result -> cb result value

    let all (decoders: Decoder<'a> list): Decoder<'a list> =
        fun value ->
            let rec runner (decoders: Decoder<'a> list) (values: 'a list) =
                match decoders with
                | decoder :: tail ->
                    match decoder value with
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
        fun value ->
            match d1  value with
            | Ok v1 -> Ok (ctor v1)
            | Error er -> Error er

    let map2
        (ctor : 'a -> 'b -> 'value)
        (d1 : Decoder<'a>)
        (d2 : Decoder<'b>) : Decoder<'value> =
        fun value ->
            match d1 value, d2 value with
            | Ok v1, Ok v2 -> Ok (ctor v1 v2)
            | Error er,_ -> Error er
            | _,Error er -> Error er

    let map3
        (ctor : 'a -> 'b -> 'c -> 'value)
        (d1 : Decoder<'a>)
        (d2 : Decoder<'b>)
        (d3 : Decoder<'c>) : Decoder<'value> =
        fun value ->
            match d1 value, d2 value, d3 value with
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
        fun value ->
            match d1 value, d2 value, d3 value, d4 value with
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
        fun value ->
            match d1 value, d2 value, d3 value, d4 value, d5 value with
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
        fun value ->
            match d1 value, d2 value, d3 value, d4 value, d5 value, d6 value with
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
        fun value ->
            match d1 value, d2 value, d3 value, d4 value, d5 value, d6 value, d7 value with
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
        fun value ->
            match d1 value, d2 value, d3 value, d4 value, d5 value, d6 value, d7 value, d8 value with
            | Ok v1, Ok v2, Ok v3, Ok v4, Ok v5, Ok v6, Ok v7, Ok v8 -> Ok (ctor v1 v2 v3 v4 v5 v6 v7 v8)
            | Error er,_,_,_,_,_,_,_ -> Error er
            | _,Error er,_,_,_,_,_,_ -> Error er
            | _,_,Error er,_,_,_,_,_ -> Error er
            | _,_,_,Error er,_,_,_,_ -> Error er
            | _,_,_,_,Error er,_,_,_ -> Error er
            | _,_,_,_,_,Error er,_,_ -> Error er
            | _,_,_,_,_,_,Error er,_ -> Error er
            | _,_,_,_,_,_,_,Error er -> Error er

    let dict<'JsonValue, 'value> (decoder : Decoder<'value>) : Decoder<Map<string, 'value>> =
        map Map.ofList (keyValuePairs decoder)

    //////////////////////
    // Object builder ///
    ////////////////////

    type IRequiredGetter =
        abstract Field : string -> Decoder<'T> -> 'T
        abstract At : List<string> -> Decoder<'T> -> 'T
        abstract Raw : Decoder<'T> -> 'T

    type IOptionalGetter =
        abstract Field : string -> Decoder<'T> -> 'T option
        abstract At : List<string> -> Decoder<'T> -> 'T option
        abstract Raw : Decoder<'T> -> 'T option

    type IGetters =
        abstract Required: IRequiredGetter
        abstract Optional: IOptionalGetter

    let private unwrapWith (errors: ResizeArray<DecoderError>) (decoder: Decoder<'T>) (value: JsonValue) : 'T =
        match decoder value with
        | Ok v -> v
        | Error er ->
            er |> errors.Add
            Unchecked.defaultof<'T>

    type Getters<'T>(v: JsonValue) =
        let mutable errors = ResizeArray<DecoderError>()
        let required =
            { new IRequiredGetter with
                member __.Field (fieldName : string) (decoder : Decoder<_>) =
                    unwrapWith errors (field fieldName decoder) v
                member __.At (fieldNames : string list) (decoder : Decoder<_>) =
                    unwrapWith errors (at fieldNames decoder) v
                member __.Raw (decoder: Decoder<_>) =
                    unwrapWith errors decoder v }
        let optional =
            { new IOptionalGetter with
                member __.Field (fieldName : string) (decoder : Decoder<_>) =
                    unwrapWith errors (optional fieldName decoder) v
                member __.At (fieldNames : string list) (decoder : Decoder<_>) =
                    unwrapWith errors (optionalAt fieldNames decoder) v
                member __.Raw (decoder: Decoder<_>) =
                    match decoder v with
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

    let object (builder: IGetters -> 'Value) : Decoder<'Value> =
        fun v ->
            let getters = Getters(v)
            let result = builder getters
            match getters.Errors with
            | [] -> Ok result
            | fst::_ as errors ->
                if errors.Length > 1 then
                    // let errors =
                    //     errors
                    //     |> List.map ((DecoderError.prependPath "$") >> DecoderError.errorToStringWithPath)

                    ("", BadOneOf errors) |> Error
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

    ////////////
    // Enum ///
    /////////

    #if !FABLE_REPL_LIB
    module Enum =

        #if FABLE_COMPILER
        let inline byte<'JsonValue, 'TEnum when 'TEnum : enum<byte>> : Decoder<'TEnum> =
        #else
        let byte<'JsonValue, 'TEnum when 'TEnum : enum<byte>> : Decoder<'TEnum> =
        #endif
            byte
            |> andThen (fun value ->
                LanguagePrimitives.EnumOfValue<byte, 'TEnum> value
                |> succeed
            )

        #if FABLE_COMPILER
        let inline sbyte<'JsonValue, 'TEnum when 'TEnum : enum<sbyte>> : Decoder<'TEnum> =
        #else
        let sbyte<'JsonValue, 'TEnum when 'TEnum : enum<sbyte>> : Decoder<'TEnum> =
        #endif
            sbyte
            |> andThen (fun value ->
                LanguagePrimitives.EnumOfValue<sbyte, 'TEnum> value
                |> succeed
            )

        #if FABLE_COMPILER
        let inline int16<'JsonValue, 'TEnum when 'TEnum : enum<int16>> : Decoder<'TEnum> =
        #else
        let int16<'JsonValue, 'TEnum when 'TEnum : enum<int16>> : Decoder<'TEnum> =
        #endif
            int16
            |> andThen (fun value ->
                LanguagePrimitives.EnumOfValue<int16, 'TEnum> value
                |> succeed
            )

        #if FABLE_COMPILER
        let inline uint16<'JsonValue, 'TEnum when 'TEnum : enum<uint16>> : Decoder<'TEnum> =
        #else
        let uint16<'JsonValue, 'TEnum when 'TEnum : enum<uint16>> : Decoder<'TEnum> =
        #endif
            uint16
            |> andThen (fun value ->
                LanguagePrimitives.EnumOfValue<uint16, 'TEnum> value
                |> succeed
            )

        #if FABLE_COMPILER
        let inline int<'JsonValue, 'TEnum when 'TEnum : enum<int>> : Decoder<'TEnum> =
        #else
        let int<'JsonValue, 'TEnum when 'TEnum : enum<int>> : Decoder<'TEnum> =
        #endif
            int
            |> andThen (fun value ->
                LanguagePrimitives.EnumOfValue<int, 'TEnum> value
                |> succeed
            )

        #if FABLE_COMPILER
        let inline uint32<'JsonValue, 'TEnum when 'TEnum : enum<uint32>> : Decoder<'TEnum> =
        #else
        let uint32<'JsonValue, 'TEnum when 'TEnum : enum<uint32>> : Decoder<'TEnum> =
        #endif
            uint32
            |> andThen (fun value ->
                LanguagePrimitives.EnumOfValue<uint32, 'TEnum> value
                |> succeed
            )
    #endif
