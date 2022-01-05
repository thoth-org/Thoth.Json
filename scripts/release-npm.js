import shell from "shelljs"

import { release } from "./release-core.js"

export default async (baseDirectory) => {

    await release({
        baseDirectory: baseDirectory,
        projectFileName: "package.json",
        versionRegex: /(^\s*"version":\s*")(.+)(",\s*$)/gmi,
        publishFn: async () => {
            const publishResult =
                shell.exec(
                    "npm publish",
                    {
                        cwd: baseDirectory
                    }
                )

            // If published failed revert the file change
            if (publishResult.code !== 0) {
                throw "Npm publish failed"
            }
        }
    })

}
