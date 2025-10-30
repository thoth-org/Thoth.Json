module Thoth.Json.Tests.JavaScript

open Thoth.Json.Tests.Testing
open Thoth.Json.Core
open Thoth.Json.JavaScript
open Fable.Core.JsInterop
open Fable.Pyxpecto

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

[<EntryPoint>]
let main args =
    let runner = JavascriptTestRunner()

    testList
        "All"
        [

            testCase "circular structure are supported when reporting error"
            <| fun _ ->
                let a = createObj []
                let b = createObj []
                a?child <- b
                b?child <- a

                let expected: Result<float, string> =
                    Error
                        "Error at: ``\nExpecting a float but decoder failed. Couldn\'t report given value due to circular structure. "

                let actual = Decode.fromValue Decode.float b

                equal expected actual

            Decoders.tests runner
            Encoders.tests runner
            BackAndForth.tests runner
            Syntax.tests runner

        ]
    |> Pyxpecto.runTests [||]
