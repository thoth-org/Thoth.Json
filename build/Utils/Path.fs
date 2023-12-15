module Build.Utils.Path

open System.IO
open System

module Operators =

    let (</>) (p1: string) (p2: string) : string = Path.Combine(p1, p2)

type Path =

    /// <summary>
    /// Resolve a path relative to the repository root
    /// </summary>
    static member Resolve([<ParamArray>] segments: string array) : string =
        // No idea why but __SOURCE_DIRECTORY__ is not working on CI
        // it doesn't returns the correct path
        // let paths = Array.concat [ [| __SOURCE_DIRECTORY__; ".." |]; segments ]

        // Use Environment.CurrentDirectory instead even if it means that we
        // need to be in the expected directory when running the build script
        let paths =
            Array.concat
                [
                    [| Environment.CurrentDirectory |]
                    segments
                ]

        // Use GetFullPath to clean the path
        Path.GetFullPath(Path.Combine(paths))
