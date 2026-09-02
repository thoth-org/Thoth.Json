namespace Thoth.Json.Newtonsoft

open Thoth.Json.Core
open Newtonsoft.Json
open Newtonsoft.Json.Linq
open System.Globalization
open System.IO

[<RequireQualifiedAccess>]
module Decode =

    /// <summary>
    /// Reads a <c>JToken</c>, so a decoder written against Thoth.Json.Core runs on Newtonsoft.Json.
    /// </summary>
    let helpers =
        { new IDecoderHelpers<JToken> with
            member _.isString jsonValue =
                not (isNull jsonValue) && jsonValue.Type = JTokenType.String

            member _.isNumber jsonValue =
                not (isNull jsonValue)
                && (jsonValue.Type = JTokenType.Float
                    || jsonValue.Type = JTokenType.Integer)

            member _.isBoolean jsonValue =
                not (isNull jsonValue) && jsonValue.Type = JTokenType.Boolean

            member _.isNullValue jsonValue =
                isNull jsonValue || jsonValue.Type = JTokenType.Null

            member _.isArray jsonValue =
                not (isNull jsonValue) && jsonValue.Type = JTokenType.Array

            member _.isObject jsonValue =
                not (isNull jsonValue) && jsonValue.Type = JTokenType.Object

            member _.hasProperty fieldName jsonValue =
                not (isNull jsonValue)
                && jsonValue.Type = JTokenType.Object
                && jsonValue.Value<JObject>().Properties()
                   |> Seq.exists (fun prop -> prop.Name = fieldName)

            member _.isIntegralValue jsonValue =
                not (isNull jsonValue) && (jsonValue.Type = JTokenType.Integer)

            member _.asString jsonValue = jsonValue.Value<string>()
            member _.asBoolean jsonValue = jsonValue.Value<bool>()

            member _.asArray jsonValue =
                jsonValue.Value<JArray>().Values() |> Seq.toArray

            member _.asFloat jsonValue = jsonValue.Value<float>()
            member _.asFloat32 jsonValue = jsonValue.Value<float32>()
            member _.asInt jsonValue = jsonValue.Value<int>()

            member _.getProperties jsonValue =
                jsonValue.Value<JObject>().Properties()
                |> Seq.map (fun prop -> prop.Name)

            member _.getProperty(fieldName: string, jsonValue: JToken) =
                jsonValue[fieldName]

            member _.anyToString jsonValue =
                if isNull jsonValue then
                    "null"
                else
                    use stream = new StringWriter(NewLine = "\n")

                    use jsonWriter =
                        new JsonTextWriter(
                            stream,
                            Formatting = Formatting.Indented,
                            Indentation = 4
                        )

                    jsonValue.WriteTo(jsonWriter)
                    stream.ToString()

            member _.numberToString jsonValue =
                if isNull jsonValue then
                    "null"
                elif jsonValue.Type = JTokenType.Float then
                    jsonValue
                        .Value<float>()
                        .ToString("R", CultureInfo.InvariantCulture)
                else
                    jsonValue.ToString()
        }

