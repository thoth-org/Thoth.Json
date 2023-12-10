(**
---
layout: standard
title: Introduction
---
*)

(*** hide ***)

#I "../../../src/bin/Debug/netstandard2.0"
#r "Thoth.Json.dll"

open Thoth.Json

(**

The manual API allows you to have full control over the decoding process.

This is useful when your JSON doesn't match your F# types one-to-one or
if you prefer to not rely on reflection.

For example, if your JSON contains the information inside of `data` property
you can directly access it without having to create a record just for this property:

```json
{
    "data": {
        "id": "9d9d9d9d-9d9d-9d9d-9d9d-9d9d9d9d9d9",
        "name": "Triss Merigold",
        "age": 42
    }
}
```

*)

(*** hide ***)

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

(**

Then you can write a decoder like this:

*)

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

Decode.fromString
    // Access the `data` property directly
    // allow us direct access to the 'User' object
    (Decode.field "data" User.decoder)
    json
