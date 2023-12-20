(***
layout: nacara/layouts/docs.njk
title: Convention
***)

(*** hide ***)

#I "../../../src/bin/Debug/netstandard2.0"
#r "Thoth.Json.dll"

open Thoth.Json

(**

When writting coders, the convention is to placed them under a module of the
same name as the type they correspond to.

*)

type User =
    {
        Name: string
        Age: int
    }

module User =

    let decoder: Decoder<User> =
        Decode.object (fun get ->
            {
                Name = get.Required.Field "name" Decode.string
                Age = get.Required.Field "age" Decode.int
            }
        )

    let encode (user: User) : JsonValue =
        Encode.object
            [
                "name", Encode.string user.Name
                "age", Encode.int user.Age
            ]

(**

The reason for this convention is that it works for all the F# types even enums
who can't have `static` methods.

*)

type Rating =
    | One = 1
    | Two = 2
    | Three = 3

module Rating =

    let decoder: Decoder<Rating> =
        Decode.int
        |> Decode.andThen (
            function
            | 1 -> Decode.succeed Rating.One
            | 2 -> Decode.succeed Rating.Two
            | 3 -> Decode.succeed Rating.Three
            | invalid ->
                Decode.fail
                    $"%i{invalid} is not a valid rating value. Expecting an integer between 1 and 3"
        )

    let encoder (rating: Rating) : JsonValue = Encode.int (int rating)
