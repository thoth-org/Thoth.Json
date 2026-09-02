import { FSharpRef } from "./Types.js";
import { Exception } from "./Util.js";
export function tryParse(str, defValue) {
    // TODO: test if value is valid and in range
    if (str != null && /\S/.test(str)) {
        const v = +str.replace("_", "");
        if (!Number.isNaN(v)) {
            defValue.contents = v;
            return true;
        }
    }
    return false;
}
export function parse(str) {
    const defValue = new FSharpRef(0);
    if (tryParse(str, defValue)) {
        return defValue.contents;
    }
    else {
        throw new Exception(`The input string ${str} was not in a correct format.`);
    }
}
// JS Number.isFinite function evals false for NaN
export function isPositiveInfinity(x) {
    return x === Number.POSITIVE_INFINITY;
}
export function isNegativeInfinity(x) {
    return x === Number.NEGATIVE_INFINITY;
}
export function isInfinity(x) {
    return x === Number.POSITIVE_INFINITY || x === Number.NEGATIVE_INFINITY;
}
export function max(x, y) {
    return Math.max(x, y);
}
export function min(x, y) {
    return Math.min(x, y);
}
export function maxMagnitude(x, y) {
    if (Number.isNaN(x) || Number.isNaN(y)) {
        return NaN;
    }
    return Math.abs(x) > Math.abs(y) ? x : y;
}
export function minMagnitude(x, y) {
    if (Number.isNaN(x) || Number.isNaN(y)) {
        return NaN;
    }
    return Math.abs(x) < Math.abs(y) ? x : y;
}
export function clamp(x, min, max) {
    return x < min ? min : x > max ? max : x;
}
