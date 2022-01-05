(**
---
layout: standard
title: Override defaults
---
*)

(*** hide ***)

#I "../../../src/bin/Debug/netstandard2.0"
#r "Thoth.Json.dll"

open Thoth.Json

(**

When using the manual API, you can override the defaults of Thoth.Json.

For example, if you don't like how the default coder works with the dates,
you can write your own coder for it and make it "replace" the default one.

*)
open System

module Encode =

    let datetime (date : DateTime) =
        DateTimeOffset(date).ToUnixTimeSeconds()
        |> box

module Decode =

    let datetime : Decoder<DateTime> =
        fun path value ->
            if Decode.Helpers.isNumber value then
                let value : int64 = unbox value
                let datetime =
                    DateTimeOffset
                        .FromUnixTimeSeconds(value)
                        .DateTime
                Ok datetime

            else
                (path, BadPrimitive("a timestamp", value)) |> Error

(**

Then in your code, you can open your `Thoth.Json.Custom` module after `Thoth.Json`

```fs
open Thoth.Json
open Thoth.Json.Custom

// From here, Encode.datetime and Decode.datetime are your custom implementation
```

*)
