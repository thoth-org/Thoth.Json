module Docs.Site

open System.IO
open System.Reflection
open Feliz.ViewEngine
open Nacara.Core
open Nacara.Plugins
open Nacara.Theme

let baseUrl = "/Thoth.Json/"

/// The public API of the Thoth.Json packages, read from the assemblies this site is built with.
let apiOptions =
    { FSharpApi.defaults with
        Root = "reference"
        Title = "API reference"
        Sources =
            [
                let beside =
                    Assembly.GetExecutingAssembly().Location
                    |> Path.GetDirectoryName

                for name in
                    [
                        "Thoth.Json.Core"
                        "Thoth.Json.Core.Auto"
                        "Thoth.Json.JavaScript"
                        "Thoth.Json.Newtonsoft"
                        "Thoth.Json.Python"
                        "Thoth.Json.System.Text.Json"
                    ] ->
                    FSharpApiSource.create (
                        Path.Combine(beside, $"%s{name}.dll")
                    )
            ]
    }

let versions =
    [
        SiteVersion.root "main"
        SiteVersion.create "Legacy" "legacy"
    ]

let changelogs =
    [
        ChangelogSource.create
            "Thoth.Json"
            "../packages/Thoth.Json/CHANGELOG.md"
        ChangelogSource.create
            "Thoth.Json.Core"
            "../packages/Thoth.Json.Core/CHANGELOG.md"
        ChangelogSource.create
            "Thoth.Json.JavaScript"
            "../packages/Thoth.Json.JavaScript/CHANGELOG.md"
        ChangelogSource.create
            "Thoth.Json.Newtonsoft"
            "../packages/Thoth.Json.Newtonsoft/CHANGELOG.md"
        ChangelogSource.create
            "Thoth.Json.Python"
            "../packages/Thoth.Json.Python/CHANGELOG.md"
    ]

let private footerLink (label: string) (icon: string) (url: string) =
    Html.li
        [
            Html.a
                [
                    prop.className "site-footer__link"
                    prop.href url
                    prop.children
                        [
                            Html.span
                                [
                                    prop.className "site-footer__icon"
                                    prop.dangerouslySetInnerHTML icon
                                ]
                            Html.span [ prop.text label ]
                        ]
                ]
        ]

let private footerSection (title: string) (links: ReactElement list) =
    Html.div
        [
            prop.className "site-footer__section"
            prop.children
                [
                    Html.p
                        [
                            prop.className "site-footer__title"
                            prop.text title
                        ]
                    Html.ul
                        [
                            prop.className "site-footer__list"
                            prop.children links
                        ]
                ]
        ]

let private footer =
    Html.div
        [
            prop.className "site-footer"
            prop.children
                [
                    Html.div
                        [
                            prop.className "site-footer__sitemap"
                            prop.children
                                [
                                    footerSection
                                        "Project ressources"
                                        [
                                            footerLink
                                                "Repository"
                                                FooterIcons.fileCode
                                                "https://github.com/thoth-org/Thoth.Json/"
                                            footerLink
                                                "Changelog"
                                                FooterIcons.list
                                                "https://thoth-org.github.io/Thoth.Json/changelogs/thoth-json/"
                                            footerLink
                                                "License"
                                                FooterIcons.idCard
                                                "https://github.com/thoth-org/Thoth.Json/blob/main/LICENSE.txt"
                                        ]
                                    footerSection
                                        "Other Links"
                                        [
                                            footerLink
                                                "Fable"
                                                FooterIcons.fable
                                                "https://fable.io"
                                            footerLink
                                                "Fable Gitter"
                                                FooterIcons.gitter
                                                "https://gitter.im/fable-compiler/Fable"
                                            footerLink
                                                "F# Slack"
                                                FooterIcons.slack
                                                "https://fsharp.org/guides/slack/"
                                            footerLink
                                                "F# Software Foundation"
                                                FooterIcons.fsharpOrg
                                                "https://fsharp.org/"
                                            footerLink
                                                "Twitter"
                                                FooterIcons.twitter
                                                "https://twitter.com/MangelMaxime"
                                        ]
                                ]
                        ]
                    Html.p
                        [
                            Html.text "Built with "
                            Html.a
                                [
                                    prop.href
                                        "https://mangelmaxime.github.io/Nacara/"
                                    prop.text "Nacara"
                                ]
                        ]
                    Html.p
                        [
                            Html.text "Copyright © 2021-present "
                            Html.a
                                [
                                    prop.href "https://twitter.com/MangelMaxime"
                                    prop.text "Maxime Mangel"
                                ]
                            Html.text " and contributors."
                        ]
                ]
        ]

