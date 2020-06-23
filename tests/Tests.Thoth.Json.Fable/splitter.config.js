module.exports = {
    entry: resolve("./Tests.Thoth.Json.Fable.fsproj"),
    outDir: resolve("./bin"),
    babel: {
        plugins: ["transform-es2015-modules-commonjs"],
    },
    fable: {
        define: [ "THOTH_JSON_FABLE" ]
    }
};

function resolve(path) {
    return require("path").join(__dirname, path);
}
