module Python

open Fable.Core

module Json =

    [<Erase>]
    type IExports =
        [<NamedParams(1)>]
        abstract dumps: obj: obj * ?separators : string array * ?indent : int -> string

    [<ImportAll("json")>]
    let json: IExports = nativeOnly
