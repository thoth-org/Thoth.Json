module Thoth.Json.Core.Auto.Casing

open System
open System.Text

let private upperFirst (str: string) =
    str.[..0].ToUpperInvariant() + str.[1..]

/// Well-known two-letter acronyms that should remain all-caps in .NET naming conventions.
/// Per Microsoft's guidelines, common acronyms like ID and IP should be kept uppercase
/// (e.g., UserID instead of UserId, CustomerIP instead of CustomerIp).
/// This matches how .NET Framework types are named (IPAddress, IDisposable, etc.).
let private dotNetAcronyms =
    Set.ofSeq
        [
            "id"
            "ip"
        ]

let convertCase (source: CaseStrategy) (dest: CaseStrategy) (text: string) =
    if source = dest then
        text
    else
        let words =
            match source with
            | SnakeCase
            | ScreamingSnakeCase ->
                text.Split([| '_' |], StringSplitOptions.RemoveEmptyEntries)
                |> Seq.toList
            | PascalCase
            | CamelCase
            | DotNetPascalCase
            | DotNetCamelCase ->
                seq {
                    let sb = StringBuilder()

                    for c in text do
                        if Char.IsUpper c && sb.Length > 0 then
                            sb.ToString()
                            sb.Clear() |> ignore

                        sb.Append(c) |> ignore

                    if sb.Length > 0 then
                        sb.ToString()
                }
                |> Seq.fold
                    (fun state next ->
                        if next.Length > 1 then
                            next :: state
                        else
                            match state with
                            | [] -> [ next ]
                            | x :: xs ->
                                if
                                    x.Length = 1
                                    || x |> Seq.forall Char.IsUpper
                                then
                                    (x + next) :: xs
                                else
                                    next :: x :: xs
                    )
                    []
                |> Seq.rev
                |> Seq.toList

        match dest with
        | ScreamingSnakeCase ->
            words
            |> Seq.map (fun x -> x.ToUpperInvariant())
            |> String.concat "_"
        | SnakeCase ->
            words
            |> Seq.map (fun x -> x.ToLowerInvariant())
            |> String.concat "_"
        | PascalCase ->
            words
            |> Seq.map (fun x -> x.ToLowerInvariant() |> upperFirst)
            |> String.concat ""
        | CamelCase ->
            words
            |> Seq.mapi (fun i x ->
                if i = 0 then
                    x.ToLowerInvariant()
                else
                    x.ToLowerInvariant() |> upperFirst
            )
            |> String.concat ""
        | DotNetPascalCase ->
            words
            |> Seq.map (fun x ->
                let u = x.ToLowerInvariant()

                if Set.contains u dotNetAcronyms then
                    u.ToUpperInvariant()
                else
                    upperFirst u
            )
            |> String.concat ""
        | DotNetCamelCase ->
            words
            |> Seq.mapi (fun i x ->
                if i = 0 then
                    x.ToLowerInvariant()
                else
                    let u = x.ToLowerInvariant()

                    if Set.contains u dotNetAcronyms then
                        u.ToUpperInvariant()
                    else
                        upperFirst u
            )
            |> String.concat ""
