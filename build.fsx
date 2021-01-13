#r "paket: groupref netcorebuild //"
#load ".fake/build.fsx/intellisense.fsx"
#if !FAKE
#r "Facades/netstandard"
#r "netstandard"
#endif

#nowarn "52"

open System
open System.IO
open System.Text.RegularExpressions
open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.IO.FileSystemOperators
open Fake.Tools
open Fake.Api
open Fake.JavaScript
open BlackFox.Fake

let versionFromGlobalJson : DotNet.CliInstallOptions -> DotNet.CliInstallOptions = (fun o ->
        { o with Version = DotNet.Version (DotNet.getSDKVersionFromGlobalJson()) }
    )

let dotnetSdk = lazy DotNet.install versionFromGlobalJson
let inline dtntWorkDir wd =
    DotNet.Options.lift dotnetSdk.Value
    >> DotNet.Options.withWorkingDirectory wd

let inline yarnWorkDir (ws : string) (yarnParams : Yarn.YarnParams) =
    { yarnParams with WorkingDirectory = ws }

let root = __SOURCE_DIRECTORY__
let projectFile = "./src/Thoth.Json.fsproj"
let testsFile = "./tests/Thoth.Tests.fsproj"

let gitOwner = "thoth-org"
let repoName = "Thoth.Json"

module Util =

    let visitFile (visitor: string -> string) (fileName : string) =
        File.ReadAllLines(fileName)
        |> Array.map (visitor)
        |> fun lines -> File.WriteAllLines(fileName, lines)

    let replaceLines (replacer: string -> Match -> string option) (reg: Regex) (fileName: string) =
        fileName |> visitFile (fun line ->
            let m = reg.Match(line)
            if not m.Success
            then line
            else
                match replacer line m with
                | None -> line
                | Some newLine -> newLine)

// Module to print colored message in the console
module Logger =
    let consoleColor (fc : ConsoleColor) =
        let current = Console.ForegroundColor
        Console.ForegroundColor <- fc
        { new IDisposable with
              member x.Dispose() = Console.ForegroundColor <- current }

    let warn str = Printf.kprintf (fun s -> use c = consoleColor ConsoleColor.DarkYellow in printf "%s" s) str
    let warnfn str = Printf.kprintf (fun s -> use c = consoleColor ConsoleColor.DarkYellow in printfn "%s" s) str
    let error str = Printf.kprintf (fun s -> use c = consoleColor ConsoleColor.Red in printf "%s" s) str
    let errorfn str = Printf.kprintf (fun s -> use c = consoleColor ConsoleColor.Red in printfn "%s" s) str

let run (cmd:string) dir args  =
    Command.RawCommand(cmd, Arguments.OfArgs args)
    |> CreateProcess.fromCommand
    |> CreateProcess.withWorkingDirectory dir
    |> Proc.run
    |> ignore


let needsPublishing (versionRegex: Regex) (newVersion: string) projFile =
    printfn "Project: %s" projFile
    if newVersion.ToUpper().EndsWith("NEXT")
        || newVersion.ToUpper().EndsWith("UNRELEASED")
    then
        Logger.warnfn "Version marked as unreleased version in Changelog, don't publish yet."
        false
    else
        File.ReadLines(projFile)
        |> Seq.tryPick (fun line ->
            let m = versionRegex.Match(line)
            if m.Success then Some m else None)
        |> function
            | None -> failwith "Couldn't find version in project file"
            | Some m ->
                let sameVersion = m.Groups.[1].Value = newVersion
                if sameVersion then
                    Logger.warnfn "Already version %s, no need to publish." newVersion
                not sameVersion

let pushNuget (newVersion: string) (projFile: string) =
    let versionRegex = Regex("<Version>(.*?)</Version>", RegexOptions.IgnoreCase)

    let projDir = Path.GetDirectoryName(projFile)
    let nugetKey =
        match Environment.environVarOrNone "NUGET_KEY" with
        | Some nugetKey -> nugetKey
        | None -> failwith "The Nuget API key must be set in a NUGET_KEY environmental variable"

    let needsPublishing = needsPublishing versionRegex newVersion projFile

    (versionRegex, projFile) ||> Util.replaceLines (fun line _ ->
        versionRegex.Replace(line, "<Version>" + newVersion + "</Version>") |> Some)

    DotNet.pack (fun p ->
        { p with
            Configuration = DotNet.Release
            Common = { p.Common with DotNetCliPath = "dotnet" } } )
        projFile

    if needsPublishing then
        let files =
            Directory.GetFiles(projDir </> "bin" </> "Release", "*.nupkg")
            |> Array.find (fun nupkg -> nupkg.Contains(newVersion))
            |> fun x -> [x]

        Paket.pushFiles (fun o ->
            { o with ApiKey = nugetKey
                     PublishUrl = "https://www.nuget.org/api/v2/package"
                     WorkingDir = __SOURCE_DIRECTORY__ })
            files

