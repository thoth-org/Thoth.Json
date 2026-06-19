---
layout: nacara/layouts/docs.njk
title: Codecs
---

Conceptually, a codec is an encoder decoder pair that operate together on the same type.

Why use codecs?

 * Easier to keep encoding and decoding in sync
 * Less code in many cases
 * Clearer semantics when both encoding and decoding are required

A well-formed codec will allow an arbitary number of encoding-decoding round-trips.

You can create a codec from existing encoders and decoders like so:

```fsharp
let codec = Codec.create Encode.string Decode.string
```

However, it is recommended to use the built-in primitives and [computation expressions](https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/computation-expressions).

```fsharp
Codec.int
Codec.bool
Codec.string

// etc...
```

You can encode values like this:

```fsharp
let json =
    123
    |> Encode.codec Codec.int
    |> Encode.toString 2
```

And decode JSON like this:

```fsharp
let decoded =
    "true"
    |> Decode.fromString Codec.bool
```

### Objects

Object codecs, typically used for records, can be constructed using the `objectCodec` computation expression:

```fsharp
type FooBar =
  {
    Foo : int
    Bar : string
  }

module FooBar =

    let codec : Codec<FooBar> =
      objectCodec {
          let! foo = Codec.field "foo" (fun x -> x.Foo) Codec.int
          and! bar = Codec.field "bar" (fun x -> x.Bar) Codec.string

          return
              {
                  Foo = foo
                  Bar = bar
              }
      }
```

The JSON looks like this:

```json
{
  "foo": 123,
  "bar": "abc"
}
```

*Note the use of `and!`*

### Variants

Variants, such as discriminated unions, should be constructed using the `variantCodec` computation expression:

```fsharp
type Shape =
    | Square of width : int
    | Rectangle of width : int * height : int

module Shape =

    let codec : Codec<Shape> =
      variantCodec {
          let! square = Codec.case "square" Square Codec.int
          and! rectangle = Codec.case "rectangle" Rectangle (Codec.tuple2 Codec.int Codec.int)

          return
              function
              | Square w -> square w
              | Rectangle (w, h) -> rectangle (w, h)
      }
```

*Again, note the use of `and!`*

With the above codec, the case value will be encoded to a property with the name of the tag.

In other words, the JSON will look like:

```json
{
  "square": 16
}
```

```json
{
  "rectangle": [
    3,
    4
  ]
}
```

If you prefer an object with `tag` and `value` properties, you can do the following:

```fsharp
module Shape =

    let codec : Codec<Shape> =
        variantCodecWithTag "tag" "value" {
          let! square = Codec.case "square" Square Codec.int
          and! rectangle = Codec.case "rectangle" Rectangle (Codec.tuple2 Codec.int Codec.int)

          return
              function
              | Square w -> square w
              | Rectangle (w, h) -> rectangle (w, h)
      }
```

This gives JSON like so:

```json
{
  "tag": "square",
  "value": 16
}
```

```json
{
  "tag": "rectangle",
  "value": [
    3,
    4
  ]
}
```
