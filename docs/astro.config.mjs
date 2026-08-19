// @ts-check
import { dirname, resolve } from "node:path";
import { fileURLToPath } from "node:url";
import { defineConfig } from "astro/config";
import starlight from "@astrojs/starlight";
import starlightChangelogs, { makeChangelogsSidebarLinks } from "starlight-changelogs";
import starlightFSharpOracle from "starlight-fsharp-oracle";
import starlightVersions from "starlight-versions";
import fsharpLiterate from "starlight-fsharp-literate";
import { assemblyPath, packages } from "./packages.mjs";

const __dirname = dirname(fileURLToPath(import.meta.url));

// https://astro.build/config
export default defineConfig({
    site: "https://thoth-org.github.io",
    base: "/Thoth.Json/",
    server: { host: true },
    integrations: [
        // Turns the F# literate files into MDX pages inside `src/content/docs`.
        //
        // Must run before Starlight so the generated pages are visible to the content layer.
        //
        // The sources are kept outside of `src/content/docs` because `starlight-versions`
        // archives every file found there, and it parses them as MDX.
        fsharpLiterate({ mode: "mirror" }),
        starlight({
            title: "Thoth.Json",
            description: "JSON the simple and safe way",
            expressiveCode: {
                // "One Light" is the theme used by the previous version of the documentation
                themes: ["one-dark-pro", "one-light"]
            },
            customCss: ["./src/styles/custom.css", "./src/styles/home.css"],
            editLink: {
                baseUrl: "https://github.com/thoth-org/Thoth.Json/edit/main/docs/"
            },
            social: [
                {
                    icon: "github",
                    label: "GitHub",
                    href: "https://github.com/thoth-org/Thoth.Json"
                },
                {
                    icon: "twitter",
                    label: "Twitter",
                    href: "https://twitter.com/MangelMaxime"
                },
                {
                    icon: "gitter",
                    label: "Support",
                    href: "https://gitter.im/fable-compiler/Fable"
                },
                {
                    icon: "patreon",
                    label: "Donate",
                    href: "https://www.patreon.com/MangelMaxime"
                }
            ],
            plugins: [
                starlightChangelogs(),
                // Generates the API reference from the compiled assemblies.
                //
                // The pages are written to `src/pages/api` and the sidebar group is
                // appended by the plugin itself.
                starlightFSharpOracle({
                    assemblies: packages.map(({ name }) => resolve(__dirname, assemblyPath(name))),
                    output: "api",
                    sidebar: { label: "API Reference" }
                }),
                // Comes last so the versioned sidebars include the groups added by
                // the plugins above
                starlightVersions({
                    versions: [
                        {
                            // Documentation of the `Thoth.Json` package, kept around
                            // while the current version is rewritten for `Thoth.Json.Core`
                            slug: "legacy",
                            label: "Legacy"
                        }
                    ]
                })
            ],
            sidebar: [
                {
                    label: "Concept",
                    items: [
                        "concept/introduction",
                        "concept/decoder",
                        "concept/encoder",
                        "concept/cross-target-support"
                    ]
                },
                {
                    label: "Manual API",
                    items: [
                        "manual/introduction",
                        "manual/convention",
                        "manual/composition",
                        "manual/override-defaults",
                        "manual/json-representation"
                    ]
                },
                {
                    label: "Auto API",
                    items: [
                        "auto/introduction",
                        "auto/extra-coders",
                        "auto/caching",
                        "auto/json-representation"
                    ]
                },
                {
                    label: "Advanced",
                    items: ["advanced/introduction", "advanced/unknown-fields"]
                },
                {
                    label: "Changelogs",
                    items: makeChangelogsSidebarLinks(
                        packages.map(({ name, slug }) => ({
                            base: `changelogs/${slug}`,
                            label: name,
                            type: "all"
                        }))
                    )
                }
            ]
        })
    ]
});
