# Thoth.Json [![Build Status](https://dev.azure.com/thoth-org/Thoth.Json/_apis/build/status/thoth-org.Thoth.Json?branchName=master)](https://dev.azure.com/thoth-org/Thoth.Json/_build/latest?definitionId=1&branchName=master)

| Stable                                                                                                 | Prerelease                                                                                                                     |
| ------------------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------------------------------------------------------------ |
| [![NuGet Badge](https://buildstats.info/nuget/Thoth.Json)](https://www.nuget.org/packages/Thoth.Json/) | [![NuGet Badge](https://buildstats.info/nuget/Thoth.Json?includePreReleases=true)](https://www.nuget.org/packages/Thoth.Json/) |

## Building and Testing

| Command                                              | Comment                                                         |
| ---------------------------------------------------- | --------------------------------------------------------------- |
| `./fake.sh build`                                    | Build and run tests. Note that this does not have a watch mode. |
| `./fake.sh build -t WatchDocs`                       | Watch and serve documentation                                   |
| `yarn fable-splitter -c tests/splitter.config.js -w` |                                                                 |
| `yarn run mocha tests/bin`                           |                                                                 |
