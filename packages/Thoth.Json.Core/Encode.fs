namespace Thoth.Json.Core

open Fable.Core
open System.Globalization
open System

[<RequireQualifiedAccess>]
module Encode =

    let inline string value =
        { new IEncodable with
            member _.Encode(helpers) = helpers.encodeString value
        }

    let inline char value =
        { new IEncodable with
            member _.Encode(helpers) = helpers.encodeChar value
        }

    let inline guid value = value.ToString() |> string

    let inline float value =
        { new IEncodable with
            member _.Encode(helpers) = helpers.encodeDecimalNumber value
        }

    let float32 (value: float32) = float (Operators.float value)

    let inline decimal (value: decimal) =
        value.ToString(CultureInfo.InvariantCulture) |> string

    let inline nil<'T> =
        { new IEncodable with
            member _.Encode(helpers) = helpers.encodeNull ()
        }

    let inline bool value =
        { new IEncodable with
            member _.Encode(helpers) = helpers.encodeBool value
        }

    let inline object (values: seq<string * IEncodable>) =
        { new IEncodable with
            member _.Encode(helpers) =
                values
                |> Seq.map (fun (k, v) -> (k, v.Encode(helpers)))
                |> helpers.encodeObject
        }

    let inline array (values: IEncodable array) =
        { new IEncodable with
            member _.Encode(helpers) =
                values
                |> Array.map (fun v -> v.Encode(helpers))
                |> helpers.encodeArray
        }

    let list (values: IEncodable list) =
        { new IEncodable with
            member _.Encode(helpers) =
                values
                |> List.map (fun v -> v.Encode(helpers))
                |> helpers.encodeList
        }

    let seq (values: IEncodable seq) =
        { new IEncodable with
            member _.Encode(helpers) =
                values
                |> Seq.map (fun v -> v.Encode(helpers))
                |> helpers.encodeSeq
        }

    let resizeArray (values: IEncodable ResizeArray) =
        { new IEncodable with
            member _.Encode(helpers) =
                let result = ResizeArray(values.Count)

                for v in values do
                    result.Add(v.Encode(helpers))

                helpers.encodeResizeArray result
        }

    let dict (values: Map<string, IEncodable>) : IEncodable =
        values |> Map.toSeq |> object

    let inline bigint (value: bigint) = value.ToString() |> string

    let inline datetimeOffset (value: DateTimeOffset) =
        value.ToString("O", CultureInfo.InvariantCulture) |> string

    let inline timespan value = value.ToString() |> string

    let inline datetime (value: DateTime) =
        value.ToString("O", CultureInfo.InvariantCulture) |> string

    let inline sbyte (value: sbyte) =
        { new IEncodable with
            member _.Encode(helpers) =
                helpers.encodeSignedIntegralNumber (int32 value)
        }

    let inline byte (value: byte) =
        { new IEncodable with
            member _.Encode(helpers) =
                helpers.encodeUnsignedIntegralNumber (uint32 value)
        }

    let inline int16 (value: int16) =
        { new IEncodable with
            member _.Encode(helpers) =
                helpers.encodeSignedIntegralNumber (int32 value)
        }

    let inline uint16 (value: uint16) =
        { new IEncodable with
            member _.Encode(helpers) =
                helpers.encodeUnsignedIntegralNumber (uint32 value)
        }

    let inline int (value: int) =
        { new IEncodable with
            member _.Encode(helpers) =
                helpers.encodeSignedIntegralNumber value
        }

    let inline uint32 (value: uint32) =
        { new IEncodable with
            member _.Encode(helpers) =
                helpers.encodeUnsignedIntegralNumber value
        }

    let inline int64 (value: int64) =
        value.ToString(CultureInfo.InvariantCulture) |> string

    let inline uint64 (value: uint64) =
        value.ToString(CultureInfo.InvariantCulture) |> string

    let inline unit () = nil

    let tuple2 (enc1: Encoder<'T1>) (enc2: Encoder<'T2>) (v1, v2) : IEncodable =
        array
            [|
                enc1 v1
                enc2 v2
            |]

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

    ////////////
    /// Enum ///
    ///////////

    module Enum =

        let byte<'TEnum when 'TEnum: enum<byte>> (value: 'TEnum) : IEncodable =
            LanguagePrimitives.EnumToValue value |> byte

        let sbyte<'TEnum when 'TEnum: enum<sbyte>>
            (value: 'TEnum)
            : IEncodable
            =
            LanguagePrimitives.EnumToValue value |> sbyte

        let int16<'TEnum when 'TEnum: enum<int16>>
            (value: 'TEnum)
            : IEncodable
            =
            LanguagePrimitives.EnumToValue value |> int16

        let uint16<'TEnum when 'TEnum: enum<uint16>>
            (value: 'TEnum)
            : IEncodable
            =
            LanguagePrimitives.EnumToValue value |> uint16

        let int<'TEnum when 'TEnum: enum<int>> (value: 'TEnum) : IEncodable =
            LanguagePrimitives.EnumToValue value |> int

        let uint32<'TEnum when 'TEnum: enum<uint32>>
            (value: 'TEnum)
            : IEncodable
            =
            LanguagePrimitives.EnumToValue value |> uint32

    /// <summary>
    /// Encodes an option value using the provided encoder.
    ///
    /// Attention, this encoder is lossy, it's result will not be able to distinguish between `'T option` and `'T option option`.
    ///
    /// If you need to distinguish between `'T option` and `'T option option`, use `losslessOption`.
    /// </summary>
    /// <param name="encoder">The encoder to apply if the value is Some</param>
    /// <typeparam name="'a">The type of the value to encode</typeparam>
    /// <returns>
    /// The result of the encoder if the value is Some, otherwise nil
    /// </returns>
    let lossyOption (encoder: Encoder<'a>) =
        Option.map encoder >> Option.defaultWith (fun _ -> nil)

    /// <summary>
    /// Encodes an option value using the provided encoder.
    ///
    /// This encoder is lossless, it's result will be able to distinguish between `'T option` and `'T option option`.
    ///
    /// If you don't need to distinguish between `'T option` and `'T option option`, use `lossyOption`.
    /// </summary>
    /// <param name="encoder">The encoder to apply if the value is Some</param>
    /// <typeparam name="'a">The type of the value to encode</typeparam>
    /// <returns>
    /// If the value is Some, the object will have the following fields:
    ///
    /// - `$type` field set to `option`
    /// - `$case` field set to `some`
    /// - `$value` field set to the result of the encoder.
    ///
    /// If the value is None, the object will have the following fields:
    ///
    /// - `$type` field set to `option`
    /// - `$case` field set to `none`
    /// </returns>
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

    let inline toJsonValue
        (helpers: IEncoderHelpers<'JsonValue>)
        (json: IEncodable)
        =
        json.Encode(helpers)
