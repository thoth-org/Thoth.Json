---
title: Introduction
---

Thoth.Json is a library aiming to make it **safe** and **easy** to work with JSON.

It revolves around two operations:

- **Decoding**, which is the process of converting JSON into an F# value
- **Encoding**, which is the process of converting an F# value into JSON

:::note
When referring to both **decoder** and **encoder**, we use the term **coder**. A **codec** is an
encoder and a decoder for the same type, bundled together.
:::

With Thoth.Json, you don't work directly from the JSON data, but instead describe what JSON you are
expecting. If it is valid, then you can work with concrete F# types.

If it is not, you get an error explaining what was expected, what was received, and where the
problem is:

```text
Error at: `$`
Expecting an object with a field named `age` but instead got:
{
    "name": "Maxime"
}
```

## Core and runtimes

Coders are written against `Thoth.Json.Core`, which describes how a value is read and written
without knowing anything about the JSON representation of your target. A runtime package takes care
of that part, and is the only piece of the puzzle which changes between .NET and the runtimes
supported by [Fable](https://fable.io/).

This means the coders you write are shared by all your projects, whatever they compile to. See
[Cross-target support](/Thoth.Json/concept/cross-target-support/) for the details.

:::note
If you don't want to write your coders by hand, the [Auto API](/Thoth.Json/auto/introduction/)
generates them from your types.
:::
