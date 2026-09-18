module Thoth.Json.Tests.JavaScript

open Thoth.Json.Tests.Testing
open Thoth.Json.Core
open Thoth.Json.Core.Auto
open Thoth.Json.JavaScript
open Fable.Core.JsInterop
open Scriptorium.Nib.Assertion
open type Scriptorium.Quill.Test
open type Scriptorium.Quill.Runner

type JavaScriptEncode() =
    interface IEncode with
        override _.toString spaces json = Encode.toString spaces json

type JavaScriptDecode() =
    interface IDecode<obj> with
        override _.fromValue<'T>(decoder: Decoder<'T>) =
            Decode.fromValue decoder

        override _.fromString<'T> (decoder: Decoder<'T>) json =
            Decode.fromString decoder json

        override _.unsafeFromString<'T> (decoder: Decoder<'T>) json =
            Decode.unsafeFromString decoder json

type JavascriptTestRunner() =
    inherit TestRunner<obj, obj>()

    override _.Encode = JavaScriptEncode()

    override _.Decode = JavaScriptDecode()

    override _.EncoderHelpers = Encode.helpers

    override _.DecoderHelpers = Decode.helpers

    override _.MapEncoderValueToDecoderValue(encoderValue: obj) : obj =
        id encoderValue

let exactNumbersTests =
    let options =
        { DecodeOptions.defaults with
            ExactNumbers = true
        }

    testList (
        "DecodeOptions.ExactNumbers",
        [
            test (
                "a number written with a fraction is not an integer",
                fun _ ->
                    let actual =
                        Decode.fromStringWithOptions
                            (options, Decode.int16)
                            "25.0"

                    assertThat
                        actual
                        (Result.errorValue
                         >> contains "Value is not an integral value")
            )

            test (
                "a number written without a fraction still is",
                fun _ ->
                    let actual =
                        Decode.fromStringWithOptions
                            (options, Decode.int16)
                            "25"

                    assertThat actual (isEqualTo (Ok 25s))
            )

            test (
                "the reported value keeps the literal",
                fun _ ->
                    let actual =
                        Decode.fromStringWithOptions
                            (options, Decode.int16)
                            "25.0"

                    assertThat actual (Result.errorValue >> contains "25.0")
            )

            test (
                "a large integer keeps its digits",
                fun _ ->
                    let actual =
                        Decode.fromStringWithOptions
                            (options, Decode.int64)
                            "9223372036854775806"

                    assertThat actual (isEqualTo (Ok 9223372036854775806L))
            )

            test (
                "floats are untouched",
                fun _ ->
                    let actual =
                        Decode.fromStringWithOptions
                            (options, Decode.float)
                            "25.5"

                    assertThat actual (isEqualTo (Ok 25.5))
            )

            test (
                "a decoded record is unaffected",
                fun _ ->
                    let actual =
                        Decode.fromStringWithOptions
                            (options, Decode.field "a" Decode.int)
                            """{"a":1}"""

                    assertThat actual (isEqualTo (Ok 1))
            )

            test (
                "the default options keep the JSON.parse behaviour",
                fun _ ->
                    let actual = Decode.fromString Decode.int16 "25.0"

                    assertThat actual (isEqualTo (Ok 25s))
            )

            test (
                "a reviver injected by the caller reaches fromValue",
                fun _ ->
                    let parsed: obj =
                        emitJsExpr
                            Decode.numberLiteralReviver
                            """JSON.parse("25.0", $0)"""

                    let actual = Decode.fromValue Decode.int16 parsed

                    assertThat
                        actual
                        (Result.errorValue
                         >> contains "Value is not an integral value")
            )
        ]
    )

[<EntryPoint>]
let main args =
    let runner = JavascriptTestRunner()

    runTests (
        testList (
            "All",
            [

                test (
                    "circular structure are supported when reporting error",
                    fun _ ->
                        let a = createObj []
                        let b = createObj []
                        a?child <- b
                        b?child <- a

                        let expected: Result<float, string> =
                            Error
                                "Error at: ``\nExpecting a float but decoder failed. Couldn\'t report given value due to circular structure. "

                        let actual = Decode.fromValue Decode.float b

                        equal actual expected
                )

                Decoders.tests runner
                Encoders.tests runner
                BackAndForth.tests runner
                DecoderCE.tests runner
                Auto.tests runner
                Codec.Primitives.tests runner
                Codec.Combinators.tests runner
                Codec.ObjectCodec.tests runner
                Codec.VariantCodec.tests runner
                Codec.AutoCodec.tests runner
                Properties.tests runner
                exactNumbersTests

            ]
        )
    )
