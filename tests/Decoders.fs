module Tests.Decoders

#if !NETFRAMEWORK
open Fable.Core
#endif

open Fable.Core.JsInterop
open Thoth.Json
open Util.Testing
open System

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

open Tests.Types

type RecordWithPrivateConstructor = private { Foo1: int; Foo2: float }
type UnionWithPrivateConstructor = private Bar of string | Baz
type UnionWithMultipleFields = Multi of string * int * float

let tests : Test =
    testList "Thoth.Json.Decode" [

        testList "Errors" [

            #if FABLE_COMPILER

            testCase "circular structure are supported when reporting error" <| fun _ ->
                let a = createObj [ ]
                let b = createObj [ ]
                a?child <- b
                b?child <- a

                let expected : Result<float, string> = Error "Error at: `$`\nExpecting a float but decoder failed. Couldn\'t report given value due to circular structure. "
                let actual = Decode.fromValue "$" Decode.float b

                equal expected actual
            #endif

            testCase "invalid json" <| fun _ ->
                #if FABLE_COMPILER
                let expected : Result<float, string> = Error "Given an invalid JSON: Unexpected token m in JSON at position 0"
                #else
                let expected : Result<float, string> = Error "Given an invalid JSON: Unexpected character encountered while parsing value: m. Path '', line 0, position 0."
                #endif
                let actual = Decode.fromString Decode.float "maxime"

                equal expected actual

            testCase "invalid json #2 - Special case for Thoth.Json.Net" <| fun _ ->
                // See: https://github.com/thoth-org/Thoth.Json.Net/issues/42
                #if FABLE_COMPILER
                let expected : Result<MyUnion, string> = Error "Given an invalid JSON: Unexpected token , in JSON at position 5"
                #else
                let expected : Result<MyUnion, string> = Error "Given an invalid JSON: Additional text encountered after finished reading JSON content: ,. Path '', line 1, position 5."
                #endif
                let actual = Decode.Auto.fromString<MyUnion>(""""Foo","42"]""")

                equal expected actual

            testCase "user exceptions are not captured by the decoders" <| fun _ ->
                let expected = true

                let decoder =
                    (fun _ _ ->
                        raise CustomException
                    )

                let actual =
                    try
                        Decode.fromString decoder "\"maxime\""
                        |> ignore // Ignore the result as we only want to trigger the decoder and capture the exception
                        false
                    with
                        | CustomException ->
                            true

                equal expected actual
        ]

        testList "Primitives" [

            testCase "unit works" <| fun _ ->
                let expected = Ok ()
                let actual =
                    Decode.fromString Decode.unit "null"

                equal expected actual

            testCase "a string works" <| fun _ ->
                let expected = Ok("maxime")
                let actual =
                    Decode.fromString Decode.string "\"maxime\""

                equal expected actual

            testCase "a string with new line works" <| fun _ ->
                let expected = Ok("a\nb")
                let actual =
                    Decode.fromString Decode.string "\"a\\nb\""

                equal expected actual

            testCase "a string with new line character works" <| fun _ ->
                let expected = Ok("a\\nb")
                let actual =
                    Decode.fromString Decode.string "\"a\\\\nb\""

                equal expected actual

            testCase "a string with tab works" <| fun _ ->
                let expected = Ok("a\tb")
                let actual =
                    Decode.fromString Decode.string "\"a\\tb\""

                equal expected actual

            testCase "a string with tab character works" <| fun _ ->
                let expected = Ok("a\\tb")
                let actual =
                    Decode.fromString Decode.string "\"a\\\\tb\""

                equal expected actual

            testCase "a float works" <| fun _ ->
                let expected = Ok(1.2)
                let actual =
                    Decode.fromString Decode.float "1.2"

                equal expected actual

            testCase "a float from int works" <| fun _ ->
                let expected = Ok(1.0)
                let actual =
                    Decode.fromString Decode.float "1"

                equal expected actual

            testCase "a bool works" <| fun _ ->
                let expected = Ok(true)
                let actual =
                    Decode.fromString Decode.bool "true"

                equal expected actual

            testCase "an invalid bool output an error" <| fun _ ->
                let expected = Error("Error at: `$`\nExpecting a boolean but instead got: 2")
                let actual =
                    Decode.fromString Decode.bool "2"

                equal expected actual

            testCase "an int works" <| fun _ ->
                let expected = Ok(25)
                let actual =
                    Decode.fromString Decode.int "25"

                equal expected actual

            testCase "an invalid int [invalid range: too big] output an error" <| fun _ ->
                let expected = Error("Error at: `$`\nExpecting an int but instead got: 2147483648\nReason: Value was either too large or too small for an int")
                let actual =
                    Decode.fromString Decode.int "2147483648"

                equal expected actual


            testCase "an invalid int [invalid range: too small] output an error" <| fun _ ->
                let expected = Error("Error at: `$`\nExpecting an int but instead got: -2147483649\nReason: Value was either too large or too small for an int")
                let actual =
                    Decode.fromString Decode.int "-2147483649"

                equal expected actual

            testCase "an int16 works from number" <| fun _ ->
                let expected = Ok(int16 25)
                let actual =
                    Decode.fromString Decode.int16 "25"

                equal expected actual

            testCase "an int16 works from string" <| fun _ ->
                let expected = Ok(int16 -25)
                let actual =
                    Decode.fromString Decode.int16 "\"-25\""

                equal expected actual

            testCase "an int16 output an error if value is too big" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting an int16 but instead got: 32768
Reason: Value was either too large or too small for an int16
                        """.Trim())
                let actual =
                    Decode.fromString Decode.int16 "32768"

                equal expected actual

            testCase "an int16 output an error if value is too small" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting an int16 but instead got: -32769
Reason: Value was either too large or too small for an int16
                        """.Trim())
                let actual =
                    Decode.fromString Decode.int16 "-32769"

                equal expected actual

            testCase "an int16 output an error if incorrect string" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting an int16 but instead got: "maxime"
                        """.Trim())
                let actual =
                    Decode.fromString Decode.int16 "\"maxime\""

                equal expected actual

            testCase "an uint16 works from number" <| fun _ ->
                let expected = Ok(uint16 25)
                let actual =
                    Decode.fromString Decode.uint16 "25"

                equal expected actual

            testCase "an uint16 works from string" <| fun _ ->
                let expected = Ok(uint16 25)
                let actual =
                    Decode.fromString Decode.uint16 "\"25\""

                equal expected actual

            testCase "an uint16 output an error if value is too big" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting an uint16 but instead got: 65536
Reason: Value was either too large or too small for an uint16
                        """.Trim())
                let actual =
                    Decode.fromString Decode.uint16 "65536"

                equal expected actual

            testCase "an uint16 output an error if value is too small" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting an uint16 but instead got: -1
Reason: Value was either too large or too small for an uint16
                        """.Trim())
                let actual =
                    Decode.fromString Decode.uint16 "-1"

                equal expected actual

            testCase "an uint16 output an error if incorrect string" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting an uint16 but instead got: "maxime"
                        """.Trim())
                let actual =
                    Decode.fromString Decode.uint16 "\"maxime\""

                equal expected actual

            testCase "an int64 works from number" <| fun _ ->
                let expected = Ok 1000L
                let actual =
                    Decode.fromString Decode.int64 "1000"

                equal expected actual

            testCase "an int64 works from string" <| fun _ ->
                let expected = Ok 99L
                let actual =
                    Decode.fromString Decode.int64 "\"99\""

                equal expected actual

            testCase "an int64 works output an error if incorrect string" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting an int64 but instead got: "maxime"
                        """.Trim())
                let actual =
                    Decode.fromString Decode.int64 "\"maxime\""

                equal expected actual

            testCase "an uint32 works from number" <| fun _ ->
                let expected = Ok 1000u
                let actual =
                    Decode.fromString Decode.uint32 "1000"

                equal expected actual

            testCase "an uint32 works from string" <| fun _ ->
                let expected = Ok 1000u
                let actual =
                    Decode.fromString Decode.uint32 "\"1000\""

                equal expected actual

            testCase "an uint32 output an error if incorrect string" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting an uint32 but instead got: "maxime"
                        """.Trim())

                let actual =
                    Decode.fromString Decode.uint32 "\"maxime\""

                equal expected actual

            testCase "an uint64 works from number" <| fun _ ->
                let expected = Ok 1000UL
                let actual =
                    Decode.fromString Decode.uint64 "1000"

                equal expected actual

            testCase "an uint64 works from string" <| fun _ ->
                let expected = Ok 1000UL
                let actual =
                    Decode.fromString Decode.uint64 "\"1000\""

                equal expected actual

            testCase "an uint64 output an error if incorrect string" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting an uint64 but instead got: "maxime"
                        """.Trim())

                let actual =
                    Decode.fromString Decode.uint64 "\"maxime\""

                equal expected actual

            testCase "a byte works from number" <| fun _ ->
                let expected = Ok 25uy
                let actual =
                    Decode.fromString Decode.byte "25"

                equal expected actual

            testCase "a byte works from string" <| fun _ ->
                let expected = Ok 25uy
                let actual =
                    Decode.fromString Decode.byte "\"25\""

                equal expected actual

            testCase "a byte output an error if value is too big" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting a byte but instead got: 256
Reason: Value was either too large or too small for a byte
                        """.Trim())
                let actual =
                    Decode.fromString Decode.byte "256"

                equal expected actual

            testCase "a byte output an error if value is too small" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting a byte but instead got: -1
