// @ts-check
import { dirname, resolve } from "node:path";
import { fileURLToPath } from "node:url";
import { defineConfig } from "astro/config";
import starlight from "@astrojs/starlight";
import starlightChangelogs, { makeChangelogsSidebarLinks } from "starlight-changelogs";
import starlightFSharpOracle from "starlight-fsharp-oracle";
import fsharpLiterate from "starlight-fsharp-literate";
import { assemblyPath, packages } from "./packages.mjs";

const __dirname = dirname(fileURLToPath(import.meta.url));

// https://astro.build/config
export default defineConfig({
    site: "https://thoth-org.github.io",
    base: "/Thoth.Json/",
    server: { host: true },
    integrations: [
        // Turns the `*.source.fsx` files into MDX pages.
        //
        // Must run before Starlight so the generated pages are visible to the content layer.
        fsharpLiterate(),
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
                })
            ],
            sidebar: [
                {
                    label: "Concept",
                    items: [
                        "concept/introduction",
                        "concept/decoder",
                        "concept/encoder",
                        "concept/fable-and-dotnet-support"
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
