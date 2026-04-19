module Thoth.Json.Tests.Auto

open Thoth.Json.Tests.Testing
open Thoth.Json.Core
open Thoth.Json.Core.Auto
open Fable.Pyxpecto
open System
open Thoth.Json.Tests.Types

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

type UnionWithMultipleFields = | Multi of string * int * float

let tests (runner: TestRunner<_, _>) =

    let inline autoEncodeWithOptions
        (json: 'T)
        (caseStrategy: CaseStrategy)
        (extra: ExtraCoders)
        =
        json
        |> Encode.Auto.generateEncoder (
            caseStrategy = caseStrategy,
            extra = extra
        )
        |> runner.Encode.toString 4

    let inline autoEncodeWithExtra json extra =
        json
        |> Encode.Auto.generateEncoder (extra = extra)
        |> runner.Encode.toString 4

    let inline autoEncode json =
        json |> Encode.Auto.generateEncoder () |> runner.Encode.toString 4

    let inline autoDecodeUnsafeWithOptions
        (json: string)
        (caseStrategy: CaseStrategy)
        (extra: ExtraCoders)
        : 'T
        =
        json
        |> runner.Decode.unsafeFromString (
            Decode.Auto.generateDecoder<'T> (
                caseStrategy = caseStrategy,
                extra = extra
            )
        )

    let inline autoDecodeUnsafe (json: string) : 'T =
        json
        |> runner.Decode.unsafeFromString (Decode.Auto.generateDecoder<'T> ())

    let inline autoDecodeUnsafeWithExtra
        (json: string)
        (extra: ExtraCoders)
        : 'T
        =
        json
        |> runner.Decode.unsafeFromString (
            Decode.Auto.generateDecoder<'T> (extra = extra)
        )

    let inline autoDecode (json: string) : Result<'T, string> =
        json |> runner.Decode.fromString (Decode.Auto.generateDecoder<'T> ())

    let inline autoDecodeWithOptions
        (json: string)
        (caseStrategy: CaseStrategy)
        (extra: ExtraCoders)
        : Result<'T, string>
        =
        json
        |> runner.Decode.fromString (
            Decode.Auto.generateDecoder<'T> (
                caseStrategy = caseStrategy,
                extra = extra
            )
        )

    testList
        "Thoth.Json.Core.Auto"
        [
            testCase "Auto.runner.Decode.fromString works"
            <| fun _ ->
                let now = DateTime.Now

                let value: Record9 =
                    {
                        a = 5
                        b = "bar"
                        c =
                            [
                                false, 3
                                true, 5
                                false, 10
                            ]
                        d =
                            [|
                                Some(Foo 14)
                                None
                            |]
                        e =
                            Map
                                [
                                    ("oh",
                                     {
                                         a = 2.
                                         b = 2.
                                     })
                                    ("ah",
                                     {
                                         a = -1.5
                                         b = 0.
                                     })
                                ]
                        f = now
                        g =
                            set
                                [
                                    {
                                        a = 2.
                                        b = 2.
                                    }
                                    {
                                        a = -1.5
                                        b = 0.
                                    }
                                ]
                        h = TimeSpan.FromSeconds(5.)
                        i = 120y
                        j = 120uy
                        k = 250s
                        l = 250us
                        m = 99u
                        n = 99L
                        o = 999UL
                        p = ()
                        r =
                            Map
                                [
                                    ({
                                        a = 1.
                                        b = 2.
                                     },
                                     "value 1")
                                    ({
                                        a = -2.5
                                        b = 22.1
                                     },
                                     "value 2")
                                ]
                        s = 'y'
                    // s = seq [ "item n°1"; "item n°2"]
                    }

                let extra = Extra.empty |> Extra.withInt64 |> Extra.withUInt64

                let json = autoEncodeWithOptions value PascalCase extra

                let r2: Record9 =
                    autoDecodeUnsafeWithOptions json PascalCase extra

                equal 5 r2.a
                equal "bar" r2.b

                equal
                    [
                        false, 3
                        true, 5
                        false, 10
                    ]
                    r2.c

                equal (Some(Foo 14)) r2.d.[0]
                equal None r2.d.[1]
                equal -1.5 (Map.find "ah" r2.e).a
                equal 2. (Map.find "oh" r2.e).b
                equal (now.ToString()) (value.f.ToString())

                equal
                    true
                    (Set.contains
                        {
                            a = -1.5
                            b = 0.
                        }
                        r2.g)

                equal
                    false
                    (Set.contains
                        {
                            a = 1.5
                            b = 0.
                        }
                        r2.g)

                equal 5000. value.h.TotalMilliseconds
                equal 120y r2.i
                equal 120uy r2.j
                equal 250s r2.k
                equal 250us r2.l
                equal 99u r2.m
                equal 99L r2.n
                equal 999UL r2.o
                equal () r2.p

                equal
                    (Map
                        [
                            ({
                                a = 1.
                                b = 2.
                             },
                             "value 1")
                            ({
                                a = -2.5
                                b = 22.1
                             },
                             "value 2")
                        ])
                    r2.r

                equal 'y' r2.s
            // equal ((seq [ "item n°1"; "item n°2"]) |> Seq.toList) (r2.s |> Seq.toList)

            // testCase "Auto serialization works with recursive types"
            // <| fun _ ->
            //     let len xs =
            //         let rec lenInner acc =
            //             function
            //             | Cons(_, rest) -> lenInner (acc + 1) rest
            //             | Nil -> acc

            //         lenInner 0 xs

            //     let li = Cons(1, Cons(2, Cons(3, Nil)))
            //     let json = Encode.Auto.toString (4, li)
            //     // printfn "AUTO ENCODED MYLIST %s" json
            // let res = autoDecode json
            //     len li2 |> equal 3

            //     match li with
            //     | Cons(i1, Cons(i2, Cons(i3, Nil))) -> i1 + i2 + i3
            //     | Cons(i, _) -> i
            //     | Nil -> 0
            //     |> equal 6

            testCase "Auto decoders works for string"
            <| fun _ ->
                let value = "maxime"
                let json = autoEncode value
                let res = autoDecodeUnsafe json
                equal value res

            testCase "Auto decoders works for guid"
            <| fun _ ->
                let value = Guid.NewGuid()
                let json = autoEncode value
                let res = autoDecodeUnsafe json
                equal value res

            testCase "Auto decoders works for int"
            <| fun _ ->
                let value = 12
                let json = autoEncode value
                let res = autoDecodeUnsafe json
                equal value res

            testCase "Auto decoders works for int64"
            <| fun _ ->
                let extra = Extra.empty |> Extra.withInt64
                let value = 9999999999L
                let json = autoEncodeWithExtra value extra

                let res = autoDecodeUnsafeWithExtra json extra

                equal value res

            testCase "Auto decoders works for uint32"
            <| fun _ ->
                let value = 12u
                let json = autoEncode value
                let res = autoDecodeUnsafe json
                equal value res

            testCase "Auto decoders works for uint64"
            <| fun _ ->
                let extra = Extra.empty |> Extra.withUInt64
                let value = 9999999999999999999UL
                let json = autoEncodeWithExtra value extra

                let res = autoDecodeUnsafeWithExtra json extra

                equal value res

            testCase "Auto decoders works for bigint"
            <| fun _ ->
                let extra = Extra.empty |> Extra.withBigInt
                let value = 99999999999999999999999I
                let json = autoEncodeWithExtra value extra

                let res = autoDecodeUnsafeWithExtra json extra

                equal value res

            testCase "Auto decoders works for bool"
            <| fun _ ->
                let value = false
                let json = autoEncode value
                let res = autoDecodeUnsafe json
                equal value res

            testCase "Auto decoders works for float"
            <| fun _ ->
                let value = 12.
                let json = autoEncode value
                let res = autoDecodeUnsafe json
                equal value res

            testCase "Auto decoders works for decimal"
            <| fun _ ->
                let extra = Extra.empty |> Extra.withDecimal
                let value = 0.7833M
                let json = autoEncodeWithExtra value extra
                let res = autoDecodeUnsafeWithExtra json extra

                equal value res

            testCase "Auto extra decoders can override default decoders"
            <| fun _ ->
                let extra =
                    Extra.empty
                    |> Extra.withCustom IntAsRecord.encode IntAsRecord.decode

                let json =
                    """
{
    "type": "int",
    "value": 12
}
                """

                let res = autoDecodeUnsafeWithExtra json extra

                equal 12 res

            // testCase "Auto decoders works for datetime"
            // <| fun _ ->
            //     let value = DateTime.Now
            //     let json = autoEncode value
            //     let res: DateTime = autoDecode json
            //     equal value.Date res.Date
            //     equal value.Hour res.Hour
            //     equal value.Minute res.Minute
            //     equal value.Second res.Second

            testCase "Auto decoders works for datetime UTC"
            <| fun _ ->
                let value = DateTime.UtcNow
                let json = autoEncode value
                let res: DateTime = autoDecodeUnsafe json
                equal value.Date res.Date
                equal value.Hour res.Hour
                equal value.Minute res.Minute
                equal value.Second res.Second

            testCase "Auto decoders works for datetimeOffset"
            <| fun _ ->
                let value = DateTimeOffset.Now
                let json = autoEncode value

                let res: DateTimeOffset = autoDecodeUnsafe json
                // let res = res.ToLocalTime()

                equal value.Date res.Date
                equal value.Hour res.Hour
                equal value.Minute res.Minute
                equal value.Second res.Second

            testCase "Auto decoders works for datetimeOffset UTC"
            <| fun _ ->
                let value = DateTimeOffset.UtcNow
                let json = autoEncode value

                let res: DateTimeOffset = autoDecodeUnsafe json
                let res = res.ToUniversalTime()

                equal value.Date res.Date
                equal value.Hour res.Hour
                equal value.Minute res.Minute
                equal value.Second res.Second

            testCase "Auto decoders works for TimeSpan"
            <| fun _ ->
                let value = TimeSpan(1, 2, 3, 4, 5)
                let json = autoEncode value
                let res: TimeSpan = autoDecodeUnsafe json
                equal value.Days res.Days
                equal value.Hours res.Hours
                equal value.Minutes res.Minutes
                equal value.Seconds res.Seconds
                equal value.Milliseconds res.Milliseconds

            testCase "Auto decoders works for list"
            <| fun _ ->
                let value =
                    [
                        1
                        2
                        3
                        4
                    ]

                let json = autoEncode value
                let res = autoDecodeUnsafe json
                equal value res

            testCase "Auto decoders works for array"
            <| fun _ ->
                let value =
                    [|
                        1
                        2
                        3
                        4
                    |]

                let json = autoEncode value
                let res = autoDecodeUnsafe json
                equal value res

            testCase "Auto decoders works for Map with string keys"
            <| fun _ ->
                let value =
                    Map.ofSeq
                        [
                            "a", 1
                            "b", 2
                            "c", 3
                        ]

                let json = autoEncode value
                let res = autoDecodeUnsafe json
                equal value res

            testCase "Auto decoders works for Map with complex keys"
            <| fun _ ->
                let value =
                    Map.ofSeq
                        [
                            (1, 6), "a"
                            (2, 7), "b"
                            (3, 8), "c"
                        ]

                let json = autoEncode value

                let res = autoDecodeUnsafe json

                equal value res

            testCase "Auto decoders works for option None"
            <| fun _ ->
                let value: int option = None
                let json = autoEncode value
                let res: int option = autoDecodeUnsafe json
                equal value res

            testCase "Auto decoders works for option Some"
            <| fun _ ->
                let value = Some 5
                let json = autoEncode value
                let res = autoDecodeUnsafe json
                equal value res

            testCase "Auto decoders works for Unit"
            <| fun _ ->
                let value = ()
                let json = autoEncode value
                let res = autoDecodeUnsafe json
                equal value res

            testCase "Auto decoders works for enum<int8>"
            <| fun _ ->
                let res: Enum_Int8 = autoDecodeUnsafe "99"
                equal Enum_Int8.NinetyNine res

            testCase
                "Auto decoders for enum<int8> returns an error if the Enum value is invalid"
            <| fun _ ->
#if FABLE_COMPILER
                let value =
                    Error(
                        """
Error at: `$`
Expecting Thoth.Json.Tests.Types.Enum_Int8[System.SByte] but instead got: 2
Reason: Unkown value provided for the enum
                        """
                            .Trim()
                    )
#else
                let value =
                    Error(
                        """
Error at: `$`
Expecting Thoth.Json.Tests.Types+Enum_Int8 but instead got: 2
Reason: Unkown value provided for the enum
                        """
                            .Trim()
                    )
#endif

                let res: Result<Enum_Int8, string> = autoDecode "2"
                equal value res

            testCase "Auto decoders works for enum<uint8>"
            <| fun _ ->
                let res: Enum_UInt8 = autoDecodeUnsafe "99"
                equal Enum_UInt8.NinetyNine res

            testCase
                "Auto decoders for enum<uint8> returns an error if the Enum value is invalid"
            <| fun _ ->
#if FABLE_COMPILER
                let value =
                    Error(
                        """
Error at: `$`
Expecting Thoth.Json.Tests.Types.Enum_UInt8[System.Byte] but instead got: 2
Reason: Unkown value provided for the enum
                        """
                            .Trim()
                    )
#else
                let value =
                    Error(
                        """
Error at: `$`
Expecting Thoth.Json.Tests.Types+Enum_UInt8 but instead got: 2
Reason: Unkown value provided for the enum
                        """
                            .Trim()
                    )
#endif

                let res: Result<Enum_UInt8, string> = autoDecode "2"
                equal value res

            testCase "Auto decoders works for enum<int16>"
            <| fun _ ->
                let res: Enum_Int16 = autoDecodeUnsafe "99"
                equal Enum_Int16.NinetyNine res

            testCase
                "Auto decoders for enum<int16> returns an error if the Enum value is invalid"
            <| fun _ ->
#if FABLE_COMPILER
                let value =
                    Error(
                        """
Error at: `$`
Expecting Thoth.Json.Tests.Types.Enum_Int16[System.Int16] but instead got: 2
Reason: Unkown value provided for the enum
                        """
                            .Trim()
                    )
#else
                let value =
                    Error(
                        """
Error at: `$`
Expecting Thoth.Json.Tests.Types+Enum_Int16 but instead got: 2
Reason: Unkown value provided for the enum
                        """
                            .Trim()
                    )
#endif

                let res: Result<Enum_Int16, string> = autoDecode "2"
                equal value res

            testCase "Auto decoders works for enum<uint16>"
            <| fun _ ->
                let res: Enum_UInt16 = autoDecodeUnsafe "99"
                equal Enum_UInt16.NinetyNine res

            testCase
                "Auto decoders for enum<ºint16> returns an error if the Enum value is invalid"
            <| fun _ ->
#if FABLE_COMPILER
                let value =
                    Error(
                        """
Error at: `$`
Expecting Thoth.Json.Tests.Types.Enum_UInt16[System.UInt16] but instead got: 2
Reason: Unkown value provided for the enum
                        """
                            .Trim()
                    )
#else
                let value =
                    Error(
                        """
Error at: `$`
Expecting Thoth.Json.Tests.Types+Enum_UInt16 but instead got: 2
Reason: Unkown value provided for the enum
                        """
                            .Trim()
                    )
#endif

                let res: Result<Enum_UInt16, string> = autoDecode "2"
                equal value res

            testCase "Auto decoders works for enum<int>"
            <| fun _ ->
                let res: Enum_Int = autoDecodeUnsafe "1"
                equal Enum_Int.One res

            testCase
                "Auto decoders for enum<int> returns an error if the Enum value is invalid"
            <| fun _ ->
#if FABLE_COMPILER
                let value =
                    Error(
                        """
Error at: `$`
Expecting Thoth.Json.Tests.Types.Enum_Int[System.Int32] but instead got: 4
Reason: Unkown value provided for the enum
                        """
                            .Trim()
                    )
#else
                let value =
                    Error(
                        """
Error at: `$`
Expecting Thoth.Json.Tests.Types+Enum_Int but instead got: 4
Reason: Unkown value provided for the enum
                        """
                            .Trim()
                    )
#endif

                let res: Result<Enum_Int, string> = autoDecode "4"
                equal value res

            testCase "Auto decoders works for enum<uint32>"
            <| fun _ ->
                let res: Enum_UInt32 = autoDecodeUnsafe "99"
                equal Enum_UInt32.NinetyNine res

            testCase
                "Auto decoders for enum<uint32> returns an error if the Enum value is invalid"
            <| fun _ ->
#if FABLE_COMPILER
                let value =
                    Error(
                        """
Error at: `$`
Expecting Thoth.Json.Tests.Types.Enum_UInt32[System.UInt32] but instead got: 2
Reason: Unkown value provided for the enum
                        """
                            .Trim()
                    )
#else
                let value =
                    Error(
                        """
Error at: `$`
Expecting Thoth.Json.Tests.Types+Enum_UInt32 but instead got: 2
Reason: Unkown value provided for the enum
                        """
                            .Trim()
                    )
#endif

                let res: Result<Enum_UInt32, string> = autoDecode "2"
                equal value res

            (*
#if NETFRAMEWORK
                    testCase "Auto decoders  works with char based Enums" <| fun _ ->
                        let value = CharEnum.A
                        let json = autoEncode value
                        let res : CharEnum = autoDecodeUnsaf(json)
                        equal value res
#endif
            *)

            // Cannot be tested because of type resolution issues
            // testCase "Auto decoders works for null"
            // <| fun _ ->
            //     let value : string = null
            //     let json = autoEncode value
            //     let res : string = autoDecodeUnsafe json
            //     equal value res

            testCase "Auto decoders works for anonymous record"
            <| fun _ ->
                let value =
                    {|
                        A = "string"
                    |}

                let json = autoEncode value
                let res = autoDecodeUnsafe json
                equal value res

            testCase "Auto decoders works for nested anonymous record"
            <| fun _ ->
                let value =
                    {|
                        A =
                            {|
                                B = "string"
                            |}
                    |}

                let json = autoEncode value
                let res = autoDecodeUnsafe json
                equal value res

            testCase
                "Auto decoders works even if type is determined by the compiler"
            <| fun _ ->
                let value =
                    [
                        1
                        2
                        3
                        4
                    ]

                let json = autoEncode value
                let res = autoDecodeUnsafe json
                equal value res

            testCase "Auto.unsafeFromString works with camelCase"
            <| fun _ ->
                let json =
                    """{ "id" : 0, "name": "maxime", "email": "mail@domain.com", "followers": 0 }"""

                let user: User =
                    autoDecodeUnsafeWithOptions json CamelCase Extra.empty

                equal "maxime" user.Name
                equal 0 user.Id
                equal 0 user.Followers
                equal "mail@domain.com" user.Email

            testCase "works with snake_case"
            <| fun _ ->
                let json =
                    """{ "one" : 1, "two_part": 2, "three_part_field": 3 }"""

                let decoded: RecordForCharacterCase =
                    autoDecodeUnsafeWithOptions json SnakeCase Extra.empty

                let expected =
                    {
                        One = 1
                        TwoPart = 2
                        ThreePartField = 3
                    }

                equal expected decoded

            testCase
                "works for records with an actual value for the optional field value"
            <| fun _ ->
                let json =
                    """{ "maybe" : "maybe value", "must": "must value"}"""

                let actual: TestMaybeRecord =
                    autoDecodeUnsafeWithOptions json CamelCase Extra.empty

                let expected =
                    {
                        Maybe = Some "maybe value"
                        Must = "must value"
                    }

                equal expected actual

            testCase
                "works for records with `null` for the optional field value"
            <| fun _ ->
                let json = """{ "maybe" : null, "must": "must value"}"""

                let actual: TestMaybeRecord =
                    autoDecodeUnsafeWithOptions json CamelCase Extra.empty

                let expected =
                    {
                        Maybe = None
                        Must = "must value"
                    }

                equal expected actual

            testCase
                "works for records with `null` for the optional field value on classes"
            <| fun _ ->
                let json = """{ "maybeClass" : null, "must": "must value"}"""

                let actual: RecordWithOptionalClass =
                    autoDecodeUnsafeWithOptions json CamelCase Extra.empty

                let expected =
                    {
                        MaybeClass = None
                        Must = "must value"
                    }

                equal expected actual

            testCase "works for records missing optional field value on classes"
            <| fun _ ->
                let json = """{ "must": "must value"}"""

                let actual: RecordWithOptionalClass =
                    autoDecodeUnsafeWithOptions json CamelCase Extra.empty

                let expected =
                    {
                        MaybeClass = None
                        Must = "must value"
                    }

                equal expected actual

            testCase
                "Auto.generateDecoder throws for field using a non optional class"
            <| fun _ ->
                let expected =
                    """Cannot generate auto decoder for 'Thoth.Json.Tests.Types.BaseClass'. Please pass an extra decoder.

Documentation available at: https://thoth-org.github.io/Thoth.Json/documentation/auto/extra-coders.html#ready-to-use-extra-coders"""

                let errorMsg =
                    try
                        let decoder =
                            Decode.Auto.generateDecoder<RecordWithRequiredClass> (
                                caseStrategy = CamelCase
                            )

                        failwith "Should have thrown an error"
                    with ex ->
                        ex.Message

                errorMsg.Replace("+", ".") |> equal expected

            testCase "works for Class marked as optional"
            <| fun _ ->
                let json = """null"""

                let actual: BaseClass option = autoDecodeUnsafe json

                let expected = None
                equal expected actual

            testCase "Auto.generateDecoder throws for Class"
            <| fun _ ->
                let expected =
                    """Cannot generate auto decoder for 'Thoth.Json.Tests.Types.BaseClass'. Please pass an extra decoder.

Documentation available at: https://thoth-org.github.io/Thoth.Json/documentation/auto/extra-coders.html#ready-to-use-extra-coders"""

                let errorMsg =
                    try
                        let decoder =
                            Decode.Auto.generateDecoder<BaseClass> (
                                caseStrategy = CamelCase
                            )

                        ""
                    with ex ->
                        ex.Message

                errorMsg.Replace("+", ".") |> equal expected

            testCase "works for records missing an optional field"
            <| fun _ ->
                let json = """{ "must": "must value"}"""

                let actual: TestMaybeRecord =
                    autoDecodeUnsafeWithOptions json CamelCase Extra.empty

                let expected =
                    {
                        Maybe = None
                        Must = "must value"
                    }

                equal expected actual

            testCase "works with maps encoded as objects"
            <| fun _ ->
                let expected =
                    Map
                        [
                            ("oh",
                             {
                                 a = 2.
                                 b = 2.
                             })
                            ("ah",
                             {
                                 a = -1.5
                                 b = 0.
                             })
                        ]

                let json = """{"ah":{"a":-1.5,"b":0},"oh":{"a":2,"b":2}}"""
                let actual = autoDecode json
                equal (Ok expected) actual

            testCase "works with maps encoded as arrays"
            <| fun _ ->
                let expected =
                    Map
                        [
                            ({
                                a = 2.
                                b = 2.
                             },
                             "oh")
                            ({
                                a = -1.5
                                b = 0.
                             },
                             "ah")
                        ]

                let json = """[[{"a":-1.5,"b":0},"ah"],[{"a":2,"b":2},"oh"]]"""
                let actual = autoDecode json
                equal (Ok expected) actual

            testCase "Decoder.Auto.toString works with bigint extra"
            <| fun _ ->
                let extra = Extra.empty |> Extra.withBigInt

                let expected =
                    {
                        bigintField = 9999999999999999999999I
                    }

                let actual =
                    autoDecodeUnsafeWithExtra
                        """{"bigintField":"9999999999999999999999"}"""
                        extra

                equal expected actual

            testCase "Decoder.Auto.toString works with custom extra"
            <| fun _ ->
                let extra =
                    Extra.empty
                    |> Extra.withCustom ChildType.Encode ChildType.Decoder

                let expected =
                    {
                        ParentField =
                            {
                                ChildField = "bumbabon"
                            }
                    }

                let actual =
                    autoDecodeUnsafeWithExtra
                        """{"ParentField":"bumbabon"}"""
                        extra

                equal expected actual

            testCase "works with records with private constructors"
            <| fun _ ->
                let json = """{ "foo1": 5, "foo2": 7.8 }"""

                let actual: RecordWithPrivateConstructor =
                    autoDecodeUnsafeWithOptions json CamelCase Extra.empty

                actual
                |> equal (
                    {
                        Foo1 = 5
                        Foo2 = 7.8
                    }
                    : RecordWithPrivateConstructor
                )

            testCase "works with unions with private constructors"
            <| fun _ ->
                let json = """[ "Baz", ["Bar", "foo"]]"""

                let actual: UnionWithPrivateConstructor list =
                    autoDecodeUnsafeWithOptions json CamelCase Extra.empty

                actual
                |> equal (
                    [
                        Baz
                        Bar "foo"
                    ]
                )

            testCase "works gives proper error for wrong union fields"
            <| fun _ ->
                let json = """["Multi", "bar", "foo", "zas"]"""

                let actual: Result<UnionWithMultipleFields, string> =
                    autoDecodeWithOptions json CamelCase Extra.empty

                actual
                |> equal (
                    Error
                        "Error at: `$.[2]`\nExpecting an int but instead got: \"foo\""
                )

            // TODO: Should we allow shorter arrays when last fields are options?
            testCase "works gives proper error for wrong array length"
            <| fun _ ->
                let json = """["Multi", "bar", 1]"""

                let actual: Result<UnionWithMultipleFields, string> =
                    autoDecodeWithOptions json CamelCase Extra.empty

                actual
                |> equal (
                    Error
                        """Error at: `$.[3]`
Expecting a longer array. Need index `3` but there are only `3` entries.
[
    "Multi",
    "bar",
    1
]"""
                )

            testCase
                "works gives proper error for wrong array length when no fields"
            <| fun _ ->
                let json = """["Multi"]"""

                let actual: Result<UnionWithMultipleFields, string> =
                    autoDecodeWithOptions json CamelCase Extra.empty

                actual
                |> equal (
                    Error
                        """Error at: `$.[1]`
Expecting a longer array. Need index `1` but there are only `1` entries.
[
    "Multi"
]"""
                )

            testCase "works gives proper error for wrong case name"
            <| fun _ ->
                let json = """[1]"""

                let actual: Result<UnionWithMultipleFields, string> =
                    autoDecodeWithOptions json CamelCase Extra.empty

                actual
                |> equal (
                    Error
                        "Error at: `$.[0]`\nExpecting a string but instead got: 1"
                )

            testCase "Auto.generateDecoderCached works"
            <| fun _ ->
                let expected =
                    Ok
                        {
                            Id = 0
                            Name = "maxime"
                            Email = "mail@domain.com"
                            Followers = 0
                        }

                let json =
                    """{ "id" : 0, "name": "maxime", "email": "mail@domain.com", "followers": 0 }"""

                let decoder1 =
                    Decode.Auto.generateDecoderCached<User> (
                        caseStrategy = CamelCase
                    )

                let decoder2 =
                    Decode.Auto.generateDecoderCached<User> (
                        caseStrategy = CamelCase
                    )

                let actual1 = runner.Decode.fromString decoder1 json
                let actual2 = runner.Decode.fromString decoder2 json
                equal expected actual1
                equal expected actual2
                equal actual1 actual2

            testCase
                "Auto.generateDecoderCached returns same decoder for same configuration"
            <| fun _ ->
                let decoder1 =
                    Decode.Auto.generateDecoderCached<User> (
                        caseStrategy = CamelCase
                    )

                let decoder2 =
                    Decode.Auto.generateDecoderCached<User> (
                        caseStrategy = CamelCase
                    )

                let json =
                    """{ "id": 0, "name": "maxime", "email": "test@test.com", "followers": 5 }"""

                let res1 = runner.Decode.unsafeFromString decoder1 json
                let res2 = runner.Decode.unsafeFromString decoder2 json

                equal res1 res2

                let expected =
                    {
                        Id = 0
                        Name = "maxime"
                        Email = "test@test.com"
                        Followers = 5
                    }

                equal expected res1

            testCase "works with strange types if they are None"
            <| fun _ ->
                let json = """{"Id":0}"""

                let res: RecordWithStrangeType = autoDecodeUnsafe json

                res
                |> equal (
                    {
                        Id = 0
                        Thread = None
                    }
                )

            testCase "works with recursive types"
            <| fun _ ->
                let vater =
                    {
                        Name = "Alfonso"
                        Children =
                            [
                                {
                                    Name = "Narumi"
                                    Children = []
                                }
                                {
                                    Name = "Takumi"
                                    Children = []
                                }
                            ]
                    }

                let json =
                    """{"Name":"Alfonso","Children":[{"Name":"Narumi","Children":[]},{"Name":"Takumi","Children":[]}]}"""

                autoDecodeUnsafe json |> equal vater

            testCase "Auto.unsafeFromString works for unit"
            <| fun _ ->
                let json = Encode.unit () |> runner.Encode.toString 4
                let res: unit = autoDecodeUnsafe json
                equal () res

            testCase "Erased single-case DUs works"
            <| fun _ ->
                let expected = NoAllocAttributeId(Guid.NewGuid())
                let json = autoEncode expected

                let actual: NoAllocAttributeId = autoDecodeUnsafe json

                equal expected actual

            testCase "Auto.unsafeFromString works with HTML inside of a string"
            <| fun _ ->
                let expected =
                    {
                        FeedName = "Ars"
                        Content =
                            "<div><figure class=\"intro-image intro-left\"><img src=\"https://cdn.arstechnica.net/wp-content/uploads/2019/05/qualcomm-enforcer-800x450.jpg\" alt=\"How Qualcomm shook down the cell phone industry for almost 20 years\"><p class=\"caption\" style=\"font-size: 0.8em\"><a href=\"https://cdn.arstechnica.net/wp-content/uploads/2019/05/qualcomm-enforcer.jpg\" class=\"enlarge-link\">Enlarge</a> (credit: Getty / Aurich Lawson)</p>  </figure><div><a name=\"page-1\"></a></div><p>In 2005, Apple contacted Qualcomm as a potential supplier for modem chips in the first iPhone. Qualcomm's response was unusual: a letter demanding that Apple sign a patent licensing agreement before Qualcomm would even consider supplying chips.</p><p>\"I'd spent 20 years in the industry, I had never seen a letter like this,\" said Tony Blevins, Apple's vice president of procurement.</p><p>Most suppliers are eager to talk to new customers—especially customers as big and prestigious as Apple. But Qualcomm wasn't like other suppliers; it enjoyed a dominant position in the market for cellular chips. That gave Qualcomm a lot of leverage, and the company wasn't afraid to use it.</p></div><p><a href=\"https://arstechnica.com/?p=1510419#p3\">Read 70 remaining paragraphs</a> | <a href=\"https://arstechnica.com/?p=1510419&amp;comments=1\">Comments</a></p><div class=\"feedflare\"><a href=\"http://feeds.arstechnica.com/~ff/arstechnica/index?a=7NLlD3YvqFA:DF_-B3_cDwc:V_sGLiPBpWU\"><img src=\"http://feeds.feedburner.com/~ff/arstechnica/index?i=7NLlD3YvqFA:DF_-B3_cDwc:V_sGLiPBpWU\" border=\"0\"></a> <a href=\"http://feeds.arstechnica.com/~ff/arstechnica/index?a=7NLlD3YvqFA:DF_-B3_cDwc:F7zBnMyn0Lo\"><img src=\"http://feeds.feedburner.com/~ff/arstechnica/index?i=7NLlD3YvqFA:DF_-B3_cDwc:F7zBnMyn0Lo\" border=\"0\"></a> <a href=\"http://feeds.arstechnica.com/~ff/arstechnica/index?a=7NLlD3YvqFA:DF_-B3_cDwc:qj6IDK7rITs\"><img src=\"http://feeds.feedburner.com/~ff/arstechnica/index?d=qj6IDK7rITs\" border=\"0\"></a> <a href=\"http://feeds.arstechnica.com/~ff/arstechnica/index?a=7NLlD3YvqFA:DF_-B3_cDwc:yIl2AUoC8zA\"><img src=\"http://feeds.feedburner.com/~ff/arstechnica/index?d=yIl2AUoC8zA\" border=\"0\"></a></div>"
                    }

                let articleJson =
                    """
                {
                    "FeedName": "Ars",
                    "Content": "<div><figure class=\"intro-image intro-left\"><img src=\"https://cdn.arstechnica.net/wp-content/uploads/2019/05/qualcomm-enforcer-800x450.jpg\" alt=\"How Qualcomm shook down the cell phone industry for almost 20 years\"><p class=\"caption\" style=\"font-size: 0.8em\"><a href=\"https://cdn.arstechnica.net/wp-content/uploads/2019/05/qualcomm-enforcer.jpg\" class=\"enlarge-link\">Enlarge</a> (credit: Getty / Aurich Lawson)</p>  </figure><div><a name=\"page-1\"></a></div><p>In 2005, Apple contacted Qualcomm as a potential supplier for modem chips in the first iPhone. Qualcomm's response was unusual: a letter demanding that Apple sign a patent licensing agreement before Qualcomm would even consider supplying chips.</p><p>\"I'd spent 20 years in the industry, I had never seen a letter like this,\" said Tony Blevins, Apple's vice president of procurement.</p><p>Most suppliers are eager to talk to new customers—especially customers as big and prestigious as Apple. But Qualcomm wasn't like other suppliers; it enjoyed a dominant position in the market for cellular chips. That gave Qualcomm a lot of leverage, and the company wasn't afraid to use it.</p></div><p><a href=\"https://arstechnica.com/?p=1510419#p3\">Read 70 remaining paragraphs</a> | <a href=\"https://arstechnica.com/?p=1510419&amp;comments=1\">Comments</a></p><div class=\"feedflare\"><a href=\"http://feeds.arstechnica.com/~ff/arstechnica/index?a=7NLlD3YvqFA:DF_-B3_cDwc:V_sGLiPBpWU\"><img src=\"http://feeds.feedburner.com/~ff/arstechnica/index?i=7NLlD3YvqFA:DF_-B3_cDwc:V_sGLiPBpWU\" border=\"0\"></a> <a href=\"http://feeds.arstechnica.com/~ff/arstechnica/index?a=7NLlD3YvqFA:DF_-B3_cDwc:F7zBnMyn0Lo\"><img src=\"http://feeds.feedburner.com/~ff/arstechnica/index?i=7NLlD3YvqFA:DF_-B3_cDwc:F7zBnMyn0Lo\" border=\"0\"></a> <a href=\"http://feeds.arstechnica.com/~ff/arstechnica/index?a=7NLlD3YvqFA:DF_-B3_cDwc:qj6IDK7rITs\"><img src=\"http://feeds.feedburner.com/~ff/arstechnica/index?d=qj6IDK7rITs\" border=\"0\"></a> <a href=\"http://feeds.arstechnica.com/~ff/arstechnica/index?a=7NLlD3YvqFA:DF_-B3_cDwc:yIl2AUoC8zA\"><img src=\"http://feeds.feedburner.com/~ff/arstechnica/index?d=yIl2AUoC8zA\" border=\"0\"></a></div>"
                }
                    """

                let actual: TestStringWithHTML = autoDecodeUnsafe articleJson

                equal expected actual

            // =====================
            // Lossless Option Tests
            // =====================

            testCase "losslessOption: encodes Some value as array with type"
            <| fun _ ->
                let value = Some 42

                let encoder =
                    Encode.Auto.generateEncoder<int option> (
                        losslessOption = true
                    )

                let json = encoder value |> runner.Encode.toString 4

                // Lossless encoding should produce: {"$type":"option","$value":42}
                Expect.stringContains
                    json
                    "\"$type\""
                    "Should contain $type field"

                Expect.stringContains
                    json
                    "\"option\""
                    "Should contain 'option' as type"

                Expect.stringContains json "42" "Should contain the value 42"

            testCase "losslessOption: encodes None as null"
            <| fun _ ->
                let expected =
                    """{
    "$type": "option",
    "$case": "none"
}"""

                let value: int option = None

                let encoder =
                    Encode.Auto.generateEncoder<int option> (
                        losslessOption = true
                    )

                let json = encoder value |> runner.Encode.toString 4

                equal json expected

            testCase "losslessOption: roundtrip with Some value"
            <| fun _ ->
                let expected = Some 42

                let encoder =
                    Encode.Auto.generateEncoder<int option> (
                        losslessOption = true
                    )

                let decoder =
                    Decode.Auto.generateDecoder<int option> (
                        losslessOption = true
                    )

                let json = encoder expected |> runner.Encode.toString 4
                let actual = runner.Decode.unsafeFromString decoder json

                equal expected actual

            testCase "losslessOption: roundtrip with None"
            <| fun _ ->
                let expected: int option = None

                let encoder =
                    Encode.Auto.generateEncoder<int option> (
                        losslessOption = true
                    )

                let decoder =
                    Decode.Auto.generateDecoder<int option> (
                        losslessOption = true
                    )

                let json = encoder expected |> runner.Encode.toString 4
                let actual = runner.Decode.unsafeFromString decoder json

                equal expected actual

            testCase
                "losslessOption: can distinguish between Some None and None"
            <| fun _ ->
                // Some None should encode differently than None
                let someNone: int option option = Some None
                let none: int option option = None

                let encoder =
                    Encode.Auto.generateEncoder<int option option> (
                        losslessOption = true
                    )

                let someNoneJson = encoder someNone |> runner.Encode.toString 0
                let noneJson = encoder none |> runner.Encode.toString 0

                // They should be different
                notEqual someNoneJson noneJson

                equal
                    someNoneJson
                    """{"$type":"option","$case":"some","$value":{"$type":"option","$case":"none"}}"""

                equal noneJson """{"$type":"option","$case":"none"}"""

            testCase "losslessOption: roundtrip with nested options"
            <| fun _ ->
                let expected: int option option = Some(Some 42)

                let encoder =
                    Encode.Auto.generateEncoder<int option option> (
                        losslessOption = true
                    )

                let decoder =
                    Decode.Auto.generateDecoder<int option option> (
                        losslessOption = true
                    )

                let json = encoder expected |> runner.Encode.toString 4
                let actual = runner.Decode.unsafeFromString decoder json

                equal expected actual

            testCase "losslessOption: roundtrip with Some None"
            <| fun _ ->
                let expected: int option option = Some None

                let encoder =
                    Encode.Auto.generateEncoder<int option option> (
                        losslessOption = true
                    )

                let decoder =
                    Decode.Auto.generateDecoder<int option option> (
                        losslessOption = true
                    )

                let json = encoder expected |> runner.Encode.toString 4
                let actual = runner.Decode.unsafeFromString decoder json

                equal expected actual

            testCase "losslessOption: works with record containing option field"
            <| fun _ ->
                let expected =
                    {
                        Maybe = Some "hello"
                        Must = "world"
                    }

                let encoder =
                    Encode.Auto.generateEncoder<TestMaybeRecord> (
                        losslessOption = true
                    )

                let decoder =
                    Decode.Auto.generateDecoder<TestMaybeRecord> (
                        losslessOption = true
                    )

                let json = encoder expected |> runner.Encode.toString 4
                let actual = runner.Decode.unsafeFromString decoder json

                equal expected actual

            testCase "losslessOption: default is false (lossy encoding)"
            <| fun _ ->
                let value = Some 42
                let lossyEncoder = Encode.Auto.generateEncoder<int option> ()

                let losslessEncoder =
                    Encode.Auto.generateEncoder<int option> (
                        losslessOption = true
                    )

                let lossyJson = lossyEncoder value |> runner.Encode.toString 4

                let losslessJson =
                    losslessEncoder value |> runner.Encode.toString 4

                // Lossy should just be the value
                equal "42" lossyJson
                // Lossless should be different
                Expect.notEqual
                    lossyJson
                    losslessJson
                    "Lossy and lossless should be different"

            testCase "losslessOption: cached encoder works correctly"
            <| fun _ ->
                let expected = Some "test"

                let encoder =
                    Encode.Auto.generateEncoderCached<string option> (
                        losslessOption = true
                    )

                let decoder =
                    Decode.Auto.generateDecoderCached<string option> (
                        losslessOption = true
                    )

                let json = encoder expected |> runner.Encode.toString 4
                let actual = runner.Decode.unsafeFromString decoder json

                equal expected actual

            testCase
                "losslessOption: different cache entries for different settings"
            <| fun _ ->
                let value = Some 123

                let lossyEncoder =
                    Encode.Auto.generateEncoderCached<int option> (
                        losslessOption = false
                    )

                let losslessEncoder =
                    Encode.Auto.generateEncoderCached<int option> (
                        losslessOption = true
                    )

                let lossyJson = lossyEncoder value |> runner.Encode.toString 0

                let losslessJson =
                    losslessEncoder value |> runner.Encode.toString 0

                // Verify they produce different outputs
                equal "123" lossyJson

                equal
                    """{"$type":"option","$case":"some","$value":123}"""
                    losslessJson

            // =====================
            // Generic Type Extra Coder Tests
            // Issue #169: Extra encoder doesn't get called for type with generic params
            // =====================

            testCase
                "Extra coder for generic type gets called for concrete instantiation"
            <| fun _ ->
                // Define a generic type wrapper
                let value: GenericWrapper<int> =
                    {
                        Node = 1234
                        Source =
                            [
                                1
                                2
                                3
                            ]
                    }

                // Create an extra encoder for the generic type (not a concrete instantiation)
                // This encoder only encodes the Node field, ignoring Source
                let cstNodeIntEncoder (cstNode: GenericWrapper<int>) =
                    Encode.int cstNode.Node

                let cstNodeIntDecoder: Decoder<GenericWrapper<int>> =
                    Decode.fail "Not implemented"

                // Register the generic coder
                let extra =
                    Extra.empty
                    |> Extra.withCustom cstNodeIntEncoder cstNodeIntDecoder

                // Generate encoder for the concrete type
                let encoder =
                    Encode.Auto.generateEncoder<GenericWrapper<int>> (
                        extra = extra
                    )

                let json = encoder value |> runner.Encode.toString 4

                // The JSON should only contain the node value (1234), not the source field
                equal "1234" json

            testCase
                "Extra coder for generic type works with string instantiation"
            <| fun _ ->
                // Test with string instead of int
                let value: GenericWrapper<string> =
                    {
                        Node = "hello"
                        Source =
                            [
                                1
                                2
                                3
                            ]
                    }

                // Create an extra encoder that only encodes the Node field
                let cstNodeStringEncoder (cstNode: GenericWrapper<string>) =
                    Encode.string cstNode.Node

                let cstNodeStringDecoder: Decoder<GenericWrapper<string>> =
                    Decode.fail "Not implemented"

                let extra =
                    Extra.empty
                    |> Extra.withCustom
                        cstNodeStringEncoder
                        cstNodeStringDecoder

                let encoder =
                    Encode.Auto.generateEncoder<GenericWrapper<string>> (
                        extra = extra
                    )

                let json = encoder value |> runner.Encode.toString 4

                // The JSON should only contain the node value ("hello"), not the source field
                equal "\"hello\"" json

            testCase
                "Extra decoder for generic type gets called for concrete instantiation"
            <| fun _ ->
                // Create an extra decoder that reconstructs the GenericWrapper from just the node value
                let cstNodeIntDecoder: Decoder<GenericWrapper<int>> =
                    Decode.int
                    |> Decode.map (fun node ->
                        {
                            Node = node
                            Source = [] // Default empty source
                        }
                    )

                // Use the standard encoder
                let cstNodeIntEncoder (cstNode: GenericWrapper<int>) =
                    Encode.object
                        [
                            "node", Encode.int cstNode.Node
                            "source",
                            Encode.list (List.map Encode.int cstNode.Source)
                        ]

                let extra =
                    Extra.empty
                    |> Extra.withCustom cstNodeIntEncoder cstNodeIntDecoder

                let decoder =
                    Decode.Auto.generateDecoder<GenericWrapper<int>> (
                        extra = extra
                    )

                // JSON just contains the node value, no source field
                let json = "1234"
                let result = runner.Decode.fromString decoder json

                let expected: GenericWrapper<int> =
                    {
                        Node = 1234
                        Source = []
                    }

                equal (Ok expected) result

            testCase "Generic type coder works with list instantiation"
            <| fun _ ->
                let value: GenericWrapper<int list> =
                    GenericWrapper<int list>.Create
                        [
                            1
                            2
                            3
                        ]
                        [
                            4
                            5
                        ]

                // Encoder that only encodes the Node field
                let cstNodeListEncoder (cstNode: GenericWrapper<int list>) =
                    Encode.list (List.map Encode.int cstNode.Node)

                let cstNodeListDecoder: Decoder<GenericWrapper<int list>> =
                    Decode.fail "Not implemented"

                let extra =
                    Extra.empty
                    |> Extra.withCustom cstNodeListEncoder cstNodeListDecoder

                let encoder =
                    Encode.Auto.generateEncoder<GenericWrapper<int list>> (
                        extra = extra
                    )

                let json = encoder value |> runner.Encode.toString 4

                // Should only contain the list [1,2,3], not the Source field
                equal "[\n    1,\n    2,\n    3\n]" json

            // =====================
            // Override Built-in Generic Types Tests
            // Issue #88: Check if user can override behaviour of generic type
            // =====================

            testCase "User can override Option<'T> encoding behavior"
            <| fun _ ->
                let value: int option = Some 42

                // Custom encoder that wraps option in a custom object
                let customOptionEncoder
                    (encoder: Encoder<int>)
                    : Encoder<int option>
                    =
                    fun (opt: int option) ->
                        match opt with
                        | Some v ->
                            Encode.object
                                [
                                    "kind", Encode.string "customSome"
                                    "data", encoder v
                                ]
                        | None ->
                            Encode.object [ "kind", Encode.string "customNone" ]

                let customOptionDecoder
                    (decoder: Decoder<int>)
                    : Decoder<int option>
                    =
                    Decode.fail "Not implemented"

                let extra =
                    Extra.empty
                    |> Extra.withCustom
                        (customOptionEncoder Encode.int)
                        (customOptionDecoder Decode.int)

                let encoder =
                    Encode.Auto.generateEncoder<int option> (extra = extra)

                let json = encoder value |> runner.Encode.toString 0

                // Should use custom encoding format
                equal json """{"kind":"customSome","data":42}"""

            testCase "User can override Option<'T> decoding behavior"
            <| fun _ ->
                // Custom decoder that reads custom format
                let customOptionDecoder
                    (decoder: Decoder<int>)
                    : Decoder<int option>
                    =
                    Decode.field "kind" Decode.string
                    |> Decode.andThen (fun kind ->
                        match kind with
                        | "customSome" ->
                            Decode.field "data" decoder |> Decode.map Some
                        | "customNone" -> Decode.succeed None
                        | _ -> Decode.fail "Unknown kind"
                    )

                let customOptionEncoder
                    (encoder: Encoder<int>)
                    : Encoder<int option>
                    =
                    fun _ -> Encode.nil

                let extra =
                    Extra.empty
                    |> Extra.withCustom
                        (customOptionEncoder Encode.int)
                        (customOptionDecoder Decode.int)

                let decoder =
                    Decode.Auto.generateDecoder<int option> (extra = extra)

                let json = """{"kind":"customSome","data":42}"""
                let result = runner.Decode.fromString decoder json

                equal result (Ok(Some 42))

            testCase "User can override Map<string, 'V> encoding behavior"
            <| fun _ ->
                let value =
                    Map.ofList
                        [
                            "a", 1
                            "b", 2
                        ]

                // Custom encoder that converts map to array of key-value objects
                let customMapEncoder
                    (encoder: Encoder<int>)
                    : Encoder<Map<string, int>>
                    =
                    fun (m: Map<string, int>) ->
                        m
                        |> Map.toList
                        |> List.map (fun (k, v) ->
                            Encode.object
                                [
                                    "key", Encode.string k
                                    "value", encoder v
                                ]
                        )
                        |> Encode.list

                let customMapDecoder
                    (decoder: Decoder<int>)
                    : Decoder<Map<string, int>>
                    =
                    Decode.fail "Not implemented"

                let extra =
                    Extra.empty
                    |> Extra.withCustom
                        (customMapEncoder Encode.int)
                        (customMapDecoder Decode.int)

                let encoder =
                    Encode.Auto.generateEncoder<Map<string, int>> (
                        extra = extra
                    )

                let json = encoder value |> runner.Encode.toString 0

                // Should use custom array format instead of default object format
                // The order of keys in a Map is not guaranteed, so we check the format structure
                // Custom format: [{"key":"a","value":1},{"key":"b","value":2}] or with keys reversed
                let expected1 =
                    """[{"key":"a","value":1},{"key":"b","value":2}]"""

                let expected2 =
                    """[{"key":"b","value":2},{"key":"a","value":1}]"""

                if json = expected1 then
                    equal json expected1
                else
                    equal json expected2

            testCase "User can override Map<string, 'V> decoding behavior"
            <| fun _ ->
                // Custom decoder that reads array of key-value objects
                let customMapDecoder
                    (decoder: Decoder<int>)
                    : Decoder<Map<string, int>>
                    =
                    Decode.list (
                        Decode.object (fun get ->
                            let key = get.Required.Field "key" Decode.string
                            let value = get.Required.Field "value" decoder
                            key, value
                        )
                    )
                    |> Decode.map Map.ofList

                let customMapEncoder
                    (encoder: Encoder<int>)
                    : Encoder<Map<string, int>>
                    =
                    fun _ -> Encode.nil

                let extra =
                    Extra.empty
                    |> Extra.withCustom
                        (customMapEncoder Encode.int)
                        (customMapDecoder Decode.int)

                let decoder =
                    Decode.Auto.generateDecoder<Map<string, int>> (
                        extra = extra
                    )

                let json = """[{"key":"x","value":10},{"key":"y","value":20}]"""
                let result = runner.Decode.fromString decoder json

                let expected =
                    Map.ofList
                        [
                            "x", 10
                            "y", 20
                        ]

                equal result (Ok expected)

            testCase "User can override List<'T> encoding behavior"
            <| fun _ ->
                let value =
                    [
                        1
                        2
                        3
                    ]

                // Custom encoder that wraps list in metadata
                let customListEncoder
                    (encoder: Encoder<int>)
                    : Encoder<int list>
                    =
                    fun (lst: int list) ->
                        Encode.object
                            [
                                "count", Encode.int (List.length lst)
                                "items", Encode.list (List.map encoder lst)
                            ]

                let customListDecoder
                    (decoder: Decoder<int>)
                    : Decoder<int list>
                    =
                    Decode.fail "Not implemented"

                let extra =
                    Extra.empty
                    |> Extra.withCustom
                        (customListEncoder Encode.int)
                        (customListDecoder Decode.int)

                let encoder =
                    Encode.Auto.generateEncoder<int list> (extra = extra)

                let json = encoder value |> runner.Encode.toString 0

                // Should use custom object format with count and items
                equal json """{"count":3,"items":[1,2,3]}"""

            testCase "User can override List<'T> decoding behavior"
            <| fun _ ->
                // Custom decoder that unwraps metadata
                let customListDecoder
                    (decoder: Decoder<int>)
                    : Decoder<int list>
                    =
                    Decode.field "items" (Decode.list decoder)

                let customListEncoder
                    (encoder: Encoder<int>)
                    : Encoder<int list>
                    =
                    fun _ -> Encode.nil

                let extra =
                    Extra.empty
                    |> Extra.withCustom
                        (customListEncoder Encode.int)
                        (customListDecoder Decode.int)

                let decoder =
                    Decode.Auto.generateDecoder<int list> (extra = extra)

                let json = """{"count":3,"items":[1,2,3]}"""
                let result = runner.Decode.fromString decoder json

                equal
                    result
                    (Ok
                        [
                            1
                            2
                            3
                        ])

            // =====================
            // Override Primitive Types Tests
            // Verify users can override built-in primitive encoders/decoders
            // =====================

            testCase "User can override string encoding behavior"
            <| fun _ ->
                let value = "hello"

                // Custom encoder that wraps strings
                let customStringEncoder (s: string) =
                    Encode.object
                        [
                            "type", Encode.string "string"
                            "value", Encode.string s
                        ]

                let customStringDecoder: Decoder<string> =
                    Decode.fail "Not implemented"

                let extra =
                    Extra.empty
                    |> Extra.withCustom customStringEncoder customStringDecoder

                let encoder =
                    Encode.Auto.generateEncoder<string> (extra = extra)

                let json = encoder value |> runner.Encode.toString 0

                // Should use custom object format
                equal json """{"type":"string","value":"hello"}"""

            testCase "User can override string decoding behavior"
            <| fun _ ->
                // Custom decoder that unwraps string object
                let customStringDecoder: Decoder<string> =
                    Decode.field "value" Decode.string

                let customStringEncoder (s: string) = Encode.nil

                let extra =
                    Extra.empty
                    |> Extra.withCustom customStringEncoder customStringDecoder

                let decoder =
                    Decode.Auto.generateDecoder<string> (extra = extra)

                let json = """{"type":"string","value":"world"}"""
                let result = runner.Decode.fromString decoder json

                equal result (Ok "world")

            testCase "User can override int encoding behavior"
            <| fun _ ->
                let value = 42

                // Custom encoder that wraps ints in hex format string
                let customIntEncoder (i: int) =
                    Encode.object
                        [
                            "type", Encode.string "int"
                            "hex", Encode.string (sprintf "0x%X" i)
                        ]

                let customIntDecoder: Decoder<int> =
                    Decode.fail "Not implemented"

                let extra =
                    Extra.empty
                    |> Extra.withCustom customIntEncoder customIntDecoder

                let encoder = Encode.Auto.generateEncoder<int> (extra = extra)

                let json = encoder value |> runner.Encode.toString 0

                // Should use custom object format
                equal json """{"type":"int","hex":"0x2A"}"""

            testCase "User can override int decoding behavior"
            <| fun _ ->
                // Custom decoder that reads hex string
                let customIntDecoder: Decoder<int> =
                    Decode.field "hex" Decode.string
                    |> Decode.map (fun hexStr ->
                        if hexStr.StartsWith("0x") then
                            Convert.ToInt32(hexStr, 16)
                        else
                            int hexStr
                    )

                let customIntEncoder (i: int) = Encode.nil

                let extra =
                    Extra.empty
                    |> Extra.withCustom customIntEncoder customIntDecoder

                let decoder = Decode.Auto.generateDecoder<int> (extra = extra)

                let json = """{"type":"int","hex":"0x2A"}"""
                let result = runner.Decode.fromString decoder json

                equal result (Ok 42)

            testCase "User can override bool encoding behavior"
            <| fun _ ->
                let value = true

                // Custom encoder that encodes bool as string
                let customBoolEncoder (b: bool) =
                    Encode.string (
                        if b then
                            "yes"
                        else
                            "no"
                    )

                let customBoolDecoder: Decoder<bool> =
                    Decode.fail "Not implemented"

                let extra =
                    Extra.empty
                    |> Extra.withCustom customBoolEncoder customBoolDecoder

                let encoder = Encode.Auto.generateEncoder<bool> (extra = extra)

                let json = encoder value |> runner.Encode.toString 0

                // Should use custom string format (bool encoded as "yes" for true)
                equal json "\"yes\""

            testCase "User can override bool decoding behavior"
            <| fun _ ->
                // Custom decoder that reads bool from string
                let customBoolDecoder: Decoder<bool> =
                    Decode.string
                    |> Decode.map (fun s ->
                        match s.ToLower() with
                        | "yes"
                        | "true" -> true
                        | "no"
                        | "false" -> false
                        | _ -> failwith "Invalid boolean string"
                    )

                let customBoolEncoder (b: bool) = Encode.nil

                let extra =
                    Extra.empty
                    |> Extra.withCustom customBoolEncoder customBoolDecoder

                let decoder = Decode.Auto.generateDecoder<bool> (extra = extra)

                let json = "\"yes\""
                let resultYes = runner.Decode.fromString decoder json
                let json2 = "\"no\""
                let resultNo = runner.Decode.fromString decoder json2

                equal resultYes (Ok true)
                equal resultNo (Ok false)
        ]
