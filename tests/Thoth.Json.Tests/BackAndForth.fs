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

            testCase "int64 is symmetric"
            <| fun _ ->
                let cases =
                    [
                        0L
                        1L
                        2L
                        System.Int64.MaxValue
                        System.Int64.MinValue
                    ]

                for expected in cases do
                    let json =
                        expected |> Encode.int64 |> runner.Encode.toString 0

                    let decoded = runner.Decode.fromString Decode.int64 json

                    equal (Ok expected) decoded

            testCase "uint64 is symmetric"
            <| fun _ ->
                let cases =
                    [
                        0UL
                        1UL
                        2UL
                        System.UInt64.MaxValue
                        System.UInt64.MinValue
                    ]

                for expected in cases do
                    let json =
                        expected |> Encode.uint64 |> runner.Encode.toString 0

                    let decoded = runner.Decode.fromString Decode.uint64 json

                    equal (Ok expected) decoded
        ]
