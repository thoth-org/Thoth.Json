module Thoth.Json.Tests.JavaScript

open Fable.Mocha
open Thoth.Json.Tests.Testing
open Fable.Core.Testing
open Thoth.Json.Core
open Thoth.Json.JavaScript
open Fable.Core.JsInterop

type JavaScriptEncode () =
    interface IEncode with
        override _.toString spaces json = Encode.toString spaces json

type JavaScriptDecode () =
    interface IDecode with
        override _.fromString decoder json = Decode.fromString decoder json

type JavascriptTestRunner() =
    inherit TestRunner<TestCase, obj>()

    override _.testList = testList
    override _.testCase = testCase

    override _.equal a b = Assert.AreEqual(a, b)

    override _.Encode = JavaScriptEncode()

    override _.Decode = JavaScriptDecode()

[<EntryPoint>]
let main args =
    let runner = JavascriptTestRunner()

    testList
        "All"
        [

            runner.testCase "circular structure are supported when reporting error" <| fun _ ->
                let a = createObj [ ]
                let b = createObj [ ]
                a?child <- b
                b?child <- a

                let expected : Result<float, string> = Error "Error at: `$`\nExpecting a float but decoder failed. Couldn\'t report given value due to circular structure. "
                let actual = Decode.fromValue Decode.helpers "$" Decode.float b

                runner.equal expected actual

            Decoders.tests runner
            Encoders.tests runner

            runner.testCase "field output an error explaining why the value is considered invalid" <| fun _ ->
                let json = """{ "name": null, "age": 25 }"""
                let expected =
                    Error(
                        """
Error at: `$.name`
Expecting an int but instead got: null
                        """.Trim()
                    )

                let actual =
                    runner.Decode.fromString (Decode.field "name" Decode.int) json

                runner.equal expected actual

            runner.testCase "at works" <| fun _ ->

                let json = """{ "user": { "name": "maxime", "age": 25 } }"""
                let expected = Ok "maxime"

                let actual =
                    runner.Decode.fromString (Decode.at ["user"; "name"] Decode.string) json

                runner.equal expected actual
        ]
    |> Mocha.runTests
