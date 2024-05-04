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

            member _.encodeSignedIntegralNumber(value: int32) = JValue(value)

            member _.encodeUnsignedIntegralNumber(value: uint32) =
                // We need to force the cast to uint64 here, otherwise
                // Newtonsoft resolve the constructor to JValue(decimal)
                // when we actually want to output a number without decimals
                JValue(uint64 value)
        }

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
