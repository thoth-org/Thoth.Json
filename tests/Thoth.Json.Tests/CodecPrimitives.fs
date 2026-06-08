module Thoth.Json.Tests.CodecPrimitives

open System

#if !NETFRAMEWORK
open Fable.Core
#endif

open Thoth.Json.Tests.Testing
open Fable.Pyxpecto

open Thoth.Json.Core

let tests (runner: TestRunner<'DecoderJsonValue, 'EncoderJsonValue>) =
    testList
        "Primitives"
        [
            test "Codec.int works for simple case 1" {
                let expected = 123

                let actual = roundTrip runner Codec.int expected

                Expect.equal
                    actual
                    expected
                    "The decoded value must match the original"
            }

            test "Codec.string works for simple case 1" {
                let expected = "Hello, world. "

                let actual = roundTrip runner Codec.string expected

                Expect.equal
                    actual
                    expected
                    "The decoded value must match the original"
            }

            test "Codec.bool works for simple case 1" {
                let expected = true

                let actual = roundTrip runner Codec.bool expected

                Expect.equal
                    actual
                    expected
                    "The decoded value must match the original"
            }

            test "Codec.guid works for simple case 1" {
                let expected = Guid.Parse "11850f17-351a-436c-8358-b28eb85a52a6"

                let actual = roundTrip runner Codec.guid expected

                Expect.equal
                    actual
                    expected
                    "The decoded value must match the original"
            }

#if !FABLE_COMPILER_PYTHON
            test "Codec.datetimeOffset works for simple case 1" {
                let expected = DateTimeOffset.Parse "2022-05-23T07:45:39.700Z"

                let actual = roundTrip runner Codec.datetimeOffset expected

                Expect.equal
                    actual
                    expected
                    "The decoded value must match the original"
            }
#endif
        ]
