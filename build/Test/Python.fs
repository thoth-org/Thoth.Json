module Build.Test.Python

open BlackFox.CommandLine
open Build
open SimpleExec

let private outDir = "fableBuild"

let handle (args: string list) =
    let isWatch = args |> List.contains "--watch"

    let runArg =
        if isWatch then
            "--runWatch"
        else
            "--run"

    Command.Run(
        "dotnet",
        CmdLine.empty
        |> CmdLine.appendRaw "fable"
        |> CmdLine.appendPrefix "--outDir" outDir
        |> CmdLine.appendPrefix "--lang" "python"
        |> CmdLine.appendRaw "--noCache"
        |> CmdLine.appendIf isWatch "--watch"
        |> CmdLine.appendRaw runArg
        |> CmdLine.appendRaw "python"
        |> CmdLine.appendRaw "fableBuild/main.py"
        |> CmdLine.appendRaw "--silent"
        |> CmdLine.toString,
        workingDirectory = Workspace.ProjectDir.Tests.python
    )
