---
layout: nacara/layouts/docs.njk
title: Codecs
---

Codecs can be generated automatically.

```fsharp
type FooBar =
    {
        Foo : int
        Bar : bool
        Baz : string list
    }

module FooBar =

    let codec : Codec<FooBar> = Codec.Auto.generateCodec(CamelCase)
```

Beware that at this time, the generated codec may not guarantee the round-trip property!
