(() => {
    "use strict";

    const reveal = () => {
        const id = location.hash.slice(1);
        if (!id) return;

        const target = document.getElementById(id);
        if (!target) return;

        const entry =
            target.querySelector("details.nacara-api__member") ||
            target.closest("details.nacara-api__member");

        if (entry) entry.open = true;

        (entry || target).scrollIntoView({ block: "start" });
    };

    addEventListener("beforeprint", () => {
        for (const entry of document.querySelectorAll("details.nacara-api__member")) {
            entry.open = true;
        }
    });

    addEventListener("hashchange", reveal);
    reveal();
})();
