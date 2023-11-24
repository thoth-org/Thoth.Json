module Thoth.Json.Tests.Decoders

#if !NETFRAMEWORK
open Fable.Core
#endif

open Fable.Core.JsInterop
open Thoth.Json.Tests.Testing
open System

open Thoth.Json.Core

let jsonRecord =
    """{ "a": 1.0,
         "b": 2.0,
         "c": 3.0,
         "d": 4.0,
         "e": 5.0,
         "f": 6.0,
         "g": 7.0,
         "h": 8.0 }"""

let jsonRecordInvalid =
    """{ "a": "invalid_a_field",
         "b": "invalid_a_field",
         "c": "invalid_a_field",
         "d": "invalid_a_field",
         "e": "invalid_a_field",
         "f": "invalid_a_field",
         "g": "invalid_a_field",
         "h": "invalid_a_field" }"""

open Thoth.Json.Tests.Types

type RecordWithPrivateConstructor = private { Foo1: int; Foo2: float }
type UnionWithPrivateConstructor = private Bar of string | Baz
type UnionWithMultipleFields = Multi of string * int * float

let tests (runner : TestRunner<_, _>) =
    runner.testList "Thoth.Json.Decode" [

        runner.testList "Errors" [

            runner.testCase "invalid json" <| fun _ ->
                #if FABLE_COMPILER_JAVASCRIPT
                let expected : Result<float, string> = Error "Given an invalid JSON: Unexpected token 'm', \"maxime\" is not valid JSON"
                #endif

                #if FABLE_COMPILER_PYTHON
                let expected : Result<float, string> = Error "Given an invalid JSON: Expecting value: line 1 column 1 (char 0)"
                #endif

                #if !FABLE_COMPILER
                let expected : Result<float, string> = Error "Given an invalid JSON: Unexpected character encountered while parsing value: m. Path '', line 0, position 0."
                #endif

                let actual = runner.Decode.fromString Decode.float "maxime"

                runner.equal expected actual

            // runner.testCase "invalid json #2 - Special case for Thoth.Json.Net" <| fun _ ->
            //     // See: https://github.com/thoth-org/Thoth.Json.Net/issues/42
            //     #if FABLE_COMPILER
            //     let expected : Result<MyUnion, string> = Error "Given an invalid JSON: Unexpected token , in JSON at position 5"
            //     #else
            //     let expected : Result<MyUnion, string> = Error "Given an invalid JSON: Additional text encountered after finished reading JSON content: ,. Path '', line 1, position 5."
            //     #endif
            //     let actual = Decode.Auto.fromString<MyUnion>(""""Foo","42"]""")

            //     runner.equal expected actual

            runner.testCase "invalid json #3 - Special case for Thoth.Json.Net" <| fun _ ->
                // See: https://github.com/thoth-org/Thoth.Json.Net/pull/48
                #if FABLE_COMPILER_JAVASCRIPT
                let expected : Result<float, string> = Error "Given an invalid JSON: Expected double-quoted property name in JSON at position 172"
                #endif

                #if FABLE_COMPILER_PYTHON
                let expected : Result<float, string> = Error "Given an invalid JSON: Expecting property name enclosed in double quotes: line 8 column 17 (char 172)"
                #endif

                #if !FABLE_COMPILER
                let expected : Result<float, string> = Error "Given an invalid JSON: Unexpected end when reading token. Path 'Ab[1]'."
                #endif

                let incorrectJson = """
                {
                "Ab": [
                    "RecordC",
                    {
                    "C1": "",
                    "C2": "",
                """

                let actual = runner.Decode.fromString Decode.float incorrectJson

                runner.equal expected actual

            runner.testCase "user exceptions are not captured by the decoders" <| fun _ ->
                let expected = true

                let decoder =
                    { new Decoder<'a> with
                        member _.Decode(_, _, _) = raise CustomException
                    }

                let actual =
                    try
                        runner.Decode.fromString decoder "\"maxime\""
                        |> ignore // Ignore the result as we only want to trigger the decoder and capture the exception
                        false
                    with
                        | CustomException ->
                            true

                runner.equal expected actual
        ]

        runner.testList "Primitives" [

            runner.testCase "unit works" <| fun _ ->
                let expected = Ok ()
                let actual =
                    runner.Decode.fromString Decode.unit "null"

                runner.equal expected actual

            runner.testCase "a string works" <| fun _ ->
                let expected = Ok("maxime")
                let actual =
                    runner.Decode.fromString Decode.string "\"maxime\""

                runner.equal expected actual

            runner.testCase "a string with new line works" <| fun _ ->
                let expected = Ok("a\nb")
                let actual =
                    runner.Decode.fromString Decode.string "\"a\\nb\""

                runner.equal expected actual

            runner.testCase "a string with new line character works" <| fun _ ->
                let expected = Ok("a\\nb")
                let actual =
                    runner.Decode.fromString Decode.string "\"a\\\\nb\""

                runner.equal expected actual

            runner.testCase "a string with tab works" <| fun _ ->
                let expected = Ok("a\tb")
                let actual =
                    runner.Decode.fromString Decode.string "\"a\\tb\""

                runner.equal expected actual

            runner.testCase "a string with tab character works" <| fun _ ->
                let expected = Ok("a\\tb")
                let actual =
                    runner.Decode.fromString Decode.string "\"a\\\\tb\""

                runner.equal expected actual

            runner.testCase "a char works" <| fun _ ->
                let expected = Ok('a')
                let actual =
                    runner.Decode.fromString Decode.char "\"a\""

                runner.equal expected actual

            runner.testCase "a char reports an error if there are more than 1 characters in the string" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting a single character string but instead got: "ab"
                        """.Trim())

                let actual =
                    runner.Decode.fromString Decode.char "\"ab\""

                runner.equal expected actual

            runner.testCase "a float works" <| fun _ ->
                let expected = Ok(1.2)
                let actual =
                    runner.Decode.fromString Decode.float "1.2"

                runner.equal expected actual

            runner.testCase "a float from int works" <| fun _ ->
                let expected = Ok(1.0)
                let actual =
                    runner.Decode.fromString Decode.float "1"

                runner.equal expected actual

            runner.testCase "a bool works" <| fun _ ->
                let expected = Ok(true)
                let actual =
                    runner.Decode.fromString Decode.bool "true"

                runner.equal expected actual

            runner.testCase "an invalid bool output an error" <| fun _ ->
                let expected = Error("Error at: `$`\nExpecting a boolean but instead got: 2")
                let actual =
                    runner.Decode.fromString Decode.bool "2"

                runner.equal expected actual

            runner.testCase "an int works" <| fun _ ->
                let expected = Ok(25)
                let actual =
                    runner.Decode.fromString Decode.int "25"

                runner.equal expected actual

            runner.testCase "an invalid int [invalid range: too big] output an error" <| fun _ ->
                let expected = Error("Error at: `$`\nExpecting an int but instead got: 2147483648\nReason: Value was either too large or too small for an int")
                let actual =
                    runner.Decode.fromString Decode.int "2147483648"

                runner.equal expected actual


            runner.testCase "an invalid int [invalid range: too small] output an error" <| fun _ ->
                let expected = Error("Error at: `$`\nExpecting an int but instead got: -2147483649\nReason: Value was either too large or too small for an int")
                let actual =
                    runner.Decode.fromString Decode.int "-2147483649"

                runner.equal expected actual

            runner.testCase "an int16 works from number" <| fun _ ->
                let expected = Ok(int16 25)
                let actual =
                    runner.Decode.fromString Decode.int16 "25"

                runner.equal expected actual

            runner.testCase "an int16 works from string" <| fun _ ->
                let expected = Ok(int16 -25)
                let actual =
                    runner.Decode.fromString Decode.int16 "\"-25\""

                runner.equal expected actual

            runner.testCase "an int16 output an error if value is too big" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting an int16 but instead got: 32768
Reason: Value was either too large or too small for an int16
                        """.Trim())
                let actual =
                    runner.Decode.fromString Decode.int16 "32768"

                runner.equal expected actual

            runner.testCase "an int16 output an error if value is too small" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting an int16 but instead got: -32769
Reason: Value was either too large or too small for an int16
                        """.Trim())
                let actual =
                    runner.Decode.fromString Decode.int16 "-32769"

                runner.equal expected actual

            runner.testCase "an int16 output an error if incorrect string" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting an int16 but instead got: "maxime"
                        """.Trim())
                let actual =
                    runner.Decode.fromString Decode.int16 "\"maxime\""

                runner.equal expected actual

            runner.testCase "an uint16 works from number" <| fun _ ->
                let expected = Ok(uint16 25)
                let actual =
                    runner.Decode.fromString Decode.uint16 "25"

                runner.equal expected actual

            runner.testCase "an uint16 works from string" <| fun _ ->
                let expected = Ok(uint16 25)
                let actual =
                    runner.Decode.fromString Decode.uint16 "\"25\""

                runner.equal expected actual

            runner.testCase "an uint16 output an error if value is too big" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting an uint16 but instead got: 65536
Reason: Value was either too large or too small for an uint16
                        """.Trim())
                let actual =
                    runner.Decode.fromString Decode.uint16 "65536"

                runner.equal expected actual

            runner.testCase "an uint16 output an error if value is too small" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting an uint16 but instead got: -1
Reason: Value was either too large or too small for an uint16
                        """.Trim())
                let actual =
                    runner.Decode.fromString Decode.uint16 "-1"

                runner.equal expected actual

            runner.testCase "an uint16 output an error if incorrect string" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting an uint16 but instead got: "maxime"
                        """.Trim())
                let actual =
                    runner.Decode.fromString Decode.uint16 "\"maxime\""

                runner.equal expected actual

            runner.testCase "an int64 works from number" <| fun _ ->
                let expected = Ok 1000L
                let actual =
                    runner.Decode.fromString Decode.int64 "1000"

                runner.equal expected actual

            runner.testCase "an int64 works from string" <| fun _ ->
                let expected = Ok 99L
                let actual =
                    runner.Decode.fromString Decode.int64 "\"99\""

                runner.equal expected actual

            runner.testCase "an int64 works output an error if incorrect string" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting an int64 but instead got: "maxime"
                        """.Trim())
                let actual =
                    runner.Decode.fromString Decode.int64 "\"maxime\""

                runner.equal expected actual

            runner.testCase "an uint32 works from number" <| fun _ ->
                let expected = Ok 1000u
                let actual =
                    runner.Decode.fromString Decode.uint32 "1000"

                runner.equal expected actual

            runner.testCase "an uint32 works from string" <| fun _ ->
                let expected = Ok 1000u
                let actual =
                    runner.Decode.fromString Decode.uint32 "\"1000\""

                runner.equal expected actual

            runner.testCase "an uint32 output an error if incorrect string" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting an uint32 but instead got: "maxime"
                        """.Trim())

                let actual =
                    runner.Decode.fromString Decode.uint32 "\"maxime\""

                runner.equal expected actual

            runner.testCase "an uint64 works from number" <| fun _ ->
                let expected = Ok 1000UL
                let actual =
                    runner.Decode.fromString Decode.uint64 "1000"

                runner.equal expected actual

            runner.testCase "an uint64 works from string" <| fun _ ->
                let expected = Ok 1000UL
                let actual =
                    runner.Decode.fromString Decode.uint64 "\"1000\""

                runner.equal expected actual

            runner.testCase "an uint64 output an error if incorrect string" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting an uint64 but instead got: "maxime"
                        """.Trim())

                let actual =
                    runner.Decode.fromString Decode.uint64 "\"maxime\""

                runner.equal expected actual

            runner.testCase "a byte works from number" <| fun _ ->
                let expected = Ok 25uy
                let actual =
                    runner.Decode.fromString Decode.byte "25"

                runner.equal expected actual

            runner.testCase "a byte works from string" <| fun _ ->
                let expected = Ok 25uy
                let actual =
                    runner.Decode.fromString Decode.byte "\"25\""

                runner.equal expected actual

            runner.testCase "a byte output an error if value is too big" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting a byte but instead got: 256
Reason: Value was either too large or too small for a byte
                        """.Trim())
                let actual =
                    runner.Decode.fromString Decode.byte "256"

                runner.equal expected actual

            runner.testCase "a byte output an error if value is too small" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting a byte but instead got: -1
Reason: Value was either too large or too small for a byte
                        """.Trim())
                let actual =
                    runner.Decode.fromString Decode.byte "-1"

                runner.equal expected actual

            runner.testCase "a byte output an error if incorrect string" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting a byte but instead got: "maxime"
                        """.Trim())
                let actual =
                    runner.Decode.fromString Decode.byte "\"maxime\""

                runner.equal expected actual


            runner.testCase "a sbyte works from number" <| fun _ ->
                let expected = Ok 25y
                let actual =
                    runner.Decode.fromString Decode.sbyte "25"

                runner.equal expected actual

            runner.testCase "a sbyte works from string" <| fun _ ->
                let expected = Ok -25y
                let actual =
                    runner.Decode.fromString Decode.sbyte "\"-25\""

                runner.equal expected actual

            runner.testCase "a sbyte output an error if value is too big" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting a sbyte but instead got: 128
Reason: Value was either too large or too small for a sbyte
                        """.Trim())
                let actual =
                    runner.Decode.fromString Decode.sbyte "128"

                runner.equal expected actual

            runner.testCase "a sbyte output an error if value is too small" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting a sbyte but instead got: -129
