module Build.Publish

open System
open System.IO
open SimpleExec
open Build.Utils

let publish (projectDir: string) =

    let nugetKey = Environment.GetEnvironmentVariable("NUGET_KEY")

    if isNull nugetKey then
        failwithf $"Missing NUGET_KEY environment variable"

    let nupkgPath = Dotnet.pack projectDir
    // Dotnet.Nuget.push (nupkgPath, nugetKey, skipDuplicate = true)
    ()

let handle (_args: string list) =
    publish Workspace.Fsproj.Packages.core
    publish Workspace.Fsproj.Packages.javascript
    publish Workspace.Fsproj.Packages.newtonsoft
    publish Workspace.Fsproj.Packages.python
