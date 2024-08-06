module EasyBuild.Main

open SimpleExec
open Spectre.Console.Cli
open EasyBuild.Commands.Test
open EasyBuild.Commands.Publish

[<EntryPoint>]
let main args =
    Command.Run("dotnet", "husky install")

    let app = CommandApp()

    app.Configure(fun config ->
        config.AddBranch<TestSettings>(
            "test",
            fun test ->
                test.SetDescription("Run the main tests suite")

                test.SetDefaultCommand<TestCommand>()

                test
                    .AddCommand<TestJavaScriptCommand>("javascript")
                    .WithDescription("Run the tests for JavaScript")
                |> ignore

                test
                    .AddCommand<TestNewtonsoftCommand>("newtonsoft")
                    .WithDescription("Run the tests for Newtonsoft")
                |> ignore

                test
                    .AddCommand<TestPythonCommand>("python")
                    .WithDescription("Run the tests for Python")
                |> ignore

                test
                    .AddCommand<TestTypeScriptCommand>("typescript")
                    .WithDescription("Run the tests for TypeScript")
                |> ignore

                test
                    .AddCommand<TestLegacyCommand>("legacy")
                    .WithDescription("Run the tests for the legacy version")
                |> ignore

        )
        |> ignore

        config
            .AddCommand<PublishCommand>("publish")
            .WithDescription(
                """Publish the different packages to NuGet and NPM based on the CHANGELOG.md files

If the last version in the CHANGELOG.md is different from the version in the packages, the package will be published"""
            )
        |> ignore
    )

    app.Run(args)
