const path = require("path");

module.exports = {
    permalink: data => {
        return path.format({
            ...path.parse(data.page.inputPath),
            base: undefined,
            ext: ".html"
        })
    }
}
