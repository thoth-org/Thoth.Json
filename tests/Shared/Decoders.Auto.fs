module Tests.Decoders.Auto

#if THOTH_JSON_FABLE
open Thoth.Json.Fable
open Fable.Mocha
open Fable.Core.JsInterop
#endif

#if THOTH_JSON_NEWTONSOFT
open Thoth.Json.Newtonsoft
open Expecto
open Fable.Core
#endif

open Tests.Types
open System

type RecordWithPrivateConstructor = private { Foo1: int; Foo2: float }

type UnionWithPrivateConstructor = private Bar of string | Baz

let tests =
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
                    r = 'r'
                    // r = seq [ "item n°1"; "item n°2"]
                }
            let extra =
                Extra.empty
                |> Extra.withInt64
                |> Extra.withUInt64
            let json = Encode.Auto.toString(4, value, extra = extra)
            // printfn "AUTO ENCODED %s" json
            let r2 = Decode.Auto.unsafeFromString<Record9>(json, extra = extra)
            Expect.equal 5 r2.a ""
            Expect.equal "bar" r2.b ""
            Expect.equal [false, 3; true, 5; false, 10] r2.c ""
            Expect.equal (Some(Foo 14)) r2.d.[0] ""
            Expect.equal None r2.d.[1] ""
            Expect.equal -1.5 (Map.find "ah" r2.e).a ""
            Expect.equal 2.   (Map.find "oh" r2.e).b ""
            Expect.equal (now.ToString())  (value.f.ToString()) ""
            Expect.equal true (Set.contains { a = -1.5; b = 0. } r2.g) ""
            Expect.equal false (Set.contains { a = 1.5; b = 0. } r2.g) ""
            Expect.equal 5000. value.h.TotalMilliseconds ""
            Expect.equal 120y r2.i ""
            Expect.equal 120uy r2.j ""
            Expect.equal 250s r2.k ""
            Expect.equal 250us r2.l ""
            Expect.equal 99u r2.m ""
            Expect.equal 99L r2.n ""
            Expect.equal 999UL r2.o ""
            Expect.equal () r2.p ""
            Expect.equal 'r' r2.r ""
            // Expect.equal ((seq [ "item n°1"; "item n°2"]) |> Seq.toList) (r2.r |> Seq.toList) ""

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
            Expect.equal (len li2) 3 ""
            let actual =
                match li with
                | Cons(i1, Cons(i2, Cons(i3, Nil))) -> i1 + i2 + i3
                | Cons(i,_) -> i
                | Nil -> 0
            Expect.equal actual 6 ""

        testCase "Auto decoders works for string" <| fun _ ->
            let value = "maxime"
            let json = Encode.Auto.toString(4, value)
            let res = Decode.Auto.unsafeFromString<string>(json)
            Expect.equal res value ""

        testCase "Auto decoders works for guid" <| fun _ ->
            let value = Guid.NewGuid()
            let json = Encode.Auto.toString(4, value)
            let res = Decode.Auto.unsafeFromString<Guid>(json)
            Expect.equal res value ""

        testCase "Auto decoders works for int" <| fun _ ->
            let value = 12
            let json = Encode.Auto.toString(4, value)
            let res = Decode.Auto.unsafeFromString<int>(json)
            Expect.equal res value ""

        testCase "Auto decoders works for int64" <| fun _ ->
            let extra = Extra.empty |> Extra.withInt64
            let value = 9999999999L
            let json = Encode.Auto.toString(4, value, extra=extra)
            let res = Decode.Auto.unsafeFromString<int64>(json, extra=extra)
            Expect.equal res value ""

        testCase "Auto decoders works for uint32" <| fun _ ->
            let value = 12u
            let json = Encode.Auto.toString(4, value)
            let res = Decode.Auto.unsafeFromString<uint32>(json)
            Expect.equal res value ""

        testCase "Auto decoders works for uint64" <| fun _ ->
            let extra = Extra.empty |> Extra.withUInt64
            let value = 9999999999999999999UL
            let json = Encode.Auto.toString(4, value, extra=extra)
            let res = Decode.Auto.unsafeFromString<uint64>(json, extra=extra)
            Expect.equal res value ""

        testCase "Auto decoders works for bigint" <| fun _ ->
            let extra = Extra.empty |> Extra.withBigInt
            let value = 99999999999999999999999I
            let json = Encode.Auto.toString(4, value, extra=extra)
            let res = Decode.Auto.unsafeFromString<bigint>(json, extra=extra)
            Expect.equal res value ""

        testCase "Auto decoders works for bool" <| fun _ ->
            let value = false
            let json = Encode.Auto.toString(4, value)
            let res = Decode.Auto.unsafeFromString<bool>(json)
            Expect.equal res value ""

        testCase "Auto decoders works for float" <| fun _ ->
            let value = 12.
            let json = Encode.Auto.toString(4, value)
            let res = Decode.Auto.unsafeFromString<float>(json)
            Expect.equal res value ""

        testCase "Auto decoders works for decimal" <| fun _ ->
            let extra = Extra.empty |> Extra.withDecimal
            let value = 0.7833M
            let json = Encode.Auto.toString(4, value, extra=extra)
            let res = Decode.Auto.unsafeFromString<decimal>(json, extra=extra)
            Expect.equal res value ""

        // testCase "Auto decoders works for datetime" <| fun _ ->
        //     let value = DateTime.Now
        //     let json = Encode.Auto.toString(4, value)
        //     let res = Decode.Auto.unsafeFromString<DateTime>(json)
        //     Expect.equal res.Date value.Date ""
        //     Expect.equal res.Hour value.Hour ""
        //     Expect.equal res.Minute value.Minute ""
        //     Expect.equal res.Second value.Second ""

        testCase "Auto decoders works for datetime UTC" <| fun _ ->
            let value = DateTime.UtcNow
            let json = Encode.Auto.toString(4, value)
            let res = Decode.Auto.unsafeFromString<DateTime>(json)
            Expect.equal res.Date value.Date ""
            Expect.equal res.Hour value.Hour ""
            Expect.equal res.Minute value.Minute ""
            Expect.equal res.Second value.Second ""

        testCase "Auto decoders works for datetimeOffset" <| fun _ ->
            let value = DateTimeOffset.Now
            let json = Encode.Auto.toString(4, value)
            let res = Decode.Auto.unsafeFromString<DateTimeOffset>(json).ToLocalTime()
            Expect.equal res.Date value.Date ""
            Expect.equal res.Hour value.Hour ""
            Expect.equal res.Minute value.Minute ""
            Expect.equal res.Second value.Second ""

        testCase "Auto decoders works for datetimeOffset UTC" <| fun _ ->
            let value = DateTimeOffset.UtcNow
            let json = Encode.Auto.toString(4, value)
            let res = Decode.Auto.unsafeFromString<DateTimeOffset>(json).ToUniversalTime()
            // printfn "SOURCE %A JSON %s OUTPUT %A" value json res
            Expect.equal res.Date value.Date ""
            Expect.equal res.Hour value.Hour ""
            Expect.equal res.Minute value.Minute ""
            Expect.equal res.Second value.Second ""

        testCase "Auto decoders works for TimeSpan" <| fun _ ->
            let value = TimeSpan(1,2,3,4,5)
            let json = Encode.Auto.toString(4, value)
            let res = Decode.Auto.unsafeFromString<TimeSpan>(json)
            Expect.equal res.Days value.Days ""
            Expect.equal res.Hours value.Hours ""
            Expect.equal res.Minutes value.Minutes ""
            Expect.equal res.Seconds value.Seconds ""
            Expect.equal res.Milliseconds value.Milliseconds ""

        testCase "Auto decoders works for list" <| fun _ ->
            let value = [1; 2; 3; 4]
            let json = Encode.Auto.toString(4, value)
            let res = Decode.Auto.unsafeFromString<int list>(json)
            Expect.equal res value ""

        testCase "Auto decoders works for array" <| fun _ ->
            let value = [| 1; 2; 3; 4 |]
            let json = Encode.Auto.toString(4, value)
            let res = Decode.Auto.unsafeFromString<int array>(json)
            Expect.equal res value ""

        testCase "Auto decoders works for seq" <| fun _ ->
            let value = [1; 2; 3; 4]
            let json = Encode.Auto.toString(4, value)
            let res = Decode.Auto.unsafeFromString<int seq>(json)
            // Comparing directly against a seq won't work, because
            // res is actually an array in disguise
            Expect.equal value (List.ofSeq res) ""

        testCase "Auto decoders works for option None" <| fun _ ->
            let value = None
            let json = Encode.Auto.toString(4, value)
            let res = Decode.Auto.unsafeFromString<int option>(json)
            Expect.equal res value ""

        testCase "Auto decoders works for option Some" <| fun _ ->
            let value = Some 5
            let json = Encode.Auto.toString(4, value)
            let res = Decode.Auto.unsafeFromString<int option>(json)
            Expect.equal res value ""

        testCase "Auto decoders works for Unit" <| fun _ ->
            let value = ()
            let json = Encode.Auto.toString(4, value)
            let res = Decode.Auto.unsafeFromString(json)
            Expect.equal res value ""

        testCase "Auto decoders works for enum<int8>" <| fun _ ->
            let res = Decode.Auto.unsafeFromString<Enum_Int8>("99")
            Expect.equal res Enum_Int8.NinetyNine ""

        testCase "Auto decoders for enum<int8> returns an error if the Enum value is invalid" <| fun _ ->
