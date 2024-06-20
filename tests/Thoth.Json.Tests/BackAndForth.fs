module Thoth.Json.Tests.BackAndForth

open Thoth.Json.Tests.Testing
open System
open Thoth.Json.Tests.Types
open Thoth.Json.Core

let tests (runner: TestRunner<_, _>) =
    runner.testList
        "Thoth.Json - BackAndForth"
        [

            runner.testCase "losslessOption is symmetric"
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

                runner.equal (Ok expected) decoded

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

                runner.equal (Ok expected) decoded

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

                runner.equal (Ok expected) decoded

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

                runner.equal (Ok expected) decoded

        ]
