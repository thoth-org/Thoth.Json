import path from "node:path"
import fs from "node:fs"
import chalk from "chalk"
import parseChangelog from "changelog-parser"

const log = console.log

/**
 * @typedef Options
 * @type {object}
 * @property {string} baseDirectory - Path to the project directory the project file and the CHANGELOG.md should be at the root of this repository
 * @property {string} projectFileName - Name of the project file when we need to check/change the version information
 * @property {RegExp} versionRegex - Regex used to detect the current version it should have 3 groups: 1. Text before the version - 2. The version - 3. Rest after the function
 * @property {function} publishFn - The function to execute in order to publish the package
 */

/**
 *
 * @param {Options} options
 */
export const release = async (options) => {

    // checks if the package.json and CHANGELOG exist
    const changelogPath = path.resolve(options.baseDirectory, "CHANGELOG.md")
    const packageJsonPath = path.resolve(options.baseDirectory, options.projectFileName)

    if (!fs.existsSync(changelogPath)) {
        log(chalk.red(`CHANGELOG.md not found in ${options.baseDirectory}`))
    }

    if (!fs.existsSync(packageJsonPath)) {
        log(chalk.red(`${options.projectFileName} not found in ${options.baseDirectory}`))
    }

    // read files content
    const changelogContent = fs.readFileSync(changelogPath).toString().replace("\r\n", "\n")
    const packageJsonContent = fs.readFileSync(packageJsonPath).toString()

    const changelog = await parseChangelog({ text: changelogContent })

    let versionInfo = undefined;

    // Find the first version which is not Unreleased
    for (const version of changelog.versions) {
        if (version.title.toLowerCase() !== "unreleased") {
            versionInfo = version;
            break;
        }
    }

    if (versionInfo === undefined) {
        log(chalk.red(`No version ready to be released found in the CHANGELOG.md`))
        process.exit(1)
    }

    const m = options.versionRegex.exec(packageJsonContent)

    if (m === null) {
        log(chalk.red(`No version property found in the ${options.projectFileName}`))
        process.exit(1)
    }

    const lastPublishedVersion = m[2];

    if (versionInfo.version === lastPublishedVersion) {
        log(chalk.blue(`Last version has already been published. Skipping...`))
        return;
    }

    log(chalk.blue("New version detected"))

    const newPackageJsonContent = packageJsonContent.replace(options.versionRegex, `$1${versionInfo.version}$3`)

    // Update fsproj file on the disk
    fs.writeFileSync(packageJsonPath, newPackageJsonContent)

    try {
        await options.publishFn(versionInfo)
    } catch (e) {
        fs.writeFileSync(packageJsonPath, packageJsonContent)
        log(chalk.red("An error occured while publishing the new package"))
        log(chalk.blue("The files have been reverted to their original state"))
        process.exit(1)
    }

    log(chalk.green(`Package published`))
}
