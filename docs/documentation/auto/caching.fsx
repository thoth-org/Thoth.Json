(***
layout: nacara/layouts/docs.njk
title: Cache
***)


(*** hide ***)

#I "../../../src/bin/Debug/netstandard2.0"
#r "Thoth.Json.dll"
#r "nuget: Fable.Core"

open Thoth.Json
open Fable.Core

(**

Auto decoders use reflection to generate the decoders at runtime.

To avoid having to regenerate the decoders every time you run you need them,
you can use the helpers with the `Cached` suffix instead.

The cache is dependent on the `caseStrategy` and `extra` parameters. If you need
different case strategies or extra parameters, you need to create as many caches.

The easiest way to use the cache is to add some helpers in your code and use them in
the rest of your application.

*)

// Build your extra coders if needed
let myExtraCoders = Extra.empty

// Note the helpers must be inlined to resolve generic parameters in Fable
let inline myDecoder<'T> =
    Decode.Auto.generateDecoderCached<'T> (
        caseStrategy = CamelCase,
        extra = myExtraCoders
    )

let inline myyEncoder<'T> =
    Encode.Auto.generateEncoderCached<'T> (
        caseStrategy = CamelCase,
        extra = myExtraCoders
    )

(**

Now you can use this helper when you need to decode a type:

*)

type User =
    {
        Name: string
        Age: int
    }

let userJson =
    """
{
    name: "Geralt de Riv",
    age: 92
}
    """

Decode.fromString myDecoder<User> userJson

// Ok { Name = "Geralt de Riv", Age = 92 }
