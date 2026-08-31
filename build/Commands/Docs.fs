module EasyBuild.Commands.Docs

open System.ComponentModel
open Spectre.Console.Cli
open SimpleExec
open BlackFox.CommandLine
open EasyBuild.Tools.Npm
open EasyBuild.Workspace

/// <summary>What every one of these takes: the port, and anything the site knows that this
/// does not - written after <c>--</c> and passed on as it stands.</summary>
type DocsSettings() =
    inherit CommandSettings()

    [<CommandOption("-p|--port <PORT>")>]
    [<Description("The port to serve on.")>]
    member val Port = 0 with get, set

/// <summary>Everything this command did not recognise, put back the way it was written.</summary>
let private forwarded (context: CommandContext) =
    let options =
        context.Remaining.Parsed
        |> Seq.collect (fun option ->
            [
                // The key arrives with its dashes already on it.
                for value in option do
                    option.Key

                    if not (isNull value) then
                        value
            ]
        )

    let positionals =
        context.Remaining.Raw
        |> Seq.filter (fun argument -> not (argument.StartsWith "-"))

    Seq.append options positionals

/// <summary>Runs the site with a command of its own, and whatever else was asked for.</summary>
let private site
    (command: string)
    (watch: bool)
    (settings: DocsSettings)
    (extra: string seq)
    =
    let program, before =
        if watch then
            // Without this, dotnet watch hot-reloads the running site in place instead of restarting it.
            "dotnet",
            [
                "watch"
                "--no-hot-reload"
            ]
        else
            "dotnet", [ "run" ]

    let arguments =
        before
        |> List.fold
            (fun line argument -> CmdLine.appendRaw argument line)
            CmdLine.empty
        |> CmdLine.appendPrefix "--project" Workspace.docs.``Docs.fsproj``
        |> CmdLine.appendRaw "--"
        |> CmdLine.appendRaw command
        |> fun line ->
            if settings.Port > 0 then
                CmdLine.appendPrefix "--port" (string settings.Port) line
            else
                line
        |> fun line ->
            extra
            |> Seq.fold
                (fun line argument -> CmdLine.appendRaw argument line)
                line
        |> CmdLine.toString

    Command.Run(program, arguments, workingDirectory = Workspace.``.``)
    0

type BuildCommand() =
    inherit Command<DocsSettings>()
    interface ICommandLimiter<CommandSettings>

    override _.Execute(context, settings) =
        site "build" false settings (forwarded context)

type CheckCommand() =
    inherit Command<DocsSettings>()
    interface ICommandLimiter<CommandSettings>

    override _.Execute(context, settings) =
        site "check" false settings (forwarded context)

type CleanCommand() =
    inherit Command<DocsSettings>()
    interface ICommandLimiter<CommandSettings>

    override _.Execute(context, settings) =
        site "clean" false settings (forwarded context)

type DeployCommand() =
    inherit Command<DocsSettings>()
    interface ICommandLimiter<CommandSettings>

    override _.Execute(context, settings) =
        site "gh-pages" false settings (forwarded context)

type WatchSettings() =
    inherit DocsSettings()

    [<CommandOption("--host [HOST]")>]
    [<Description("Listen on an address other than localhost. On its own, every interface.")>]
    member val Host = FlagValue<string>() with get, set

    [<CommandOption("--no-restart")>]
    [<Description("Serve without rebuilding the site when its own code changes.")>]
    member val NoRestart = false with get, set

type WatchCommand() =
    inherit Command<WatchSettings>()
    interface ICommandLimiter<CommandSettings>

    override _.Execute(context, settings) =
        let host =
            [
                if settings.Host.IsSet then
                    "--host"

                    if not (isNull settings.Host.Value) then
                        settings.Host.Value
            ]

        site
            "watch"
            (not settings.NoRestart)
            settings
            (Seq.append host (forwarded context))
