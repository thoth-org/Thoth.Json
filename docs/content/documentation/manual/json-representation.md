---
title: JSON representation
toc:
  to: 3
---

With the manual API you have full control over your JSON representation.

If you don't like the default representation offered by Thoth.Json, you can implement your own
version of the coders.

It also means that you are responsible for choosing how you want to represent types which don't have
a direct representation in JSON, like discriminated unions.

This page shows how the built-in coders represent the most common F# types. Remember that you can
choose your own representation.

## Numbers

Thoth.Json follows the [IEEE 754](https://en.wikipedia.org/wiki/IEEE_754) standard for representing
numbers, like other libraries such as the
[Google APIs](https://developers.google.com/discovery/v1/type-format).

Numbers which don't fit in a `float` without losing precision are represented as strings.

Represented using **numbers**:

- `sbyte`
- `byte`
- `int16`
- `uint16`
- `int`
- `int32`
- `float`
- `float32`

Represented using **strings**:

- `decimal`
- `bigint`
- `int64`
- `uint64`

:::info
The decoders accept both string and numeric JSON values.
:::

## Record

Records are mapped to JSON objects.

```json
{ "name": "Kelsier", "age": 30 }
```

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

type Person =
    {
        Name: string
        Age: int option
    }

module Person =

    let encoder (person: Person) =
        Encode.object
            [
                "name", Encode.string person.Name
                "age", Encode.lossyOption Encode.int person.Age
            ]

    let decoder: Decoder<Person> =
        Decode.object (fun get ->
            {
                Name = get.Required.Field "name" Decode.string
                Age = get.Optional.Field "age" Decode.int
            }
        )

{
    Name = "Kelsier"
    Age = Some 30
}
|> Person.encoder
|> Encode.toString 0
|> printfn "%s"

Decode.fromString Person.decoder """{ "name": "Kelsier" }""" |> Docs.print
```

## Option

Two representations are available.

### Lossy

`Encode.lossyOption` writes `Some x` as `x`, and `None` as `null`. `Decode.lossyOption` reads them
back.

This is the representation most APIs use. It can't tell `Some None` from `None`, nor `Some 42` from
`Some (Some 42)`, so nested options don't round-trip.

### Lossless

`Encode.losslessOption` writes an object carrying the case, which round-trips at any nesting depth.

```json
{ "$type": "option", "$case": "some", "$value": 42 }
```

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

let value: int option option = Some None

value
|> Encode.losslessOption (Encode.losslessOption Encode.int)
|> Encode.toString 0
|> printfn "%s"

value
|> Encode.lossyOption (Encode.lossyOption Encode.int)
|> Encode.toString 0
|> printfn "%s"
```

## Discriminated union

### Using array

We store the case name in the first element of the array, then one element per field.

```json
[ "Red" ]

[ "Rgb", 255, 0, 0 ]
```

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

type Color =
    | Red
    | Rgb of red: int * green: int * blue: int

module Color =

    let encoder (color: Color) =
        match color with
        | Red -> [ Encode.string "Red" ] |> Encode.list

        | Rgb(red, green, blue) ->
            [
                Encode.string "Rgb"
                Encode.int red
                Encode.int green
                Encode.int blue
            ]
            |> Encode.list

    let decoder: Decoder<Color> =
        Decode.index 0 Decode.string
        |> Decode.andThen (fun caseName ->
            match caseName with
            | "Red" -> Decode.succeed Red

            | "Rgb" ->
                Decode.map3
                    (fun red green blue -> Rgb(red, green, blue))
                    (Decode.index 1 Decode.int)
                    (Decode.index 2 Decode.int)
                    (Decode.index 3 Decode.int)

            | invalid ->
                Decode.fail
                    $"""`%s{invalid}` is not a valid case for Color. Expecting one of the following:
- Red
- Rgb"""
        )

Rgb(255, 0, 0) |> Color.encoder |> Encode.toString 0 |> printfn "%s"

Decode.fromString Color.decoder """[ "Rgb", 255, 0, 0 ]""" |> Docs.print
```

### Using an object with metadata

We use an object with a `$case` property holding the case name, and one property per field.

```json
{ "$case" : "Red" }

{ "$case" : "Rgb", "red" : 255, "green" : 0, "blue" : 0 }
```

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

type Color =
    | Red
    | Rgb of red: int * green: int * blue: int

module Color =

    let encoder (color: Color) =
        match color with
        | Red -> Encode.object [ "$case", Encode.string "Red" ]

        | Rgb(red, green, blue) ->
            Encode.object
                [
                    "$case", Encode.string "Rgb"
                    "red", Encode.int red
                    "green", Encode.int green
                    "blue", Encode.int blue
                ]

    let decoder: Decoder<Color> =
        Decode.field "$case" Decode.string
        |> Decode.andThen (fun caseName ->
            match caseName with
            | "Red" -> Decode.succeed Red

            | "Rgb" ->
                Decode.object (fun get ->
                    Rgb(
                        red = get.Required.Field "red" Decode.int,
                        green = get.Required.Field "green" Decode.int,
                        blue = get.Required.Field "blue" Decode.int
                    )
                )

            | invalid ->
                Decode.fail
                    $"""`%s{invalid}` is not a valid case for Color. Expecting one of the following:
- Red
- Rgb"""
        )

Rgb(255, 0, 0) |> Color.encoder |> Encode.toString 0 |> printfn "%s"
```

The [Codec API](../codec/unions.md) writes both of these for you.

## Map

`Encode.dict` and `Decode.dict` represent a `Map<string, 'T>` as a JSON object.

`Encode.map` and `Decode.map'` take a coder for the key as well, and represent the map as an array
of `[ key, value ]` pairs. Use them when the key isn't a string.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

Map.ofList
    [
        "one", 1
        "two", 2
    ]
|> Map.map (fun _ value -> Encode.int value)
|> Encode.dict
|> Encode.toString 0
|> printfn "%s"

Map.ofList
    [
        1, "one"
        2, "two"
    ]
|> Encode.map Encode.int Encode.string
|> Encode.toString 0
|> printfn "%s"
```
