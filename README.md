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
BenchmarkDotNet v0.15.8, Linux Arch Linux
Intel Core i9-14900K 0.80GHz, 1 CPU, 32 logical and 24 physical cores
.NET SDK 10.0.300
  [Host]     : .NET 10.0.8 (10.0.8, 10.0.826.23019), X64 RyuJIT x86-64-v3 DEBUG
  DefaultJob : .NET 10.0.8 (10.0.8, 10.0.826.23019), X64 RyuJIT x86-64-v3


| Method                      | Mean       | Error    | StdDev   | Ratio | RatioSD |
|---------------------------- |-----------:|---------:|---------:|------:|--------:|
| System.Text.Json            |   423.4 ns |  1.68 ns |  1.57 ns |  1.00 |    0.01 |
| Newtonsoft                  |   790.8 ns |  4.27 ns |  4.00 ns |  1.87 |    0.01 |
| Thoth.Json.System.Text.Json | 1,524.5 ns | 12.77 ns | 11.95 ns |  3.60 |    0.03 |
| Thoth.Json.Newtonsoft       | 2,059.9 ns |  8.62 ns |  7.64 ns |  4.87 |    0.02 |
```

## Blogs post

This is to keep track of different blog post that I refer to sometimes when thinking or helping people.

- [Introduction about Thoth.Json.Net](https://jordanmarr.github.io/fsharp/thoth-json-net-intro/)
- [Our journey to F#: JSON serialization with a mix of C# and F#](https://www.planetgeek.ch/2021/04/19/our-journey-to-f-json-serialization-with-a-mix-of-c-and-f/)

## Project structure

### Tests

For the tests, we use a shared project `Thoth.Json.Tests` that is referenced by the different runners. This is because we want each runner to only have the minimum amount of dependencies, and also if we include files from outside the `.fsproj` folder, then some generated files by Fable escape from the specify `outDir`.
