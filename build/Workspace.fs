module EasyBuild.Workspace

open System.IO
open Build.Utils.Path
open EasyBuild.FileSystemProvider

[<Literal>]
let ROOT = __SOURCE_DIRECTORY__ + "/.."

type Workspace = RelativeFileSystem<ROOT>

type VirtualWorkspace =
    VirtualFileSystem<ROOT, """
tests
    Thoth.Json.Tests.JavaScript
        fableBuild/
""">