#if FABLE_COMPILER
            let value =
                Error(
                    """
Error at: `$`
Expecting Tests.Types.Enum_Int8[System.SByte] but instead got: 2
Reason: Unknown value provided for the enum
                    """.Trim())
#else
            let value =
                Error(
                    """
Error at: `$`
Expecting Tests.Types+Enum_Int8 but instead got: 2
Reason: Unknown value provided for the enum
                    """.Trim())
#endif

            let res = Decode.Auto.fromString<Enum_Int8>("2")
            Expect.equal res value ""

        testCase "Auto decoders works for enum<uint8>" <| fun _ ->
            let res = Decode.Auto.unsafeFromString<Enum_UInt8>("99")
            Expect.equal res Enum_UInt8.NinetyNine ""

        testCase "Auto decoders for enum<uint8> returns an error if the Enum value is invalid" <| fun _ ->
#if FABLE_COMPILER
            let value =
                Error(
                    """
Error at: `$`
Expecting Tests.Types.Enum_UInt8[System.Byte] but instead got: 2
Reason: Unknown value provided for the enum
                    """.Trim())
#else
            let value =
                Error(
                    """
Error at: `$`
Expecting Tests.Types+Enum_UInt8 but instead got: 2
Reason: Unknown value provided for the enum
                    """.Trim())
