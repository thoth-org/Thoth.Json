namespace Thoth.Json.Core

open System.Globalization
open System

[<RequireQualifiedAccess>]
module Encode =

    /// <summary>Encode a string.</summary>
    let inline string value =
        { new IEncodable with
            member _.Encode(helpers) = helpers.encodeString value
        }

    /// <summary>Encode a char as a single character string.</summary>
    let inline char value =
        { new IEncodable with
            member _.Encode(helpers) = helpers.encodeChar value
        }

    /// <summary>Encode a Guid as a string.</summary>
    let inline guid value = value.ToString() |> string

    /// <summary>Encode a Uri as a string, keeping the original form.</summary>
    let inline uri (value: Uri) = value.OriginalString |> string

    /// <summary>Encode a float as a number.</summary>
    let inline float value =
        { new IEncodable with
            member _.Encode(helpers) = helpers.encodeDecimalNumber value
        }

    /// <summary>Encode a float32 as a number.</summary>
    let float32 (value: float32) = float (Operators.float value)

    /// <summary>Encode a decimal as a string, so no precision is lost.</summary>
    let inline decimal (value: decimal) =
        value.ToString(CultureInfo.InvariantCulture) |> string

    /// <summary>Encode <c>null</c>.</summary>
    let inline nil<'T> =
        { new IEncodable with
            member _.Encode(helpers) = helpers.encodeNull ()
        }

    /// <summary>Encode a boolean.</summary>
    let inline bool value =
        { new IEncodable with
            member _.Encode(helpers) = helpers.encodeBool value
        }

    /// <summary>Encode an object from the given properties.</summary>
    let inline object (values: seq<string * IEncodable>) =
        { new IEncodable with
            member _.Encode(helpers) =
                values
                |> Seq.map (fun (k, v) -> (k, v.Encode(helpers)))
                |> helpers.encodeObject
        }

    /// <summary>Encode an array.</summary>
    let inline array (values: IEncodable array) =
        { new IEncodable with
            member _.Encode(helpers) =
                values
                |> Array.map (fun v -> v.Encode(helpers))
                |> helpers.encodeArray
        }

    /// <summary>Encode a list as a JSON array.</summary>
    let list (values: IEncodable list) =
        { new IEncodable with
            member _.Encode(helpers) =
                values
                |> List.map (fun v -> v.Encode(helpers))
                |> helpers.encodeList
        }

    /// <summary>Encode a sequence as a JSON array.</summary>
    let seq (values: IEncodable seq) =
        { new IEncodable with
            member _.Encode(helpers) =
                values
                |> Seq.map (fun v -> v.Encode(helpers))
                |> helpers.encodeSeq
        }

    /// <summary>Encode a ResizeArray as a JSON array.</summary>
    let resizeArray (values: IEncodable ResizeArray) =
        { new IEncodable with
            member _.Encode(helpers) =
                let result = ResizeArray(values.Count)

                for v in values do
                    result.Add(v.Encode(helpers))

                helpers.encodeResizeArray result
        }

    /// <summary>Encode each element of a list with the given encoder.</summary>
    let mapList (encoder: Encoder<'a>) (values: 'a list) : IEncodable =
        values |> List.map encoder |> list

    /// <summary>Encode each element of an array with the given encoder.</summary>
    let mapArray (encoder: Encoder<'a>) (values: 'a array) : IEncodable =
        values |> Array.map encoder |> array

    /// <summary>Encode each element of a sequence with the given encoder.</summary>
    let mapSeq (encoder: Encoder<'a>) (values: 'a seq) : IEncodable =
        values |> Seq.map encoder |> seq

    /// <summary>Encode each element of a ResizeArray with the given encoder.</summary>
    let mapResizeArray
        (encoder: Encoder<'a>)
        (values: 'a ResizeArray)
        : IEncodable
        =
        values |> Seq.map encoder |> ResizeArray |> resizeArray

    /// <summary>Encode a map as an object, one property per key.</summary>
    let dict (values: Map<string, IEncodable>) : IEncodable =
        values |> Map.toSeq |> object

    /// <summary>Encode a bigint as a string.</summary>
    let inline bigint (value: bigint) = value.ToString() |> string

    /// <summary>Encode a DateTimeOffset as an ISO 8601 string.</summary>
    let inline datetimeOffset (value: DateTimeOffset) =
        value.ToString("O", CultureInfo.InvariantCulture) |> string

    /// <summary>Encode a TimeSpan as a string.</summary>
    let inline timespan value = value.ToString() |> string

    /// <summary>Encode a DateTime as an ISO 8601 string.</summary>
    let inline datetime (value: DateTime) =
        value.ToString("O", CultureInfo.InvariantCulture) |> string

    /// <summary>Encode an sbyte as a number.</summary>
    let inline sbyte (value: sbyte) =
        { new IEncodable with
            member _.Encode(helpers) =
                helpers.encodeSignedIntegralNumber (int32 value)
        }

    /// <summary>Encode a byte as a number.</summary>
    let inline byte (value: byte) =
        { new IEncodable with
            member _.Encode(helpers) =
                helpers.encodeUnsignedIntegralNumber (uint32 value)
        }

    /// <summary>Encode an int16 as a number.</summary>
    let inline int16 (value: int16) =
        { new IEncodable with
            member _.Encode(helpers) =
                helpers.encodeSignedIntegralNumber (int32 value)
        }

    /// <summary>Encode a uint16 as a number.</summary>
    let inline uint16 (value: uint16) =
        { new IEncodable with
            member _.Encode(helpers) =
                helpers.encodeUnsignedIntegralNumber (uint32 value)
        }

    /// <summary>Encode an int as a number.</summary>
    let inline int (value: int) =
        { new IEncodable with
            member _.Encode(helpers) =
                helpers.encodeSignedIntegralNumber value
        }

    /// <summary>Encode a uint32 as a number.</summary>
    let inline uint32 (value: uint32) =
        { new IEncodable with
            member _.Encode(helpers) =
                helpers.encodeUnsignedIntegralNumber value
        }

    /// <summary>Encode an int64 as a string, so no precision is lost.</summary>
    let inline int64 (value: int64) =
        value.ToString(CultureInfo.InvariantCulture) |> string

    /// <summary>Encode a uint64 as a string, so no precision is lost.</summary>
    let inline uint64 (value: uint64) =
        value.ToString(CultureInfo.InvariantCulture) |> string

    /// <summary>Encode unit as <c>null</c>.</summary>
    let inline unit () = nil

    /// <summary>Encode a tuple of 2 elements as a JSON array.</summary>
    let tuple2 (enc1: Encoder<'T1>) (enc2: Encoder<'T2>) (v1, v2) : IEncodable =
        array
            [|
                enc1 v1
                enc2 v2
            |]

    /// <summary>Encode a tuple of 3 elements as a JSON array.</summary>
    let tuple3
        (enc1: Encoder<'T1>)
        (enc2: Encoder<'T2>)
        (enc3: Encoder<'T3>)
        (v1, v2, v3)
        : IEncodable
        =
        array
            [|
                enc1 v1
                enc2 v2
                enc3 v3
            |]

    /// <summary>Encode a tuple of 4 elements as a JSON array.</summary>
    let tuple4
        (enc1: Encoder<'T1>)
        (enc2: Encoder<'T2>)
        (enc3: Encoder<'T3>)
        (enc4: Encoder<'T4>)
        (v1, v2, v3, v4)
        : IEncodable
        =
        array
            [|
                enc1 v1
                enc2 v2
                enc3 v3
                enc4 v4
            |]

    /// <summary>Encode a tuple of 5 elements as a JSON array.</summary>
    let tuple5
        (enc1: Encoder<'T1>)
        (enc2: Encoder<'T2>)
        (enc3: Encoder<'T3>)
        (enc4: Encoder<'T4>)
        (enc5: Encoder<'T5>)
        (v1, v2, v3, v4, v5)
        : IEncodable
        =
        array
            [|
                enc1 v1
                enc2 v2
                enc3 v3
                enc4 v4
                enc5 v5
            |]

    /// <summary>Encode a tuple of 6 elements as a JSON array.</summary>
    let tuple6
        (enc1: Encoder<'T1>)
        (enc2: Encoder<'T2>)
        (enc3: Encoder<'T3>)
        (enc4: Encoder<'T4>)
        (enc5: Encoder<'T5>)
        (enc6: Encoder<'T6>)
        (v1, v2, v3, v4, v5, v6)
        : IEncodable
        =
        array
            [|
                enc1 v1
                enc2 v2
                enc3 v3
                enc4 v4
                enc5 v5
                enc6 v6
            |]

    /// <summary>Encode a tuple of 7 elements as a JSON array.</summary>
    let tuple7
        (enc1: Encoder<'T1>)
        (enc2: Encoder<'T2>)
        (enc3: Encoder<'T3>)
        (enc4: Encoder<'T4>)
        (enc5: Encoder<'T5>)
        (enc6: Encoder<'T6>)
        (enc7: Encoder<'T7>)
        (v1, v2, v3, v4, v5, v6, v7)
        : IEncodable
        =
        array
            [|
                enc1 v1
                enc2 v2
                enc3 v3
                enc4 v4
                enc5 v5
                enc6 v6
                enc7 v7
            |]

    /// <summary>Encode a tuple of 8 elements as a JSON array.</summary>
    let tuple8
        (enc1: Encoder<'T1>)
        (enc2: Encoder<'T2>)
        (enc3: Encoder<'T3>)
        (enc4: Encoder<'T4>)
        (enc5: Encoder<'T5>)
        (enc6: Encoder<'T6>)
        (enc7: Encoder<'T7>)
        (enc8: Encoder<'T8>)
        (v1, v2, v3, v4, v5, v6, v7, v8)
        : IEncodable
        =
        array
            [|
                enc1 v1
                enc2 v2
                enc3 v3
                enc4 v4
                enc5 v5
                enc6 v6
                enc7 v7
                enc8 v8
            |]


    /// <summary>Encode a map as an array of <c>[ key, value ]</c> pairs. Use it when the key is not a string.</summary>
    let map
        (keyEncoder: Encoder<'key>)
        (valueEncoder: Encoder<'value>)
        (values: Map<'key, 'value>)
        : IEncodable
        =
        values
        |> Map.toList
        |> List.map (tuple2 keyEncoder valueEncoder)
        |> list

    /// <summary>Defer the construction of an encoder until it is first used, for recursive types.</summary>
    let lazily<'t> (enc: Lazy<Encoder<'t>>) : Encoder<'t> =
        fun (x: 't) -> enc.Value x

    ////////////
    /// Enum ///
    ///////////

    module Enum =

        /// <summary>Encode an enum with an underlying byte as a number.</summary>
        let byte<'TEnum when 'TEnum: enum<byte>> (value: 'TEnum) : IEncodable =
            LanguagePrimitives.EnumToValue value |> byte

        /// <summary>Encode an enum with an underlying sbyte as a number.</summary>
        let sbyte<'TEnum when 'TEnum: enum<sbyte>>
            (value: 'TEnum)
            : IEncodable
            =
            LanguagePrimitives.EnumToValue value |> sbyte

        /// <summary>Encode an enum with an underlying int16 as a number.</summary>
        let int16<'TEnum when 'TEnum: enum<int16>>
            (value: 'TEnum)
            : IEncodable
            =
            LanguagePrimitives.EnumToValue value |> int16

        /// <summary>Encode an enum with an underlying uint16 as a number.</summary>
        let uint16<'TEnum when 'TEnum: enum<uint16>>
            (value: 'TEnum)
            : IEncodable
            =
            LanguagePrimitives.EnumToValue value |> uint16

        /// <summary>Encode an enum with an underlying int as a number.</summary>
        let int<'TEnum when 'TEnum: enum<int>> (value: 'TEnum) : IEncodable =
            LanguagePrimitives.EnumToValue value |> int

        /// <summary>Encode an enum with an underlying uint32 as a number.</summary>
        let uint32<'TEnum when 'TEnum: enum<uint32>>
            (value: 'TEnum)
            : IEncodable
            =
            LanguagePrimitives.EnumToValue value |> uint32

    /// <summary>
    /// Encode <c>Some x</c> as <c>x</c>, and <c>None</c> as <c>null</c>.
    /// </summary>
    /// <remarks>
    /// Lossy: a nested option does not round-trip. Use <see cref="losslessOption"/> when you need
    /// the distinction.
    /// </remarks>
    let lossyOption (encoder: Encoder<'a>) =
        Option.map encoder >> Option.defaultWith (fun _ -> nil)

    /// <summary>
    /// Encode an option as an object carrying the case, so a nested option round-trips.
    /// </summary>
    /// <remarks>
    /// <c>Some x</c> gives <c>{ "$type": "option", "$case": "some", "$value": x }</c>, and
    /// <c>None</c> gives <c>{ "$type": "option", "$case": "none" }</c>.
    /// </remarks>
    let losslessOption (encoder: Encoder<'a>) (value: 'a option) =
        match value with
        | Some v ->
            object
                [
                    "$type", string "option"
                    "$case", string "some"
                    "$value", encoder v
                ]
        | None ->
            object
                [
                    "$type", string "option"
                    "$case", string "none"
                ]

    /// <summary>Encode a <see cref="T:Thoth.Json.Core.Json"/> value as it stands.</summary>
    let rec value: Encoder<Json> =
        fun json ->
            match json with
            | Json.Null -> nil
            | Json.Boolean b -> bool b
            | Json.String s -> string s
            | Json.Number f -> float f
            | Json.Array xs -> list (List.map value xs)
            | Json.Object kvps ->
                kvps |> Seq.map (fun (k, v) -> k, value v) |> object

    /// <summary>Write an encodable through the given helpers, giving the runtime's own JSON value.</summary>
    let inline toJsonValue
        (helpers: IEncoderHelpers<'JsonValue>)
        (json: IEncodable)
        =
        json.Encode(helpers)

    /// <summary>The encoder half of a codec.</summary>
    let codec (c: Codec<'t>) : Encoder<'t> = c.Encoder
