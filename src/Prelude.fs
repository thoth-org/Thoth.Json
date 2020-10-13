namespace Thoth.Json

open System.Text
open Fable.Core
open System
open Thoth.Json.Parser

type JsonValue = Json

module Helpers =

    let anyToString(value: JsonValue): string =
        let builder = StringBuilder()
        value.Write(builder, JsonSaveOptions.Format, 4)
        builder.ToString()

    let asArray(value: JsonValue): JsonValue [] =
        match value with
        | Json.Array values ->
            values
        | _ ->
            failwith "Given JSON values is not an array"

    let asBool(value: JsonValue): bool =
        match value with
        | Json.Bool value ->
            value
        | _ ->
            failwith "Given JSON values is not a boolean"

    let asFloat(value: JsonValue): float =
        match value with
        | Json.Number value ->
            value
        | _ ->
            failwith "Given JSON values is not a number"

    let asFloat32(value: JsonValue): float32 =
        match value with
        | Json.Number value ->
            float32 value
        | _ ->
            failwith "Given JSON values is not a number"

    let asInt(value: JsonValue): int =
        match value with
        | Json.Number value ->
            int value
        | _ ->
            failwith "Given JSON values is not a number"

    let asString(value: JsonValue): string =
        match value with
        | Json.String value ->
            value
        | _ ->
            failwith "Given JSON values is not a string"

    let getField(fieldName: string) (value: JsonValue): JsonValue =
        match value with
        | Json.Object properties ->
            match Map.tryFind fieldName properties with
            | Some value -> value
            | None ->
                Json.Undefined
        | _ ->
            failwith "Given JSON values is not a number"

    let isArray(value: JsonValue): bool =
        match value with
        | Json.Array _ -> true
        | _ -> false

    let isBoolean(value: JsonValue): bool =
        match value with
        | Json.Bool _ -> true
        | _ -> false

    let isIntegralValue(value: JsonValue): bool =
        match value with
        | Json.Number value ->
            // Mimic Thoth.Json.Fable implementation
            Math.Floor(value) = value
        | _ -> false

    let isNullValue(value: JsonValue): bool =
        match value with
        | Json.Null
        | Json.Undefined -> true
        | _ -> false

    let isNumber(value: JsonValue): bool =
        match value with
        | Json.Number _ -> true
        | _ -> false

    let isObject(value: JsonValue): bool =
        match value with
        | Json.Object _ -> true
        | _ -> false

    let isString(value: JsonValue): bool =
        match value with
        | Json.String _ -> true
        | _ -> false

    let isUndefined(value: JsonValue): bool =
        // Is it the correct way? Or should I use "undefined" for JavaScript runtime and isNull for .NET runtime?
        value = Json.Undefined

    let objectKeys(value: JsonValue): seq<string> =
        match value with
        | Json.Object properties ->
            properties
            |> Map.toSeq
            |> Seq.map fst
        | _ ->
            failwith "Given JSON values is not an object"

    let objectKeyValues(value: JsonValue): seq<string * JsonValue> =
        match value with
        | Json.Object properties ->
            properties
            |> Map.toSeq
        | _ ->
            failwith "Given JSON values is not an object"
