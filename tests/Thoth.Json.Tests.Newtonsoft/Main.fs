module Thoth.Json.Tests.Newtonsoft

open Thoth.Json.Tests.Testing
open Thoth.Json.Newtonsoft
open Newtonsoft.Json.Linq
open Fable.Pyxpecto

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
    inherit TestRunner<JToken, JToken>()

    override _.Encode = NewtonsoftEncode()

    override _.Decode = NewtonsoftDecode()

    override _.EncoderHelpers = Encode.helpers

    override _.DecoderHelpers = Decode.helpers

    override _.MapEncoderValueToDecoderValue(encoderValue: JToken) : JToken =
        id encoderValue

[<EntryPoint>]
let main args =
    let runner = NewtonsoftTestRunner()

    testList
        "All"
        [
            Decoders.tests runner
            Encoders.tests runner
            BackAndForth.tests runner
            Syntax.tests runner
        ]
    |> Pyxpecto.runTests [||]
