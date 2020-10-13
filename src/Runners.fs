namespace Thoth.Json

open Fable.Core

[<AutoOpen>]
module Extensions =

    [<RequireQualifiedAccess>]
    module Decode =

        let fromValue (decoder : Decoder<'T>) =
            fun value ->
                match decoder value with
                | Ok success ->
                    Ok success
                | Error error ->
                    let finalError = error |> DecoderError.prependPath "$"
                    Error (DecoderError.errorToStringWithPath finalError)

        let fromString (decoder : Decoder<'T>) =
            fun value ->
                try
                    let json = Parser.parse value
                    match decoder json with
                    | Ok success -> Ok success
                    | Error error ->
                        let finalError = error |> DecoderError.prependPath "$"
                        Error (DecoderError.errorToStringWithPath finalError)
                with
                    | Parser.JsonParserError msg ->
                        Error("Given an invalid JSON: " + msg)

        let unsafeFromString (decoder : Decoder<'T>) =
            fun value ->
                match fromString decoder value with
                | Ok x -> x
                | Error msg -> failwith msg
