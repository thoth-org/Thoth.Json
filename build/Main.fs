module Build.Main

open SimpleExec

// This is a basic help message, as the CLI parser is not a "real" CLI parser
// For now, it is enough as this is just a dev tool
let printHelp () =
    let helpText =
        """
Usage: dotnet run <command> [<args>]

Available commands:
    test                            Run the main tests suite
        Subcommands:
            legacy                  Run the tests for the legacy version
            javascript              Run the tests for JavaScript
            newtonsoft              Run the tests for Newtonsoft
            python                  Run the tests for Python

        Options for all except integration and standalone:
            --watch                 Watch for changes and re-run the tests

    publish                         Publish the different packages to NuGet and NPM
                                    based on the CHANGELOG.md files
                                    If the last version in the CHANGELOG.md is
                                    different from the version in the packages,
                                    the package will be published
        """

    printfn $"%s{helpText}"

[<EntryPoint>]
let main argv =
    let argv = argv |> Array.map (fun x -> x.ToLower()) |> Array.toList

    Command.Run("dotnet", "husky install")

    match argv with
    | "test" :: args ->
        match args with
        | "legacy" :: args -> Test.Legacy.handle args
        | "javascript" :: args -> Test.JavaScript.handle args
        | "newtonsoft" :: args -> Test.Newtonsoft.handle args
        | "python" :: args -> Test.Python.handle args
        | []
        | "all" :: _ ->
            Test.JavaScript.handle []
            Test.Newtonsoft.handle []
            Test.Python.handle []
        | _ -> printHelp ()
    | "publish" :: args -> Publish.handle args
    | "help" :: _
    | "--help" :: _
    | _ -> printHelp ()

    0
