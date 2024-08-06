namespace Thoth.Json.Python

open Fable.Core
open Fable.Core.PyInterop
open Thoth.Json.Core
open Fable.Python

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
                let o = emitPyExpr () "{}"

                for key, value in values do
                    o?(key) <- value

                o

            member _.encodeArray values = values

            member this.encodeList values =
                values |> List.toArray |> this.encodeArray

            member this.encodeSeq values =
                values |> Seq.toArray |> this.encodeArray

            member this.encodeResizeArray values =
                values.ToArray() |> this.encodeArray

            member _.encodeSignedIntegralNumber value = box value
            member _.encodeUnsignedIntegralNumber value = box value
        }

    let toString (space: int) (value: IEncodable) : string =
        let json = Encode.toJsonValue helpers value
        // If we pass an indention of 0 to Python's json.dumps, it will
        // insert newlines, between each element instead of compressing
        // let space =
        //     if space = 0 then
        //         None
        //     else
        //         Some space

        // Python.Json.json.dumps(json, ?indent = space)
        if space = 0 then
            Python.Json.json.dumps (
                json,
                separators =
                    [|
                        ","
                        ":"
                    |],
                ensure_ascii = false
            )
        else
            Python.Json.json.dumps (json, indent = space, ensure_ascii = false)
