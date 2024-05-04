module Thoth.Json.Tests.Encoders

open Thoth.Json.Tests.Testing
open System
open Thoth.Json.Tests.Types
open Thoth.Json.Core

type RecordWithPrivateConstructor =
    private
        {
            Foo1: int
            Foo2: float
        }

type UnionWithPrivateConstructor =
    private
    | Bar of string
    | Baz

let tests (runner: TestRunner<_, _>) =
    runner.testList
        "Thoth.Json.Encode"
        [

            runner.testList
                "Basic"
                [

                    runner.testCase "a string works"
                    <| fun _ ->
                        let expected = "\"maxime\""

                        let actual =
                            Encode.string "maxime" |> runner.Encode.toString 0

                        runner.equal actual expected

                    runner.testCase "a string with new line works"
                    <| fun _ ->
                        let expected = "\"a\\nb\""

                        let actual =
                            Encode.string "a\nb" |> runner.Encode.toString 4

                        runner.equal actual expected

                    runner.testCase "a string with new line character works"
                    <| fun _ ->
                        let expected = "\"a\\\\nb\""

                        let actual =
                            Encode.string "a\\nb" |> runner.Encode.toString 4

                        runner.equal actual expected

                    runner.testCase "a string with tab works"
                    <| fun _ ->
                        let expected = "\"a\\tb\""

                        let actual =
                            Encode.string "a\tb" |> runner.Encode.toString 4

                        runner.equal actual expected

                    runner.testCase "a string with tab character works"
                    <| fun _ ->
                        let expected = "\"a\\\\tb\""

                        let actual =
                            Encode.string "a\\tb" |> runner.Encode.toString 4

                        runner.equal actual expected

                    runner.testCase
                        "a string with non ascii characters works returns the characters as is"
                    <| fun _ ->
                        let expected = "\"Timo Mühlhaus\""

                        let actual =
                            Encode.string "Timo Mühlhaus"
                            |> runner.Encode.toString 0

                        runner.equal actual expected

                    runner.testCase "a char works"
                    <| fun _ ->
                        let expected = "\"a\""
                        let actual = Encode.char 'a' |> runner.Encode.toString 0

                        runner.equal actual expected

                    runner.testCase "an int works"
                    <| fun _ ->
                        let expected = "1"
                        let actual = Encode.int 1 |> runner.Encode.toString 0
                        runner.equal actual expected

                    runner.testCase "negative int keeps the sign"
                    <| fun _ ->
                        let expected = "-1"
                        let actual = Encode.int -1 |> runner.Encode.toString 0
                        runner.equal actual expected

                    runner.testCase "a float works"
                    <| fun _ ->
                        let expected = "1.2"

                        let actual =
                            Encode.float 1.2 |> runner.Encode.toString 0

                        runner.equal actual expected

                    runner.testCase "an array works"
                    <| fun _ ->
                        let expected = """["maxime",2]"""

                        let actual =
                            Encode.array
                                [|
                                    Encode.string "maxime"
                                    Encode.int 2
                                |]
                            |> runner.Encode.toString 0

                        runner.equal actual expected

                    runner.testCase "a list works"
                    <| fun _ ->
                        let expected = """["maxime",2]"""

                        let actual =
                            Encode.list
                                [
                                    Encode.string "maxime"
                                    Encode.int 2
                                ]
                            |> runner.Encode.toString 0

                        runner.equal actual expected

                    runner.testCase "a bool works"
                    <| fun _ ->
                        let expected = "false"

                        let actual =
                            Encode.bool false |> runner.Encode.toString 0

                        runner.equal actual expected

                    runner.testCase "a null works"
                    <| fun _ ->
                        let expected = "null"
                        let actual = Encode.nil |> runner.Encode.toString 0
                        runner.equal actual expected

                    runner.testCase "unit works"
                    <| fun _ ->
                        let expected = "null"
                        let actual = Encode.unit () |> runner.Encode.toString 0
                        runner.equal actual expected

                    runner.testCase "an object works"
                    <| fun _ ->
                        let expected = """{"firstname":"maxime","age":25}"""

                        let actual =
                            Encode.object
                                [
                                    ("firstname", Encode.string "maxime")
                                    ("age", Encode.int 25)
                                ]
                            |> runner.Encode.toString 0

                        runner.equal actual expected

                    runner.testCase "a dict works"
                    <| fun _ ->
                        let expected = """{"a":1,"b":2,"c":3}"""

                        let actual =
                            Map.ofList
                                [
                                    ("a", Encode.int 1)
                                    ("b", Encode.int 2)
                                    ("c", Encode.int 3)
                                ]
                            |> Encode.dict
                            |> runner.Encode.toString 0

                        runner.equal actual expected

                    runner.testCase "a map works"
                    <| fun _ ->
                        let expected = """[["a",1],["b",2],["c",3]]"""

                        let actual =
                            Map.ofList
                                [
                                    ("a", 1)
                                    ("b", 2)
                                    ("c", 3)
                                ]
                            |> Encode.map Encode.string Encode.int
                            |> runner.Encode.toString 0

                        runner.equal actual expected

                    runner.testCase "a bigint works"
                    <| fun _ ->
                        let expected = "\"12\""

                        let actual =
                            Encode.bigint 12I |> runner.Encode.toString 0

                        runner.equal actual expected

                    runner.testCase "a datetime works"
                    <| fun _ ->
#if FABLE_COMPILER_JAVASCRIPT
                        let expected = "\"2018-10-01T11:12:55.000Z\""
#endif

#if FABLE_COMPILER_PYTHON
                        let expected = "\"2018-10-01T11:12:55.000000Z\""
#endif

#if !FABLE_COMPILER
                        let expected = "\"2018-10-01T11:12:55.0000000Z\""
#endif
                        let actual =
                            DateTime(2018, 10, 1, 11, 12, 55, DateTimeKind.Utc)
                            |> Encode.datetime
                            |> runner.Encode.toString 0

                        runner.equal actual expected

#if !FABLE_COMPILER_PYTHON
                    runner.testCase "a datetimeOffset works"
                    <| fun _ ->
#if FABLE_COMPILER
                        let expected = "\"2018-07-02T12:23:45.000+02:00\""
#else
                        let expected = "\"2018-07-02T12:23:45.0000000+02:00\""
#endif
                        let actual =
                            DateTimeOffset(
                                2018,
                                7,
                                2,
                                12,
                                23,
                                45,
                                0,
                                TimeSpan.FromHours(2.)
                            )
                            |> Encode.datetimeOffset
                            |> runner.Encode.toString 0

                        runner.equal actual expected
#endif

                    runner.testCase "a timeSpan works"
                    <| fun _ ->
                        let expected = "\"1.02:03:04.0050000\""

                        let actual =
                            TimeSpan(1, 2, 3, 4, 5)
                            |> Encode.timespan
                            |> runner.Encode.toString 0

                        runner.equal actual expected

                    runner.testCase "a decimal works"
                    <| fun _ ->
                        let expected = "\"0.7833\""

                        let actual =
                            0.7833M
                            |> Encode.decimal
                            |> runner.Encode.toString 0

                        runner.equal actual expected

                    runner.testCase "a guid works"
                    <| fun _ ->
                        let expected =
                            "\"1e5dee25-8558-4392-a9fb-aae03f81068f\""

                        let actual =
                            Guid.Parse("1e5dee25-8558-4392-a9fb-aae03f81068f")
                            |> Encode.guid
                            |> runner.Encode.toString 0

                        runner.equal actual expected

                    runner.testCase "an byte works"
                    <| fun _ ->
                        let expected = "99"

                        let actual =
                            99uy |> Encode.byte |> runner.Encode.toString 0

                        runner.equal actual expected

                    runner.testCase "an sbyte works"
                    <| fun _ ->
                        let expected = "99"

                        let actual =
                            99y |> Encode.sbyte |> runner.Encode.toString 0

                        runner.equal actual expected

                    runner.testCase "negative sbyte keeps the sign"
                    <| fun _ ->
                        let expected = "-99"

                        let actual =
                            -99y |> Encode.sbyte |> runner.Encode.toString 0

                        runner.equal actual expected

                    runner.testCase "an int16 works"
                    <| fun _ ->
                        let expected = "99"

                        let actual =
                            99s |> Encode.int16 |> runner.Encode.toString 0

                        runner.equal actual expected

                    runner.testCase "negative int16 keeps the sign"
                    <| fun _ ->
                        let expected = "-99"

                        let actual =
                            -99s |> Encode.int16 |> runner.Encode.toString 0

                        runner.equal actual expected

                    runner.testCase "an uint16 works"
                    <| fun _ ->
                        let expected = "99"

                        let actual =
                            99us |> Encode.uint16 |> runner.Encode.toString 0

                        runner.equal actual expected

                    runner.testCase "an int64 works"
                    <| fun _ ->
                        let expected = "\"7923209\""

                        let actual =
                            7923209L |> Encode.int64 |> runner.Encode.toString 0

                        runner.equal actual expected

                    runner.testCase "an uint64 works"
                    <| fun _ ->
                        let expected = "\"7923209\""

                        let actual =
                            7923209UL
                            |> Encode.uint64
                            |> runner.Encode.toString 0

                        runner.equal actual expected

                    runner.testCase "an enum<sbyte> works"
                    <| fun _ ->
                        let expected = "99"

                        let actual =
                            runner.Encode.toString
                                0
                                (Encode.Enum.sbyte Enum_Int8.NinetyNine)

                        runner.equal actual expected

                    runner.testCase "an enum<byte> works"
                    <| fun _ ->
                        let expected = "99"

                        let actual =
                            runner.Encode.toString
                                0
                                (Encode.Enum.byte Enum_UInt8.NinetyNine)

                        runner.equal actual expected

                    runner.testCase "an enum<int> works"
                    <| fun _ ->
                        let expected = "1"

                        let actual =
                            runner.Encode.toString
                                0
                                (Encode.Enum.int Enum_Int.One)

                        runner.equal actual expected

                    runner.testCase "an enum<uint32> works"
                    <| fun _ ->
                        let expected = "99"

                        let actual =
                            runner.Encode.toString
                                0
                                (Encode.Enum.uint32 Enum_UInt32.NinetyNine)

                        runner.equal actual expected

                    runner.testCase "an enum<int16> works"
                    <| fun _ ->
                        let expected = "99"

                        let actual =
                            runner.Encode.toString
                                0
                                (Encode.Enum.int16 Enum_Int16.NinetyNine)

                        runner.equal actual expected

                    runner.testCase "an enum<uint16> works"
                    <| fun _ ->
                        let expected = "99"

                        let actual =
                            runner.Encode.toString
                                0
                                (Encode.Enum.uint16 Enum_UInt16.NinetyNine)

                        runner.equal actual expected

                    runner.testCase "a tuple2 works"
                    <| fun _ ->
                        let expected = """[1,"maxime"]"""

                        let actual =
                            Encode.tuple2 Encode.int Encode.string (1, "maxime")
                            |> runner.Encode.toString 0

                        runner.equal actual expected

                    runner.testCase "a tuple3 works"
                    <| fun _ ->
                        let expected = """[1,"maxime",2.5]"""

                        let actual =
                            Encode.tuple3
                                Encode.int
                                Encode.string
                                Encode.float
                                (1, "maxime", 2.5)
                            |> runner.Encode.toString 0

                        runner.equal actual expected

                    runner.testCase "a tuple4 works"
                    <| fun _ ->
                        let expected = """[1,"maxime",2.5,{"fieldA":"test"}]"""

                        let actual =
                            Encode.tuple4
                                Encode.int
                                Encode.string
                                Encode.float
                                SmallRecord.Encoder
                                (1,
                                 "maxime",
                                 2.5,
                                 {
                                     fieldA = "test"
                                 })
                            |> runner.Encode.toString 0

                        runner.equal actual expected

                    runner.testCase "a tuple5 works"
                    <| fun _ ->
#if FABLE_COMPILER_JAVASCRIPT
                        let expected =
                            """[1,"maxime",2.5,{"fieldA":"test"},"2018-10-01T11:12:55.000Z"]"""
#endif

#if FABLE_COMPILER_PYTHON
                        let expected =
                            """[1,"maxime",2.5,{"fieldA":"test"},"2018-10-01T11:12:55.000000Z"]"""
#endif

#if !FABLE_COMPILER
                        let expected =
                            """[1,"maxime",2.5,{"fieldA":"test"},"2018-10-01T11:12:55.0000000Z"]"""
#endif

                        let actual =
                            Encode.tuple5
                                Encode.int
                                Encode.string
                                Encode.float
                                SmallRecord.Encoder
                                Encode.datetime
                                (1,
                                 "maxime",
                                 2.5,
                                 {
                                     fieldA = "test"
                                 },
                                 DateTime(
                                     2018,
                                     10,
                                     1,
                                     11,
                                     12,
                                     55,
                                     DateTimeKind.Utc
                                 ))
                            |> runner.Encode.toString 0

                        runner.equal actual expected

                    runner.testCase "a tuple6 works"
                    <| fun _ ->
                        let expected =
                            """[1,"maxime",2.5,{"fieldA":"test"},false,null]"""

                        let actual =
                            Encode.tuple6
                                Encode.int
                                Encode.string
                                Encode.float
                                SmallRecord.Encoder
                                Encode.bool
                                (fun _ -> Encode.nil)
                                (1,
                                 "maxime",
                                 2.5,
                                 {
                                     fieldA = "test"
                                 },
                                 false,
                                 null)
                            |> runner.Encode.toString 0

                        runner.equal actual expected

                    runner.testCase "a tuple7 works"
                    <| fun _ ->
                        let expected =
                            """[1,"maxime",2.5,{"fieldA":"test"},false,null,true]"""

                        let actual =
                            Encode.tuple7
                                Encode.int
                                Encode.string
                                Encode.float
                                SmallRecord.Encoder
                                Encode.bool
                                (fun _ -> Encode.nil)
                                Encode.bool
                                (1,
                                 "maxime",
                                 2.5,
                                 {
                                     fieldA = "test"
                                 },
                                 false,
                                 null,
                                 true)
                            |> runner.Encode.toString 0

                        runner.equal actual expected

                    runner.testCase "a tuple8 works"
                    <| fun _ ->
                        let expected =
                            """[1,"maxime",2.5,{"fieldA":"test"},false,null,true,98]"""

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
                                (1,
                                 "maxime",
                                 2.5,
                                 {
                                     fieldA = "test"
                                 },
                                 false,
                                 null,
                                 true,
                                 98)
                            |> runner.Encode.toString 0

                        runner.equal actual expected

                    runner.testCase "using pretty space works"
                    <| fun _ ->
                        let expected =
                            "{\n    \"firstname\": \"maxime\",\n    \"age\": 25\n}"

                        let actual =
                            Encode.object
                                [
                                    ("firstname", Encode.string "maxime")
                                    ("age", Encode.int 25)
                                ]
                            |> runner.Encode.toString 4

                        runner.equal actual expected

                    runner.testCase "complex structure works"
                    <| fun _ ->
                        let expected =
                            "{\n    \"firstname\": \"maxime\",\n    \"age\": 25,\n    \"address\": {\n        \"street\": \"main road\",\n        \"city\": \"Bordeaux\"\n    }\n}"

                        let actual =
                            Encode.object
                                [
                                    ("firstname", Encode.string "maxime")
                                    ("age", Encode.int 25)
                                    ("address",
                                     Encode.object
                                         [
                                             "street", Encode.string "main road"
                                             "city", Encode.string "Bordeaux"
                                         ])
                                ]
                            |> runner.Encode.toString 4

                        runner.equal actual expected

                    runner.testCase "option with a value `Some ...` works"
                    <| fun _ ->
                        let expected = """{"id":1,"operator":"maxime"}"""

                        let actual =
                            Encode.object
                                [
                                    ("id", Encode.int 1)
                                    ("operator",
                                     Encode.option Encode.string (Some "maxime"))
                                ]
                            |> runner.Encode.toString 0

                        runner.equal actual expected

                    runner.testCase "option without a value `None` works"
                    <| fun _ ->
                        let expected = """{"id":1,"operator":null}"""

                        let actual =
                            Encode.object
                                [
                                    ("id", Encode.int 1)
                                    ("operator",
                                     Encode.option Encode.string None)
                                ]
                            |> runner.Encode.toString 0

                        runner.equal actual expected

                //             runner.testCase "by default, we keep the case defined in type" <| fun _ ->
                //                 let expected =
                //                     """{"Id":0,"Name":"Maxime","Email":"mail@test.com","followers":33}"""
                //                 let value =
                //                     { Id = 0
                //                       Name = "Maxime"
                //                       Email = "mail@test.com"
                //                       followers = 33 }

                //                 let actual = Encode.Auto.toString(0, value)
                //                 runner.equal actual expected

                //             runner.testCase "force_snake_case works" <| fun _ ->
                //                 let expected =
                //                     """{"one":1,"two_part":2,"three_part_field":3}"""
                //                 let value = { One = 1; TwoPart = 2; ThreePartField = 3 }
                //                 let actual = Encode.Auto.toString(0, value, SnakeCase)
                //                 runner.equal actual expected

                //             runner.testCase "forceCamelCase works" <| fun _ ->
                //                 let expected =
                //                     """{"id":0,"name":"Maxime","email":"mail@test.com","followers":33}"""
                //                 let value =
                //                     { Id = 0
                //                       Name = "Maxime"
                //                       Email = "mail@test.com"
                //                       followers = 33 }

                //                 let actual = Encode.Auto.toString(0, value, CamelCase)
                //                 runner.equal actual expected

                //             runner.testCase "Encode.Auto.generateEncoder works" <| fun _ ->
                //                 let value =
                //                     {
                //                         a = 5
                //                         b = "bar"
                //                         c = [false, 3; true, 5; false, 10]
                //                         d = [|Some(Foo 14); None|]
                //                         e = Map [("oh", { a = 2.; b = 2. }); ("ah", { a = -1.5; b = 0. })]
                //                         f = DateTime(2018, 11, 28, 11, 10, 29, DateTimeKind.Utc)
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
                //                         s = 'z'
                //                         // s = seq [ "item n°1"; "item n°2"]
                //                     }
                //                 let extra =
                //                     Extra.empty
                //                     |> Extra.withInt64
                //                     |> Extra.withUInt64
                //                 let encoder = Encode.Auto.generateEncoder<Record9>(extra = extra)
                //                 let actual = encoder value |> runner.Encode.toString 0
                //                 let expected = """{"a":5,"b":"bar","c":[[false,3],[true,5],[false,10]],"d":[["Foo",14],null],"e":{"ah":{"a":-1.5,"b":0},"oh":{"a":2,"b":2}},"f":"2018-11-28T11:10:29Z","g":[{"a":-1.5,"b":0},{"a":2,"b":2}],"h":"00:00:05","i":120,"j":120,"k":250,"l":250,"m":99,"n":"99","o":"999","r":[[{"a":-2.5,"b":22.1},"value 2"],[{"a":1,"b":2},"value 1"]],"s":"z"}"""
                //                 // Don't fail because of non-meaningful decimal digits ("2" vs "2.0")
                //                 let actual = System.Text.RegularExpressions.Regex.Replace(actual, @"\.0+(?!\d)", "")
                //                 runner.equal actual expected

                //             runner.testCase "Encode.Auto.generateEncoderCached works" <| fun _ ->
                //                 let value =
                //                     {
                //                         a = 5
                //                         b = "bar"
                //                         c = [false, 3; true, 5; false, 10]
                //                         d = [|Some(Foo 14); None|]
                //                         e = Map [("oh", { a = 2.; b = 2. }); ("ah", { a = -1.5; b = 0. })]
                //                         f = DateTime(2018, 11, 28, 11, 10, 29, DateTimeKind.Utc)
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
                //                         s = 'z'
                //                         // s = seq [ "item n°1"; "item n°2"]
                //                     }
                //                 let extra =
                //                     Extra.empty
                //                     |> Extra.withInt64
                //                     |> Extra.withUInt64
                //                 let encoder1 = Encode.Auto.generateEncoderCached<Record9>(extra = extra)
                //                 let encoder2 = Encode.Auto.generateEncoderCached<Record9>(extra = extra)
                //                 let actual1 = encoder1 value |> runner.Encode.toString 0
                //                 let actual2 = encoder2 value |> runner.Encode.toString 0
                //                 let expected = """{"a":5,"b":"bar","c":[[false,3],[true,5],[false,10]],"d":[["Foo",14],null],"e":{"ah":{"a":-1.5,"b":0},"oh":{"a":2,"b":2}},"f":"2018-11-28T11:10:29Z","g":[{"a":-1.5,"b":0},{"a":2,"b":2}],"h":"00:00:05","i":120,"j":120,"k":250,"l":250,"m":99,"n":"99","o":"999","r":[[{"a":-2.5,"b":22.1},"value 2"],[{"a":1,"b":2},"value 1"]],"s":"z"}"""
                //                 // Don't fail because of non-meaningful decimal digits ("2" vs "2.0")
                //                 let actual1 = System.Text.RegularExpressions.Regex.Replace(actual1, @"\.0+(?!\d)", "")
                //                 let actual2 = System.Text.RegularExpressions.Regex.Replace(actual2, @"\.0+(?!\d)", "")
                //                 runner.equal actual expected 1
                //                 runner.equal actual expected 2
                //                 runner.equal actual1 actual2

                //             runner.testCase "Encode.Auto.toString emit null field if setted for" <| fun _ ->
                //                 let value = { fieldA = null }
                //                 let expected = """{"fieldA":null}"""
                //                 let actual = Encode.Auto.toString(0, value, skipNullField = false)
                //                 runner.equal actual expected

                //             runner.testCase "Encode.Auto.toString works with bigint extra" <| fun _ ->
                //                 let extra =
                //                     Extra.empty
                //                     |> Extra.withBigInt
                //                 let expected = """{"bigintField":"9999999999999999999999"}"""
                //                 let value = { bigintField = 9999999999999999999999I }
                //                 let actual = Encode.Auto.toString(0, value, extra=extra)
                //                 runner.equal actual expected

                //             runner.testCase "Encode.Auto.toString works with custom extra" <| fun _ ->
                //                 let extra =
                //                     Extra.empty
                //                     |> Extra.withCustom ChildType.Encode ChildType.Decoder
                //                 let expected = """{"ParentField":"bumbabon"}"""
                //                 let value = { ParentField = { ChildField = "bumbabon" } }
                //                 let actual = Encode.Auto.toString(0, value, extra=extra)
                //                 runner.equal actual expected

                //             runner.testCase "Encode.Auto.toString serializes maps with Guid keys as JSON objects" <| fun _ ->
                //                 let m = Map [Guid.NewGuid(), 1; Guid.NewGuid(), 2]
                //                 let json = Encode.Auto.toString(0, m)
                //                 json.[0] = '{' |> runner.equal true

                //             runner.testCase "Encode.Auto.toString works with records with private constructors" <| fun _ ->
                //                 let expected = """{"foo1":5,"foo2":7.8}"""
                //                 let x = { Foo1 = 5; Foo2 = 7.8 }: RecordWithPrivateConstructor
                //                 Encode.Auto.toString(0, x, caseStrategy=CamelCase)
                //                 |> runner.equal expected

                //             runner.testCase "Encode.Auto.toString works with unions with private constructors" <| fun _ ->
                //                 let expected = """["Baz",["Bar","foo"]]"""
                //                 let x = [Baz; Bar "foo"]
                //                 Encode.Auto.toString(0, x, caseStrategy=CamelCase)
                //                 |> runner.equal expected

                //             runner.testCase "Encode.Auto.toString works with strange types if they are None" <| fun _ ->
                //                 let expected =
                //                     """{"Id":0}"""

                //                 let value =
                //                     { Id = 0
                //                       Thread = None }

                //                 Encode.Auto.toString(0, value)
                //                 |> runner.equal expected

                //             runner.testCase "Encode.Auto.toString works with interfaces if they are None" <| fun _ ->
                //                 let expected =
                //                     """{"Id":0}"""

                //                 let value =
                //                     { Id = 0
                //                       Interface = None }

                //                 Encode.Auto.toString(0, value)
                //                 |> runner.equal expected

                //             runner.testCase "Encode.Auto.toString works with recursive types" <| fun _ ->
                //                 let vater =
                //                     { Name = "Alfonso"
                //                       Children = [ { Name = "Narumi"; Children = [] }
                //                                    { Name = "Takumi"; Children = [] } ] }
                //                 let json = """{"Name":"Alfonso","Children":[{"Name":"Narumi","Children":[]},{"Name":"Takumi","Children":[]}]}"""
                //                 Encode.Auto.toString(0, vater)
                //                 |> runner.equal json

                //             #if !NETFRAMEWORK
                //             runner.testCase "Encode.Auto.toString works with [<StringEnum>]" <| fun _ ->
                //                 let expected = "\"firstPerson\""
                //                 let actual = Encode.Auto.toString(0, Camera.FirstPerson)
                //                 runner.equal actual expected

                //             runner.testCase "Encode.Auto.toString works with [<StringEnum(CaseRules.LowerFirst)>]" <| fun _ ->
                //                 let expected = "\"react\""
                //                 let actual = Encode.Auto.toString(0, Framework.React)
                //                 runner.equal actual expected

                //             runner.testCase "Encode.Auto.toString works with [<StringEnum(CaseRules.None)>]" <| fun _ ->
                //                 let expected = "\"Fsharp\""
                //                 let actual = Encode.Auto.toString(0, Language.Fsharp)
                //                 runner.equal actual expected

                //             runner.testCase "Encode.Auto.toString works with [<StringEnum>] + [<CompiledName>]" <| fun _ ->
                //                 let expected = "\"C#\""
                //                 let actual = Encode.Auto.toString(0, Language.Csharp)
                //                 runner.equal actual expected
                //             #endif

                //             runner.testCase "Encode.Auto.toString works with normal Enums" <| fun _ ->
                //                 let expected = "2"
                //                 let actual = Encode.Auto.toString(0, Enum_Int.Two)
                //                 runner.equal actual expected

                //             runner.testCase "Encode.Auto.toString works with System.DayOfWeek" <| fun _ ->
                //                 let expected = "2"
                //                 let actual = Encode.Auto.toString(0, DayOfWeek.Tuesday)
                //                 runner.equal actual expected

                //             runner.testCase "Encode.Auto.toString generate `null` if skipNullField is true and the optional field value of type classes is None" <| fun _ ->
                //                 let value =
                //                     {
                //                         MaybeClass = None
                //                         Must = "must value"
                //                     } : RecordWithOptionalClass

                //                 let actual = Encode.Auto.toString(0, value, caseStrategy = CamelCase, skipNullField = false)
                //                 let expected =
                //                     """{"maybeClass":null,"must":"must value"}"""
                //                 runner.equal actual expected

                //             runner.testCase "Encode.Auto.toString doesn't generate the optional field of type classe if it's value is None" <| fun _ ->
                //                 let value =
                //                     {
                //                         MaybeClass = None
                //                         Must = "must value"
                //                     } : RecordWithOptionalClass

                //                 let actual = Encode.Auto.toString(0, value, caseStrategy = CamelCase)
                //                 let expected =
                //                     """{"must":"must value"}"""
                //                 runner.equal actual expected

                //             runner.testCase "Encode.Auto.generateEncoder throws for field using a non optional class" <| fun _ ->
                //                 let expected = """Cannot generate auto encoder for Tests.Types.BaseClass. Please pass an extra encoder.

                // Documentation available at: https://thoth-org.github.io/Thoth.Json/documentation/auto/extra-coders.html#ready-to-use-extra-coders"""

                //                 let errorMsg =
                //                     try
                //                         let encoder = Encode.Auto.generateEncoder<RecordWithRequiredClass>(caseStrategy = CamelCase)
                //                         ""
                //                     with ex ->
                //                         ex.Message
                //                 errorMsg.Replace("+", ".") |> runner.equal expected

                //             runner.testCase "Encode.Auto allows to re-define primitive types" <| fun _ ->
                //                 let customIntEncoder (value : int) =
                //                     Encode.object [
                //                         "type", Encode.string "customInt"
                //                         "value", Encode.int value
                //                     ]

                //                 let customIntDecoder =
                //                     Decode.field "type" Decode.string
                //                     |> Decode.andThen (function
                //                         | "customInt" ->
                //                             Decode.field "value" Decode.int

                //                         | invalid ->
                //                             Decode.fail "Invalid type for customInt"
                //                     )

                //                 let extra =
                //                     Extra.empty
                //                     |> Extra.withCustom customIntEncoder customIntDecoder

                //                 let actual = Encode.Auto.toString(0, 42, extra=extra)

                //                 let expected =
                //                     """{"type":"customInt","value":42}"""

                //                 runner.equal actual expected

                //             runner.testCase "Encode.Auto.toString(value, ...) is equivalent to Encode.Auto.toString(0, value, ...)" <| fun _ ->
                //                 let expected = Encode.Auto.toString(0, {| Name = "Maxime" |})
                //                 let actual = Encode.Auto.toString({| Name = "Maxime" |})
                //                 runner.equal actual expected

                (*
#if NETFRAMEWORK
            runner.testCase "Encode.Auto.toString works with char based Enums" <| fun _ ->
                let expected = ((int) 'A').ToString()  // "65"
                let actual = Encode.Auto.toString(0, CharEnum.A)
                runner.equal actual expected
#endif
    *)
                ]

        ]
