module EasyBuild.Commands.Publish

open System.IO
open EasyBuild.Tools.DotNet
open EasyBuild.Tools.Git
open Spectre.Console.Cli
open EasyBuild.Commands.Test
open EasyBuild.Workspace
open SimpleExec
open BlackFox.CommandLine

let private publish (projectDir: string) (tag: string) =
    printfn $"Publishing {projectDir}"

    let changelogPath = Path.Combine(projectDir, "CHANGELOG.md")

    let _ =
        DotNet.changelogGen (
            changelogPath,
            tagFilter = [ tag ],
            config = Workspace.``commit.config.json``,
            // Allow dirty working directory because we make a single commit for all packages
            allowDirty = true
        )

    // Delete the bin folder, so dotnet pack will always create a new package
    // Otherwise, if the package already exists, it will not be created
    // and we can't get the package path
    let binFolder = Path.Combine(projectDir, "bin")

    if Directory.Exists binFolder then
        Directory.Delete(binFolder, true)

    let nupkgPath = DotNet.pack projectDir

    DotNet.nugetPush (nupkgPath, skipDuplicate = true)

type PublishSettings() =
    inherit CommandSettings()

type PublishCommand() =
    inherit Command<PublishSettings>()
    interface ICommandLimiter<PublishSettings>

    override _.Execute(context: CommandContext, settings: PublishSettings) =
        TestCommand().Execute(context, TestSettings()) |> ignore

        publish Workspace.packages.``Thoth.Json``.``.`` "legacy"
        publish Workspace.packages.``Thoth.Json.Core``.``.`` "core"
        publish Workspace.packages.``Thoth.Json.JavaScript``.``.`` "javascript"
        publish Workspace.packages.``Thoth.Json.Newtonsoft``.``.`` "newtonsoft"
        publish Workspace.packages.``Thoth.Json.Python``.``.`` "python"

        publish
            Workspace.packages.``Thoth.Json.System.Text.Json``.``.``
            "system.text.json"

        // To not pollute the git history too much, we make a single commit for all packages
        Git.addAll ()

        Command.Run(
            "git",
            CmdLine.empty
            |> CmdLine.appendRaw "commit"
            |> CmdLine.appendPrefix "-m" $"chore: release"
            |> CmdLine.toString
        )

        Git.push ()

        0
