namespace Thoth.Json.Newtonsoft

open Thoth.Json.Core
open Newtonsoft.Json
open Newtonsoft.Json.Linq
open System.IO

[<RequireQualifiedAccess>]
module Encode =

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
    /// Serialize a value to a JSON string using a caller-supplied
    /// <see cref="T:Newtonsoft.Json.JsonSerializerSettings"/>, giving full
    /// control over the underlying Newtonsoft serializer (for example to set
    /// <c>Formatting</c>, date handling or string escaping).
    /// </summary>
    /// <remarks>
    /// Indentation follows <c>settings.Formatting</c>; the indent <em>width</em>
    /// is the Newtonsoft default of two spaces and is not configurable through
    /// settings. Use <see cref="M:toString"/> when you need a specific indent
    /// width.
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
