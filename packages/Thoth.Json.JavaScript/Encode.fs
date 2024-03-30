namespace Thoth.Json.JavaScript

open Fable.Core
open Fable.Core.JsInterop
open Thoth.Json.Core
open System.Globalization

[<RequireQualifiedAccess>]
module Encode =

    let helpers =
        { new IEncoderHelpers<obj> with
            member _.encodeString value = box value
            member _.encodeChar value = box value
            member _.encodeDecimalNumber value = box value
            member _.encodeBool value = box value
            member _.encodeNull() = box null
            member _.createEmptyObject() = obj ()

            member _.setPropertyOnObject(o: obj, key: string, value: obj) =
                o?(key) <- value

            member _.encodeArray values = JS.Constructors.Array.from values
            member _.encodeList values = JS.Constructors.Array.from values
            member _.encodeSeq values = JS.Constructors.Array.from values

            member _.encodeIntegralNumber value = box value
        }

    let toString (space: int) (value: IEncodable) : string =
        let json = Encode.toJsonValue helpers value
        JS.JSON.stringify (json, space = space)