#endif

            let res = Decode.Auto.fromString<Enum_UInt8>("2")
            Expect.equal res value ""

        testCase "Auto decoders works for enum<int16>" <| fun _ ->
            let res = Decode.Auto.unsafeFromString<Enum_Int16>("99")
            Expect.equal res Enum_Int16.NinetyNine ""

        testCase "Auto decoders for enum<int16> returns an error if the Enum value is invalid" <| fun _ ->
#if FABLE_COMPILER
            let value =
                Error(
                    """
Error at: `$`
Expecting Tests.Types.Enum_Int16[System.Int16] but instead got: 2
Reason: Unknown value provided for the enum
                    """.Trim())
#else
            let value =
                Error(
                    """
Error at: `$`
Expecting Tests.Types+Enum_Int16 but instead got: 2
Reason: Unknown value provided for the enum
                    """.Trim())
#endif

            let res = Decode.Auto.fromString<Enum_Int16>("2")
            Expect.equal res value ""

        testCase "Auto decoders works for enum<uint16>" <| fun _ ->
            let res = Decode.Auto.unsafeFromString<Enum_UInt16>("99")
            Expect.equal res Enum_UInt16.NinetyNine ""

        testCase "Auto decoders for enum<uint16> returns an error if the Enum value is invalid" <| fun _ ->
#if FABLE_COMPILER
            let value =
                Error(
                    """
Error at: `$`
Expecting Tests.Types.Enum_UInt16[System.UInt16] but instead got: 2
Reason: Unknown value provided for the enum
                    """.Trim())
