module Tests.Encoders.Manual

#if !NETFRAMEWORK
open Fable.Core
#endif

#if (THOTH_JSON && FABLE_COMPILER)
open Thoth.Json
open Fable.Mocha
open Fable.Core.JsInterop
#endif

#if THOTH_JSON && !FABLE_COMPILER
open Thoth.Json
open Expecto
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

let tests =
    testList "Thoth.Json.Encode" [

        testList "Basic" [

            testCase "a string works" <| fun _ ->
                let expected = "\"maxime\""
                let actual =
                    Encode.string "maxime"
                    |> Encode.toString 0
                Expect.equal actual expected ""

            testCase "a char works" <| fun _ ->
                let expected = "\"m\""
                let actual =
                    Encode.char 'm'
                    |> Encode.toString 0
                Expect.equal actual expected ""

            testCase "an int works" <| fun _ ->
                let expected = "1"
                let actual =
                    Encode.int 1
                    |> Encode.toString 0
                Expect.equal actual expected ""

            testCase "a float works" <| fun _ ->
                let expected = "1.2"
                let actual =
                    Encode.float 1.2
                    |> Encode.toString 0
                Expect.equal actual expected ""

            testCase "an array works" <| fun _ ->
                let expected =
                    """["maxime",2]"""
                let actual =
                    Encode.array
                        [| Encode.string "maxime"
                           Encode.int 2
                        |] |> Encode.toString 0
                Expect.equal actual expected ""

            testCase "a list works" <| fun _ ->
                let expected =
                    """["maxime",2]"""
                let actual =
                    Encode.list
                        [ Encode.string "maxime"
                          Encode.int 2
                        ] |> Encode.toString 0
                Expect.equal actual expected ""

            testCase "a bool works" <| fun _ ->
                let expected = "false"
                let actual =
                    Encode.bool false
                    |> Encode.toString 0
                Expect.equal actual expected ""

            testCase "a null works" <| fun _ ->
                let expected = "null"
                let actual =
                    Encode.nil
                    |> Encode.toString 0
                Expect.equal actual expected ""

            testCase "unit works" <| fun _ ->
                let expected = "null"
                let actual =
                    Encode.unit ()
                    |> Encode.toString 0
                Expect.equal actual expected ""

            testCase "an object works" <| fun _ ->
                let expected =
                    // THOTH_JSON parser doesn't re-order the properties
                    // This is not a problem because JSON object are an unordered structure
                    #if THOTH_JSON
                    """{"age":25,"firstname":"maxime"}"""
                    #else
                    """{"firstname":"maxime","age":25}"""
                    #endif
                let actual =
                    Encode.object
                        [ ("firstname", Encode.string "maxime")
                          ("age", Encode.int 25)
                        ] |> Encode.toString 0
                Expect.equal actual expected ""

            testCase "a dict works" <| fun _ ->
                let expected =
                    """{"a":1,"b":2,"c":3}"""
                let actual =
                    Map.ofList
                        [ ("a", Encode.int 1)
                          ("b", Encode.int 2)
                          ("c", Encode.int 3)
                        ]
                    |> Encode.dict
                    |> Encode.toString 0
                Expect.equal actual expected ""

            testCase "a bigint works" <| fun _ ->
                let expected = "\"12\""
                let actual =
                    Encode.bigint 12I
                    |> Encode.toString 0

                Expect.equal actual expected ""

            testCase "a datetime works" <| fun _ ->
                #if FABLE_COMPILER
                let expected = "\"2018-10-01T11:12:55.000Z\""
                #else
                let expected = "\"2018-10-01T11:12:55.0000000Z\""
                #endif
                let actual =
                    DateTime(2018, 10, 1, 11, 12, 55, DateTimeKind.Utc)
                    |> Encode.datetime
                    |> Encode.toString 0

                Expect.equal actual expected ""

            testCase "a datetimeOffset works" <| fun _ ->
                #if FABLE_COMPILER
                let expected = "\"2018-07-02T12:23:45.000+02:00\""
                #else
                let expected = "\"2018-07-02T12:23:45.0000000+02:00\""
                #endif
                let actual =
                    DateTimeOffset(2018, 7, 2, 12, 23, 45, 0, TimeSpan.FromHours(2.))
                    |> Encode.datetimeOffset
                    |> Encode.toString 0

                Expect.equal actual expected ""

            testCase "a timeSpan works" <| fun _ ->
                let expected = "\"1.02:03:04.0050000\""
                let actual =
                    TimeSpan(1, 2, 3, 4, 5)
                    |> Encode.timespan
                    |> Encode.toString 0

                Expect.equal actual expected ""

            testCase "a decimal works" <| fun _ ->
                let expected = "\"0.7833\""
                let actual =
                    0.7833M
                    |> Encode.decimal
                    |> Encode.toString 0

                Expect.equal actual expected ""

            testCase "a guid works" <| fun _ ->
                let expected = "\"1e5dee25-8558-4392-a9fb-aae03f81068f\""
                let actual =
                    Guid.Parse("1e5dee25-8558-4392-a9fb-aae03f81068f")
                    |> Encode.guid
                    |> Encode.toString 0

                Expect.equal actual expected ""

            testCase "an byte works" <| fun _ ->
                let expected = "\"99\""
                let actual =
                    99uy
                    |> Encode.byte
                    |> Encode.toString 0

                Expect.equal actual expected ""

            testCase "an sbyte works" <| fun _ ->
                let expected = "\"99\""
                let actual =
                    99y
                    |> Encode.sbyte
                    |> Encode.toString 0

                Expect.equal actual expected ""

            testCase "an int16 works" <| fun _ ->
                let expected = "\"99\""
                let actual =
                    99s
                    |> Encode.int16
                    |> Encode.toString 0

                Expect.equal actual expected ""

            testCase "an uint16 works" <| fun _ ->
                let expected = "\"99\""
                let actual =
                    99us
                    |> Encode.uint16
                    |> Encode.toString 0

                Expect.equal actual expected ""

            testCase "an int64 works" <| fun _ ->
                let expected = "\"7923209\""
                let actual =
                    7923209L
                    |> Encode.int64
                    |> Encode.toString 0

                Expect.equal actual expected ""

            testCase "an uint64 works" <| fun _ ->
                let expected = "\"7923209\""
                let actual =
                    7923209UL
                    |> Encode.uint64
                    |> Encode.toString 0

                Expect.equal actual expected ""

            testCase "an enum<sbyte> works" <| fun _ ->
                let expected = "\"99\""
                let actual =
                    Encode.toString 0 (Encode.Enum.sbyte Enum_Int8.NinetyNine)

                Expect.equal actual expected ""

            testCase "an enum<byte> works" <| fun _ ->
                let expected = "\"99\""
                let actual =
                    Encode.toString 0 (Encode.Enum.byte Enum_UInt8.NinetyNine)

                Expect.equal actual expected ""

            testCase "an enum<int> works" <| fun _ ->
                let expected = "1"
                let actual =
                    Encode.toString 0 (Encode.Enum.int Enum_Int.One)

                Expect.equal actual expected ""

            testCase "an enum<uint32> works" <| fun _ ->
                let expected = "99"
                let actual =
                    Encode.toString 0 (Encode.Enum.uint32 Enum_UInt32.NinetyNine)

                Expect.equal actual expected ""

            testCase "an enum<int16> works" <| fun _ ->
                let expected = "\"99\""
                let actual =
                    Encode.toString 0 (Encode.Enum.int16 Enum_Int16.NinetyNine)

                Expect.equal actual expected ""

            testCase "an enum<uint16> works" <| fun _ ->
                let expected = "\"99\""
                let actual =
                    Encode.toString 0 (Encode.Enum.uint16 Enum_UInt16.NinetyNine)

                Expect.equal actual expected ""

            testCase "a tuple2 works" <| fun _ ->
                let expected = """[1,"maxime"]"""
                let actual =
                    Encode.tuple2
                        Encode.int
                        Encode.string
                        (1, "maxime")
                    |> Encode.toString 0

                Expect.equal actual expected ""

            testCase "a tuple3 works" <| fun _ ->
                let expected = """[1,"maxime",2.5]"""
                let actual =
                    Encode.tuple3
                        Encode.int
                        Encode.string
                        Encode.float
                        (1, "maxime", 2.5)
                    |> Encode.toString 0

                Expect.equal actual expected ""

            testCase "a tuple4 works" <| fun _ ->
                let expected = """[1,"maxime",2.5,{"fieldA":"test"}]"""
                let actual =
                    Encode.tuple4
                        Encode.int
                        Encode.string
                        Encode.float
                        SmallRecord.Encoder
                        (1, "maxime", 2.5, { fieldA = "test" })
                    |> Encode.toString 0

                Expect.equal actual expected ""

            testCase "a tuple5 works" <| fun _ ->
                #if FABLE_COMPILER
                let expected = """[1,"maxime",2.5,{"fieldA":"test"},"2018-10-01T11:12:55.000Z"]"""
                #else
                let expected = """[1,"maxime",2.5,{"fieldA":"test"},"2018-10-01T11:12:55.0000000Z"]"""
                #endif
                let actual =
                    Encode.tuple5
                        Encode.int
                        Encode.string
                        Encode.float
                        SmallRecord.Encoder
                        Encode.datetime
                        (1, "maxime", 2.5, { fieldA = "test" }, DateTime(2018, 10, 1, 11, 12, 55, DateTimeKind.Utc))
                    |> Encode.toString 0

                Expect.equal actual expected ""

            testCase "a tuple6 works" <| fun _ ->
                let expected = """[1,"maxime",2.5,{"fieldA":"test"},false,null]"""
                let actual =
                    Encode.tuple6
                        Encode.int
                        Encode.string
                        Encode.float
                        SmallRecord.Encoder
                        Encode.bool
                        (fun _ -> Encode.nil)
                        (1, "maxime", 2.5, { fieldA = "test" }, false, null)
                    |> Encode.toString 0

                Expect.equal actual expected ""

            testCase "a tuple7 works" <| fun _ ->
                let expected = """[1,"maxime",2.5,{"fieldA":"test"},false,null,true]"""
                let actual =
                    Encode.tuple7
                        Encode.int
                        Encode.string
                        Encode.float
                        SmallRecord.Encoder
                        Encode.bool
                        (fun _ -> Encode.nil)
                        Encode.bool
                        (1, "maxime", 2.5, { fieldA = "test" }, false, null, true)
                    |> Encode.toString 0

                Expect.equal actual expected ""

            testCase "a tuple8 works" <| fun _ ->
                let expected = """[1,"maxime",2.5,{"fieldA":"test"},false,null,true,98]"""
                let actual =
                    Encode.tuple8
                        Encode.int
                        Encode.string
                        Encode.float
                        SmallRecord.Encoder
                        Encode.bool
                        (fun _ -> Encode.nil)
                        Encode.bool
                        Encode.int
                        (1, "maxime", 2.5, { fieldA = "test" }, false, null, true, 98)
                    |> Encode.toString 0

                Expect.equal actual expected ""

            testCase "using pretty space works" <| fun _ ->
                // THOTH_JSON parser doesn't re-order the properties
                // This is not a problem because JSON object are an unordered structure
                #if THOTH_JSON
                let expected = "{\n    \"age\": 25,\n    \"firstname\": \"maxime\"\n}"
                #else
                let expected = "{\n    \"firstname\": \"maxime\",\n    \"age\": 25\n}"
                #endif

                let actual =
                    Encode.object
                        [ ("firstname", Encode.string "maxime")
                          ("age", Encode.int 25)
                        ] |> Encode.toString 4
                Expect.equal actual expected ""

            testCase "complex structure works" <| fun _ ->
                // THOTH_JSON parser doesn't re-order the properties
                // This is not a problem because JSON object are an unordered structure
                #if THOTH_JSON
                let expected =
                    "{\n    \"address\": {\n        \"city\": \"Bordeaux\",\n        \"street\": \"main road\"\n    },\n    \"age\": 25,\n    \"firstname\": \"maxime\"\n}"
                #else
                let expected =
                    "{\n    \"firstname\": \"maxime\",\n    \"age\": 25,\n    \"address\": {\n        \"street\": \"main road\",\n        \"city\": \"Bordeaux\"\n    }\n}"
                #endif

                let actual =
                    Encode.object
                        [ ("firstname", Encode.string "maxime")
                          ("age", Encode.int 25)
                          ("address", Encode.object
                                        [ "street", Encode.string "main road"
                                          "city", Encode.string "Bordeaux"
                                        ])
                        ] |> Encode.toString 4
                Expect.equal actual expected ""

            testCase "option with a value `Some ...` works" <| fun _ ->
                let expected = """{"id":1,"operator":"maxime"}"""

                let actual =
                    Encode.object
                        [ ("id", Encode.int 1)
                          ("operator", Encode.option Encode.string (Some "maxime"))
                        ] |> Encode.toString 0

                Expect.equal actual expected ""

            testCase "option without a value `None` works" <| fun _ ->
                let expected = """{"id":1,"operator":null}"""

                let actual =
                    Encode.object
                        [ ("id", Encode.int 1)
                          ("operator", Encode.option Encode.string None)
                        ] |> Encode.toString 0

                Expect.equal actual expected ""

      ]

    ]
