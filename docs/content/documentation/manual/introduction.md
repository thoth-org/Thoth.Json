---
title: Introduction
---

The manual API gives you full control over the decoding and encoding process.

Use it when your JSON doesn't match your F# types one-to-one, or when you prefer not to rely on
reflection.

For example, if your JSON holds the information inside a `data` property, you can reach into it
directly instead of creating a record for that property alone.

```json
{
    "data": {
        "id": "9d9d9d9d-9d9d-9d9d-9d9d-9d9d9d9d9d9d",
        "name": "Triss Merigold",
        "age": 42
    }
}
```

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

let json =
    """
{
    "data": {
        "id": "9d9d9d9d-9d9d-9d9d-9d9d-9d9d9d9d9d9d",
        "name": "Triss Merigold",
        "age": 42
    }
}
    """

type User =
    {
        Id: System.Guid
        Name: string
        Age: int
    }

module User =

    // Decoder specific to the user type
    let decoder: Decoder<User> =
        Decode.object (fun get ->
            {
                Id = get.Required.Field "id" Decode.guid
                Name = get.Required.Field "name" Decode.string
                Age = get.Required.Field "age" Decode.int
            }
        )

json
// Access the `data` property directly
// which gives us direct access to the 'User' object
|> Decode.fromString (Decode.field "data" User.decoder)
|> Docs.print
```
