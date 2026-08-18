import { defineCollection } from "astro:content";
import { docsLoader } from "@astrojs/starlight/loaders";
import { docsSchema } from "@astrojs/starlight/schema";
import { changelogsLoader } from "starlight-changelogs/loader";
import { docsVersionsLoader } from "starlight-versions/loader";
import { changelogPath, packages } from "../packages.mjs";

/**
 * Keeps the released versions only and strips the release date from their title.
 *
 * The `CHANGELOG.md` files start with a YAML front matter block used by
 * EasyBuild.ShipIt. The changelog parser doesn't know about front matter, so
 * without this filter its content would be reported as a version.
 */
const versionTitle = ({ title }: { title: string }) =>
    /^(?<version>\d+\.\d+\.\d+\S*) - \d{4}-\d{2}-\d{2}$/.exec(title.trim())?.groups?.["version"];

export const collections = {
    docs: defineCollection({ loader: docsLoader(), schema: docsSchema() }),
    versions: defineCollection({ loader: docsVersionsLoader() }),
    changelogs: defineCollection({
        loader: changelogsLoader(
            packages.map(({ name, slug }) => ({
                provider: "keep-a-changelog" as const,
                base: `changelogs/${slug}`,
                changelog: changelogPath(name),
                title: name,
                process: versionTitle
            }))
        )
    })
};
