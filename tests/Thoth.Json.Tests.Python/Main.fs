module Thoth.Json.Tests.Python

open Fable.Pyxpecto
open Thoth.Json.Tests.Testing
open Thoth.Json.Core
open Thoth.Json.Python
open System

type PythonEncode() =
    interface IEncode with
        override _.toString a b = Encode.toString a b

type PythonDecode() =
    interface IDecode with
        override _.fromString a b = Decode.fromString a b

type PythonTestRunner() =
    inherit TestRunner<TestCase, obj>()

    override _.testList = testList
    override _.testCase = testCase

    override _.equal a b = Expect.equal b a ""

    override _.Encode = PythonEncode()

    override _.Decode = PythonDecode()

[<EntryPoint>]
let main args =
    let runner = PythonTestRunner()

    try
        testList
            "All"
            [
                Decoders.tests runner
                Encoders.tests runner
            ]
        |> Pyxpecto.runTests
    with
        | _ ->
            1
