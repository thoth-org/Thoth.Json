
import { printf, toConsole } from "fable-library-js/String.js";

/**
 * Print a decoding result: the value on success, the error message as Thoth.Json wrote it.
 */
export function print(result) {
    if (result.tag === 1) {
        toConsole(printf("%s"))(result.fields[0]);
    }
    else {
        toConsole(printf("%A"))(result.fields[0]);
    }
}

