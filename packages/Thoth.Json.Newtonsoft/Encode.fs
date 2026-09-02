namespace Thoth.Json.Newtonsoft

open Thoth.Json.Core
open Newtonsoft.Json
open Newtonsoft.Json.Linq
open System.IO

[<RequireQualifiedAccess>]
module Encode =

    /// <summary>
    /// Builds a <c>JToken</c>, so an encoder written against Thoth.Json.Core runs on Newtonsoft.Json.
    /// </summary>
    let helpers =
        { new IEncoderHelpers<JToken> with
            member _.encodeString value = JValue(value)
            member _.encodeChar value = JValue(value)
            member _.encodeDecimalNumber value = JValue(value)
            member _.encodeBool value = JValue(value)
            member _.encodeNull() = JValue.CreateNull()

            member _.encodeObject(values) =
                let o = JObject()

                for key, value in values do
                    o.[key] <- value

                o

            member _.encodeArray values = JArray(values)
            member _.encodeList values = JArray(values)
            member _.encodeSeq values = JArray(values)
            member _.encodeResizeArray values = JArray(values)

            member _.encodeSignedIntegralNumber(value: int32) = JValue(value)

            member _.encodeUnsignedIntegralNumber(value: uint32) =
                // We need to force the cast to uint64 here, otherwise
                // Newtonsoft resolve the constructor to JValue(decimal)
                // when we actually want to output a number without decimals
                JValue(uint64 value)
        }

    /// <summary>
    /// Write an encodable as a JSON string with caller-supplied
    /// <see cref="T:Newtonsoft.Json.JsonSerializerSettings"/>.
    /// </summary>
    /// <remarks>
    /// Indentation follows <c>settings.Formatting</c>. Its width is Newtonsoft's own two spaces and
    /// settings cannot change it, so use <see cref="M:toString"/> when you need another width.
    /// </remarks>
    /// <example>
    /// <code lang="fsharp">
    /// let settings = JsonSerializerSettings(Formatting = Formatting.Indented)
    ///
    /// value |> Encode.toStringWithOptions settings
    /// </code>
    /// </example>
    let toStringWithOptions
        (settings: JsonSerializerSettings)
        (value: IEncodable)
        : string
        =
        let serializer = JsonSerializer.Create(settings)

        use stream = new StringWriter(NewLine = "\n")
        use jsonWriter = new JsonTextWriter(stream)

        let json = Encode.toJsonValue helpers value
        serializer.Serialize(jsonWriter, json)
        stream.ToString()

    /// <summary>
    /// Write an encodable as a JSON string, indented by the given number of spaces. Pass <c>0</c>
    /// for a single line.
    /// </summary>
    let toString (space: int) (value: IEncodable) : string =
        let format =
            if space = 0 then
                Formatting.None
            else
                Formatting.Indented

        use stream = new StringWriter(NewLine = "\n")

        use jsonWriter =
            new JsonTextWriter(stream, Formatting = format, Indentation = space)

        let json = Encode.toJsonValue helpers value
        json.WriteTo(jsonWriter)
        stream.ToString()
