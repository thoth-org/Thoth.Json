(**
---
layout: standard
title: Encoder
---
*)

(*** hide ***)

#I "../../src/bin/Debug/netstandard2.0"
#r "Thoth.Json.dll"

open Thoth.Json

(**

An encoder is a function that takes an F# value and transforms it into a JSON value.

**Example of an encoder:**

*)

type User =
    {
        Name: string
        Age: int
        KnownLangs : string list
    }

module User =

    let encoder (user : User) =
        Encode.object [
            "name", Encode.string user.Name
            "age", Encode.int user.Age
            "known-langs",
                user.KnownLangs
                |> List.map Encode.string
                |> Encode.list
        ]
