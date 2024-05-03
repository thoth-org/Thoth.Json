module Build.Test.Legacy

open BlackFox.CommandLine
open Build
open SimpleExec

let private outDir = "fableBuild"

let private mochaComand =
    CmdLine.empty
    |> CmdLine.appendRaw "npx"
    |> CmdLine.appendRaw "mocha"
    |> CmdLine.appendRaw outDir
    |> CmdLine.appendPrefix "--reporter" "dot"
    |> CmdLine.toString

let handle (args: string list) =
    let isWatch = args |> List.contains "--watch"

    let fableArgs =
        CmdLine.concat
            [
                CmdLine.empty
                |> CmdLine.appendRaw "fable"
                |> CmdLine.appendPrefix "--outDir" outDir
                |> CmdLine.appendRaw "--noCache"
                |> CmdLine.appendRaw "--test:MSBuildCracker"

                if isWatch then
                    CmdLine.empty
                    |> CmdLine.appendRaw "--watch"
                    |> CmdLine.appendRaw "--runWatch"
                    |> CmdLine.appendRaw mochaComand
                else
                    CmdLine.empty
                    |> CmdLine.appendRaw "--run"
                    |> CmdLine.appendRaw mochaComand
            ]
        |> CmdLine.toString

    Command.Run(
        "dotnet",
        fableArgs,
        workingDirectory = Workspace.ProjectDir.Tests.legacy
    )
