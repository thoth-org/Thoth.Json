module Build.Utils

open System.IO
open System

module Path =

    module Operators =

        let (</>) (p1: string) (p2: string) : string = Path.Combine(p1, p2)

type Path =

    /// <summary>
    /// Resolve a path relative to the repository root
    /// </summary>
    static member Resolve([<ParamArray>] segments: string array) : string =
        let paths = Array.concat [ [| __SOURCE_DIRECTORY__; ".." |]; segments ]

        printfn "__SOURCE_DIRECTORY__: %A" __SOURCE_DIRECTORY__
        printfn "segments: %A" segments
        printfn "CWD: %A" Environment.CurrentDirectory
        printfn "__SOURCE_FILE__: %A" __SOURCE_FILE__

        // Use GetFullPath to clean the path
        Path.GetFullPath(Path.Combine(paths))
