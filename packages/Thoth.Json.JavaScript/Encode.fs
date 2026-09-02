namespace Thoth.Json.JavaScript

open Fable.Core
open Fable.Core.JsInterop
open Thoth.Json.Core
open System.Globalization

[<RequireQualifiedAccess>]
module Encode =

    /// <summary>
    /// Builds a <c>obj</c>, so an encoder written against Thoth.Json.Core runs on JavaScript.
    /// </summary>
    let helpers =
        { new IEncoderHelpers<obj> with
            member _.encodeString value = box value
            member _.encodeChar value = box value
            member _.encodeDecimalNumber value = box value
            member _.encodeBool value = box value
            member _.encodeNull() = box null

            member _.encodeObject(values) =
                let o = obj ()

                for key, value in values do
                    o?(key) <- value

                o

            member _.encodeArray values = JS.Constructors.Array.from values
            member _.encodeList values = JS.Constructors.Array.from values
            member _.encodeSeq values = JS.Constructors.Array.from values

            member _.encodeResizeArray values =
                JS.Constructors.Array.from values

            member _.encodeSignedIntegralNumber value = box value
            member _.encodeUnsignedIntegralNumber value = box value
        }

    /// <summary>
    /// Write an encodable as a JSON string, indented by the given number of spaces. Pass <c>0</c>
    /// for a single line.
    /// </summary>
    let toString (space: int) (value: IEncodable) : string =
        let json = Encode.toJsonValue helpers value
        JS.JSON.stringify (json, space = space)

    /// <summary>
    /// Write a value as a JSON string with the encoder half of a codec.
    /// </summary>
    let fromCodec (space: int) (codec: Codec<'T>) (value: 'T) : string =
        value |> Encode.codec codec |> toString space
