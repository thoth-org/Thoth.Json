module Tests.Encoders.Auto

#if !NETFRAMEWORK
open Fable.Core
#endif

#if FABLE_COMPILER
open Thoth.Json
open Fable.Mocha
open Fable.Core.JsInterop
#endif

#if !FABLE_COMPILER
open Thoth.Json
open Expecto
#endif

open Tests.Types
open System

type RecordWithPrivateConstructor = private { Foo1: int; Foo2: float }

type UnionWithPrivateConstructor = private Bar of string | Baz

let tests =
    testList "Thoth.Json.Encode" [
        testList "Encode.Auto" [
            testCase "by default, we keep the case defined in type" <| fun _ ->
                let expected =
                    """{"Email":"mail@test.com","Id":0,"Name":"Maxime","followers":33}"""

                let value : UserCaseSensitive =
                    {
                        Email = "mail@test.com"
                        followers = 33
                        Id = 0
                        Name = "Maxime"
                    }

                let actual = Encode.Auto.toString(0, value)
                Expect.equal actual expected ""

            testCase "force_snake_case works" <| fun _ ->
                let expected =
                    """{"one":1,"three_part_field":3,"two_part":2}"""
                let value = { One = 1; ThreePartField = 3; TwoPart = 2 }
                let actual = Encode.Auto.toString(0, value, SnakeCase)
                Expect.equal actual expected ""

            testCase "forceCamelCase works" <| fun _ ->
                let expected =
                    """{"email":"mail@test.com","followers":33,"id":0,"name":"Maxime"}"""
                let value : UserCaseSensitive =
                    {
                        Email = "mail@test.com"
                        followers = 33
                        Id = 0
                        Name = "Maxime"
                    }

                let actual = Encode.Auto.toString(0, value, CamelCase)
                Expect.equal actual expected ""

            testCase "Encode.Auto.generateEncoder works" <| fun _ ->
                let value =
                    {
                        a = 5
                        b = "bar"
                        c = [false, 3; true, 5; false, 10]
                        d = [|Some(Foo 14); None|]
                        e = Map [("oh", { a = 2.; b = 2. }); ("ah", { a = -1.5; b = 0. })]
                        f = DateTime(2018, 11, 28, 11, 10, 29, DateTimeKind.Utc)
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
                        r = 'p'
                        s = Guid("2e053897-15a9-4647-a005-e954666e24d3")
                        t = seq [ "item n°1"; "item n°2"]
                        u = SeveralArgs (10, {| Name = "maxime"; Age = 28 |})
                    }
                let extra =
                    Extra.empty
                    |> Extra.withInt64
                    |> Extra.withUInt64
                let encoder = Encode.Auto.generateEncoder<Record9>(extra = extra)
                let actual = encoder value |> Encode.toString 0
                let expected = """{"a":5,"b":"bar","c":[[false,3],[true,5],[false,10]],"d":[14,null],"e":{"ah":{"a":-1.5,"b":0},"oh":{"a":2,"b":2}},"f":"2018-11-28T11:10:29Z","g":[{"a":-1.5,"b":0},{"a":2,"b":2}],"h":"00:00:05","i":"120","j":"120","k":"250","l":"250","m":99,"n":"99","o":"999","r":"p","s":"2e053897-15a9-4647-a005-e954666e24d3","t":["item n°1","item n°2"]}"""
                // Don't fail because of non-meaningful decimal digits ("2" vs "2.0")
                let actual = System.Text.RegularExpressions.Regex.Replace(actual, @"\.0+(?!\d)", "")
                Expect.equal actual expected ""

            testCase "Encode.Auto.generateEncoderCached works" <| fun _ ->
                let value =
                    {
                        a = 5
                        b = "bar"
                        c = [false, 3; true, 5; false, 10]
                        d = [|Some(Foo 14); None|]
                        e = Map [("oh", { a = 2.; b = 2. }); ("ah", { a = -1.5; b = 0. })]
                        f = DateTime(2018, 11, 28, 11, 10, 29, DateTimeKind.Utc)
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
                        r = 'p'
                        s = Guid("2e053897-15a9-4647-a005-e954666e24d3")
                        t = seq [ "item n°1"; "item n°2"]
                        u = SeveralArgs (10, {| Name = "maxime"; Age = 28 |})
                    }
                let extra =
                    Extra.empty
                    |> Extra.withInt64
                    |> Extra.withUInt64
                let encoder1 = Encode.Auto.generateEncoderCached<Record9>(extra = extra)
                let encoder2 = Encode.Auto.generateEncoderCached<Record9>(extra = extra)
                let actual1 = encoder1 value |> Encode.toString 0
                let actual2 = encoder2 value |> Encode.toString 0
                let expected = """{"a":5,"b":"bar","c":[[false,3],[true,5],[false,10]],"d":[14,null],"e":{"ah":{"a":-1.5,"b":0},"oh":{"a":2,"b":2}},"f":"2018-11-28T11:10:29Z","g":[{"a":-1.5,"b":0},{"a":2,"b":2}],"h":"00:00:05","i":"120","j":"120","k":"250","l":"250","m":99,"n":"99","o":"999","r":"p","s":"2e053897-15a9-4647-a005-e954666e24d3","t":["item n°1","item n°2"]}"""
                // Don't fail because of non-meaningful decimal digits ("2" vs "2.0")
                let actual1 = System.Text.RegularExpressions.Regex.Replace(actual1, @"\.0+(?!\d)", "")
                let actual2 = System.Text.RegularExpressions.Regex.Replace(actual2, @"\.0+(?!\d)", "")
                Expect.equal expected actual1 ""
                Expect.equal expected actual2 ""
                Expect.equal actual1 actual2 ""

            testCase "Encode.Auto.toString emit null field if set for" <| fun _ ->
                let value = { fieldA = null }
                let expected = """{"fieldA":null}"""
                let actual = Encode.Auto.toString(0, value, skipNullField = false)
                Expect.equal actual expected ""

            testCase "Encode.Auto.toString works with bigint extra" <| fun _ ->
                let extra =
                    Extra.empty
                    |> Extra.withBigInt
                let expected = """{"bigintField":"9999999999999999999999"}"""
                let value = { bigintField = 9999999999999999999999I }
                let actual = Encode.Auto.toString(0, value, extra=extra)
                Expect.equal actual expected ""

            testCase "Encode.Auto.toString works with custom extra" <| fun _ ->
                let extra =
                    Extra.empty
                    |> Extra.withCustom ChildType.Encode ChildType.Decoder
                let expected = """{"ParentField":"bumbabon"}"""
                let value = { ParentField = { ChildField = "bumbabon" } }
                let actual = Encode.Auto.toString(0, value, extra=extra)
                Expect.equal actual expected ""

            testCase "Encode.Auto.toString serializes maps with Guid keys as JSON objects" <| fun _ ->
                let m = Map [Guid.NewGuid(), 1; Guid.NewGuid(), 2]
                let json = Encode.Auto.toString(0, m)
                Expect.equal true (json.[0] = '{') ""

            testCase "Encode.Auto.toString works with records with private constructors" <| fun _ ->
                let expected = """{"foo1":5,"foo2":7.8}"""
                let x = { Foo1 = 5; Foo2 = 7.8 }: RecordWithPrivateConstructor
                let actual = Encode.Auto.toString(0, x, caseStrategy=CamelCase)
                Expect.equal actual expected ""

            testCase "Encode.Auto.toString works with unions with private constructors" <| fun _ ->
                let expected = """["Baz",["Bar","foo"]]"""
                let x = [Baz; Bar "foo"]
                let actual = Encode.Auto.toString(0, x, caseStrategy=CamelCase)
                Expect.equal actual expected ""

            testCase "Encode.Auto.toString works with strange types if they are None" <| fun _ ->
                let expected =
                    """{"Id":0}"""

                let value =
                    { Id = 0
                      Thread = None }

                let actual = Encode.Auto.toString(0, value)
                Expect.equal actual expected ""

            testCase "Encode.Auto.toString works with interfaces if they are None" <| fun _ ->
                let expected =
                    """{"Id":0}"""

                let value =
                    { Id = 0
                      Interface = None }

                let actual = Encode.Auto.toString(0, value)
                Expect.equal actual expected ""

            testCase "Encode.Auto.toString works with recursive types" <| fun _ ->
                let vater =
                    {
                        Children =
                            [
                                { Children = [] ; Name = "Narumi" }
                                { Children = [] ; Name = "Takumi" }
                            ]
                        Name = "Alfonso"
                    }

                let json = """{"Children":[{"Children":[],"Name":"Narumi"},{"Children":[],"Name":"Takumi"}],"Name":"Alfonso"}"""
                let actual = Encode.Auto.toString(0, vater)
                Expect.equal actual json ""

            #if !NETFRAMEWORK
            testCase "Encode.Auto.toString works with [<StringEnum>]" <| fun _ ->
                let expected = "\"firstPerson\""
                let actual = Encode.Auto.toString(0, Camera.FirstPerson)
                Expect.equal actual expected ""

            testCase "Encode.Auto.toString works with [<StringEnum(CaseRules.LowerFirst)>]" <| fun _ ->
                let expected = "\"react\""
                let actual = Encode.Auto.toString(0, Framework.React)
                Expect.equal actual expected ""

            testCase "Encode.Auto.toString works with [<StringEnum(CaseRules.None)>]" <| fun _ ->
                let expected = "\"Fsharp\""
                let actual = Encode.Auto.toString(0, Language.Fsharp)
                Expect.equal actual expected ""

            testCase "Encode.Auto.toString works with [<StringEnum>] + [<CompiledName>]" <| fun _ ->
                let expected = "\"C#\""
                let actual = Encode.Auto.toString(0, Language.Csharp)
                Expect.equal actual expected ""
            #endif

            testCase "Encode.Auto.toString works with normal Enums" <| fun _ ->
                let expected = "2"
                let actual = Encode.Auto.toString(0, Enum_Int.Two)
                Expect.equal actual expected ""

            testCase "Encode.Auto.toString works with System.DayOfWeek" <| fun _ ->
                let expected = "2"
                let actual = Encode.Auto.toString(0, DayOfWeek.Tuesday)
                Expect.equal actual expected ""

            testCase "Encode.Auto.toString generate `null` if skipNullField is true and the optional field value of type classes is None" <| fun _ ->
                let value =
                    {
                        MaybeClass = None
                        Must = "must value"
                    } : RecordWithOptionalClass

                let actual = Encode.Auto.toString(0, value, caseStrategy = CamelCase, skipNullField = false)
                let expected =
                    """{"maybeClass":null,"must":"must value"}"""
                Expect.equal actual expected ""

            testCase "Encode.Auto.toString doesn't generate the optional field of type classe if it's value is None" <| fun _ ->
                let value =
                    {
                        MaybeClass = None
                        Must = "must value"
                    } : RecordWithOptionalClass

                let actual = Encode.Auto.toString(0, value, caseStrategy = CamelCase)
                let expected =
                    """{"must":"must value"}"""
                Expect.equal actual expected ""

            testCase "Encode.Auto.generateEncoder throws for field using a non optional class" <| fun _ ->
                let expected = "Cannot generate auto encoder for Tests.Types.BaseClass. Please pass an extra coder."
                let errorMsg =
                    try
                        let encoder = Encode.Auto.generateEncoder<RecordWithRequiredClass>(caseStrategy = CamelCase)
                        ""
                    with ex ->
                        ex.Message
                let actual = errorMsg.Replace("+", ".")

                Expect.equal actual expected ""

            (*
                    #if NETFRAMEWORK
                    testCase "Encode.Auto.toString works with char based Enums" <| fun _ ->
                        let expected = ((int) 'A').ToString()  // "65"
                        let actual = Encode.Auto.toString(0, CharEnum.A)
                        Expect.equal actual expected ""
                    #endif
            *)

            testCase "Encode.Auto.toString allows to customize default known types" <| fun _ ->
                let customizedIntEncoder (value : int) =
                    Encode.object [
                        "type", Encode.string "int"
                        "value", Encode.int value
                    ]

                let customizedIntDecoder =
                    Decode.field "type" Decode.string
                    |> Decode.andThen(function
                    | "int" ->
                        Decode.field "value" Decode.int
                    | invalid ->
                        sprintf "`%s` is not a valid type value for customizedInt" invalid
                        |> Decode.fail
                    )

                let extra =
                    Extra.empty
                    |> Extra.withCustom customizedIntEncoder customizedIntDecoder

                let json = Encode.Auto.toString(0, 99, extra=extra)

                Expect.equal json "{\"type\":\"int\",\"value\":99}" ""

// It doesn't work on Fable because F# doesn't generate type information for erased types
//            testCase "Erased single-case DUs works" <| fun _ ->
//                let expected = "\"2e053897-15a9-4647-a005-e954666e24d3\""
//                let actual = Encode.Auto.toString(4, NoAllocAttributeSingleCaseDU (Guid("2e053897-15a9-4647-a005-e954666e24d3")))
//                Expect.equal actual expected ""

            testCase "Single case unions generates simplify JSON"  <| fun _ ->
                let expected = "\"Maxime\""
                let actual = Encode.Auto.toString(4, SingleCaseDUSimple "Maxime")
                Expect.equal actual expected ""

            testCase "Single case unions generates simplify JSON and works with complex types" <| fun _ ->
                let expected =
                    """
{
    "Age": 28,
    "FirstName": "Maxime"
}
                    """.Trim()
                let actual = Encode.Auto.toString(4, SingleCaseDUComplex {| FirstName = "Maxime"; Age = 28 |})
                Expect.equal actual expected ""

            testCase "Union case with several case are encoding as array or string" <| fun _ ->
                let circleExpected =
                    """
[
    "Circle",
    10
]
                    """.Trim()
                let circleActual = Encode.Auto.toString(4, Shape.Circle 10)
                Expect.equal circleActual circleExpected ""

                let voidExpected = "\"Void\""
                let voidActual = Encode.Auto.toString(4, Shape.Void)
                Expect.equal voidActual voidExpected ""

            testCase "Encode.Auto.toString serializes mutable hashsets" <| fun _ ->
                let s = System.Collections.Generic.HashSet()
                s.Add(1) |> ignore
                s.Add(1) |> ignore
                s.Add(2) |> ignore
                let actual = Encode.Auto.toString(0, s)
                Expect.equal actual """[1,2]""" ""

            testCase "Encode.Auto.toString works with seq" <| fun _ ->
                let value = seq { yield 1; yield 2 }
                let expected = """[1,2]"""
                let actual = Encode.Auto.toString(0, value, skipNullField = false)
                Expect.equal actual expected ""

            testCase "Encode.Auto.toString serializes mutable dictionaries" <| fun _ ->
                let d = System.Collections.Generic.Dictionary()
                d.Add("Bar", 2)
                d.Add("Foo", 1)
                let actual = Encode.Auto.toString(0, d)
                Expect.equal actual """{"Bar":2,"Foo":1}""" ""

            testCase "Encode.Auto.toString serializes mutable dictionaries with non simple keys" <| fun _ ->
                let d = System.Collections.Generic.Dictionary()
                d.Add({| ComplexKey = 1 |}, 1)
                d.Add({| ComplexKey = 2 |}, 2)
                let actual = Encode.Auto.toString(0, d)
                Expect.equal actual """[[{"ComplexKey":1},1],[{"ComplexKey":2},2]]""" ""

            testCase "Encode.Auto.toString works the same for option as for other DUs" <| fun _ ->
                // Check Some case of primitive type
                let realOptionSome : Option<_> = Some "maxime"
                let fakeOptionSome : Fake.FakeOption<_> = Fake.FakeOption.Some "maxime"

                let realOptionSomeJson = Encode.Auto.toString(0, realOptionSome)
                let fakeOptionSomeJson = Encode.Auto.toString(0, fakeOptionSome)

                Expect.equal realOptionSomeJson fakeOptionSomeJson ""

                // Check None case
                let realOptionNone : Option<_> = None
                let fakeOptionNone : Fake.FakeOption<_> = Fake.FakeOption.None

                let realOptionNoneJson = Encode.Auto.toString(0, realOptionNone)
                let fakeOptionNoneJson = Encode.Auto.toString(0, fakeOptionNone)

                Expect.equal realOptionNoneJson fakeOptionNoneJson ""

                // Check Some case of complex type
                let realOptionSomeComplex : Option<_> = Some {| Firstname = "maxime"; Age = 28 |}
                let fakeOptionSomeComplex : Fake.FakeOption<_> = Fake.FakeOption.Some {| Firstname = "maxime"; Age = 28 |}

                let realOptionSomeComplexJson = Encode.Auto.toString(0, realOptionSomeComplex)
                let fakeOptionSomeComplexJson = Encode.Auto.toString(0, fakeOptionSomeComplex)

                Expect.equal realOptionSomeComplexJson fakeOptionSomeComplexJson ""
        ]
    ]
