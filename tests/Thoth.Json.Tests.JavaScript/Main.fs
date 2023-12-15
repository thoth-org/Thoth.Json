module Thoth.Json.Tests.JavaScript

open Fable.Mocha
open Thoth.Json.Tests.Testing
open Fable.Core.Testing
open Thoth.Json.Core
open Thoth.Json.JavaScript
open Fable.Core.JsInterop

type JavaScriptEncode() =
    interface IEncode with
        override _.toString spaces json = Encode.toString spaces json

type JavaScriptDecode() =
    interface IDecode with
        override _.fromString decoder json = Decode.fromString decoder json

type JavascriptTestRunner() =
    inherit TestRunner<TestCase, obj>()

    override _.testList = testList
    override _.testCase = testCase

    override _.equal a b = Assert.AreEqual(b, a)

    override _.Encode = JavaScriptEncode()

    override _.Decode = JavaScriptDecode()

[<EntryPoint>]
let main args =
    let runner = JavascriptTestRunner()

    testList
        "All"
        [

            runner.testCase
                "circular structure are supported when reporting error"
            <| fun _ ->
                let a = createObj []
                let b = createObj []
                a?child <- b
                b?child <- a

                let expected: Result<float, string> =
                    Error
                        "Error at: ``\nExpecting a float but decoder failed. Couldn\'t report given value due to circular structure. "

                let actual = Decode.fromValue Decode.helpers Decode.float b

                runner.equal expected actual

            Decoders.tests runner
            Encoders.tests runner

        ]
    |> Mocha.runTests
