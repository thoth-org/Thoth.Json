module Prototype.JavaScript

open Prototype.Shared
open Thoth.Json.JavaScript

let json =
    """
{
    "firstName": "John",
    "lastName": "Doe"
}
    """

let userResult = Decode.fromString User.decoder json

match userResult with
| Ok user ->
    printfn "Fist name: %s" user.FirstName
    printfn "Last name: %s" user.LastName
| Error err -> printfn "Error: %s" err

let encodedUser =
    {
        FirstName = "John"
        LastName = "Doe"
    }
    |> User.encoder
    |> Encode.toString 4

open Fable.Core.JS

console.log encodedUser

let l =
    [
        "a"
        "b"
        "c"
    ]

open Thoth.Json.Core

let json2 = l |> List.map Encode.string |> Encode.list |> Encode.toString 4