Reason: Value was either too large or too small for a sbyte
                        """.Trim())
                let actual =
                    runner.Decode.fromString Decode.sbyte "-129"

                runner.equal expected actual

            runner.testCase "a sbyte output an error if incorrect string" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting a sbyte but instead got: "maxime"
                        """.Trim())
                let actual =
                    runner.Decode.fromString Decode.sbyte "\"maxime\""

                runner.equal expected actual

            runner.testCase "an bigint works from number" <| fun _ ->
                let expected = Ok 12I
                let actual =
                    runner.Decode.fromString Decode.bigint "12"

                runner.equal expected actual

            runner.testCase "an bigint works from string" <| fun _ ->
                let expected = Ok 12I
                let actual =
                    runner.Decode.fromString Decode.bigint "\"12\""

                runner.equal expected actual

            runner.testCase "an bigint output an error if invalid string" <| fun _ ->
                let expected =
                    Error (
                        """
Error at: `$`
Expecting a bigint but instead got: "maxime"
                        """.Trim())
                let actual =
                    runner.Decode.fromString Decode.bigint "\"maxime\""

                runner.equal expected actual

            runner.testCase "a string representing a DateTime should be accepted as a string" <| fun _ ->
                let expected = "2018-10-01T11:12:55.00Z"
                let actual =
                    runner.Decode.fromString Decode.string "\"2018-10-01T11:12:55.00Z\""

                runner.equal (Ok expected) actual

            #if !FABLE_COMPILER_PYTHON
            runner.testCase "a datetime works" <| fun _ ->
                let expected = new DateTime(2018, 10, 1, 11, 12, 55, DateTimeKind.Utc)
                let actual =
                    runner.Decode.fromString Decode.datetimeUtc "\"2018-10-01T11:12:55.00Z\""

                runner.equal (Ok expected) actual
            #endif

            runner.testCase "a non-UTC datetime works" <| fun _ ->
                let expected = new DateTime(2018, 10, 1, 11, 12, 55)
                let actual =
                    runner.Decode.fromString Decode.datetimeLocal "\"2018-10-01T11:12:55\""

                runner.equal (Ok expected) actual

            #if !FABLE_COMPILER_PYTHON
            runner.testCase "a datetime output an error if invalid string" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting a datetime but instead got: "invalid_string"
                        """.Trim())

                let actual =
                    runner.Decode.fromString Decode.datetimeUtc "\"invalid_string\""

                runner.equal expected actual

            runner.testCase "a datetime works with TimeZone" <| fun _ ->
                let localDate = DateTime(2018, 10, 1, 11, 12, 55, DateTimeKind.Local)

                let expected = Ok (localDate.ToUniversalTime())
                let json = sprintf "\"%s\"" (localDate.ToString("O"))
                let actual =
                    runner.Decode.fromString Decode.datetimeUtc json

                runner.equal expected actual
            #endif

            runner.testCase "a datetimeOffset works" <| fun _ ->
                let expected =
                    DateTimeOffset(2018, 7, 2, 12, 23, 45, 0, TimeSpan.FromHours(2.))
                    |> Ok
                let json = "\"2018-07-02T12:23:45+02:00\""
                let actual =
                    runner.Decode.fromString Decode.datetimeOffset json
                runner.equal expected actual

            runner.testCase "a datetimeOffset returns Error if invalid format" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting a datetimeoffset but instead got: "NOT A DATETIMEOFFSET"
                        """.Trim())
                let json = "\"NOT A DATETIMEOFFSET\""
                let actual =
                    runner.Decode.fromString Decode.datetimeOffset json

                runner.equal expected actual

            #if !FABLE_COMPILER_PYTHON
            runner.testCase "a timespan works" <| fun _ ->
                let expected =
                    TimeSpan(23, 45, 0)
                    |> Ok
                let json = "\"23:45:00\""
                let actual =
                    runner.Decode.fromString Decode.timespan json
                runner.equal expected actual

            runner.testCase "a timespan returns Error if invalid format" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting a timespan but instead got: "NOT A TimeSpan"
                        """.Trim())
                let json = "\"NOT A TimeSpan\""
                let actual =
                    runner.Decode.fromString Decode.timespan json

                runner.equal expected actual
            #endif

            runner.testCase "an enum<sbyte> works" <| fun _ ->
                let expected = Ok Enum_Int8.NinetyNine
                let actual =
                    runner.Decode.fromString Decode.Enum.sbyte "99"

                runner.equal expected actual

            runner.testCase "an enum<byte> works" <| fun _ ->
                let expected = Ok Enum_UInt8.NinetyNine
                let actual =
                    runner.Decode.fromString Decode.Enum.byte "99"

                runner.equal expected actual

            runner.testCase "an enum<int> works" <| fun _ ->
                let expected = Ok Enum_Int.One
                let actual =
                    runner.Decode.fromString Decode.Enum.int "1"

                runner.equal expected actual

            runner.testCase "an enum<uint32> works" <| fun _ ->
                let expected = Ok Enum_UInt32.NinetyNine
                let actual =
                    runner.Decode.fromString Decode.Enum.uint32 "99"

                runner.equal expected actual

            runner.testCase "an enum<int16> works" <| fun _ ->
                let expected = Ok Enum_Int16.NinetyNine
                let actual =
                    runner.Decode.fromString Decode.Enum.int16 "99"

                runner.equal expected actual

            runner.testCase "an enum<uint16> works" <| fun _ ->
                let expected = Ok Enum_UInt16.NinetyNine
                let actual =
                    runner.Decode.fromString Decode.Enum.uint16 "99"

                runner.equal expected actual

        ]

        runner.testList "Tuples" [
            runner.testCase "tuple2 works" <| fun _ ->
                let json = """[1, "maxime"]"""
                let expected = Ok(1, "maxime")

                let actual =
                    runner.Decode.fromString (Decode.tuple2 Decode.int Decode.string) json

                runner.equal expected actual

            runner.testCase "tuple3 works" <| fun _ ->
                let json = """[1, "maxime", 2.5]"""
                let expected = Ok(1, "maxime", 2.5)

                let actual =
                    runner.Decode.fromString
                        (Decode.tuple3
                            Decode.int
                            Decode.string
                            Decode.float) json

                runner.equal expected actual

            runner.testCase "tuple4 works" <| fun _ ->
                let json = """[1, "maxime", 2.5, { "fieldA" : "test" }]"""
                let expected = Ok(1, "maxime", 2.5, { fieldA = "test" })

                let actual =
                    runner.Decode.fromString
                        (Decode.tuple4
                            Decode.int
                            Decode.string
                            Decode.float
                            SmallRecord.Decoder) json

                runner.equal expected actual

            runner.testCase "tuple5 works" <| fun _ ->
                let json = """[1, "maxime", 2.5, { "fieldA" : "test" }, false]"""
                let expected = Ok(1, "maxime", 2.5, { fieldA = "test" }, false)

                let actual =
                    runner.Decode.fromString
                        (Decode.tuple5
                            Decode.int
                            Decode.string
                            Decode.float
                            SmallRecord.Decoder
                            Decode.bool) json

                runner.equal expected actual

            runner.testCase "tuple6 works" <| fun _ ->
                let json = """[1, "maxime", 2.5, { "fieldA" : "test" }, false, null]"""
                let expected = Ok(1, "maxime", 2.5, { fieldA = "test" }, false, null)

                let actual =
                    runner.Decode.fromString
                        (Decode.tuple6
                            Decode.int
                            Decode.string
                            Decode.float
                            SmallRecord.Decoder
                            Decode.bool
                            (Decode.nil null)) json

                runner.equal expected actual

            runner.testCase "tuple7 works" <| fun _ ->
                let json = """[1, "maxime", 2.5, { "fieldA" : "test" }, false, null, 56]"""
                let expected = Ok(1, "maxime", 2.5, { fieldA = "test" }, false, null, 56)

                let actual =
                    runner.Decode.fromString
                        (Decode.tuple7
                            Decode.int
                            Decode.string
                            Decode.float
                            SmallRecord.Decoder
                            Decode.bool
                            (Decode.nil null)
                            Decode.int) json

                runner.equal expected actual

            runner.testCase "tuple8 works" <| fun _ ->
                let json = """[1, "maxime", 2.5, { "fieldA" : "test" }, false, null, true, 98]"""
                let expected = Ok(1, "maxime", 2.5, { fieldA = "test" }, false, null, true, 98)

                let actual =
                    runner.Decode.fromString
                        (Decode.tuple8
                            Decode.int
                            Decode.string
                            Decode.float
                            SmallRecord.Decoder
                            Decode.bool
                            (Decode.nil null)
                            Decode.bool
                            Decode.int) json

                runner.equal expected actual

            runner.testCase "tuple2 returns an error if invalid json" <| fun _ ->
                let json = """[1, false, "unused value"]"""
                let expected =
                    Error(
                        """
Error at: `$.[1]`
Expecting a string but instead got: false
                        """.Trim())

                let actual =
                    runner.Decode.fromString
                        (Decode.tuple2
                            Decode.int
                            Decode.string) json

                runner.equal expected actual

            runner.testCase "tuple3 returns an error if invalid json" <| fun _ ->
                let json = """[1, "maxime", false]"""
                let expected =
                    Error(
                        """
Error at: `$.[2]`
Expecting a float but instead got: false
                        """.Trim())

                let actual =
                    runner.Decode.fromString
                        (Decode.tuple3
                            Decode.int
                            Decode.string
                            Decode.float) json

                runner.equal expected actual

            runner.testCase "tuple4 returns an error if invalid json (missing index)" <| fun _ ->
                let json = """[1, "maxime", 2.5]"""
                let expected =
                    Error(
                        """
Error at: `$.[3]`
Expecting a longer array. Need index `3` but there are only `3` entries.
[
    1,
    "maxime",
    2.5
]
                        """.Trim())

                let actual =
                    runner.Decode.fromString
                        (Decode.tuple4
                            Decode.int
                            Decode.string
                            Decode.float
                            SmallRecord.Decoder) json

                runner.equal expected actual

            runner.testCase "tuple4 returns an error if invalid json (error in the nested object)" <| fun _ ->
                let json = """[1, "maxime", 2.5, { "fieldA" : false }]"""
                let expected =
                    Error(
                        """
Error at: `$.[3].fieldA`
Expecting a string but instead got: false
                        """.Trim())

                let actual =
                    runner.Decode.fromString
                        (Decode.tuple4
                            Decode.int
                            Decode.string
                            Decode.float
                            SmallRecord.Decoder) json

                runner.equal expected actual
            #if !FABLE_COMPILER_PYTHON
            runner.testCase "tuple5 returns an error if invalid json" <| fun _ ->
                let json = """[1, "maxime", 2.5, { "fieldA" : "test" }, false]"""
                let expected =
                    Error(
                        """
Error at: `$.[4]`
Expecting a datetime but instead got: false
                        """.Trim())

                let actual =
                    runner.Decode.fromString
                        (Decode.tuple5
                            Decode.int
                            Decode.string
                            Decode.float
                            SmallRecord.Decoder
                            Decode.datetimeUtc) json

                runner.equal expected actual

            runner.testCase "tuple6 returns an error if invalid json" <| fun _ ->
                let json = """[1, "maxime", 2.5, { "fieldA" : "test" }, "2018-10-01T11:12:55.00Z", false]"""
                let expected =
                    Error(
                        """
Error at: `$.[5]`
Expecting null but instead got: false
                        """.Trim())

                let actual =
                    runner.Decode.fromString
                        (Decode.tuple6
                            Decode.int
                            Decode.string
                            Decode.float
                            SmallRecord.Decoder
                            Decode.datetimeUtc
                            (Decode.nil null)) json

                runner.equal expected actual

            runner.testCase "tuple7 returns an error if invalid json" <| fun _ ->
                let json = """[1, "maxime", 2.5, { "fieldA" : "test" }, "2018-10-01T11:12:55.00Z", null, false]"""
                let expected =
                    Error(
                        """
Error at: `$.[6]`
Expecting an int but instead got: false
                        """.Trim())

                let actual =
                    runner.Decode.fromString
                        (Decode.tuple7
                            Decode.int
                            Decode.string
                            Decode.float
                            SmallRecord.Decoder
                            Decode.datetimeUtc
                            (Decode.nil null)
                            Decode.int) json

                runner.equal expected actual

            runner.testCase "tuple8 returns an error if invalid json" <| fun _ ->
                let json = """[1, "maxime", 2.5, { "fieldA" : "test" }, "2018-10-01T11:12:55.00Z", null, 56, "maxime"]"""
                let expected =
                    Error(
                        """
Error at: `$.[7]`
Expecting an int but instead got: "maxime"
                        """.Trim())

                let actual =
                    runner.Decode.fromString
                        (Decode.tuple8
                            Decode.int
                            Decode.string
                            Decode.float
                            SmallRecord.Decoder
                            Decode.datetimeUtc
                            (Decode.nil null)
                            Decode.int
                            Decode.int) json

                runner.equal expected actual
            #endif
        ]

        runner.testList "Object primitives" [

            runner.testCase "field works" <| fun _ ->
                let json = """{ "name": "maxime", "age": 25 }"""
                let expected = Ok("maxime")

                let actual =
                    runner.Decode.fromString (Decode.field "name" Decode.string) json

                runner.equal expected actual

            runner.testCase "field output an error explaining why the value is considered invalid" <| fun _ ->
                let json = """{ "name": null, "age": 25 }"""
                let expected =
                    Error(
                        """
