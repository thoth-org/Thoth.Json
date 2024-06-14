namespace Thoth.Json.Newtonsoft

open Thoth.Json.Core
open Newtonsoft.Json
open Newtonsoft.Json.Linq
open System.IO

[<RequireQualifiedAccess>]
module Decode =

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
        }

    let fromValue (decoder: Decoder<'T>) =
        Decode.Advanced.fromValue helpers decoder

    let fromString (decoder: Decoder<'T>) =
        fun value ->
            try
                let serializationSettings =
                    JsonSerializerSettings(
                        DateParseHandling = DateParseHandling.None,
                        CheckAdditionalContent = true
                    )

                let serializer = JsonSerializer.Create(serializationSettings)

                use reader = new JsonTextReader(new StringReader(value))
                let res = serializer.Deserialize<JToken>(reader)

                match decoder.Decode(helpers, res) with
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
