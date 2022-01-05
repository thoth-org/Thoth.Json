(**
---
layout: standard
title: JSON representation
---
*)

(*** hide ***)

#I "../../../src/bin/Debug/netstandard2.0"
#r "Thoth.Json.dll"
#r "nuget: Fable.Core"

open Thoth.Json
open Fable.Core

(**

When using the manual API, you have full control over your JSON representation.

This means that if you don't like the default representation offered by Thoth.Json,
you can implement your own version of the decoders.

It also means that you are responsible for choosing how you want to represent
types which don't have a direct representation in JSON like discriminated unions.

This section aims to show you an example of how to represents most of the
F# types but remember that you can choose your own representation.

## Numbers

Thoth.Json follows the [IEEE 754](https://en.wikipedia.org/wiki/IEEE_754) standard for representing numbers.

Others libraries like [Google API](https://developers.google.com/discovery/v1/type-format) follow the same standard.

Meaning that big numbers are represented as strings.

Represented using **numbers**:
- `sbyte`
- `byte`
- `int16`
- `uint16`
- `int`
- `int32`
- `float`
- `float32`

Represented using **string**:
- `decimal`
- `bigint`
- `int64`
- `uint64`

:::info
The decoders accept both string and numeric JSON values.
:::

:::warning{title="Important"}
Before Thoth.Json 7.0.0 and Thoth.Json.Net 8.0.0, the following were using string and not numbers:

This was an error.

- `sbyte`
- `byte`
- `int16`
- `uint16`
:::

## Record

Records can be mapped to JSON objects.

Example:

```json
{ "name": "Kelsier", "age": 30 }
```

Code:

*)

type Person =
    {
        Name : string
        Age : int option
    }

module Person =

    let encoder (person : Person) =
        Encode.object [
            "name", Encode.string person.Name
            "age", Encode.option Encode.int person.Age
        ]

    let decoder : Decoder<Person> =
        Decode.object (fun get ->
            {
                Name = get.Required.Field "name" Decode.string
                Age = get.Optional.Field "age" Decode.int
            }
        )

(**

## Dicriminated union

*)

type Color =
    | Red
    | Rgb of red : int * green : int * blue : int

(**

### Using array

We are storing the case name in the first element of the array and then have
one element per fields.

Example:

```json
[ "Red" ]

[ "Rgb", 255, 0, 0 ]
```

Code:

*)

module Color =

    let encoder (color : Color) =
        match color with
        | Red ->
            [
                Encode.string "Red"
            ]
            |> Encode.list

        | Rgb (red, green, blue) ->
            [
                Encode.string "Rgb"
                Encode.int red
                Encode.int green
                Encode.int blue
            ]
            |> Encode.list

    let decoder : Decoder<Color> =
        Decode.index 0 Decode.string
        |> Decode.andThen (fun caseName ->
            match caseName with
            | "Red" ->
                Decode.succeed Red

            | "Rgb" ->
                Decode.map3 (fun red green blue ->
                    Rgb (red, green, blue)
                )
                    (Decode.index 1 Decode.int)
                    (Decode.index 2 Decode.int)
                    (Decode.index 3 Decode.int)

            | invalid ->
                Decode.fail $"""`%s{invalid}` is not a valid case for Color. Expecting one of the following:
- Red
- Rgb"""
        )

(**

### Using object with metadata

We are using an object with a special property `$case` to store the case name
and one property for each of the fields.

Example:

```json
{ "$case" : "Red" }

{ "$case" : "Rgb", "red" : 255, "green" : 0, "blue" : 0 }
```

Code:
*)

module Color2 =

    let encoder (color : Color) =
        match color with
        | Red ->
            Encode.object [
                "$case", Encode.string "Red"
            ]

        | Rgb (red, green, blue) ->
            Encode.object [
                "$case", Encode.string "Rgb"
                "red", Encode.int red
                "green", Encode.int green
                "blue", Encode.int blue
            ]

    let decoder : Decoder<Color> =
        Decode.field "$case" Decode.string
        |> Decode.andThen (fun caseName ->
            match caseName with
            | "Red" ->
                Decode.succeed Red

            | "Rgb" ->
                Decode.object (fun get ->
                    Rgb (
                        red = get.Required.Field "red" Decode.int,
                        green = get.Required.Field "green" Decode.int,
                        blue = get.Required.Field "blue" Decode.int
                    )
                )

            | invalid ->
                Decode.fail $"""`%s{invalid}` is not a valid case for Color. Expecting one of the following:
- Red
- Rgb"""
        )
