module Thoth.Json.Tests.Properties

// Fable emits Hedgehog's RecheckData as an FSharpRef in its TypeScript
// output, so Property.fs does not type check there.
#if !FABLE_COMPILER_TYPESCRIPT

open System
open Thoth.Json.Tests.Testing
open Thoth.Json.Tests.Types
open Thoth.Json.Core
open Thoth.Json.Core.Auto
open Hedgehog
open Hedgehog.FSharp
open Scriptorium.Nib.Assertion
open Scriptorium.Quill
open Scriptorium.Hedgehog
open type Scriptorium.Quill.Test
open type Scriptorium.Hedgehog.Test

let private pair
    (runner: TestRunner<_, _>)
    (encode: 'T -> IEncodable)
    (decoder: Decoder<'T>)
    (value: 'T)
    =
    let json = encode value |> runner.Encode.toString 0
    let actual = runner.Decode.fromString decoder json

    assertThat actual (isEqualTo (Ok value))

let private codec (runner: TestRunner<_, _>) (codec: Codec<'T>) (value: 'T) =
    let actual = roundTrip runner codec value

    assertThat actual (isEqualTo value)

let private smallSize = Range.linear 0 20

// Fable's Python runtime rejects UInt64.TryParse above Int64.MaxValue.
let private uint64Range =
#if FABLE_COMPILER_PYTHON
    Range.exponentialFrom 0UL 0UL (uint64 Int64.MaxValue)
#else
    Range.exponentialBounded ()
#endif

let private dateTimeRange =
    Range.constant (DateTime(1970, 1, 1)) (DateTime(2100, 1, 1))

let private dateTimeOffsetRange =
    Range.constant
        (DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
        (DateTimeOffset(2100, 1, 1, 0, 0, 0, TimeSpan.Zero))

// bigint is unbounded; this range reaches past what Int64 can hold.
let private bigintRange =
    let bound = bigint Int64.MaxValue * 1000I
    Range.exponentialFrom 0I -bound bound

// Hedgehog's Gen.guid builds a Guid from a byte array, which Fable's Python
// runtime does not implement.
let private guidGen =
    Gen.string (Range.singleton 32) (Gen.item (List.ofSeq "0123456789abcdef"))
    |> Gen.map (fun hex ->
        Guid.Parse(
            hex.Substring(0, 8)
            + "-"
            + hex.Substring(8, 4)
            + "-"
            + hex.Substring(12, 4)
            + "-"
            + hex.Substring(16, 4)
            + "-"
            + hex.Substring(20, 12)
        )
    )

// Hedgehog's Gen.timeSpan is .NET only.
let private timeSpanGen =
    Gen.zip4
        (Gen.int32 (Range.linear -3650 3650))
        (Gen.int32 (Range.linear 0 23))
        (Gen.int32 (Range.linear 0 59))
        (Gen.int32 (Range.linear 0 59))
    |> Gen.map (fun (days, hours, minutes, seconds) ->
        TimeSpan(days, hours, minutes, seconds)
    )

let tests (runner: TestRunner<'DecoderJsonValue, 'EncoderJsonValue>) =
    testList (
        "Properties",
        [
            testList (
                "Encoder and decoder pairs round-trip",
                [
                    testProperty (
                        "Encode.int / Decode.int",
                        Gen.int32 (Range.exponentialBounded ()),
                        pair runner Encode.int Decode.int
                    )

                    testProperty (
                        "Encode.int64 / Decode.int64",
                        Gen.int64 (Range.exponentialBounded ()),
                        pair runner Encode.int64 Decode.int64
                    )

                    testProperty (
                        "Encode.uint32 / Decode.uint32",
                        Gen.uint32 (Range.exponentialBounded ()),
                        pair runner Encode.uint32 Decode.uint32
                    )

                    testProperty (
                        "Encode.uint64 / Decode.uint64",
                        Gen.uint64 uint64Range,
                        pair runner Encode.uint64 Decode.uint64
                    )

                    testProperty (
                        "Encode.int16 / Decode.int16",
                        Gen.int16 (Range.exponentialBounded ()),
                        pair runner Encode.int16 Decode.int16
                    )

                    testProperty (
                        "Encode.uint16 / Decode.uint16",
                        Gen.uint16 (Range.exponentialBounded ()),
                        pair runner Encode.uint16 Decode.uint16
                    )

                    testProperty (
                        "Encode.sbyte / Decode.sbyte",
                        Gen.sbyte (Range.exponentialBounded ()),
                        pair runner Encode.sbyte Decode.sbyte
                    )

                    testProperty (
                        "Encode.byte / Decode.byte",
                        Gen.byte (Range.exponentialBounded ()),
                        pair runner Encode.byte Decode.byte
                    )

                    testProperty (
                        "Encode.float / Decode.float",
                        Gen.double (Range.exponentialBounded ()),
                        pair runner Encode.float Decode.float
                    )

                    testProperty (
                        "Encode.bool / Decode.bool",
                        Gen.bool,
                        pair runner Encode.bool Decode.bool
                    )

                    testProperty (
                        "Encode.string / Decode.string",
                        Gen.string smallSize Gen.unicode,
                        pair runner Encode.string Decode.string
                    )

                    testProperty (
                        "Encode.char / Decode.char",
                        Gen.unicode,
                        pair runner Encode.char Decode.char
                    )

                    testProperty (
                        "Encode.guid / Decode.guid",
                        guidGen,
                        pair runner Encode.guid Decode.guid
                    )

                    testProperty (
                        "Encode.float32 / Decode.float32",
                        Gen.single (Range.exponentialBounded ()),
                        pair runner Encode.float32 Decode.float32
                    )

                    testProperty (
                        "Encode.decimal / Decode.decimal",
                        Gen.decimal (Range.exponentialBounded ()),
                        pair runner Encode.decimal Decode.decimal
                    )

                    testProperty (
                        "Encode.bigint / Decode.bigint",
                        Gen.bigint bigintRange,
                        pair runner Encode.bigint Decode.bigint
                    )

                    testProperty (
                        "Encode.timespan / Decode.timespan",
                        timeSpanGen,
                        pair runner Encode.timespan Decode.timespan
                    )

                // Thoth.Json.Core does not expose these decoders under Python.
#if !FABLE_COMPILER_PYTHON
                    testProperty (
                        "Encode.datetimeOffset / Decode.datetimeOffset",
                        Gen.dateTimeOffset dateTimeOffsetRange,
                        pair runner Encode.datetimeOffset Decode.datetimeOffset
                    )

                    testProperty (
                        "Encode.datetime / Decode.datetimeUtc",
                        Gen.dateTimeUtc dateTimeRange,
                        pair runner Encode.datetime Decode.datetimeUtc
                    )
#endif
                ]
            )

            testList (
                "Combinators round-trip",
                [
                    testProperty (
                        "Codec.list",
                        Gen.list
                            smallSize
                            (Gen.int32 (Range.exponentialBounded ())),
                        codec runner (Codec.list Codec.int)
                    )

                    testProperty (
                        "Codec.array",
                        Gen.array smallSize (Gen.string smallSize Gen.unicode),
                        codec runner (Codec.array Codec.string)
                    )

                    testProperty (
                        "Codec.lossyOption",
                        Gen.option (Gen.int32 (Range.exponentialBounded ())),
                        codec runner (Codec.lossyOption Codec.int)
                    )

                    testProperty (
                        "Codec.losslessOption of an option",
                        Gen.option (Gen.option Gen.bool),
                        codec
                            runner
                            (Codec.losslessOption (
                                Codec.losslessOption Codec.bool
                            ))
                    )

                    testProperty (
                        "Codec.tuple2",
                        Gen.zip
                            (Gen.int32 (Range.exponentialBounded ()))
                            (Gen.string smallSize Gen.unicode),
                        codec runner (Codec.tuple2 Codec.int Codec.string)
                    )

                    testProperty (
                        "Codec.list of Codec.lossyOption",
                        Gen.list
                            smallSize
                            (Gen.option (Gen.string smallSize Gen.unicode)),
                        codec
                            runner
                            (Codec.list (Codec.lossyOption Codec.string))
                    )
                ]
            )

            testList (
                "Robustness",
                [
                    testProperty (
                        "Decode.fromString reports an error rather than throwing",
                        Gen.string (Range.linear 0 40) Gen.unicode,
                        fun (input: string) ->
                            runner.Decode.fromString Decode.int input |> ignore
                    )

                    testProperty (
                        "an encoded value is always valid JSON",
                        Gen.list smallSize (Gen.string smallSize Gen.unicode),
                        fun values ->
                            let json =
                                values
                                |> List.map Encode.string
                                |> Encode.list
                                |> runner.Encode.toString 0

                            let reparsed =
                                runner.Decode.fromString Decode.value json

                            assertThat reparsed Result.isOk
                    )
                ]
            )

            // The Auto API does not pass its own suite under Python yet.
            testList (
                "Auto round-trip",
                skipIfPython,
                [
                    testProperty (
                        "a record",
                        Derive.gen<User>,
                        fun (value: User) ->
                            let json =
                                value
                                |> Encode.Auto.generateEncoder ()
                                |> runner.Encode.toString 0

                            let actual =
                                runner.Decode.fromString
                                    (Decode.Auto.generateDecoder<User> ())
                                    json

                            assertThat actual (isEqualTo (Ok value))
                    )
                ]
            )
        ]
    )

#endif
