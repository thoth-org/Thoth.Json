(() => {
    "use strict";

    const bundlePath =
        document.querySelector("[data-nacara-search]")?.dataset.bundle || "/pagefind/";

    let loading = null;
    let opener = null;

    const load = () =>
        (loading ||= (async () => {
            const styles = document.createElement("link");
            styles.rel = "stylesheet";
            styles.href = `${bundlePath}pagefind-component-ui.css`;
            document.head.append(styles);

            await new Promise((resolve, reject) => {
                const script = document.createElement("script");
                script.type = "module";
                script.src = `${bundlePath}pagefind-component-ui.js`;
                script.onload = resolve;
                script.onerror = reject;
                document.head.append(script);
            });

            // pagefind-config is read once, when the component connects.
            const config = document.createElement("pagefind-config");
            config.setAttribute("bundle-path", bundlePath);
            document.body.append(config);

            document.body.append(document.createElement("pagefind-modal"));

            opener = document.createElement("pagefind-modal-trigger");
            opener.className = "nacara-search__opener";
            opener.setAttribute("hide-shortcut", "");
            document.body.append(opener);

            await customElements.whenDefined("pagefind-modal-trigger");
        })().catch(() => {
            loading = null;
            throw new Error("Pagefind could not be loaded");
        }));

    const open = async () => {
        try {
            await load();
        } catch {
            return;
        }

        // The button inside the trigger carries the click handler.
        (opener.querySelector("button") || opener).click();
    };

    const isApple = /Mac|iPhone|iPad|iPod/.test(navigator.platform || navigator.userAgent);

    for (const hint of document.querySelectorAll("[data-nacara-search-shortcut]")) {
        hint.textContent = isApple ? "⌘ K" : "Ctrl K";
    }

    document.addEventListener("click", (event) => {
        if (event.target.closest("[data-nacara-search]")) open();
    });

    // Once Pagefind is loaded its own trigger listens for the chord, and two openers would fight.
    document.addEventListener("keydown", (event) => {
        if (!loading && (event.metaKey || event.ctrlKey) && event.key.toLowerCase() === "k") {
            event.preventDefault();
            open();
            return;
        }

        const typing =
            /^(input|textarea|select)$/i.test(event.target.tagName) ||
            event.target.isContentEditable;

        if (event.key === "/" && !typing) {
            event.preventDefault();
            open();
        }
    });
})();