Error at: `$.name`
Expecting an int but instead got: null
                        """.Trim()
                    )

                let actual =
                    runner.Decode.fromString (Decode.field "name" Decode.int) json

                runner.equal expected actual

            runner.testCase "field output an error when field is missing" <| fun _ ->
                let json = """{ "name": "maxime", "age": 25 }"""
                let expected =
                    Error(
                        """
Error at: `$`
Expecting an object with a field named `height` but instead got:
{
    "name": "maxime",
    "age": 25
}
                        """.Trim())

                let actual =
                    runner.Decode.fromString (Decode.field "height" Decode.float) json

                runner.equal expected actual

            runner.testCase "at works" <| fun _ ->

                let json = """{ "user": { "name": "maxime", "age": 25 } }"""
                let expected = Ok "maxime"

                let actual =
                    runner.Decode.fromString (Decode.at ["user"; "name"] Decode.string) json

                runner.equal expected actual

            runner.testCase "at output an error if the path failed" <| fun _ ->
                let json = """{ "user": { "name": "maxime", "age": 25 } }"""
                let expected =
                    Error(
                        """
Error at: `$.user.firstname`
Expecting an object with path `user.firstname` but instead got:
{
    "user": {
        "name": "maxime",
        "age": 25
    }
}
Node `firstname` is unkown.
                        """.Trim())

                let actual =
                    runner.Decode.fromString (Decode.at ["user"; "firstname"] Decode.string) json

                runner.equal expected actual

            runner.testCase "at output an error explaining why the value is considered invalid" <| fun _ ->
                let json = """{ "name": null, "age": 25 }"""
                let expected =
                    Error(
                        """
Error at: `$.name`
Expecting an int but instead got: null
                        """.Trim()
                    )

                let actual =
                    runner.Decode.fromString (Decode.at [ "name" ] Decode.int) json

                runner.equal expected actual

            runner.testCase "index works" <| fun _ ->
                let json = """["maxime", "alfonso", "steffen"]"""
                let expected = Ok("alfonso")

                let actual =
                    runner.Decode.fromString (Decode.index 1 Decode.string) json

                runner.equal expected actual

            runner.testCase "index output an error if array is to small" <| fun _ ->
                let json = """["maxime", "alfonso", "steffen"]"""
                let expected =
                    Error(
                        """
Error at: `$.[5]`
Expecting a longer array. Need index `5` but there are only `3` entries.
[
    "maxime",
    "alfonso",
    "steffen"
]
                        """.Trim())

                let actual =
                    runner.Decode.fromString (Decode.index 5 Decode.string) json

                runner.equal expected actual

            runner.testCase "index output an error if value isn't an array" <| fun _ ->
                let json = "1"
                let expected =
                    Error(
                        """
Error at: `$.[5]`
Expecting an array but instead got: 1
                        """.Trim())

                let actual =
                    runner.Decode.fromString (Decode.index 5 Decode.string) json

                runner.equal expected actual

        ]


        runner.testList "Data structure" [

            runner.testCase "list works" <| fun _ ->
                let expected = Ok([1; 2; 3])

                let actual =
                    runner.Decode.fromString (Decode.list Decode.int) "[1, 2, 3]"

                runner.equal expected actual

            runner.testCase "nested lists work" <| fun _ ->
                [ [ "maxime2" ] ]
                |> List.map (fun d ->
                    d
                    |> List.map Encode.string
                    |> Encode.list)
                |> Encode.list
                |> runner.Encode.toString 4
                |> runner.Decode.fromString (Decode.list (Decode.list Decode.string))
                |> function Ok v -> runner.equal [["maxime2"]] v | Error er -> failwith er

            runner.testCase "an invalid list output an error" <| fun _ ->
                let expected = Error("Error at: `$`\nExpecting a list but instead got: 1")

                let actual =
                    runner.Decode.fromString (Decode.list Decode.int) "1"

                runner.equal expected actual

            runner.testCase "array works" <| fun _ ->
                // Need to pass by a list otherwise Fable use:
                // new Int32Array([1, 2, 3]) and the test fails
                // And this would give:
                // Expected: Result { tag: 0, data: Int32Array [ 1, 2, 3 ] }
                // Actual: Result { tag: 0, data: [ 1, 2, 3 ] }
                let expected = Ok([1; 2; 3] |> List.toArray)

                let actual =
                    runner.Decode.fromString (Decode.array Decode.int) "[1, 2, 3]"

                runner.equal expected actual

            runner.testCase "an invalid array output an error" <| fun _ ->
                let expected = Error("Error at: `$`\nExpecting an array but instead got: 1")

                let actual =
                    runner.Decode.fromString (Decode.array Decode.int) "1"

                runner.equal expected actual

            runner.testCase "keys works" <| fun _ ->
                let expected = Ok(["a"; "b"; "c"])

                let actual =
                    runner.Decode.fromString Decode.keys """{ "a": 1, "b": 2, "c": 3 }"""

                runner.equal expected actual

            runner.testCase "keys returns an error for invalid objects" <| fun _ ->
                let expected = Error("Error at: `$`\nExpecting an object but instead got: 1")

                let actual =
                    runner.Decode.fromString Decode.keys "1"

                runner.equal expected actual

            runner.testCase "keyValuePairs works" <| fun _ ->
                let expected = Ok([("a", 1) ; ("b", 2) ; ("c", 3)])

                let actual =
                    runner.Decode.fromString (Decode.keyValuePairs Decode.int) """{ "a": 1, "b": 2, "c": 3 }"""

                runner.equal expected actual

            runner.testCase "dict works" <| fun _ ->
                let expected = Ok(Map.ofList([("a", 1) ; ("b", 2) ; ("c", 3)]))

                let actual =
                    runner.Decode.fromString (Decode.dict Decode.int) """{ "a": 1, "b": 2, "c": 3 }"""

                runner.equal expected actual

            runner.testCase "dict with custom decoder works" <| fun _ ->
                let expected = Ok(Map.ofList([("a", Record2.Create 1. 1.) ; ("b", Record2.Create 2. 2.) ; ("c", Record2.Create 3. 3.)]))

                let decodePoint =
                    Decode.map2 Record2.Create
                        (Decode.field "a" Decode.float)
                        (Decode.field "b" Decode.float)

                let actual =
                    runner.Decode.fromString (Decode.dict decodePoint)
                        """
{
    "a":
        {
            "a": 1.0,
            "b": 1.0
        },
    "b":
        {
            "a": 2.0,
            "b": 2.0
        },
    "c":
        {
            "a": 3.0,
            "b": 3.0
        }
}
                        """

                runner.equal expected actual

            runner.testCase "an invalid dict output an error" <| fun _ ->
                let expected = Error("Error at: `$`\nExpecting an object but instead got: 1")

                let actual =
                    runner.Decode.fromString (Decode.dict Decode.int) "1"

                runner.equal expected actual

            runner.testCase "map' works" <| fun _ ->
                let expected = Ok(Map.ofList([(1, "x") ; (2, "y") ; (3, "z")]))

                let actual =
                    runner.Decode.fromString (Decode.map' Decode.int Decode.string) """[ [ 1, "x" ], [ 2, "y" ], [ 3, "z" ] ]"""

                runner.equal expected actual

            runner.testCase "map' with custom key decoder works" <| fun _ ->
                let expected = Ok(Map.ofList([ ((1, 6), "a") ; ((2, 7), "b") ; ((3, 8), "c") ]))

                let decodePoint =
                    Decode.map2
                        (fun x y -> x, y)
                        (Decode.field "x" Decode.int)
                        (Decode.field "y" Decode.int)

                let actual =
                    runner.Decode.fromString (Decode.map' decodePoint Decode.string)
                        """
[
    [
        {
            "x": 1,
            "y": 6
        },
        "a"
    ],
    [
        {
            "x": 2,
            "y": 7
        },
        "b"
    ],
    [
        {
            "x": 3,
            "y": 8
        },
        "c"
    ]
]
                        """

                runner.equal expected actual
        ]

        runner.testList "Inconsistent structure" [

            runner.testCase "oneOf works" <| fun _ ->
                let expected = Ok([1; 2; 0; 4])

                let badInt =
                    Decode.oneOf [ Decode.int; Decode.nil 0 ]

                let actual =
                    runner.Decode.fromString (Decode.list badInt) "[1,2,null,4]"

                runner.equal expected actual

            runner.testCase "oneOf works in combination with object builders" <| fun _ ->
                let json = """{ "Bar": { "name": "maxime", "age": 25 } }"""
                let expected = Ok(Choice2Of2 { fieldA = "maxime" })

                let decoder1 =
                    Decode.object (fun get ->
                        { fieldA = get.Required.Field "name" Decode.string })

                let decoder2 =
                    Decode.oneOf [
                        Decode.field "Foo" decoder1 |> Decode.map Choice1Of2
                        Decode.field "Bar" decoder1 |> Decode.map Choice2Of2
                    ]

                let actual =
                    runner.Decode.fromString decoder2 json

                runner.equal expected actual

            runner.testCase "oneOf works with optional" <| fun _ ->
                let decoder =
                    Decode.oneOf
                        [
                            Decode.field "Normal" Decode.float |> Decode.map Normal
                            Decode.field "Reduced" (Decode.option Decode.float) |> Decode.map Reduced
                            Decode.field "Zero" Decode.bool |> Decode.map (fun _ -> Zero)
                        ]

                """{"Normal": 4.5}""" |> runner.Decode.fromString decoder |> runner.equal (Ok(Normal 4.5))
                """{"Reduced": 4.5}""" |> runner.Decode.fromString decoder |> runner.equal (Ok(Reduced(Some 4.5)))
                """{"Reduced": null}""" |> runner.Decode.fromString decoder |> runner.equal (Ok(Reduced None))
                """{"Zero": true}""" |> runner.Decode.fromString decoder |> runner.equal (Ok Zero)

            runner.testCase "oneOf output errors if all case fails" <| fun _ ->
                let expected =
                    Error (
                        """
The following errors were found:

Error at: `$.[0]`
Expecting a string but instead got: 1

Error at: `$.[0]`
Expecting an object but instead got:
1
                        """.Trim())

                let badInt =
                    Decode.oneOf [ Decode.string; Decode.field "test" Decode.string ]

                let actual =
                    runner.Decode.fromString (Decode.list badInt) "[1,2,null,4]"

                runner.equal expected actual

            runner.testCase "optional works" <| fun _ ->
                let json = """{ "name": "maxime", "age": 25, "something_undefined": null }"""

                let expectedValid = Ok(Some "maxime")
                let actualValid =
                    runner.Decode.fromString (Decode.optional "name" Decode.string) json

                runner.equal expectedValid actualValid

                match runner.Decode.fromString (Decode.optional "name" Decode.int) json with
                | Error _ -> ()
                | Ok _ -> failwith "Expected type error for `name` field"

                let expectedMissingField = Ok(None)
                let actualMissingField =
                    runner.Decode.fromString (Decode.optional "height" Decode.int) json

                runner.equal expectedMissingField actualMissingField

                let expectedUndefinedField = Ok(None)
                let actualUndefinedField =
                    runner.Decode.fromString (Decode.optional "something_undefined" Decode.string) json

                runner.equal expectedUndefinedField actualUndefinedField

            runner.testCase "optional returns Error value if decoder fails" <| fun _ ->
                let json = """{ "name": 12, "age": 25 }"""
                let expected =
                    Error(
                        """
Error at: `$.name`
Expecting a string but instead got: 12
                        """.Trim())

                let actual =
                    runner.Decode.fromString (Decode.optional "name" Decode.string) json

                runner.equal expected actual

            runner.testCase "optionalAt works" <| fun _ ->
                let json = """{ "data" : { "name": "maxime", "age": 25, "something_undefined": null } }"""

                let expectedValid = Ok(Some "maxime")
                let actualValid =
                    runner.Decode.fromString (Decode.optionalAt [ "data"; "name" ] Decode.string) json

                runner.equal expectedValid actualValid

                match runner.Decode.fromString (Decode.optionalAt [ "data"; "name" ] Decode.int) json with
                | Error _ -> ()
                | Ok _ -> failwith "Expected type error for `name` field"

                let expectedMissingField = Ok None
                let actualMissingField =
                    runner.Decode.fromString (Decode.optionalAt [ "data"; "height" ] Decode.int) json

                runner.equal expectedMissingField actualMissingField

                let expectedUndefinedField = Ok(None)
                let actualUndefinedField =
                    runner.Decode.fromString (Decode.optionalAt [ "data"; "something_undefined" ] Decode.string) json

                runner.equal expectedUndefinedField actualUndefinedField

                let expectedUndefinedField = Ok(None)
                let actualUndefinedField =
                    runner.Decode.fromString (Decode.optionalAt [ "data"; "something_undefined"; "name" ] Decode.string) json

                runner.equal expectedUndefinedField actualUndefinedField

            runner.testCase "combining field and option decoders works" <| fun _ ->
                let json = """{ "name": "maxime", "age": 25, "something_undefined": null }"""

                let expectedValid = Ok(Some "maxime")
                let actualValid =
                    runner.Decode.fromString (Decode.field "name" (Decode.option Decode.string)) json

                runner.equal expectedValid actualValid

                match runner.Decode.fromString (Decode.field "name" (Decode.option Decode.int)) json with
                | Error msg ->
                    let expected =
                        """
Error at: `$.name`
Expecting an int but instead got: "maxime"
                        """.Trim()
                    runner.equal expected msg
                | Ok _ -> failwith "Expected type error for `name` field #1"

                match runner.Decode.fromString (Decode.field "this_field_do_not_exist" (Decode.option Decode.int)) json with
                | Error msg ->
                    let expected =
                        """