#else
            let value =
                Error(
                    """
Error at: `$`
Expecting Tests.Types+Enum_UInt16 but instead got: 2
Reason: Unknown value provided for the enum
                    """.Trim())
#endif

            let res = Decode.Auto.fromString<Enum_UInt16>("2")
            Expect.equal res value ""

        testCase "Auto decoders works for enum<int>" <| fun _ ->
            let res = Decode.Auto.unsafeFromString<Enum_Int>("1")
            Expect.equal res Enum_Int.One ""

        testCase "Auto decoders for enum<int> returns an error if the Enum value is invalid" <| fun _ ->
#if FABLE_COMPILER
            let value =
                Error(
                    """
Error at: `$`
Expecting Tests.Types.Enum_Int[System.Int32] but instead got: 4
Reason: Unknown value provided for the enum
                    """.Trim())
#else
            let value =
                Error(
                    """
Error at: `$`
Expecting Tests.Types+Enum_Int but instead got: 4
Reason: Unknown value provided for the enum
                    """.Trim())
#endif

            let res = Decode.Auto.fromString<Enum_Int>("4")
            Expect.equal res value ""

        testCase "Auto decoders works for enum<uint32>" <| fun _ ->
            let res = Decode.Auto.unsafeFromString<Enum_UInt32>("99")
            Expect.equal res Enum_UInt32.NinetyNine ""

        testCase "Auto decoders for enum<uint32> returns an error if the Enum value is invalid" <| fun _ ->
#if FABLE_COMPILER
            let value =
                Error(
                    """
Error at: `$`
Expecting Tests.Types.Enum_UInt32[System.UInt32] but instead got: 2
Reason: Unknown value provided for the enum
                    """.Trim())
#else
            let value =
                Error(
                    """
Error at: `$`
Expecting Tests.Types+Enum_UInt32 but instead got: 2
Reason: Unknown value provided for the enum
                    """.Trim())
#endif

            let res = Decode.Auto.fromString<Enum_UInt32>("2")
            Expect.equal res value ""

