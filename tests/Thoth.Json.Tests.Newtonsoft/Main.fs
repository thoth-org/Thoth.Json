module Thoth.Json.Tests.Newtonsoft

open Expecto
open Thoth.Json.Tests.Testing
open Thoth.Json.Core
open Thoth.Json.Newtonsoft
open Newtonsoft.Json.Linq

type NewtonsoftEncode() =
    interface IEncode with
        override _.toString spaces json = Encode.toString spaces json

type NewtonsoftDecode() =
    interface IDecode<JToken> with
        override _.fromValue decoder = Decode.fromValue decoder

        override _.fromString decoder json = Decode.fromString decoder json

        override _.unsafeFromString decoder json =
            Decode.unsafeFromString decoder json

type NewtonsoftTestRunner() =
    inherit TestRunner<Test, JToken>()

    override _.testList = testList
    override _.testCase = testCase
    override _.ftestCase = ftestCase

    override _.equal expected actual = Expect.equal actual expected ""

    override _.Encode = NewtonsoftEncode()

    override _.Decode = NewtonsoftDecode()

    override _.EncoderHelpers = Encode.helpers

    override _.DecoderHelpers = Decode.helpers

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
