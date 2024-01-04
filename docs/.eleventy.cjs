const eleventyLayoutNacara = require("@mangelmaxime/eleventy-layout-nacara");
const eleventyFsharpLiterate = require("@mangelmaxime/eleventy-plugin-fsharp");
const syntaxHighlight = require("@11ty/eleventy-plugin-syntaxhighlight");
const loadLanguages = require("prismjs/components/");

loadLanguages("fsharp");

/** @param {import("@11ty/eleventy/src/UserConfig")} eleventyConfig */
module.exports = function (eleventyConfig) {

    // Add the layout plugin
    eleventyConfig.addPlugin(eleventyLayoutNacara);
    eleventyConfig.addPlugin(syntaxHighlight, {
        init: function ({ Prism }) {
            Prism.languages.fs = Prism.languages.fsharp;
            // Prism.languages.njk = Prism.languages.twig;
            // The line above does not work, so we have to do use `twig` directly
            // the markdown files...
        }
    });
    eleventyConfig.addPlugin(eleventyFsharpLiterate);

    eleventyConfig.addWatchTarget("./../packages/Thoth.Json/CHANGELOG.md");
    eleventyConfig.addWatchTarget("./../packages/Thoth.Json.Core/CHANGELOG.md");
    eleventyConfig.addWatchTarget("./../packages/Thoth.Json.JavaScript/CHANGELOG.md");
    eleventyConfig.addWatchTarget("./../packages/Thoth.Json.Newtonsoft/CHANGELOG.md");
    eleventyConfig.addWatchTarget("./../packages/Thoth.Json.Python/CHANGELOG.md");

    // Configure Eleventy options to your liking
    return {
        dir: {
            input: ".",
            includes: "_includes",
            data: "_data",
            output: "_site",
        },
        dataTemplateEngine: "njk",
        htmlTemplateEngine: "njk",
        markdownTemplateEngine: "njk",
        templateEngineOverride: "njk",
    };
};
