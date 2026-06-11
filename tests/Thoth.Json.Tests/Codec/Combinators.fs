module Thoth.Json.Tests.Codec.Combinators

#if !NETFRAMEWORK
open Fable.Core
#endif

open Thoth.Json.Tests.Testing
open Fable.Pyxpecto

open Thoth.Json.Core

let tests (runner: TestRunner<'DecoderJsonValue, 'EncoderJsonValue>) =
    testList
        "Combinators"
        [
            test "Codec.lossyOption works for simple case 1" {
                let codec = Codec.lossyOption Codec.string

                let expected = Some "abc"

                let actual = roundTrip runner codec expected

                Expect.equal
                    actual
                    expected
                    "The decoded value must match the original"
            }

            test "Codec.lossyOption works for simple case 2" {
                let codec = Codec.lossyOption Codec.string

                let expected = None

                let actual = roundTrip runner codec expected

                Expect.equal
                    actual
                    expected
                    "The decoded value must match the original"
            }

            test "Codec.list works for simple case 1" {
                let codec = Codec.list Codec.int

                let expected =
                    [
                        4
                        6
                        1
                        2
                        2
                        8
                        0
                        5
                        3
                    ]

                let actual = roundTrip runner codec expected

                Expect.equal
                    actual
                    expected
                    "The decoded value must match the original"
            }

            test "Codec.array works for simple case 1" {
                let codec = Codec.array Codec.string

                let expected =
                    [|
                        "the"
                        "quick"
                        "brown"
                        "fox"
                        "jumped"
                        "over"
                        "the"
                        "lazy"
                        "dog"
                    |]

                let actual = roundTrip runner codec expected

                Expect.equal
                    actual
                    expected
                    "The decoded value must match the original"
            }
        ]
