#!node

import yargs from 'yargs';
import { hideBin } from 'yargs/helpers';
import shell from 'shelljs';
import chalk from 'chalk';
import concurrently from 'concurrently';
import releaseNpm from './scripts/release-npm.js';
import releaseNuget from './scripts/release-nuget.js';
import { simpleSpawn } from './scripts/await-spawn.js';

const info = chalk.blueBright
const warn = chalk.yellow
const error = chalk.red
const success = chalk.green
const log = console.log

// Crash script on error
shell.config.fatal = true;

const cleanHandler = async () => {
    log(info("Cleaning..."));
    shell.rm("-rf", "./src/bin");
    shell.rm("-rf", "./src/obj");

    shell.rm("-rf", "./tests/bin");
    shell.rm("-rf", "./tests/obj");
    shell.rm("-rf", "./tests/fableBuild");

    shell.rm("-rf", "./.nacara");
}

const testHandler = async (argv) => {
    await cleanHandler();

    // Make sure we have the tests/fableBuild folder so nodemon can watch it
    shell.mkdir("-p", "./tests/fableBuild");

    if (argv.watch) {
        concurrently([
            {
                command: "dotnet fable --watch --outDir fableBuild",
                cwd: "./tests",
                name: "Fable",
                prefixColor: "magenta"
            },
            // We use nodemon for watching the tests files
            // because mocha doesn't support watching ESM files yet
            {
                command: `npx nodemon \
--watch fableBuild \
--delay 150ms \
--exec "npx mocha fableBuild --reporter dot"
                `,
                cwd: "./tests",
                name: "Mocha",
                prefixColor: "cyan"
            }
        ])
    } else {

        await simpleSpawn(
            "dotnet fable --outDir fableBuild",
            "./tests"
        )

        await simpleSpawn(
            "npx mocha fableBuild",
            "./tests"
        )
    }
}

const documentationHandler = async (argv) => {
    await cleanHandler();

    await simpleSpawn("dotnet build", "./src/");

    if (argv.watch) {
        await simpleSpawn("npx nacara watch");

    } else {
        await simpleSpawn("npx nacara");
    }
}

const publishDocsHandler = async () => {
    await documentationHandler();

    await simpleSpawn("npx gh-pages -d docs_deploy");
}

const releaseHandler = async (argv) => {
    await testHandler(argv);

    await releaseNuget(".", "./src/Thoth.Json.fsproj");
}

yargs(hideBin(process.argv))
    .completion()
    .strict()
    .help()
    .alias("help", "h")
    .command(
        "clean",
        "Clean all build artifacts",
        () => { },
        cleanHandler
    )
    .command(
        "test",
        "Run all tests",
        (argv) => {
            argv
                .options(
                    "watch",
                    {
                        alias: "w",
                        describe: "Watch for file changes and re-run tests",
                        type: "boolean",
                        default: false
                    }
                )
        },
        testHandler
    )
    .command(
        "docs",
        "Build documentation",
        (argv) => {
            argv
                .options(
                    "watch",
                    {
                        alias: "w",
                        describe: "Watch for file changes and re-build documentation",
                        type: "boolean",
                        default: false
                    }
                )
        },
        documentationHandler
    )
    .command(
        "publish-docs",
        "Publish documentation to gh-pages",
        () => { },
        publishDocsHandler,
    )
    .command(
        "release",
        "Release a new version",
        () => { },
        releaseHandler
    )
    .version(false)
    .argv