Reason: Value was either too large or too small for a byte
                        """.Trim())
                let actual =
                    Decode.fromString Decode.byte "-1"

                equal expected actual

            testCase "a byte output an error if incorrect string" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting a byte but instead got: "maxime"
                        """.Trim())
                let actual =
                    Decode.fromString Decode.byte "\"maxime\""

                equal expected actual


            testCase "a sbyte works from number" <| fun _ ->
                let expected = Ok 25y
                let actual =
                    Decode.fromString Decode.sbyte "25"

                equal expected actual

            testCase "a sbyte works from string" <| fun _ ->
                let expected = Ok -25y
                let actual =
                    Decode.fromString Decode.sbyte "\"-25\""

                equal expected actual

            testCase "a sbyte output an error if value is too big" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting a sbyte but instead got: 128
Reason: Value was either too large or too small for a sbyte
                        """.Trim())
                let actual =
                    Decode.fromString Decode.sbyte "128"

                equal expected actual

            testCase "a sbyte output an error if value is too small" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting a sbyte but instead got: -129
Reason: Value was either too large or too small for a sbyte
                        """.Trim())
                let actual =
                    Decode.fromString Decode.sbyte "-129"

                equal expected actual

            testCase "a sbyte output an error if incorrect string" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting a sbyte but instead got: "maxime"
                        """.Trim())
                let actual =
                    Decode.fromString Decode.sbyte "\"maxime\""

                equal expected actual

            testCase "an bigint works from number" <| fun _ ->
                let expected = Ok 12I
                let actual =
                    Decode.fromString Decode.bigint "12"

                equal expected actual

            testCase "an bigint works from string" <| fun _ ->
                let expected = Ok 12I
                let actual =
                    Decode.fromString Decode.bigint "\"12\""

                equal expected actual

            testCase "an bigint output an error if invalid string" <| fun _ ->
                let expected =
                    Error (
                        """
