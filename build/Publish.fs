module Build.Publish

open System
open System.IO
open SimpleExec
open Build.Utils

let publish (projectDir: string) =
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


let handle (_args: string list) =
    Build.Test.JavaScript.handle []
    Build.Test.Newtonsoft.handle []
    Build.Test.Python.handle []

    publish Workspace.ProjectDir.Packages.legacy
    publish Workspace.ProjectDir.Packages.core
    publish Workspace.ProjectDir.Packages.javascript
    publish Workspace.ProjectDir.Packages.newtonsoft
    publish Workspace.ProjectDir.Packages.python
