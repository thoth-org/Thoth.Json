---
title: Exact numbers
---

`JSON.parse` does not keep the text a number was written with.

`10` and `10.0` give the same value, and integers above 2<sup>53</sup> are rounded.

Because of that, when using `Thoth.Json.JavaScript`, some numbers decode differently than on .NET
and Python.

| JSON | Decoder | Default | `ExactNumbers = true` |
|---|---|---|---|
| `25` | `Decode.int16` | `Ok 25s` | `Ok 25s` |
| `25.0` | `Decode.int16` | `Ok 25s` | `Error`, value is not an integral value |
| `25.5` | `Decode.float` | `Ok 25.5` | `Ok 25.5` |
| `1e3` | `Decode.int` | `Ok 1000` | `Ok 1000` |
| `9223372036854775806` | `Decode.int64` | `Error`, got `9223372036854776000` | `Ok 9223372036854775806L` |
| `9223372036854775806` | `Decode.uint64` | `Ok 9223372036854776000UL` | `Ok 9223372036854775806UL` |

The last column is what .NET and Python return.

If you need exact numbers, pass `DecodeOptions` to `Decode.fromStringWithOptions`:

```fs
open Thoth.Json.Core
open Thoth.Json.JavaScript

let options =
    { DecodeOptions.defaults with
        ExactNumbers = true
    }

Decode.fromStringWithOptions (options, Decode.int16) "25.0"
// Error "Error at: `$`
// Expecting an int16 but instead got: 25.0
// Reason: Value is not an integral value"

Decode.fromStringWithOptions (options, Decode.int64) "9223372036854775806"
// Ok 9223372036854775806L
```

`Decode.unsafeFromStringWithOptions` takes the same options and raises on failure. Both accept a
`Codec<'T>` in place of a `Decoder<'T>`.

This option is only in `Thoth.Json.JavaScript`. The other packages already tell `10` apart
from `10.0`.

## Options

### ExactNumbers

**type:** `bool`

**default:** `false`

Read the text of number literals. A number written with a fraction, such as `10.0`, is not an
integral value, and an integer keeps every digit it was written with.

## Cost

`ExactNumbers` installs a `JSON.parse` reviver, which parses more slowly. Measured on Node 22
against a payload of records:

| Payload | Default | `ExactNumbers` | Ratio |
|---|---|---|---|
| 1 KB | 0.036 ms | 0.078 ms | 2.2x |
| 10 KB | 0.62 ms | 1.03 ms | 1.7x |
| 105 KB | 38 ms | 41 ms | 1.1x |

The share of a decode spent parsing falls as the payload grows, so the overhead is largest on small
payloads.

## Parsing the JSON yourself

`Decode.numberLiteralReviver` is the reviver behind the option. Pass it to your own `JSON.parse`
call and decode the result with `Decode.fromValue`.

```fs
open Fable.Core.JsInterop
open Thoth.Json.Core
open Thoth.Json.JavaScript

let json = "25.0"

let parsed: obj =
    emitJsExpr (json, Decode.numberLiteralReviver) "JSON.parse($0, $1)"

Decode.fromValue Decode.int16 parsed
// Error "Error at: `$`
// Expecting an int16 but instead got: 25.0
// Reason: Value is not an integral value"
```

`Decode.fromValue` receives an already parsed value, so it reads number literals only when the
value was parsed with this reviver.

## Runtime support

`ExactNumbers` needs a runtime that passes the source text to the reviver.

| Runtime | Version |
|---|---|
| Chrome | 114 |
| Firefox | 135 |
| Safari | 18.4 |
| Node.js | 21.0.0 |
| Deno | 1.33 |
| Bun | 1.1.43 |

On an older runtime the option has no effect and decoding behaves as it does by default.