Error at: `$`
Expecting a bigint but instead got: "maxime"
                        """.Trim())
                let actual =
                    Decode.fromString Decode.bigint "\"maxime\""

                equal expected actual

            testCase "a string representing a DateTime should be accepted as a string" <| fun _ ->
                let expected = "2018-10-01T11:12:55.00Z"
                let actual =
                    Decode.fromString Decode.string "\"2018-10-01T11:12:55.00Z\""

                equal (Ok expected) actual

            testCase "a datetime works" <| fun _ ->
                let expected = new DateTime(2018, 10, 1, 11, 12, 55, DateTimeKind.Utc)
                let actual =
                    Decode.fromString Decode.datetime "\"2018-10-01T11:12:55.00Z\""

                equal (Ok expected) actual

            testCase "a datetime output an error if invalid string" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting a datetime but instead got: "invalid_string"
                        """.Trim())

                let actual =
                    Decode.fromString Decode.datetime "\"invalid_string\""

                equal expected actual

            testCase "a datetime works with TimeZone" <| fun _ ->
                let localDate = DateTime(2018, 10, 1, 11, 12, 55, DateTimeKind.Local)

                let expected = Ok (localDate.ToUniversalTime())
                let json = sprintf "\"%s\"" (localDate.ToString("O"))
                let actual =
                    Decode.fromString Decode.datetime json

                equal expected actual

            testCase "a datetimeOffset works" <| fun _ ->
                let expected =
                    DateTimeOffset(2018, 7, 2, 12, 23, 45, 0, TimeSpan.FromHours(2.))
                    |> Ok
                let json = "\"2018-07-02T12:23:45+02:00\""
                let actual =
                    Decode.fromString Decode.datetimeOffset json
                equal expected actual

            testCase "a datetimeOffset returns Error if invalid format" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting a datetimeoffset but instead got: "NOT A DATETIMEOFFSET"
                        """.Trim())
                let json = "\"NOT A DATETIMEOFFSET\""
                let actual =
                    Decode.fromString Decode.datetimeOffset json

                equal expected actual

            testCase "a timespan works" <| fun _ ->
                let expected =
                    TimeSpan(23, 45, 0)
                    |> Ok
                let json = "\"23:45:00\""
                let actual =
                    Decode.fromString Decode.timespan json
                equal expected actual

            testCase "a timespan returns Error if invalid format" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting a timespan but instead got: "NOT A TimeSpan"
                        """.Trim())
                let json = "\"NOT A TimeSpan\""
                let actual =
                    Decode.fromString Decode.timespan json

                equal expected actual

            testCase "an enum<sbyte> works" <| fun _ ->
                let expected = Ok Enum_Int8.NinetyNine
                let actual =
                    Decode.fromString Decode.Enum.sbyte "99"

                equal expected actual

            testCase "an enum<byte> works" <| fun _ ->
                let expected = Ok Enum_UInt8.NinetyNine
                let actual =
                    Decode.fromString Decode.Enum.byte "99"

                equal expected actual

            testCase "an enum<int> works" <| fun _ ->
                let expected = Ok Enum_Int.One
                let actual =
                    Decode.fromString Decode.Enum.int "1"

                equal expected actual

            testCase "an enum<uint32> works" <| fun _ ->
                let expected = Ok Enum_UInt32.NinetyNine
                let actual =
                    Decode.fromString Decode.Enum.uint32 "99"

                equal expected actual

            testCase "an enum<int16> works" <| fun _ ->
                let expected = Ok Enum_Int16.NinetyNine
                let actual =
                    Decode.fromString Decode.Enum.int16 "99"

                equal expected actual

            testCase "an enum<uint16> works" <| fun _ ->
                let expected = Ok Enum_UInt16.NinetyNine
                let actual =
                    Decode.fromString Decode.Enum.uint16 "99"

                equal expected actual

        ]

        testList "Tuples" [
            testCase "tuple2 works" <| fun _ ->
                let json = """[1, "maxime"]"""
                let expected = Ok(1, "maxime")

                let actual =
                    Decode.fromString (Decode.tuple2 Decode.int Decode.string) json

                equal expected actual

            testCase "tuple3 works" <| fun _ ->
                let json = """[1, "maxime", 2.5]"""
                let expected = Ok(1, "maxime", 2.5)

                let actual =
                    Decode.fromString
                        (Decode.tuple3
                            Decode.int
                            Decode.string
                            Decode.float) json

                equal expected actual

            testCase "tuple4 works" <| fun _ ->
                let json = """[1, "maxime", 2.5, { "fieldA" : "test" }]"""
                let expected = Ok(1, "maxime", 2.5, { fieldA = "test" })

                let actual =
                    Decode.fromString
                        (Decode.tuple4
                            Decode.int
                            Decode.string
                            Decode.float
                            SmallRecord.Decoder) json

                equal expected actual

            testCase "tuple5 works" <| fun _ ->
                let json = """[1, "maxime", 2.5, { "fieldA" : "test" }, false]"""
                let expected = Ok(1, "maxime", 2.5, { fieldA = "test" }, false)

                let actual =
                    Decode.fromString
                        (Decode.tuple5
                            Decode.int
                            Decode.string
                            Decode.float
                            SmallRecord.Decoder
                            Decode.bool) json

                equal expected actual

            testCase "tuple6 works" <| fun _ ->
                let json = """[1, "maxime", 2.5, { "fieldA" : "test" }, false, null]"""
                let expected = Ok(1, "maxime", 2.5, { fieldA = "test" }, false, null)

                let actual =
                    Decode.fromString
                        (Decode.tuple6
                            Decode.int
                            Decode.string
                            Decode.float
                            SmallRecord.Decoder
                            Decode.bool
                            (Decode.nil null)) json

                equal expected actual

            testCase "tuple7 works" <| fun _ ->
                let json = """[1, "maxime", 2.5, { "fieldA" : "test" }, false, null, 56]"""
                let expected = Ok(1, "maxime", 2.5, { fieldA = "test" }, false, null, 56)

                let actual =
                    Decode.fromString
                        (Decode.tuple7
                            Decode.int
                            Decode.string
                            Decode.float
                            SmallRecord.Decoder
                            Decode.bool
                            (Decode.nil null)
                            Decode.int) json

                equal expected actual

            testCase "tuple8 works" <| fun _ ->
                let json = """[1, "maxime", 2.5, { "fieldA" : "test" }, false, null, true, 98]"""
                let expected = Ok(1, "maxime", 2.5, { fieldA = "test" }, false, null, true, 98)

                let actual =
                    Decode.fromString
                        (Decode.tuple8
                            Decode.int
                            Decode.string
                            Decode.float
                            SmallRecord.Decoder
                            Decode.bool
                            (Decode.nil null)
                            Decode.bool
                            Decode.int) json

                equal expected actual

            testCase "tuple2 returns an error if invalid json" <| fun _ ->
                let json = """[1, false, "unused value"]"""
                let expected =
                    Error(
                        """
Error at: `$.[1]`
Expecting a string but instead got: false
                        """.Trim())

                let actual =
                    Decode.fromString
                        (Decode.tuple2
                            Decode.int
                            Decode.string) json

                equal expected actual

            testCase "tuple3 returns an error if invalid json" <| fun _ ->
                let json = """[1, "maxime", false]"""
                let expected =
                    Error(
                        """
Error at: `$.[2]`
Expecting a float but instead got: false
                        """.Trim())

                let actual =
                    Decode.fromString
                        (Decode.tuple3
                            Decode.int
                            Decode.string
                            Decode.float) json

                equal expected actual

            testCase "tuple4 returns an error if invalid json (missing index)" <| fun _ ->
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
                    Decode.fromString
                        (Decode.tuple4
                            Decode.int
                            Decode.string
                            Decode.float
                            SmallRecord.Decoder) json

                equal expected actual

            testCase "tuple4 returns an error if invalid json (error in the nested object)" <| fun _ ->
                let json = """[1, "maxime", 2.5, { "fieldA" : false }]"""
                let expected =
                    Error(
                        """
Error at: `$.[3].fieldA`
Expecting a string but instead got: false
                        """.Trim())

                let actual =
                    Decode.fromString
                        (Decode.tuple4
                            Decode.int
                            Decode.string
                            Decode.float
                            SmallRecord.Decoder) json

                equal expected actual

            testCase "tuple5 returns an error if invalid json" <| fun _ ->
                let json = """[1, "maxime", 2.5, { "fieldA" : "test" }, false]"""
                let expected =
                    Error(
                        """
Error at: `$.[4]`
Expecting a datetime but instead got: false
                        """.Trim())

                let actual =
                    Decode.fromString
                        (Decode.tuple5
                            Decode.int
                            Decode.string
                            Decode.float
                            SmallRecord.Decoder
                            Decode.datetime) json

                equal expected actual

            testCase "tuple6 returns an error if invalid json" <| fun _ ->
                let json = """[1, "maxime", 2.5, { "fieldA" : "test" }, "2018-10-01T11:12:55.00Z", false]"""
                let expected =
                    Error(
                        """
Error at: `$.[5]`
Expecting null but instead got: false
                        """.Trim())

                let actual =
                    Decode.fromString
                        (Decode.tuple6
                            Decode.int
                            Decode.string
                            Decode.float
                            SmallRecord.Decoder
                            Decode.datetime
                            (Decode.nil null)) json

                equal expected actual

            testCase "tuple7 returns an error if invalid json" <| fun _ ->
                let json = """[1, "maxime", 2.5, { "fieldA" : "test" }, "2018-10-01T11:12:55.00Z", null, false]"""
                let expected =
                    Error(
                        """
Error at: `$.[6]`
Expecting an int but instead got: false
                        """.Trim())

                let actual =
                    Decode.fromString
                        (Decode.tuple7
                            Decode.int
                            Decode.string
                            Decode.float
                            SmallRecord.Decoder
                            Decode.datetime
                            (Decode.nil null)
                            Decode.int) json

                equal expected actual

            testCase "tuple8 returns an error if invalid json" <| fun _ ->
                let json = """[1, "maxime", 2.5, { "fieldA" : "test" }, "2018-10-01T11:12:55.00Z", null, 56, "maxime"]"""
                let expected =
                    Error(
                        """
Error at: `$.[7]`
Expecting an int but instead got: "maxime"
                        """.Trim())

                let actual =
                    Decode.fromString
                        (Decode.tuple8
                            Decode.int
                            Decode.string
                            Decode.float
                            SmallRecord.Decoder
                            Decode.datetime
                            (Decode.nil null)
                            Decode.int
                            Decode.int) json

                equal expected actual

        ]

        testList "Object primitives" [

            testCase "field works" <| fun _ ->
                let json = """{ "name": "maxime", "age": 25 }"""
                let expected = Ok("maxime")

                let actual =
                    Decode.fromString (Decode.field "name" Decode.string) json

                equal expected actual

            testCase "field output an error explaining why the value is considered invalid" <| fun _ ->
                let json = """{ "name": null, "age": 25 }"""
                let expected =
                    Error(
                        """
Error at: `$.name`
Expecting an int but instead got: null
                        """.Trim()
                    )

                let actual =
                    Decode.fromString (Decode.field "name" Decode.int) json

                equal expected actual

            testCase "field output an error when field is missing" <| fun _ ->
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
                    Decode.fromString (Decode.field "height" Decode.float) json

                equal expected actual

            testCase "at works" <| fun _ ->

                let json = """{ "user": { "name": "maxime", "age": 25 } }"""
                let expected = Ok "maxime"

                let actual =
                    Decode.fromString (Decode.at ["user"; "name"] Decode.string) json

                equal expected actual

            testCase "at output an error if the path failed" <| fun _ ->
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
                    Decode.fromString (Decode.at ["user"; "firstname"] Decode.string) json

                equal expected actual

            testCase "at output an error explaining why the value is considered invalid" <| fun _ ->
                let json = """{ "name": null, "age": 25 }"""
                let expected =
                    Error(
                        """
Error at: `$.name`
Expecting an int but instead got: null
                        """.Trim()
                    )

                let actual =
                    Decode.fromString (Decode.at [ "name" ] Decode.int) json

                equal expected actual

            testCase "index works" <| fun _ ->
                let json = """["maxime", "alfonso", "steffen"]"""
                let expected = Ok("alfonso")

                let actual =
                    Decode.fromString (Decode.index 1 Decode.string) json

                equal expected actual

            testCase "index output an error if array is to small" <| fun _ ->
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
                    Decode.fromString (Decode.index 5 Decode.string) json

                equal expected actual

            testCase "index output an error if value isn't an array" <| fun _ ->
                let json = "1"
                let expected =
                    Error(
                        """
Error at: `$.[5]`
Expecting an array but instead got: 1
                        """.Trim())

                let actual =
                    Decode.fromString (Decode.index 5 Decode.string) json

                equal expected actual

        ]


        testList "Data structure" [

            testCase "list works" <| fun _ ->
                let expected = Ok([1; 2; 3])

                let actual =
                    Decode.fromString (Decode.list Decode.int) "[1, 2, 3]"

                equal expected actual

            testCase "nested lists work" <| fun _ ->
                [ [ "maxime2" ] ]
                |> List.map (fun d ->
                    d
                    |> List.map Encode.string
                    |> Encode.list)
                |> Encode.list
                |> Encode.toString 4
                |> Decode.fromString (Decode.list (Decode.list Decode.string))
                |> function Ok v -> equal [["maxime2"]] v | Error er -> failwith er

            testCase "an invalid list output an error" <| fun _ ->
                let expected = Error("Error at: `$`\nExpecting a list but instead got: 1")

                let actual =
                    Decode.fromString (Decode.list Decode.int) "1"

                equal expected actual

            testCase "array works" <| fun _ ->
                // Need to pass by a list otherwise Fable use:
                // new Int32Array([1, 2, 3]) and the test fails
                // And this would give:
                // Expected: Result { tag: 0, data: Int32Array [ 1, 2, 3 ] }
                // Actual: Result { tag: 0, data: [ 1, 2, 3 ] }
                let expected = Ok([1; 2; 3] |> List.toArray)

                let actual =
                    Decode.fromString (Decode.array Decode.int) "[1, 2, 3]"

                equal expected actual

            testCase "an invalid array output an error" <| fun _ ->
                let expected = Error("Error at: `$`\nExpecting an array but instead got: 1")

                let actual =
                    Decode.fromString (Decode.array Decode.int) "1"

                equal expected actual

            testCase "keys works" <| fun _ ->
                let expected = Ok(["a"; "b"; "c"])

                let actual =
                    Decode.fromString Decode.keys """{ "a": 1, "b": 2, "c": 3 }"""

                equal expected actual

            testCase "keys returns an error for invalid objects" <| fun _ ->
                let expected = Error("Error at: `$`\nExpecting an object but instead got: 1")

                let actual =
                    Decode.fromString Decode.keys "1"

                equal expected actual

            testCase "keyValuePairs works" <| fun _ ->
                let expected = Ok([("a", 1) ; ("b", 2) ; ("c", 3)])

                let actual =
                    Decode.fromString (Decode.keyValuePairs Decode.int) """{ "a": 1, "b": 2, "c": 3 }"""

                equal expected actual

            testCase "dict works" <| fun _ ->
                let expected = Ok(Map.ofList([("a", 1) ; ("b", 2) ; ("c", 3)]))

                let actual =
                    Decode.fromString (Decode.dict Decode.int) """{ "a": 1, "b": 2, "c": 3 }"""

                equal expected actual

            testCase "dict with custom decoder works" <| fun _ ->
                let expected = Ok(Map.ofList([("a", Record2.Create 1. 1.) ; ("b", Record2.Create 2. 2.) ; ("c", Record2.Create 3. 3.)]))

                let decodePoint =
                    Decode.map2 Record2.Create
                        (Decode.field "a" Decode.float)
                        (Decode.field "b" Decode.float)

                let actual =
                    Decode.fromString (Decode.dict decodePoint)
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

                equal expected actual

            testCase "an invalid dict output an error" <| fun _ ->
                let expected = Error("Error at: `$`\nExpecting an object but instead got: 1")

                let actual =
                    Decode.fromString (Decode.dict Decode.int) "1"

                equal expected actual

        ]

        testList "Inconsistent structure" [

            testCase "oneOf works" <| fun _ ->
                let expected = Ok([1; 2; 0; 4])

                let badInt =
                    Decode.oneOf [ Decode.int; Decode.nil 0 ]

                let actual =
                    Decode.fromString (Decode.list badInt) "[1,2,null,4]"

                equal expected actual

            testCase "oneOf works in combination with object builders" <| fun _ ->
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
                    Decode.fromString decoder2 json

                equal expected actual

            testCase "oneOf works with optional" <| fun _ ->
                let decoder =
                    Decode.oneOf
                        [
                            Decode.field "Normal" Decode.float |> Decode.map Normal
                            Decode.field "Reduced" (Decode.option Decode.float) |> Decode.map Reduced
                            Decode.field "Zero" Decode.bool |> Decode.map (fun _ -> Zero)
                        ]

                """{"Normal": 4.5}""" |> Decode.fromString decoder |> equal (Ok(Normal 4.5))
                """{"Reduced": 4.5}""" |> Decode.fromString decoder |> equal (Ok(Reduced(Some 4.5)))
                """{"Reduced": null}""" |> Decode.fromString decoder |> equal (Ok(Reduced None))
                """{"Zero": true}""" |> Decode.fromString decoder |> equal (Ok Zero)

            testCase "oneOf output errors if all case fails" <| fun _ ->
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
                    Decode.fromString (Decode.list badInt) "[1,2,null,4]"

                equal expected actual

            testCase "optional works" <| fun _ ->
                let json = """{ "name": "maxime", "age": 25, "something_undefined": null }"""

                let expectedValid = Ok(Some "maxime")
                let actualValid =
                    Decode.fromString (Decode.optional "name" Decode.string) json

                equal expectedValid actualValid

                match Decode.fromString (Decode.optional "name" Decode.int) json with
                | Error _ -> ()
                | Ok _ -> failwith "Expected type error for `name` field"

                let expectedMissingField = Ok(None)
                let actualMissingField =
                    Decode.fromString (Decode.optional "height" Decode.int) json

                equal expectedMissingField actualMissingField

                let expectedUndefinedField = Ok(None)
                let actualUndefinedField =
                    Decode.fromString (Decode.optional "something_undefined" Decode.string) json

                equal expectedUndefinedField actualUndefinedField

            testCase "optional returns Error value if decoder fails" <| fun _ ->
                let json = """{ "name": 12, "age": 25 }"""
                let expected =
                    Error(
                        """
Error at: `$.name`
Expecting a string but instead got: 12
                        """.Trim())

                let actual =
                    Decode.fromString (Decode.optional "name" Decode.string) json

                equal expected actual

            testCase "optionalAt works" <| fun _ ->
                let json = """{ "data" : { "name": "maxime", "age": 25, "something_undefined": null } }"""

                let expectedValid = Ok(Some "maxime")
                let actualValid =
                    Decode.fromString (Decode.optionalAt [ "data"; "name" ] Decode.string) json

                equal expectedValid actualValid

                match Decode.fromString (Decode.optionalAt [ "data"; "name" ] Decode.int) json with
                | Error _ -> ()
                | Ok _ -> failwith "Expected type error for `name` field"

                let expectedMissingField = Ok None
                let actualMissingField =
                    Decode.fromString (Decode.optionalAt [ "data"; "height" ] Decode.int) json

                equal expectedMissingField actualMissingField

                let expectedUndefinedField = Ok(None)
                let actualUndefinedField =
                    Decode.fromString (Decode.optionalAt [ "data"; "something_undefined" ] Decode.string) json

                equal expectedUndefinedField actualUndefinedField

                let expectedUndefinedField = Ok(None)
                let actualUndefinedField =
                    Decode.fromString (Decode.optionalAt [ "data"; "something_undefined"; "name" ] Decode.string) json

                equal expectedUndefinedField actualUndefinedField

            testCase "combining field and option decoders works" <| fun _ ->
                let json = """{ "name": "maxime", "age": 25, "something_undefined": null }"""

                let expectedValid = Ok(Some "maxime")
                let actualValid =
                    Decode.fromString (Decode.field "name" (Decode.option Decode.string)) json

                equal expectedValid actualValid

                match Decode.fromString (Decode.field "name" (Decode.option Decode.int)) json with
                | Error msg ->
                    let expected =
                        """
Error at: `$.name`
Expecting an int but instead got: "maxime"
                        """.Trim()
                    equal expected msg
                | Ok _ -> failwith "Expected type error for `name` field #1"

                match Decode.fromString (Decode.field "this_field_do_not_exist" (Decode.option Decode.int)) json with
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
                    equal expected msg
                | Ok _ ->
                    failwith "Expected type error for `name` field #2"

                match Decode.fromString (Decode.field "something_undefined" (Decode.option Decode.int)) json with
                | Error _ -> failwith """`Decode.field "something_undefined" (Decode.option Decode.int)` test should pass"""
                | Ok result -> equal None result

                // Same tests as before but we are calling `option` then `field`

                let expectedValid2 = Ok(Some "maxime")
                let actualValid2 =
                    Decode.fromString (Decode.option (Decode.field "name" Decode.string)) json

                equal expectedValid2 actualValid2

                match Decode.fromString (Decode.option (Decode.field "name" Decode.int)) json with
                | Error msg ->
                    let expected =
                        """
Error at: `$.name`
Expecting an int but instead got: "maxime"
                        """.Trim()
                    equal expected msg
                | Ok _ -> failwith "Expected type error for `name` field #3"

                match Decode.fromString (Decode.option (Decode.field "this_field_do_not_exist" Decode.int)) json with
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
                    equal expected msg
                | Ok _ -> failwith "Expected type error for `name` field #4"

                match Decode.fromString (Decode.option (Decode.field "something_undefined" Decode.int)) json with
                | Error msg ->
                    let expected =
                        """
Error at: `$.something_undefined`
Expecting an int but instead got: null
                        """.Trim()
                    equal expected msg
                | Ok _ -> failwith "Expected type error for `name` field"

                /// Alfonso: Should this test pass? We should use Decode.optional instead
                /// - `Decode.fromString (Decode.field "height" (Decode.option Decode.int)) json` == `Ok(None)`
                ///
                /// Maxime here :)
                /// I don't think this test should pass.
                /// For me `Decode.field "height" (Decode.option Decode.int)` means:
                /// 1. The field `height` is required
                /// 2. If `height` exist then, it's value can be `Some X` where `X` is an `int` or `None`
                ///
                /// I am keep the comments here so we keep track of the explanation if we later need to give it a second though.
                ///
                match Decode.fromString (Decode.field "height" (Decode.option Decode.int)) json with
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

                    equal expected msg

                | Ok _ -> failwith "Expected type error for `height` field"

                let expectedUndefinedField = Ok(None)
                let actualUndefinedField =
                    Decode.fromString (Decode.field "something_undefined" (Decode.option Decode.string)) json

                equal expectedUndefinedField actualUndefinedField

        ]

        testList "Fancy decoding" [

            testCase "null works (test on an int)" <| fun _ ->
                let expected = Ok(20)
                let actual =
                    Decode.fromString (Decode.nil 20) "null"

                equal expected actual

            testCase "null works (test on a boolean)" <| fun _ ->
                let expected = Ok(false)
                let actual =
                    Decode.fromString (Decode.nil false) "null"

                equal expected actual

            testCase "succeed works" <| fun _ ->
                let expected = Ok(7)
                let actual =
                    Decode.fromString (Decode.succeed 7) "true"

                equal expected actual

            testCase "succeed output an error if the JSON is invalid" <| fun _ ->
                #if FABLE_COMPILER
                let expected = Error("Given an invalid JSON: Unexpected token m in JSON at position 0")
                #else
                let expected = Error("Given an invalid JSON: Unexpected character encountered while parsing value: m. Path '', line 0, position 0.")
                #endif
                let actual =
                    Decode.fromString (Decode.succeed 7) "maxime"

                equal expected actual

            testCase "fail works" <| fun _ ->
                let msg = "Failing because it's fun"
                let expected = Error("Error at: `$`\nThe following `failure` occurred with the decoder: " + msg)
                let actual =
                    Decode.fromString (Decode.fail msg) "true"

                equal expected actual

            testCase "andThen works" <| fun _ ->
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
                    Decode.fromString info """{ "version": 3, "data": 2 }"""

                equal expected actual


            testCase "andThen generate an error if an error occuered" <| fun _ ->
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
                    Decode.fromString info """{ "info": 3, "data": 2 }"""

                equal expected actual


            testCase "all works" <| fun _ ->
                let expected = Ok [1; 2; 3]

                let decodeAll = Decode.all [
                    Decode.succeed 1
                    Decode.succeed 2
                    Decode.succeed 3
                ]

                let actual = Decode.fromString decodeAll "{}"

                equal expected actual

            testCase "combining Decode.all and Decode.keys works" <| fun _ ->
                let expected = Ok [1; 2; 3]

                let decoder =
                    Decode.keys
                    |> Decode.andThen (fun keys ->
                        keys
                        |> List.except ["special_property"]
                        |> List.map (fun key -> Decode.field key Decode.int)
                        |> Decode.all)

                let actual = Decode.fromString decoder """{ "a": 1, "b": 2, "c": 3 }"""

                equal expected actual

            testCase "all succeeds on empty lists" <| fun _ ->
                let expected = Ok []

                let decodeNone = Decode.all []

                let actual = Decode.fromString decodeNone "{}"

                equal expected actual


            testCase "all fails when one decoder fails" <| fun _ ->
                let expected = Error("Error at: `$`\nExpecting an int but instead got: {}")

                let decodeAll = Decode.all [
                    Decode.succeed 1
                    Decode.int
                    Decode.succeed 3
                ]

                let actual = Decode.fromString decodeAll "{}"

                equal expected actual
        ]

        testList "Mapping" [

            testCase "map works" <| fun _ ->
                let expected = Ok(6)
                let stringLength =
                    Decode.map String.length Decode.string

                let actual =
                    Decode.fromString stringLength "\"maxime\""
                equal expected actual


            testCase "map2 works" <| fun _ ->
                let expected = Ok({a = 1.; b = 2.} : Record2)

                let decodePoint =
                    Decode.map2 Record2.Create
                        (Decode.field "a" Decode.float)
                        (Decode.field "b" Decode.float)

                let actual =
                    Decode.fromString decodePoint jsonRecord

                equal expected actual

            testCase "map3 works" <| fun _ ->
                let expected = Ok({ a = 1.
                                    b = 2.
                                    c = 3. } : Record3)

                let decodePoint =
                    Decode.map3 Record3.Create
                        (Decode.field "a" Decode.float)
                        (Decode.field "b" Decode.float)
                        (Decode.field "c" Decode.float)

                let actual =
                    Decode.fromString decodePoint jsonRecord

                equal expected actual

            testCase "map4 works" <| fun _ ->
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
                    Decode.fromString decodePoint jsonRecord

                equal expected actual

            testCase "map5 works" <| fun _ ->
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
                    Decode.fromString decodePoint jsonRecord

                equal expected actual

            testCase "map6 works" <| fun _ ->
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
                    Decode.fromString decodePoint jsonRecord

                equal expected actual

            testCase "map7 works" <| fun _ ->
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
                    Decode.fromString decodePoint jsonRecord

                equal expected actual

            testCase "map8 works" <| fun _ ->
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
                    Decode.fromString decodePoint jsonRecord

                equal expected actual

            testCase "map2 generate an error if invalid" <| fun _ ->
                let expected = Error("Error at: `$.a`\nExpecting a float but instead got: \"invalid_a_field\"")

                let decodePoint =
                    Decode.map2 Record2.Create
                        (Decode.field "a" Decode.float)
                        (Decode.field "b" Decode.float)

                let actual =
                    Decode.fromString decodePoint jsonRecordInvalid

                equal expected actual

        ]

        testList "object builder" [

            testCase "get.Required.Field works" <| fun _ ->
                let json = """{ "name": "maxime", "age": 25 }"""
                let expected = Ok({ fieldA = "maxime" })

                let decoder =
                    Decode.object
                        (fun get ->
                            { fieldA = get.Required.Field "name" Decode.string }
                        )

                let actual =
                    Decode.fromString decoder json

                equal expected actual

            testCase "get.Required.Field returns Error if field is missing" <| fun _ ->
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
                    Decode.fromString decoder json

                equal expected actual

            testCase "get.Required.Field returns Error if type is incorrect" <| fun _ ->
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
                    Decode.fromString decoder json

                equal expected actual

            testCase "get.Optional.Field works" <| fun _ ->
                let json = """{ "name": "maxime", "age": 25 }"""
                let expected = Ok({ optionalField = Some "maxime" })

                let decoder =
                    Decode.object
                        (fun get ->
                            { optionalField = get.Optional.Field "name" Decode.string }
                        )

                let actual =
                    Decode.fromString decoder json

                equal expected actual

            testCase "get.Optional.Field returns None value if field is missing" <| fun _ ->
                let json = """{ "age": 25 }"""
                let expected = Ok({ optionalField = None })

                let decoder =
                    Decode.object
                        (fun get ->
                            { optionalField = get.Optional.Field "name" Decode.string }
                        )

                let actual =
                    Decode.fromString decoder json

                equal expected actual

            testCase "get.Optional.Field returns None if field is null" <| fun _ ->
                let json = """{ "name": null, "age": 25 }"""
                let expected = Ok({ optionalField = None })

                let decoder =
                    Decode.object
                        (fun get ->
                            { optionalField = get.Optional.Field "name" Decode.string }
                        )

                let actual =
                    Decode.fromString decoder json

                equal expected actual

            testCase "get.Optional.Field returns Error value if decoder fails" <| fun _ ->
                let json = """{ "name": 12, "age": 25 }"""
                let expected =
                    Error(
                        """
Error at: `$.name`
Expecting a string but instead got: 12
                        """.Trim())

                let decoder = Decode.object (fun get ->
                    { optionalField = get.Optional.Field "name" Decode.string })

                let actual = Decode.fromString decoder json

                equal expected actual

            testCase "nested get.Optional.Field > get.Required.Field returns None if field is null" <| fun _ ->
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
                    Decode.fromString decoder json

                equal expected actual

            testCase "get.Optional.Field returns Error if type is incorrect" <| fun _ ->
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
                    Decode.fromString decoder json

                equal expected actual


            testCase "get.Required.At works" <| fun _ ->

                let json = """{ "user": { "name": "maxime", "age": 25 } }"""
                let expected = Ok({ fieldA = "maxime" })

                let decoder =
                    Decode.object
                        (fun get ->
                            { fieldA = get.Required.At [ "user"; "name" ] Decode.string }
                        )

                let actual =
                    Decode.fromString decoder json

                equal expected actual

            testCase "get.Required.At returns Error if non-object in path" <| fun _ ->
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
                    Decode.fromString decoder json

                equal expected actual

            testCase "get.Required.At returns Error if field missing" <| fun _ ->
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
                    Decode.fromString decoder json

                equal expected actual

            testCase "get.Required.At returns Error if type is incorrect" <| fun _ ->
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
                    Decode.fromString decoder json

                equal expected actual

            testCase "get.Optional.At works" <| fun _ ->

                let json = """{ "user": { "name": "maxime", "age": 25 } }"""
                let expected = Ok({ optionalField = Some "maxime" })

                let decoder =
                    Decode.object
                        (fun get ->
                            { optionalField = get.Optional.At [ "user"; "name" ] Decode.string }
                        )

                let actual =
                    Decode.fromString decoder json

                equal expected actual

            testCase "get.Optional.At returns 'type error' if non-object in path" <| fun _ ->
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
                    Decode.fromString decoder json

                equal expected actual

            testCase "get.Optional.At returns None if field missing" <| fun _ ->
                let json = """{ "user": { "name": "maxime", "age": 25 } }"""
                let expected = Ok({ optionalField = None })

                let decoder =
                    Decode.object
                        (fun get ->
                            { optionalField = get.Optional.At [ "user"; "firstname" ] Decode.string }
                        )

                let actual =
                    Decode.fromString decoder json

                equal expected actual

            testCase "get.Optional.At returns Error if type is incorrect" <| fun _ ->
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
                    Decode.fromString decoder json

                equal expected actual

            testCase "complex object builder works" <| fun _ ->
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
                    Decode.fromString
                        userDecoder
                        """{ "id": 67, "email": "user@mail.com" }"""

                equal expected actual

            testCase "get.Field.Raw works" <| fun _ ->
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
                    Decode.fromString
                        decoder
                        json

                let expected =
                    Ok ({ Enabled = true
                          Shape = Circle 20 } : MyObj)

                equal expected actual

            testCase "get.Field.Raw returns Error if a decoder fail" <| fun _ ->
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
                    Decode.fromString
                        decoder
                        json

                let expected =
                    Error "Error at: `$`\nThe following `failure` occurred with the decoder: Unknown shape type custom_shape"

                equal expected actual

            testCase "get.Field.Raw returns Error if a field is missing in the 'raw decoder'" <| fun _ ->
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
                    Decode.fromString
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

                equal expected actual

            testCase "get.Optional.Raw works" <| fun _ ->
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
                    Decode.fromString
                        decoder
                        json

                let expected =
                    Ok { Enabled = true
                         Shape = Some (Circle 20) }

                equal expected actual

            testCase "get.Optional.Raw returns None if a field is missing" <| fun _ ->
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
                    Decode.fromString
                        decoder
                        json

                let expected =
                    Ok { Enabled = true
                         Shape = None }

                equal expected actual

            testCase "get.Optional.Raw returns an Error if a decoder fail" <| fun _ ->
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
                    Decode.fromString
                        decoder
                        json

                let expected =
                    Error "Error at: `$`\nThe following `failure` occurred with the decoder: Unknown shape type invalid_shape"

                equal expected actual

            testCase "get.Optional.Raw returns an Error if the type is invalid" <| fun _ ->
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
                    Decode.fromString
                        decoder
                        json

                let expected =
                    Error "Error at: `$.radius`\nExpecting an int but instead got: \"maxime\""

                equal expected actual

            testCase "get.Optional.Raw returns None if a decoder fails with null" <| fun _ ->
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
                    Decode.fromString
                        decoder
                        json

                let expected =
                     Ok { Enabled = true
                          Shape = None }

                equal expected actual

            testCase "Object builders returns all the Errors" <| fun _ ->
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
                    Decode.fromString decoder json

                equal expected actual

        ]

        testList "Auto" [
            testCase "Auto.Decode.fromString works" <| fun _ ->
                let now = DateTime.Now
                let value : Record9 =
                    {
                        a = 5
                        b = "bar"
                        c = [false, 3; true, 5; false, 10]
                        d = [|Some(Foo 14); None|]
                        e = Map [("oh", { a = 2.; b = 2. }); ("ah", { a = -1.5; b = 0. })]
                        f = now
                        g = set [{ a = 2.; b = 2. }; { a = -1.5; b = 0. }]
                        h = TimeSpan.FromSeconds(5.)
                        i = 120y
                        j = 120uy
                        k = 250s
                        l = 250us
                        m = 99u
                        n = 99L
                        o = 999UL
                        p = ()
                        // r = seq [ "item n°1"; "item n°2"]
                    }
                let extra =
                    Extra.empty
                    |> Extra.withInt64
                    |> Extra.withUInt64
                let json = Encode.Auto.toString(4, value, extra = extra)
                // printfn "AUTO ENCODED %s" json
                let r2 = Decode.Auto.unsafeFromString<Record9>(json, extra = extra)
                equal 5 r2.a
                equal "bar" r2.b
                equal [false, 3; true, 5; false, 10] r2.c
                equal (Some(Foo 14)) r2.d.[0]
                equal None r2.d.[1]
                equal -1.5 (Map.find "ah" r2.e).a
                equal 2.   (Map.find "oh" r2.e).b
                equal (now.ToString())  (value.f.ToString())
                equal true (Set.contains { a = -1.5; b = 0. } r2.g)
                equal false (Set.contains { a = 1.5; b = 0. } r2.g)
                equal 5000. value.h.TotalMilliseconds
                equal 120y r2.i
                equal 120uy r2.j
                equal 250s r2.k
                equal 250us r2.l
                equal 99u r2.m
                equal 99L r2.n
                equal 999UL r2.o
                equal () r2.p
                // equal ((seq [ "item n°1"; "item n°2"]) |> Seq.toList) (r2.r |> Seq.toList)

            testCase "Auto serialization works with recursive types" <| fun _ ->
                let len xs =
                    let rec lenInner acc = function
                        | Cons(_,rest) -> lenInner (acc + 1) rest
                        | Nil -> acc
                    lenInner 0 xs
                let li = Cons(1, Cons(2, Cons(3, Nil)))
                let json = Encode.Auto.toString(4, li)
                // printfn "AUTO ENCODED MYLIST %s" json
                let li2 = Decode.Auto.unsafeFromString<MyList<int>>(json)
                len li2 |> equal 3
                match li with
                | Cons(i1, Cons(i2, Cons(i3, Nil))) -> i1 + i2 + i3
                | Cons(i,_) -> i
                | Nil -> 0
                |> equal 6

            testCase "Auto decoders works for string" <| fun _ ->
                let value = "maxime"
                let json = Encode.Auto.toString(4, value)
                let res = Decode.Auto.unsafeFromString<string>(json)
                equal value res

            testCase "Auto decoders works for guid" <| fun _ ->
                let value = Guid.NewGuid()
                let json = Encode.Auto.toString(4, value)
                let res = Decode.Auto.unsafeFromString<Guid>(json)
                equal value res

            testCase "Auto decoders works for int" <| fun _ ->
                let value = 12
                let json = Encode.Auto.toString(4, value)
                let res = Decode.Auto.unsafeFromString<int>(json)
                equal value res

            testCase "Auto decoders works for int64" <| fun _ ->
                let extra = Extra.empty |> Extra.withInt64
                let value = 9999999999L
                let json = Encode.Auto.toString(4, value, extra=extra)
                let res = Decode.Auto.unsafeFromString<int64>(json, extra=extra)
                equal value res

            testCase "Auto decoders works for uint32" <| fun _ ->
                let value = 12u
                let json = Encode.Auto.toString(4, value)
                let res = Decode.Auto.unsafeFromString<uint32>(json)
                equal value res

            testCase "Auto decoders works for uint64" <| fun _ ->
                let extra = Extra.empty |> Extra.withUInt64
                let value = 9999999999999999999UL
                let json = Encode.Auto.toString(4, value, extra=extra)
                let res = Decode.Auto.unsafeFromString<uint64>(json, extra=extra)
                equal value res

            testCase "Auto decoders works for bigint" <| fun _ ->
                let extra = Extra.empty |> Extra.withBigInt
                let value = 99999999999999999999999I
                let json = Encode.Auto.toString(4, value, extra=extra)
                let res = Decode.Auto.unsafeFromString<bigint>(json, extra=extra)
                equal value res

            testCase "Auto decoders works for bool" <| fun _ ->
                let value = false
                let json = Encode.Auto.toString(4, value)
                let res = Decode.Auto.unsafeFromString<bool>(json)
                equal value res

            testCase "Auto decoders works for float" <| fun _ ->
                let value = 12.
                let json = Encode.Auto.toString(4, value)
                let res = Decode.Auto.unsafeFromString<float>(json)
                equal value res

            testCase "Auto decoders works for decimal" <| fun _ ->
                let extra = Extra.empty |> Extra.withDecimal
                let value = 0.7833M
                let json = Encode.Auto.toString(4, value, extra=extra)
                let res = Decode.Auto.unsafeFromString<decimal>(json, extra=extra)
                equal value res

            // testCase "Auto decoders works for datetime" <| fun _ ->
            //     let value = DateTime.Now
            //     let json = Encode.Auto.toString(4, value)
            //     let res = Decode.Auto.unsafeFromString<DateTime>(json)
            //     equal value.Date res.Date
            //     equal value.Hour res.Hour
            //     equal value.Minute res.Minute
            //     equal value.Second res.Second

            testCase "Auto decoders works for datetime UTC" <| fun _ ->
                let value = DateTime.UtcNow
                let json = Encode.Auto.toString(4, value)
                let res = Decode.Auto.unsafeFromString<DateTime>(json)
                equal value.Date res.Date
                equal value.Hour res.Hour
                equal value.Minute res.Minute
                equal value.Second res.Second

            testCase "Auto decoders works for datetimeOffset" <| fun _ ->
                let value = DateTimeOffset.Now
                let json = Encode.Auto.toString(4, value)
                let res = Decode.Auto.unsafeFromString<DateTimeOffset>(json).ToLocalTime()
                equal value.Date res.Date
                equal value.Hour res.Hour
                equal value.Minute res.Minute
                equal value.Second res.Second

            testCase "Auto decoders works for datetimeOffset UTC" <| fun _ ->
                let value = DateTimeOffset.UtcNow
                let json = Encode.Auto.toString(4, value)
                let res = Decode.Auto.unsafeFromString<DateTimeOffset>(json).ToUniversalTime()
                // printfn "SOURCE %A JSON %s OUTPUT %A" value json res
                equal value.Date res.Date
                equal value.Hour res.Hour
                equal value.Minute res.Minute
                equal value.Second res.Second

            testCase "Auto decoders works for TimeSpan" <| fun _ ->
                let value = TimeSpan(1,2,3,4,5)
                let json = Encode.Auto.toString(4, value)
                let res = Decode.Auto.unsafeFromString<TimeSpan>(json)
                equal value.Days res.Days
                equal value.Hours res.Hours
                equal value.Minutes res.Minutes
                equal value.Seconds res.Seconds
                equal value.Milliseconds res.Milliseconds

            testCase "Auto decoders works for list" <| fun _ ->
                let value = [1; 2; 3; 4]
                let json = Encode.Auto.toString(4, value)
                let res = Decode.Auto.unsafeFromString<int list>(json)
                equal value res

            testCase "Auto decoders works for array" <| fun _ ->
                let value = [| 1; 2; 3; 4 |]
                let json = Encode.Auto.toString(4, value)
                let res = Decode.Auto.unsafeFromString<int array>(json)
                equal value res

            testCase "Auto decoders works for option None" <| fun _ ->
                let value = None
                let json = Encode.Auto.toString(4, value)
                let res = Decode.Auto.unsafeFromString<int option>(json)
                equal value res

            testCase "Auto decoders works for option Some" <| fun _ ->
                let value = Some 5
                let json = Encode.Auto.toString(4, value)
                let res = Decode.Auto.unsafeFromString<int option>(json)
                equal value res

            testCase "Auto decoders works for Unit" <| fun _ ->
                let value = ()
                let json = Encode.Auto.toString(4, value)
                let res = Decode.Auto.unsafeFromString(json)
                equal value res

            testCase "Auto decoders works for enum<int8>" <| fun _ ->
                let res = Decode.Auto.unsafeFromString<Enum_Int8>("99")
                equal Enum_Int8.NinetyNine res

            testCase "Auto decoders for enum<int8> returns an error if the Enum value is invalid" <| fun _ ->
#if FABLE_COMPILER
                let value =
                    Error(
                        """
Error at: `$`
Expecting Tests.Types.Enum_Int8[System.SByte] but instead got: 2
Reason: Unkown value provided for the enum
                        """.Trim())
#else
                let value =
                    Error(
                        """
Error at: `$`
Expecting Tests.Types+Enum_Int8 but instead got: 2
Reason: Unkown value provided for the enum
                        """.Trim())
#endif

                let res = Decode.Auto.fromString<Enum_Int8>("2")
                equal value res

            testCase "Auto decoders works for enum<uint8>" <| fun _ ->
                let res = Decode.Auto.unsafeFromString<Enum_UInt8>("99")
                equal Enum_UInt8.NinetyNine res

            testCase "Auto decoders for enum<uint8> returns an error if the Enum value is invalid" <| fun _ ->
#if FABLE_COMPILER
                let value =
                    Error(
                        """
Error at: `$`
Expecting Tests.Types.Enum_UInt8[System.Byte] but instead got: 2
Reason: Unkown value provided for the enum
                        """.Trim())
#else
                let value =
                    Error(
                        """
Error at: `$`
Expecting Tests.Types+Enum_UInt8 but instead got: 2
Reason: Unkown value provided for the enum
                        """.Trim())
#endif

                let res = Decode.Auto.fromString<Enum_UInt8>("2")
                equal value res

            testCase "Auto decoders works for enum<int16>" <| fun _ ->
                let res = Decode.Auto.unsafeFromString<Enum_Int16>("99")
                equal Enum_Int16.NinetyNine res

            testCase "Auto decoders for enum<int16> returns an error if the Enum value is invalid" <| fun _ ->
#if FABLE_COMPILER
                let value =
                    Error(
                        """
Error at: `$`
Expecting Tests.Types.Enum_Int16[System.Int16] but instead got: 2
Reason: Unkown value provided for the enum
                        """.Trim())
#else
                let value =
                    Error(
                        """
Error at: `$`
Expecting Tests.Types+Enum_Int16 but instead got: 2
Reason: Unkown value provided for the enum
                        """.Trim())
#endif

                let res = Decode.Auto.fromString<Enum_Int16>("2")
                equal value res

            testCase "Auto decoders works for enum<uint16>" <| fun _ ->
                let res = Decode.Auto.unsafeFromString<Enum_UInt16>("99")
                equal Enum_UInt16.NinetyNine res

            testCase "Auto decoders for enum<ºint16> returns an error if the Enum value is invalid" <| fun _ ->
#if FABLE_COMPILER
                let value =
                    Error(
                        """
Error at: `$`
Expecting Tests.Types.Enum_UInt16[System.UInt16] but instead got: 2
Reason: Unkown value provided for the enum
                        """.Trim())
#else
                let value =
                    Error(
                        """
Error at: `$`
Expecting Tests.Types+Enum_UInt16 but instead got: 2
Reason: Unkown value provided for the enum
                        """.Trim())
#endif

                let res = Decode.Auto.fromString<Enum_UInt16>("2")
                equal value res

            testCase "Auto decoders works for enum<int>" <| fun _ ->
                let res = Decode.Auto.unsafeFromString<Enum_Int>("1")
                equal Enum_Int.One res

            testCase "Auto decoders for enum<int> returns an error if the Enum value is invalid" <| fun _ ->
#if FABLE_COMPILER
                let value =
                    Error(
                        """
Error at: `$`
Expecting Tests.Types.Enum_Int[System.Int32] but instead got: 4
Reason: Unkown value provided for the enum
                        """.Trim())
#else
                let value =
                    Error(
                        """
Error at: `$`
Expecting Tests.Types+Enum_Int but instead got: 4
Reason: Unkown value provided for the enum
                        """.Trim())
#endif

                let res = Decode.Auto.fromString<Enum_Int>("4")
                equal value res

            testCase "Auto decoders works for enum<uint32>" <| fun _ ->
                let res = Decode.Auto.unsafeFromString<Enum_UInt32>("99")
                equal Enum_UInt32.NinetyNine res

            testCase "Auto decoders for enum<uint32> returns an error if the Enum value is invalid" <| fun _ ->
#if FABLE_COMPILER
                let value =
                    Error(
                        """
Error at: `$`
Expecting Tests.Types.Enum_UInt32[System.UInt32] but instead got: 2
Reason: Unkown value provided for the enum
                        """.Trim())
#else
                let value =
                    Error(
                        """
Error at: `$`
Expecting Tests.Types+Enum_UInt32 but instead got: 2
Reason: Unkown value provided for the enum
                        """.Trim())
#endif

                let res = Decode.Auto.fromString<Enum_UInt32>("2")
                equal value res

    (*
            #if NETFRAMEWORK
            testCase "Auto decoders  works with char based Enums" <| fun _ ->
                let value = CharEnum.A
                let json = Encode.Auto.toString(4, value)
                let res = Decode.Auto.unsafeFromString<CharEnum>(json)
                equal value res
            #endif
    *)
            testCase "Auto decoders works for null" <| fun _ ->
                let value = null
                let json = Encode.Auto.toString(4, value)
                let res = Decode.Auto.unsafeFromString<obj>(json)
                equal value res

            testCase "Auto decoders works even if type is determined by the compiler" <| fun _ ->
                let value = [1; 2; 3; 4]
                let json = Encode.Auto.toString(4, value)
                let res = Decode.Auto.unsafeFromString<_>(json)
                equal value res

            testCase "Auto.unsafeFromString works with camelCase" <| fun _ ->
                let json = """{ "id" : 0, "name": "maxime", "email": "mail@domain.com", "followers": 0 }"""
                let user = Decode.Auto.unsafeFromString<User>(json, caseStrategy=CamelCase)
                equal "maxime" user.Name
                equal 0 user.Id
                equal 0 user.Followers
                equal "mail@domain.com" user.Email

            testCase "Auto.fromString works with snake_case" <| fun _ ->
                let json = """{ "one" : 1, "two_part": 2, "three_part_field": 3 }"""
                let decoded = Decode.Auto.fromString<RecordForCharacterCase>(json, caseStrategy=SnakeCase)
                let expected = Ok { One = 1; TwoPart = 2; ThreePartField = 3 }
                equal expected decoded

            testCase "Auto.fromString works with camelCase" <| fun _ ->
                let json = """{ "id" : 0, "name": "maxime", "email": "mail@domain.com", "followers": 0 }"""
                let user = Decode.Auto.fromString<User>(json, caseStrategy=CamelCase)
                let expected = Ok { Id = 0; Name = "maxime"; Email = "mail@domain.com"; Followers = 0 }
                equal expected user

            testCase "Auto.fromString works for records with an actual value for the optional field value" <| fun _ ->
                let json = """{ "maybe" : "maybe value", "must": "must value"}"""
                let actual = Decode.Auto.fromString<TestMaybeRecord>(json, caseStrategy=CamelCase)
                let expected =
                    Ok ({ Maybe = Some "maybe value"
                          Must = "must value" } : TestMaybeRecord)
                equal expected actual

            testCase "Auto.fromString works for records with `null` for the optional field value" <| fun _ ->
                let json = """{ "maybe" : null, "must": "must value"}"""
                let actual = Decode.Auto.fromString<TestMaybeRecord>(json, caseStrategy=CamelCase)
                let expected =
                    Ok ({ Maybe = None
                          Must = "must value" } : TestMaybeRecord)
                equal expected actual

            testCase "Auto.fromString works for records with `null` for the optional field value on classes" <| fun _ ->
                let json = """{ "maybeClass" : null, "must": "must value"}"""
                let actual = Decode.Auto.fromString<RecordWithOptionalClass>(json, caseStrategy=CamelCase)
                let expected =
                    Ok ({ MaybeClass = None
                          Must = "must value" } : RecordWithOptionalClass)
                equal expected actual

            testCase "Auto.fromString works for records missing optional field value on classes" <| fun _ ->
                let json = """{ "must": "must value"}"""
                let actual = Decode.Auto.fromString<RecordWithOptionalClass>(json, caseStrategy=CamelCase)
                let expected =
                    Ok ({ MaybeClass = None
                          Must = "must value" } : RecordWithOptionalClass)
                equal expected actual

            testCase "Auto.generateDecoder throws for field using a non optional class" <| fun _ ->
                let expected = "Cannot generate auto decoder for Tests.Types.BaseClass. Please pass an extra decoder."
                let errorMsg =
                    try
                        let decoder = Decode.Auto.generateDecoder<RecordWithRequiredClass>(caseStrategy=CamelCase)
                        ""
                    with ex ->
                        ex.Message
                errorMsg.Replace("+", ".") |> equal expected

            testCase "Auto.fromString works for Class marked as optional" <| fun _ ->
                let json = """null"""

                let actual = Decode.Auto.fromString<BaseClass option>(json, caseStrategy=CamelCase)
                let expected = Ok None
                equal expected actual

            testCase "Auto.generateDecoder throws for Class" <| fun _ ->
                let expected = "Cannot generate auto decoder for Tests.Types.BaseClass. Please pass an extra decoder."
                let errorMsg =
                    try
                        let decoder = Decode.Auto.generateDecoder<BaseClass>(caseStrategy=CamelCase)
                        ""
                    with ex ->
                        ex.Message
                errorMsg.Replace("+", ".") |> equal expected

            testCase "Auto.fromString works for records missing an optional field" <| fun _ ->
                let json = """{ "must": "must value"}"""
                let actual = Decode.Auto.fromString<TestMaybeRecord>(json, caseStrategy=CamelCase)
                let expected =
                    Ok ({ Maybe = None
                          Must = "must value" } : TestMaybeRecord)
                equal expected actual

            testCase "Auto.fromString works with maps encoded as objects" <| fun _ ->
                let expected = Map [("oh", { a = 2.; b = 2. }); ("ah", { a = -1.5; b = 0. })]
                let json = """{"ah":{"a":-1.5,"b":0},"oh":{"a":2,"b":2}}"""
                let actual = Decode.Auto.fromString json
                equal (Ok expected) actual

            testCase "Auto.fromString works with maps encoded as arrays" <| fun _ ->
                let expected = Map [({ a = 2.; b = 2. }, "oh"); ({ a = -1.5; b = 0. }, "ah")]
                let json = """[[{"a":-1.5,"b":0},"ah"],[{"a":2,"b":2},"oh"]]"""
                let actual = Decode.Auto.fromString json
                equal (Ok expected) actual

            testCase "Decoder.Auto.toString works with bigint extra" <| fun _ ->
                let extra = Extra.empty |> Extra.withBigInt
                let expected = { bigintField = 9999999999999999999999I }
                let actual = Decode.Auto.fromString("""{"bigintField":"9999999999999999999999"}""", extra=extra)
                equal (Ok expected) actual

            testCase "Decoder.Auto.toString works with custom extra" <| fun _ ->
                let extra = Extra.empty |> Extra.withCustom ChildType.Encode ChildType.Decoder
                let expected = { ParentField = { ChildField = "bumbabon" } }
                let actual = Decode.Auto.fromString("""{"ParentField":"bumbabon"}""", extra=extra)
                equal (Ok expected) actual

            testCase "Auto.fromString works with records with private constructors" <| fun _ ->
                let json = """{ "foo1": 5, "foo2": 7.8 }"""
                Decode.Auto.fromString(json, caseStrategy=CamelCase)
                |> equal (Ok ({ Foo1 = 5; Foo2 = 7.8 }: RecordWithPrivateConstructor))

            testCase "Auto.fromString works with unions with private constructors" <| fun _ ->
                let json = """[ "Baz", ["Bar", "foo"]]"""
                Decode.Auto.fromString<UnionWithPrivateConstructor list>(json, caseStrategy=CamelCase)
                |> equal (Ok [Baz; Bar "foo"])

            testCase "Auto.fromString works gives proper error for wrong union fields" <| fun _ ->
                let json = """["Multi", "bar", "foo", "zas"]"""
                Decode.Auto.fromString<UnionWithMultipleFields>(json, caseStrategy=CamelCase)
                |> equal (Error "Error at: `$[2]`\nExpecting an int but instead got: \"foo\"")

            // TODO: Should we allow shorter arrays when last fields are options?
            testCase "Auto.fromString works gives proper error for wrong array length" <| fun _ ->
                let json = """["Multi", "bar", 1]"""
                Decode.Auto.fromString<UnionWithMultipleFields>(json, caseStrategy=CamelCase)
                |> equal (Error "Error at: `$`\nThe following `failure` occurred with the decoder: Expected array of length 4 but got 3")

            testCase "Auto.fromString works gives proper error for wrong case name" <| fun _ ->
                let json = """[1]"""
                Decode.Auto.fromString<UnionWithMultipleFields>(json, caseStrategy=CamelCase)
                |> equal (Error "Error at: `$[0]`\nExpecting a string but instead got: 1")

            testCase "Auto.generateDecoderCached works" <| fun _ ->
                let expected = Ok { Id = 0; Name = "maxime"; Email = "mail@domain.com"; Followers = 0 }
                let json = """{ "id" : 0, "name": "maxime", "email": "mail@domain.com", "followers": 0 }"""
                let decoder1 = Decode.Auto.generateDecoderCached<User>(caseStrategy=CamelCase)
                let decoder2 = Decode.Auto.generateDecoderCached<User>(caseStrategy=CamelCase)
                let actual1 = Decode.fromString decoder1 json
                let actual2 = Decode.fromString decoder2 json
                equal expected actual1
                equal expected actual2
                equal actual1 actual2

            testCase "Auto.fromString works with strange types if they are None" <| fun _ ->
                let json = """{"Id":0}"""
                Decode.Auto.fromString<RecordWithStrangeType>(json)
                |> equal (Ok { Id = 0; Thread = None })

            testCase "Auto.fromString works with recursive types" <| fun _ ->
                let vater =
                    { Name = "Alfonso"
                      Children = [ { Name = "Narumi"; Children = [] }
                                   { Name = "Takumi"; Children = [] } ] }
                let json = """{"Name":"Alfonso","Children":[{"Name":"Narumi","Children":[]},{"Name":"Takumi","Children":[]}]}"""
                Decode.Auto.fromString<MyRecType>(json)
                |> equal (Ok vater)

            testCase "Auto.unsafeFromString works for unit" <| fun _ ->
                let json = Encode.unit () |> Encode.toString 4
                let res = Decode.Auto.unsafeFromString<unit>(json)
                equal () res

            testCase "Erased single-case DUs works" <| fun _ ->
                let expected = NoAllocAttributeId (Guid.NewGuid())
                let json = Encode.Auto.toString(4, expected)
                let actual = Decode.Auto.unsafeFromString<NoAllocAttributeId>(json)
                equal expected actual

            testCase "Auto.unsafeFromString works with HTML inside of a string" <| fun _ ->
                let expected =
                    {
                        FeedName = "Ars"
                        Content = "<div><figure class=\"intro-image intro-left\"><img src=\"https://cdn.arstechnica.net/wp-content/uploads/2019/05/qualcomm-enforcer-800x450.jpg\" alt=\"How Qualcomm shook down the cell phone industry for almost 20 years\"><p class=\"caption\" style=\"font-size: 0.8em\"><a href=\"https://cdn.arstechnica.net/wp-content/uploads/2019/05/qualcomm-enforcer.jpg\" class=\"enlarge-link\">Enlarge</a> (credit: Getty / Aurich Lawson)</p>  </figure><div><a name=\"page-1\"></a></div><p>In 2005, Apple contacted Qualcomm as a potential supplier for modem chips in the first iPhone. Qualcomm's response was unusual: a letter demanding that Apple sign a patent licensing agreement before Qualcomm would even consider supplying chips.</p><p>\"I'd spent 20 years in the industry, I had never seen a letter like this,\" said Tony Blevins, Apple's vice president of procurement.</p><p>Most suppliers are eager to talk to new customers—especially customers as big and prestigious as Apple. But Qualcomm wasn't like other suppliers; it enjoyed a dominant position in the market for cellular chips. That gave Qualcomm a lot of leverage, and the company wasn't afraid to use it.</p></div><p><a href=\"https://arstechnica.com/?p=1510419#p3\">Read 70 remaining paragraphs</a> | <a href=\"https://arstechnica.com/?p=1510419&amp;comments=1\">Comments</a></p><div class=\"feedflare\"><a href=\"http://feeds.arstechnica.com/~ff/arstechnica/index?a=7NLlD3YvqFA:DF_-B3_cDwc:V_sGLiPBpWU\"><img src=\"http://feeds.feedburner.com/~ff/arstechnica/index?i=7NLlD3YvqFA:DF_-B3_cDwc:V_sGLiPBpWU\" border=\"0\"></a> <a href=\"http://feeds.arstechnica.com/~ff/arstechnica/index?a=7NLlD3YvqFA:DF_-B3_cDwc:F7zBnMyn0Lo\"><img src=\"http://feeds.feedburner.com/~ff/arstechnica/index?i=7NLlD3YvqFA:DF_-B3_cDwc:F7zBnMyn0Lo\" border=\"0\"></a> <a href=\"http://feeds.arstechnica.com/~ff/arstechnica/index?a=7NLlD3YvqFA:DF_-B3_cDwc:qj6IDK7rITs\"><img src=\"http://feeds.feedburner.com/~ff/arstechnica/index?d=qj6IDK7rITs\" border=\"0\"></a> <a href=\"http://feeds.arstechnica.com/~ff/arstechnica/index?a=7NLlD3YvqFA:DF_-B3_cDwc:yIl2AUoC8zA\"><img src=\"http://feeds.feedburner.com/~ff/arstechnica/index?d=yIl2AUoC8zA\" border=\"0\"></a></div>"
                    }

                let articleJson =
                    """
                {
                  "FeedName": "Ars",
                  "Content": "<div><figure class=\"intro-image intro-left\"><img src=\"https://cdn.arstechnica.net/wp-content/uploads/2019/05/qualcomm-enforcer-800x450.jpg\" alt=\"How Qualcomm shook down the cell phone industry for almost 20 years\"><p class=\"caption\" style=\"font-size: 0.8em\"><a href=\"https://cdn.arstechnica.net/wp-content/uploads/2019/05/qualcomm-enforcer.jpg\" class=\"enlarge-link\">Enlarge</a> (credit: Getty / Aurich Lawson)</p>  </figure><div><a name=\"page-1\"></a></div><p>In 2005, Apple contacted Qualcomm as a potential supplier for modem chips in the first iPhone. Qualcomm's response was unusual: a letter demanding that Apple sign a patent licensing agreement before Qualcomm would even consider supplying chips.</p><p>\"I'd spent 20 years in the industry, I had never seen a letter like this,\" said Tony Blevins, Apple's vice president of procurement.</p><p>Most suppliers are eager to talk to new customers—especially customers as big and prestigious as Apple. But Qualcomm wasn't like other suppliers; it enjoyed a dominant position in the market for cellular chips. That gave Qualcomm a lot of leverage, and the company wasn't afraid to use it.</p></div><p><a href=\"https://arstechnica.com/?p=1510419#p3\">Read 70 remaining paragraphs</a> | <a href=\"https://arstechnica.com/?p=1510419&amp;comments=1\">Comments</a></p><div class=\"feedflare\"><a href=\"http://feeds.arstechnica.com/~ff/arstechnica/index?a=7NLlD3YvqFA:DF_-B3_cDwc:V_sGLiPBpWU\"><img src=\"http://feeds.feedburner.com/~ff/arstechnica/index?i=7NLlD3YvqFA:DF_-B3_cDwc:V_sGLiPBpWU\" border=\"0\"></a> <a href=\"http://feeds.arstechnica.com/~ff/arstechnica/index?a=7NLlD3YvqFA:DF_-B3_cDwc:F7zBnMyn0Lo\"><img src=\"http://feeds.feedburner.com/~ff/arstechnica/index?i=7NLlD3YvqFA:DF_-B3_cDwc:F7zBnMyn0Lo\" border=\"0\"></a> <a href=\"http://feeds.arstechnica.com/~ff/arstechnica/index?a=7NLlD3YvqFA:DF_-B3_cDwc:qj6IDK7rITs\"><img src=\"http://feeds.feedburner.com/~ff/arstechnica/index?d=qj6IDK7rITs\" border=\"0\"></a> <a href=\"http://feeds.arstechnica.com/~ff/arstechnica/index?a=7NLlD3YvqFA:DF_-B3_cDwc:yIl2AUoC8zA\"><img src=\"http://feeds.feedburner.com/~ff/arstechnica/index?d=yIl2AUoC8zA\" border=\"0\"></a></div>"
                }
                    """

                let actual : TestStringWithHTML = Decode.Auto.unsafeFromString(articleJson)
                equal expected actual
        ]
    ]
