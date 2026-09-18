
import { contains, ofSeq } from "fable-library-js/Set.js";
import { equals, comparePrimitives } from "fable-library-js/Util.js";
import { mapIndexed, map, empty as empty_1, singleton as singleton_1, collect, append, delay, forAll, fold, reverse, toList } from "fable-library-js/Seq.js";
import { join, split } from "fable-library-js/String.js";
import { empty, singleton, ofArrayWithTail, head, tail, isEmpty, cons } from "fable-library-js/List.js";
import { isUpper } from "fable-library-js/Char.js";
import { StringBuilder__Append_244C7CD6, StringBuilder__Clear, StringBuilder__get_Length, StringBuilder_$ctor } from "fable-library-js/System.Text.js";
import { toString } from "fable-library-js/Types.js";

function upperFirst(str) {
    return str.slice(undefined, 0 + 1).toUpperCase() + str.slice(1, str.length);
}

const dotNetAcronyms = ofSeq(["id", "ip"], {
    Compare: (x, y) => (comparePrimitives(x, y) | 0),
});

export function convertCase(source, dest, text) {
    if (equals(source, dest)) {
        return text;
    }
    else {
        const words = (source.tag === 1) ? toList(split(text, ["_"], undefined, 1)) : ((source.tag === 2) ? toList(reverse(fold((state, next) => {
            if (next.length > 1) {
                return cons(next, state);
            }
            else if (!isEmpty(state)) {
                const xs = tail(state);
                const x = head(state);
                if ((x.length === 1) ? true : forAll(isUpper, x.split(""))) {
                    return cons(x + next, xs);
                }
                else {
                    return ofArrayWithTail([next, x], xs);
                }
            }
            else {
                return singleton(next);
            }
        }, empty(), delay(() => {
            const sb = StringBuilder_$ctor();
            return append(collect((c) => append((isUpper(c) && (StringBuilder__get_Length(sb) > 0)) ? append(singleton_1(toString(sb)), delay(() => {
                StringBuilder__Clear(sb);
                return empty_1();
            })) : empty_1(), delay(() => {
                StringBuilder__Append_244C7CD6(sb, c);
                return empty_1();
            })), text.split("")), delay(() => ((StringBuilder__get_Length(sb) > 0) ? singleton_1(toString(sb)) : empty_1())));
        })))) : ((source.tag === 3) ? toList(reverse(fold((state, next) => {
            if (next.length > 1) {
                return cons(next, state);
            }
            else if (!isEmpty(state)) {
                const xs = tail(state);
                const x = head(state);
                if ((x.length === 1) ? true : forAll(isUpper, x.split(""))) {
                    return cons(x + next, xs);
                }
                else {
                    return ofArrayWithTail([next, x], xs);
                }
            }
            else {
                return singleton(next);
            }
        }, empty(), delay(() => {
            const sb = StringBuilder_$ctor();
            return append(collect((c) => append((isUpper(c) && (StringBuilder__get_Length(sb) > 0)) ? append(singleton_1(toString(sb)), delay(() => {
                StringBuilder__Clear(sb);
                return empty_1();
            })) : empty_1(), delay(() => {
                StringBuilder__Append_244C7CD6(sb, c);
                return empty_1();
            })), text.split("")), delay(() => ((StringBuilder__get_Length(sb) > 0) ? singleton_1(toString(sb)) : empty_1())));
        })))) : ((source.tag === 4) ? toList(reverse(fold((state, next) => {
            if (next.length > 1) {
                return cons(next, state);
            }
            else if (!isEmpty(state)) {
                const xs = tail(state);
                const x = head(state);
                if ((x.length === 1) ? true : forAll(isUpper, x.split(""))) {
                    return cons(x + next, xs);
                }
                else {
                    return ofArrayWithTail([next, x], xs);
                }
            }
            else {
                return singleton(next);
            }
        }, empty(), delay(() => {
            const sb = StringBuilder_$ctor();
            return append(collect((c) => append((isUpper(c) && (StringBuilder__get_Length(sb) > 0)) ? append(singleton_1(toString(sb)), delay(() => {
                StringBuilder__Clear(sb);
                return empty_1();
            })) : empty_1(), delay(() => {
                StringBuilder__Append_244C7CD6(sb, c);
                return empty_1();
            })), text.split("")), delay(() => ((StringBuilder__get_Length(sb) > 0) ? singleton_1(toString(sb)) : empty_1())));
        })))) : ((source.tag === 5) ? toList(reverse(fold((state, next) => {
            if (next.length > 1) {
                return cons(next, state);
            }
            else if (!isEmpty(state)) {
                const xs = tail(state);
                const x = head(state);
                if ((x.length === 1) ? true : forAll(isUpper, x.split(""))) {
                    return cons(x + next, xs);
                }
                else {
                    return ofArrayWithTail([next, x], xs);
                }
            }
            else {
                return singleton(next);
            }
        }, empty(), delay(() => {
            const sb = StringBuilder_$ctor();
            return append(collect((c) => append((isUpper(c) && (StringBuilder__get_Length(sb) > 0)) ? append(singleton_1(toString(sb)), delay(() => {
                StringBuilder__Clear(sb);
                return empty_1();
            })) : empty_1(), delay(() => {
                StringBuilder__Append_244C7CD6(sb, c);
                return empty_1();
            })), text.split("")), delay(() => ((StringBuilder__get_Length(sb) > 0) ? singleton_1(toString(sb)) : empty_1())));
        })))) : toList(split(text, ["_"], undefined, 1))))));
        switch (dest.tag) {
            case 0:
                return join("_", map((x_2) => x_2.toLowerCase(), words));
            case 2:
                return join("", map((x_3) => upperFirst(x_3.toLowerCase()), words));
            case 3:
                return join("", mapIndexed((i, x_4) => {
                    if (i === 0) {
                        return x_4.toLowerCase();
                    }
                    else {
                        return upperFirst(x_4.toLowerCase());
                    }
                }, words));
            case 4:
                return join("", map((x_5) => {
                    const u = x_5.toLowerCase();
                    if (contains(u, dotNetAcronyms)) {
                        return u.toUpperCase();
                    }
                    else {
                        return upperFirst(u);
                    }
                }, words));
            case 5:
                return join("", mapIndexed((i_1, x_6) => {
                    if (i_1 === 0) {
                        return x_6.toLowerCase();
                    }
                    else {
                        const u_1 = x_6.toLowerCase();
                        if (contains(u_1, dotNetAcronyms)) {
                            return u_1.toUpperCase();
                        }
                        else {
                            return upperFirst(u_1);
                        }
                    }
                }, words));
            default:
                return join("_", map((x_1) => x_1.toUpperCase(), words));
        }
    }
}

