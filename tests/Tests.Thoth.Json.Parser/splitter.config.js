module.exports = {
    entry: resolve("./Tests.Thoth.Json.Parser.fsproj"),
    outDir: resolve("./bin"),
    babel: {
        plugins: ["transform-es2015-modules-commonjs"],
    },
    fable: {
        define: [ ]
    }
};

function resolve(path) {
    return require("path").join(__dirname, path);
}
