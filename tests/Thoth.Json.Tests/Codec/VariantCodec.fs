module Thoth.Json.Tests.Codec.VariantCodec

#if !NETFRAMEWORK
open Fable.Core
#endif

open Thoth.Json.Tests.Testing
open Fable.Pyxpecto

open Thoth.Json.Core


type Shape =
    | Square of width: int
    | Rectangle of width: int * height: int
    | Circle of radius: int

module Shape =

    let codec: Codec<Shape> =
        variantCodec {
            let! square = Codec.case "square" Square Codec.int

            and! rectangle =
                Codec.case
                    "rectangle"
                    Rectangle
                    (Codec.tuple2 Codec.int Codec.int)

            and! circle = Codec.case "circle" Circle Codec.int

            return
                function
                | Square w -> square w
                | Rectangle(w, h) -> rectangle (w, h)
                | Circle w -> circle w
        }

    let codecWithTag: Codec<Shape> =
        variantCodecWithTag "type" "value" {
            let! square = Codec.case "square" Square Codec.int

            and! rectangle =
                Codec.case
                    "rectangle"
                    Rectangle
                    (Codec.tuple2 Codec.int Codec.int)

            and! circle = Codec.case "circle" Circle Codec.int

            return
                function
                | Square w -> square w
                | Rectangle(w, h) -> rectangle (w, h)
                | Circle w -> circle w
        }

    let codecTuple: Codec<Shape> =
        variantCodecTuple {
            let! square = Codec.case "square" Square Codec.int

            and! rectangle =
                Codec.case
                    "rectangle"
                    Rectangle
                    (Codec.tuple2 Codec.int Codec.int)

            and! circle = Codec.case "circle" Circle Codec.int

            return
                function
                | Square w -> square w
                | Rectangle(w, h) -> rectangle (w, h)
                | Circle w -> circle w
        }

let tests (runner: TestRunner<'DecoderJsonValue, 'EncoderJsonValue>) =
    testList
        "VariantCodec"
        [
            test "variantCodec works for simple case" {
                let expected = Square 4

                let actual = roundTrip runner Shape.codec expected

                equal actual expected

                let expected = Rectangle(7, 2)

                let actual = roundTrip runner Shape.codec expected

                equal actual expected

                let expected = Circle 3

                let actual = roundTrip runner Shape.codec expected

                equal actual expected
            }

            test "variantCodecWithTag works for simple case" {
                let expected = Square 4

                let actual = roundTrip runner Shape.codecWithTag expected

                equal actual expected

                let expected = Rectangle(7, 2)

                let actual = roundTrip runner Shape.codecWithTag expected

                equal actual expected

                let expected = Circle 3

                let actual = roundTrip runner Shape.codecWithTag expected

                equal actual expected
            }

            test "variantCodecTuple works for simple case" {
                let expected = Square 99

                let actual = roundTrip runner Shape.codecTuple expected

                equal actual expected

                let expected = Rectangle(3, 4)

                let actual = roundTrip runner Shape.codecTuple expected

                equal actual expected

                let expected = Circle 8

                let actual = roundTrip runner Shape.codecTuple expected

                equal actual expected
            }

            test "variantCodecTuple produces correct JSON for a simple case" {
                let shape = Rectangle(3, 4)

                let actual =
                    shape
                    |> Encode.codec Shape.codecTuple
                    |> runner.Encode.toString 0

                let expected = """["rectangle",[3,4]]"""

                equal actual expected
            }

            test
                "variantCodecWithTag reports an unrecognised tag, not a missing value" {
                let expected =
                    Error(
                        "Error at: `$`\nThe following `failure` occurred with the decoder: The tag \"triangle\" was not recognized"
                    )

                // The value is absent, so the tag has to be reported before the decoder reaches for it.
                let actual =
                    runner.Decode.fromString
                        (Decode.codec Shape.codecWithTag)
                        """{"type":"triangle"}"""

                equal actual expected

                let actual =
                    runner.Decode.fromString
                        (Decode.codec Shape.codecWithTag)
                        """{"type":"triangle","value":1}"""

                equal actual expected
            }

            test
                "variantCodecTuple reports an unrecognised tag, not a missing element" {
                let expected =
                    Error(
                        "Error at: `$`\nThe following `failure` occurred with the decoder: The tag \"triangle\" was not recognized"
                    )

                let actual =
                    runner.Decode.fromString
                        (Decode.codec Shape.codecTuple)
                        """["triangle"]"""

                equal actual expected

                let actual =
                    runner.Decode.fromString
                        (Decode.codec Shape.codecTuple)
                        """["triangle",1]"""

                equal actual expected
            }
        ]
