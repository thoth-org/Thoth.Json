export default {
    siteMetadata: {
        title: "Thoth.Json",
        url: "https://thoth-org.github.io/",
        baseUrl: "/Thoth.Json/",
        editUrl: "https://github.com/thoth-org/Thoth.Json/edit/master/docs"
    },
    navbar: {
        start: [
            {
                pinned: true,
                section: "documentation",
                label: "Docs",
                url: "/Thoth.Json/documentation/concept/introduction.html"
            }
        ],
        end: [
            {
                url: "https://github.com/MangelMaxime/Nacara",
                icon: "fab fa-github",
                label: "GitHub"
            },
            {
                url: "https://twitter.com/MangelMaxime",
                icon: "fab fa-twitter",
                label: "Twitter"
            }
        ]
    },
    remarkPlugins: [
        {
            resolve: "gatsby-remark-vscode",
            property: "remarkPlugin",
            options: {
                theme: "Atom One Light",
                extensions: [
                    "vscode-theme-onelight"
                ]
            }
        },
        {
            resolve: "remark-github",
            options: {
                repository: "thoth-org/Thoth.Json"
            }
        }
    ],
    layouts: [
        "nacara-layout-standard"
    ]
};
