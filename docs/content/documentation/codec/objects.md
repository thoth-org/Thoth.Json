---
title: Objects
---

The `objectCodec` computation expression builds a codec for a record.

Each `Codec.field` names a JSON property, says how to read that property out of the record, and
gives the codec for its value. The `return` rebuilds the record from the decoded fields.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

type User =
    {
        Name: string
        Age: int
    }

module User =

    let codec: Codec<User> =
        objectCodec {
            let! name = Codec.field "name" _.Name Codec.string
            and! age = Codec.field "age" _.Age Codec.int

            return
                {
                    Name = name
                    Age = age
                }
        }

let json =
    {
        Name = "Geralt de Riv"
        Age = 92
    }
    |> Encode.codec User.codec
    |> Encode.toString 4

printfn "%s" json

Decode.fromString User.codec json |> Docs.print
```

Use `and!` between fields. They are decoded independently, so all of them are read whatever the
others do.

## Optional fields

`Codec.optional` reads a `'T option`. The property is omitted when the value is `None`, and its
absence decodes back to `None`.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

type User =
    {
        Name: string
        Nickname: string option
    }

module User =

    let codec: Codec<User> =
        objectCodec {
            let! name = Codec.field "name" _.Name Codec.string
            and! nickname = Codec.optional "nickname" _.Nickname Codec.string

            return
                {
                    Name = name
                    Nickname = nickname
                }
        }

{
    Name = "Geralt de Riv"
    Nickname = None
}
|> Encode.codec User.codec
|> Encode.toString 0
|> printfn "%s"

Decode.fromString User.codec """{ "name": "Geralt de Riv" }""" |> Docs.print
```

## Nesting

A field takes any codec, including one built by another `objectCodec`.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

type Author =
    {
        Name: string
    }

type Post =
    {
        Title: string
        Author: Author
    }

module Author =

    let codec: Codec<Author> =
        objectCodec {
            let! name = Codec.field "name" _.Name Codec.string

            return
                {
                    Name = name
                }
        }

module Post =

    let codec: Codec<Post> =
        objectCodec {
            let! title = Codec.field "title" _.Title Codec.string
            and! author = Codec.field "author" _.Author Author.codec

            return
                {
                    Title = title
                    Author = author
                }
        }

{
    Title = "Handle JSON with Fable"
    Author =
        {
            Name = "Triss Merigold"
        }
}
|> Encode.codec Post.codec
|> Encode.toString 4
|> printfn "%s"
```

## Reaching into a path

`Codec.at` moves a whole codec under a path of property names. Decoding reads the value at that
path, encoding wraps it back in nested objects.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

let codec = Codec.at [ "data"; "count" ] Codec.int

Encode.codec codec 42 |> Encode.toString 0 |> printfn "%s"

Decode.fromString codec """{ "data": { "count": 42 } }""" |> Docs.print
```
