---
description: Explanations of the concepts of Decoders and how to write one
---

# Decoder

Turn JSON values into F\# values.

By using a Decoder, you will be guaranteed that the JSON structure is correct. This is especially useful if you use Fable without sharing your domain with the server.

_This module is inspired by_ [_Json.Decode from Elm_](http://package.elm-lang.org/packages/elm-lang/core/latest/Json-Decode) _and_ [_elm-decode-pipeline_](http://package.elm-lang.org/packages/NoRedInk/elm-decode-pipeline/latest)_._

You can also take a look at the [Elm documentation](https://guide.elm-lang.org/interop/json.html).

## What is a Decoder?

Here is the signature of a `Decoder`:

```fsharp
type Decoder<'T> = string -> obj -> Result<'T, DecoderError>
```

This is taking two arguments:

* the traveled path
* an "untyped" value and checking if it has the expected structure.

If the structure is correct, then you get an `Ok` result, otherwise an `Error` explaining where and why the decoder failed.

Example of a decoder error:

```fsharp
Error at: `$.user.firstname`
Expecting an object with path `user.firstname` but instead got:
{
    "user": {
        "name": "maxime",
        "age": 25
    }
}
Node `firstname` is unknown.
```

The path generated is a valid `JSONPath`, so you can use tools like [JSONPath Online Evaluator](http://jsonpath.com/) to explore your JSON.

## Primitives decoders

* `string : Decoder<string>`
* `guid : Decoder<System.Guid>`
* `int : Decoder<int>`
* `int64 : Decoder<int64>`
* `uint64 : Decoder<uint64>`
* `bigint : Decoder<bigint>`
* `bool : Decoder<bool>`
* `float : Decoder<float>`
* `decimal : Decoder<decimal>`
* `datetime : Decoder<System.DateTime>`
* `datetimeOffset : Decoder<System.DateTimeOffset>`
* `timespan : Decoder<System.TimeSpan>`

```fsharp
open Thoth.Json

> Decode.fromString Decode.string "\"maxime\""
val it : Result<string, string> = Ok "maxime"

> Decode.fromString Decode.int "25"
val it : Result<int, string> = Ok 25

> Decode.fromString Decode.bool "true"
val it : Result<bool, string> = Ok true

> Decode.fromString Decode.float "true"
val it : Result<float, string> = Error "Error at: `$$`\n Expecting a float but instead got: true"
```

With these primitives decoders we can handle basic JSON values.

## Collections

There are special decoders for the following collections.

* `list : Decoder<'value> -> Decoder<'value list>`
* `array : Decoder<'value> -> Decoder<'value array>`
* `index : -> int -> Decoder<'value> -> Decoder<'value>`

```fsharp
open Thoth.Json

> Decode.fromString (Decode.array Decode.int) "[1, 2, 3]"
val it : Result<int [], string> =  Ok [|1, 2, 3|]

> Decode.fromString (Decode.list Decode.string) """["Maxime", "Alfonso", "Vesper"]"""
val it : Result<string list, string> = Ok ["Maxime", "Alfonso", "Vesper"]

> Decode.fromString (Decode.index 1 Decode.string) """["maxime", "alfonso", "steffen"]"""
val it : Result<string, string> = Ok("alfonso")
```

## Decoding Objects

In order to decode objects, you can use:

* `field : string -> Decoder<'value> -> Decoder<'value>`
  * Decode a JSON object, requiring a particular field.
* `at : string list -> Decoder<'value> -> Decoder<'value>`
  * Decode a JSON object, requiring certain path.

```fsharp
open Thoth.Json

> Decode.fromString (Decode.field "x" Decode.int) """{"x": 10, "y": 21}"""
val it : Result<int, string> = Ok 10

> Decode.fromString (Decode.field "y" Decode.int) """{"x": 10, "y": 21}"""
val it : Result<int, string> = Ok 21
```

**Important:**

These two decoders only take into account the provided field or path. The object can have other fields/paths with other content.

### **Map functions**

To get data from several fields and convert them into a record you will need to use the `map` functions: `map2`, `map3`, ..., `map8`.

```fsharp
open Thoth.Json

type Point =
    { X : int
      Y : int }

    static member Decoder : Decoder<Point> =
        Decode.map2 (fun x y ->
                { X = x
                  Y = y } : Point)
             (Decode.field "x" Decode.int)
             (Decode.field "y" Decode.int)

> Decode.fromString Point.Decoder """{"x": 10, "y": 21}"""
val it : Result<Point, string> = Ok { X = 10; Y = 21 }
```

### **Object builder style**

When working with larger objects, you can use the object builder helper.

```fsharp
open Thoth.Json

type User =
    { Id : int
      Name : string
      Email : string
      Followers : int }

    static member Decoder : Decoder<User> =
        Decode.object
            (fun get ->
                { Id = get.Required.Field "id" Decode.int
                    Name = get.Optional.Field "name" Decode.string
                            |> Option.defaultValue ""
                    Email = get.Required.Field "email" Decode.string
                    Followers = 0 }
            )

> Decode.fromString User.Decoder """{ "id": 67, "email": "user@mail.com" }"""
val it : Result<User, string> = Ok { Id = 67; Name = ""; Email = "user@mail.com"; Followers = 0 }
```

It is encouraged to use **object builder style** when working with Objects because you declare the decoders at the same place you use them. When using **map functions** it is easy to mess up the order of arguments;

