module Thoth.Json.Tests.BackAndForth

open Thoth.Json.Tests.Testing
open Thoth.Json.Core
open Fable.Pyxpecto

let tests (runner: TestRunner<_, _>) =
    testList
        "Thoth.Json - BackAndForth"
        [

            testCase "losslessOption is symmetric"
            <| fun _ ->
                // Simple Some 'T
                let expected = Some 42

                let json =
                    expected
                    |> Encode.losslessOption Encode.int
                    |> runner.Encode.toString 0

                let decoded =
                    runner.Decode.fromString
                        (Decode.losslessOption Decode.int)
                        json

                equal (Ok expected) decoded

                // Simple None

                let expected = None

                let json =
                    expected
                    |> Encode.losslessOption Encode.int
                    |> runner.Encode.toString 0

                let decoded =
                    runner.Decode.fromString
                        (Decode.losslessOption Decode.int)
                        json

                equal (Ok expected) decoded

                // Nested option with value

                let expected = Some(Some(Some 42))

                let json =
                    expected
                    |> Encode.losslessOption (
                        Encode.losslessOption (Encode.losslessOption Encode.int)
                    )
                    |> runner.Encode.toString 0

                let decoded =
                    runner.Decode.fromString
                        (Decode.losslessOption (
                            Decode.losslessOption (
                                Decode.losslessOption Decode.int
                            )
                        ))
                        json

                equal (Ok expected) decoded

                // Nested option with None

                let expected = Some(Some None)

                let json =
                    expected
                    |> Encode.losslessOption (
                        Encode.losslessOption (Encode.losslessOption Encode.int)
                    )
                    |> runner.Encode.toString 0

                let decoded =
                    runner.Decode.fromString
                        (Decode.losslessOption (
                            Decode.losslessOption (
                                Decode.losslessOption Decode.int
                            )
                        ))
                        json

                equal (Ok expected) decoded

            testCase "float keeps nan and infinity symmetric"
            <| fun _ ->
                // JSON has no number for these, so they travel as named strings.
                let cases =
                    [
                        System.Double.PositiveInfinity
                        System.Double.NegativeInfinity
                        0.0
                        -0.0
                        1.5
                        System.Double.Epsilon
                        System.Double.MaxValue
                        System.Double.MinValue
                    ]

                for expected in cases do
                    let json =
                        expected |> Encode.float |> runner.Encode.toString 0

                    let decoded = runner.Decode.fromString Decode.float json

                    equal (Ok expected) decoded

                // nan is never equal to itself, so it is checked apart.
                let json =
                    System.Double.NaN
                    |> Encode.float
                    |> runner.Encode.toString 0

                match runner.Decode.fromString Decode.float json with
                | Ok value -> equal true (System.Double.IsNaN value)
                | Error error -> failwith error

            testCase "float32 keeps nan and infinity symmetric"
            <| fun _ ->
                let cases =
                    [
                        System.Single.PositiveInfinity
                        System.Single.NegativeInfinity
                        0.0f
                        1.5f
                    ]

                for expected in cases do
                    let json =
                        expected |> Encode.float32 |> runner.Encode.toString 0

                    let decoded = runner.Decode.fromString Decode.float32 json

                    equal (Ok expected) decoded

                let json =
                    System.Single.NaN
                    |> Encode.float32
                    |> runner.Encode.toString 0

                match runner.Decode.fromString Decode.float32 json with
                | Ok value -> equal true (System.Single.IsNaN value)
                | Error error -> failwith error

            testCase "decimal is symmetric"
            <| fun _ ->
                let cases =
                    [
                        0m
                        1m
                        -1m
                        0.1m
                        123456789.123456789m
#if !FABLE_COMPILER_PYTHON
                        // Full 28 digit precision, which a float cannot hold. Fable's Python
                        // backend caps Decimal well below this.
                        79228162514264337593543950335m
                        -79228162514264337593543950335m
                        0.0000000000000000000000000001m
                        123456789012345678901234567.8m
                        System.Decimal.MaxValue
                        System.Decimal.MinValue
#endif
                    ]

                for expected in cases do
                    let json =
                        expected |> Encode.decimal |> runner.Encode.toString 0

                    let decoded = runner.Decode.fromString Decode.decimal json

                    equal (Ok expected) decoded

            testCase "bigint is symmetric"
            <| fun _ ->
                let cases =
                    [
                        bigint 0
                        bigint 1
                        bigint -1
                        bigint System.Int64.MaxValue
                        bigint System.UInt64.MaxValue
                        // Beyond anything a fixed width integer can hold.
                        bigint System.UInt64.MaxValue
                        * bigint System.UInt64.MaxValue
                        -(bigint System.UInt64.MaxValue
                          * bigint System.UInt64.MaxValue)
                    ]

                for expected in cases do
                    let json =
                        expected |> Encode.bigint |> runner.Encode.toString 0

                    let decoded = runner.Decode.fromString Decode.bigint json

                    equal (Ok expected) decoded

            testCase "the types that fit a float are written as JSON numbers"
            <| fun _ ->
                // Not a string: anything that round-trips through a 64 bit float stays a number.
                equal
                    "127"
                    (System.SByte.MaxValue
                     |> Encode.sbyte
                     |> runner.Encode.toString 0)

                equal
                    "-128"
                    (System.SByte.MinValue
                     |> Encode.sbyte
                     |> runner.Encode.toString 0)

                equal
                    "255"
                    (System.Byte.MaxValue
                     |> Encode.byte
                     |> runner.Encode.toString 0)

                equal
                    "32767"
                    (System.Int16.MaxValue
                     |> Encode.int16
                     |> runner.Encode.toString 0)

                equal
                    "-32768"
                    (System.Int16.MinValue
                     |> Encode.int16
                     |> runner.Encode.toString 0)

                equal
                    "65535"
                    (System.UInt16.MaxValue
                     |> Encode.uint16
                     |> runner.Encode.toString 0)

                equal
                    "2147483647"
                    (System.Int32.MaxValue
                     |> Encode.int
                     |> runner.Encode.toString 0)

                equal
                    "-2147483648"
                    (System.Int32.MinValue
                     |> Encode.int
                     |> runner.Encode.toString 0)

                equal
                    "4294967295"
                    (System.UInt32.MaxValue
                     |> Encode.uint32
                     |> runner.Encode.toString 0)

            testCase
                "the types that do not fit a float are written as JSON strings"
            <| fun _ ->
                equal
                    "\"9223372036854775807\""
                    (System.Int64.MaxValue
                     |> Encode.int64
                     |> runner.Encode.toString 0)

                equal
                    "\"18446744073709551615\""
                    (System.UInt64.MaxValue
                     |> Encode.uint64
                     |> runner.Encode.toString 0)

#if !FABLE_COMPILER_PYTHON
                equal
                    "\"79228162514264337593543950335\""
                    (System.Decimal.MaxValue
                     |> Encode.decimal
                     |> runner.Encode.toString 0)
#endif

            testCase "the boundary types survive a round trip"
            <| fun _ ->
                equal
                    (Ok System.Int32.MaxValue)
                    (System.Int32.MaxValue
                     |> Encode.int
                     |> runner.Encode.toString 0
                     |> runner.Decode.fromString Decode.int)

                equal
                    (Ok System.Int32.MinValue)
                    (System.Int32.MinValue
                     |> Encode.int
                     |> runner.Encode.toString 0
                     |> runner.Decode.fromString Decode.int)

                equal
                    (Ok System.UInt32.MaxValue)
                    (System.UInt32.MaxValue
                     |> Encode.uint32
                     |> runner.Encode.toString 0
                     |> runner.Decode.fromString Decode.uint32)

                equal
                    (Ok System.Int16.MaxValue)
                    (System.Int16.MaxValue
                     |> Encode.int16
                     |> runner.Encode.toString 0
                     |> runner.Decode.fromString Decode.int16)

                equal
                    (Ok System.UInt16.MaxValue)
                    (System.UInt16.MaxValue
                     |> Encode.uint16
                     |> runner.Encode.toString 0
                     |> runner.Decode.fromString Decode.uint16)

                equal
                    (Ok System.Byte.MaxValue)
                    (System.Byte.MaxValue
                     |> Encode.byte
                     |> runner.Encode.toString 0
                     |> runner.Decode.fromString Decode.byte)

                equal
                    (Ok System.SByte.MinValue)
                    (System.SByte.MinValue
                     |> Encode.sbyte
                     |> runner.Encode.toString 0
                     |> runner.Decode.fromString Decode.sbyte)

            testCase "int64 is symmetric"
            <| fun _ ->
                let cases =
                    [
                        0L
                        1L
                        2L
                        -1L
                        -2L
                        9007199254740993L
                        -9007199254740993L
                        123456789012345L
                        -123456789012345L
                        System.Int64.MaxValue
                        System.Int64.MinValue
                    ]

                for expected in cases do
                    let json =
                        expected |> Encode.int64 |> runner.Encode.toString 0

                    let decoded = runner.Decode.fromString Decode.int64 json

                    equal (Ok expected) decoded

#if !FABLE_COMPILER_PYTHON
            testCase "uint64 is symmetric"
            <| fun _ ->
                let cases =
                    [
                        0UL
                        1UL
                        2UL
                        9007199254740993UL
                        9223372036854775808UL
                        123456789012345UL
                        999999999999UL
                        System.UInt64.MaxValue
                        System.UInt64.MinValue
                    ]

                for expected in cases do
                    let json =
                        expected |> Encode.uint64 |> runner.Encode.toString 0

                    let decoded = runner.Decode.fromString Decode.uint64 json

                    equal (Ok expected) decoded
#endif
        ]
