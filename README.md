# Thoth.Json [![Build Status](https://dev.azure.com/thoth-org/Thoth.Json/_apis/build/status/thoth-org.Thoth.Json?branchName=master)](https://dev.azure.com/thoth-org/Thoth.Json/_build/latest?definitionId=1&branchName=master)

| Stable | Prerelease
--- | ---
[![NuGet Badge](https://buildstats.info/nuget/Thoth.Json)](https://www.nuget.org/packages/Thoth.Json/) | [![NuGet Badge](https://buildstats.info/nuget/Thoth.Json?includePreReleases=true)](https://www.nuget.org/packages/Thoth.Json/)

## Blogs post

This is to keep track of different blog post that I refer to sometimes when thinking or helping people.

- [Introduction about Thoth.Json.Net](https://jordanmarr.github.io/fsharp/thoth-json-net-intro/)
- [Our journey to F#: JSON serialization with a mix of C# and F#](https://www.planetgeek.ch/2021/04/19/our-journey-to-f-json-serialization-with-a-mix-of-c-and-f/)

## Project structure

### Tests

For the tests, we use a shared project `Thoth.Json.Tests` that is referenced by the different runners. This is because we want each runner to only have the minimum amount of dependencies, and also if we include files from outside the `.fsproj` folder, then some generated files by Fable escape from the specify `outDir`.
