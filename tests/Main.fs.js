import { Mocha_runTests, Test_testList } from "./fable_modules/Fable.Mocha.2.17.0/Mocha.fs.js";
import { tests } from "./Decoders.fs.js";
import { singleton } from "./fable_modules/fable-library.4.5.0/List.js";

export const allTests = Test_testList("All", singleton(tests));

(function (args) {
    return Mocha_runTests(allTests);
})(typeof process === 'object' ? process.argv.slice(2) : []);

