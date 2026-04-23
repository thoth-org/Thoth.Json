module EasyBuild.Commands.Publish

open EasyBuild.Tools.DotNet
open Spectre.Console.Cli
open EasyBuild.Commands.Test
open EasyBuild.Workspace

let private publish (projectDir: string) =
    printfn $"Publishing {projectDir}"

    let nupkgPath = DotNet.pack projectDir

    DotNet.nugetPush (nupkgPath, skipDuplicate = true)

type PublishSettings() =
    inherit CommandSettings()

type PublishCommand() =
    inherit Command<PublishSettings>()
    interface ICommandLimiter<PublishSettings>

    override _.Execute(context: CommandContext, settings: PublishSettings) =
        publish Workspace.packages.``Thoth.Json``.``.``
        publish Workspace.packages.``Thoth.Json.Core``.``.``
        publish Workspace.packages.``Thoth.Json.JavaScript``.``.``
        publish Workspace.packages.``Thoth.Json.Newtonsoft``.``.``
        publish Workspace.packages.``Thoth.Json.Python``.``.``
        publish Workspace.packages.``Thoth.Json.System.Text.Json``.``.``
        publish Workspace.packages.``Thoth.Json.Core.Auto``.``.``

        0
