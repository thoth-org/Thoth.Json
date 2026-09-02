---
title: Encoder
---

An encoder describes the JSON to produce from an F# value.

`Encoder<'T>` is a function from `'T` to `IEncodable`. `Encode.toString` turns an `IEncodable` into
a JSON string, with the given number of spaces of indentation.

**Example of an encoder:**

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

type User =
    {
        Name: string
        Age: int
        KnownLangs: string list
    }

module User =

    let encoder (user: User) =
        Encode.object
            [
                "name", Encode.string user.Name
                "age", Encode.int user.Age
                "known-langs",
                user.KnownLangs |> List.map Encode.string |> Encode.list
            ]

{
    Name = "maxime"
    Age = 25
    KnownLangs = [ "fsharp"; "javascript" ]
}
|> User.encoder
|> Encode.toString 4
|> printfn "%s"
```

Pass `0` as the indentation to get the JSON on a single line.
