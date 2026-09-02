---
title: Override defaults
---

With the manual API you decide how each value is represented, including the values Thoth.Json
already has a coder for.

If you don't like how the default coder works with dates, write your own and shadow the default one.

```fsharp live
open System
open Thoth.Json.Core
open Thoth.Json.JavaScript

module Encode =

    let datetime (date: DateTime) =
        DateTimeOffset(date).ToUnixTimeSeconds() |> float |> Encode.float

module Decode =

    let datetime: Decoder<DateTime> =
        { new Decoder<DateTime> with
            member _.Decode(helpers, value) =
                if helpers.isNumber value then
                    helpers.asFloat value
                    |> int64
                    |> DateTimeOffset.FromUnixTimeSeconds
                    |> _.DateTime
                    |> Ok
                else
                    Error("", BadPrimitive("a timestamp", value))
        }

DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)
|> Encode.datetime
|> Encode.toString 0
|> printfn "%s"

Decode.fromString Decode.datetime "1577836800" |> Docs.print

Decode.fromString Decode.datetime "\"2020-01-01\"" |> Docs.print
```

Because the modules are named `Encode` and `Decode`, the rest of your code keeps calling
`Decode.datetime` and gets yours, as long as your module is opened after `Thoth.Json.Core`.

## Writing a decoder by hand

`Decoder<'T>` is an interface with a single method.

```fs
type Decoder<'T> =
    abstract member Decode<'JsonValue> :
        helpers: IDecoderHelpers<'JsonValue> * value: 'JsonValue ->
            Result<'T, DecoderError<'JsonValue>>
```

`helpers` is what makes the decoder work on every runtime. It tests and reads the JSON value without
naming a concrete JSON type: `isString`, `isNumber`, `isObject`, `asString`, `getProperty`, and so
on. See [`IDecoderHelpers`](../../reference/thoth-json-core/thoth-json-core/idecoderhelpers.md).

On failure, return an `ErrorReason` paired with a path. Pass `""` as the path: Thoth.Json prepends
the position it was decoding at.

| Reason | Reported as |
|---|---|
| `BadPrimitive(expected, value)` | Expecting *expected* but instead got *value* |
| `BadPrimitiveExtra(expected, value, message)` | The same, followed by *message* |
| `BadType(expected, value)` | Expecting *expected* but instead got *value* |
| `BadField(expected, value)` | Expecting an object with *expected* |
| `BadPath(expected, value, path)` | Expecting an object with path *path* |
| `TooSmallArray(expected, value)` | Expecting a longer array |
| `FailMessage(message)` | The message alone |
| `BadOneOf(errors)` | Every error that was tried |

## Writing an encoder by hand

An encoder is a function returning `IEncodable`, which writes itself through
[`IEncoderHelpers`](../../reference/thoth-json-core/thoth-json-core/iencoderhelpers.md).

Most of the time you compose the encoders from `Encode` instead, as above. Reach for the interface
when you need a representation none of them produces.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

let unquotedNull: IEncodable =
    { new IEncodable with
        member _.Encode(helpers) = helpers.encodeNull ()
    }

Encode.object [ "value", unquotedNull ] |> Encode.toString 0 |> printfn "%s"
```
