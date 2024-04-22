namespace Thoth.Json.JavaScript

open Fable.Core
open Fable.Core.JsInterop
open Thoth.Json.Core

[<RequireQualifiedAccess>]
module Encode =

    let helpers =
        { new IEncoderHelpers<obj> with
            member _.encodeString value = box value
            member _.encodeChar value = box value
            member _.encodeDecimalNumber value = box value
            member _.encodeBool value = box value
            member _.encodeNull() = box null

            member _.encodeObject(values: (string * obj) seq) =
                let o = obj ()

                for key, value in values do
                    o?(key) <- value

                o

            member _.encodeArray values = JS.Constructors.Array.from values
            member _.encodeList values = JS.Constructors.Array.from values
            member _.encodeSeq values = JS.Constructors.Array.from values

            member _.encodeIntegralNumber value = box value
        }

    let toString (space: int) (value: Json) : string =
        let json = Encode.toJsonValue helpers value
        JS.JSON.stringify (json, space = space)
