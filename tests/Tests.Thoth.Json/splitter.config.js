module.exports = {
    entry: resolve("./Tests.Thoth.Json.FableRuntime.fsproj"),
    outDir: resolve("./bin"),
    babel: {
        plugins: ["transform-es2015-modules-commonjs"],
    },
    fable: {
        define: [ "THOTH_JSON" ]
    }
};

function resolve(path) {
    return require("path").join(__dirname, path);
}
