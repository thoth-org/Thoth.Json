---
title: Thoth.Json
---

[[toc]]

This documentation is for `Thoth.Json` v3, documentation for older versions can be found here:

- [Thoth.Json v1](/Thoth.Json/legacy/v1.html)
- [Thoth.Json v2](/Thoth.Json/legacy/v2.html)
- [Thoth.Json v3](/Thoth.Json/legacy/v3.html)

## Decoder

Turn JSON values into F# values.

By using a Decoder, you will be guaranteed that the JSON structure is correct.
This is especially useful if you use Fable without sharing your domain with the server.

*This module is inspired by [Json.Decode from Elm](http://package.elm-lang.org/packages/elm-lang/core/latest/Json-Decode)
and [elm-decode-pipeline](http://package.elm-lang.org/packages/NoRedInk/elm-decode-pipeline/latest).*

You can also take a look at the [Elm documentation](https://guide.elm-lang.org/interop/json.html).

### What is a Decoder?

Here is the signature of a `Decoder`:

```fsharp
type Decoder<'T> = string -> obj -> Result<'T, DecoderError>
```

This is taking two arguments:

- the traveled path
- an "untyped" value and checking if it has the expected structure.

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

### Primitives decoders

- `string : Decoder<string>`
- `guid : Decoder<System.Guid>`
- `int : Decoder<int>`
- `int64 : Decoder<int64>`
- `uint64 : Decoder<uint64>`
- `bigint : Decoder<bigint>`
- `bool : Decoder<bool>`
- `float : Decoder<float>`
- `decimal : Decoder<decimal>`
- `datetime : Decoder<System.DateTime>`
- `datetimeOffset : Decoder<System.DateTimeOffset>`
- `timespan : Decoder<System.TimeSpan>`

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

### Collections

There are special decoders for the following collections.

- `list : Decoder<'value> -> Decoder<'value list>`
- `array : Decoder<'value> -> Decoder<'value array>`
- `index : -> int -> Decoder<'value> -> Decoder<'value>`

```fsharp
open Thoth.Json

> Decode.fromString (array int) "[1, 2, 3]"
val it : Result<int [], string> =  Ok [|1, 2, 3|]

> Decode.fromString (list string) """["Maxime", "Alfonso", "Vesper"]"""
val it : Result<string list, string> = Ok ["Maxime", "Alfonso", "Vesper"]

> Decode.fromString (Decode.index 1 Decode.string) """["maxime", "alfonso", "steffen"]"""
val it : Result<string, string> = Ok("alfonso")
```

### Decoding Objects

In order to decode objects, you can use:

- `field : string -> Decoder<'value> -> Decoder<'value>`
    - Decode a JSON object, requiring a particular field.
- `at : string list -> Decoder<'value> -> Decoder<'value>`
    - Decode a JSON object, requiring certain path.

```fsharp
open Thoth.Json

> Decode.fromString (field "x" int) """{"x": 10, "y": 21}"""
val it : Result<int, string> = Ok 10

> Decode.fromString (field "y" int) """{"x": 10, "y": 21}"""
val it : Result<int, string> = Ok 21
```

**Important:**

These two decoders only take into account the provided field or path. The object can have other fields/paths with other content.

#### Map functions

To get data from several fields and convert them into a record you will need to use the `map` functions:
`map2`, `map3`, ..., `map8`.

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

#### Object builder style

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

## Encoder

Module for turning F# values into JSON values.

*This module is inspired by [Json.Encode from Elm](http://package.elm-lang.org/packages/elm-lang/core/latest/Json-Encode).*

### How to use it?

```fsharp
    open Thoth.Json

    let person =
        Encode.object
            [ "firstname", Encode.string "maxime"
              "surname", Encode.string "mangel"
              "age", Encode.int 25
              "address", Encode.object
                            [ "street", Encode.string "main street"
                              "city", Encode.string "Bordeaux" ]
            ]

    let compact = Encode.toString 0 person
    // {"firstname":"maxime","surname":"mangel","age":25,"address":{"street":"main street","city":"Bordeaux"}}

    let readable = Encode.toString 4 person
    // {
    //     "firstname": "maxime",
    //     "surname": "mangel",
    //     "age": 25,
    //     "address": {
    //         "street": "main street",
    //         "city": "Bordeaux"
    //     }
    // }
```

## Auto coders

<article class="message is-info">
  <div class="message-body">

When using **auto coders**, we are referring to both **auto encoders** and **auto decoders**.
  </div>
</article>


If your JSON structure is a one to one match with your F# type, then you can use auto coders.

### Auto decoder

Auto decoders will generate the decoder at runtime for you and still guarantee that the JSON structure is correct.

```fsharp
> let json = """{ "Id" : 0, "Name": "maxime", "Email": "mail@domain.com", "Followers": 0 }"""
> Decode.Auto.fromString<User>(json)
val it : Result<User, string> = Ok { Id = 0; Name = "maxime"; Email = "mail@domain.com"; Followers = 0 }
```

`Decode.Auto` helpers accept an optional argument `caseStrategy` that applies to keys:

- `CamelCase`, then the keys in the JSON are considered `camelCase`
- `PascalCase`, then the keys in the JSON are considered `PascalCase`
- `SnakeCase`, then the keys in the JSON are considered `snake_cases`

```fsharp
> let json = """{ "id" : 0, "name": "maxime", "email": "mail@domain.com", "followers": 0 }"""
> Decode.Auto.fromString<User>(json, caseStrategy=CamelCase)
val it : Result<User, string> = Ok { Id = 0; Name = "maxime"; Email = "mail@domain.com"; Followers = 0 }
```

### Auto encoder

Auto decoders will generate the encoder at runtime for you.

```fsharp
type User =
    { Id : int
      Name : string
      Email : string
      Followers : int }

let user =
    { Id = 0
      Name = "maxime"
      Email = "mail@domain.com"
      Followers = 0 }

let json = Encode.Auto.toString(4, user)
// {
//     "Id": 0,
//     "Name": "maxime",
//     "Email": "mail@domain.com",
//     "Followers": 0
// }
```

### Extra coders

When generating an auto coder, sometimes you will want to use a manual coder for a type nested in your domain hierarchy.

In those cases you can pass **extra coders** that will replace the default (or missing) coders. Use the `Extra` module to build the map for extra coders (see example below).

```fsharp
let myExtraCoders =
    Extra.empty
    |> Extra.withCustom MyType.Encode MyType.Decode
```

### Decoding int64, decimal or bigint

Coders for `int64`, `decimal` or `bigint` won't be automatically generated by default. If they were, the bundle size would increase significantly even if you don't intend to use these types.

If required, you can easily include them in your extra coders with the helpers in the `Extra` module:

```fsharp
let myExtraCoders =
    Extra.empty
    |> Extra.withInt64
    |> Extra.withDecimal
    |> Extra.withCustom MyType.Encode MyType.Decode
```

### Caching

To avoid having to regenerate your auto coders every time you need them, you can use the helpers with the `Cached` suffix instead (please note in these cases you shouldn't change the value of extra parameters like `caseStrategy` or `extra`).

The easiest way to do it is to include some helpers in your app to easily generate (or retrieve from cache) coders whenever you need them. For example:

```fsharp
// Note the helpers must be inlined to resolve generic parameters in Fable
let inline encoder<'T> = Encode.Auto.generateEncoderCached<'T>(caseStrategy = CamelCase, extra = myExtraCoders)
let inline decoder<'T> = Decode.Auto.generateDecoderCached<'T>(caseStrategy = CamelCase, extra = myExtraCoders)
```

Now you can easily invoke the helpers whenever you need a coder. Most of the times you can omit the generic argument as it will be inferred.

```fsharp
let demo(x : RequestType) : ResponseType =
    let requestJson = encoder x |> Encode.toString 4
    let responseJson = (* Send JSON to server and receive response *)
    Decode.unsafeFromString decoder responseJson
```

## .Net & NetCore support

You can share your decoders and encoders **between your client and server**.

In order to use Thoth.Json API on .Net or NetCore you need to use the `Thoth.Json.Net` package.

### Code sample

```fsharp
// By adding this condition, you can share your code between your client and server
#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

type User =
    { Id : int
      Name : string
      Email : string
      Followers : int }

    static member Decoder : Decode.Decoder<User> =
        Decode.object
            (fun get ->
                { Id = get.Required.Field "id" Decode.int
                    Name = get.Optional.Field "name" Decode.string
                            |> Option.defaultValue ""
                    Email = get.Required.Field "email" Decode.string
                    Followers = 0 }
            )

    static member Encoder (user : User) =
        Encode.object
            [ "id", Encode.int user.Id
              "name", Encode.string user.Name
              "email", Encode.string user.Email
              "followers", Encode.int user.Followers
            ]
```

### Giraffe

If you're using the [Giraffe](https://github.com/giraffe-fsharp/Giraffe) or [Saturn](https://saturnframework.org/) web servers, you can use the `Thoth.Json.Giraffe` package to enable automatic JSON serialization with Thoth in your responses. Check [how to add a custom serializer](https://github.com/giraffe-fsharp/Giraffe/blob/master/DOCUMENTATION.md#using-a-different-json-serializer) to Giraffe.

> The `ThothSerializer` type also includes some static helpers to deal with JSON directly for request and response streams.
