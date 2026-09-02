---
title: JSON representation
toc:
  to: 3
---

The auto API gives you very limited control over the JSON representation. What it produces is
described here.

Where the default doesn't fit, override the type with an [extra coder](extra-coders.md), or write
the coder yourself with the [manual](../manual/introduction.md) or [codec](../codec/introduction.md)
API.

## Primitives

Primitives are represented the same way as with the manual API. See
[Manual API - JSON representation - Numbers](../manual/json-representation.md#numbers).

`int64`, `uint64`, `decimal` and `bigint` need an [extra coder](extra-coders.md).

## DateTime

A `DateTime` is represented as an ISO 8601 string, and decoded as UTC.

A value whose `Kind` is `Local` or `Unspecified` comes back as the same instant with `Kind` set to
`Utc`, not as the value you encoded.

```fsharp live
open System
open Thoth.Json.Core
open Thoth.Json.Core.Auto
open Thoth.Json.JavaScript

type Event =
    {
        At: DateTime
    }

let encoder = Encode.Auto.generateEncoder<Event> ()
let decoder = Decode.Auto.generateDecoder<Event> ()

for kind in [ DateTimeKind.Utc; DateTimeKind.Local ] do
    let json =
        {
            At = DateTime(2020, 1, 1, 12, 0, 0, kind)
        }
        |> encoder
        |> Encode.toString 0

    printfn "%A" kind
    printfn "%s" json
    Decode.fromString decoder json |> Docs.print
```

A `DateTimeOffset` keeps its offset.

## Records

Records are represented as JSON objects, one property per field, named after the field and renamed
by the [case strategy](configuration.md#casestrategy).

```fsharp live
open Thoth.Json.Core
open Thoth.Json.Core.Auto
open Thoth.Json.JavaScript

type User =
    {
        Name: string
        Age: int
    }

{
    Name = "Geralt de Riv"
    Age = 92
}
|> Encode.Auto.generateEncoder<User> ()
|> Encode.toString 4
|> printfn "%s"
```

## Union case with no fields

A case without fields is represented as a string holding the case name.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.Core.Auto
open Thoth.Json.JavaScript

[<RequireQualifiedAccess>]
type Language =
    | FSharp
    | CSharp

Language.FSharp
|> Encode.Auto.generateEncoder<Language> ()
|> Encode.toString 0
|> printfn "%s"
```

## Union case with fields

A case with fields is represented as a JSON array, holding the case name followed by one element per
field.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.Core.Auto
open Thoth.Json.JavaScript

type MenuElement =
    | Label of label: string
    | ExternalLink of label: string * url: string

Label "Introduction"
|> Encode.Auto.generateEncoder<MenuElement> ()
|> Encode.toString 4
|> printfn "%s"

ExternalLink(label = "Fable", url = "http://fable.io")
|> Encode.Auto.generateEncoder<MenuElement> ()
|> Encode.toString 4
|> printfn "%s"
```

## Option

By default the option type is erased:

- `Some 42` is encoded as `42`
- `None` is encoded as `null`, and left out of an object unless
  [`skipNullField`](configuration.md#skipnullfield) is `false`

:::warning
This means that nested option support is limited.

With a nested option like `(int option) option`, `42` can't tell `Some 42` from `Some (Some 42)`,
and `null` can't tell `None` from `Some None`.

Set [`losslessOption`](configuration.md#losslessoption) to `true` when you need the distinction.
:::

## Collections

Lists, arrays, sequences, sets and tuples are represented as JSON arrays.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.Core.Auto
open Thoth.Json.JavaScript

(1, "one", true)
|> Encode.Auto.generateEncoder<int * string * bool> ()
|> Encode.toString 0
|> printfn "%s"

set [ 1; 2; 3 ]
|> Encode.Auto.generateEncoder<Set<int>> ()
|> Encode.toString 0
|> printfn "%s"
```

## Map

A `Map<string, 'T>` is represented as a JSON object.

Any other key type gives an array of `[ key, value ]` pairs.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.Core.Auto
open Thoth.Json.JavaScript

Map.ofList
    [
        "one", 1
        "two", 2
    ]
|> Encode.Auto.generateEncoder<Map<string, int>> ()
|> Encode.toString 0
|> printfn "%s"

Map.ofList
    [
        1, "one"
        2, "two"
    ]
|> Encode.Auto.generateEncoder<Map<int, string>> ()
|> Encode.toString 0
|> printfn "%s"
```

## Enum

An enum is represented as the number it compiles to. Decoding rejects a number which isn't one of
the declared values.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.Core.Auto
open Thoth.Json.JavaScript

type Rating =
    | One = 1
    | Two = 2
    | Three = 3

Rating.Two
|> Encode.Auto.generateEncoder<Rating> ()
|> Encode.toString 0
|> printfn "%s"

"7" |> Decode.fromString (Decode.Auto.generateDecoder<Rating> ()) |> Docs.print
```

## Class

Classes have to be added case by case through the [`extra`](extra-coders.md) argument.

Fable offers a limited reflection API, and classes are not part of it.