/// <summary>
/// Runs a decoder against Newtonsoft.Json.
/// </summary>
type Decode =

    /// <summary>
    /// Run a decoder against a <c>JToken</c> which is already parsed.
    /// </summary>
    static member fromValue(decoder: Decoder<'T>) =
        Decode.Advanced.fromValue Decode.helpers decoder

    /// <summary>
    /// Run the decoder half of a codec against a <c>JToken</c> which is already parsed.
    /// </summary>
    static member fromValue(codec: Codec<'T>) =
        codec |> Decode.codec |> Decode.fromValue

    /// <summary>
    /// Parse a JSON string with caller-supplied
    /// <see cref="T:Newtonsoft.Json.JsonSerializerSettings"/> and run the decoder against it.
    /// </summary>
    /// <remarks>
    /// The settings <b>replace</b> the ones <see cref="M:fromString"/> uses, they are not merged.
    /// Two of them are what Thoth.Json relies on, so set both unless you have a reason not to:
    /// <list type="bullet">
    /// <item><description>
    /// <c>DateParseHandling = DateParseHandling.None</c> - Newtonsoft otherwise turns date-like
    /// strings into <c>Date</c> tokens, which the date decoders and <c>Decode.string</c> then
    /// reject.
    /// </description></item>
    /// <item><description>
    /// <c>CheckAdditionalContent = true</c> - so content after the JSON value is reported as
    /// invalid instead of being ignored.
    /// </description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code lang="fsharp">
    /// let settings =
    ///     JsonSerializerSettings(
    ///         DateParseHandling = DateParseHandling.None,
    ///         CheckAdditionalContent = true,
    ///         MaxDepth = 256
    ///     )
    ///
    /// json |> Decode.fromStringWithOptions(settings, decoder)
    /// </code>
    /// </example>
    static member fromStringWithOptions
        (settings: JsonSerializerSettings, decoder: Decoder<'T>)
        =
        fun (value: string) ->
            try
                let serializer = JsonSerializer.Create(settings)

                use reader = new JsonTextReader(new StringReader(value))
                let res = serializer.Deserialize<JToken>(reader)

                match decoder.Decode(Decode.helpers, res) with
                | Ok success -> Ok success
                | Error error ->
                    let finalError = error |> Decode.Helpers.prependPath "$"

                    Error(Decode.errorToString Decode.helpers finalError)
            with :? JsonException as ex ->
                Error("Given an invalid JSON: " + ex.Message)

    /// <summary>
    /// Parse a JSON string with caller-supplied
    /// <see cref="T:Newtonsoft.Json.JsonSerializerSettings"/> and run the decoder half of a codec
    /// against it.
    /// </summary>
    /// <remarks>
    /// The settings <b>replace</b> the ones <see cref="M:fromString"/> uses, they are not merged.
    /// Two of them are what Thoth.Json relies on, so set both unless you have a reason not to:
    /// <list type="bullet">
    /// <item><description>
    /// <c>DateParseHandling = DateParseHandling.None</c> - Newtonsoft otherwise turns date-like
    /// strings into <c>Date</c> tokens, which the date decoders and <c>Decode.string</c> then
    /// reject.
    /// </description></item>
    /// <item><description>
    /// <c>CheckAdditionalContent = true</c> - so content after the JSON value is reported as
    /// invalid instead of being ignored.
    /// </description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code lang="fsharp">
    /// let settings =
    ///     JsonSerializerSettings(
    ///         DateParseHandling = DateParseHandling.None,
    ///         CheckAdditionalContent = true,
    ///         MaxDepth = 256
    ///     )
    ///
    /// json |> Decode.fromStringWithOptions(settings, codec)
    /// </code>
    /// </example>
    static member fromStringWithOptions
        (settings: JsonSerializerSettings, codec: Codec<'T>)
        =
        Decode.fromStringWithOptions (settings, Decode.codec codec)

    /// <summary>
    /// Parse a JSON string and run the decoder against it.
    /// </summary>
    /// <returns>
    /// <c>Ok</c> with the decoded value, or <c>Error</c> with the formatted message.
    /// </returns>
    static member fromString(decoder: Decoder<'T>) =
        let settings =
            JsonSerializerSettings(
                DateParseHandling = DateParseHandling.None,
                CheckAdditionalContent = true
            )

        Decode.fromStringWithOptions (settings, decoder)

    /// <summary>
    /// Parse a JSON string and run the decoder half of a codec against it.
    /// </summary>
    static member fromString(codec: Codec<'T>) =
        codec |> Decode.codec |> Decode.fromString

    /// <summary>
    /// Parse a JSON string and run the decoder, raising an exception carrying the message on
    /// failure.
    /// </summary>
    static member unsafeFromString(decoder: Decoder<'T>) =
        fun value ->
            match Decode.fromString decoder value with
            | Ok x -> x
            | Error e -> failwith e

    /// <summary>
    /// Parse a JSON string and run the decoder half of a codec, raising an exception carrying the
    /// message on failure.
    /// </summary>
    static member unsafeFromString(codec: Codec<'T>) =
        codec |> Decode.codec |> Decode.unsafeFromString
