module EasyBuild.Commands.Publish

open System
open System.IO
open Build.Utils
open Spectre.Console.Cli
open EasyBuild.Commands.Test
open EasyBuild.Workspace

let private publish (projectDir: string) =
    printfn $"Publishing {projectDir}"

    let nugetKey = Environment.GetEnvironmentVariable("NUGET_KEY")

    // Delete the bin folder, so dotnet pack will always create a new package
    // Otherwise, if the package already exists, it will not be created
    // and we can't get the package path
    let binFolder = Path.Combine(projectDir, "bin")

    if Directory.Exists binFolder then
        Directory.Delete(binFolder, true)

    if isNull nugetKey then
        failwithf $"Missing NUGET_KEY environment variable"

    let nupkgPath = Dotnet.pack projectDir
    Dotnet.Nuget.push (nupkgPath, nugetKey, skipDuplicate = true)

type PublishSettings() =
    inherit CommandSettings()

type PublishCommand() =
    inherit Command<PublishSettings>()
    interface ICommandLimiter<PublishSettings>

    override _.Execute(context: CommandContext, settings: PublishSettings) =
        TestCommand().Execute(context, TestSettings()) |> ignore

        publish Workspace.packages.``Thoth.Json``.``.``
        publish Workspace.packages.``Thoth.Json.Core``.``.``
        publish Workspace.packages.``Thoth.Json.JavaScript``.``.``
        publish Workspace.packages.``Thoth.Json.Newtonsoft``.``.``
        publish Workspace.packages.``Thoth.Json.Python``.``.``

        0
