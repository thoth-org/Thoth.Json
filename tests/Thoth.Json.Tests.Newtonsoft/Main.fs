module Thoth.Json.Tests.Newtonsoft

open Expecto
open Thoth.Json.Tests.Testing
open Thoth.Json.Core
open Thoth.Json.Newtonsoft

type NewtonsoftEncode() =
    interface IEncode with
        override _.toString spaces json = Encode.toString spaces json

type NewtonsoftDecode() =
    interface IDecode with
        override _.fromString decoder json = Decode.fromString decoder json

type NewtonsoftTestRunner() =
    inherit TestRunner<Test, obj>()

    override _.testList = testList
    override _.testCase = testCase
    override _.ftestCase = ftestCase

    override _.equal actual expected = Expect.equal actual expected ""

    override _.Encode = NewtonsoftEncode()

    override _.Decode = NewtonsoftDecode()

[<EntryPoint>]
let main args =
    let runner = NewtonsoftTestRunner()

    testList
        "All"
        [
            Decoders.tests runner
            Encoders.tests runner
            runner.testCase
                "field output an error explaining why the value is considered invalid"
            <| fun _ ->
                let json = """{ "name": null, "age": 25 }"""

                let expected =
                    Error(
                        """
Error at: `$.name`
Expecting an int but instead got: null
                        """
                            .Trim()
                    )

                let actual =
                    runner.Decode.fromString
                        (Decode.field "name" Decode.int)
                        json

                runner.equal expected actual
        ]
    |> runTestsWithArgs defaultConfig args
