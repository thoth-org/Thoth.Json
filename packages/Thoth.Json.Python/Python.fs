module Python

open Fable.Core
open System

module Json =

    [<Import("JSONDecodeError", "json")>]
    type JSONDecodeError() =
        inherit Exception()
