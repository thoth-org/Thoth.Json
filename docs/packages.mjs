/**
 * Packages published from this repository.
 *
 * This list drives both the API reference (generated from the compiled assemblies)
 * and the changelog pages (generated from the `CHANGELOG.md` files).
 *
 * @type {{ name: string, slug: string }[]}
 */
export const packages = [
    // { name: "Thoth.Json.Core", slug: "thoth-json-core" },
    // { name: "Thoth.Json.Core.Auto", slug: "thoth-json-core-auto" },
    // { name: "Thoth.Json.JavaScript", slug: "thoth-json-javascript" },
    // { name: "Thoth.Json.Newtonsoft", slug: "thoth-json-newtonsoft" },
    // { name: "Thoth.Json.Python", slug: "thoth-json-python" },
    // { name: "Thoth.Json.System.Text.Json", slug: "thoth-json-system-text-json" },
    { name: "Thoth.Json", slug: "thoth-json" }
];

/** Path of a package `CHANGELOG.md`, relative to the root of the Astro project. */
export const changelogPath = (/** @type {string} */ name) => `../packages/${name}/CHANGELOG.md`;

/** Path of a package assembly, relative to the root of the Astro project. */
export const assemblyPath = (/** @type {string} */ name) =>
    `../packages/${name}/bin/Debug/netstandard2.0/${name}.dll`;