Error at: `$`
Expecting an object with a field named `this_field_do_not_exist` but instead got:
{
    "name": "maxime",
    "age": 25,
    "something_undefined": null
}
                        """.Trim()
                    runner.equal expected msg
                | Ok _ ->
                    failwith "Expected type error for `name` field #2"

                match runner.Decode.fromString (Decode.field "something_undefined" (Decode.option Decode.int)) json with
                | Error _ -> failwith """`Decode.field "something_undefined" (Decode.option Decode.int)` test should pass"""
                | Ok result -> runner.equal None result

                // Same tests as before but we are calling `option` then `field`

                let expectedValid2 = Ok(Some "maxime")
                let actualValid2 =
                    runner.Decode.fromString (Decode.option (Decode.field "name" Decode.string)) json

                runner.equal expectedValid2 actualValid2

                match runner.Decode.fromString (Decode.option (Decode.field "name" Decode.int)) json with
                | Error msg ->
                    let expected =
                        """
Error at: `$.name`
Expecting an int but instead got: "maxime"
                        """.Trim()
                    runner.equal expected msg
                | Ok _ -> failwith "Expected type error for `name` field #3"

                match runner.Decode.fromString (Decode.option (Decode.field "this_field_do_not_exist" Decode.int)) json with
                | Error msg ->
                    let expected =
                        """
Error at: `$`
Expecting an object with a field named `this_field_do_not_exist` but instead got:
{
    "name": "maxime",
    "age": 25,
    "something_undefined": null
}
                        """.Trim()
                    runner.equal expected msg
                | Ok _ -> failwith "Expected type error for `name` field #4"

                match runner.Decode.fromString (Decode.option (Decode.field "something_undefined" Decode.int)) json with
                | Error msg ->
                    let expected =
                        """
Error at: `$.something_undefined`
Expecting an int but instead got: null
                        """.Trim()
                    runner.equal expected msg
                | Ok _ -> failwith "Expected type error for `name` field"

                // Alfonso: Should this test pass? We should use Decode.optional instead
                // - `runner.Decode.fromString (Decode.field "height" (Decode.option Decode.int)) json` == `Ok(None)`
                //
                // Maxime here :)
                // I don't think this test should pass.
                // For me `Decode.field "height" (Decode.option Decode.int)` means:
                // 1. The field `height` is required
                // 2. If `height` exist then, it's value can be `Some X` where `X` is an `int` or `None`
                //
                // I am keep the comments here so we keep track of the explanation if we later need to give it a second though.
                //
                match runner.Decode.fromString (Decode.field "height" (Decode.option Decode.int)) json with
                | Error msg ->
                    let expected =
                        """
