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

    let codec': Codec<Shape> =
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

let tests (runner: TestRunner<'DecoderJsonValue, 'EncoderJsonValue>) =
    testList
        "VariantCodec"
        [
            test "variantCodec works for simple case 1" {
                let expected = Square 4

                let actual = roundTrip runner Shape.codec expected

                Expect.equal
                    actual
                    expected
                    "The decoded value must match the original"

                let expected = Rectangle(7, 2)

                let actual = roundTrip runner Shape.codec expected

                Expect.equal
                    actual
                    expected
                    "The decoded value must match the original"

                let expected = Circle 3

                let actual = roundTrip runner Shape.codec expected

                Expect.equal
                    actual
                    expected
                    "The decoded value must match the original"
            }

            test "variantCodec works for simple case 2" {
                let expected = Square 4

                let actual = roundTrip runner Shape.codec' expected

                Expect.equal
                    actual
                    expected
                    "The decoded value must match the original"

                let expected = Rectangle(7, 2)

                let actual = roundTrip runner Shape.codec' expected

                Expect.equal
                    actual
                    expected
                    "The decoded value must match the original"

                let expected = Circle 3

                let actual = roundTrip runner Shape.codec' expected

                Expect.equal
                    actual
                    expected
                    "The decoded value must match the original"
            }
        ]
