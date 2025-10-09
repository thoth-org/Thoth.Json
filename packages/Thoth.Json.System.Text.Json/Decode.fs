namespace Thoth.Json.System.Text.Json

open Thoth.Json.Core
open System.Text.Json

[<RequireQualifiedAccess>]
module Decode =

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
                let d = ref (JsonElement())

                jsonValue.ValueKind = JsonValueKind.Object
                && jsonValue.TryGetProperty(fieldName, d)

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
                let options =
                    JsonSerializerOptions(
                        WriteIndented = true,
                        NewLine = "\n",
                        IndentSize = 4
                    )

                JsonSerializer.Serialize(jsonValue, options)
        }

    let fromValue (decoder: Decoder<'T>) =
        Decode.Advanced.fromValue helpers decoder

    let fromString (decoder: Decoder<'T>) =
        fun (value: string) ->
            try
                let options = JsonDocumentOptions(AllowTrailingCommas = true)

                let jsonDocument = JsonDocument.Parse(value, options)

                match decoder.Decode(helpers, jsonDocument.RootElement) with
                | Ok success -> Ok success
                | Error error ->
                    let finalError = error |> Decode.Helpers.prependPath "$"
                    Error(Decode.errorToString helpers finalError)
            with :? JsonException as ex ->
                Error("Given an invalid JSON: " + ex.Message)

    let unsafeFromString (decoder: Decoder<'T>) =
        fun value ->
            match fromString decoder value with
            | Ok x -> x
            | Error e -> failwith e