Error at: `$`
Expecting an object with a field named `height` but instead got:
{
    "name": "maxime",
    "age": 25,
    "something_undefined": null
}
                        """.Trim()

                    runner.equal expected msg

                | Ok _ -> failwith "Expected type error for `height` field"

                let expectedUndefinedField = Ok(None)
                let actualUndefinedField =
                    runner.Decode.fromString (Decode.field "something_undefined" (Decode.option Decode.string)) json

                runner.equal expectedUndefinedField actualUndefinedField

        ]

        runner.testList "Fancy decoding" [

            runner.testCase "null works (test on an int)" <| fun _ ->
                let expected = Ok(20)
                let actual =
                    runner.Decode.fromString (Decode.nil 20) "null"

                runner.equal expected actual

            runner.testCase "null works (test on a boolean)" <| fun _ ->
                let expected = Ok(false)
                let actual =
                    runner.Decode.fromString (Decode.nil false) "null"

                runner.equal expected actual

            runner.testCase "succeed works" <| fun _ ->
                let expected = Ok(7)
                let actual =
                    runner.Decode.fromString (Decode.succeed 7) "true"

                runner.equal expected actual

            runner.testCase "succeed output an error if the JSON is invalid" <| fun _ ->
                #if FABLE_COMPILER_JAVASCRIPT
                let expected = Error("Given an invalid JSON: Unexpected token 'm', \"maxime\" is not valid JSON")
                #endif

                #if FABLE_COMPILER_PYTHON
                let expected = Error("Given an invalid JSON: Expecting value: line 1 column 1 (char 0)")
                #endif

                #if !FABLE_COMPILER
                let expected = Error("Given an invalid JSON: Unexpected character encountered while parsing value: m. Path '', line 0, position 0.")
                #endif

                let actual =
                    runner.Decode.fromString (Decode.succeed 7) "maxime"

                runner.equal expected actual

            runner.testCase "fail works" <| fun _ ->
                let msg = "Failing because it's fun"
                let expected = Error("Error at: `$`\nThe following `failure` occurred with the decoder: " + msg)
                let actual =
                    runner.Decode.fromString (Decode.fail msg) "true"

                runner.equal expected actual

            runner.testCase "andMap works for any arity" <| fun _ ->
                // In the past maximum arity in Fable was 8
                let json =
                    """{"a": 1,"b": 2,"c": 3,"d": 4,"e": 5,"f": 6,"g": 7,"h": 8,"i": 9,"j": 10,"k": 11}"""

                let decodeRecord10 =
                    Decode.succeed Record10.Create
                        |> Decode.andMap (Decode.field "a" Decode.int)
                        |> Decode.andMap (Decode.field "b" Decode.int)
                        |> Decode.andMap (Decode.field "c" Decode.int)
                        |> Decode.andMap (Decode.field "d" Decode.int)
                        |> Decode.andMap (Decode.field "e" Decode.int)
                        |> Decode.andMap (Decode.field "f" Decode.int)
                        |> Decode.andMap (Decode.field "g" Decode.int)
                        |> Decode.andMap (Decode.field "h" Decode.int)
                        |> Decode.andMap (Decode.field "i" Decode.int)
                        |> Decode.andMap (Decode.field "j" Decode.int)
                        |> Decode.andMap (Decode.field "k" Decode.int)

                let actual =
                    runner.Decode.fromString decodeRecord10 json

                let expected = Ok { a = 1; b = 2; c = 3; d = 4; e = 5; f = 6; g = 7; h = 8; i = 9; j = 10; k = 11 }

                runner.equal expected actual

            runner.testCase "andThen works" <| fun _ ->
                let expected = Ok 1
                let infoHelp version =
                    match version with
                    | 4 ->
                        Decode.succeed 1
                    | 3 ->
                        Decode.succeed 1
                    | _ ->
                        Decode.fail <| "Trying to decode info, but version " + (version.ToString()) + "is not supported"

                let info : Decoder<int> =
                    Decode.field "version" Decode.int
                    |> Decode.andThen infoHelp

                let actual =
                    runner.Decode.fromString info """{ "version": 3, "data": 2 }"""

                runner.equal expected actual


            runner.testCase "andThen generate an error if an error occuered" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting an object with a field named `version` but instead got:
{
    "info": 3,
    "data": 2
}
                        """.Trim())
                let infoHelp version : Decoder<int> =
                    match version with
                    | 4 ->
                        Decode.succeed 1
                    | 3 ->
                        Decode.succeed 1
                    | _ ->
                        Decode.fail <| "Trying to decode info, but version " + (version.ToString()) + "is not supported"

                let info =
                    Decode.field "version" Decode.int
                    |> Decode.andThen infoHelp

                let actual =
                    runner.Decode.fromString info """{ "info": 3, "data": 2 }"""

                runner.equal expected actual


            runner.testCase "all works" <| fun _ ->
                let expected = Ok [1; 2; 3]

                let decodeAll = Decode.all [
                    Decode.succeed 1
                    Decode.succeed 2
                    Decode.succeed 3
                ]

                let actual = runner.Decode.fromString decodeAll "{}"

                runner.equal expected actual

            runner.testCase "combining Decode.all and Decode.keys works" <| fun _ ->
                let expected = Ok [1; 2; 3]

                let decoder =
                    Decode.keys
                    |> Decode.andThen (fun keys ->
                        keys
                        |> List.except ["special_property"]
                        |> List.map (fun key -> Decode.field key Decode.int)
                        |> Decode.all)

                let actual = runner.Decode.fromString decoder """{ "a": 1, "b": 2, "c": 3 }"""

                runner.equal expected actual

            runner.testCase "all succeeds on empty lists" <| fun _ ->
                let expected = Ok []

                let decodeNone = Decode.all []

                let actual = runner.Decode.fromString decodeNone "{}"

                runner.equal expected actual


            runner.testCase "all fails when one decoder fails" <| fun _ ->
                let expected = Error("Error at: `$`\nExpecting an int but instead got: {}")

                let decodeAll = Decode.all [
                    Decode.succeed 1
                    Decode.int
                    Decode.succeed 3
                ]

                let actual = runner.Decode.fromString decodeAll "{}"

                runner.equal expected actual
        ]

        runner.testList "Mapping" [

            runner.testCase "map works" <| fun _ ->
                let expected = Ok(6)
                let stringLength =
                    Decode.map String.length Decode.string

                let actual =
                    runner.Decode.fromString stringLength "\"maxime\""
                runner.equal expected actual


            runner.testCase "map2 works" <| fun _ ->
                let expected = Ok({a = 1.; b = 2.} : Record2)

                let decodePoint =
                    Decode.map2 Record2.Create
                        (Decode.field "a" Decode.float)
                        (Decode.field "b" Decode.float)

                let actual =
                    runner.Decode.fromString decodePoint jsonRecord

                runner.equal expected actual

            runner.testCase "map3 works" <| fun _ ->
                let expected = Ok({ a = 1.
                                    b = 2.
                                    c = 3. } : Record3)

                let decodePoint =
                    Decode.map3 Record3.Create
                        (Decode.field "a" Decode.float)
                        (Decode.field "b" Decode.float)
                        (Decode.field "c" Decode.float)

                let actual =
                    runner.Decode.fromString decodePoint jsonRecord

                runner.equal expected actual

            runner.testCase "map4 works" <| fun _ ->
                let expected = Ok({ a = 1.
                                    b = 2.
                                    c = 3.
                                    d = 4. } : Record4)

                let decodePoint =
                    Decode.map4 Record4.Create
                        (Decode.field "a" Decode.float)
                        (Decode.field "b" Decode.float)
                        (Decode.field "c" Decode.float)
                        (Decode.field "d" Decode.float)

                let actual =
                    runner.Decode.fromString decodePoint jsonRecord

                runner.equal expected actual

            runner.testCase "map5 works" <| fun _ ->
                let expected = Ok({ a = 1.
                                    b = 2.
                                    c = 3.
                                    d = 4.
                                    e = 5. } : Record5)

                let decodePoint =
                    Decode.map5 Record5.Create
                        (Decode.field "a" Decode.float)
                        (Decode.field "b" Decode.float)
                        (Decode.field "c" Decode.float)
                        (Decode.field "d" Decode.float)
                        (Decode.field "e" Decode.float)

                let actual =
                    runner.Decode.fromString decodePoint jsonRecord

                runner.equal expected actual

            runner.testCase "map6 works" <| fun _ ->
                let expected = Ok({ a = 1.
                                    b = 2.
                                    c = 3.
                                    d = 4.
                                    e = 5.
                                    f = 6. } : Record6)

                let decodePoint =
                    Decode.map6 Record6.Create
                        (Decode.field "a" Decode.float)
                        (Decode.field "b" Decode.float)
                        (Decode.field "c" Decode.float)
                        (Decode.field "d" Decode.float)
                        (Decode.field "e" Decode.float)
                        (Decode.field "f" Decode.float)

                let actual =
                    runner.Decode.fromString decodePoint jsonRecord

                runner.equal expected actual

            runner.testCase "map7 works" <| fun _ ->
                let expected = Ok({ a = 1.
                                    b = 2.
                                    c = 3.
                                    d = 4.
                                    e = 5.
                                    f = 6.
                                    g = 7. } : Record7)

                let decodePoint =
                    Decode.map7 Record7.Create
                        (Decode.field "a" Decode.float)
                        (Decode.field "b" Decode.float)
                        (Decode.field "c" Decode.float)
                        (Decode.field "d" Decode.float)
                        (Decode.field "e" Decode.float)
                        (Decode.field "f" Decode.float)
                        (Decode.field "g" Decode.float)

                let actual =
                    runner.Decode.fromString decodePoint jsonRecord

                runner.equal expected actual

            runner.testCase "map8 works" <| fun _ ->
                let expected = Ok({ a = 1.
                                    b = 2.
                                    c = 3.
                                    d = 4.
                                    e = 5.
                                    f = 6.
                                    g = 7.
                                    h = 8. } : Record8)

                let decodePoint =
                    Decode.map8 Record8.Create
                        (Decode.field "a" Decode.float)
                        (Decode.field "b" Decode.float)
                        (Decode.field "c" Decode.float)
                        (Decode.field "d" Decode.float)
                        (Decode.field "e" Decode.float)
                        (Decode.field "f" Decode.float)
                        (Decode.field "g" Decode.float)
                        (Decode.field "h" Decode.float)

                let actual =
                    runner.Decode.fromString decodePoint jsonRecord

                runner.equal expected actual

            runner.testCase "map2 generate an error if invalid" <| fun _ ->
                let expected = Error("Error at: `$.a`\nExpecting a float but instead got: \"invalid_a_field\"")

                let decodePoint =
                    Decode.map2 Record2.Create
                        (Decode.field "a" Decode.float)
                        (Decode.field "b" Decode.float)

                let actual =
                    runner.Decode.fromString decodePoint jsonRecordInvalid

                runner.equal expected actual

        ]

        runner.testList "object builder" [

            runner.testCase "get.Required.Field works" <| fun _ ->
                let json = """{ "name": "maxime", "age": 25 }"""
                let expected = Ok({ fieldA = "maxime" })

                let decoder =
                    Decode.object
                        (fun get ->
                            { fieldA = get.Required.Field "name" Decode.string }
                        )

                let actual =
                    runner.Decode.fromString decoder json

                runner.equal expected actual

            runner.testCase "get.Required.Field returns Error if field is missing" <| fun _ ->
                let json = """{ "age": 25 }"""
                let expected =
                    Error(
                        """
Error at: `$`
Expecting an object with a field named `name` but instead got:
{
    "age": 25
}
                        """.Trim())

                let decoder =
                    Decode.object
                        (fun get ->
                            { fieldA = get.Required.Field "name" Decode.string }
                        )

                let actual =
                    runner.Decode.fromString decoder json

                runner.equal expected actual

            runner.testCase "get.Required.Field returns Error if type is incorrect" <| fun _ ->
                let json = """{ "name": 12, "age": 25 }"""
                let expected =
                    Error(
                        """
Error at: `$.name`
Expecting a string but instead got: 12
                        """.Trim())

                let decoder =
                    Decode.object
                        (fun get ->
                            { fieldA = get.Required.Field "name" Decode.string }
                        )

                let actual =
                    runner.Decode.fromString decoder json

                runner.equal expected actual

            runner.testCase "get.Optional.Field works" <| fun _ ->
                let json = """{ "name": "maxime", "age": 25 }"""
                let expected = Ok({ optionalField = Some "maxime" })

                let decoder =
                    Decode.object
                        (fun get ->
                            { optionalField = get.Optional.Field "name" Decode.string }
                        )

                let actual =
                    runner.Decode.fromString decoder json

                runner.equal expected actual

            runner.testCase "get.Optional.Field returns None value if field is missing" <| fun _ ->
                let json = """{ "age": 25 }"""
                let expected = Ok({ optionalField = None })

                let decoder =
                    Decode.object
                        (fun get ->
                            { optionalField = get.Optional.Field "name" Decode.string }
                        )

                let actual =
                    runner.Decode.fromString decoder json

                runner.equal expected actual

            runner.testCase "get.Optional.Field returns None if field is null" <| fun _ ->
                let json = """{ "name": null, "age": 25 }"""
                let expected = Ok({ optionalField = None })

                let decoder =
                    Decode.object
                        (fun get ->
                            { optionalField = get.Optional.Field "name" Decode.string }
                        )

                let actual =
                    runner.Decode.fromString decoder json

                runner.equal expected actual

            runner.testCase "get.Optional.Field returns Error value if decoder fails" <| fun _ ->
                let json = """{ "name": 12, "age": 25 }"""
                let expected =
                    Error(
                        """
Error at: `$.name`
Expecting a string but instead got: 12
                        """.Trim())

                let decoder = Decode.object (fun get ->
                    { optionalField = get.Optional.Field "name" Decode.string })

                let actual = runner.Decode.fromString decoder json

                runner.equal expected actual

            runner.testCase "nested get.Optional.Field > get.Required.Field returns None if field is null" <| fun _ ->
                let json = """{ "user": null, "field2": 25 }"""
                let expected = Ok({ User = None; Field2 = 25 })

                let userDecoder =
                    Decode.object
                        (fun get ->
                            { Id = get.Required.Field "id" Decode.int
                              Name = get.Required.Field "name" Decode.string
                              Email = get.Required.Field "email" Decode.string
                              Followers = 0 }
                        )

                let decoder =
                    Decode.object
                        (fun get ->
                            { User = get.Optional.Field "user" userDecoder
                              Field2 = get.Required.Field "field2" Decode.int }
                        )

                let actual =
                    runner.Decode.fromString decoder json

                runner.equal expected actual

            runner.testCase "get.Optional.Field returns Error if type is incorrect" <| fun _ ->
                let json = """{ "name": 12, "age": 25 }"""
                let expected =
                    Error(
                        """
Error at: `$.name`
Expecting a string but instead got: 12
                        """.Trim())

                let decoder =
                    Decode.object
                        (fun get ->
                            { optionalField = get.Optional.Field "name" Decode.string }
                        )

                let actual =
                    runner.Decode.fromString decoder json

                runner.equal expected actual


            runner.testCase "get.Required.At works" <| fun _ ->

                let json = """{ "user": { "name": "maxime", "age": 25 } }"""
                let expected = Ok({ fieldA = "maxime" })

                let decoder =
                    Decode.object
                        (fun get ->
                            { fieldA = get.Required.At [ "user"; "name" ] Decode.string }
                        )

                let actual =
                    runner.Decode.fromString decoder json

                runner.equal expected actual

            runner.testCase "get.Required.At returns Error if non-object in path" <| fun _ ->
                let json = """{ "user": "maxime" }"""
                let expected =
                    Error(
                        """
Error at: `$.user`
Expecting an object but instead got:
"maxime"
                        """.Trim())

                let decoder =
                    Decode.object
                        (fun get ->
                            { fieldA = get.Required.At [ "user"; "name" ] Decode.string }
                        )

                let actual =
                    runner.Decode.fromString decoder json

                runner.equal expected actual

            runner.testCase "get.Required.At returns Error if field missing" <| fun _ ->
                let json = """{ "user": { "name": "maxime", "age": 25 } }"""
                let expected =
                    Error(
                        """
Error at: `$.user.firstname`
Expecting an object with path `user.firstname` but instead got:
{
    "user": {
        "name": "maxime",
        "age": 25
    }
}
Node `firstname` is unkown.
                        """.Trim())

                let decoder =
                    Decode.object
                        (fun get ->
                            { fieldA = get.Required.At [ "user"; "firstname" ] Decode.string }
                        )

                let actual =
                    runner.Decode.fromString decoder json

                runner.equal expected actual

            runner.testCase "get.Required.At returns Error if type is incorrect" <| fun _ ->
                let json = """{ "user": { "name": 12, "age": 25 } }"""
                let expected =
                    Error(
                        """
Error at: `$.user.name`
Expecting a string but instead got: 12
                        """.Trim())

                let decoder =
                    Decode.object
                        (fun get ->
                            { fieldA = get.Required.At [ "user"; "name" ] Decode.string }
                        )

                let actual =
                    runner.Decode.fromString decoder json

                runner.equal expected actual

            runner.testCase "get.Optional.At works" <| fun _ ->

                let json = """{ "user": { "name": "maxime", "age": 25 } }"""
                let expected = Ok({ optionalField = Some "maxime" })

                let decoder =
                    Decode.object
                        (fun get ->
                            { optionalField = get.Optional.At [ "user"; "name" ] Decode.string }
                        )

                let actual =
                    runner.Decode.fromString decoder json

                runner.equal expected actual

            runner.testCase "get.Optional.At returns 'type error' if non-object in path" <| fun _ ->
                let json = """{ "user": "maxime" }"""
                let expected =
                    Error(
                        """
Error at: `$.user`
Expecting an object but instead got:
"maxime"
                        """.Trim()
                    )

                let decoder =
                    Decode.object
                        (fun get ->
                            { optionalField = get.Optional.At [ "user"; "name" ] Decode.string }
                        )

                let actual =
                    runner.Decode.fromString decoder json

                runner.equal expected actual

            runner.testCase "get.Optional.At returns None if field missing" <| fun _ ->
                let json = """{ "user": { "name": "maxime", "age": 25 } }"""
                let expected = Ok({ optionalField = None })

                let decoder =
                    Decode.object
                        (fun get ->
                            { optionalField = get.Optional.At [ "user"; "firstname" ] Decode.string }
                        )

                let actual =
                    runner.Decode.fromString decoder json

                runner.equal expected actual

            runner.testCase "get.Optional.At returns Error if type is incorrect" <| fun _ ->
                let json = """{ "user": { "name": 12, "age": 25 } }"""
                let expected =
                    Error(
                        """
Error at: `$.user.name`
Expecting a string but instead got: 12
                        """.Trim())

                let decoder =
                    Decode.object
                        (fun get ->
                            { optionalField = get.Optional.At [ "user"; "name" ] Decode.string }
                        )

                let actual =
                    runner.Decode.fromString decoder json

                runner.equal expected actual

            runner.testCase "complex object builder works" <| fun _ ->
                let expected =
                    Ok(User.Create 67 "" "user@mail.com" 0)

                let userDecoder =
                    Decode.object
                        (fun get ->
                            { Id = get.Required.Field "id" Decode.int
                              Name = get.Optional.Field "name" Decode.string
                                        |> Option.defaultValue ""
                              Email = get.Required.Field "email" Decode.string
                              Followers = 0 }
                        )

                let actual =
                    runner.Decode.fromString
                        userDecoder
                        """{ "id": 67, "email": "user@mail.com" }"""

                runner.equal expected actual

            runner.testCase "get.Field.Raw works" <| fun _ ->
                let json = """{
    "enabled": true,
	"shape": "circle",
    "radius": 20
}"""
                let shapeDecoder =
                    Decode.field "shape" Decode.string
                    |> Decode.andThen (function
                        | "circle" ->
                            Shape.DecoderCircle
                        | "rectangle" ->
                            Shape.DecoderRectangle
                        | shape ->
                            Decode.fail (sprintf "Unknown shape type %s" shape))

                let decoder =
                    Decode.object (fun get ->
                        { Enabled = get.Required.Field "enabled" Decode.bool
                          Shape = get.Required.Raw shapeDecoder } : MyObj
                    )

                let actual =
                    runner.Decode.fromString
                        decoder
                        json

                let expected =
                    Ok ({ Enabled = true
                          Shape = Circle 20 } : MyObj)

                runner.equal expected actual

            runner.testCase "get.Field.Raw returns Error if a decoder fail" <| fun _ ->
                let json = """{
    "enabled": true,
	"shape": "custom_shape",
    "radius": 20
}"""
                let shapeDecoder =
                    Decode.field "shape" Decode.string
                    |> Decode.andThen (function
                        | "circle" ->
                            Shape.DecoderCircle
                        | "rectangle" ->
                            Shape.DecoderRectangle
                        | shape ->
                            Decode.fail (sprintf "Unknown shape type %s" shape))

                let decoder =
                    Decode.object (fun get ->
                        { Enabled = get.Required.Field "enabled" Decode.bool
                          Shape = get.Required.Raw shapeDecoder } : MyObj
                    )

                let actual =
                    runner.Decode.fromString
                        decoder
                        json

                let expected =
                    Error "Error at: `$`\nThe following `failure` occurred with the decoder: Unknown shape type custom_shape"

                runner.equal expected actual

            runner.testCase "get.Field.Raw returns Error if a field is missing in the 'raw decoder'" <| fun _ ->
                let json = """{
    "enabled": true,
	"shape": "circle"
}"""
                let shapeDecoder =
                    Decode.field "shape" Decode.string
                    |> Decode.andThen (function
                        | "circle" ->
                            Shape.DecoderCircle
                        | "rectangle" ->
                            Shape.DecoderRectangle
                        | shape ->
                            Decode.fail (sprintf "Unknown shape type %s" shape))

                let decoder =
                    Decode.object (fun get ->
                        { Enabled = get.Required.Field "enabled" Decode.bool
                          Shape = get.Required.Raw shapeDecoder } : MyObj
                    )

                let actual =
                    runner.Decode.fromString
                        decoder
                        json

                let expected =
                    Error (
                        """
Error at: `$`
Expecting an object with a field named `radius` but instead got:
{
    "enabled": true,
    "shape": "circle"
}                   """.Trim())

                runner.equal expected actual

            runner.testCase "get.Optional.Raw works" <| fun _ ->
                let json = """{
    "enabled": true,
	"shape": "circle",
    "radius": 20
}"""
                let shapeDecoder =
                    Decode.field "shape" Decode.string
                    |> Decode.andThen (function
                        | "circle" ->
                            Shape.DecoderCircle
                        | "rectangle" ->
                            Shape.DecoderRectangle
                        | shape ->
                            Decode.fail (sprintf "Unknown shape type %s" shape))

                let decoder =
                    Decode.object (fun get ->
                        { Enabled = get.Required.Field "enabled" Decode.bool
                          Shape = get.Optional.Raw shapeDecoder }
                    )

                let actual =
                    runner.Decode.fromString
                        decoder
                        json

                let expected =
                    Ok { Enabled = true
                         Shape = Some (Circle 20) }

                runner.equal expected actual

            runner.testCase "get.Optional.Raw returns None if a field is missing" <| fun _ ->
                let json = """{
    "enabled": true,
	"shape": "circle"
}"""
                let shapeDecoder =
                    Decode.field "shape" Decode.string
                    |> Decode.andThen (function
                        | "circle" ->
                            Shape.DecoderCircle
                        | "rectangle" ->
                            Shape.DecoderRectangle
                        | shape ->
                            Decode.fail (sprintf "Unknown shape type %s" shape))

                let decoder =
                    Decode.object (fun get ->
                        { Enabled = get.Required.Field "enabled" Decode.bool
                          Shape = get.Optional.Raw shapeDecoder }
                    )

                let actual =
                    runner.Decode.fromString
                        decoder
                        json

                let expected =
                    Ok { Enabled = true
                         Shape = None }

                runner.equal expected actual

            runner.testCase "get.Optional.Raw returns an Error if a decoder fail" <| fun _ ->
                let json = """{
    "enabled": true,
	"shape": "invalid_shape"
}"""
                let shapeDecoder =
                    Decode.field "shape" Decode.string
                    |> Decode.andThen (function
                        | "circle" ->
                            Shape.DecoderCircle
                        | "rectangle" ->
                            Shape.DecoderRectangle
                        | shape ->
                            Decode.fail (sprintf "Unknown shape type %s" shape))

                let decoder =
                    Decode.object (fun get ->
                        { Enabled = get.Required.Field "enabled" Decode.bool
                          Shape = get.Optional.Raw shapeDecoder }
                    )

                let actual =
                    runner.Decode.fromString
                        decoder
                        json

                let expected =
                    Error "Error at: `$`\nThe following `failure` occurred with the decoder: Unknown shape type invalid_shape"

                runner.equal expected actual

            runner.testCase "get.Optional.Raw returns an Error if the type is invalid" <| fun _ ->
                let json = """{
    "enabled": true,
	"shape": "circle",
    "radius": "maxime"
}"""
                let shapeDecoder =
                    Decode.field "shape" Decode.string
                    |> Decode.andThen (function
                        | "circle" ->
                            Shape.DecoderCircle
                        | "rectangle" ->
                            Shape.DecoderRectangle
                        | shape ->
                            Decode.fail (sprintf "Unknown shape type %s" shape))

                let decoder =
                    Decode.object (fun get ->
                        { Enabled = get.Required.Field "enabled" Decode.bool
                          Shape = get.Optional.Raw shapeDecoder }
                    )

                let actual =
                    runner.Decode.fromString
                        decoder
                        json

                let expected =
                    Error "Error at: `$.radius`\nExpecting an int but instead got: \"maxime\""

                runner.equal expected actual

            runner.testCase "get.Optional.Raw returns None if a decoder fails with null" <| fun _ ->
                let json = """{
    "enabled": true,
	"shape": null
}"""
                let shapeDecoder =
                    Decode.field "shape" Decode.string
                    |> Decode.andThen (function
                        | "circle" ->
                            Shape.DecoderCircle
                        | "rectangle" ->
                            Shape.DecoderRectangle
                        | shape ->
                            Decode.fail (sprintf "Unknown shape type %s" shape))

                let decoder =
                    Decode.object (fun get ->
                        { Enabled = get.Required.Field "enabled" Decode.bool
                          Shape = get.Optional.Raw shapeDecoder }
                    )

                let actual =
                    runner.Decode.fromString
                        decoder
                        json

                let expected =
                     Ok { Enabled = true
                          Shape = None }

                runner.equal expected actual

            runner.testCase "Object builders returns all the Errors" <| fun _ ->
                let json = """{ "age": 25, "fieldC": "not_a_number", "fieldD": { "sub_field": "not_a_boolean" } }"""
                let expected =
                    Error(
                        """
The following errors were found:

Error at: `$`
Expecting an object with a field named `missing_field_1` but instead got:
{
    "age": 25,
    "fieldC": "not_a_number",
    "fieldD": {
        "sub_field": "not_a_boolean"
    }
}

Error at: `$.missing_field_2.sub_field`
Expecting an object with path `missing_field_2.sub_field` but instead got:
{
    "age": 25,
    "fieldC": "not_a_number",
    "fieldD": {
        "sub_field": "not_a_boolean"
    }
}
Node `sub_field` is unkown.

Error at: `$.fieldC`
Expecting an int but instead got: "not_a_number"

Error at: `$.fieldD.sub_field`
Expecting a boolean but instead got: "not_a_boolean"
                        """.Trim())

                let decoder =
                    Decode.object (fun get ->
                        { FieldA = get.Required.Field "missing_field_1" Decode.string
                          FieldB = get.Required.At [ "missing_field_2"; "sub_field" ] Decode.string
                          FieldC = get.Optional.Field "fieldC" Decode.int
                                    |> Option.defaultValue -1
                          FieldD = get.Optional.At [ "fieldD"; "sub_field" ] Decode.bool
                                    |> Option.defaultValue false }
                    )

                let actual =
                    runner.Decode.fromString decoder json

                runner.equal expected actual

            runner.testCase "Test" <| fun _ ->
                let json =
                    """
                    {
                        "person": {
                            "name": "maxime"
                        },
                        "post": null
                    }
                    """

                let personDecoder : Decoder<Person> =
                    Decode.object (fun get ->
                        {
                            Name = get.Required.Field "name" Decode.string
                        }
                    )

                let postDecoder : Decoder<Post> =
                    Decode.object (fun get ->
                        let title = get.Required.Field "title" Decode.string

                        // Accessing the value and doing something with it
                        // To reproduce bug reported in:
                        // https://github.com/thoth-org/Thoth.Json.Net/issues/53
                        title
                        |> Seq.head
                        |> printfn "Title: %A"

                        {
                            Title = title
                        }
                    )

                let dataDecoder =
                    Decode.object (fun get ->
                        {
                            Person = get.Required.Field "person" personDecoder
                            Post = get.Optional.Field "post" postDecoder
                        }
                    )

                let actual = runner.Decode.fromString dataDecoder json
                let expected = Ok { Person = { Name = "maxime" }; Post = None }

                runner.equal expected actual
        ]

