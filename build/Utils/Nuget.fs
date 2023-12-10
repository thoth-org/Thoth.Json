module Build.Utils.Dotnet

open SimpleExec
open System.Text.RegularExpressions
open BlackFox.CommandLine

type Nuget =

    static member push
        (
            nupkgPath: string,
            nugetKey: string,
            ?skipDuplicate: bool
        )
        =
        let skipDuplicate = defaultArg skipDuplicate false

        Command.Run(
            "dotnet",
            CmdLine.empty
            |> CmdLine.appendRaw "nuget"
            |> CmdLine.appendRaw "push"
            |> CmdLine.appendRaw nupkgPath
            |> CmdLine.appendPrefix "-k" nugetKey
            |> CmdLine.appendPrefix "-s" "https://api.nuget.org/v3/index.json"
            |> CmdLine.appendIf skipDuplicate "--skip-duplicate"
            |> CmdLine.toString
        )

let pack (projectDir: string) =
    let struct (standardOutput, _) =
        Command.ReadAsync(
            "dotnet",
            "pack -c Release",
            workingDirectory = projectDir
        )
        |> Async.AwaitTask
        |> Async.RunSynchronously

    let m =
        Regex.Match(
            standardOutput,
            "Successfully created package '(?'nupkgPath'.*\.nupkg)'"
        )

    if m.Success then
        m.Groups.["nupkgPath"].Value
    else
        failwithf
            $"""Failed to find nupkg path in output:
Output:
{standardOutput}"""
