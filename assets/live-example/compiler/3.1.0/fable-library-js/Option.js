import { Exception, structuralHash, equals, compare } from "./Util.js";
// Using a class here for better compatibility with TS files importing Some
export class Some {
    constructor(value) {
        this.value = value;
    }
    toJSON() {
        return this.value;
    }
    // Don't add "Some" for consistency with erased options
    toString() {
        return String(this.value);
    }
    GetHashCode() {
        return structuralHash(this.value);
    }
    Equals(other) {
        if (other == null) {
            return false;
        }
        else {
            return equals(this.value, other instanceof Some ? other.value : other);
        }
    }
    CompareTo(other) {
        if (other == null) {
            return 1;
        }
        else {
            return compare(this.value, other instanceof Some ? other.value : other);
        }
    }
}
export function nonNullValue(x) {
    if (x == null) {
        throw new Exception("Nullable has no value");
    }
    else {
        return x;
    }
}
export function value(x) {
    if (x == null) {
        throw new Exception("Option has no value");
    }
    else {
        return x instanceof Some ? x.value : x;
    }
}
export function unwrap(opt) {
    return opt instanceof Some ? opt.value : opt;
}
export function some(x) {
    return x == null || x instanceof Some ? new Some(x) : x;
}
export function ofNullable(x) {
    // This will fail with unit probably, an alternative would be:
    // return x === null ? undefined : (x === undefined ? new Some(x) : x);
    return x == null ? undefined : x;
}
export function toNullable(x) {
    return x == null ? null : value(x);
}
export function flatten(x) {
    return x == null ? undefined : value(x);
}
export function toArray(opt) {
    return (opt == null) ? [] : [value(opt)];
}
export function defaultArg(opt, defaultValue) {
    return (opt != null) ? value(opt) : defaultValue;
}
export function orElse(opt, ifNone) {
    return opt == null ? ifNone : opt;
}
// Only used by Replacements.Util.fs's curry/uncurry helpers, not by `Option.map` itself.
export function map(mapping, opt) {
    return (opt != null) ? some(mapping(value(opt))) : undefined;
}
export function tryOp(op, arg) {
    try {
        return some(op(arg));
    }
    catch {
        return undefined;
    }
}
