namespace Thoth.Json.System.Text.Json

open Thoth.Json.Core
open System.Text.Json
open System.Text.Json.Nodes
open System.IO

[<RequireQualifiedAccess>]
module Encode =

    let helpers =
        { new IEncoderHelpers<JsonNode> with
            member _.encodeString value = JsonValue.Create(value)
            member _.encodeChar value = JsonValue.Create(value)
            member _.encodeDecimalNumber value = JsonValue.Create(value)
            member _.encodeBool value = JsonValue.Create(value)
            member _.encodeNull() = JsonValue.Create(null)

            member _.encodeObject(values) =
                let o = JsonObject()

                for key, value in values do
                    o.Add(key, value)

                o

            member _.encodeArray values = JsonArray(values)
            member _.encodeList values = JsonArray(values |> Seq.toArray)
            member _.encodeSeq values = JsonArray(values |> Seq.toArray)
            member _.encodeResizeArray values = JsonArray(values |> Seq.toArray)

            member _.encodeSignedIntegralNumber(value: int32) =
                JsonValue.Create(value)

            member _.encodeUnsignedIntegralNumber(value: uint32) =
                JsonValue.Create(value)
        }

    let toString (space: int) (value: IEncodable) : string =
        let json = Encode.toJsonValue helpers value
        let writeIndented = space > 0

        let options =
            JsonSerializerOptions(
                WriteIndented = writeIndented,
                NewLine = "\n",
                IndentSize = space,
                Encoder =
                    System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            )

        JsonSerializer.Serialize(json, options)
