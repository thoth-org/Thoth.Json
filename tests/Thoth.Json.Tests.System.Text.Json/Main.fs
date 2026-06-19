module Thoth.Json.Tests.System.Text.Json

open Thoth.Json.Tests.Testing
open Thoth.Json.Core
open Thoth.Json.System.Text.Json
open System.Text.Json
open System.Text.Json.Nodes
open Fable.Pyxpecto
open Thoth.Json.Tests

// Build a JSON value nested `depth` objects deep: {"a":{"a":...{"a":1}...}}
let private deeplyNestedJson (depth: int) =
    let mutable json = "1"

    for _ in 1..depth do
        json <- "{\"a\":" + json + "}"

    json

// Decoder that drills `depth` levels down through the "a" fields
let rec private nestedDecoder (depth: int) : Decoder<int> =
    if depth = 0 then
        Decode.int
    else
        Decode.field "a" (nestedDecoder (depth - 1))

// Same shape as a codec (the encoder is irrelevant, we only decode here)
let private nestedCodec (depth: int) : Thoth.Json.Core.Codec<int> =
    Thoth.Json.Core.Codec.create
        (fun (_: int) -> Thoth.Json.Core.Encode.nil)
        (nestedDecoder depth)

let backendSpecificTests =
    testList
        "System.Text.Json specific"
        [
            // Issue #194: allow customising the underlying JSON library, e.g.
            // raising System.Text.Json's default MaxDepth of 64.
            testCase "fromString fails on JSON deeper than the default MaxDepth"
            <| fun _ ->
                let json = deeplyNestedJson 80

                let actual = Decode.fromString (nestedDecoder 80) json

                match actual with
                | Error msg ->
                    Expect.isTrue
                        (msg.Contains "depth")
                        $"Expected a depth error but got: {msg}"
                | Ok _ ->
                    failwith "Expected a depth error but decoding succeeded"

            testCase
                "fromStringWithOptions can raise MaxDepth to decode deep JSON"
            <| fun _ ->
                let json = deeplyNestedJson 80
                let options = JsonDocumentOptions(MaxDepth = 256)

                let actual =
                    Decode.fromStringWithOptions
                        (options, nestedDecoder 80)
                        json

                equal (Ok 1) actual

            testCase
                "fromStringWithOptions accepts a codec and applies the options"
            <| fun _ ->
                let json = deeplyNestedJson 80
                let options = JsonDocumentOptions(MaxDepth = 256)

                let actual =
                    Decode.fromStringWithOptions (options, nestedCodec 80) json

                equal (Ok 1) actual

            testCase "toStringWithOptions honors the provided options"
            <| fun _ ->
                let value =
                    Thoth.Json.Core.Encode.object
                        [ "a", Thoth.Json.Core.Encode.int 1 ]

                let options =
                    JsonSerializerOptions(
                        WriteIndented = true,
                        NewLine = "\n",
                        IndentSize = 2
                    )

                let actual = Encode.toStringWithOptions options value

                equal "{\n  \"a\": 1\n}" actual
        ]

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
            DecoderCE.tests runner
            Auto.tests runner
            Codec.Primitives.tests runner
            Codec.Combinators.tests runner
            Codec.ObjectCodec.tests runner
            Codec.VariantCodec.tests runner
            Codec.AutoCodec.tests runner
            backendSpecificTests
        ]
    |> Pyxpecto.runTests [||]
