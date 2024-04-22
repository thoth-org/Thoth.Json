module Build.Test.Newtonsoft

open BlackFox.CommandLine
open Build
open SimpleExec

let handle (args: string list) =
    let isWatch = args |> List.contains "--watch"

    Command.Run(
        "dotnet",
        CmdLine.empty
        |> CmdLine.appendIf isWatch "watch"
        |> CmdLine.appendRaw "run"
        |> CmdLine.appendPrefix "--project" Workspace.Fsproj.Tests.newtonsoft
        |> CmdLine.toString,
        workingDirectory = Workspace.ProjectDir.Tests.newtonsoft
    )
