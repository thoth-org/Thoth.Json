module Thoth.Json.Tests.Codec.Combinators

#if !NETFRAMEWORK
open Fable.Core
#endif

open Thoth.Json.Tests.Testing
open Fable.Pyxpecto

open Thoth.Json.Core

type Tree = | Tree of value: int * children: Tree list

module Tree =

    let codec: Codec<Tree> =
        Codec.fix (fun self ->
            objectCodec {
                let! value =
                    Codec.field "value" (fun (Tree(v, _)) -> v) Codec.int

                and! children =
                    Codec.field
                        "children"
                        (fun (Tree(_, c)) -> c)
                        (Codec.list self)

                return Tree(value, children)
            }
        )

/// A scalar mirroring a JSON primitive. Decoding dispatches on the raw JSON
/// shape, which is why the codec is built on top of Codec.value.
type Scalar =
    | SString of string
    | SNumber of float
    | SBool of bool

module Scalar =

    let codec: Codec<Scalar> =
        Codec.value
        |> Codec.map
            (function
            | Json.String s -> SString s
            | Json.Number n -> SNumber n
            | Json.Boolean b -> SBool b
            | json -> failwith $"Unsupported scalar: %A{json}")
            (function
            | SString s -> Json.String s
            | SNumber n -> Json.Number n
            | SBool b -> Json.Boolean b
            )

