module Prototype.Shared

open Fable.Core
open Fable.Core.JsInterop
open Thoth.Json.Core

type User =
    {
        FirstName: string
        LastName: string
    }

module User =

    let decoder: Decoder<User> =
        Decode.map2
            (fun firstName lastName ->
                {
                    FirstName = firstName
                    LastName = lastName
                }
            )
            (Decode.field "firstName" Decode.string)
            (Decode.field "lastName" Decode.string)

    let encoder user =
        Encode.object
            [
                "firstName", Encode.string user.FirstName
                "lastName", Encode.string user.LastName
            ]
