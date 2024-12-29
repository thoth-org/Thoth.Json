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

        ]