let tests (runner: TestRunner<'DecoderJsonValue, 'EncoderJsonValue>) =
    testList
        "Combinators"
        [
            test "Codec.lossyOption works with Some value" {
                let codec = Codec.lossyOption Codec.string

                let expected = Some "abc"

                let actual = roundTrip runner codec expected

                equal actual expected
            }

            test "Codec.lossyOption works with None" {
                let codec = Codec.lossyOption Codec.string

                let expected = None

                let actual = roundTrip runner codec expected

                equal actual expected
            }

            test "Codec.oneOf works" {
                // Reads a number or a single-element array, writes a number.
                let codec =
                    Codec.oneOf
                        [
                            Codec.int
                            Codec.create
                                (fun (i: int) -> Encode.list [ Encode.int i ])
                                (Decode.index 0 Decode.int)
                        ]

                let expected = 42

                let actual = roundTrip runner codec expected

                equal actual expected
            }

            test "Codec.oneOf decodes a fallback representation" {
                let codec =
                    Codec.oneOf
                        [
                            Codec.int
                            Codec.create
                                (fun (i: int) -> Encode.list [ Encode.int i ])
                                (Decode.index 0 Decode.int)
                        ]

                let actual =
                    runner.Decode.fromString (Decode.codec codec) "[ 42 ]"

                equal actual (Ok 42)
            }

            test "Codec.oneOf reports an error when no codec matches" {
                let codec =
                    Codec.oneOf
                        [
                            Codec.int
                            Codec.create
                                (fun (i: int) -> Encode.list [ Encode.int i ])
                                (Decode.index 0 Decode.int)
                        ]

                let expected =
                    Error(
                        """
The following errors were found:

Error at: `$`
Expecting an int but instead got: true

Error at: `$`
Expecting an array but instead got: true
                        """
                            .Trim()
                    )

                let actual =
                    runner.Decode.fromString (Decode.codec codec) "true"

                equal actual expected
            }

            test "Codec.oneOf throws for an empty list" {
                Expect.throwsC
                    (fun () -> (Codec.oneOf []: Codec<int>) |> ignore)
                    (fun exn ->
                        equal
                            exn.Message
                            "Codec.oneOf requires at least one codec (Parameter 'codecs')"
                    )
            }

            test "Codec.list works" {
                let codec = Codec.list Codec.int

                let expected =
                    [
                        4
                        6
                        1
                        2
                        2
                        8
                        0
                        5
                        3
                    ]

                let actual = roundTrip runner codec expected

                equal actual expected
            }

            test "Codec.array works" {
                let codec = Codec.array Codec.string

                let expected =
                    [|
                        "the"
                        "quick"
                        "brown"
                        "fox"
                        "jumped"
                        "over"
                        "the"
                        "lazy"
                        "dog"
                    |]

                let actual = roundTrip runner codec expected

                equal actual expected
            }

            test "Codec.seq works" {
                let codec = Codec.seq Codec.int

                let expected =
                    seq
                        [
                            4
                            6
                            1
                            2
                            2
                            8
                        ]

                let actual = roundTrip runner codec expected

                equal (List.ofSeq actual) (List.ofSeq expected)
            }

            test "Codec.seq reports an error for an invalid element" {
                let codec = Codec.seq Codec.int

                let expected =
                    Error(
                        """
Error at: `$.[1]`
Expecting an int but instead got: "two"
                        """
                            .Trim()
                    )

                let actual =
                    runner.Decode.fromString
                        (Decode.codec codec)
                        """[ 1, "two", 3 ]"""

                equal actual expected
            }

            test "Codec.resizeArray works" {
                let codec = Codec.resizeArray Codec.string

                let expected =
                    ResizeArray
                        [
                            "the"
                            "quick"
                            "brown"
                            "fox"
                        ]

                let actual = roundTrip runner codec expected

                equal (List.ofSeq actual) (List.ofSeq expected)
            }

            test "Codec.resizeArray reports an error for an invalid value" {
                let codec = Codec.resizeArray Codec.string

                let expected =
                    Error(
                        """
Error at: `$`
Expecting a ResizeArray but instead got: "not an array"
                        """
                            .Trim()
                    )

                let actual =
                    runner.Decode.fromString
                        (Decode.codec codec)
                        "\"not an array\""

                equal actual expected
            }

            test "Codec.dict works" {
                let codec = Codec.dict Codec.int

                let expected =
                    Map.ofList
                        [
                            "a", 1
                            "b", 2
                            "c", 3
                        ]

                let actual = roundTrip runner codec expected

                equal actual expected
            }

            test "Codec.dict reports an error for an invalid value" {
                let codec = Codec.dict Codec.int

                let expected =
                    Error(
                        """
Error at: `$.a`
Expecting an int but instead got: "not an int"
                        """
                            .Trim()
                    )

                let actual =
                    runner.Decode.fromString
                        (Decode.codec codec)
                        """{ "a": "not an int" }"""

                equal actual expected
            }

            test "Codec.map' works" {
                let codec = Codec.map' Codec.int Codec.string

                let expected =
                    Map.ofList
                        [
                            1, "one"
                            2, "two"
                            3, "three"
                        ]

                let actual = roundTrip runner codec expected

                equal actual expected
            }

            test "Codec.map' reports an error for an invalid value" {
                let codec = Codec.map' Codec.int Codec.string

                let expected =
                    Error(
                        """
Error at: `$.[0].[1]`
Expecting a string but instead got: 2
                        """
                            .Trim()
                    )

                let actual =
                    runner.Decode.fromString
                        (Decode.codec codec)
                        """[ [ 1, 2 ] ]"""

                equal actual expected
            }

            test "Codec.value round-trips raw JSON" {
                let expected =
                    Json.Object
                        [
                            "name", Json.String "Kaladin"
                            "age", Json.Number 23.0
                            "active", Json.Boolean true
                            "tags",
                            Json.Array
                                [
                                    Json.String "windrunner"
                                    Json.Null
                                ]
                        ]

                let actual = roundTrip runner Codec.value expected

                equal actual expected
            }

            test "Codec.nil works" {
                let codec = Codec.nil 42

                let actual = roundTrip runner codec 42

                equal actual 42
            }

            test "Codec.nil reports an error for a non-null value" {
                let codec = Codec.nil 42

                let expected =
                    Error(
                        """
Error at: `$`
Expecting null but instead got: 5
                        """
                            .Trim()
                    )

                let actual = runner.Decode.fromString (Decode.codec codec) "5"

                equal actual expected
            }

            test "Codec.keyValuePairs works" {
                let codec = Codec.keyValuePairs Codec.int

                let expected =
                    [
                        "a", 1
                        "b", 2
                        "c", 3
                    ]

                let actual = roundTrip runner codec expected

                equal actual expected
            }

            test "Codec.keyValuePairs reports an error for an invalid value" {
                let codec = Codec.keyValuePairs Codec.int

                let expected =
                    Error(
                        """
Error at: `$.a`
Expecting an int but instead got: "not an int"
                        """
                            .Trim()
                    )

                let actual =
                    runner.Decode.fromString
                        (Decode.codec codec)
                        """{ "a": "not an int" }"""

                equal actual expected
            }

            test "Codec.at works" {
                let codec =
                    Codec.at
                        [
                            "data"
                            "user"
                        ]
                        Codec.string

                let expected = "Shallan"

                let actual = roundTrip runner codec expected

                equal actual expected
            }

            test "Codec.at reports an error for a missing path" {
                let codec =
                    Codec.at
                        [
                            "data"
                            "user"
                        ]
                        Codec.string

                let expected =
                    Error(
                        """
Error at: `$.data.user`
Expecting an object with path `data.user` but instead got:
{}
Node `user` is unknown.
                        """
                            .Trim()
                    )

                let actual = runner.Decode.fromString (Decode.codec codec) "{}"

                equal actual expected
            }

            test "Codec.lazily works" {
                let codec = Codec.lazily (lazy Codec.int)

                let actual = roundTrip runner codec 123

                equal actual 123
            }

            test "Codec.fix works for a recursive type" {
                let expected =
                    Tree(
                        1,
                        [
                            Tree(2, [])
                            Tree(3, [ Tree(4, []) ])
                        ]
                    )

                let actual = roundTrip runner Tree.codec expected

                equal actual expected
            }

            test "Codec.value |> Codec.map builds a custom codec" {
                let str = roundTrip runner Scalar.codec (SString "abc")
                equal str (SString "abc")

                let num = roundTrip runner Scalar.codec (SNumber 42.5)
                equal num (SNumber 42.5)

                let bln = roundTrip runner Scalar.codec (SBool true)
                equal bln (SBool true)
            }
        ]
