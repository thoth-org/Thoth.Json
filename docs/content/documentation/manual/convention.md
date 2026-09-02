---
title: Convention
---

When writing coders, the convention is to place them under a module of the same name as the type
they correspond to.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

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

    let encoder (user: User) =
        Encode.object
            [
                "name", Encode.string user.Name
                "age", Encode.int user.Age
            ]

{
    Name = "Geralt de Riv"
    Age = 92
}
|> User.encoder
|> Encode.toString 4
|> printfn "%s"
```

The reason for this convention is that it works for all the F# types, including enums which can't
have `static` methods.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

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

    let encoder (rating: Rating) = Encode.int (int rating)

Decode.fromString Rating.decoder "2" |> Docs.print

Decode.fromString Rating.decoder "7" |> Docs.print
```

Name a [codec](../codec/introduction.md) `codec` in the same module. A module can hold a codec and
the two coders at once, when part of your code needs one half on its own.
