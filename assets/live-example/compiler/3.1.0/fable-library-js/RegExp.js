import { Exception } from "./Util.js";
export function create(pattern, options = 0) {
    // Supported RegexOptions
    // * IgnoreCase:  0x0001
    // * Multiline:   0x0002
    // * Compiled:    0x0008 (ignored)
    // * Singleline:  0x0010
    // * ECMAScript:  0x0100 (ignored)
    if ((options & ~(1 ^ 2 ^ 8 ^ 16 ^ 256)) !== 0) {
        throw new Exception("RegexOptions only supports: IgnoreCase, Multiline, Compiled, Singleline and ECMAScript");
    }
    // Set always global and unicode flags for compatibility with dotnet, see #2925
    let flags = "gu";
    flags += options & 1 ? "i" : ""; // 0x0001 RegexOptions.IgnoreCase
    flags += options & 2 ? "m" : "";
    flags += options & 16 ? "s" : "";
    return new RegExp(pattern, flags);
}
// From http://stackoverflow.com/questions/3446170/escape-string-for-use-in-javascript-regex
export function escape(str) {
    // Matches the characters escaped by .NET's Regex.Escape.
    // Note:
    //
    //  .NET also escapes space and # (relevant for IgnorePatternWhitespace mode),
    //  but JS unicode-mode regex rejects \  and \# as invalid escapes, and we don't
    //  support IgnorePatternWhitespace, so we omit them.
    //
    //  .NET does not escape ] and } but JS unicode-mode regex rejects bare ] and }
    //  as invalid, so we escape them too for compatibility.
    return str.replace(/[$()*+.?[\\\^{|}\]]/g, "\\$&");
}
export function unescape(str) {
    return str.replace(/\\([$()*+.?[\\\^{|}\]])/g, "$1");
}
export function isMatch(reg, input, startAt = 0) {
    reg.lastIndex = startAt;
    return reg.test(input);
}
export function match(reg, input, startAt = 0) {
    reg.lastIndex = startAt;
    return reg.exec(input);
}
export function matches(reg, input, startAt = 0) {
    if (input == null) {
        throw new Exception("Input cannot ve null");
    }
    if (!reg.global) {
        throw new Exception("Non-global RegExp"); // Prevent infinite loop
    }
    reg.lastIndex = startAt;
    const matches = [];
    let m;
    let lastMatchIndex = -1;
    while ((m = reg.exec(input)) != null) {
        // It can happen even global regex get stuck, see #2845
        if (m.index === lastMatchIndex) {
            reg.lastIndex++;
        }
        else {
            lastMatchIndex = m.index;
            matches.push(m);
        }
    }
    return matches;
}
export function options(reg) {
    let options = 256; // ECMAScript
    options |= reg.ignoreCase ? 1 : 0;
    options |= reg.multiline ? 2 : 0;
    return options;
}
export function replace(reg, input, replacement, limit, offset = 0) {
    function replacer() {
        let res = arguments[0];
        if (limit) {
            limit--;
            const match = [];
            const len = arguments.length;
            // arguments: https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/String/replace#specifying_a_function_as_a_parameter
            // * match: matched substring
            // * p1, p2, ...: nth capture group string
            // * offset: offset of matched substring
            // * string: whole string examined
            // * groups: named capturing groups
            //           ONLY if regex contains a named capture group AND browser supports named groups
            // -> last element can be groups OR input string
            // -> check if last element is string
            const withGroups = typeof arguments[len - 1] !== "string";
            let pLast = withGroups ? len - 3 : len - 2;
            for (let i = 0; i < pLast; i++) {
                match.push(arguments[i]);
            }
            match.index = arguments[pLast++];
            match.input = arguments[pLast++];
            if (withGroups) {
                match.groups = arguments[pLast];
            }
            res = replacement(match);
        }
        return res;
    }
    if (typeof reg === "string") {
        const tmp = reg;
        reg = create(input, limit ?? 0);
        input = tmp;
        limit = undefined;
    }
    if (typeof replacement === "string") {
        const rep = replacement;
        // .NET replacement patterns cannot be rewritten as JS replacement strings unambiguously
        // (e.g. `$$0` must stay literal "$0", `$0` doesn't work with JS regex (see #1155), and
        // `${1}` followed by a digit must not become `$1` + digit), so interpret them with an
        // evaluator instead, honoring the `$$` escape left-to-right
        replacement = (match) => rep.replace(/\$(?:(\$)|(&)|(`)|(')|(\d+)|\{(\d+)\}|\{([^}]+)\})/g, (s, dollar, and, before, after, num, numBraced, name) => {
            if (dollar != null) {
                return "$";
            }
            if (and != null) {
                return match[0];
            }
            if (before != null) {
                return match.input.substring(0, match.index);
            }
            if (after != null) {
                return match.input.substring(match.index + match[0].length);
            }
            const n = num ?? numBraced;
            if (n != null) {
                const i = parseInt(n, 10);
                // Same as .NET: an unmatched group is replaced by an empty string,
                // a nonexistent group number keeps the substitution pattern literally
                return i < match.length ? (match[i] ?? "") : s;
            }
            // Named groups are `${name}` in .NET (in regex: groups are `(?<name>...)` in both)
            return match.groups != null && name in match.groups ? (match.groups[name] ?? "") : s;
        });
    }
    limit = limit == null ? -1 : limit;
    return input.substring(0, offset) + input.substring(offset).replace(reg, replacer);
}
export function split(reg, input, limit, offset = 0) {
    if (typeof reg === "string") {
        const tmp = reg;
        reg = create(input, limit ?? 0);
        input = tmp;
        limit = undefined;
    }
    // JS String.split(regex, limit) truncates the result and discards the remainder,
    // whereas .NET keeps it in the last element (`limit` is the max number of elements),
    // keeps the text before `offset` in the first element and includes the values of
    // matched capture groups, so iterate the matches manually like .NET Regex.Split does
    const result = [];
    let prev = 0;
    let count = limit == null ? -1 : limit - 1;
    for (const m of matches(reg, input, offset)) {
        if (count === 0) {
            break;
        }
        result.push(input.substring(prev, m.index));
        prev = m.index + m[0].length;
        for (let i = 1; i < m.length; i++) {
            if (m[i] !== undefined) {
                result.push(m[i]);
            }
        }
        count--;
    }
    result.push(input.substring(prev));
    return result;
}
