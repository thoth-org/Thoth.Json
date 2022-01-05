import React from 'react';

const SitemapSection = ({ title, children }) => (
    <div className="sitemap-section">
        <div className="sitemap-section-title">
            {title}
        </div>
        <ul className="sitemap-section-list">
            {children}
        </ul>
    </div>
)

const SitemapSectionItem = ({ text, icon, url }) => (
    <li>
        <a href={url} className="icon-text sitemap-section-list-item">
            <span className="icon">
                <i className={icon}></i>
            </span>
            <span className="sitemap-section-list-item-text">
                {text}
            </span>
        </a>
    </li>
)

const CopyrightScript = () => (
    <script dangerouslySetInnerHTML={{
        __html: `
        const year = new Date().getFullYear();
        document.getElementById('copyright-end-year').innerHTML = year;
        `
    }} />
)

export default (
    <div className="is-size-5">
        <div className="sitemap">
            <SitemapSection title="Project ressources">
                <SitemapSectionItem
                    text="Repository"
                    icon="fas fa-file-code"
                    url="https://github.com/thoth-org/Thoth.Json" />

                <SitemapSectionItem
                    text="Changelog"
                    icon="fas fa-list"
                    url="/Thoth.Json/changelog/index.html" />

                <SitemapSectionItem
                    text="License"
                    icon="fas fa-id-card"
                    url="https://github.com/thoth-org/Thoth.Json/blob/main/LICENSE.md" />
            </SitemapSection>
            <SitemapSection title="Community">
                <SitemapSectionItem
                    text="Support"
                    icon="fab fa-gitter"
                    url="https://gitter.im/fable-compiler/Fable" />

                <SitemapSectionItem
                    text="Twitter"
                    icon="fab fa-twitter"
                    url="https://twitter.com/MangelMaxime" />
            </SitemapSection>

        </div>
        <p className="has-text-centered">
            Built with <a className="is-underlined" href="https://mangelmaxime.github.io/Nacara/">Nacara</a>
        </p>
        <p className="has-text-centered mt-2">
            Copyright © 2021-<span id="copyright-end-year"/> <a className="is-underlined" href="https://twitter.com/MangelMaxime">Maxime Mangel</a> and contributors.
        </p>
        <CopyrightScript />
    </div>
)
