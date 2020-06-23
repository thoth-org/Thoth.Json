#r "../_lib/Fornax.Core.dll"


type Shortcut = {
    title: string
    link: string
    icon: string
}

let loader (projectRoot: string) (siteContent: SiteContents) =
    siteContent.Add({title = "Home"; link = "/"; icon = "fas fa-home"})
    siteContent.Add({title = "GitHub repo"; link = "https://github.com/thoth-org/Thoth.Json/"; icon = "fab fa-github"})
    siteContent