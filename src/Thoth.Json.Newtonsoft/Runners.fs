namespace Thoth.Json.Newtonsoft

open Newtonsoft.Json
open System.IO

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
                    use reader = new JsonTextReader(new StringReader(value), DateParseHandling = DateParseHandling.None)

                    let json = Newtonsoft.Json.Linq.JValue.ReadFrom reader
                    match decoder json with
                    | Ok success -> Ok success
                    | Error error ->
                        let finalError = error |> DecoderError.prependPath "$"
                        Error (DecoderError.errorToStringWithPath finalError)
                with
                    | :? Newtonsoft.Json.JsonReaderException as ex ->
                        Error("Given an invalid JSON: " + ex.Message)

        let unsafeFromString (decoder : Decoder<'T>) =
            fun value ->
                match fromString decoder value with
                | Ok x -> x
                | Error msg -> failwith msg
