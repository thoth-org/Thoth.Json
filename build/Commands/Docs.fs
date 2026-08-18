module EasyBuild.Commands.Docs

open System.ComponentModel
open System.IO
open BlackFox.CommandLine
open Spectre.Console.Cli
open SimpleExec
open EasyBuild.Workspace

/// <summary>
/// Projects included in the API reference.
///
/// They need to be compiled because <c>starlight-fsharp-oracle</c> generates the
/// documentation by inspecting their assemblies.
/// </summary>
let private documentedProjects =
    [
        Workspace.packages.``Thoth.Json``.``.``
        Workspace.packages.``Thoth.Json.Core``.``.``
        Workspace.packages.``Thoth.Json.Core.Auto``.``.``
        Workspace.packages.``Thoth.Json.JavaScript``.``.``
        Workspace.packages.``Thoth.Json.Newtonsoft``.``.``
        Workspace.packages.``Thoth.Json.Python``.``.``
        Workspace.packages.``Thoth.Json.System.Text.Json``.``.``
    ]

/// <summary>
/// Type checks the F# literate files used to write the documentation.
///
/// This catches snippets which don't compile anymore, they would otherwise be
/// published as is because <c>starlight-fsharp-literate</c> only transforms them
/// into Markdown.
/// </summary>
/// <returns><c>true</c> if all the files type check</returns>
let private typeCheckLiterateFiles () =
    let files =
        Directory.GetFiles(
            Workspace.docs.src.content.docs.``.``,
            "*.source.fsx",
            SearchOption.AllDirectories
        )
        |> Array.sort

    printfn $"Type checking %i{files.Length} F# literate files..."

    let failures =
        files
        |> Array.filter (fun file ->
            try
                Command.Run(
                    "dotnet",
                    CmdLine.empty
                    |> CmdLine.appendRaw "fsi"
                    |> CmdLine.appendRaw "--typecheck-only"
                    |> CmdLine.appendRaw "--nologo"
                    |> CmdLine.append file
                    |> CmdLine.toString
                )

                false
            with _ ->
                true
        )

    if Array.isEmpty failures then
        true
    else
        printfn ""
        printfn $"%i{failures.Length} F# literate file(s) don't type check:"

        for file in failures do
            printfn $"    - {Path.GetRelativePath(Workspace.``.``, file)}"

        printfn ""
        printfn "Run the command with --skip-typecheck to ignore this check"

        false

type DocsSettings() =

    inherit CommandSettings()

    [<CommandOption("-w|--watch")>]
    [<Description("Start the documentation server and watch for changes")>]
    member val IsWatch: bool = false with get, set

    [<CommandOption("--skip-typecheck")>]
    [<Description("Don't type check the F# literate files")>]
    member val SkipTypeCheck: bool = false with get, set

type DocsCommand() =
    inherit Command<DocsSettings>()
    interface ICommandLimiter<DocsSettings>

    override _.Execute(context: CommandContext, settings: DocsSettings) =
        for project in documentedProjects do
            Command.Run("dotnet", "build --nologo", workingDirectory = project)

        // Type checking is slow, so we skip it when watching to keep the feedback loop fast
        let shouldTypeCheck = not settings.IsWatch && not settings.SkipTypeCheck

        if shouldTypeCheck && not (typeCheckLiterateFiles ()) then
            1
        else

            let astroCommand =
                if settings.IsWatch then
                    "astro dev"
                else
                    "astro build"

            Command.Run(
                "npx",
                astroCommand,
                workingDirectory = Workspace.docs.``.``
            )

            0
