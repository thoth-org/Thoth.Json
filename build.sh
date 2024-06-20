#!/bin/sh -x

dotnet tool restore
dotnet run --project build/EasyBuild.fsproj -- $@
