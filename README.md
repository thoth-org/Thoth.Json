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
BenchmarkDotNet v0.14.0, Arch Linux
Intel Core i9-14900K, 1 CPU, 32 logical and 24 physical cores
.NET SDK 8.0.413
  [Host]     : .NET 8.0.19 (8.0.1925.36514), X64 RyuJIT AVX2 DEBUG
  DefaultJob : .NET 8.0.19 (8.0.1925.36514), X64 RyuJIT AVX2


| Method                      | Mean       | Error    | StdDev   | Ratio | RatioSD |
|---------------------------- |-----------:|---------:|---------:|------:|--------:|
| System.Text.Json            |   524.5 ns |  2.25 ns |  2.11 ns |  1.00 |    0.01 |
| Newtonsoft                  | 1,757.2 ns | 25.57 ns | 22.66 ns |  3.35 |    0.04 |
| Thoth.Json.System.Text.Json | 3,173.4 ns | 14.25 ns | 12.63 ns |  6.05 |    0.03 |
| Thoth.Json.Newtonsoft       | 5,094.4 ns | 32.62 ns | 28.92 ns |  9.71 |    0.07 |
```

## Blogs post

This is to keep track of different blog post that I refer to sometimes when thinking or helping people.

- [Introduction about Thoth.Json.Net](https://jordanmarr.github.io/fsharp/thoth-json-net-intro/)
- [Our journey to F#: JSON serialization with a mix of C# and F#](https://www.planetgeek.ch/2021/04/19/our-journey-to-f-json-serialization-with-a-mix-of-c-and-f/)

## Project structure

### Tests

For the tests, we use a shared project `Thoth.Json.Tests` that is referenced by the different runners. This is because we want each runner to only have the minimum amount of dependencies, and also if we include files from outside the `.fsproj` folder, then some generated files by Fable escape from the specify `outDir`.

Some of the tests require specific versions of Node.js, Python, etc. You can enter a shell with pinned versions available using `nix develop`.
