---
title: Composition
---

Thoth.Json provides a decoder for most primitive types, like `int`, `bool` or `System.Guid`.

To build decoders for your own types, it offers several ways to compose them.

## Object builder style

When working with objects, use the object builder helper.

It puts the field name and its decoder at the same place. With
[map functions](#map-functions) it's easy to get the argument order wrong.

It also supports a record with any number of properties.

You first choose whether the property is required or optional, then describe how to decode it.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

type Point =
    {
        X: int
        Y: int
    }

let decoder =
    Decode.object (fun get ->
        {
            X = get.Required.Raw(Decode.field "x" Decode.int)
            Y = get.Required.Raw(Decode.field "y" Decode.int)
        }
    )

Decode.fromString decoder """{ "x": 1, "y": 2 }""" |> Docs.print
```

The object builder also provides a friendlier syntax for the most common cases. The decoder above
can be written as:

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

type Point =
    {
        X: int
        Y: int
    }

let decoder =
    Decode.object (fun get ->
        {
            X = get.Required.Field "x" Decode.int
            Y = get.Required.Field "y" Decode.int
        }
    )

Decode.fromString decoder """{ "x": 1, "y": 2 }""" |> Docs.print
```

`get.Required` and `get.Optional` each offer three members.

### Field

Decodes the value under the given property name.

### At

Decodes the value under a path of property names.

```fs
get.Required.At [ "user"; "name" ] Decode.string
```

### Raw

Runs the decoder against the object itself. Use it to reach a value the two others can't describe.

## Combine decoders

If your data is composed of several objects, you can construct the decoders top down. First you
create the decoders of the different records, then you combine them together.

If we have the following JSON:

```json
{
    "data": {
        "user": {
            "name": "Triss Merigold",
            "age": 42
        },
        "post": {
            "title": "Handle JSON with Fable",
            "abstract": "How to simply read data with Thoth.Json"
        }
    }
}
```

We create types and decoders for `User` and `Post`, then combine them to form the parent record.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

let json =
    """
{
    "data": {
        "user": {
            "name": "Triss Merigold",
            "age": 42
        },
        "post": {
            "title": "Handle JSON with Fable",
            "abstract": "How to simply read data with Thoth.Json"
        }
    }
}
    """

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

type Post =
    {
        Title: string
        Abstract: string
    }

module Post =

    let decoder: Decoder<Post> =
        Decode.object (fun get ->
            {
                Title = get.Required.Field "title" Decode.string
                Abstract = get.Required.Field "abstract" Decode.string
            }
        )

type Data =
    {
        User: User
        Post: Post
    }

module Data =

    // Get both structures and decode them with their own decoder accordingly
    let decoder: Decoder<Data> =
        Decode.object (fun get ->
            {
                User = get.Required.Field "user" User.decoder
                Post = get.Required.Field "post" Post.decoder
            }
        )

json
|> Decode.fromString (Decode.field "data" Data.decoder)
|> Docs.print
```

## Computation expression

The `decoder` computation expression composes decoders without naming a combinator.

`let!` runs the decoders one after the other. `and!` runs them independently, which is what you want
when a decoder doesn't depend on the result of the previous one.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

type Point =
    {
        X: int
        Y: int
    }

let decoder =
    decoder {
        let! x = Decode.field "x" Decode.int
        and! y = Decode.field "y" Decode.int

        return
            {
                X = x
                Y = y
            }
    }

Decode.fromString decoder """{ "x": 1, "y": 2 }""" |> Docs.print
```

## Map functions

The `map2`, `map3`, ..., `map8` functions take a function to build a concrete type from the results
of the provided decoders.

Thoth.Json only provides `map` functions up to 8 arguments. If you need more, consider using the
[object builder](#object-builder-style) or the [computation expression](#computation-expression).

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

type Point =
    {
        X: int
        Y: int
    }

let decoder =
    Decode.map2
        (fun x y ->
            {
                X = x
                Y = y
            }
        )
        (Decode.field "x" Decode.int)
        (Decode.field "y" Decode.int)

Decode.fromString decoder """{ "x": 1, "y": 2 }""" |> Docs.print
```

## Chain decoders

Use `andThen` to use the result of one decoder as the input of another one.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

type PersonType =
    | Student
    | Teacher

module PersonType =

    let decoder: Decoder<PersonType> =
        Decode.string
        |> Decode.andThen (fun textValue ->
            match textValue with
            | "student" -> Decode.succeed Student

            | "teacher" -> Decode.succeed Teacher

            | invalid ->
                Decode.fail
                    $"Expecting \"student\" or \"teacher\" but instead got: \"%s{invalid}\""
        )

Decode.fromString PersonType.decoder "\"student\"" |> Docs.print

Decode.fromString PersonType.decoder "\"pilot\"" |> Docs.print
```

The decoder succeeds only if the JSON value is a string, and that string is `"student"` or
`"teacher"`.

## Map result to another type

When using DDD (aka Domain Driven Design) you often need to map your types.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

type Email = Email of string

module Email =

    let decoder: Decoder<Email> = Decode.string |> Decode.map Email

Decode.fromString Email.decoder "\"maxime@mail.com\"" |> Docs.print
```

If the provided JSON is a string, the decoder succeeds and returns an `Email`.

:::info
For simplicity, we used `Decode.string` but in a real world scenario, you would probably want to
validate the email format.
:::

## Inconsistent JSON

Sometimes the JSON you received is not consistent, or has several ways to represent a type.

In these cases, use `Decode.oneOf` to try different decoders.

Imagine you are parsing a list of numbers but some of them are represented as `null`.

```json
[ 1, null, 2, 3 ]
```

You can write a decoder like that:

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

let nullableIntDecoder: Decoder<int> =
    Decode.oneOf
        [
            // First try to decode it as a standard int
            Decode.int
            // If it fails, try to decode it as a null
            Decode.nil 0
        ]

"[ 1, null, 2, 3 ]"
|> Decode.fromString (Decode.list nullableIntDecoder)
|> Docs.print
```

:::info
`Decode.oneOf` tries the decoders in order, and stops after finding a successful one.
:::

## Recursive types

A decoder for a type that refers to itself is built with `Decode.fix`. It hands you the decoder
being defined, so you can use it inside its own definition.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

type Comment =
    {
        Text: string
        Replies: Comment list
    }

let decoder: Decoder<Comment> =
    Decode.fix (fun self ->
        Decode.object (fun get ->
            {
                Text = get.Required.Field "text" Decode.string
                Replies = get.Required.Field "replies" (Decode.list self)
            }
        )
    )

"""
{
    "text": "Hello",
    "replies": [
        { "text": "Hi", "replies": [] }
    ]
}
"""
|> Decode.fromString decoder
|> Docs.print
```
