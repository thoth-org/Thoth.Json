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
            member _.createEmptyObject() = JObject()

            member _.setPropertyOnObject
                (
                    o: JToken,
                    key: string,
                    value: JToken
                )
                =
                o[key] <- value

            member _.encodeArray values = JArray(values)
            member _.encodeList values = JArray(values)
            member _.encodeSeq values = JArray(values)
            member _.encodeIntegralNumber(value: uint32) =
                // We need to force the cast to uint64 here, otherwise
                // Newtonsoft resolve the constructor to JValue(decimal)
                // when we actually want to output a number without decimals
                JValue(uint64 value)
        }

    let toString (space: int) (value: Json) : string =
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
