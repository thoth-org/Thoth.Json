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

    let option (encoder: Encoder<'a>) =
        Option.map encoder >> Option.defaultWith (fun _ -> nil)

    let inline toJsonValue
        (helpers: IEncoderHelpers<'JsonValue>)
        (json: IEncodable)
        =
        json.Encode(helpers)
