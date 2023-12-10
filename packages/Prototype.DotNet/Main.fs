module Prototype.DotnNet

open Prototype.Shared
open Thoth.Json.Newtonsoft

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
