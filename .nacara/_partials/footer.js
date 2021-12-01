import React from 'react';

const SitemapSection = ({
  title,
  children
}) => /*#__PURE__*/React.createElement("div", {
  className: "sitemap-section"
}, /*#__PURE__*/React.createElement("div", {
  className: "sitemap-section-title"
}, title), /*#__PURE__*/React.createElement("ul", {
  className: "sitemap-section-list"
}, children));

const SitemapSectionItem = ({
  text,
  icon,
  url
}) => /*#__PURE__*/React.createElement("li", null, /*#__PURE__*/React.createElement("a", {
  href: url,
  className: "icon-text sitemap-section-list-item"
}, /*#__PURE__*/React.createElement("span", {
  className: "icon"
}, /*#__PURE__*/React.createElement("i", {
  className: icon
})), /*#__PURE__*/React.createElement("span", {
  className: "sitemap-section-list-item-text"
}, text)));

const CopyrightScript = () => /*#__PURE__*/React.createElement("script", {
  dangerouslySetInnerHTML: {
    __html: `
        const year = new Date().getFullYear();
        document.getElementById('copyright-end-year').innerHTML = year;
        `
  }
});

export default /*#__PURE__*/React.createElement("div", {
  className: "is-size-5"
}, /*#__PURE__*/React.createElement("div", {
  className: "sitemap"
}, /*#__PURE__*/React.createElement(SitemapSection, {
  title: "Project ressources"
}, /*#__PURE__*/React.createElement(SitemapSectionItem, {
  text: "Repository",
  icon: "fas fa-file-code",
  url: "https://github.com/thoth-org/Thoth.Json"
}), /*#__PURE__*/React.createElement(SitemapSectionItem, {
  text: "Changelog",
  icon: "fas fa-list",
  url: "/Thoth.Json/changelog/index.html"
}), /*#__PURE__*/React.createElement(SitemapSectionItem, {
  text: "License",
  icon: "fas fa-id-card",
  url: "https://github.com/thoth-org/Thoth.Json/blob/main/LICENSE.md"
})), /*#__PURE__*/React.createElement(SitemapSection, {
  title: "Community"
}, /*#__PURE__*/React.createElement(SitemapSectionItem, {
  text: "Support",
  icon: "fab fa-gitter",
  url: "https://gitter.im/fable-compiler/Fable"
}), /*#__PURE__*/React.createElement(SitemapSectionItem, {
  text: "Twitter",
  icon: "fab fa-twitter",
  url: "https://github.com/thoth-org/Thoth.Json/blob/main/LICENSE.md"
}))), /*#__PURE__*/React.createElement("p", {
  className: "has-text-centered"
}, "Built with ", /*#__PURE__*/React.createElement("a", {
  className: "is-underlined",
  href: "https://mangelmaxime.github.io/Nacara/"
}, "Nacara")), /*#__PURE__*/React.createElement("p", {
  className: "has-text-centered mt-2"
}, "Copyright \xA9 2021-", /*#__PURE__*/React.createElement("span", {
  id: "copyright-end-year"
}), " ", /*#__PURE__*/React.createElement("a", {
  className: "is-underlined",
  href: "https://twitter.com/MangelMaxime"
}, "Maxime Mangel"), " and contributors."), /*#__PURE__*/React.createElement(CopyrightScript, null));