//         runner.testList "Auto" [
//             runner.testCase "Auto.runner.Decode.fromString works" <| fun _ ->
//                 let now = DateTime.Now
//                 let value : Record9 =
//                     {
//                         a = 5
//                         b = "bar"
//                         c = [false, 3; true, 5; false, 10]
//                         d = [|Some(Foo 14); None|]
//                         e = Map [("oh", { a = 2.; b = 2. }); ("ah", { a = -1.5; b = 0. })]
//                         f = now
//                         g = set [{ a = 2.; b = 2. }; { a = -1.5; b = 0. }]
//                         h = TimeSpan.FromSeconds(5.)
//                         i = 120y
//                         j = 120uy
//                         k = 250s
//                         l = 250us
//                         m = 99u
//                         n = 99L
//                         o = 999UL
//                         p = ()
//                         r = Map [( {a = 1.; b = 2.}, "value 1"); ( {a = -2.5; b = 22.1}, "value 2")]
//                         s = 'y'
//                         // s = seq [ "item n°1"; "item n°2"]
//                     }
//                 let extra =
//                     Extra.empty
//                     |> Extra.withInt64
//                     |> Extra.withUInt64
//                 let json = Encode.Auto.toString(4, value, extra = extra)
//                 // printfn "AUTO ENCODED %s" json
//                 let r2 = Decode.Auto.unsafeFromString<Record9>(json, extra = extra)
//                 runner.equal 5 r2.a
//                 runner.equal "bar" r2.b
//                 runner.equal [false, 3; true, 5; false, 10] r2.c
//                 runner.equal (Some(Foo 14)) r2.d.[0]
//                 runner.equal None r2.d.[1]
//                 runner.equal -1.5 (Map.find "ah" r2.e).a
//                 runner.equal 2.   (Map.find "oh" r2.e).b
//                 runner.equal (now.ToString())  (value.f.ToString())
//                 runner.equal true (Set.contains { a = -1.5; b = 0. } r2.g)
//                 runner.equal false (Set.contains { a = 1.5; b = 0. } r2.g)
//                 runner.equal 5000. value.h.TotalMilliseconds
//                 runner.equal 120y r2.i
//                 runner.equal 120uy r2.j
//                 runner.equal 250s r2.k
//                 runner.equal 250us r2.l
//                 runner.equal 99u r2.m
//                 runner.equal 99L r2.n
//                 runner.equal 999UL r2.o
//                 runner.equal () r2.p
//                 runner.equal (Map [( {a = 1.; b = 2.}, "value 1"); ( {a = -2.5; b = 22.1}, "value 2")]) r2.r
//                 runner.equal 'y' r2.s
//                 // runner.equal ((seq [ "item n°1"; "item n°2"]) |> Seq.toList) (r2.s |> Seq.toList)

//             runner.testCase "Auto serialization works with recursive types" <| fun _ ->
//                 let len xs =
//                     let rec lenInner acc = function
//                         | Cons(_,rest) -> lenInner (acc + 1) rest
//                         | Nil -> acc
//                     lenInner 0 xs
//                 let li = Cons(1, Cons(2, Cons(3, Nil)))
//                 let json = Encode.Auto.toString(4, li)
//                 // printfn "AUTO ENCODED MYLIST %s" json
//                 let li2 = Decode.Auto.unsafeFromString<MyList<int>>(json)
//                 len li2 |> runner.equal 3
//                 match li with
//                 | Cons(i1, Cons(i2, Cons(i3, Nil))) -> i1 + i2 + i3
//                 | Cons(i,_) -> i
//                 | Nil -> 0
//                 |> runner.equal 6

//             runner.testCase "Auto decoders works for string" <| fun _ ->
//                 let value = "maxime"
//                 let json = Encode.Auto.toString(4, value)
//                 let res = Decode.Auto.unsafeFromString<string>(json)
//                 runner.equal value res

//             runner.testCase "Auto decoders works for guid" <| fun _ ->
//                 let value = Guid.NewGuid()
//                 let json = Encode.Auto.toString(4, value)
//                 let res = Decode.Auto.unsafeFromString<Guid>(json)
//                 runner.equal value res

//             runner.testCase "Auto decoders works for int" <| fun _ ->
//                 let value = 12
//                 let json = Encode.Auto.toString(4, value)
//                 let res = Decode.Auto.unsafeFromString<int>(json)
//                 runner.equal value res

//             runner.testCase "Auto decoders works for int64" <| fun _ ->
//                 let extra = Extra.empty |> Extra.withInt64
//                 let value = 9999999999L
//                 let json = Encode.Auto.toString(4, value, extra=extra)
//                 let res = Decode.Auto.unsafeFromString<int64>(json, extra=extra)
//                 runner.equal value res

//             runner.testCase "Auto decoders works for uint32" <| fun _ ->
//                 let value = 12u
//                 let json = Encode.Auto.toString(4, value)
//                 let res = Decode.Auto.unsafeFromString<uint32>(json)
//                 runner.equal value res

//             runner.testCase "Auto decoders works for uint64" <| fun _ ->
//                 let extra = Extra.empty |> Extra.withUInt64
//                 let value = 9999999999999999999UL
//                 let json = Encode.Auto.toString(4, value, extra=extra)
//                 let res = Decode.Auto.unsafeFromString<uint64>(json, extra=extra)
//                 runner.equal value res

//             runner.testCase "Auto decoders works for bigint" <| fun _ ->
//                 let extra = Extra.empty |> Extra.withBigInt
//                 let value = 99999999999999999999999I
//                 let json = Encode.Auto.toString(4, value, extra=extra)
//                 let res = Decode.Auto.unsafeFromString<bigint>(json, extra=extra)
//                 runner.equal value res

//             runner.testCase "Auto decoders works for bool" <| fun _ ->
//                 let value = false
//                 let json = Encode.Auto.toString(4, value)
//                 let res = Decode.Auto.unsafeFromString<bool>(json)
//                 runner.equal value res

//             runner.testCase "Auto decoders works for float" <| fun _ ->
//                 let value = 12.
//                 let json = Encode.Auto.toString(4, value)
//                 let res = Decode.Auto.unsafeFromString<float>(json)
//                 runner.equal value res

//             runner.testCase "Auto decoders works for decimal" <| fun _ ->
//                 let extra = Extra.empty |> Extra.withDecimal
//                 let value = 0.7833M
//                 let json = Encode.Auto.toString(4, value, extra=extra)
//                 let res = Decode.Auto.unsafeFromString<decimal>(json, extra=extra)
//                 runner.equal value res

//             runner.testCase "Auto extra decoders can override default decoders" <| fun _ ->
//                 let extra = Extra.empty |> Extra.withCustom IntAsRecord.encode IntAsRecord.decode
//                 let json = """
// {
//     "type": "int",
//     "value": 12
// }
//                 """
//                 let res = Decode.Auto.unsafeFromString<int>(json, extra=extra)
//                 runner.equal 12 res

//             // runner.testCase "Auto decoders works for datetime" <| fun _ ->
//             //     let value = DateTime.Now
//             //     let json = Encode.Auto.toString(4, value)
//             //     let res = Decode.Auto.unsafeFromString<DateTime>(json)
//             //     runner.equal value.Date res.Date
//             //     runner.equal value.Hour res.Hour
//             //     runner.equal value.Minute res.Minute
//             //     runner.equal value.Second res.Second

//             runner.testCase "Auto decoders works for datetime UTC" <| fun _ ->
//                 let value = DateTime.UtcNow
//                 let json = Encode.Auto.toString(4, value)
//                 let res = Decode.Auto.unsafeFromString<DateTime>(json)
//                 runner.equal value.Date res.Date
//                 runner.equal value.Hour res.Hour
//                 runner.equal value.Minute res.Minute
//                 runner.equal value.Second res.Second

//             runner.testCase "Auto decoders works for datetimeOffset" <| fun _ ->
//                 let value = DateTimeOffset.Now
//                 let json = Encode.Auto.toString(4, value)
//                 let res = Decode.Auto.unsafeFromString<DateTimeOffset>(json).ToLocalTime()
//                 runner.equal value.Date res.Date
//                 runner.equal value.Hour res.Hour
//                 runner.equal value.Minute res.Minute
//                 runner.equal value.Second res.Second

