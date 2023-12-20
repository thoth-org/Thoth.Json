---
layout: nacara/layouts/docs.njk
title: Introduction
---

Thoth.Json is a library aiming to make it **safe** and **easy** to work with JSON.

It revolved around two concepts:

- **Decoding**, which is the process of converting a JSON string into an F# type
- **Encoding**, which is the process of converting an F# type into a JSON string

:::info
When referring to both **decoder** and **encoder**, we use the term **coder**
:::

With Thoth.Json, you don't work directly from the JSON data, but instead describe what JSON you are expecting and if it is valid, then you can work with concrete F# types.

This allows to reports helpful errors, making it easy to spot errors and fix them.
