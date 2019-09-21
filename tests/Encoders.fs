module Tests.Encoders

open Thoth.Json
open Util.Testing
open System
open Tests.Types

type RecordWithPrivateConstructor = private { Foo1: int; Foo2: float }
type UnionWithPrivateConstructor = private Bar of string | Baz

let tests : Test =
    testList "Thoth.Json.Encode" [

        testList "Basic" [

            testCase "a string works" <| fun _ ->
                let expected = "\"maxime\""
                let actual =
                    Encode.string "maxime"
                    |> Encode.toString 0
                equal expected actual

            testCase "an int works" <| fun _ ->
                let expected = "1"
                let actual =
                    Encode.int 1
                    |> Encode.toString 0
                equal expected actual

            testCase "a float works" <| fun _ ->
                let expected = "1.2"
                let actual =
                    Encode.float 1.2
                    |> Encode.toString 0
                equal expected actual

            testCase "an array works" <| fun _ ->
                let expected =
                    """["maxime",2]"""
                let actual =
                    Encode.array
                        [| Encode.string "maxime"
                           Encode.int 2
                        |] |> Encode.toString 0
                equal expected actual

            testCase "a list works" <| fun _ ->
                let expected =
                    """["maxime",2]"""
                let actual =
                    Encode.list
                        [ Encode.string "maxime"
                          Encode.int 2
                        ] |> Encode.toString 0
                equal expected actual

            testCase "a bool works" <| fun _ ->
                let expected = "false"
                let actual =
                    Encode.bool false
                    |> Encode.toString 0
                equal expected actual

            testCase "a null works" <| fun _ ->
                let expected = "null"
                let actual =
                    Encode.nil
                    |> Encode.toString 0
                equal expected actual

            testCase "unit works" <| fun _ ->
                let expected = ""
                let actual =
                    Encode.unit ()
                    |> Encode.toString 0
                equal expected actual

            testCase "an object works" <| fun _ ->
                let expected =
                    """{"firstname":"maxime","age":25}"""
                let actual =
                    Encode.object
                        [ ("firstname", Encode.string "maxime")
                          ("age", Encode.int 25)
                        ] |> Encode.toString 0
                equal expected actual

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
                equal expected actual

            testCase "a bigint works" <| fun _ ->
                let expected = "\"12\""
                let actual =
                    Encode.bigint 12I
                    |> Encode.toString 0

                equal expected actual

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

                equal expected actual

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

                equal expected actual

            testCase "a timeSpan works" <| fun _ ->
                let expected = "\"1.02:03:04.0050000\""
                let actual =
                    TimeSpan(1, 2, 3, 4, 5)
                    |> Encode.timespan
                    |> Encode.toString 0

                equal expected actual

            testCase "a decimal works" <| fun _ ->
                let expected = "\"0.7833\""
                let actual =
                    0.7833M
                    |> Encode.decimal
                    |> Encode.toString 0

                equal expected actual

            testCase "a guid works" <| fun _ ->
                let expected = "\"1e5dee25-8558-4392-a9fb-aae03f81068f\""
                let actual =
                    Guid.Parse("1e5dee25-8558-4392-a9fb-aae03f81068f")
                    |> Encode.guid
                    |> Encode.toString 0

                equal expected actual

            testCase "an int64 works" <| fun _ ->
                let expected = "\"7923209\""
                let actual =
                    7923209L
                    |> Encode.int64
                    |> Encode.toString 0

                equal expected actual

            testCase "an uint64 works" <| fun _ ->
                let expected = "\"7923209\""
                let actual =
                    7923209UL
                    |> Encode.uint64
                    |> Encode.toString 0

                equal expected actual

            testCase "a tuple2 works" <| fun _ ->
                let expected = """[1,"maxime"]"""
                let actual =
                    Encode.tuple2
                        Encode.int
                        Encode.string
                        (1, "maxime")
                    |> Encode.toString 0

                equal expected actual

            testCase "a tuple3 works" <| fun _ ->
                let expected = """[1,"maxime",2.5]"""
                let actual =
                    Encode.tuple3
                        Encode.int
                        Encode.string
                        Encode.float
                        (1, "maxime", 2.5)
                    |> Encode.toString 0

                equal expected actual

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

                equal expected actual

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

                equal expected actual

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

                equal expected actual

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

                equal expected actual

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

                equal expected actual

            testCase "using pretty space works" <| fun _ ->
                let expected = "{\n    \"firstname\": \"maxime\",\n    \"age\": 25\n}"

                let actual =
                    Encode.object
                        [ ("firstname", Encode.string "maxime")
                          ("age", Encode.int 25)
                        ] |> Encode.toString 4
                equal expected actual

            testCase "complexe structure works" <| fun _ ->
                let expected =
                    "{\n    \"firstname\": \"maxime\",\n    \"age\": 25,\n    \"address\": {\n        \"street\": \"main road\",\n        \"city\": \"Bordeaux\"\n    }\n}"

                let actual =
                    Encode.object
                        [ ("firstname", Encode.string "maxime")
                          ("age", Encode.int 25)
                          ("address", Encode.object
                                        [ "street", Encode.string "main road"
                                          "city", Encode.string "Bordeaux"
                                        ])
                        ] |> Encode.toString 4
                equal expected actual

            testCase "option with a value `Some ...` works" <| fun _ ->
                let expected = """{"id":1,"operator":"maxime"}"""

                let actual =
                    Encode.object
                        [ ("id", Encode.int 1)
                          ("operator", Encode.option Encode.string (Some "maxime"))
                        ] |> Encode.toString 0

                equal expected actual

            testCase "option without a value `None` works" <| fun _ ->
                let expected = """{"id":1,"operator":null}"""

                let actual =
                    Encode.object
                        [ ("id", Encode.int 1)
                          ("operator", Encode.option Encode.string None)
                        ] |> Encode.toString 0

                equal expected actual

            testCase "by default, we keep the case defined in type" <| fun _ ->
                let expected =
                    """{"Id":0,"Name":"Maxime","Email":"mail@test.com","followers":33}"""
                let value =
                    { Id = 0
                      Name = "Maxime"
                      Email = "mail@test.com"
                      followers = 33 }

                let actual = Encode.Auto.toString(0, value)
                equal expected actual

            testCase "forceCamelCase works" <| fun _ ->
                let expected =
                    """{"id":0,"name":"Maxime","email":"mail@test.com","followers":33}"""
                let value =
                    { Id = 0
                      Name = "Maxime"
                      Email = "mail@test.com"
                      followers = 33 }

                let actual = Encode.Auto.toString(0, value, true)
                equal expected actual

            testCase "Encode.Auto.generateEncoder works" <| fun _ ->
                let value =
                    { a = 5
                      b = "bar"
                      c = [false, 3; true, 5; false, 10]
                      d = [|Some(Foo 14); None|]
                      e = Map [("oh", { a = 2.; b = 2. }); ("ah", { a = -1.5; b = 0. })]
                      f = DateTime(2018, 11, 28, 11, 10, 29, DateTimeKind.Utc)
                      g = set [{ a = 2.; b = 2. }; { a = -1.5; b = 0. }]
                      h = TimeSpan.FromSeconds(5.)
                    }
                let encoder = Encode.Auto.generateEncoder<Record9>()
                let actual = encoder value |> Encode.toString 0
                let expected = """{"a":5,"b":"bar","c":[[false,3],[true,5],[false,10]],"d":[["Foo",14],null],"e":{"ah":{"a":-1.5,"b":0},"oh":{"a":2,"b":2}},"f":"2018-11-28T11:10:29Z","g":[{"a":-1.5,"b":0},{"a":2,"b":2}],"h":"00:00:05"}"""
                // Don't fail because of non-meaningful decimal digits ("2" vs "2.0")
                let actual = System.Text.RegularExpressions.Regex.Replace(actual, @"\.0+(?!\d)", "")
                equal expected actual

            testCase "Encode.Auto.generateEncoderCached works" <| fun _ ->
                let value =
                    { a = 5
                      b = "bar"
                      c = [false, 3; true, 5; false, 10]
                      d = [|Some(Foo 14); None|]
                      e = Map [("oh", { a = 2.; b = 2. }); ("ah", { a = -1.5; b = 0. })]
                      f = DateTime(2018, 11, 28, 11, 10, 29, DateTimeKind.Utc)
                      g = set [{ a = 2.; b = 2. }; { a = -1.5; b = 0. }]
                      h = TimeSpan.FromSeconds(5.)
                    }
                let encoder1 = Encode.Auto.generateEncoderCached<Record9>()
                let encoder2 = Encode.Auto.generateEncoderCached<Record9>()
                let actual1 = encoder1 value |> Encode.toString 0
                let actual2 = encoder2 value |> Encode.toString 0
                let expected = """{"a":5,"b":"bar","c":[[false,3],[true,5],[false,10]],"d":[["Foo",14],null],"e":{"ah":{"a":-1.5,"b":0},"oh":{"a":2,"b":2}},"f":"2018-11-28T11:10:29Z","g":[{"a":-1.5,"b":0},{"a":2,"b":2}],"h":"00:00:05"}"""
                // Don't fail because of non-meaningful decimal digits ("2" vs "2.0")
                let actual1 = System.Text.RegularExpressions.Regex.Replace(actual1, @"\.0+(?!\d)", "")
                let actual2 = System.Text.RegularExpressions.Regex.Replace(actual2, @"\.0+(?!\d)", "")
                equal expected actual1
                equal expected actual2
                equal actual1 actual2

            testCase "Encode.Auto.toString works with bigint extra" <| fun _ ->
                let extra =
                    Extra.empty
                    |> Extra.withBigInt
                let expected = """{"bigintField":"9999999999999999999999"}"""
                let value = { bigintField = 9999999999999999999999I }
                let actual = Encode.Auto.toString(0, value, extra=extra)
                equal expected actual

            testCase "Encode.Auto.toString works with custom extra" <| fun _ ->
                let extra =
                    Extra.empty
                    |> Extra.withCustom ChildType.Encode ChildType.Decoder
                let expected = """{"ParentField":"bumbabon"}"""
                let value = { ParentField = { ChildField = "bumbabon" } }
                let actual = Encode.Auto.toString(0, value, extra=extra)
                equal expected actual

            testCase "Encode.Auto.toString serializes maps with Guid keys as JSON objects" <| fun _ ->
                let m = Map [Guid.NewGuid(), 1; Guid.NewGuid(), 2]
                let json = Encode.Auto.toString(0, m)
                json.[0] = '{' |> equal true

            testCase "Encode.Auto.toString works with records with private constructors" <| fun _ ->
                let expected = """{"foo1":5,"foo2":7.8}"""
                let x = { Foo1 = 5; Foo2 = 7.8 }: RecordWithPrivateConstructor
                Encode.Auto.toString(0, x, isCamelCase=true)
                |> equal expected

            testCase "Encode.Auto.toString works with unions with private constructors" <| fun _ ->
                let expected = """["Baz",["Bar","foo"]]"""
                let x = [Baz; Bar "foo"]
                Encode.Auto.toString(0, x, isCamelCase=true)
                |> equal expected

            testCase "Encode.Auto.toString works with strange types if they are None" <| fun _ ->
                let expected =
                    """{"Id":0}"""

                let value =
                    { Id = 0
                      Thread = None }

                Encode.Auto.toString(0, value)
                |> equal expected

            testCase "Encode.Auto.toString works with interfaces if they are None" <| fun _ ->
                let expected =
                    """{"Id":0}"""

                let value =
                    { Id = 0
                      Interface = None }

                Encode.Auto.toString(0, value)
                |> equal expected

            testCase "Encode.Auto.toString works with recursive types" <| fun _ ->
                let vater =
                    { Name = "Alfonso"
                      Children = [ { Name = "Narumi"; Children = [] }
                                   { Name = "Takumi"; Children = [] } ] }
                let json = """{"Name":"Alfonso","Children":[{"Name":"Narumi","Children":[]},{"Name":"Takumi","Children":[]}]}"""
                Encode.Auto.toString(0, vater)
                |> equal json

            #if !NETFRAMEWORK
            testCase "Encode.Auto.toString works with [<StringEnum>]" <| fun _ ->
                let expected = "\"firstPerson\""
                let actual = Encode.Auto.toString(0, Camera.FirstPerson)
                equal expected actual

            testCase "Encode.Auto.toString works with [<StringEnum(CaseRules.LowerFirst)>]" <| fun _ ->
                let expected = "\"react\""
                let actual = Encode.Auto.toString(0, Framework.React)
                equal expected actual

            testCase "Encode.Auto.toString works with [<StringEnum(CaseRules.None)>]" <| fun _ ->
                let expected = "\"Fsharp\""
                let actual = Encode.Auto.toString(0, Language.Fsharp)
                equal expected actual

            testCase "Encode.Auto.toString works with [<StringEnum>] + [<CompiledName>]" <| fun _ ->
                let expected = "\"C#\""
                let actual = Encode.Auto.toString(0, Language.Csharp)
                equal expected actual
            #endif
            
            testCase "Encode.Auto.toString works with normal Enums" <| fun _ ->
                let expected = "2"
                let actual = Encode.Auto.toString(0, IntEnum.Two)
                equal expected actual
          
            testCase "Encode.Auto.toString works with System.DayOfWeek" <| fun _ ->
                let expected = "2"
                let actual = Encode.Auto.toString(0, DayOfWeek.Tuesday)
                equal expected actual
    (* 
            #if NETFRAMEWORK
            testCase "Encode.Auto.toString works with char based Enums" <| fun _ ->
                let expected = ((int) 'A').ToString()  // "65"
                let actual = Encode.Auto.toString(0, CharEnum.A)
                equal expected actual
            #endif 
    *)        
      ]

    ]
