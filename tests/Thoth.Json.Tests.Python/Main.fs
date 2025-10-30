module Thoth.Json.Tests.Python

open Fable.Pyxpecto
open Thoth.Json.Tests.Testing
open Thoth.Json.Python

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
    inherit TestRunner<obj, obj>()

    override _.Encode = PythonEncode()

    override _.Decode = PythonDecode()

    override _.EncoderHelpers = Encode.helpers

    override _.DecoderHelpers = Decode.helpers

    override _.MapEncoderValueToDecoderValue(encoderValue: obj) : obj =
        id encoderValue

[<EntryPoint>]
let main args =
    let runner = PythonTestRunner()

    testList
        "All"
        [
            Decoders.tests runner
            Encoders.tests runner
            BackAndForth.tests runner
            Syntax.tests runner
        ]
    |> Pyxpecto.runTests [||]
