---
title: Writing a runtime package
---

A coder written against `Thoth.Json.Core` never names a concrete JSON type. It goes through two
interfaces, and a runtime package is an implementation of them plus the entry points that use them.

Write one when you want Thoth.Json on top of a JSON library none of the shipped packages covers.

## The decoder side

[`IDecoderHelpers<'JsonValue>`](../../reference/thoth-json-core/thoth-json-core/idecoderhelpers.md)
tests and reads a value of your library's JSON type.

```fs
open Thoth.Json.Core

module Decode =

    let helpers =
        { new IDecoderHelpers<MyJson> with
            member _.isString value = ...
            member _.isNumber value = ...
            member _.isBoolean value = ...
            member _.isNullValue value = ...
            member _.isArray value = ...
            member _.isObject value = ...
            member _.isIntegralValue value = ...
            member _.hasProperty name value = ...
            member _.asString value = ...
            member _.asFloat value = ...
            member _.asFloat32 value = ...
            member _.asInt value = ...
            member _.asBoolean value = ...
            member _.asArray value = ...
            member _.getProperty(name, value) = ...
            member _.getProperties value = ...
            member _.anyToString value = ...
            member _.numberToString value = ...
        }
```

`anyToString` is what the error messages print, so it should produce readable JSON. `numberToString`
prints a number without the formatting the runtime would otherwise apply.

## The encoder side

[`IEncoderHelpers<'JsonValue>`](../../reference/thoth-json-core/thoth-json-core/iencoderhelpers.md)
builds a value of your library's JSON type.

```fs
module Encode =

    let helpers =
        { new IEncoderHelpers<MyJson> with
            member _.encodeString value = ...
            member _.encodeChar value = ...
            member _.encodeDecimalNumber value = ...
            member _.encodeBool value = ...
            member _.encodeNull() = ...
            member _.encodeObject values = ...
            member _.encodeArray values = ...
            member _.encodeList values = ...
            member _.encodeSeq values = ...
            member _.encodeResizeArray values = ...
            member _.encodeSignedIntegralNumber value = ...
            member _.encodeUnsignedIntegralNumber value = ...
        }
```

Signed and unsigned integral numbers have separate members because some runtimes represent them
differently.

## The entry points

`Decode.Advanced.fromValue` runs a decoder with your helpers. Around it, provide the three members
every runtime package offers, so code moving between runtimes keeps compiling.

```fs
type Decode =

    static member fromValue(decoder: Decoder<'T>) =
        Decode.Advanced.fromValue Decode.helpers decoder

    static member fromString(decoder: Decoder<'T>) =
        fun (value: string) ->
            match MyJsonLibrary.parse value with
            | json ->
                match decoder.Decode(Decode.helpers, json) with
                | Ok success -> Ok success
                | Error error ->
                    let finalError = error |> Decode.Helpers.prependPath "$"
                    Error(Decode.errorToString Decode.helpers finalError)

    static member unsafeFromString(decoder: Decoder<'T>) =
        fun value ->
            match Decode.fromString decoder value with
            | Ok x -> x
            | Error e -> failwith e
```

`Decode.Helpers.prependPath "$"` puts the root at the head of the JSONPath in the error.
`Decode.errorToString` formats the message.

Add an overload of each taking a `Codec<'T>`, forwarding through `Decode.codec`.

Encoding needs `Encode.toJsonValue`, then whatever your library does to write a string.

```fs
module Encode =

    let toString (space: int) (value: IEncodable) : string =
        Encode.toJsonValue helpers value |> MyJsonLibrary.stringify space
```

## Testing it

The test suite is shared. `tests/Thoth.Json.Tests` holds every test, and each runtime package has a
runner implementing `IEncode` and `IDecode` on top of its own entry points. Add a runner and the
whole suite runs against your package.
