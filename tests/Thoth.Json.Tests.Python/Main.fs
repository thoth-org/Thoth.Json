module Thoth.Json.Tests.Python

open Fable.Pyxpecto
open Thoth.Json.Tests.Testing
open Thoth.Json.Core
open Thoth.Json.Python
open System
open Thoth.Json.Tests.Types
open Fable.Core.Testing
open Fable.Pyxpecto.Model

type PythonEncode() =
    interface IEncode with
        override _.toString a b = Encode.toString a b

type PythonDecode() =
    interface IDecode<obj> with
        override _.fromValue a = Decode.fromValue a

        override _.fromString a b = Decode.fromString a b

        override _.unsafeFromString decoder json =
            Decode.unsafeFromString decoder json

type PythonTestRunner() =
    inherit TestRunner<TestCase, obj>()

    override _.testList = testList
    override _.testCase = testCase
    override _.ftestCase = ftestCase

    override _.equal expected actual = Expect.equal actual expected ""

    override _.Encode = PythonEncode()

    override _.Decode = PythonDecode()

    override _.EncoderHelpers = Encode.helpers

    override _.DecoderHelpers = Decode.helpers

[<EntryPoint>]
let main args =
    let runner = PythonTestRunner()

    testList
        "All"
        [
            Decoders.tests runner
            Encoders.tests runner
        ]
    |> Pyxpecto.runTests [||]
