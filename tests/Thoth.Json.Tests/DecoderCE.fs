module Thoth.Json.Tests.DecoderCE

open Thoth.Json.Tests.Testing
open Thoth.Json.Core
open Fable.Pyxpecto

let tests (runner: TestRunner<_, _>) =
    testList
        "Thoth.Json - DecoderCE"
        [

            testCase "decoder computation expression works for bind"
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

            testCase
                "decoder computation expression works for and! applicative bind"
            <| fun _ ->
                let expected = 123, "abc", true

                let json =
                    Encode.object
                        [
                            "x", Encode.int 123
                            "y", Encode.string "abc"
                            "z", Encode.bool true
                        ]
                    |> runner.Encode.toString 0

                let decoded =
                    runner.Decode.fromString
                        (decoder {
                            let! x = Decode.field "x" Decode.int
                            and! y = Decode.field "y" Decode.string
                            and! z = Decode.field "z" Decode.bool

                            return x, y, z
                        })
                        json

                equal (Ok expected) decoded

            testCase "decoder CE supports mixed let! and and!"
            <| fun _ ->
                let expected = 42, "hello", 3.14

                let json =
                    Encode.object
                        [
                            "a", Encode.int 42
                            "b", Encode.string "hello"
                            "c", Encode.float 3.14
                        ]
                    |> runner.Encode.toString 0

                let decoded =
                    runner.Decode.fromString
                        (decoder {
                            let! a = Decode.field "a" Decode.int
                            and! b = Decode.field "b" Decode.string
                            and! c = Decode.field "c" Decode.float

                            return a, b, c
                        })
                        json

                equal (Ok expected) decoded

        ]
