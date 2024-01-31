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

        override _.unsafeFromString decoder json =
            Decode.unsafeFromString decoder json

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
        ]
    |> runTestsWithArgs defaultConfig args
