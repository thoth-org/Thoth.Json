namespace Thoth.Json.Newtonsoft

open System.IO
open Newtonsoft.Json
open Newtonsoft.Json.Linq

type JsonValue = Linq.JToken

module Helpers =

    let anyToString(value: JsonValue): string =
        if isNull value then "null"
        else
            use stream = new StringWriter(NewLine = "\n")
            use jsonWriter = new JsonTextWriter(
                                    stream,
                                    Formatting = Formatting.Indented,
                                    Indentation = 4 )

            value.WriteTo(jsonWriter)
            stream.ToString()

    let asArray(value: JsonValue): JsonValue [] =
        value.Value<JArray>().Values() |> Seq.toArray

    let asBool(value: JsonValue): bool =
        value.Value<bool>()

    let asFloat(value: JsonValue): float =
        value.Value<float>()

    let asFloat32(value: JsonValue): float32 =
        value.Value<float32>()

    let asInt(value: JsonValue): int =
        value.Value<int>()

    let asString(value: JsonValue): string =
        value.Value<string>()

    let getField(fieldName: string) (value: JsonValue): JsonValue =
        value.Item(fieldName)

    let isArray(value: JsonValue): bool =
        not(isNull value) && value.Type = JTokenType.Array

    let isBoolean(value: JsonValue): bool =
        not(isNull value) && value.Type = JTokenType.Boolean

    let isIntegralValue(value: JsonValue): bool =
        not(isNull value) && (value.Type = JTokenType.Integer)

    let isNullValue(value: JsonValue): bool =
        isNull value || value.Type = JTokenType.Null

    let isNumber(value: JsonValue): bool =
        not(isNull value) && (value.Type = JTokenType.Float || value.Type = JTokenType.Integer)

    let isObject(value: JsonValue): bool =
        not(isNull value) && value.Type = JTokenType.Object

    let isString(value: JsonValue): bool =
        not(isNull value) && value.Type = JTokenType.String

    let isUndefined(value: JsonValue): bool =
        isNull value

    let objectKeys(value: JsonValue): seq<string> =
        let value = value.Value<JObject>()
        value.Properties()
        |> Seq.map (fun prop ->
            prop.Name
        )

    let objectKeyValues(value: JsonValue) : seq<string * JToken> =
        let value = value.Value<JObject>()
        value.Properties()
        |> Seq.map (fun prop ->
            prop.Name, prop.Value
        )
