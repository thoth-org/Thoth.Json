module Tests.Decoders.Manual

#if !NETFRAMEWORK
open Fable.Core
#endif

#if THOTH_JSON_FABLE
open Thoth.Json.Fable
open Fable.Mocha
open Fable.Core.JsInterop
#endif

#if THOTH_JSON_NEWTONSOFT
open Thoth.Json.Newtonsoft
open Expecto
#endif

open Tests.Types
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

type RecordWithPrivateConstructor = private { Foo1: int; Foo2: float }
type UnionWithPrivateConstructor = private Bar of string | Baz
let tests =
    testList "Thoth.Json.Decode" [

        testList "Errors" [

            #if FABLE_COMPILER

            testCase "circular structure are supported when reporting error" <| fun _ ->
                let a = createObj [ ]
                let b = createObj [ ]
                a?child <- b
                b?child <- a

                let expected : Result<float, string> = Error "Error at: `$`\nExpecting a float but decoder failed. Couldn\'t report given value due to circular structure. "
                let actual = Decode.fromValue Decode.float b

                Expect.equal actual expected ""
            #endif

            testCase "invalid json" <| fun _ ->
                #if FABLE_COMPILER
                let expected : Result<float, string> = Error "Given an invalid JSON: Unexpected token m in JSON at position 0"
                #else
                let expected : Result<float, string> = Error "Given an invalid JSON: Unexpected character encountered while parsing value: m. Path '', line 0, position 0."
                #endif
                let actual = Decode.fromString Decode.float "maxime"

                Expect.equal actual expected ""

            testCase "user exceptions are not captured by the decoders" <| fun _ ->
                let expected = true

                let decoder =
                    (fun _ ->
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

                Expect.equal actual expected ""
        ]

        testList "Primitives" [

            testCase "unit works" <| fun _ ->
                let expected = Ok ()
                let actual =
                    Decode.fromString Decode.unit "null"

                Expect.equal actual expected ""

            testCase "a string works" <| fun _ ->
                let expected = Ok("maxime")
                let actual =
                    Decode.fromString Decode.string "\"maxime\""

                Expect.equal actual expected ""

            testCase "a float works" <| fun _ ->
                let expected = Ok(1.2)
                let actual =
                    Decode.fromString Decode.float "1.2"

                Expect.equal actual expected ""

            testCase "a float from int works" <| fun _ ->
                let expected = Ok(1.0)
                let actual =
                    Decode.fromString Decode.float "1"

                Expect.equal actual expected ""

            testCase "a bool works" <| fun _ ->
                let expected = Ok(true)
                let actual =
                    Decode.fromString Decode.bool "true"

                Expect.equal actual expected ""

            testCase "an invalid bool output an error" <| fun _ ->
                let expected = Error("Error at: `$`\nExpecting a boolean but instead got: 2")
                let actual =
                    Decode.fromString Decode.bool "2"

                Expect.equal actual expected ""

            testCase "an int works" <| fun _ ->
                let expected = Ok(25)
                let actual =
                    Decode.fromString Decode.int "25"

                Expect.equal actual expected ""

            testCase "an invalid int [invalid range: too big] output an error" <| fun _ ->
                let expected = Error("Error at: `$`\nExpecting an int but instead got: 2147483648\nReason: Value was either too large or too small for an int")
                let actual =
                    Decode.fromString Decode.int "2147483648"

                Expect.equal actual expected ""

            testCase "an invalid int [invalid range: too small] output an error" <| fun _ ->
                let expected = Error("Error at: `$`\nExpecting an int but instead got: -2147483649\nReason: Value was either too large or too small for an int")
                let actual =
                    Decode.fromString Decode.int "-2147483649"

                Expect.equal actual expected ""

            testCase "an int16 works from number" <| fun _ ->
                let expected = Ok(int16 25)
                let actual =
                    Decode.fromString Decode.int16 "25"

                Expect.equal actual expected ""

            testCase "an int16 works from string" <| fun _ ->
                let expected = Ok(int16 -25)
                let actual =
                    Decode.fromString Decode.int16 "\"-25\""

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

            testCase "an int16 output an error if incorrect string" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting an int16 but instead got: "maxime"
                        """.Trim())
                let actual =
                    Decode.fromString Decode.int16 "\"maxime\""

                Expect.equal actual expected ""

            testCase "an uint16 works from number" <| fun _ ->
                let expected = Ok(uint16 25)
                let actual =
                    Decode.fromString Decode.uint16 "25"

                Expect.equal actual expected ""

            testCase "an uint16 works from string" <| fun _ ->
                let expected = Ok(uint16 25)
                let actual =
                    Decode.fromString Decode.uint16 "\"25\""

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

            testCase "an uint16 output an error if incorrect string" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting an uint16 but instead got: "maxime"
                        """.Trim())
                let actual =
                    Decode.fromString Decode.uint16 "\"maxime\""

                Expect.equal actual expected ""

            testCase "an int64 works from number" <| fun _ ->
                let expected = Ok 1000L
                let actual =
                    Decode.fromString Decode.int64 "1000"

                Expect.equal actual expected ""

            testCase "an int64 works from string" <| fun _ ->
                let expected = Ok 99L
                let actual =
                    Decode.fromString Decode.int64 "\"99\""

                Expect.equal actual expected ""

            testCase "an int64 works output an error if incorrect string" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting an int64 but instead got: "maxime"
                        """.Trim())
                let actual =
                    Decode.fromString Decode.int64 "\"maxime\""

                Expect.equal actual expected ""

            testCase "an uint32 works from number" <| fun _ ->
                let expected = Ok 1000u
                let actual =
                    Decode.fromString Decode.uint32 "1000"

                Expect.equal actual expected ""

            testCase "an uint32 works from string" <| fun _ ->
                let expected = Ok 1000u
                let actual =
                    Decode.fromString Decode.uint32 "\"1000\""

                Expect.equal actual expected ""

            testCase "an uint32 output an error if incorrect string" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting an uint32 but instead got: "maxime"
                        """.Trim())

                let actual =
                    Decode.fromString Decode.uint32 "\"maxime\""

                Expect.equal actual expected ""

            testCase "an uint64 works from number" <| fun _ ->
                let expected = Ok 1000UL
                let actual =
                    Decode.fromString Decode.uint64 "1000"

                Expect.equal actual expected ""

            testCase "an uint64 works from string" <| fun _ ->
                let expected = Ok 1000UL
                let actual =
                    Decode.fromString Decode.uint64 "\"1000\""

                Expect.equal actual expected ""

            testCase "an uint64 output an error if incorrect string" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting an uint64 but instead got: "maxime"
                        """.Trim())

                let actual =
                    Decode.fromString Decode.uint64 "\"maxime\""

                Expect.equal actual expected ""

            testCase "a byte works from number" <| fun _ ->
                let expected = Ok 25uy
                let actual =
                    Decode.fromString Decode.byte "25"

                Expect.equal actual expected ""

            testCase "a byte works from string" <| fun _ ->
                let expected = Ok 25uy
                let actual =
                    Decode.fromString Decode.byte "\"25\""

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

            testCase "a byte output an error if incorrect string" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting a byte but instead got: "maxime"
                        """.Trim())
                let actual =
                    Decode.fromString Decode.byte "\"maxime\""

                Expect.equal actual expected ""

            testCase "a sbyte works from number" <| fun _ ->
                let expected = Ok 25y
                let actual =
                    Decode.fromString Decode.sbyte "25"

                Expect.equal actual expected ""

            testCase "a sbyte works from string" <| fun _ ->
                let expected = Ok -25y
                let actual =
                    Decode.fromString Decode.sbyte "\"-25\""

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

            testCase "a sbyte output an error if incorrect string" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting a sbyte but instead got: "maxime"
                        """.Trim())
                let actual =
                    Decode.fromString Decode.sbyte "\"maxime\""

                Expect.equal actual expected ""

            testCase "an bigint works from number" <| fun _ ->
                let expected = Ok 12I
                let actual =
                    Decode.fromString Decode.bigint "12"

                Expect.equal actual expected ""

            testCase "an bigint works from string" <| fun _ ->
                let expected = Ok 12I
                let actual =
                    Decode.fromString Decode.bigint "\"12\""

                Expect.equal actual expected ""

            testCase "an bigint output an error if invalid string" <| fun _ ->
                let expected =
                    Error (
                        """
