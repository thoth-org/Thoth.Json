---
layout: standard
title: Fable & .NET Support
---

.NET and Fable support are available via 2 different libraries:

- `Thoth.Json` for Fable
- `Thoth.Json.Net` for .NET

You can share your code between Fable and .NET by using compiler directives.

```fs
#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

// Here you can write your code as usual
```
