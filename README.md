# Thoth.Json [![Build Status](https://dev.azure.com/thoth-org/Thoth.Json/_apis/build/status/thoth-org.Thoth.Json?branchName=master)](https://dev.azure.com/thoth-org/Thoth.Json/_build/latest?definitionId=1&branchName=master)

| Stable | Prerelease
--- | ---
[![NuGet Badge](https://buildstats.info/nuget/Thoth.Json)](https://www.nuget.org/packages/Thoth.Json/) | [![NuGet Badge](https://buildstats.info/nuget/Thoth.Json?includePreReleases=true)](https://www.nuget.org/packages/Thoth.Json/)

## Benchmark

**Remark**

We are comparing the performance of Thoth.Json with standard JSON libraries like Newtonsoft.Json and System.Text.Json.

Thoth.Json is expected to be slower than these libraries because it uses them internally.

It is also important to note that Thoth.Json solve a different problem than these libraries. It aims to provide a cross-platform API for .NET, JavaScript, Python, etc. with a focus on the developer experience and type safety.

For most of the use cases, Thoth.Json should be fast enough, not everyone needs to parse JSON at the speed of light.

```text
BenchmarkDotNet v0.14.0, macOS Sequoia 15.0.1 (24A348) [Darwin 24.0.0]
Apple M1 Max, 1 CPU, 10 logical and 10 physical cores
.NET SDK 8.0.401
  [Host]     : .NET 8.0.8 (8.0.824.36612), Arm64 RyuJIT AdvSIMD DEBUG
  DefaultJob : .NET 8.0.8 (8.0.824.36612), Arm64 RyuJIT AdvSIMD


| Method                      | Mean       | Error    | StdDev   | Ratio | RatioSD |
|---------------------------- |-----------:|---------:|---------:|------:|--------:|
| System.Text.Json            |   499.1 ns |  1.03 ns |  0.96 ns |  1.00 |    0.00 |
| Newtonsoft                  | 1,553.4 ns |  4.08 ns |  3.81 ns |  3.11 |    0.01 |
| Thoth.Json.System.Text.Json | 4,330.8 ns | 20.67 ns | 19.33 ns |  8.68 |    0.04 |
| Thoth.Json.Newtonsoft       | 5,783.3 ns | 57.05 ns | 50.57 ns | 11.59 |    0.10 |
```

## Blogs post

This is to keep track of different blog post that I refer to sometimes when thinking or helping people.

- [Introduction about Thoth.Json.Net](https://jordanmarr.github.io/fsharp/thoth-json-net-intro/)
- [Our journey to F#: JSON serialization with a mix of C# and F#](https://www.planetgeek.ch/2021/04/19/our-journey-to-f-json-serialization-with-a-mix-of-c-and-f/)

## Project structure

### Tests

For the tests, we use a shared project `Thoth.Json.Tests` that is referenced by the different runners. This is because we want each runner to only have the minimum amount of dependencies, and also if we include files from outside the `.fsproj` folder, then some generated files by Fable escape from the specify `outDir`.
