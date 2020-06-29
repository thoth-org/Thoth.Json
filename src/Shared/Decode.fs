#if THOTH_JSON_FABLE
namespace Thoth.Json.Fable
#endif

#if THOTH_JSON_NEWTONSOFT
namespace Thoth.Json.Newtonsoft

open Newtonsoft.Json.Linq
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

    //////////////////
    // Reflection ///
    ////////////////

    open FSharp.Reflection

    type private DecoderCrate<'T>(decoder : Decoder<'T>) =
        inherit BoxedDecoder()

        override __.Decode(value) =
            match decoder value with
            | Ok value -> Ok(box value)
            | Error error -> Error error

        member __.UnboxedDecoder = decoder

    let boxDecoder (decoder : Decoder<'T>) : BoxedDecoder =
        DecoderCrate(decoder) :> BoxedDecoder

    let unboxDecoder<'T> (decoder : BoxedDecoder) : Decoder<'T> =
        (decoder :?> DecoderCrate<'T>).UnboxedDecoder

    let private autoObject (decoderInfos : (string * BoxedDecoder) []) (value : JsonValue) =
        if (not (Helpers.isObject value)) then
            ("", BadPrimitive ("an object", value)) |> Error
        else
            (decoderInfos, Ok [])
            ||> Array.foldBack (fun (name : string, decoder : BoxedDecoder) acc ->
                match acc with
                | Error _ -> acc // Todo path construction here???
                | Ok result ->
                    Helpers.getField name value
                    |> decoder.BoxedDecoder
                    |> Result.map (fun value ->
                        value::result
                    )
            )

    let mixedArray (msg : string)
                    (decoders : BoxedDecoder[])
                    (values : JsonValue[]) : Result<obj list, DecoderError> =
        if decoders.Length <> values.Length then
            ("", sprintf "Expected %i %s but got %i" decoders.Length msg values.Length |> FailMessage)
            |> Error
        else
            (values, decoders, Ok [])
            |||> Array.foldBack2 (fun value decoder acc ->
                match acc with
                | Error _ -> acc
                | Ok result ->
                    decoder.Decode value
                    |> Result.map (fun value -> value::result)
            )

    let inline private enumDecoder<'UnderlineType when 'UnderlineType : equality>
        (decoder : Decoder<'UnderlineType>)
        (toString : 'UnderlineType -> string)
        (t : System.Type) =
            fun value ->
                match decoder value with
                | Ok enumValue ->
                    System.Enum.GetValues(t)
                    |> Seq.cast<'UnderlineType>
                    |> Seq.contains enumValue
                    |> function
                    | true ->
                        System.Enum.Parse(t, toString enumValue)
                        |> Ok
                    | false ->
                        ("", BadPrimitiveExtra(t.FullName, value, "Unknown value provided for the enum"))
                        |> Error
                | Error message ->
                    Error message

    let private genericOption t (decoder: BoxedDecoder) =
        let ucis = FSharpType.GetUnionCases(t)
        fun (value: JsonValue) ->
            if Helpers.isNullValue value then
                Ok (FSharpValue.MakeUnion(ucis.[0], [||]))
            else
                decoder.Decode(value)
                |> Result.map (fun value ->
                    FSharpValue.MakeUnion(ucis.[1], [| value |])
                )

    let private genericList t (decoder: BoxedDecoder) =
        fun (value: JsonValue) ->
            if not (Helpers.isArray value) then
                ("", BadPrimitive ("a list", value)) |> Error
            else
                let values = value.Value<JArray>()
                let ucis = FSharpType.GetUnionCases(t, allowAccessToPrivateRepresentation=true)
                let empty = FSharpValue.MakeUnion(ucis.[0], [||], allowAccessToPrivateRepresentation=true)
                (values, Ok empty)
                ||> Seq.foldBack (fun value acc ->
                    match acc with
                    | Error _ -> acc
                    | Ok acc ->
                        match decoder.Decode(value) with
                        | Error er ->
                            Error er

                        | Ok result ->
                            FSharpValue.MakeUnion(ucis.[1], [|result; acc|], allowAccessToPrivateRepresentation=true)
                            |> Ok
                    )

    let private (|StringifiableType|_|) (t: System.Type): (string->obj) option =
        let fullName = t.FullName
        if fullName = typeof<string>.FullName then
            Some box
        elif fullName = typeof<System.Guid>.FullName then
            let ofString = t.GetConstructor([|typeof<string>|])
            Some(fun (v: string) -> ofString.Invoke([|v|]))
        else None

    let rec inline private handleArray (extra : Map<string, ref<BoxedDecoder>>)
                                    (caseStrategy : CaseStrategy)
                                    (t : System.Type) : BoxedDecoder =
        let elementType = t.GetElementType()
        let decoder = autoDecoder extra caseStrategy false elementType
        boxDecoder(fun value ->
            match array decoder.BoxedDecoder value with
            | Ok items ->
                let result = System.Array.CreateInstance(elementType, items.Length)
                for i = 0 to result.Length - 1 do
                    result.SetValue(items.[i], i)
                Ok result
            | Error error -> Error error
        )

    and private genericMap extra isCamelCase (t: System.Type) =
        let keyType   = t.GenericTypeArguments.[0]
        let valueType = t.GenericTypeArguments.[1]
        let valueDecoder = autoDecoder extra isCamelCase false valueType
        let keyDecoder = autoDecoder extra isCamelCase false keyType
        let tupleType = typedefof<obj * obj>.MakeGenericType([|keyType; valueType|])
        let listType = typedefof< ResizeArray<obj> >.MakeGenericType([|tupleType|])
        let addMethod = listType.GetMethod("Add")
        fun (value: JsonValue) ->
            let empty = System.Activator.CreateInstance(listType)
            let kvs =
                if Helpers.isArray value then
                    (Ok empty, value.Value<JArray>()) ||> Seq.fold (fun acc value ->
                        match acc with
                        | Error _ -> acc
                        | Ok acc ->
                            if not (Helpers.isArray value) then
                                ("", BadPrimitive ("an array", value)) |> Error
                            else
                                let kv = value.Value<JArray>()
                                match keyDecoder.Decode(kv.[0]), valueDecoder.Decode(kv.[1]) with
                                | Error er, _ ->
                                    er
                                    |> DecoderError.prependPath ".[0]"
                                    |> Error
                                | _, Error er ->
                                    er
                                    |> DecoderError.prependPath ".[1]"
                                    |> Error
                                | Ok key, Ok value ->
                                    addMethod.Invoke(acc, [|FSharpValue.MakeTuple([|key; value|], tupleType)|]) |> ignore
                                    Ok acc)
                else
                    match keyType with
                    | StringifiableType ofString when Helpers.isObject value ->
                        (Ok empty, value :?> JObject |> Seq.cast<JProperty>)
                        ||> Seq.fold (fun acc prop ->
                            match acc with
                            | Error _ -> acc
                            | Ok acc ->
                                match valueDecoder.Decode(prop.Value) with
                                | Error er ->
                                    er
                                    |> DecoderError.prependPath ("." + prop.Name)
                                    |> Error

                                | Ok v ->
                                    addMethod.Invoke(acc, [|FSharpValue.MakeTuple([|ofString prop.Name; v|], tupleType)|]) |> ignore
                                    Ok acc)
                    | _ ->
                        ("", BadPrimitive ("an array or an object", value)) |> Error
            kvs |> Result.map (fun kvs -> System.Activator.CreateInstance(t, kvs))

    and inline private handleRecord (extra : Map<string, ref<BoxedDecoder>>)
                                    (caseStrategy : CaseStrategy)
                                    (t : System.Type) : BoxedDecoder =
        let decoders =
            FSharpType.GetRecordFields(t, allowAccessToPrivateRepresentation = true)
            |> Array.map (fun fieldInfo ->
                let name = Util.Casing.convert caseStrategy fieldInfo.Name
                name, autoDecoder extra caseStrategy false fieldInfo.PropertyType
            )

        boxDecoder(fun value ->
            autoObject decoders value
            |> Result.map (fun values ->
                FSharpValue.MakeRecord(t, List.toArray values, allowAccessToPrivateRepresentation = true)
            )
        )

    and inline private makeUnion (extra : Map<string, ref<BoxedDecoder>>)
                        (caseStrategy : CaseStrategy)
                        (t : System.Type)
                        (name :string)
                        (values : JsonValue []) =
        let unionCaseInfo =
            FSharpType.GetUnionCases(t, allowAccessToPrivateRepresentation = true)
            |> Array.tryFind (fun unionCaseInfo -> unionCaseInfo.Name = name)

        match unionCaseInfo with
        | None ->
            ("", FailMessage("Cannot find case " + name + " in " + t.FullName)) |> Error

        | Some unionCaseInfo ->
            if values.Length = 0 then
                FSharpValue.MakeUnion(unionCaseInfo, [||], allowAccessToPrivateRepresentation = true) |> Ok
            else
                let decoders =
                    unionCaseInfo.GetFields()
                    |> Array.map (fun fieldInfo ->
                        autoDecoder extra caseStrategy false fieldInfo.PropertyType
                    )

                mixedArray "union field" decoders values
                |> Result.map (fun values ->
                    FSharpValue.MakeUnion(unionCaseInfo, List.toArray values, allowAccessToPrivateRepresentation = true)
                )

    and inline private handleUnion (extra : Map<string, ref<BoxedDecoder>>)
                                    (caseStrategy : CaseStrategy)
                                    (isOptional : bool)
                                    (t : System.Type) : BoxedDecoder =
        boxDecoder(fun value ->
            if Helpers.isString value then
                let name = Helpers.asString value
                makeUnion extra caseStrategy t name [||]
            else if Helpers.isArray value then
                let values = Helpers.asArray value
                let name = Helpers.asString values.[0]
                makeUnion extra caseStrategy t name values.[1..]
            else
                ("", BadPrimitive("a string or an array", value)) |> Error
        )

    and inline private autoDecodeRecordAndUnions (extra : Map<string, ref<BoxedDecoder>>)
                                                (caseStrategy : CaseStrategy)
                                                (isOptional : bool)
                                                (t : System.Type) : BoxedDecoder =
        // Add the decoder to extra in case one of the fields is recursive
        let decoderRef = ref Unchecked.defaultof<_>
        let extra = extra |> Map.add t.FullName decoderRef
        let decoder =
            if FSharpType.IsRecord(t, allowAccessToPrivateRepresentation = true) then
                handleRecord extra caseStrategy t
            else if FSharpType.IsUnion(t, allowAccessToPrivateRepresentation = true) then
                handleUnion extra caseStrategy isOptional t
            else if isOptional then
                // The error will only happen at runtime if the value is not null
                // See https://github.com/MangelMaxime/Thoth/pull/84#issuecomment-444837773
                boxDecoder (fun value ->
                    Error("", BadType("an extra coder for " + t.FullName, value))
                )
            else
                failwithf "Cannot generate auto decoder for %s. Please pass an extra decoder." t.FullName

        decoderRef := decoder
        decoder

    and inline private handleEnum (t : System.Type) =
        let enumType = System.Enum.GetUnderlyingType(t).FullName
        if enumType = typeof<sbyte>.FullName then
            enumDecoder<sbyte> sbyte Operators.string t |> boxDecoder
        else if enumType = typeof<byte>.FullName then
            enumDecoder<byte> byte Operators.string t |> boxDecoder
        else if enumType = typeof<int16>.FullName then
            enumDecoder<int16> int16 Operators.string t |> boxDecoder
        else if enumType = typeof<uint16>.FullName then
            enumDecoder<uint16> uint16 Operators.string t |> boxDecoder
        else if enumType = typeof<int>.FullName then
            enumDecoder<int> int Operators.string t |> boxDecoder
        else if enumType = typeof<uint32>.FullName then
            enumDecoder<uint32> uint32 Operators.string t |> boxDecoder
        else
            failwithf
                """Cannot generate auto decoder for %s.
Thoth.Json.Net only support the folluwing enum types:
- sbyte
- byte
- int16
- uint16
- int
- uint32
If you can't use one of these types, please pass an extra decoder.
                """ t.FullName

    and inline private handleTuple (extra : Map<string, ref<BoxedDecoder>>)
                                    (caseStrategy : CaseStrategy)
                                    (t : System.Type) : BoxedDecoder =
        let decoders =
            FSharpType.GetTupleElements(t)
            |> Array.map (autoDecoder extra caseStrategy false)
        boxDecoder(fun value ->
            if Helpers.isArray value then
                mixedArray "tuple elements" decoders (Helpers.asArray value)
                |> Result.map (fun xs -> FSharpValue.MakeTuple(List.toArray xs, t))
            else
                ("", BadPrimitive ("an array", value)) |> Error
        )

    and inline private handleGeneric (extra : Map<string, ref<BoxedDecoder>>)
                                        (caseStrategy : CaseStrategy)
                                        (isOptional : bool)
                                        (t : System.Type) : BoxedDecoder =
        let fullName = t.GetGenericTypeDefinition().FullName
        if fullName = typedefof<obj option>.FullName then
            autoDecoder extra caseStrategy true t.GenericTypeArguments.[0]
            |> genericOption t
            |> boxDecoder
        else if fullName = typedefof<obj list>.FullName then
            autoDecoder extra caseStrategy false t.GenericTypeArguments.[0]
            |> genericList t
            |> boxDecoder
        else if fullName = typedefof<Map<string, obj>>.FullName then
            genericMap extra caseStrategy t
            |> boxDecoder
        else if fullName = typedefof<Set<string>>.FullName then
            let t = t.GetGenericArguments().[0]
            let decoder = autoDecoder extra caseStrategy false t
            boxDecoder(fun value ->
                match array decoder.BoxedDecoder value with
                | Ok items ->
                    let ar = System.Array.CreateInstance(t, items.Length)
                    for i = 0 to ar.Length - 1 do
                        ar.SetValue(items.[i], i)
                    let setType = typedefof< Set<string> >.MakeGenericType([|t|])
                    System.Activator.CreateInstance(setType, ar) |> Ok
                | Error error -> Error error
            )
        else
            autoDecodeRecordAndUnions extra caseStrategy isOptional t

    and private autoDecoder (extra : Map<string, ref<BoxedDecoder>>)
                            (caseStrategy : CaseStrategy)
                            (isOptional : bool)
                            (t : System.Type) : BoxedDecoder =
        let fullName = t.FullName
        match Map.tryFind fullName extra with
        | Some decoderRef ->
            boxDecoder(fun value -> decoderRef.contents.BoxedDecoder value)
        | None ->
            if t.IsArray then
                handleArray extra caseStrategy t
            else if FSharpType.IsTuple(t) then
                handleTuple extra caseStrategy t
            else if t.IsGenericType then
                handleGeneric extra caseStrategy isOptional t
            else if t.IsEnum then
                handleEnum t
            else if fullName = typeof<bool>.FullName then
                boxDecoder bool
            else if fullName = typedefof<unit>.FullName then
                boxDecoder unit
            else if fullName = typeof<string>.FullName then
                boxDecoder string
            else if fullName = typeof<sbyte>.FullName then
                boxDecoder sbyte
            else if fullName = typeof<byte>.FullName then
                boxDecoder byte
            else if fullName = typeof<int16>.FullName then
                boxDecoder int16
            else if fullName = typeof<uint16>.FullName then
                boxDecoder uint16
            else if fullName = typeof<int>.FullName then
                boxDecoder int
            else if fullName = typeof<uint32>.FullName then
                boxDecoder uint32
            else if fullName = typeof<float>.FullName then
                boxDecoder float
            else if fullName = typeof<float32>.FullName then
                boxDecoder float32
            else if fullName = typeof<System.DateTime>.FullName then
                boxDecoder datetime
            else if fullName = typeof<System.DateTimeOffset>.FullName then
                boxDecoder datetimeOffset
            else if fullName = typeof<System.TimeSpan>.FullName then
                boxDecoder timespan
            else if fullName = typeof<System.Guid>.FullName then
                boxDecoder guid
            // Allows to decode null values
            else if fullName = typeof<obj>.FullName then
                boxDecoder (fun value ->
                    if Helpers.isNullValue value then
                        Ok(null: obj)
                    else
                        value.Value<obj>() |> Ok
                )
            else
                autoDecodeRecordAndUnions extra caseStrategy isOptional t

    let private makeExtra (extra: ExtraCoders option) =
        match extra with
        | None -> Map.empty
        | Some e -> Map.map (fun _ (_,dec) -> ref dec) e.Coders

    module Auto =

        /// This API  is only implemented inside Thoth.Json.Net for now
        /// The goal of this API is to provide better interop when consuming Thoth.Json.Net from a C# project
        type LowLevel =
            /// ATTENTION: Use this only when other arguments (isCamelCase, extra) don't change
            static member generateDecoderCached<'T> (t:System.Type, ?caseStrategy : CaseStrategy, ?extra: ExtraCoders): Decoder<'T> =
                let caseStrategy = defaultArg caseStrategy PascalCase

                let key =
                    t.FullName
                    |> (+) (Operators.string caseStrategy)
                    |> (+) (extra |> Option.map (fun e -> e.Hash) |> Option.defaultValue "")

                let decoderCrate =
                    Cache.Decoders.Value.GetOrAdd(key, fun _ ->
                        autoDecoder (makeExtra extra) caseStrategy false t)

                fun value ->
                    match decoderCrate.Decode(value) with
                    | Ok x -> Ok(x :?> 'T)
                    | Error er -> Error er

            static member generateDecoder<'T> (t: System.Type, ?caseStrategy : CaseStrategy, ?extra: ExtraCoders): Decoder<'T> =
                let caseStrategy = defaultArg caseStrategy PascalCase
                let decoderCrate = autoDecoder (makeExtra extra) caseStrategy false t
                fun value ->
                    match decoderCrate.Decode(value) with
                    | Ok x -> Ok(x :?> 'T)
                    | Error er -> Error er

            static member fromString<'T>(json: string, t: System.Type, ?caseStrategy : CaseStrategy, ?extra: ExtraCoders): Result<'T, string> =
                let decoder = LowLevel.generateDecoder(t, ?caseStrategy=caseStrategy, ?extra=extra)
                Decode.fromString decoder json

    type Auto =
        /// ATTENTION: Use this only when other arguments (isCamelCase, extra) don't change
        static member generateDecoderCached<'T> (?caseStrategy : CaseStrategy, ?extra: ExtraCoders): Decoder<'T> =
            let t = typeof<'T>
            Auto.LowLevel.generateDecoderCached (t, ?caseStrategy = caseStrategy, ?extra = extra)

        static member generateDecoder<'T> (?caseStrategy : CaseStrategy, ?extra: ExtraCoders): Decoder<'T> =
            let t = typeof<'T>
            Auto.LowLevel.generateDecoder(t, ?caseStrategy = caseStrategy, ?extra = extra)

        static member fromString<'T>(json: string, ?caseStrategy : CaseStrategy, ?extra: ExtraCoders): Result<'T, string> =
            let decoder = Auto.generateDecoder(?caseStrategy=caseStrategy, ?extra=extra)
            Decode.fromString decoder json

        static member unsafeFromString<'T>(json: string, ?caseStrategy : CaseStrategy, ?extra: ExtraCoders): 'T =
            let decoder = Auto.generateDecoder(?caseStrategy=caseStrategy, ?extra=extra)
            match Decode.fromString decoder json with
            | Ok x -> x
            | Error msg -> failwith msg
