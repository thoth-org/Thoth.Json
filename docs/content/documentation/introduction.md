---
title: Introduction
---

Thoth.Json is a library aiming to make it **safe** and **easy** to work with JSON.

It revolves around two concepts:

- **Decoding**, which is the process of converting a JSON string into an F# type
- **Encoding**, which is the process of converting an F# type into a JSON string

:::info
When referring to both **decoder** and **encoder**, we use the term **coder**.
:::

With Thoth.Json, you don't work directly from the JSON data, but instead describe what JSON you
are expecting and if it is valid, then you can work with concrete F# types.

This allows to report helpful errors, making it easy to spot errors and fix them.

## The three APIs

Thoth.Json offers three ways to obtain a coder. They share the same types, so you can mix them in
the same program.

| API | What you write | Use it when |
|---|---|---|
| [Manual](manual/introduction.md) | A decoder and an encoder, separately | The JSON doesn't match your F# types one-to-one |
| [Codec](codec/introduction.md) | Both directions at once | Encoding and decoding must stay in sync |
| [Auto](auto/introduction.md) | Nothing, reflection reads your types | The JSON is a one-to-one mapping of your F# types |

## Running the examples

Code blocks with a **Run** button compile in your browser and run there. Edit them and press Run
again to see what changes.

They use `Docs.print`, a helper from this site which prints the decoded value on success and the
error message as Thoth.Json wrote it on failure.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

Decode.fromString Decode.int "1" |> Docs.print

Decode.fromString Decode.int "\"maxime\"" |> Docs.print
```
