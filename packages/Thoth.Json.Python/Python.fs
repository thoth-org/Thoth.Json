module Python

open Fable.Core
open System

module Json =

    [<Erase>]
    type IExports =
        [<NamedParams(1)>]
        abstract dumps:
            obj: obj *
            ?separators: string array *
            ?indent: int *
            ?ensure_ascii: bool ->
                string

    [<ImportAll("json")>]
    let json: IExports = nativeOnly

    [<Import("JSONDecodeError", "json")>]
    type JSONDecodeError() =
        inherit Exception()
