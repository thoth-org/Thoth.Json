module EasyBuild.Commands.Test

open Spectre.Console.Cli
open System.ComponentModel
open BlackFox.CommandLine
open SimpleExec
open EasyBuild.Workspace
open System.IO

type TestSettings() =
    inherit CommandSettings()

    [<CommandOption("-w|--watch")>]
    [<Description("Watch for changes and re-run the tests")>]
    member val IsWatch: bool = false with get, set

let cleanUp (dir: string) =
    let dir = DirectoryInfo(dir)

    if dir.Exists then
        dir.Delete(true)

let private outDir = "fableBuild"

let private runTestForJavaScript isWatch cwd =
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
                    |> CmdLine.appendRaw "--runScript"
                else
                    CmdLine.empty |> CmdLine.appendRaw "--runScript"
            ]
        |> CmdLine.toString

    Command.Run("dotnet", fableArgs, workingDirectory = cwd)

type TestJavaScriptCommand() =
    inherit Command<TestSettings>()
    interface ICommandLimiter<TestSettings>

    override _.Execute(context: CommandContext, settings: TestSettings) =
        // Remove generated files because this folder is used for both JavaScript and TypeScript tests
        cleanUp
            VirtualWorkspace.tests.``Thoth.Json.Tests.JavaScript``.fableBuild.``.``

        runTestForJavaScript
            settings.IsWatch
            Workspace.tests.``Thoth.Json.Tests.JavaScript``.``.``

        0


type TestNewtonsoftCommand() =
    inherit Command<TestSettings>()
    interface ICommandLimiter<TestSettings>

    override _.Execute(context: CommandContext, settings: TestSettings) =
        Command.Run(
            "dotnet",
            CmdLine.empty
            |> CmdLine.appendIf settings.IsWatch "watch"
            |> CmdLine.appendRaw "run"
            |> CmdLine.appendPrefix
                "--project"
                Workspace.tests.``Thoth.Json.Tests.Newtonsoft``.``.``
            |> CmdLine.toString
        )

        0

type TestPythonCommand() =
    inherit Command<TestSettings>()
    interface ICommandLimiter<TestSettings>

    override _.Execute(context: CommandContext, settings: TestSettings) =
        let runArg =
            if settings.IsWatch then
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
            |> CmdLine.appendRaw "--test:MSBuildCracker"
            |> CmdLine.appendIf settings.IsWatch "--watch"
            |> CmdLine.appendRaw runArg
            |> CmdLine.appendRaw "python"
            |> CmdLine.appendRaw "fableBuild/main.py"
            |> CmdLine.appendRaw "--silent"
            |> CmdLine.toString,
            workingDirectory = Workspace.tests.``Thoth.Json.Tests.Python``.``.``
        )

        0

type TestLegacyCommand() =
    inherit Command<TestSettings>()
    interface ICommandLimiter<TestSettings>

    override _.Execute(context: CommandContext, settings: TestSettings) =
        runTestForJavaScript
            settings.IsWatch
            Workspace.tests.``Thoth.Json.Tests.Legacy``.``.``

        0

type TestTypeScriptCommand() =
    inherit Command<TestSettings>()
    interface ICommandLimiter<TestSettings>

    override _.Execute(context: CommandContext, settings: TestSettings) =
        // Remove generated files because this folder is used for both JavaScript and TypeScript tests
        cleanUp
            VirtualWorkspace.tests.``Thoth.Json.Tests.JavaScript``.fableBuild.``.``

        Command.Run(
            "dotnet",
            CmdLine.empty
            |> CmdLine.appendRaw "fable"
            |> CmdLine.appendPrefix "--outDir" outDir
            |> CmdLine.appendPrefix "--lang" "typescript"
            |> CmdLine.appendRaw "--noCache"
            |> CmdLine.appendRaw "--test:MSBuildCracker"
            |> CmdLine.appendIf settings.IsWatch "--watch"
            |> CmdLine.appendRaw "--runWatch"
            |> CmdLine.appendRaw "npx tsc"
            |> CmdLine.toString,
            workingDirectory =
                Workspace.tests.``Thoth.Json.Tests.JavaScript``.``.``
        )

        0

type TestCommand() =
    inherit Command<TestSettings>()
    interface ICommandLimiter<TestSettings>

    override _.Execute(context: CommandContext, settings: TestSettings) =
        TestJavaScriptCommand().Execute(context, settings) |> ignore
        // Not stable offically supported, yet as there are bugs that needs to be fixed in Fable
        // TestTypeScriptCommand().Execute(context, settings) |> ignore
        TestNewtonsoftCommand().Execute(context, settings) |> ignore
        TestPythonCommand().Execute(context, settings) |> ignore
        TestLegacyCommand().Execute(context, settings) |> ignore

        0
