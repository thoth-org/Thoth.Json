namespace Thoth.Json.System.Text.Json

open Thoth.Json.Core
open System.Text
open System.Text.Json
open System.Text.Json.Nodes
open System.IO

[<RequireQualifiedAccess>]
module Encode =

    /// <summary>
    /// Builds a <c>JsonNode</c>, so an encoder written against Thoth.Json.Core runs on System.Text.Json.
    /// </summary>
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

    /// <summary>
    /// Write an encodable as a JSON string with caller-supplied
    /// <see cref="T:System.Text.Json.JsonSerializerOptions"/>, giving control over indentation,
    /// escaping and depth.
    /// </summary>
    let toStringWithOptions
        (options: JsonSerializerOptions)
        (value: IEncodable)
        : string
        =
        let json = Encode.toJsonValue helpers value

        use stream = new MemoryStream()

        use writer =
            new Utf8JsonWriter(
                stream,
                JsonWriterOptions(
                    Indented = options.WriteIndented,
                    IndentCharacter = options.IndentCharacter,
                    IndentSize = options.IndentSize,
                    NewLine = options.NewLine,
                    Encoder = options.Encoder,
                    MaxDepth = options.MaxDepth
                )
            )

        match json with
        | null -> writer.WriteNullValue()
        | node -> node.WriteTo(writer)

        writer.Flush()

        Encoding.UTF8.GetString(stream.ToArray())

    /// <summary>
    /// Write an encodable as a JSON string, indented by the given number of spaces. Pass <c>0</c>
    /// for a single line.
    /// </summary>
    let toString (space: int) (value: IEncodable) : string =
        let writeIndented = space > 0

        let options =
            JsonSerializerOptions(
                WriteIndented = writeIndented,
                NewLine = "\n",
                IndentSize = space,
                Encoder =
                    System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            )

        toStringWithOptions options value
