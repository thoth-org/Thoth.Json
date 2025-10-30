module Thoth.Json.Tests.Syntax

open Thoth.Json.Tests.Testing
open Thoth.Json.Core
open Fable.Pyxpecto

let tests (runner: TestRunner<_, _>) =
    testList
        "Thoth.Json - Syntax"
        [

            testCase "decoder syntax works for bind"
            <| fun _ ->
                let expected = 123, "abc"

                let json =
                    Encode.object
                        [
                            "x", Encode.int 123
                            "y", Encode.string "abc"
                        ]
                    |> runner.Encode.toString 0

                let decoded =
                    runner.Decode.fromString
                        (decoder {
                            let! x = Decode.field "x" Decode.int
                            let! y = Decode.field "y" Decode.string

                            return x, y
                        })
                        json

                equal (Ok expected) decoded

        ]