let versionRegex = Regex("^## ?\\[?v?([\\w\\d.-]+\\.[\\w\\d.-]+[a-zA-Z0-9])\\]?", RegexOptions.IgnoreCase)

let getLastVersion () =
    File.ReadLines("CHANGELOG.md")
        |> Seq.tryPick (fun line ->
            let m = versionRegex.Match(line)
            if m.Success then Some m else None)
        |> function
            | None -> failwith "Couldn't find version in changelog file"
            | Some m ->
                m.Groups.[1].Value

let isPreRelease (version : string) =
    let regex = Regex(".*(alpha|beta|rc).*", RegexOptions.IgnoreCase)
    regex.IsMatch(version)

let getNotes (version : string) =
    File.ReadLines("CHANGELOG.md")
    |> Seq.skipWhile(fun line ->
        let m = versionRegex.Match(line)

        if m.Success then
            not (m.Groups.[1].Value = version)
        else
            true
    )
    // Remove the version line
    |> Seq.skip 1
    // Take all until the next version line
    |> Seq.takeWhile (fun line ->
        let m = versionRegex.Match(line)
        not m.Success
    )

let clean = BuildTask.create "Clean" [] {
    !! "src/**/bin"
    ++ "src/**/obj"
    ++ "tests/**/bin"
    ++ "tests/**/obj"
    ++ "docs_deploy"
    |> Shell.cleanDirs
}

let yarnInstall = BuildTask.create "YarnInstall" [ ] {
    Yarn.install id
}

let dotnetRestore = BuildTask.create "DotnetRestore" [ clean.IfNeeded ] {
    DotNet.restore id projectFile
    DotNet.restore id testsFile
}

let mochaTest = BuildTask.create "MochaTest" [ clean.IfNeeded; yarnInstall; dotnetRestore ] {
    let projDir = testsFile |> Path.getDirectory
    let configFile = projDir </> "splitter.config.js"
    //Compile to JS
    Yarn.exec ("fable-splitter -c " + configFile) id

    //Run mocha tests
    let projDirOutput = projDir </> "bin"
    Yarn.exec ("run mocha " + projDirOutput) id
}

let publish = BuildTask.create "Publish" [ clean; dotnetRestore; mochaTest ] {
    let version = getLastVersion()
    pushNuget version projectFile
}

let buildDocs = BuildTask.create "BuildDocs" [ clean; yarnInstall ] {
    Yarn.exec "nacara" id
}

Target.description "Start a local webserver allowing you to work on the docs site locally"
let _watchDocs = BuildTask.create "WatchDocs" [ ] {
    Yarn.exec "nacara --watch" id
}

Target.description "Publish a new version of the documentation on github"
let publishDocs = BuildTask.create "PublishDocs" [ buildDocs ] {
    let version = getLastVersion()
    let versionRegex = Regex("version: \"(.*)\",", RegexOptions.IgnoreCase)

    (versionRegex, "./nacara.js") ||> Util.replaceLines (fun line _ ->
        versionRegex.Replace(line, "version: \"" + version + "\",") |> Some)

    Yarn.exec "gh-pages -d docs_deploy" id
}

let _release = BuildTask.create "Release" [ publish; publishDocs ] {
    let version = getLastVersion()

    Git.Staging.stageAll root
    let commitMsg = sprintf "Release version %s" version
    Git.Commit.exec root commitMsg
    Git.Branches.push root

    let token =
        match Environment.environVarOrDefault "GITHUB_TOKEN" "" with
        | s when not (String.IsNullOrWhiteSpace s) -> s
        | _ -> failwith "The Github token must be set in a GITHUB_TOKEN environmental variable"

    let nupkg =
        let projDir = Path.GetDirectoryName(projectFile)

        Directory.GetFiles(projDir </> "bin" </> "Release", "*.nupkg")
        |> Array.find (fun nupkg -> nupkg.Contains(version))

    GitHub.createClientWithToken token
    |> GitHub.draftNewRelease gitOwner repoName version (isPreRelease version) (getNotes version)
    // |> GitHub.uploadFile nupkg
    |> GitHub.publishDraft
    |> Async.RunSynchronously
}


BuildTask.runOrList ()
