module Docs.Site

open Feliz.ViewEngine
open Nacara.Core
open Nacara.Plugins
open Nacara.Theme

let baseUrl = "/Thoth.Json/"

/// <summary>The version this branch builds. Every url it writes sits under it.</summary>
let version = "legacy"

let versions =
    [
        SiteVersion.root "main"
        SiteVersion.create version version
    ]

// The theme writes navbar and footer urls verbatim.
let private url (path: string) = baseUrl + version + "/" + path

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
                                                (url "changelogs/thoth-json/")
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
                url "documentation/concept/introduction/"
            )
            NavbarSection(
                "Changelogs",
                "changelogs",
                url "changelogs/thoth-json/"
            )
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
                "Concept"
                [
                    Menu.page "documentation/concept/introduction.md"
                    Menu.page "documentation/concept/decoder.fsx"
                    Menu.page "documentation/concept/encoder.fsx"
                    Menu.page
                        "documentation/concept/fable-and-dotnet-support.md"
                ]
            Menu.section
                "Manual API"
                [
                    Menu.page "documentation/manual/introduction.fsx"
                    Menu.page "documentation/manual/convention.fsx"
                    Menu.page "documentation/manual/composition.fsx"
                    Menu.page "documentation/manual/override-defaults.fsx"
                    Menu.page "documentation/manual/json-representation.fsx"
                ]
            Menu.section
                "Auto API"
                [
                    Menu.page "documentation/auto/introduction.fsx"
                    Menu.page "documentation/auto/extra-coders.fsx"
                    Menu.page "documentation/auto/caching.fsx"
                    Menu.page "documentation/auto/json-representation.fsx"
                ]
            Menu.section
                "Advanced"
                [
                    Menu.page "documentation/advanced/introduction.md"
                    Menu.page "documentation/advanced/unknown-fields.fsx"
                ]
        ]
    |> Theme.editUrl "https://github.com/thoth-org/Thoth.Json/edit/main/docs"
    |> Theme.footer footer

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
    // The snippets predate the current packages, so they no longer compile as they stand.
    |> Literate.registerWith (fun options ->
        { options with
            TypeCheck = false
        }
    )
    |> Changelog.registerWith "changelogs" changelogs
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
    |> Site.collection changelog

[<EntryPoint>]
let main argv = Nacara.run site argv
