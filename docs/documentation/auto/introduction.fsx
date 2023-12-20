(***
layout: nacara/layouts/docs.njk
title: Introduction
***)


(*** hide ***)

#I "../../../src/bin/Debug/netstandard2.0"
#r "Thoth.Json.dll"
#r "nuget: Fable.Core"

open Thoth.Json
open Fable.Core

(**

Auto decoders are generated at runtime for you and will still garantee
that the JSON structure is correct.

They are handy when using F# on both server and client or
if your JSON is a one-to-one mapping with your F# type.

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

Decode.Auto.fromString<User> (userJson)

// Ok { Name = "Geralt de Riv", Age = 92 }
