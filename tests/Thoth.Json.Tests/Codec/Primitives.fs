module Thoth.Json.Tests.Codec.Primitives

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

                equal actual expected
            }

            test "Codec.string works for simple case 1" {
                let expected = "Hello, world. "

                let actual = roundTrip runner Codec.string expected

                equal actual expected
            }

            test "Codec.char works for simple case 1" {
                let expected = 'c'

                let actual = roundTrip runner Codec.char expected

                equal actual expected
            }

            test "Codec.bool works for simple case 1" {
                let expected = true

                let actual = roundTrip runner Codec.bool expected

                equal actual expected
            }

            test "Codec.guid works for simple case 1" {
                let expected = Guid.Parse "11850f17-351a-436c-8358-b28eb85a52a6"

                let actual = roundTrip runner Codec.guid expected

                equal actual expected
            }

            test "Codec.uri works for simple case 1" {
                let expected = Uri "http://example.com/path?q=1"

                let actual = roundTrip runner Codec.uri expected

                // Compare on OriginalString because Fable's Uri does not
                // implement structural equality
                equal actual.OriginalString expected.OriginalString
            }

            test "Codec.unit works for simple case 1" {
                let expected = ()

                let actual = roundTrip runner Codec.unit expected

                equal actual expected
            }

            test "Codec.sbyte works for simple case 1" {
                let expected = 99y

                let actual = roundTrip runner Codec.sbyte expected

                equal actual expected
            }

            test "Codec.byte works for simple case 1" {
                let expected = 99uy

                let actual = roundTrip runner Codec.byte expected

                equal actual expected
            }

            test "Codec.int16 works for simple case 1" {
                let expected = 999s

                let actual = roundTrip runner Codec.int16 expected

                equal actual expected
            }

            test "Codec.uint16 works for simple case 1" {
                let expected = 999us

                let actual = roundTrip runner Codec.uint16 expected

                equal actual expected
            }

            test "Codec.uint32 works for simple case 1" {
                let expected = 999u

                let actual = roundTrip runner Codec.uint32 expected

                equal actual expected
            }

            test "Codec.int64 works for simple case 1" {
                let expected = 7923209L

                let actual = roundTrip runner Codec.int64 expected

                equal actual expected
            }

            test "Codec.uint64 works for simple case 1" {
                let expected = 7923209UL

                let actual = roundTrip runner Codec.uint64 expected

                equal actual expected
            }

            test "Codec.bigint works for simple case 1" {
                let expected = 12I

                let actual = roundTrip runner Codec.bigint expected

                equal actual expected
            }

            test "Codec.float works for simple case 1" {
                let expected = 42.5

                let actual = roundTrip runner Codec.float expected

                equal actual expected
            }

            test "Codec.float32 works for simple case 1" {
                let expected = 42.5f

                let actual = roundTrip runner Codec.float32 expected

                equal actual expected
            }

            test "Codec.decimal works for simple case 1" {
                let expected = 1234.75M

                let actual = roundTrip runner Codec.decimal expected

                equal actual expected
            }

            test "Codec.timespan works for simple case 1" {
                let expected = TimeSpan(23, 45, 0)

                let actual = roundTrip runner Codec.timespan expected

                equal actual expected
            }

#if !FABLE_COMPILER_PYTHON
            test "Codec.datetimeOffset works for simple case 1" {
                let expected = DateTimeOffset.Parse "2022-05-23T07:45:39.700Z"

                let actual = roundTrip runner Codec.datetimeOffset expected

                equal actual expected
            }
#endif
        ]
