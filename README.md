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


| Method                      | Mean       | Error     | StdDev   | Ratio | RatioSD |
|---------------------------- |-----------:|----------:|---------:|------:|--------:|
| Thoth.Json.Newtonsoft       | 6,359.6 ns | 102.35 ns | 95.74 ns | 10.40 |    0.19 |
| Thoth.Json.System.Text.Json | 4,936.8 ns |  45.64 ns | 38.11 ns |  8.07 |    0.11 |
| Newtonsoft                  | 1,497.8 ns |  24.72 ns | 23.13 ns |  2.45 |    0.05 |
| System.Text.Json            |   611.7 ns |   8.62 ns |  7.20 ns |  1.00 |    0.02 |
```

## Blogs post

This is to keep track of different blog post that I refer to sometimes when thinking or helping people.

- [Introduction about Thoth.Json.Net](https://jordanmarr.github.io/fsharp/thoth-json-net-intro/)
- [Our journey to F#: JSON serialization with a mix of C# and F#](https://www.planetgeek.ch/2021/04/19/our-journey-to-f-json-serialization-with-a-mix-of-c-and-f/)

## Project structure

### Tests

For the tests, we use a shared project `Thoth.Json.Tests` that is referenced by the different runners. This is because we want each runner to only have the minimum amount of dependencies, and also if we include files from outside the `.fsproj` folder, then some generated files by Fable escape from the specify `outDir`.
