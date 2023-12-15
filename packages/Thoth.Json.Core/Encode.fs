namespace Thoth.Json.Core

open Fable.Core
open System.Globalization
open System

[<RequireQualifiedAccess>]
module Encode =

    let inline string value = Json.String value
    let inline char value = Json.Char value
    let inline guid value = value.ToString() |> string
    let inline float value = Json.DecimalNumber value

    let float32 (value: float32) =
        Json.DecimalNumber(Operators.float value)

    let inline decimal (value: decimal) =
        value.ToString(CultureInfo.InvariantCulture) |> string

    let inline nil<'T> = Json.Null
    let inline bool value = Json.Boolean value
    let inline object values = Json.Object values
    let inline array values = Json.Array values
    let list values = Json.Array(values |> List.toArray)
    let seq values = Json.Array(values |> Seq.toArray)
    let dict (values: Map<string, Json>) : Json = values |> Map.toList |> object

    let inline bigint (value: bigint) = value.ToString() |> string

    let inline datetimeOffset (value: DateTimeOffset) =
        value.ToString("O", CultureInfo.InvariantCulture) |> string

    let inline timespan value = value.ToString() |> string

    let inline datetime (value: DateTime) =
        value.ToString("O", CultureInfo.InvariantCulture) |> string

    let inline sbyte (value: sbyte) = Json.IntegralNumber(uint32 value)
    let inline byte (value: byte) = Json.IntegralNumber(uint32 value)
    let inline int16 (value: int16) = Json.IntegralNumber(uint32 value)
    let inline uint16 (value: uint16) = Json.IntegralNumber(uint32 value)
    let inline int (value: int) = Json.IntegralNumber(uint32 value)
    let inline uint32 (value: uint32) = Json.IntegralNumber value

    let inline int64 (value: int64) =
        value.ToString(CultureInfo.InvariantCulture) |> string

    let inline uint64 (value: uint64) =
        value.ToString(CultureInfo.InvariantCulture) |> string

    let inline unit () = Json.Unit

    let tuple2 (enc1: Encoder<'T1>) (enc2: Encoder<'T2>) (v1, v2) : Json =
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
        : Json
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
        : Json
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
        : Json
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
        : Json
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
        : Json
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
        : Json
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
        : Json
        =
        values
        |> Map.toList
        |> List.map (tuple2 keyEncoder valueEncoder)
        |> list

    ////////////
    /// Enum ///
    ///////////

    module Enum =

        let byte<'TEnum when 'TEnum: enum<byte>> (value: 'TEnum) : Json =
            LanguagePrimitives.EnumToValue value |> byte

        let sbyte<'TEnum when 'TEnum: enum<sbyte>> (value: 'TEnum) : Json =
            LanguagePrimitives.EnumToValue value |> sbyte

        let int16<'TEnum when 'TEnum: enum<int16>> (value: 'TEnum) : Json =
            LanguagePrimitives.EnumToValue value |> int16

        let uint16<'TEnum when 'TEnum: enum<uint16>> (value: 'TEnum) : Json =
            LanguagePrimitives.EnumToValue value |> uint16

        let int<'TEnum when 'TEnum: enum<int>> (value: 'TEnum) : Json =
            LanguagePrimitives.EnumToValue value |> int

        let uint32<'TEnum when 'TEnum: enum<uint32>> (value: 'TEnum) : Json =
            LanguagePrimitives.EnumToValue value |> uint32

    let option (encoder: 'a -> Json) =
        Option.map encoder >> Option.defaultWith (fun _ -> nil)

    let rec toJsonValue (helpers: IEncoderHelpers<'JsonValue>) (json: Json) =
        match json with
        | Json.String value -> helpers.encodeString value
        | Json.IntegralNumber value -> helpers.encodeIntegralNumber value
        | Json.Object values ->
            let o = helpers.createEmptyObject ()

            values
            |> Seq.iter (fun (k, v) ->
                helpers.setPropertyOnObject (o, k, toJsonValue helpers v)
            )

            o
        | Json.Char value -> helpers.encodeChar value
        | Json.DecimalNumber value -> helpers.encodeDecimalNumber value
        | Json.Null -> helpers.encodeNull ()
        | Json.Boolean value -> helpers.encodeBool value
        | Json.Array value ->
            value |> Array.map (toJsonValue helpers) |> helpers.encodeArray
        | Json.Unit -> helpers.encodeNull ()