//             runner.testCase "Auto decoders works for datetimeOffset UTC" <| fun _ ->
//                 let value = DateTimeOffset.UtcNow
//                 let json = Encode.Auto.toString(4, value)
//                 let res = Decode.Auto.unsafeFromString<DateTimeOffset>(json).ToUniversalTime()
//                 // printfn "SOURCE %A JSON %s OUTPUT %A" value json res
//                 runner.equal value.Date res.Date
//                 runner.equal value.Hour res.Hour
//                 runner.equal value.Minute res.Minute
//                 runner.equal value.Second res.Second

//             runner.testCase "Auto decoders works for TimeSpan" <| fun _ ->
//                 let value = TimeSpan(1,2,3,4,5)
//                 let json = Encode.Auto.toString(4, value)
//                 let res = Decode.Auto.unsafeFromString<TimeSpan>(json)
//                 runner.equal value.Days res.Days
//                 runner.equal value.Hours res.Hours
//                 runner.equal value.Minutes res.Minutes
//                 runner.equal value.Seconds res.Seconds
//                 runner.equal value.Milliseconds res.Milliseconds

//             runner.testCase "Auto decoders works for list" <| fun _ ->
//                 let value = [1; 2; 3; 4]
//                 let json = Encode.Auto.toString(4, value)
//                 let res = Decode.Auto.unsafeFromString<int list>(json)
//                 runner.equal value res

//             runner.testCase "Auto decoders works for array" <| fun _ ->
//                 let value = [| 1; 2; 3; 4 |]
//                 let json = Encode.Auto.toString(4, value)
//                 let res = Decode.Auto.unsafeFromString<int array>(json)
//                 runner.equal value res

//             runner.testCase "Auto decoders works for Map with string keys" <| fun _ ->
//                 let value = Map.ofSeq [ "a", 1; "b", 2; "c", 3 ]
//                 let json = Encode.Auto.toString(4, value)
//                 let res = Decode.Auto.unsafeFromString<Map<string, int>>(json)
//                 runner.equal value res

//             runner.testCase "Auto decoders works for Map with complex keys" <| fun _ ->
//                 let value = Map.ofSeq [ (1, 6), "a"; (2, 7), "b"; (3, 8), "c" ]
//                 let json = Encode.Auto.toString(4, value)
//                 let res = Decode.Auto.unsafeFromString<Map<int * int, string>>(json)
//                 runner.equal value res

//             runner.testCase "Auto decoders works for option None" <| fun _ ->
//                 let value = None
//                 let json = Encode.Auto.toString(4, value)
//                 let res = Decode.Auto.unsafeFromString<int option>(json)
//                 runner.equal value res

//             runner.testCase "Auto decoders works for option Some" <| fun _ ->
//                 let value = Some 5
//                 let json = Encode.Auto.toString(4, value)
//                 let res = Decode.Auto.unsafeFromString<int option>(json)
//                 runner.equal value res

//             runner.testCase "Auto decoders works for Unit" <| fun _ ->
//                 let value = ()
//                 let json = Encode.Auto.toString(4, value)
//                 let res = Decode.Auto.unsafeFromString(json)
//                 runner.equal value res

//             runner.testCase "Auto decoders works for enum<int8>" <| fun _ ->
//                 let res = Decode.Auto.unsafeFromString<Enum_Int8>("99")
//                 runner.equal Enum_Int8.NinetyNine res

//             runner.testCase "Auto decoders for enum<int8> returns an error if the Enum value is invalid" <| fun _ ->
// #if FABLE_COMPILER
//                 let value =
//                     Error(
//                         """
// Error at: `$`
// Expecting Tests.Types.Enum_Int8[System.SByte] but instead got: 2
// Reason: Unkown value provided for the enum
//                         """.Trim())
// #else
//                 let value =
//                     Error(
//                         """
// Error at: `$`
// Expecting Tests.Types+Enum_Int8 but instead got: 2
// Reason: Unkown value provided for the enum
//                         """.Trim())
// #endif

//                 let res = Decode.Auto.fromString<Enum_Int8>("2")
//                 runner.equal value res

//             runner.testCase "Auto decoders works for enum<uint8>" <| fun _ ->
//                 let res = Decode.Auto.unsafeFromString<Enum_UInt8>("99")
//                 runner.equal Enum_UInt8.NinetyNine res

//             runner.testCase "Auto decoders for enum<uint8> returns an error if the Enum value is invalid" <| fun _ ->
// #if FABLE_COMPILER
//                 let value =
//                     Error(
//                         """
// Error at: `$`
// Expecting Tests.Types.Enum_UInt8[System.Byte] but instead got: 2
// Reason: Unkown value provided for the enum
//                         """.Trim())
// #else
//                 let value =
//                     Error(
//                         """
// Error at: `$`
// Expecting Tests.Types+Enum_UInt8 but instead got: 2
// Reason: Unkown value provided for the enum
//                         """.Trim())
// #endif

//                 let res = Decode.Auto.fromString<Enum_UInt8>("2")
//                 runner.equal value res

//             runner.testCase "Auto decoders works for enum<int16>" <| fun _ ->
//                 let res = Decode.Auto.unsafeFromString<Enum_Int16>("99")
//                 runner.equal Enum_Int16.NinetyNine res

//             runner.testCase "Auto decoders for enum<int16> returns an error if the Enum value is invalid" <| fun _ ->
// #if FABLE_COMPILER
//                 let value =
//                     Error(
//                         """
// Error at: `$`
// Expecting Tests.Types.Enum_Int16[System.Int16] but instead got: 2
// Reason: Unkown value provided for the enum
//                         """.Trim())
// #else
//                 let value =
//                     Error(
//                         """
// Error at: `$`
// Expecting Tests.Types+Enum_Int16 but instead got: 2
// Reason: Unkown value provided for the enum
//                         """.Trim())
// #endif

//                 let res = Decode.Auto.fromString<Enum_Int16>("2")
//                 runner.equal value res

//             runner.testCase "Auto decoders works for enum<uint16>" <| fun _ ->
//                 let res = Decode.Auto.unsafeFromString<Enum_UInt16>("99")
//                 runner.equal Enum_UInt16.NinetyNine res

//             runner.testCase "Auto decoders for enum<ºint16> returns an error if the Enum value is invalid" <| fun _ ->
// #if FABLE_COMPILER
//                 let value =
//                     Error(
//                         """
// Error at: `$`
// Expecting Tests.Types.Enum_UInt16[System.UInt16] but instead got: 2
// Reason: Unkown value provided for the enum
//                         """.Trim())
// #else
//                 let value =
//                     Error(
//                         """
// Error at: `$`
// Expecting Tests.Types+Enum_UInt16 but instead got: 2
// Reason: Unkown value provided for the enum
//                         """.Trim())
// #endif

//                 let res = Decode.Auto.fromString<Enum_UInt16>("2")
//                 runner.equal value res

//             runner.testCase "Auto decoders works for enum<int>" <| fun _ ->
//                 let res = Decode.Auto.unsafeFromString<Enum_Int>("1")
//                 runner.equal Enum_Int.One res

//             runner.testCase "Auto decoders for enum<int> returns an error if the Enum value is invalid" <| fun _ ->
// #if FABLE_COMPILER
//                 let value =
//                     Error(
//                         """
// Error at: `$`
// Expecting Tests.Types.Enum_Int[System.Int32] but instead got: 4
// Reason: Unkown value provided for the enum
//                         """.Trim())
// #else
//                 let value =
//                     Error(
//                         """
// Error at: `$`
// Expecting Tests.Types+Enum_Int but instead got: 4
// Reason: Unkown value provided for the enum
//                         """.Trim())
// #endif

//                 let res = Decode.Auto.fromString<Enum_Int>("4")
//                 runner.equal value res

//             runner.testCase "Auto decoders works for enum<uint32>" <| fun _ ->
//                 let res = Decode.Auto.unsafeFromString<Enum_UInt32>("99")
//                 runner.equal Enum_UInt32.NinetyNine res

//             runner.testCase "Auto decoders for enum<uint32> returns an error if the Enum value is invalid" <| fun _ ->
// #if FABLE_COMPILER
//                 let value =
//                     Error(
//                         """
// Error at: `$`
// Expecting Tests.Types.Enum_UInt32[System.UInt32] but instead got: 2
// Reason: Unkown value provided for the enum
//                         """.Trim())
// #else
//                 let value =
//                     Error(
//                         """
// Error at: `$`
// Expecting Tests.Types+Enum_UInt32 but instead got: 2
// Reason: Unkown value provided for the enum
//                         """.Trim())
// #endif

//                 let res = Decode.Auto.fromString<Enum_UInt32>("2")
//                 runner.equal value res

//     (*
//             #if NETFRAMEWORK
//             runner.testCase "Auto decoders  works with char based Enums" <| fun _ ->
//                 let value = CharEnum.A
//                 let json = Encode.Auto.toString(4, value)
//                 let res = Decode.Auto.unsafeFromString<CharEnum>(json)
//                 runner.equal value res
//             #endif
//     *)
//             runner.testCase "Auto decoders works for null" <| fun _ ->
//                 let value = null
//                 let json = Encode.Auto.toString(4, value)
//                 let res = Decode.Auto.unsafeFromString<obj>(json)
//                 runner.equal value res

//             runner.testCase "Auto decoders works for anonymous record" <| fun _ ->
//                 let value = {| A = "string" |}
//                 let json = Encode.Auto.toString(4, value)
//                 let res = Decode.Auto.unsafeFromString(json)
//                 runner.equal value res

//             runner.testCase "Auto decoders works for nested anonymous record" <| fun _ ->
//                 let value = {| A = {| B = "string" |} |}
//                 let json = Encode.Auto.toString(4, value)
//                 let res = Decode.Auto.unsafeFromString(json)
//                 runner.equal value res

//             runner.testCase "Auto decoders works even if type is determined by the compiler" <| fun _ ->
//                 let value = [1; 2; 3; 4]
//                 let json = Encode.Auto.toString(4, value)
//                 let res = Decode.Auto.unsafeFromString<_>(json)
//                 runner.equal value res

//             runner.testCase "Auto.unsafeFromString works with camelCase" <| fun _ ->
//                 let json = """{ "id" : 0, "name": "maxime", "email": "mail@domain.com", "followers": 0 }"""
//                 let user = Decode.Auto.unsafeFromString<User>(json, caseStrategy=CamelCase)
//                 runner.equal "maxime" user.Name
//                 runner.equal 0 user.Id
//                 runner.equal 0 user.Followers
//                 runner.equal "mail@domain.com" user.Email

//             runner.testCase "Auto.fromString works with snake_case" <| fun _ ->
//                 let json = """{ "one" : 1, "two_part": 2, "three_part_field": 3 }"""
//                 let decoded = Decode.Auto.fromString<RecordForCharacterCase>(json, caseStrategy=SnakeCase)
//                 let expected = Ok { One = 1; TwoPart = 2; ThreePartField = 3 }
//                 runner.equal expected decoded

//             runner.testCase "Auto.fromString works with camelCase" <| fun _ ->
//                 let json = """{ "id" : 0, "name": "maxime", "email": "mail@domain.com", "followers": 0 }"""
//                 let user = Decode.Auto.fromString<User>(json, caseStrategy=CamelCase)
//                 let expected = Ok { Id = 0; Name = "maxime"; Email = "mail@domain.com"; Followers = 0 }
//                 runner.equal expected user

//             runner.testCase "Auto.fromString works for records with an actual value for the optional field value" <| fun _ ->
//                 let json = """{ "maybe" : "maybe value", "must": "must value"}"""
//                 let actual = Decode.Auto.fromString<TestMaybeRecord>(json, caseStrategy=CamelCase)
//                 let expected =
//                     Ok ({ Maybe = Some "maybe value"
//                           Must = "must value" } : TestMaybeRecord)
//                 runner.equal expected actual

//             runner.testCase "Auto.fromString works for records with `null` for the optional field value" <| fun _ ->
//                 let json = """{ "maybe" : null, "must": "must value"}"""
//                 let actual = Decode.Auto.fromString<TestMaybeRecord>(json, caseStrategy=CamelCase)
//                 let expected =
//                     Ok ({ Maybe = None
//                           Must = "must value" } : TestMaybeRecord)
//                 runner.equal expected actual

//             runner.testCase "Auto.fromString works for records with `null` for the optional field value on classes" <| fun _ ->
//                 let json = """{ "maybeClass" : null, "must": "must value"}"""
//                 let actual = Decode.Auto.fromString<RecordWithOptionalClass>(json, caseStrategy=CamelCase)
//                 let expected =
//                     Ok ({ MaybeClass = None
//                           Must = "must value" } : RecordWithOptionalClass)
//                 runner.equal expected actual

//             runner.testCase "Auto.fromString works for records missing optional field value on classes" <| fun _ ->
//                 let json = """{ "must": "must value"}"""
//                 let actual = Decode.Auto.fromString<RecordWithOptionalClass>(json, caseStrategy=CamelCase)
//                 let expected =
//                     Ok ({ MaybeClass = None
//                           Must = "must value" } : RecordWithOptionalClass)
//                 runner.equal expected actual

//             runner.testCase "Auto.generateDecoder throws for field using a non optional class" <| fun _ ->
//                 let expected = """Cannot generate auto decoder for Tests.Types.BaseClass. Please pass an extra decoder.

// Documentation available at: https://thoth-org.github.io/Thoth.Json/documentation/auto/extra-coders.html#ready-to-use-extra-coders"""

//                 let errorMsg =
//                     try
//                         let decoder = Decode.Auto.generateDecoder<RecordWithRequiredClass>(caseStrategy=CamelCase)
//                         ""
//                     with ex ->
//                         ex.Message
//                 errorMsg.Replace("+", ".") |> runner.equal expected

