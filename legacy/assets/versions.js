(() => {
    "use strict";

    class NacaraVersionSwitcher extends HTMLElement {
        connectedCallback() {
            if (this.dataset.upgraded === "true") return;

            let versions;

            try {
                versions = JSON.parse(this.dataset.versions || "[]");
            } catch {
                return;
            }

            // A host answers anything it cannot find with the root 404.html, which belongs to the latest version.
            const reading = this.readingFrom(versions) || versions[0];
            if (!reading) return;

            this.dataset.upgraded = "true";

            const select = document.createElement("select");
            select.className = "nacara-versions__select";
            select.setAttribute("aria-label", "Documentation version");

            for (const version of versions) {
                const option = document.createElement("option");
                option.value = version.prefix ?? "";
                option.textContent = version.label;
                option.selected = version === reading;
                select.append(option);
            }

            select.addEventListener("change", () => this.goTo(reading, select.value));
            this.append(select);

            if (!reading.latest) this.showOutdatedNotice(reading, versions);
        }

        readingFrom(versions) {
            const base = this.dataset.base || "/";
            const path = location.pathname;

            // Longest first, so "2.0" is not mistaken for "2" when both are published.
            const inPath = [...versions]
                .filter((version) => version.prefix)
                .sort((left, right) => right.prefix.length - left.prefix.length)
                .find(
                    (version) =>
                        path === base + version.prefix ||
                        path.startsWith(base + version.prefix + "/"),
                );

            return inPath || versions.find((version) => version.current);
        }

        rewritePath(from, to) {
            const base = this.dataset.base || "/";
            let path = location.pathname;

            if (from && path.startsWith(base + from + "/")) {
                path = base + path.slice((base + from + "/").length);
            } else if (from && path === base + from) {
                path = base;
            }

            // `base` ends in a slash, and the tail no longer starts with one.
            return to ? base + to + "/" + path.slice(base.length) : path;
        }

        async goTo(reading, prefix) {
            const target = this.rewritePath(reading.prefix ?? "", prefix ?? "");
            const base = this.dataset.base || "/";
            const home = prefix ? base + prefix + "/" : base;

            try {
                const response = await fetch(target, { method: "HEAD" });
                location.href = response.ok ? target : home;
            } catch {
                location.href = home;
            }
        }

        showOutdatedNotice(reading, versions) {
            const latest = versions.find((version) => version.latest) || versions[0];
            if (!latest || latest === reading) return;

            const notice = document.createElement("aside");
            notice.className = "nacara-callout nacara-versions__notice";
            notice.dataset.kind = "warning";
            notice.dataset.title = "Older version";

            const link = document.createElement("a");
            link.href = this.rewritePath(reading.prefix ?? "", latest.prefix ?? "");
            link.textContent = `Go to ${latest.label}`;

            const text = document.createElement("p");
            text.textContent = `You are reading the documentation for ${reading.label}. The current version is ${latest.label}. `;
            text.append(link);
            notice.append(text);

            document.querySelector("main")?.prepend(notice);
        }
    }

    customElements.define("nacara-version-switcher", NacaraVersionSwitcher);
})();
