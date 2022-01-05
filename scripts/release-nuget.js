import shell from "shelljs"
import path from "node:path"

import { release } from "./release-core.js"

const getEnvVariable = function (varName) {
    const value = process.env[varName];
    if (value === undefined) {
        log(chalk.red(`Missing environnement variable ${varName}`))
        process.exit(1)
    } else {
        return value;
    }
}

export default async (baseDirectory, relativePathToFsproj) => {

    const NUGET_KEY = getEnvVariable("NUGET_KEY")
    const fullPathToFsproj = path.resolve(baseDirectory, relativePathToFsproj)
    const fsprojDirectory = path.dirname(fullPathToFsproj)
    const projectName = path.basename(fullPathToFsproj, ".fsproj")

    await release({
        baseDirectory: baseDirectory,
        projectFileName: relativePathToFsproj,
        versionRegex: /(^\s*<Version>)(.*)(<\/Version>\s*$)/gmi,
        publishFn: async (versionInfo) => {
            const packResult =
                shell.exec(
                    "dotnet pack -c Release",
                    {
                        cwd: fsprojDirectory
                    }
                )

            if (packResult.code !== 0) {
                throw "Dotnet pack failed"
            }

            const pushNugetResult =
                shell.exec(
                    `dotnet nuget push bin/Release/${projectName}.${versionInfo.version}.nupkg -s nuget.org -k ${NUGET_KEY}`,
                    {
                        cwd: fsprojDirectory
                    }
                )

            if (pushNugetResult.code !== 0) {
                throw "Dotnet push failed"
            }
        }
    })
}
