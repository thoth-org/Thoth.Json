module Thoth.Json.Tests.System.Text.Json

open Thoth.Json.Tests.Testing
open Thoth.Json.System.Text.Json
open System.Text.Json
open System.Text.Json.Nodes
open Fable.Pyxpecto
open Thoth.Json.Tests

type SystemTextJsonEncode() =
    interface IEncode with
        override _.toString spaces json = Encode.toString spaces json

type SystemTextJsonEncodeDecode() =
    interface IDecode<JsonElement> with
        override _.fromValue decoder = Decode.fromValue decoder

        override _.fromString decoder json = Decode.fromString decoder json

        override _.unsafeFromString decoder json =
            Decode.unsafeFromString decoder json

type SystemTextJsonTestRunner() =
    inherit TestRunner<JsonElement, JsonNode>()

    override _.Encode = SystemTextJsonEncode()

    override _.Decode = SystemTextJsonEncodeDecode()

    override _.EncoderHelpers = Encode.helpers

    override _.DecoderHelpers = Decode.helpers

    override _.MapEncoderValueToDecoderValue
        (encoderValue: JsonNode)
        : JsonElement
        =
        JsonSerializer.Deserialize<JsonElement>(encoderValue)

[<EntryPoint>]
let main args =
    let runner = SystemTextJsonTestRunner()

    testList
        "All"
        [
            Decoders.tests runner
            Encoders.tests runner
            BackAndForth.tests runner
            Syntax.tests runner
        ]
    |> Pyxpecto.runTests [||]
