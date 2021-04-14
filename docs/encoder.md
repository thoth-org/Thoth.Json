---
description: Explanations of the concepts of Encoder and how to write one
---

# Encoder

Module for turning F\# values into JSON values.

_This module is inspired by_ [_Json.Encode from Elm_](http://package.elm-lang.org/packages/elm-lang/core/latest/Json-Encode)_._

#### How to use it?

```fsharp
    open Thoth.Json

    let person =
        Encode.object
            [ "firstname", Encode.string "maxime"
              "surname", Encode.string "mangel"
              "age", Encode.int 25
              "address", Encode.object
                            [ "street", Encode.string "main street"
                              "city", Encode.string "Bordeaux" ]
            ]

    let compact = Encode.toString 0 person
    // {"firstname":"maxime","surname":"mangel","age":25,"address":{"street":"main street","city":"Bordeaux"}}

    let readable = Encode.toString 4 person
    // {
    //     "firstname": "maxime",
    //     "surname": "mangel",
    //     "age": 25,
    //     "address": {
    //         "street": "main street",
    //         "city": "Bordeaux"
    //     }
    //
```

