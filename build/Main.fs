module Build.Main

// This is a basic help message, as the CLI parser is not a "real" CLI parser
// For now, it is enough as this is just a dev tool
let printHelp () =
    let helpText =
        """
Usage: dotnet run <command> [<args>]

Available commands:
    fable-library

        Options:
            --javascript            Build fable-library for JavaScript
            --typescript            Build fable-library for TypeScript
            --python                Build fable-library for Python
            --dart                  Build fable-library for Dart
            --rust                  Build fable-library for Rust

    quicktest                       Watch for changes and re-run the quicktest
                                    This is useful to work on a feature in an isolated
                                    manner to avoid all the noise coming from the main tests

        Subcommands:
            javascript              Run for JavaScript
            typescript              Run for TypeScript
            python                  Run for Python
            dart                    Run for Dart
            rust                    Run for Rust

        Options:
            --skip-fable-library    Skip building fable-library if folder already exists

    test                            Run the main tests suite
        Subcommands:
            javascript              Run the tests for JavaScript
            typescript              Run the tests for TypeScript
            python                  Run the tests for Python
            dart                    Run the tests for Dart
            rust                    Run the tests for Rust
            integration             Run the integration test suite
            standalone              Tests the standalone version of Fable
                                    (Fable running on top of Node.js)

        Options for all except integration and standalone:
            --watch                 Watch for changes and re-run the tests
            --skip-fable-library    Skip building fable-library if folder already exists
            --no-dotnet             When in watch mode, do not run the .NET tests

        Options for JavaScript:
            --reat-only             Run only the tests for React (can be run in watch mode)

        Options for Rust:
            --ast-only              Run only the tests for the AST (can be run in watch mode)
            --no_std                Compile and run the tests without the standard library
            --threaded              Compile and run the tests with the threaded runtime

    standalone                      Compile standalone + worker version of Fable running
                                    on top of of Node.js

        Options:
            --skip-fable-library    Skip building fable-library if folder already exists
            --no-minify             Don't minify the JavaScript output
            --watch                 Watch for changes and recompile

    worker-js                       Compile the worker for the standalone version of Fable

        Options:
            --skip-fable-library    Skip building fable-library if folder already exists
            --no-minify             Don't minify the JavaScript output

    compiler-js                     Compile the Fable compiler to JavaScript

        Options:
            --skip-fable-library    Skip building fable-library if folder already exists
            --no-minify             Don't minify the JavaScript output

    package                         Generate local package for Fable.Cli and Fable.Core
                                    allowing to use this local package for testing
                                    inside of other projects

        Options:
            --skip-fable-library    Skip building fable-library if folder already exists

    publish                         Publish the different packages to NuGet and NPM
                                    based on the CHANGELOG.md files
                                    If the last version in the CHANGELOG.md is
                                    different from the version in the packages,
                                    the package will be published

    github-release                  Create a GitHub release based on the CHANGELOG.md
                                    file and the version in the package.json
                                    This will also invoke the publish command
        """

    printfn "%s" helpText

[<EntryPoint>]
let main argv =
    let argv = argv |> Array.map (fun x -> x.ToLower()) |> Array.toList

    match argv with
    | "test" :: args ->
        match args with
        | "javascript" :: args -> Test.JavaScript.handle args
        | "newtonsoft" :: args -> Test.Newtonsoft.handle args
        | "python" :: args -> Test.Python.handle args
        | []
        | "all" :: _ ->
            Test.JavaScript.handle []
            Test.Newtonsoft.handle []
        | _ -> printHelp ()
    // | "publish" :: args -> Publish.handle args
    // | "github-release" :: args -> GithubRelease.handle args
    // | "package" :: args -> Package.handle args
    | "help" :: _
    | "--help" :: _
    | _ -> printHelp ()

    0
