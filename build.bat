@echo off

set PYTHONIOENCODING=utf-8
dotnet tool restore
dotnet run --project build/EasyBuild.fsproj -- %*