let theme =
    Theme.defaults
    |> Theme.navbar
        [
            NavbarSection(
                "Docs",
                "documentation",
                "/documentation/introduction/"
            )
            NavbarSection("Reference", "reference", "/reference/")
            NavbarSection("Changelogs", "changelogs", "/changelogs/thoth-json/")
            NavbarLink("Support", "https://gitter.im/fable-compiler/Fable")
            NavbarLink("Donate", "https://www.patreon.com/MangelMaxime")
        ]
    |> Theme.navbarEnd
        [
            NavbarDynamicWidget Search.trigger
            NavbarDynamicWidget(
                Versions.switcher (Versions.versions versions Versions.defaults)
            )
            NavbarIcon(
                "GitHub",
                "https://github.com/thoth-org/Thoth.Json",
                Icons.github
            )
        ]
    |> Theme.menu
        "documentation"
        [
            Menu.section
                "Getting started"
                [
                    Menu.page "documentation/introduction.md"
                    Menu.page "documentation/installation.md"
                ]
            Menu.section
                "Concept"
                [
                    Menu.page "documentation/concept/decoder.md"
                    Menu.page "documentation/concept/encoder.md"
                    Menu.page "documentation/concept/codec.md"
                ]
            Menu.section
                "Manual API"
                [
                    Menu.page "documentation/manual/introduction.md"
                    Menu.page "documentation/manual/composition.md"
                    Menu.page "documentation/manual/convention.md"
                    Menu.page "documentation/manual/override-defaults.md"
                    Menu.page "documentation/manual/json-representation.md"
                ]
            Menu.section
                "Codec API"
                [
                    Menu.page "documentation/codec/introduction.md"
                    Menu.page "documentation/codec/objects.md"
                    Menu.page "documentation/codec/unions.md"
                    Menu.page "documentation/codec/recursion.md"
                ]
            Menu.section
                "Auto API"
                [
                    Menu.page "documentation/auto/introduction.md"
                    Menu.page "documentation/auto/configuration.md"
                    Menu.page "documentation/auto/codecs.md"
                    Menu.page "documentation/auto/extra-coders.md"
                    Menu.page "documentation/auto/caching.md"
                    Menu.page "documentation/auto/json-representation.md"
                ]
            Menu.section
                "Advanced"
                [
                    Menu.page "documentation/advanced/introduction.md"
                    Menu.page "documentation/advanced/unknown-fields.md"
                    Menu.page "documentation/advanced/custom-runtime.md"
                ]
        ]
    |> Theme.editUrl "https://github.com/thoth-org/Thoth.Json/edit/main/docs"
    |> Theme.footer footer

let reference =
    FSharpApi.collection "reference" DocFrontMatter.decoder apiOptions
    |> Collection.title _.Title
    |> Collection.layout (Theme.layout theme)

let changelog =
    Changelog.collection "changelogs" DocFrontMatter.decoder changelogs
    |> Collection.title _.Title
    |> Collection.routePrefix "changelogs"
    |> Collection.layout (Theme.layout theme)

let site =
    Site.create "Thoth.Json"
    |> Site.description "JSON the simple and safe way"
    |> Site.baseUrl baseUrl
    |> Site.origin "https://thoth-org.github.io"
    |> Site.noStaticFiles
    |> Site.stylesheet "assets/custom.css"
    |> Markdown.register
    |> TreeSitter.register
    |> Changelog.registerWith "changelogs" changelogs
    |> FSharpApi.register apiOptions
    |> LiveExample.registerWith (
        LiveExample.preset (
            LiveExamplePreset.create "thoth"
            |> LiveExamplePreset.project "snippets/Snippets.fsproj"
            |> LiveExamplePreset.asDefault
        )
        >> LiveExample.highlighting
            LiveExampleHighlighting.TreeSitterHighlighting
    )
    |> Search.register
    |> Sitemap.register
    |> Versions.register versions
    |> LinkValidator.registerWith (
        LinkValidator.checkExternal (
            System.Environment.GetEnvironmentVariable "NACARA_CHECK_LINKS" = "1"
        )
    )
    |> LightningCss.register
    |> Nuglify.minifyHtml
    |> GitHubPages.register
    |> Theme.register theme
    |> Site.collection (Theme.docs theme "content")
    |> Site.collection reference
    |> Site.collection changelog

[<EntryPoint>]
let main argv = Nacara.run site argv
