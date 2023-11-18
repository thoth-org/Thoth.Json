module Thoth.Json.Tests.Newtonsoft

open Expecto
open Thoth.Json.Tests.Testing
open Thoth.Json.Core
open Thoth.Json.Newtonsoft

type NewtonsoftEncode () =
    interface IEncode with
        override _.toString = Encode.toString

type NewtonsoftDecode () =
    interface IDecode with
        override _.fromString a b = Decode.fromString a b

type NewtonsoftTestRunner() =
    inherit TestRunner<Test, obj>()

    override _.testList = testList
    override _.testCase = testCase

    override _.equal a b = Expect.equal (box a) b ""

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
