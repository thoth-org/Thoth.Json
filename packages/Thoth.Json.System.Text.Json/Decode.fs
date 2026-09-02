namespace Thoth.Json.System.Text.Json

open Thoth.Json.Core
open System.IO
open System.Text
open System.Text.Json

[<RequireQualifiedAccess>]
module Decode =

    /// <summary>
    /// Reads a <c>JsonElement</c>, so a decoder written against Thoth.Json.Core runs on System.Text.Json.
    /// </summary>
    let helpers =
        { new IDecoderHelpers<JsonElement> with
            member _.isString jsonValue =
                jsonValue.ValueKind = JsonValueKind.String

            member _.isNumber jsonValue =
                jsonValue.ValueKind = JsonValueKind.Number

            member _.isBoolean jsonValue =
                jsonValue.ValueKind = JsonValueKind.True
                || jsonValue.ValueKind = JsonValueKind.False

            member _.isNullValue jsonValue =
                jsonValue.ValueKind = JsonValueKind.Null

            member _.isArray jsonValue =
                jsonValue.ValueKind = JsonValueKind.Array

            member _.isObject jsonValue =
                jsonValue.ValueKind = JsonValueKind.Object

            member _.hasProperty fieldName jsonValue =
                // Using TryGetProperty is faster than iterating over the properties
                // and checking their names
                // Result of a benchmark:
                // Iteration: 5,661.0 ns
                // TryGetProperty: 4,734.3 ns
                match jsonValue.ValueKind with
                | JsonValueKind.Object ->
                    jsonValue.TryGetProperty fieldName |> fst
                | _ -> false

            member _.isIntegralValue jsonValue =
                jsonValue.ValueKind = JsonValueKind.Number
                && jsonValue.GetRawText().IndexOf('.') = -1 // Integral value should not have a decimal point

            member _.asString jsonValue = jsonValue.GetString()
            member _.asBoolean jsonValue = jsonValue.GetBoolean()

            member _.asArray jsonValue =
                jsonValue.EnumerateArray() |> Seq.toArray

            member _.asFloat jsonValue = jsonValue.GetDouble()
            member _.asFloat32 jsonValue = jsonValue.GetSingle()
            member _.asInt jsonValue = jsonValue.GetInt32()

            member _.getProperties jsonValue =
                jsonValue.EnumerateObject() |> Seq.map (fun prop -> prop.Name)

            member _.getProperty(fieldName: string, jsonValue: JsonElement) =
                jsonValue.GetProperty(fieldName)

            member _.anyToString jsonValue =
                use stream = new MemoryStream()

                use writer =
                    new Utf8JsonWriter(
                        stream,
                        JsonWriterOptions(
                            Indented = true,
                            NewLine = "\n",
                            IndentSize = 4
                        )
                    )

                jsonValue.WriteTo(writer)
                writer.Flush()

                Encoding.UTF8.GetString(stream.ToArray())

            member _.numberToString jsonValue =
                let mutable int64Value = 0L

                if jsonValue.TryGetInt64(&int64Value) then
                    string int64Value
                else
                    let d = jsonValue.GetDouble()

                    if System.Math.Floor(d) = d then
                        string (int64 d)
                    else
                        d.ToString(
                            "R",
                            System.Globalization.CultureInfo.InvariantCulture
                        )
        }

/// <summary>
/// Runs a decoder against System.Text.Json.
/// </summary>
type Decode =

    /// <summary>
    /// Run a decoder against a <c>JsonElement</c> which is already parsed.
    /// </summary>
    static member fromValue(decoder: Decoder<'T>) =
        Decode.Advanced.fromValue Decode.helpers decoder

    /// <summary>
    /// Run the decoder half of a codec against a <c>JsonElement</c> which is already parsed.
    /// </summary>
    static member fromValue(codec: Codec<'T>) =
        codec |> Decode.codec |> Decode.fromValue

    /// <summary>
    /// Parse a JSON string with caller-supplied
    /// <see cref="T:System.Text.Json.JsonDocumentOptions"/> and run the decoder against it.
    /// </summary>
    /// <remarks>
    /// Use it to raise <c>MaxDepth</c> beyond its default of 64, to allow trailing commas, or to
    /// skip comments.
    /// </remarks>
    /// <example>
    /// <code lang="fsharp">
    /// let options = JsonDocumentOptions(MaxDepth = 256)
    ///
    /// json |> Decode.fromStringWithOptions(options, decoder)
    /// </code>
    /// </example>
    static member fromStringWithOptions
        (options: JsonDocumentOptions, decoder: Decoder<'T>)
        =
        fun (value: string) ->
            try
                use jsonDocument = JsonDocument.Parse(value, options)

                match
                    decoder.Decode(Decode.helpers, jsonDocument.RootElement)
                with
                | Ok success -> Ok success
                | Error error ->
                    let finalError = error |> Decode.Helpers.prependPath "$"

                    Error(Decode.errorToString Decode.helpers finalError)
            with :? JsonException as ex ->
                Error("Given an invalid JSON: " + ex.Message)

    /// <summary>
    /// Parse a JSON string with caller-supplied
    /// <see cref="T:System.Text.Json.JsonDocumentOptions"/> and run the decoder half of a codec
    /// against it.
    /// </summary>
    /// <example>
    /// <code lang="fsharp">
    /// let options = JsonDocumentOptions(MaxDepth = 256)
    ///
    /// json |> Decode.fromStringWithOptions(options, codec)
    /// </code>
    /// </example>
    static member fromStringWithOptions
        (options: JsonDocumentOptions, codec: Codec<'T>)
        =
        Decode.fromStringWithOptions (options, Decode.codec codec)

    /// <summary>
    /// Parse a JSON string and run the decoder against it.
    /// </summary>
    /// <returns>
    /// <c>Ok</c> with the decoded value, or <c>Error</c> with the formatted message.
    /// </returns>
    static member fromString(decoder: Decoder<'T>) =
        let options = JsonDocumentOptions()

        Decode.fromStringWithOptions (options, decoder)

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
