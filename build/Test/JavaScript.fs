module Build.Test.JavaScript

open System.IO
open System
open BlackFox.CommandLine
open Build.Utils
open Build
open SimpleExec
open Path.Operators

let private mochaComand =
    CmdLine.empty
    |> CmdLine.appendRaw "mocha"
    |> CmdLine.appendRaw Workspace.Fsproj.Tests.javascript
    |> CmdLine.appendPrefix "--reporter" "dot"
    |> CmdLine.toString

let handle (args: string list) =
    let isWatch = args |> List.contains "--watch"

    if isWatch then
        failwith "not implemented"
    else

        Command.Run(
            "dotnet",
            CmdLine.empty
            |> CmdLine.appendRaw "fable"
            |> CmdLine.appendPrefix "--outDir" "fableBuild"
            |> CmdLine.toString,
            workingDirectory = Workspace.ProjectDir.Tests.javascript
        )


        Command.Run(
            "npx",
            mochaComand,
            workingDirectory = Workspace.ProjectDir.Tests.javascript
        )