Error at: `$`
Expecting a bigint but instead got: "maxime"
                        """.Trim())
                let actual =
                    Decode.fromString Decode.bigint "\"maxime\""

                Expect.equal actual expected ""

            testCase "a string representing a DateTime should be accepted as a string" <| fun _ ->
                let expected = "2018-10-01T11:12:55.00Z"
                let actual =
                    Decode.fromString Decode.string "\"2018-10-01T11:12:55.00Z\""

                Expect.equal (Ok expected) actual ""

            testCase "a datetime works" <| fun _ ->
                let expected = new DateTime(2018, 10, 1, 11, 12, 55, DateTimeKind.Utc)
                let actual =
                    Decode.fromString Decode.datetime "\"2018-10-01T11:12:55.00Z\""

                Expect.equal (Ok expected) actual ""

            testCase "a datetime output an error if invalid string" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$`
Expecting a datetime but instead got: "invalid_string"
                        """.Trim())

                let actual =
                    Decode.fromString Decode.datetime "\"invalid_string\""

                Expect.equal actual expected ""

            testCase "a datetime works with TimeZone" <| fun _ ->
                let localDate = DateTime(2018, 10, 1, 11, 12, 55, DateTimeKind.Local)

                let expected = Ok (localDate.ToUniversalTime())
                let json = sprintf "\"%s\"" (localDate.ToString("O"))
                let actual =
                    Decode.fromString Decode.datetime json

                Expect.equal actual expected ""

            testCase "a datetimeOffset works" <| fun _ ->
                let expected =
                    DateTimeOffset(2018, 7, 2, 12, 23, 45, 0, TimeSpan.FromHours(2.))
                    |> Ok
                let json = "\"2018-07-02T12:23:45+02:00\""
                let actual =
                    Decode.fromString Decode.datetimeOffset json
                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

            testCase "a timespan works" <| fun _ ->
                let expected =
                    TimeSpan(23, 45, 0)
                    |> Ok
                let json = "\"23:45:00\""
                let actual =
                    Decode.fromString Decode.timespan json
                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

            testCase "an enum<sbyte> works" <| fun _ ->
                let expected = Ok Enum_Int8.NinetyNine
                let actual =
                    Decode.fromString Decode.Enum.sbyte "99"

                Expect.equal actual expected ""

            testCase "an enum<byte> works" <| fun _ ->
                let expected = Ok Enum_UInt8.NinetyNine
                let actual =
                    Decode.fromString Decode.Enum.byte "99"

                Expect.equal actual expected ""

            testCase "an enum<int> works" <| fun _ ->
                let expected = Ok Enum_Int.One
                let actual =
                    Decode.fromString Decode.Enum.int "1"

                Expect.equal actual expected ""

            testCase "an enum<uint32> works" <| fun _ ->
                let expected = Ok Enum_UInt32.NinetyNine
                let actual =
                    Decode.fromString Decode.Enum.uint32 "99"

                Expect.equal actual expected ""

            testCase "an enum<int16> works" <| fun _ ->
                let expected = Ok Enum_Int16.NinetyNine
                let actual =
                    Decode.fromString Decode.Enum.int16 "99"

                Expect.equal actual expected ""

            testCase "an enum<uint16> works" <| fun _ ->
                let expected = Ok Enum_UInt16.NinetyNine
                let actual =
                    Decode.fromString Decode.Enum.uint16 "99"

                Expect.equal actual expected ""

        ]

        testList "Tuples" [
            testCase "tuple2 works" <| fun _ ->
                let json = """[1, "maxime"]"""
                let expected = Ok(1, "maxime")

                let actual =
                    Decode.fromString (Decode.tuple2 Decode.int Decode.string) json

                Expect.equal actual expected ""

            testCase "tuple3 works" <| fun _ ->
                let json = """[1, "maxime", 2.5]"""
                let expected = Ok(1, "maxime", 2.5)

                let actual =
                    Decode.fromString
                        (Decode.tuple3
                            Decode.int
                            Decode.string
                            Decode.float) json

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

        ]

        testList "Object primitives" [

            testCase "field works" <| fun _ ->
                let json = """{ "name": "maxime", "age": 25 }"""
                let expected = Ok("maxime")

                let actual =
                    Decode.fromString (Decode.field "name" Decode.string) json

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

            testCase "at works" <| fun _ ->

                let json = """{ "user": { "name": "maxime", "age": 25 } }"""
                let expected = Ok "maxime"

                let actual =
                    Decode.fromString (Decode.at ["user"; "name"] Decode.string) json

                Expect.equal actual expected ""

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
Node `firstname` is unknown.
                        """.Trim())

                let actual =
                    Decode.fromString (Decode.at ["user"; "firstname"] Decode.string) json

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

            testCase "index works" <| fun _ ->
                let json = """["maxime", "alfonso", "steffen"]"""
                let expected = Ok("alfonso")

                let actual =
                    Decode.fromString (Decode.index 1 Decode.string) json

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

        ]

        testList "Data structure" [

            testCase "list works" <| fun _ ->
                let expected = Ok([1; 2; 3])

                let actual =
                    Decode.fromString (Decode.list Decode.int) "[1, 2, 3]"

                Expect.equal actual expected ""

            // testCase "nested lists work" <| fun _ ->
            //     [ [ "maxime2" ] ]
            //     |> List.map (fun d ->
            //         d
            //         |> List.map Encode.string
            //         |> Encode.list)
            //     |> Encode.list
            //     |> Encode.toString 4
            //     |> Decode.fromString (Decode.list (Decode.list Decode.string))
            //     |> function Ok v -> Expect.equal [["maxime2"]] v "" | Error er -> failwith er

            testCase "an invalid list output an error" <| fun _ ->
                let expected = Error("Error at: `$`\nExpecting a list but instead got: 1")

                let actual =
                    Decode.fromString (Decode.list Decode.int) "1"

                Expect.equal actual expected ""

            testCase "array works" <| fun _ ->
                // Need to pass by a list otherwise Fable use:
                // new Int32Array([1, 2, 3]) and the test fails
                // And this would give:
                // Expected: Result { tag: 0, data: Int32Array [ 1, 2, 3 ] }
                // Actual: Result { tag: 0, data: [ 1, 2, 3 ] }
                let expected = Ok([1; 2; 3] |> List.toArray)

                let actual =
                    Decode.fromString (Decode.array Decode.int) "[1, 2, 3]"

                Expect.equal actual expected ""

            testCase "an invalid array output an error" <| fun _ ->
                let expected = Error("Error at: `$`\nExpecting an array but instead got: 1")

                let actual =
                    Decode.fromString (Decode.array Decode.int) "1"

                Expect.equal actual expected ""

            testCase "keys works" <| fun _ ->
                let expected = Ok(["a"; "b"; "c"])

                let actual =
                    Decode.fromString Decode.keys """{ "a": 1, "b": 2, "c": 3 }"""

                Expect.equal actual expected ""

            testCase "keys returns an error for invalid objects" <| fun _ ->
                let expected = Error("Error at: `$`\nExpecting an object but instead got: 1")

                let actual =
                    Decode.fromString Decode.keys "1"

                Expect.equal actual expected ""

            testCase "keyValuePairs works" <| fun _ ->
                let expected = Ok([("a", 1) ; ("b", 2) ; ("c", 3)])

                let actual =
                    Decode.fromString (Decode.keyValuePairs Decode.int) """{ "a": 1, "b": 2, "c": 3 }"""

                Expect.equal actual expected ""

            testCase "dict works" <| fun _ ->
                let expected = Ok(Map.ofList([("a", 1) ; ("b", 2) ; ("c", 3)]))

                let actual =
                    Decode.fromString (Decode.dict Decode.int) """{ "a": 1, "b": 2, "c": 3 }"""

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

            testCase "an invalid dict output an error" <| fun _ ->
                let expected = Error("Error at: `$`\nExpecting an object but instead got: 1")

                let actual =
                    Decode.fromString (Decode.dict Decode.int) "1"

                Expect.equal actual expected ""

        ]

        testList "Inconsistent structure" [

            testCase "oneOf works" <| fun _ ->
                let expected = Ok([1; 2; 0; 4])

                let badInt =
                    Decode.oneOf [ Decode.int; Decode.nil 0 ]

                let actual =
                    Decode.fromString (Decode.list badInt) "[1,2,null,4]"

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

            testCase "oneOf works with optional" <| fun _ ->
                let decoder =
                    Decode.oneOf
                        [
                            Decode.field "Normal" Decode.float |> Decode.map Normal
                            Decode.field "Reduced" (Decode.option Decode.float) |> Decode.map Reduced
                            Decode.field "Zero" Decode.bool |> Decode.map (fun _ -> Zero)
                        ]

                let a = """{"Normal": 4.5}""" |> Decode.fromString decoder
                Expect.equal (Ok(Normal 4.5)) a ""

                let b = """{"Reduced": 4.5}""" |> Decode.fromString decoder
                Expect.equal (Ok(Reduced(Some 4.5))) b ""

                let c = """{"Reduced": null}""" |> Decode.fromString decoder
                Expect.equal (Ok(Reduced None)) c ""

                let d = """{"Zero": true}""" |> Decode.fromString decoder
                Expect.equal (Ok Zero) d ""

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

                Expect.equal actual expected ""

            testCase "optional works" <| fun _ ->
                let json = """{ "name": "maxime", "age": 25, "something_undefined": null }"""

                let expectedValid = Ok(Some "maxime")
                let actualValid =
                    Decode.fromString (Decode.optional "name" Decode.string) json

                Expect.equal expectedValid actualValid ""

                match Decode.fromString (Decode.optional "name" Decode.int) json with
                | Error _ -> ()
                | Ok _ -> failwith "Expected type error for `name` field"

                let expectedMissingField = Ok(None)
                let actualMissingField =
                    Decode.fromString (Decode.optional "height" Decode.int) json

                Expect.equal expectedMissingField actualMissingField ""

                let expectedUndefinedField = Ok(None)
                let actualUndefinedField =
                    Decode.fromString (Decode.optional "something_undefined" Decode.string) json

                Expect.equal expectedUndefinedField actualUndefinedField ""

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

                Expect.equal actual expected ""

            testCase "optionalAt works" <| fun _ ->
                let json = """{ "data" : { "name": "maxime", "age": 25, "something_undefined": null } }"""

                let expectedValid = Ok(Some "maxime")
                let actualValid =
                    Decode.fromString (Decode.optionalAt [ "data"; "name" ] Decode.string) json

                Expect.equal expectedValid actualValid ""

                match Decode.fromString (Decode.optionalAt [ "data"; "name" ] Decode.int) json with
                | Error _ -> ()
                | Ok _ -> failwith "Expected type error for `name` field"

                let expectedMissingField = Ok None
                let actualMissingField =
                    Decode.fromString (Decode.optionalAt [ "data"; "height" ] Decode.int) json

                Expect.equal expectedMissingField actualMissingField ""

                let expectedUndefinedField = Ok(None)
                let actualUndefinedField =
                    Decode.fromString (Decode.optionalAt [ "data"; "something_undefined" ] Decode.string) json

                Expect.equal expectedUndefinedField actualUndefinedField ""

            testCase "combining field and option decoders works" <| fun _ ->
                let json = """{ "name": "maxime", "age": 25, "something_undefined": null }"""

                let expectedValid = Ok(Some "maxime")
                let actualValid =
                    Decode.fromString (Decode.field "name" (Decode.option Decode.string)) json

                Expect.equal expectedValid actualValid ""

                match Decode.fromString (Decode.field "name" (Decode.option Decode.int)) json with
                | Error msg ->
                    let expected =
                        """
Error at: `$.name`
Expecting an int but instead got: "maxime"
                        """.Trim()
                    Expect.equal expected msg ""
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
                    Expect.equal expected msg ""
                | Ok _ ->
                    failwith "Expected type error for `name` field #2"

                match Decode.fromString (Decode.field "something_undefined" (Decode.option Decode.int)) json with
                | Error _ -> failwith """`Decode.field "something_undefined" (Decode.option Decode.int)` test should pass"""
                | Ok result -> Expect.equal None result ""

                // Same tests as before but we are calling `option` then `field`

                let expectedValid2 = Ok(Some "maxime")
                let actualValid2 =
                    Decode.fromString (Decode.option (Decode.field "name" Decode.string)) json

                Expect.equal expectedValid2 actualValid2 ""

                match Decode.fromString (Decode.option (Decode.field "name" Decode.int)) json with
                | Error msg ->
                    let expected =
                        """
Error at: `$.name`
Expecting an int but instead got: "maxime"
                        """.Trim()
                    Expect.equal expected msg ""
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
                    Expect.equal expected msg ""
                | Ok _ -> failwith "Expected type error for `name` field #4"

                match Decode.fromString (Decode.option (Decode.field "something_undefined" Decode.int)) json with
                | Error msg ->
                    let expected =
                        """
Error at: `$.something_undefined`
Expecting an int but instead got: null
                        """.Trim()
                    Expect.equal expected msg ""
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

                    Expect.equal expected msg ""

                | Ok _ -> failwith "Expected type error for `height` field"

                let expectedUndefinedField = Ok(None)
                let actualUndefinedField =
                    Decode.fromString (Decode.field "something_undefined" (Decode.option Decode.string)) json

                Expect.equal expectedUndefinedField actualUndefinedField ""

        ]

        testList "Fancy decoding" [

            testCase "null works (test on an int)" <| fun _ ->
                let expected = Ok(20)
                let actual =
                    Decode.fromString (Decode.nil 20) "null"

                Expect.equal actual expected ""

            testCase "null works (test on a boolean)" <| fun _ ->
                let expected = Ok(false)
                let actual =
                    Decode.fromString (Decode.nil false) "null"

                Expect.equal actual expected ""

            testCase "succeed works" <| fun _ ->
                let expected = Ok(7)
                let actual =
                    Decode.fromString (Decode.succeed 7) "true"

                Expect.equal actual expected ""

            testCase "succeed output an error if the JSON is invalid" <| fun _ ->
                #if FABLE_COMPILER
                let expected = Error("Given an invalid JSON: Unexpected token m in JSON at position 0")
                #else
                let expected = Error("Given an invalid JSON: Unexpected character encountered while parsing value: m. Path '', line 0, position 0.")
                #endif
                let actual =
                    Decode.fromString (Decode.succeed 7) "maxime"

                Expect.equal actual expected ""

            testCase "fail works" <| fun _ ->
                let msg = "Failing because it's fun"
                let expected = Error("Error at: `$`\nThe following `failure` occurred with the decoder: " + msg)
                let actual =
                    Decode.fromString (Decode.fail msg) "true"

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

            testCase "all works" <| fun _ ->
                let expected = Ok [1; 2; 3]

                let decodeAll = Decode.all [
                    Decode.succeed 1
                    Decode.succeed 2
                    Decode.succeed 3
                ]

                let actual = Decode.fromString decodeAll "{}"

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

            testCase "all succeeds on empty lists" <| fun _ ->
                let expected = Ok []

                let decodeNone = Decode.all []

                let actual = Decode.fromString decodeNone "{}"

                Expect.equal actual expected ""

            testCase "all fails when one decoder fails" <| fun _ ->
                let expected = Error("Error at: `$`\nExpecting an int but instead got: {}")

                let decodeAll = Decode.all [
                    Decode.succeed 1
                    Decode.int
                    Decode.succeed 3
                ]

                let actual = Decode.fromString decodeAll "{}"

                Expect.equal actual expected ""
        ]

        testList "Mapping" [

            testCase "map works" <| fun _ ->
                let expected = Ok(6)
                let stringLength =
                    Decode.map String.length Decode.string

                let actual =
                    Decode.fromString stringLength "\"maxime\""
                Expect.equal actual expected ""

            testCase "map2 works" <| fun _ ->
                let expected = Ok({a = 1.; b = 2.} : Record2)

                let decodePoint =
                    Decode.map2 Record2.Create
                        (Decode.field "a" Decode.float)
                        (Decode.field "b" Decode.float)

                let actual =
                    Decode.fromString decodePoint jsonRecord

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

            testCase "map2 generate an error if invalid" <| fun _ ->
                let expected = Error("Error at: `$.a`\nExpecting a float but instead got: \"invalid_a_field\"")

                let decodePoint =
                    Decode.map2 Record2.Create
                        (Decode.field "a" Decode.float)
                        (Decode.field "b" Decode.float)

                let actual =
                    Decode.fromString decodePoint jsonRecordInvalid

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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
Node `firstname` is unknown.
                        """.Trim())

                let decoder =
                    Decode.object
                        (fun get ->
                            { fieldA = get.Required.At [ "user"; "firstname" ] Decode.string }
                        )

                let actual =
                    Decode.fromString decoder json

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

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

                Expect.equal actual expected ""

            testCase "complex object builder works" <| fun _ ->
                let expected =
                    Ok(User.Create 67 "" "user@mail.com" 0)

                let userDecoder =
                    Decode.object
                        (fun get ->
                            {
                                Id = get.Required.Field "id" Decode.int
                                Name = get.Optional.Field "name" Decode.string
                                        |> Option.defaultValue ""
                                Email = get.Required.Field "email" Decode.string
                                Followers = 0
                            }
                        )

                let actual =
                    Decode.fromString
                        userDecoder
                        """{ "id": 67, "email": "user@mail.com" }"""

                Expect.equal actual expected ""

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
                        {
                            Enabled = get.Required.Field "enabled" Decode.bool
                            Shape = get.Required.Raw shapeDecoder
                        } : MyObj
                    )

                let actual =
                    Decode.fromString
                        decoder
                        json

                let expected =
                    Ok (
                        {
                            Enabled = true
                            Shape = Circle 20
                        } : MyObj)

                Expect.equal actual expected ""

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
                        {
                            Enabled = get.Required.Field "enabled" Decode.bool
                            Shape = get.Required.Raw shapeDecoder
                        } : MyObj
                    )

                let actual =
                    Decode.fromString
                        decoder
                        json

                let expected =
                    Error "Error at: `$`\nThe following `failure` occurred with the decoder: Unknown shape type custom_shape"

                Expect.equal actual expected ""

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
                        {
                            Enabled = get.Required.Field "enabled" Decode.bool
                            Shape = get.Required.Raw shapeDecoder
                        } : MyObj
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

                Expect.equal actual expected ""

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
                        {
                            Enabled = get.Required.Field "enabled" Decode.bool
                            Shape = get.Optional.Raw shapeDecoder
                        }
                    )

                let actual =
                    Decode.fromString
                        decoder
                        json

                let expected =
                    Ok
                        {
                            Enabled = true
                            Shape = Some (Circle 20)
                        }

                Expect.equal actual expected ""

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
                        {
                            Enabled = get.Required.Field "enabled" Decode.bool
                            Shape = get.Optional.Raw shapeDecoder
                        }
                    )

                let actual =
                    Decode.fromString
                        decoder
                        json

                let expected =
                    Ok
                        {
                            Enabled = true
                            Shape = None
                        }

                Expect.equal actual expected ""

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
                        {
                            Enabled = get.Required.Field "enabled" Decode.bool
                            Shape = get.Optional.Raw shapeDecoder
                        }
                    )

                let actual =
                    Decode.fromString
                        decoder
                        json

                let expected =
                    Error "Error at: `$`\nThe following `failure` occurred with the decoder: Unknown shape type invalid_shape"

                Expect.equal actual expected ""

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

                 Expect.equal actual expected ""

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
                        {
                            Enabled = get.Required.Field "enabled" Decode.bool
                            Shape = get.Optional.Raw shapeDecoder
                        }
                    )

                let actual =
                    Decode.fromString
                        decoder
                        json

                let expected =
                    Ok
                        {
                            Enabled = true
                            Shape = None
                        }

                Expect.equal actual expected ""

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
Node `sub_field` is unknown.

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

                Expect.equal actual expected ""

        ]

    ]
