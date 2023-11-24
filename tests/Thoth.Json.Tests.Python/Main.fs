module Thoth.Json.Tests.Python

open Fable.Pyxpecto
open Thoth.Json.Tests.Testing
open Thoth.Json.Core
open Thoth.Json.Python
open System
open Thoth.Json.Tests.Types
open Fable.Core.Testing

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


    testList "All" [
        Decoders.tests runner
        Encoders.tests runner

        runner.testCase
            "get.Optional.Field returns None value if field is missing"
        <| fun _ ->
            let json = """{ "age": 25 }"""
            let expected = Ok({ optionalField = None })

            let decoder =
                Decode.object (fun get -> {
                    optionalField = get.Optional.Field "name" Decode.string
                })

            let actual = runner.Decode.fromString decoder json

            printfn "%A" actual
            printfn "%A" expected

            runner.equal expected actual
    ]
    |> Pyxpecto.runTests
