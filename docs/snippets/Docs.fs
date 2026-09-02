module Docs

/// <summary>
/// Print a decoding result: the value on success, the error message as Thoth.Json wrote it.
/// </summary>
let print (result: Result<'T, string>) =
    match result with
    | Ok value -> printfn "%A" value
    | Error error -> printfn "%s" error