(*
        #if NETFRAMEWORK
        testCase "Auto decoders  works with char based Enums" <| fun _ ->
            let value = CharEnum.A
            let json = Encode.Auto.toString(4, value)
            let res = Decode.Auto.unsafeFromString<CharEnum>(json)
            Expect.equal res value ""
        #endif
*)
        testCase "Auto decoders works for null" <| fun _ ->
            let value = null
            let json = Encode.Auto.toString(4, value)
            let res = Decode.Auto.unsafeFromString<obj>(json)
            Expect.equal res value ""

        testCase "Auto decoders works even if type is determined by the compiler" <| fun _ ->
            let value = [1; 2; 3; 4]
            let json = Encode.Auto.toString(4, value)
            let res = Decode.Auto.unsafeFromString<_>(json)
            Expect.equal res value ""

        testCase "Auto.unsafeFromString works with camelCase" <| fun _ ->
            let json = """{ "id" : 0, "name": "maxime", "email": "mail@domain.com", "followers": 0 }"""
            let user = Decode.Auto.unsafeFromString<User>(json, caseStrategy=CamelCase)
            Expect.equal user.Name "maxime" ""
            Expect.equal user.Id 0 ""
            Expect.equal user.Followers 0 ""
            Expect.equal user.Email "mail@domain.com" ""

        testCase "Auto.fromString works with snake_case" <| fun _ ->
            let json = """{ "one" : 1, "two_part": 2, "three_part_field": 3 }"""
            let decoded = Decode.Auto.fromString<RecordForCharacterCase>(json, caseStrategy=SnakeCase)
            let expected = Ok { One = 1; TwoPart = 2; ThreePartField = 3 }
            Expect.equal decoded expected ""

        testCase "Auto.fromString works with camelCase" <| fun _ ->
            let json = """{ "id" : 0, "name": "maxime", "email": "mail@domain.com", "followers": 0 }"""
            let user = Decode.Auto.fromString<User>(json, caseStrategy=CamelCase)
            let expected = Ok { Id = 0; Name = "maxime"; Email = "mail@domain.com"; Followers = 0 }
            Expect.equal user expected ""

        testCase "Auto.fromString works for records with an actual value for the optional field value" <| fun _ ->
            let json = """{ "maybe" : "maybe value", "must": "must value"}"""
            let actual = Decode.Auto.fromString<TestMaybeRecord>(json, caseStrategy=CamelCase)
            let expected =
                Ok ({ Maybe = Some "maybe value"
                      Must = "must value" } : TestMaybeRecord)
            Expect.equal actual expected ""

        testCase "Auto.fromString works for records with `null` for the optional field value" <| fun _ ->
            let json = """{ "maybe" : null, "must": "must value"}"""
            let actual = Decode.Auto.fromString<TestMaybeRecord>(json, caseStrategy=CamelCase)
            let expected =
                Ok ({ Maybe = None
                      Must = "must value" } : TestMaybeRecord)
            Expect.equal actual expected ""

        testCase "Auto.fromString works for records with `null` for the optional field value on classes" <| fun _ ->
            let json = """{ "maybeClass" : null, "must": "must value"}"""
            let actual = Decode.Auto.fromString<RecordWithOptionalClass>(json, caseStrategy=CamelCase)
            let expected =
                Ok ({ MaybeClass = None
                      Must = "must value" } : RecordWithOptionalClass)
            Expect.equal actual expected ""

        testCase "Auto.fromString works for records missing optional field value on classes" <| fun _ ->
            let json = """{ "must": "must value"}"""
            let actual = Decode.Auto.fromString<RecordWithOptionalClass>(json, caseStrategy=CamelCase)
            let expected =
                Ok ({ MaybeClass = None
                      Must = "must value" } : RecordWithOptionalClass)
            Expect.equal actual expected ""

        testCase "Auto.generateDecoder throws for field using a non optional class" <| fun _ ->
            let expected = "Cannot generate auto decoder for Tests.Types.BaseClass. Please pass an extra decoder."
            let errorMsg =
                try
                    let decoder = Decode.Auto.generateDecoder<RecordWithRequiredClass>(caseStrategy=CamelCase)
                    ""
                with ex ->
                    ex.Message
            Expect.equal (errorMsg.Replace("+", ".")) expected ""

        testCase "Auto.fromString works for Class marked as optional" <| fun _ ->
            let json = """null"""

            let actual = Decode.Auto.fromString<BaseClass option>(json, caseStrategy=CamelCase)
            let expected = Ok None
            Expect.equal actual expected ""

        testCase "Auto.generateDecoder throws for Class" <| fun _ ->
            let expected = "Cannot generate auto decoder for Tests.Types.BaseClass. Please pass an extra decoder."
            let errorMsg =
                try
                    let decoder = Decode.Auto.generateDecoder<BaseClass>(caseStrategy=CamelCase)
                    ""
                with ex ->
                    ex.Message
            Expect.equal (errorMsg.Replace("+", ".")) expected ""

        testCase "Auto.fromString works for records missing an optional field" <| fun _ ->
            let json = """{ "must": "must value"}"""
            let actual = Decode.Auto.fromString<TestMaybeRecord>(json, caseStrategy=CamelCase)
            let expected =
                Ok ({ Maybe = None
                      Must = "must value" } : TestMaybeRecord)
            Expect.equal actual expected ""

        testCase "Auto.fromString works with maps encoded as objects" <| fun _ ->
            let expected = Map [("oh", { a = 2.; b = 2. }); ("ah", { a = -1.5; b = 0. })]
            let json = """{"ah":{"a":-1.5,"b":0},"oh":{"a":2,"b":2}}"""
            let actual = Decode.Auto.fromString json
            Expect.equal actual (Ok expected) ""

        testCase "Auto.fromString works with maps encoded as arrays" <| fun _ ->
            let expected = Map [({ a = 2.; b = 2. }, "oh"); ({ a = -1.5; b = 0. }, "ah")]
            let json = """[[{"a":-1.5,"b":0},"ah"],[{"a":2,"b":2},"oh"]]"""
            let actual = Decode.Auto.fromString json
            Expect.equal actual (Ok expected) ""

        testCase "Auto.unsafeFromString works with mutable dictionaries" <| fun _ ->
            let expected = System.Collections.Generic.Dictionary()
            expected.Add("oh", { a = 2.; b = 2. })
            expected.Add("ah", { a = -1.5; b = 0. })
            let json = """{"ah":{"a":-1.5,"b":0},"oh":{"a":2,"b":2}}"""
            let actual: System.Collections.Generic.Dictionary<_,_> =
                Decode.Auto.unsafeFromString json
            for (KeyValue(k, v)) in expected do
                Expect.equal actual.[k] v ""
            Expect.equal actual.["ah"].a -1.5 ""
            actual.["ah"] <- { a = 0.; b = 0. }
            Expect.equal actual.["ah"].a 0. ""
        testCase "Auto.unsafeFromString works with mutable dictionaires with non simples keys" <| fun _ ->
            let json = """[[{"ComplexKey":1},1],[{"ComplexKey":2},2]]"""
            let d = System.Collections.Generic.Dictionary()
            d.Add({| ComplexKey = 1 |}, 1)
            d.Add({| ComplexKey = 2 |}, 2)
            let actual : Collections.Generic.Dictionary<{| ComplexKey : int |}, int> = Decode.Auto.unsafeFromString json
            Expect.equal actual.[{| ComplexKey = 1 |}] 1 ""
            Expect.equal actual.[{| ComplexKey = 2 |}] 2 ""

        testCase "Auto.unsafeFromString works with mutable hashsets" <| fun _ ->
            let expected = System.Collections.Generic.HashSet()
            expected.Add({ a = 2.; b = 2. }) |> ignore
            expected.Add({ a = -1.5; b = 0. }) |> ignore
            let json = """[{"a":2,"b":2},{"a":-1.5,"b":0}]"""
            let actual: System.Collections.Generic.HashSet<_> = Decode.Auto.unsafeFromString json
            for x in expected do
                Expect.equal (actual.Contains(x)) true ""
            actual.Add({ a = 3.; b = 3. }) |> ignore
            Expect.equal actual.Count 3 ""
            actual.Add({ a = 2.; b = 2. }) |> ignore
            Expect.equal actual.Count 3 ""

        testCase "Decoder.Auto.toString works with bigint extra" <| fun _ ->
            let extra = Extra.empty |> Extra.withBigInt
            let expected = { bigintField = 9999999999999999999999I }
            let actual = Decode.Auto.fromString("""{"bigintField":"9999999999999999999999"}""", extra=extra)
            Expect.equal actual (Ok expected) ""

        testCase "Decoder.Auto.toString works with custom extra" <| fun _ ->
            let extra = Extra.empty |> Extra.withCustom ChildType.Encode ChildType.Decoder
            let expected = { ParentField = { ChildField = "bumbabon" } }
            let actual = Decode.Auto.fromString("""{"ParentField":"bumbabon"}""", extra=extra)
            Expect.equal actual (Ok expected) ""

        testCase "Auto.fromString works with records with private constructors" <| fun _ ->
            let json = """{ "foo1": 5, "foo2": 7.8 }"""
            let actual = Decode.Auto.fromString(json, caseStrategy=CamelCase)
            let expected = Ok ({ Foo1 = 5; Foo2 = 7.8 }: RecordWithPrivateConstructor)
            Expect.equal actual expected ""

        testCase "Auto.fromString works with unions with private constructors" <| fun _ ->
            let json = """[ "Baz", ["Bar", "foo"]]"""
            let actual = Decode.Auto.fromString<UnionWithPrivateConstructor list>(json, caseStrategy=CamelCase)
            let expected = Ok [Baz; Bar "foo"]
            Expect.equal actual expected ""

        testCase "Auto.generateDecoderCached works" <| fun _ ->
            let expected = Ok { Id = 0; Name = "maxime"; Email = "mail@domain.com"; Followers = 0 }
            let json = """{ "id" : 0, "name": "maxime", "email": "mail@domain.com", "followers": 0 }"""
            let decoder1 = Decode.Auto.generateDecoderCached<User>(caseStrategy=CamelCase)
            let decoder2 = Decode.Auto.generateDecoderCached<User>(caseStrategy=CamelCase)
            let actual1 = Decode.fromString decoder1 json
            let actual2 = Decode.fromString decoder2 json
            Expect.equal actual1 expected ""
            Expect.equal actual2 expected ""
            Expect.equal actual1 actual2 ""

        testCase "Auto.fromString works with strange types if they are None" <| fun _ ->
            let json = """{"Id":0}"""
            let actual = Decode.Auto.fromString<RecordWithStrangeType>(json)
            let expected = Ok { Id = 0; Thread = None }
            Expect.equal actual expected ""

        testCase "Auto.fromString works with recursive types" <| fun _ ->
            let vater =
                { Name = "Alfonso"
                  Children = [ { Name = "Narumi"; Children = [] }
                               { Name = "Takumi"; Children = [] } ] }
            let json = """{"Name":"Alfonso","Children":[{"Name":"Narumi","Children":[]},{"Name":"Takumi","Children":[]}]}"""
            let actual = Decode.Auto.fromString<MyRecType>(json)
            let expected = Ok vater
            Expect.equal actual expected ""

        testCase "Auto.unsafeFromString works for unit" <| fun _ ->
            let json = Encode.unit () |> Encode.toString 4
            let res = Decode.Auto.unsafeFromString<unit>(json)
            Expect.equal res () ""

        testCase "Erased single-case DUs works" <| fun _ ->
            let expected = NoAllocAttributeSingleCaseDU (Guid.NewGuid())
            let json = Encode.Auto.toString(4, expected)
            let actual = Decode.Auto.unsafeFromString<NoAllocAttributeSingleCaseDU>(json)
            Expect.equal actual expected ""

        testCase "Single case unions generates simplify JSON" <| fun _ ->
            let expected = SingleCaseDUSimple "Maxime"
            let json = "\"Maxime\""
            let actual = Decode.Auto.unsafeFromString(json)
            Expect.equal actual expected ""

        testCase "Single case unions generates simplify JSON and works with complex types" <| fun _ ->
            let expected = SingleCaseDUComplex {| FirstName = "Maxime"; Age = 28 |}
            let json =
                """
{
    "Age": 28,
    "FirstName": "Maxime"
}
                """.Trim()
            let actual = Decode.Auto.unsafeFromString json
            Expect.equal actual expected ""

        testCase "Old style of single case unions is supported" <| fun _ ->
            let expected = SingleCaseDUSimple "Maxime"
            let json =
                """
[
    "SingleCaseDUSimple",
    "Maxime"
]
                """.Trim()
            let actual = Decode.Auto.unsafeFromString(json)
            Expect.equal actual expected ""

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
            Expect.equal actual expected ""

        testCase "Decode.Auto.fromString allows to customize default known types" <| fun _ ->
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

            let json = "{\"type\":\"int\",\"value\":99}"
            let actual = Decode.Auto.unsafeFromString(json, extra = extra)

            Expect.equal actual 99 ""

        testCase "Decode.Auto.fromString works with [<StrinEnum>]" <| fun _ ->
            let expected = Camera.FirstPerson
            let actual = Decode.Auto.unsafeFromString("\"firstPerson\"")
            Expect.equal actual expected ""

        testCase "Decode.Auto.fromString works with [<StringEnum(CaseRules.LowerFirst)>" <| fun _ ->
            let expected = Framework.React
            let actual = Decode.Auto.unsafeFromString("\"react\"")
            Expect.equal actual expected ""

        testCase "Decode.Auto.fromString works with [<StringEnum(CaseRules.None)>]" <| fun _ ->
            let expected = Language.Fsharp
            let actual = Decode.Auto.unsafeFromString("\"Fsharp\"")
            Expect.equal actual expected ""

        testCase "Decode.Auto.fromString works with [<StringEnum>] + [<CompiledName>]" <| fun _ ->
            let expected = Language.Csharp
            let actual = Decode.Auto.unsafeFromString("\"C#\"")
            Expect.equal actual expected ""

    ]
