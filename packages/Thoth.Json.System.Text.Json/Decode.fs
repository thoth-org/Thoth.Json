namespace Thoth.Json.System.Text.Json

open Thoth.Json.Core
open System.Text.Json
open System.IO
open System.Text

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
                jsonValue.ValueKind = JsonValueKind.Object
                && jsonValue.EnumerateObject()
                   |> Seq.exists (fun prop -> prop.Name = fieldName)

            member _.isIntegralValue jsonValue =
                jsonValue.ValueKind = JsonValueKind.Number
                && jsonValue.GetRawText().IndexOf('.') = -1 // Integral value should not have a decimal point

            member _.asString jsonValue = jsonValue.GetString()
            member _.asBoolean jsonValue = jsonValue.GetBoolean()

            member _.asArray jsonValue =
                jsonValue.EnumerateArray() |> Seq.toArray
            // jsonValue.Value<JArray>().Values() |> Seq.toArray

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
        // if isNull jsonValue then
        //     "null"
        // else
        //     use stream = new StringWriter(NewLine = "\n")

        //     use jsonWriter =
        //         new JsonTextWriter(
        //             stream,
        //             Formatting = Formatting.Indented,
        //             Indentation = 4
        //         )

        //     jsonValue.WriteTo(jsonWriter)
        //     stream.ToString()

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
