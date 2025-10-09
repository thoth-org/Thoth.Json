module EasyBuild.Commands.Benchmark

open Spectre.Console.Cli
open EasyBuild.Workspace
open SimpleExec

type BenchmarkDotNetSettings() =
    inherit CommandSettings()

type BenchmarkDotNetCommand() =
    inherit Command<BenchmarkDotNetSettings>()
    interface ICommandLimiter<BenchmarkDotNetSettings>

    override _.Execute
        (context: CommandContext, settings: BenchmarkDotNetSettings)
        =
        Command.Run(
            "dotnet",
            "run -c Release",
            workingDirectory = Workspace.benchmarks.dotnet.``.``
        )

        0