//             runner.testCase "Auto.fromString works for Class marked as optional" <| fun _ ->
//                 let json = """null"""

//                 let actual = Decode.Auto.fromString<BaseClass option>(json, caseStrategy=CamelCase)
//                 let expected = Ok None
//                 runner.equal expected actual

//             runner.testCase "Auto.generateDecoder throws for Class" <| fun _ ->
//                 let expected = """Cannot generate auto decoder for Tests.Types.BaseClass. Please pass an extra decoder.

// Documentation available at: https://thoth-org.github.io/Thoth.Json/documentation/auto/extra-coders.html#ready-to-use-extra-coders"""

//                 let errorMsg =
//                     try
//                         let decoder = Decode.Auto.generateDecoder<BaseClass>(caseStrategy=CamelCase)
//                         ""
//                     with ex ->
//                         ex.Message
//                 errorMsg.Replace("+", ".") |> runner.equal expected

//             runner.testCase "Auto.fromString works for records missing an optional field" <| fun _ ->
//                 let json = """{ "must": "must value"}"""
//                 let actual = Decode.Auto.fromString<TestMaybeRecord>(json, caseStrategy=CamelCase)
//                 let expected =
//                     Ok ({ Maybe = None
//                           Must = "must value" } : TestMaybeRecord)
//                 runner.equal expected actual

//             runner.testCase "Auto.fromString works with maps encoded as objects" <| fun _ ->
//                 let expected = Map [("oh", { a = 2.; b = 2. }); ("ah", { a = -1.5; b = 0. })]
//                 let json = """{"ah":{"a":-1.5,"b":0},"oh":{"a":2,"b":2}}"""
//                 let actual = Decode.Auto.fromString json
//                 runner.equal (Ok expected) actual

//             runner.testCase "Auto.fromString works with maps encoded as arrays" <| fun _ ->
//                 let expected = Map [({ a = 2.; b = 2. }, "oh"); ({ a = -1.5; b = 0. }, "ah")]
//                 let json = """[[{"a":-1.5,"b":0},"ah"],[{"a":2,"b":2},"oh"]]"""
//                 let actual = Decode.Auto.fromString json
//                 runner.equal (Ok expected) actual

//             runner.testCase "Decoder.Auto.toString works with bigint extra" <| fun _ ->
//                 let extra = Extra.empty |> Extra.withBigInt
//                 let expected = { bigintField = 9999999999999999999999I }
//                 let actual = Decode.Auto.fromString("""{"bigintField":"9999999999999999999999"}""", extra=extra)
//                 runner.equal (Ok expected) actual

//             runner.testCase "Decoder.Auto.toString works with custom extra" <| fun _ ->
//                 let extra = Extra.empty |> Extra.withCustom ChildType.Encode ChildType.Decoder
//                 let expected = { ParentField = { ChildField = "bumbabon" } }
//                 let actual = Decode.Auto.fromString("""{"ParentField":"bumbabon"}""", extra=extra)
//                 runner.equal (Ok expected) actual

//             runner.testCase "Auto.fromString works with records with private constructors" <| fun _ ->
//                 let json = """{ "foo1": 5, "foo2": 7.8 }"""
//                 Decode.Auto.fromString(json, caseStrategy=CamelCase)
//                 |> runner.equal (Ok ({ Foo1 = 5; Foo2 = 7.8 }: RecordWithPrivateConstructor))

//             runner.testCase "Auto.fromString works with unions with private constructors" <| fun _ ->
//                 let json = """[ "Baz", ["Bar", "foo"]]"""
//                 Decode.Auto.fromString<UnionWithPrivateConstructor list>(json, caseStrategy=CamelCase)
//                 |> runner.equal (Ok [Baz; Bar "foo"])

//             runner.testCase "Auto.fromString works gives proper error for wrong union fields" <| fun _ ->
//                 let json = """["Multi", "bar", "foo", "zas"]"""
//                 Decode.Auto.fromString<UnionWithMultipleFields>(json, caseStrategy=CamelCase)
//                 |> runner.equal (Error "Error at: `$[2]`\nExpecting an int but instead got: \"foo\"")

//             // TODO: Should we allow shorter arrays when last fields are options?
//             runner.testCase "Auto.fromString works gives proper error for wrong array length" <| fun _ ->
//                 let json = """["Multi", "bar", 1]"""
//                 Decode.Auto.fromString<UnionWithMultipleFields>(json, caseStrategy=CamelCase)
//                 |> runner.equal (Error "Error at: `$`\nThe following `failure` occurred with the decoder: Expected array of length 4 but got 3")

//             runner.testCase "Auto.fromString works gives proper error for wrong array length when no fields" <| fun _ ->
//                 let json = """["Multi"]"""
//                 Decode.Auto.fromString<UnionWithMultipleFields>(json, caseStrategy=CamelCase)
//                 |> runner.equal (Error "Error at: `$`\nThe following `failure` occurred with the decoder: Expected array of length 4 but got 1")

//             runner.testCase "Auto.fromString works gives proper error for wrong case name" <| fun _ ->
//                 let json = """[1]"""
//                 Decode.Auto.fromString<UnionWithMultipleFields>(json, caseStrategy=CamelCase)
//                 |> runner.equal (Error "Error at: `$[0]`\nExpecting a string but instead got: 1")

//             runner.testCase "Auto.generateDecoderCached works" <| fun _ ->
//                 let expected = Ok { Id = 0; Name = "maxime"; Email = "mail@domain.com"; Followers = 0 }
//                 let json = """{ "id" : 0, "name": "maxime", "email": "mail@domain.com", "followers": 0 }"""
//                 let decoder1 = Decode.Auto.generateDecoderCached<User>(caseStrategy=CamelCase)
//                 let decoder2 = Decode.Auto.generateDecoderCached<User>(caseStrategy=CamelCase)
//                 let actual1 = runner.Decode.fromString decoder1 json
//                 let actual2 = runner.Decode.fromString decoder2 json
//                 runner.equal expected actual1
//                 runner.equal expected actual2
//                 runner.equal actual1 actual2

//             runner.testCase "Auto.fromString works with strange types if they are None" <| fun _ ->
//                 let json = """{"Id":0}"""
//                 Decode.Auto.fromString<RecordWithStrangeType>(json)
//                 |> runner.equal (Ok { Id = 0; Thread = None })

//             runner.testCase "Auto.fromString works with recursive types" <| fun _ ->
//                 let vater =
//                     { Name = "Alfonso"
//                       Children = [ { Name = "Narumi"; Children = [] }
//                                    { Name = "Takumi"; Children = [] } ] }
//                 let json = """{"Name":"Alfonso","Children":[{"Name":"Narumi","Children":[]},{"Name":"Takumi","Children":[]}]}"""
//                 Decode.Auto.fromString<MyRecType>(json)
//                 |> runner.equal (Ok vater)

//             runner.testCase "Auto.unsafeFromString works for unit" <| fun _ ->
//                 let json = Encode.unit () |> Encode.toString 4
//                 let res = Decode.Auto.unsafeFromString<unit>(json)
//                 runner.equal () res

//             runner.testCase "Erased single-case DUs works" <| fun _ ->
//                 let expected = NoAllocAttributeId (Guid.NewGuid())
//                 let json = Encode.Auto.toString(4, expected)
//                 let actual = Decode.Auto.unsafeFromString<NoAllocAttributeId>(json)
//                 runner.equal expected actual

//             runner.testCase "Auto.unsafeFromString works with HTML inside of a string" <| fun _ ->
//                 let expected =
//                     {
//                         FeedName = "Ars"
//                         Content = "<div><figure class=\"intro-image intro-left\"><img src=\"https://cdn.arstechnica.net/wp-content/uploads/2019/05/qualcomm-enforcer-800x450.jpg\" alt=\"How Qualcomm shook down the cell phone industry for almost 20 years\"><p class=\"caption\" style=\"font-size: 0.8em\"><a href=\"https://cdn.arstechnica.net/wp-content/uploads/2019/05/qualcomm-enforcer.jpg\" class=\"enlarge-link\">Enlarge</a> (credit: Getty / Aurich Lawson)</p>  </figure><div><a name=\"page-1\"></a></div><p>In 2005, Apple contacted Qualcomm as a potential supplier for modem chips in the first iPhone. Qualcomm's response was unusual: a letter demanding that Apple sign a patent licensing agreement before Qualcomm would even consider supplying chips.</p><p>\"I'd spent 20 years in the industry, I had never seen a letter like this,\" said Tony Blevins, Apple's vice president of procurement.</p><p>Most suppliers are eager to talk to new customers—especially customers as big and prestigious as Apple. But Qualcomm wasn't like other suppliers; it enjoyed a dominant position in the market for cellular chips. That gave Qualcomm a lot of leverage, and the company wasn't afraid to use it.</p></div><p><a href=\"https://arstechnica.com/?p=1510419#p3\">Read 70 remaining paragraphs</a> | <a href=\"https://arstechnica.com/?p=1510419&amp;comments=1\">Comments</a></p><div class=\"feedflare\"><a href=\"http://feeds.arstechnica.com/~ff/arstechnica/index?a=7NLlD3YvqFA:DF_-B3_cDwc:V_sGLiPBpWU\"><img src=\"http://feeds.feedburner.com/~ff/arstechnica/index?i=7NLlD3YvqFA:DF_-B3_cDwc:V_sGLiPBpWU\" border=\"0\"></a> <a href=\"http://feeds.arstechnica.com/~ff/arstechnica/index?a=7NLlD3YvqFA:DF_-B3_cDwc:F7zBnMyn0Lo\"><img src=\"http://feeds.feedburner.com/~ff/arstechnica/index?i=7NLlD3YvqFA:DF_-B3_cDwc:F7zBnMyn0Lo\" border=\"0\"></a> <a href=\"http://feeds.arstechnica.com/~ff/arstechnica/index?a=7NLlD3YvqFA:DF_-B3_cDwc:qj6IDK7rITs\"><img src=\"http://feeds.feedburner.com/~ff/arstechnica/index?d=qj6IDK7rITs\" border=\"0\"></a> <a href=\"http://feeds.arstechnica.com/~ff/arstechnica/index?a=7NLlD3YvqFA:DF_-B3_cDwc:yIl2AUoC8zA\"><img src=\"http://feeds.feedburner.com/~ff/arstechnica/index?d=yIl2AUoC8zA\" border=\"0\"></a></div>"
//                     }

//                 let articleJson =
//                     """
//                 {
//                   "FeedName": "Ars",
//                   "Content": "<div><figure class=\"intro-image intro-left\"><img src=\"https://cdn.arstechnica.net/wp-content/uploads/2019/05/qualcomm-enforcer-800x450.jpg\" alt=\"How Qualcomm shook down the cell phone industry for almost 20 years\"><p class=\"caption\" style=\"font-size: 0.8em\"><a href=\"https://cdn.arstechnica.net/wp-content/uploads/2019/05/qualcomm-enforcer.jpg\" class=\"enlarge-link\">Enlarge</a> (credit: Getty / Aurich Lawson)</p>  </figure><div><a name=\"page-1\"></a></div><p>In 2005, Apple contacted Qualcomm as a potential supplier for modem chips in the first iPhone. Qualcomm's response was unusual: a letter demanding that Apple sign a patent licensing agreement before Qualcomm would even consider supplying chips.</p><p>\"I'd spent 20 years in the industry, I had never seen a letter like this,\" said Tony Blevins, Apple's vice president of procurement.</p><p>Most suppliers are eager to talk to new customers—especially customers as big and prestigious as Apple. But Qualcomm wasn't like other suppliers; it enjoyed a dominant position in the market for cellular chips. That gave Qualcomm a lot of leverage, and the company wasn't afraid to use it.</p></div><p><a href=\"https://arstechnica.com/?p=1510419#p3\">Read 70 remaining paragraphs</a> | <a href=\"https://arstechnica.com/?p=1510419&amp;comments=1\">Comments</a></p><div class=\"feedflare\"><a href=\"http://feeds.arstechnica.com/~ff/arstechnica/index?a=7NLlD3YvqFA:DF_-B3_cDwc:V_sGLiPBpWU\"><img src=\"http://feeds.feedburner.com/~ff/arstechnica/index?i=7NLlD3YvqFA:DF_-B3_cDwc:V_sGLiPBpWU\" border=\"0\"></a> <a href=\"http://feeds.arstechnica.com/~ff/arstechnica/index?a=7NLlD3YvqFA:DF_-B3_cDwc:F7zBnMyn0Lo\"><img src=\"http://feeds.feedburner.com/~ff/arstechnica/index?i=7NLlD3YvqFA:DF_-B3_cDwc:F7zBnMyn0Lo\" border=\"0\"></a> <a href=\"http://feeds.arstechnica.com/~ff/arstechnica/index?a=7NLlD3YvqFA:DF_-B3_cDwc:qj6IDK7rITs\"><img src=\"http://feeds.feedburner.com/~ff/arstechnica/index?d=qj6IDK7rITs\" border=\"0\"></a> <a href=\"http://feeds.arstechnica.com/~ff/arstechnica/index?a=7NLlD3YvqFA:DF_-B3_cDwc:yIl2AUoC8zA\"><img src=\"http://feeds.feedburner.com/~ff/arstechnica/index?d=yIl2AUoC8zA\" border=\"0\"></a></div>"
//                 }
//                     """

//                 let actual : TestStringWithHTML = Decode.Auto.unsafeFromString(articleJson)
//                 runner.equal expected actual
//         ]
    ]
