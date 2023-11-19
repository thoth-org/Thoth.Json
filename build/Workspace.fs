module Build.Workspace

open System.IO
open Build.Utils

let root = Path.Resolve()

module ProjectDir =

    module Packages =

        let newtonsoft = Path.Resolve("packages", "Thoth.Json.Newtonsoft")

        let javascript = Path.Resolve("packages", "Thoth.Json.JavaScript")

        let core = Path.Resolve("packages", "Thoth.Json.Core")

    module Tests =


        let javascript = Path.Resolve("tests", "Thoth.Json.Tests.JavaScript")
        let newtonsoft = Path.Resolve("tests", "Thoth.Json.Tests.Newtonsoft")

module Fsproj =

    module Packages =

        let newtonsoft =
            Path.Combine(
                ProjectDir.Packages.newtonsoft,
                "Thoth.Json.Newtonsoft.fsproj"
            )

        let javascript =
            Path.Combine(
                ProjectDir.Packages.javascript,
                "Thoth.Json.JavaScript.fsproj"
            )

        let core =
            Path.Combine(ProjectDir.Packages.core, "Thoth.Json.Core.fsproj")

    module Tests =

        let javascript =
            Path.Combine(
                ProjectDir.Tests.javascript,
                "Thoth.Json.Tests.JavaScript.fsproj"
            )

        let newtonsoft =
            Path.Combine(
                ProjectDir.Tests.newtonsoft,
                "Thoth.Json.Tests.Newtonsoft.fsproj"
            )
