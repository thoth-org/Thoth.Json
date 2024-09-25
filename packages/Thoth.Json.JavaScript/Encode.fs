namespace Thoth.Json.JavaScript

#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT || !FABLE_COMPILER

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

    let toString (space: int) (value: IEncodable) : string =
        let json = Encode.toJsonValue helpers value
        JS.JSON.stringify (json, space = space)

#endif
