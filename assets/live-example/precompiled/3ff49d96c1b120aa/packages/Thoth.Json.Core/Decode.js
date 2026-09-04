
import { join } from "fable-library-js/String.js";
import { length, tail as tail_1, head as head_1, isEmpty, cons, reverse as reverse_1, ofSeq, singleton, append, empty, fold, tryLast, map as map_1 } from "fable-library-js/List.js";
import { Result_Map, Result_MapError, FSharpResult$2 } from "fable-library-js/Result.js";
import { Json, ErrorReason$1 } from "./Types.js";
import { tryParse as tryParse_2 } from "fable-library-js/Guid.js";
import { Record, FSharpRef } from "fable-library-js/Types.js";
import { compare, comparePrimitives, int32ToString, defaultOf } from "fable-library-js/Util.js";
import { Uri } from "fable-library-js/Uri.js";
import { tryParse as tryParse_3 } from "fable-library-js/Int32.js";
import { tryParse as tryParse_4 } from "fable-library-js/Long.js";
import { tryParse as tryParse_5, fromInt32 } from "fable-library-js/BigInt.js";
import { tryParse as tryParse_6 } from "fable-library-js/Double.js";
import { tryParse as tryParse_7 } from "fable-library-js/Decimal.js";
import Decimal from "fable-library-js/Decimal.js";
import { toUniversalTime, tryParse as tryParse_8, minValue } from "fable-library-js/Date.js";
import { tryParse as tryParse_9, minValue as minValue_1 } from "fable-library-js/DateOffset.js";
import { tryParse as tryParse_10 } from "fable-library-js/TimeSpan.js";
import { value as value_7, defaultArg, some } from "fable-library-js/Option.js";
import { fill, setItem, fold as fold_1, item } from "fable-library-js/Array.js";
import { toArray, toList, append as append_1, reverse } from "fable-library-js/Seq.js";
import { class_type } from "fable-library-js/Reflection.js";
import { ofSeq as ofSeq_1, ofList } from "fable-library-js/Map.js";

/**
 * Put a path segment in front of a failure's own path.
 */
export function Helpers_prependPath(path, err_, err__1) {
    const err = [err_, err__1];
    return [path + err[0], err[1]];
}

function genericMsg(helpers, msg, value_1, newLine) {
    try {
        return ((("Expecting " + msg) + " but instead got:") + (newLine ? "\n" : " ")) + helpers.anyToString(value_1);
    }
    catch (matchValue) {
        return (("Expecting " + msg) + " but decoder failed. Couldn\'t report given value due to circular structure.") + (newLine ? "\n" : " ");
    }
}

/**
 * Format a decoding failure as the message the entry points return.
 */
export function errorToString(helpers, path, error) {
    const reason_1 = (error.tag === 2) ? genericMsg(helpers, error.fields[0], error.fields[1], true) : ((error.tag === 1) ? ((genericMsg(helpers, error.fields[0], error.fields[1], false) + "\nReason: ") + error.fields[2]) : ((error.tag === 3) ? genericMsg(helpers, error.fields[0], error.fields[1], true) : ((error.tag === 4) ? (genericMsg(helpers, error.fields[0], error.fields[1], true) + (("\nNode `" + error.fields[2]) + "` is unknown.")) : ((error.tag === 5) ? ((("Expecting " + error.fields[0]) + ".\n") + helpers.anyToString(error.fields[1])) : ((error.tag === 7) ? ("The following errors were found:\n\n" + join("\n\n", map_1((error_1) => {
        const tupledArg = Helpers_prependPath(path, error_1[0], error_1[1]);
        return errorToString(helpers, tupledArg[0], tupledArg[1]);
    }, error.fields[0]))) : ((error.tag === 6) ? ("The following `failure` occurred with the decoder: " + error.fields[0]) : genericMsg(helpers, error.fields[0], error.fields[1], false)))))));
    if (error.tag === 7) {
        return reason_1;
    }
    else {
        return (("Error at: `" + path) + "`\n") + reason_1;
    }
}

/**
 * Run a decoder against a JSON value of the runtime's own type, giving the formatted
 * message on failure.
 */
export function Advanced_fromValue(helpers, decoder, value_1) {
    const matchValue = decoder.Decode(helpers, value_1);
    if (matchValue.tag === 1) {
        const error = matchValue.fields[0];
        return new FSharpResult$2(/* Error */ 1, [errorToString(helpers, error[0], error[1])]);
    }
    else {
        return new FSharpResult$2(/* Ok */ 0, [matchValue.fields[0]]);
    }
}

export const string = {
    Decode(helpers, value_1) {
        return helpers.isString(value_1) ? (new FSharpResult$2(/* Ok */ 0, [helpers.asString(value_1)])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a string", value_1])]]));
    },
};

export const char = {
    Decode(helpers, value_1) {
        if (helpers.isString(value_1)) {
            const str = helpers.asString(value_1);
            return (str.length === 1) ? (new FSharpResult$2(/* Ok */ 0, [str[0]])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a single character string", value_1])]]));
        }
        else {
            return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a char", value_1])]]);
        }
    },
};

export const guid = {
    Decode(helpers, value_1) {
        if (helpers.isString(value_1)) {
            let matchValue;
            let outArg = "00000000-0000-0000-0000-000000000000";
            matchValue = [tryParse_2(helpers.asString(value_1), new FSharpRef(() => outArg, (v) => {
                outArg = v;
            })), outArg];
            return matchValue[0] ? (new FSharpResult$2(/* Ok */ 0, [matchValue[1]])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a guid", value_1])]]));
        }
        else {
            return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a guid", value_1])]]);
        }
    },
};

export const uri = {
    Decode(helpers, value_1) {
        if (helpers.isString(value_1)) {
            let matchValue;
            let outArg = defaultOf();
            matchValue = [Uri.tryCreate(helpers.asString(value_1), 0, new FSharpRef(() => outArg, (v) => {
                outArg = v;
            })), outArg];
            return matchValue[0] ? (new FSharpResult$2(/* Ok */ 0, [matchValue[1]])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a URI", value_1])]]));
        }
        else {
            return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a URI", value_1])]]);
        }
    },
};

export const unit = {
    Decode(helpers, value_1) {
        return helpers.isNullValue(value_1) ? (new FSharpResult$2(/* Ok */ 0, [undefined])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["null", value_1])]]));
    },
};

export const sbyte = {
    Decode(helpers, value_2) {
        if (helpers.isNumber(value_2)) {
            if (helpers.isIntegralValue(value_2)) {
                const floatValue = helpers.asFloat(value_2);
                return ((-128 <= floatValue) && (floatValue <= 127)) ? (new FSharpResult$2(/* Ok */ 0, [(floatValue + 0x80 & 0xFF) - 0x80])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitiveExtra */ 1, ["a sbyte", value_2, "Value was either too large or too small for a sbyte"])]]));
            }
            else {
                return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitiveExtra */ 1, ["a sbyte", value_2, "Value is not an integral value"])]]);
            }
        }
        else if (helpers.isString(value_2)) {
            let matchValue;
            let outArg = 0;
            matchValue = [tryParse_3(helpers.asString(value_2), 511, false, 8, new FSharpRef(() => (outArg | 0), (v) => {
                outArg = (v | 0);
            })), outArg];
            return matchValue[0] ? (new FSharpResult$2(/* Ok */ 0, [matchValue[1]])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a sbyte", value_2])]]));
        }
        else {
            return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a sbyte", value_2])]]);
        }
    },
};

export const byte = {
    Decode(helpers, value_2) {
        if (helpers.isNumber(value_2)) {
            if (helpers.isIntegralValue(value_2)) {
                const floatValue = helpers.asFloat(value_2);
                return ((0 <= floatValue) && (floatValue <= 255)) ? (new FSharpResult$2(/* Ok */ 0, [floatValue & 0xFF])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitiveExtra */ 1, ["a byte", value_2, "Value was either too large or too small for a byte"])]]));
            }
            else {
                return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitiveExtra */ 1, ["a byte", value_2, "Value is not an integral value"])]]);
            }
        }
        else if (helpers.isString(value_2)) {
            let matchValue;
            let outArg = 0;
            matchValue = [tryParse_3(helpers.asString(value_2), 511, true, 8, new FSharpRef(() => outArg, (v) => {
                outArg = v;
            })), outArg];
            return matchValue[0] ? (new FSharpResult$2(/* Ok */ 0, [matchValue[1]])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a byte", value_2])]]));
        }
        else {
            return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a byte", value_2])]]);
        }
    },
};

export const int16 = {
    Decode(helpers, value_2) {
        if (helpers.isNumber(value_2)) {
            if (helpers.isIntegralValue(value_2)) {
                const floatValue = helpers.asFloat(value_2);
                return ((-32768 <= floatValue) && (floatValue <= 32767)) ? (new FSharpResult$2(/* Ok */ 0, [(floatValue + 0x8000 & 0xFFFF) - 0x8000])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitiveExtra */ 1, ["an int16", value_2, "Value was either too large or too small for an int16"])]]));
            }
            else {
                return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitiveExtra */ 1, ["an int16", value_2, "Value is not an integral value"])]]);
            }
        }
        else if (helpers.isString(value_2)) {
            let matchValue;
            let outArg = 0;
            matchValue = [tryParse_3(helpers.asString(value_2), 511, false, 16, new FSharpRef(() => (outArg | 0), (v) => {
                outArg = (v | 0);
            })), outArg];
            return matchValue[0] ? (new FSharpResult$2(/* Ok */ 0, [matchValue[1]])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["an int16", value_2])]]));
        }
        else {
            return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["an int16", value_2])]]);
        }
    },
};

export const uint16 = {
    Decode(helpers, value_2) {
        if (helpers.isNumber(value_2)) {
            if (helpers.isIntegralValue(value_2)) {
                const floatValue = helpers.asFloat(value_2);
                return ((0 <= floatValue) && (floatValue <= 65535)) ? (new FSharpResult$2(/* Ok */ 0, [floatValue & 0xFFFF])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitiveExtra */ 1, ["an uint16", value_2, "Value was either too large or too small for an uint16"])]]));
            }
            else {
                return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitiveExtra */ 1, ["an uint16", value_2, "Value is not an integral value"])]]);
            }
        }
        else if (helpers.isString(value_2)) {
            let matchValue;
            let outArg = 0;
            matchValue = [tryParse_3(helpers.asString(value_2), 511, true, 16, new FSharpRef(() => outArg, (v) => {
                outArg = v;
            })), outArg];
            return matchValue[0] ? (new FSharpResult$2(/* Ok */ 0, [matchValue[1]])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["an uint16", value_2])]]));
        }
        else {
            return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["an uint16", value_2])]]);
        }
    },
};

export const int = {
    Decode(helpers, value_2) {
        if (helpers.isNumber(value_2)) {
            if (helpers.isIntegralValue(value_2)) {
                const floatValue = helpers.asFloat(value_2);
                return ((-2147483648 <= floatValue) && (floatValue <= 2147483647)) ? (new FSharpResult$2(/* Ok */ 0, [~~floatValue])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitiveExtra */ 1, ["an int", value_2, "Value was either too large or too small for an int"])]]));
            }
            else {
                return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitiveExtra */ 1, ["an int", value_2, "Value is not an integral value"])]]);
            }
        }
        else if (helpers.isString(value_2)) {
            let matchValue;
            let outArg = 0;
            matchValue = [tryParse_3(helpers.asString(value_2), 511, false, 32, new FSharpRef(() => (outArg | 0), (v) => {
                outArg = (v | 0);
            })), outArg];
            return matchValue[0] ? (new FSharpResult$2(/* Ok */ 0, [matchValue[1]])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["an int", value_2])]]));
        }
        else {
            return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["an int", value_2])]]);
        }
    },
};

export const uint32 = {
    Decode(helpers, value_2) {
        if (helpers.isNumber(value_2)) {
            if (helpers.isIntegralValue(value_2)) {
                const floatValue = helpers.asFloat(value_2);
                return ((0 <= floatValue) && (floatValue <= 4294967295)) ? (new FSharpResult$2(/* Ok */ 0, [floatValue >>> 0])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitiveExtra */ 1, ["an uint32", value_2, "Value was either too large or too small for an uint32"])]]));
            }
            else {
                return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitiveExtra */ 1, ["an uint32", value_2, "Value is not an integral value"])]]);
            }
        }
        else if (helpers.isString(value_2)) {
            let matchValue;
            let outArg = 0;
            matchValue = [tryParse_3(helpers.asString(value_2), 511, true, 32, new FSharpRef(() => outArg, (v) => {
                outArg = v;
            })), outArg];
            return matchValue[0] ? (new FSharpResult$2(/* Ok */ 0, [matchValue[1]])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["an uint32", value_2])]]));
        }
        else {
            return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["an uint32", value_2])]]);
        }
    },
};

export const int64 = (() => {
    const tryParse_1 = (text) => {
        let outArg = 0n;
        return [tryParse_4(text, 511, false, 64, new FSharpRef(() => outArg, (v) => {
            outArg = v;
        })), outArg];
    };
    return {
        Decode(helpers, value_1) {
            if (helpers.isNumber(value_1)) {
                const matchValue = tryParse_1(helpers.numberToString(value_1));
                return matchValue[0] ? (new FSharpResult$2(/* Ok */ 0, [matchValue[1]])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["an int64", value_1])]]));
            }
            else if (helpers.isString(value_1)) {
                const matchValue_1 = tryParse_1(helpers.asString(value_1));
                return matchValue_1[0] ? (new FSharpResult$2(/* Ok */ 0, [matchValue_1[1]])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["an int64", value_1])]]));
            }
            else {
                return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["an int64", value_1])]]);
            }
        },
    };
})();

export const uint64 = (() => {
    const tryParse_1 = (text) => {
        let outArg = 0n;
        return [tryParse_4(text, 511, true, 64, new FSharpRef(() => outArg, (v) => {
            outArg = v;
        })), outArg];
    };
    return {
        Decode(helpers, value_1) {
            if (helpers.isNumber(value_1)) {
                const matchValue = tryParse_1(helpers.numberToString(value_1));
                return matchValue[0] ? (new FSharpResult$2(/* Ok */ 0, [matchValue[1]])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["an uint64", value_1])]]));
            }
            else if (helpers.isString(value_1)) {
                const matchValue_1 = tryParse_1(helpers.asString(value_1));
                return matchValue_1[0] ? (new FSharpResult$2(/* Ok */ 0, [matchValue_1[1]])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["an uint64", value_1])]]));
            }
            else {
                return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["an uint64", value_1])]]);
            }
        },
    };
})();

export const bigint = {
    Decode(helpers, value_1) {
        if (helpers.isNumber(value_1)) {
            return new FSharpResult$2(/* Ok */ 0, [fromInt32(helpers.asInt(value_1))]);
        }
        else if (helpers.isString(value_1)) {
            let parseResult;
            let outArg = 0n;
            parseResult = [tryParse_5(helpers.asString(value_1), new FSharpRef(() => outArg, (v) => {
                outArg = v;
            })), outArg];
            return parseResult[0] ? (new FSharpResult$2(/* Ok */ 0, [parseResult[1]])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a bigint", value_1])]]));
        }
        else {
            return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a bigint", value_1])]]);
        }
    },
};

export const bool = {
    Decode(helpers, value_1) {
        return helpers.isBoolean(value_1) ? (new FSharpResult$2(/* Ok */ 0, [helpers.asBoolean(value_1)])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a boolean", value_1])]]));
    },
};

export const float = {
    Decode(helpers, value_1) {
        if (helpers.isNumber(value_1)) {
            const rawString = helpers.anyToString(value_1);
            switch (rawString) {
                case "NaN":
                    return new FSharpResult$2(/* Ok */ 0, [NaN]);
                case "Infinity":
                    return new FSharpResult$2(/* Ok */ 0, [Infinity]);
                case "-Infinity":
                    return new FSharpResult$2(/* Ok */ 0, [-Infinity]);
                default: {
                    let matchValue;
                    let outArg = 0;
                    matchValue = [tryParse_6(rawString, new FSharpRef(() => outArg, (v) => {
                        outArg = v;
                    })), outArg];
                    return matchValue[0] ? (new FSharpResult$2(/* Ok */ 0, [matchValue[1]])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a float", value_1])]]));
                }
            }
        }
        else if (helpers.isString(value_1)) {
            const rawString_1 = helpers.asString(value_1);
            switch (rawString_1) {
                case "NaN":
                    return new FSharpResult$2(/* Ok */ 0, [NaN]);
                case "Infinity":
                    return new FSharpResult$2(/* Ok */ 0, [Infinity]);
                case "-Infinity":
                    return new FSharpResult$2(/* Ok */ 0, [-Infinity]);
                default: {
                    let matchValue_1;
                    let outArg_1 = 0;
                    matchValue_1 = [tryParse_6(rawString_1, new FSharpRef(() => outArg_1, (v_1) => {
                        outArg_1 = v_1;
                    })), outArg_1];
                    return matchValue_1[0] ? (new FSharpResult$2(/* Ok */ 0, [matchValue_1[1]])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a float", value_1])]]));
                }
            }
        }
        else {
            return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a float", value_1])]]);
        }
    },
};

export const float32 = {
    Decode(helpers, value_1) {
        if (helpers.isNumber(value_1)) {
            return new FSharpResult$2(/* Ok */ 0, [helpers.asFloat32(value_1)]);
        }
        else if (helpers.isString(value_1)) {
            const matchValue = helpers.asString(value_1);
            return (matchValue === "NaN") ? (new FSharpResult$2(/* Ok */ 0, [NaN])) : ((matchValue === "Infinity") ? (new FSharpResult$2(/* Ok */ 0, [Infinity])) : ((matchValue === "-Infinity") ? (new FSharpResult$2(/* Ok */ 0, [-Infinity])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a float32", value_1])]]))));
        }
        else {
            return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a float32", value_1])]]);
        }
    },
};

export const decimal = {
    Decode(helpers, value_1) {
        if (helpers.isNumber(value_1)) {
            return new FSharpResult$2(/* Ok */ 0, [new Decimal(helpers.asFloat(value_1))]);
        }
        else if (helpers.isString(value_1)) {
            let matchValue;
            let outArg = new Decimal("0");
            matchValue = [tryParse_7(helpers.asString(value_1), new FSharpRef(() => outArg, (v) => {
                outArg = v;
            })), outArg];
            return matchValue[0] ? (new FSharpResult$2(/* Ok */ 0, [matchValue[1]])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a decimal", value_1])]]));
        }
        else {
            return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a decimal", value_1])]]);
        }
    },
};

export const datetimeUtc = {
    Decode(helpers, value_1) {
        if (helpers.isString(value_1)) {
            let matchValue;
            let outArg = minValue();
            matchValue = [tryParse_8(helpers.asString(value_1), new FSharpRef(() => outArg, (v) => {
                outArg = v;
            })), outArg];
            return matchValue[0] ? (new FSharpResult$2(/* Ok */ 0, [toUniversalTime(matchValue[1])])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a datetime", value_1])]]));
        }
        else {
            return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a datetime", value_1])]]);
        }
    },
};

export const datetimeLocal = {
    Decode(helpers, value_1) {
        if (helpers.isString(value_1)) {
            let matchValue;
            let outArg = minValue();
            matchValue = [tryParse_8(helpers.asString(value_1), new FSharpRef(() => outArg, (v) => {
                outArg = v;
            })), outArg];
            return matchValue[0] ? (new FSharpResult$2(/* Ok */ 0, [matchValue[1]])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a datetime", value_1])]]));
        }
        else {
            return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a datetime", value_1])]]);
        }
    },
};

export const datetimeOffset = {
    Decode(helpers, value_1) {
        if (helpers.isString(value_1)) {
            let matchValue;
            let outArg = minValue_1();
            matchValue = [tryParse_9(helpers.asString(value_1), new FSharpRef(() => outArg, (v) => {
                outArg = v;
            })), outArg];
            return matchValue[0] ? (new FSharpResult$2(/* Ok */ 0, [matchValue[1]])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a datetimeoffset", value_1])]]));
        }
        else {
            return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a datetime", value_1])]]);
        }
    },
};

export const timespan = {
    Decode(helpers, value_1) {
        if (helpers.isString(value_1)) {
            let matchValue;
            let outArg = 0;
            matchValue = [tryParse_10(helpers.asString(value_1), new FSharpRef(() => outArg, (v) => {
                outArg = v;
            })), outArg];
            return matchValue[0] ? (new FSharpResult$2(/* Ok */ 0, [matchValue[1]])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a timespan", value_1])]]));
        }
        else {
            return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a timespan", value_1])]]);
        }
    },
};

function decodeMaybeNull(helpers, path, decoder, value_1) {
    let tupledArg;
    if (helpers.isNullValue(value_1)) {
        return new FSharpResult$2(/* Ok */ 0, [undefined]);
    }
    else {
        const matchValue = decoder.Decode(helpers, value_1);
        if (matchValue.tag === 1) {
            return new FSharpResult$2(/* Error */ 1, [(tupledArg = matchValue.fields[0], Helpers_prependPath(path, tupledArg[0], tupledArg[1]))]);
        }
        else {
            return new FSharpResult$2(/* Ok */ 0, [some(matchValue.fields[0])]);
        }
    }
}

/**
 * Decode the value under a property, giving <c>None</c> when the property is missing or
 * <c>null</c>.
 */
export function optional(fieldName, decoder) {
    return {
        Decode(helpers, value_1) {
            return helpers.isObject(value_1) ? (helpers.hasProperty(fieldName, value_1) ? decodeMaybeNull(helpers, "." + fieldName, decoder, helpers.getProperty(fieldName, value_1)) : (new FSharpResult$2(/* Ok */ 0, [undefined]))) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadType */ 2, ["an object", value_1])]]));
        },
    };
}

function badPathError(fieldNames, currentPath, value_1) {
    return new FSharpResult$2(/* Error */ 1, [["." + defaultArg(currentPath, join(".", fieldNames)), new ErrorReason$1(/* BadPath */ 4, [("an object with path `" + join(".", fieldNames)) + "`", value_1, defaultArg(tryLast(fieldNames), "")])]]);
}

/**
 * Decode with two decoders and combine their results.
 */
export function map2(ctor, d1, d2) {
    return {
        Decode(helpers, value_1) {
            const matchValue = d1.Decode(helpers, value_1);
            const matchValue_1 = d2.Decode(helpers, value_1);
            const copyOfStruct = matchValue;
            if (copyOfStruct.tag === 1) {
                return new FSharpResult$2(/* Error */ 1, [copyOfStruct.fields[0]]);
            }
            else {
                const copyOfStruct_1 = matchValue_1;
                return (copyOfStruct_1.tag === 1) ? (new FSharpResult$2(/* Error */ 1, [copyOfStruct_1.fields[0]])) : (new FSharpResult$2(/* Ok */ 0, [ctor(copyOfStruct.fields[0], copyOfStruct_1.fields[0])]));
            }
        },
    };
}

/**
 * Decode the value at a path of property names, giving <c>None</c> when the path is missing or
 * holds <c>null</c>.
 */
export function optionalAt(fieldNames, decoder) {
    return {
        Decode(helpers, firstValue) {
            const _arg = fold((tupledArg, field_1) => {
                const curPath = tupledArg[0];
                const curValue = tupledArg[1];
                const res = tupledArg[2];
                if (res == null) {
                    if (helpers.isNullValue(curValue)) {
                        return [curPath, curValue, new FSharpResult$2(/* Ok */ 0, [undefined])];
                    }
                    else if (helpers.isObject(curValue)) {
                        if (helpers.hasProperty(field_1, curValue)) {
                            return [(curPath + ".") + field_1, helpers.getProperty(field_1, curValue), undefined];
                        }
                        else {
                            return [curPath, curValue, new FSharpResult$2(/* Ok */ 0, [undefined])];
                        }
                    }
                    else {
                        return [curPath, curValue, new FSharpResult$2(/* Error */ 1, [[curPath, new ErrorReason$1(/* BadType */ 2, ["an object", curValue])]])];
                    }
                }
                else {
                    return [curPath, curValue, res];
                }
            }, ["", firstValue, undefined], fieldNames);
            if (_arg[2] == null) {
                const lastValue = _arg[1];
                return helpers.isNullValue(lastValue) ? (new FSharpResult$2(/* Ok */ 0, [undefined])) : decodeMaybeNull(helpers, _arg[0], decoder, lastValue);
            }
            else {
                return _arg[2];
            }
        },
    };
}

/**
 * Decode the value under a property.
 */
export function field(fieldName, decoder) {
    return {
        Decode(helpers, value_1) {
            if (helpers.isObject(value_1)) {
                if (helpers.hasProperty(fieldName, value_1)) {
                    const fieldValue = helpers.getProperty(fieldName, value_1);
                    const path = "." + fieldName;
                    return Result_MapError((tupledArg) => Helpers_prependPath(path, tupledArg[0], tupledArg[1]), decoder.Decode(helpers, fieldValue));
                }
                else {
                    return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadField */ 3, [("an object with a field named `" + fieldName) + "`", value_1])]]);
                }
            }
            else {
                return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadType */ 2, ["an object", value_1])]]);
            }
        },
    };
}

/**
 * Decode the value at a path of property names.
 */
export function at(fieldNames, decoder) {
    return {
        Decode(helpers, firstValue) {
            const _arg = fold((tupledArg, field_1) => {
                const curPath = tupledArg[0];
                const curValue = tupledArg[1];
                const res = tupledArg[2];
                if (res == null) {
                    if (helpers.isNullValue(curValue)) {
                        return [curPath, curValue, badPathError(fieldNames, curPath, firstValue)];
                    }
                    else if (helpers.isObject(curValue)) {
                        if (helpers.hasProperty(field_1, curValue)) {
                            return [(curPath + ".") + field_1, helpers.getProperty(field_1, curValue), undefined];
                        }
                        else {
                            return [curPath, curValue, badPathError(fieldNames, undefined, firstValue)];
                        }
                    }
                    else {
                        return [curPath, curValue, new FSharpResult$2(/* Error */ 1, [[curPath, new ErrorReason$1(/* BadType */ 2, ["an object", curValue])]])];
                    }
                }
                else {
                    return [curPath, curValue, res];
                }
            }, ["", firstValue, undefined], fieldNames);
            return (_arg[2] == null) ? Result_MapError((tupledArg_1) => Helpers_prependPath(_arg[0], tupledArg_1[0], tupledArg_1[1]), decoder.Decode(helpers, _arg[1])) : _arg[2];
        },
    };
}

/**
 * Decode the element of an array at the given position.
 */
export function index(requestedIndex, decoder) {
    return {
        Decode(helpers, value_1) {
            if (helpers.isArray(value_1)) {
                const vArray = helpers.asArray(value_1);
                const path = (".[" + int32ToString(requestedIndex)) + "]";
                return (requestedIndex < vArray.length) ? Result_MapError((tupledArg) => Helpers_prependPath(path, tupledArg[0], tupledArg[1]), decoder.Decode(helpers, item(requestedIndex, vArray))) : (new FSharpResult$2(/* Error */ 1, [[path, new ErrorReason$1(/* TooSmallArray */ 5, [((("a longer array. Need index `" + int32ToString(requestedIndex)) + "` but there are only `") + int32ToString(vArray.length)) + "` entries", value_1])]]));
            }
            else {
                return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["an array", value_1])]]);
            }
        },
    };
}

/**
 * Decode a JSON array into a list.
 */
export function list(decoder) {
    return {
        Decode(helpers, value_1) {
            if (helpers.isArray(value_1)) {
                const tokens = helpers.asArray(value_1);
                let i = 0;
                let result = empty();
                let error = undefined;
                while ((i < tokens.length) && (error == null)) {
                    let tupledArg;
                    const value_2 = item(i, tokens);
                    const matchValue = decoder.Decode(helpers, value_2);
                    if (matchValue.tag === 1) {
                        const x = (tupledArg = matchValue.fields[0], Helpers_prependPath((".[" + int32ToString(i)) + "]", tupledArg[0], tupledArg[1]));
                        error = x;
                    }
                    else {
                        result = append(result, singleton(matchValue.fields[0]));
                    }
                    i = ((i + 1) | 0);
                }
                return (error == null) ? (new FSharpResult$2(/* Ok */ 0, [result])) : (new FSharpResult$2(/* Error */ 1, [value_7(error)]));
            }
            else {
                return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a list", value_1])]]);
            }
        },
    };
}

/**
 * Decode a JSON array into a ResizeArray.
 */
export function resizeArray(decoder) {
    return {
        Decode(helpers, value_1) {
            if (helpers.isArray(value_1)) {
                const tokens = helpers.asArray(value_1);
                let i = 0;
                const result = [];
                let error = undefined;
                while ((i < tokens.length) && (error == null)) {
                    let tupledArg;
                    const value_2 = item(i, tokens);
                    const matchValue = decoder.Decode(helpers, value_2);
                    if (matchValue.tag === 1) {
                        error = ((tupledArg = matchValue.fields[0], Helpers_prependPath((".[" + int32ToString(i)) + "]", tupledArg[0], tupledArg[1])));
                    }
                    else {
                        void (result.push(matchValue.fields[0]));
                    }
                    i = ((i + 1) | 0);
                }
                return (error == null) ? (new FSharpResult$2(/* Ok */ 0, [Array.from(result)])) : (new FSharpResult$2(/* Error */ 1, [value_7(error)]));
            }
            else {
                return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a ResizeArray", value_1])]]);
            }
        },
    };
}

/**
 * Decode a JSON array into a sequence.
 */
export function seq(decoder) {
    return {
        Decode(helpers, value_1) {
            if (helpers.isArray(value_1)) {
                let i = -1;
                return Result_Map(reverse, fold_1((acc, value_2) => {
                    let tupledArg;
                    i = ((i + 1) | 0);
                    if (acc.tag === 0) {
                        const matchValue = decoder.Decode(helpers, value_2);
                        if (matchValue.tag === 0) {
                            return new FSharpResult$2(/* Ok */ 0, [append_1([matchValue.fields[0]], acc.fields[0])]);
                        }
                        else {
                            return new FSharpResult$2(/* Error */ 1, [(tupledArg = matchValue.fields[0], Helpers_prependPath((".[" + int32ToString(i)) + "]", tupledArg[0], tupledArg[1]))]);
                        }
                    }
                    else {
                        return acc;
                    }
                }, new FSharpResult$2(/* Ok */ 0, [[]]), helpers.asArray(value_1)));
            }
            else {
                return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["a seq", value_1])]]);
            }
        },
    };
}

/**
 * Decode a JSON array into an array.
 */
export function array(decoder) {
    return {
        Decode(helpers, value_1) {
            if (helpers.isArray(value_1)) {
                let i = -1;
                const tokens = helpers.asArray(value_1);
                return fold_1((acc, value_2) => {
                    let tupledArg;
                    i = ((i + 1) | 0);
                    if (acc.tag === 0) {
                        const acc_1 = acc.fields[0];
                        const matchValue = decoder.Decode(helpers, value_2);
                        if (matchValue.tag === 0) {
                            setItem(acc_1, i, matchValue.fields[0]);
                            return new FSharpResult$2(/* Ok */ 0, [acc_1]);
                        }
                        else {
                            return new FSharpResult$2(/* Error */ 1, [(tupledArg = matchValue.fields[0], Helpers_prependPath((".[" + int32ToString(i)) + "]", tupledArg[0], tupledArg[1]))]);
                        }
                    }
                    else {
                        return acc;
                    }
                }, new FSharpResult$2(/* Ok */ 0, [fill(new Array(tokens.length), 0, tokens.length, null)]), tokens);
            }
            else {
                return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["an array", value_1])]]);
            }
        },
    };
}

export const keys = {
    Decode(helpers, value_1) {
        return helpers.isObject(value_1) ? (new FSharpResult$2(/* Ok */ 0, [ofSeq(helpers.getProperties(value_1))])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["an object", value_1])]]));
    },
};

/**
 * Decode every property of an object, giving its name paired with its decoded value.
 */
export function keyValuePairs(decoder) {
    return {
        Decode(helpers, value_1) {
            const matchValue = keys.Decode(helpers, value_1);
            return (matchValue.tag === 1) ? (new FSharpResult$2(/* Error */ 1, [matchValue.fields[0]])) : Result_Map(reverse_1, fold((acc, prop) => {
                if (acc.tag === 0) {
                    const fieldValue = helpers.getProperty(prop, value_1);
                    const matchValue_1 = decoder.Decode(helpers, fieldValue);
                    if (matchValue_1.tag === 0) {
                        return new FSharpResult$2(/* Ok */ 0, [cons([prop, matchValue_1.fields[0]], acc.fields[0])]);
                    }
                    else {
                        const er = matchValue_1.fields[0];
                        return new FSharpResult$2(/* Error */ 1, [Helpers_prependPath("." + prop, er[0], er[1])]);
                    }
                }
                else {
                    return acc;
                }
            }, new FSharpResult$2(/* Ok */ 0, [empty()]), matchValue.fields[0]));
        },
    };
}

/**
 * Decode with the first decoder of the list that succeeds.
 */
export function oneOf(decoders) {
    return {
        Decode(helpers, value_1) {
            const runner = (decoders_1_mut, errors_mut) => {
                runner:
                while (true) {
                    const decoders_1 = decoders_1_mut, errors = errors_mut;
                    if (isEmpty(decoders_1)) {
                        return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadOneOf */ 7, [errors])]]);
                    }
                    else {
                        const matchValue = head_1(decoders_1).Decode(helpers, value_1);
                        if (matchValue.tag === 1) {
                            decoders_1_mut = tail_1(decoders_1);
                            errors_mut = append(errors, singleton(matchValue.fields[0]));
                            continue runner;
                        }
                        else {
                            return new FSharpResult$2(/* Ok */ 0, [matchValue.fields[0]]);
                        }
                    }
                    break;
                }
            };
            return runner(decoders, empty());
        },
    };
}

/**
 * Decode <c>null</c> into the given value, and fail on anything else.
 */
export function nil(output) {
    return {
        Decode(helpers, value_1) {
            return helpers.isNullValue(value_1) ? (new FSharpResult$2(/* Ok */ 0, [output])) : (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["null", value_1])]]));
        },
    };
}

class ValueDecoder {
    constructor() {
    }
    Decode(helpers, value_1) {
        const this$ = this;
        const decoder = this$;
        if (helpers.isBoolean(value_1)) {
            return new FSharpResult$2(/* Ok */ 0, [new Json(/* Boolean */ 3, [helpers.asBoolean(value_1)])]);
        }
        else if (helpers.isNullValue(value_1)) {
            return new FSharpResult$2(/* Ok */ 0, [Json.Null]);
        }
        else if (helpers.isString(value_1)) {
            return new FSharpResult$2(/* Ok */ 0, [new Json(/* String */ 0, [helpers.asString(value_1)])]);
        }
        else if (helpers.isNumber(value_1)) {
            return new FSharpResult$2(/* Ok */ 0, [new Json(/* Number */ 1, [helpers.asFloat(value_1)])]);
        }
        else if (helpers.isArray(value_1)) {
            const tokens = helpers.asArray(value_1);
            const result = fill(new Array(tokens.length), 0, tokens.length, null);
            let i = 0;
            let error = undefined;
            while ((i < tokens.length) && (error == null)) {
                let tupledArg;
                const value_2 = item(i, tokens);
                const matchValue = decoder.Decode(helpers, value_2);
                if (matchValue.tag === 1) {
                    const x = (tupledArg = matchValue.fields[0], Helpers_prependPath(`.[${i}]`, tupledArg[0], tupledArg[1]));
                    error = x;
                }
                else {
                    setItem(result, i, matchValue.fields[0]);
                }
                i = ((i + 1) | 0);
            }
            return (error == null) ? (new FSharpResult$2(/* Ok */ 0, [new Json(/* Array */ 5, [toList(result)])])) : (new FSharpResult$2(/* Error */ 1, [value_7(error)]));
        }
        else if (helpers.isObject(value_1)) {
            const props = toArray(helpers.getProperties(value_1));
            const result_1 = fill(new Array(props.length), 0, props.length, null);
            let i_1 = 0;
            let error_1 = undefined;
            while ((i_1 < props.length) && (error_1 == null)) {
                let tupledArg_1;
                const key = item(i_1, props);
                const value_4 = helpers.getProperty(key, value_1);
                const matchValue_1 = decoder.Decode(helpers, value_4);
                if (matchValue_1.tag === 1) {
                    const x_1 = (tupledArg_1 = matchValue_1.fields[0], Helpers_prependPath("." + key, tupledArg_1[0], tupledArg_1[1]));
                    error_1 = x_1;
                }
                else {
                    setItem(result_1, i_1, [key, matchValue_1.fields[0]]);
                }
                i_1 = ((i_1 + 1) | 0);
            }
            return (error_1 == null) ? (new FSharpResult$2(/* Ok */ 0, [new Json(/* Object */ 4, [toList(result_1)])])) : (new FSharpResult$2(/* Error */ 1, [value_7(error_1)]));
        }
        else {
            return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitive */ 0, ["any", value_1])]]);
        }
    }
}

function ValueDecoder_$reflection() {
    return class_type("Thoth.Json.Core.Decode.ValueDecoder", undefined, ValueDecoder);
}

function ValueDecoder_$ctor() {
    return new ValueDecoder();
}

export const value = ValueDecoder_$ctor();

/**
 * Always succeed with the given value, whatever the JSON.
 */
export function succeed(output) {
    return {
        Decode(_arg, _arg_1) {
            return new FSharpResult$2(/* Ok */ 0, [output]);
        },
    };
}

/**
 * Always fail with the given message.
 */
export function fail(msg) {
    return {
        Decode(_arg, _arg_1) {
            return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* FailMessage */ 6, [msg])]]);
        },
    };
}

/**
 * Use the result of a decoder to choose the next one.
 */
export function andThen(cb, decoder) {
    return {
        Decode(helpers, value_1) {
            const matchValue = decoder.Decode(helpers, value_1);
            return (matchValue.tag === 0) ? cb(matchValue.fields[0]).Decode(helpers, value_1) : (new FSharpResult$2(/* Error */ 1, [matchValue.fields[0]]));
        },
    };
}

/**
 * Run every decoder against the same value, and collect their results.
 */
export function all(decoders) {
    return {
        Decode(helpers, value_1) {
            const runner = (decoders_1_mut, values_mut) => {
                runner:
                while (true) {
                    const decoders_1 = decoders_1_mut, values = values_mut;
                    if (isEmpty(decoders_1)) {
                        return new FSharpResult$2(/* Ok */ 0, [values]);
                    }
                    else {
                        const matchValue = head_1(decoders_1).Decode(helpers, value_1);
                        if (matchValue.tag === 1) {
                            return new FSharpResult$2(/* Error */ 1, [matchValue.fields[0]]);
                        }
                        else {
                            decoders_1_mut = tail_1(decoders_1);
                            values_mut = append(values, singleton(matchValue.fields[0]));
                            continue runner;
                        }
                    }
                    break;
                }
            };
            return runner(decoders, empty());
        },
    };
}

class LazyDecoder$1 extends Record {
    constructor(x) {
        super();
        this.x = x;
    }
    Decode(helpers, json) {
        const this$ = this;
        const decoder = this$.x.Value;
        return decoder.Decode(helpers, json);
    }
}

function LazyDecoder$1_$reflection(gen0) {
    return class_type("Thoth.Json.Core.Decode.LazyDecoder`1", [gen0], LazyDecoder$1, class_type("System.ValueType"));
}

function LazyDecoder$1_$ctor_Z67290E74(x) {
    return new LazyDecoder$1(x);
}

/**
 * Defer the construction of a decoder until it is first used, for mutually recursive types.
 */
export function lazily(x) {
    return LazyDecoder$1_$ctor_Z67290E74(x);
}

/**
 * Transform the result of a decoder.
 */
export function map(ctor, d1) {
    return {
        Decode(helpers, value_1) {
            const matchValue = d1.Decode(helpers, value_1);
            return (matchValue.tag === 1) ? (new FSharpResult$2(/* Error */ 1, [matchValue.fields[0]])) : (new FSharpResult$2(/* Ok */ 0, [ctor(matchValue.fields[0])]));
        },
    };
}

/**
 * Decode with three decoders and combine their results.
 */
export function map3(ctor, d1, d2, d3) {
    return {
        Decode(helpers, value_1) {
            const matchValue = d1.Decode(helpers, value_1);
            const matchValue_1 = d2.Decode(helpers, value_1);
            const matchValue_2 = d3.Decode(helpers, value_1);
            const copyOfStruct = matchValue;
            if (copyOfStruct.tag === 1) {
                return new FSharpResult$2(/* Error */ 1, [copyOfStruct.fields[0]]);
            }
            else {
                const copyOfStruct_1 = matchValue_1;
                if (copyOfStruct_1.tag === 1) {
                    return new FSharpResult$2(/* Error */ 1, [copyOfStruct_1.fields[0]]);
                }
                else {
                    const copyOfStruct_2 = matchValue_2;
                    return (copyOfStruct_2.tag === 1) ? (new FSharpResult$2(/* Error */ 1, [copyOfStruct_2.fields[0]])) : (new FSharpResult$2(/* Ok */ 0, [ctor(copyOfStruct.fields[0], copyOfStruct_1.fields[0], copyOfStruct_2.fields[0])]));
                }
            }
        },
    };
}

/**
 * Decode with four decoders and combine their results.
 */
export function map4(ctor, d1, d2, d3, d4) {
    return {
        Decode(helpers, value_1) {
            const matchValue = d1.Decode(helpers, value_1);
            const matchValue_1 = d2.Decode(helpers, value_1);
            const matchValue_2 = d3.Decode(helpers, value_1);
            const matchValue_3 = d4.Decode(helpers, value_1);
            const copyOfStruct = matchValue;
            if (copyOfStruct.tag === 1) {
                return new FSharpResult$2(/* Error */ 1, [copyOfStruct.fields[0]]);
            }
            else {
                const copyOfStruct_1 = matchValue_1;
                if (copyOfStruct_1.tag === 1) {
                    return new FSharpResult$2(/* Error */ 1, [copyOfStruct_1.fields[0]]);
                }
                else {
                    const copyOfStruct_2 = matchValue_2;
                    if (copyOfStruct_2.tag === 1) {
                        return new FSharpResult$2(/* Error */ 1, [copyOfStruct_2.fields[0]]);
                    }
                    else {
                        const copyOfStruct_3 = matchValue_3;
                        return (copyOfStruct_3.tag === 1) ? (new FSharpResult$2(/* Error */ 1, [copyOfStruct_3.fields[0]])) : (new FSharpResult$2(/* Ok */ 0, [ctor(copyOfStruct.fields[0], copyOfStruct_1.fields[0], copyOfStruct_2.fields[0], copyOfStruct_3.fields[0])]));
                    }
                }
            }
        },
    };
}

/**
 * Decode with five decoders and combine their results.
 */
export function map5(ctor, d1, d2, d3, d4, d5) {
    return {
        Decode(helpers, value_1) {
            const matchValue = d1.Decode(helpers, value_1);
            const matchValue_1 = d2.Decode(helpers, value_1);
            const matchValue_2 = d3.Decode(helpers, value_1);
            const matchValue_3 = d4.Decode(helpers, value_1);
            const matchValue_4 = d5.Decode(helpers, value_1);
            const copyOfStruct = matchValue;
            if (copyOfStruct.tag === 1) {
                return new FSharpResult$2(/* Error */ 1, [copyOfStruct.fields[0]]);
            }
            else {
                const copyOfStruct_1 = matchValue_1;
                if (copyOfStruct_1.tag === 1) {
                    return new FSharpResult$2(/* Error */ 1, [copyOfStruct_1.fields[0]]);
                }
                else {
                    const copyOfStruct_2 = matchValue_2;
                    if (copyOfStruct_2.tag === 1) {
                        return new FSharpResult$2(/* Error */ 1, [copyOfStruct_2.fields[0]]);
                    }
                    else {
                        const copyOfStruct_3 = matchValue_3;
                        if (copyOfStruct_3.tag === 1) {
                            return new FSharpResult$2(/* Error */ 1, [copyOfStruct_3.fields[0]]);
                        }
                        else {
                            const copyOfStruct_4 = matchValue_4;
                            return (copyOfStruct_4.tag === 1) ? (new FSharpResult$2(/* Error */ 1, [copyOfStruct_4.fields[0]])) : (new FSharpResult$2(/* Ok */ 0, [ctor(copyOfStruct.fields[0], copyOfStruct_1.fields[0], copyOfStruct_2.fields[0], copyOfStruct_3.fields[0], copyOfStruct_4.fields[0])]));
                        }
                    }
                }
            }
        },
    };
}

/**
 * Decode with six decoders and combine their results.
 */
export function map6(ctor, d1, d2, d3, d4, d5, d6) {
    return {
        Decode(helpers, value_1) {
            const matchValue = d1.Decode(helpers, value_1);
            const matchValue_1 = d2.Decode(helpers, value_1);
            const matchValue_2 = d3.Decode(helpers, value_1);
            const matchValue_3 = d4.Decode(helpers, value_1);
            const matchValue_4 = d5.Decode(helpers, value_1);
            const matchValue_5 = d6.Decode(helpers, value_1);
            const copyOfStruct = matchValue;
            if (copyOfStruct.tag === 1) {
                return new FSharpResult$2(/* Error */ 1, [copyOfStruct.fields[0]]);
            }
            else {
                const copyOfStruct_1 = matchValue_1;
                if (copyOfStruct_1.tag === 1) {
                    return new FSharpResult$2(/* Error */ 1, [copyOfStruct_1.fields[0]]);
                }
                else {
                    const copyOfStruct_2 = matchValue_2;
                    if (copyOfStruct_2.tag === 1) {
                        return new FSharpResult$2(/* Error */ 1, [copyOfStruct_2.fields[0]]);
                    }
                    else {
                        const copyOfStruct_3 = matchValue_3;
                        if (copyOfStruct_3.tag === 1) {
                            return new FSharpResult$2(/* Error */ 1, [copyOfStruct_3.fields[0]]);
                        }
                        else {
                            const copyOfStruct_4 = matchValue_4;
                            if (copyOfStruct_4.tag === 1) {
                                return new FSharpResult$2(/* Error */ 1, [copyOfStruct_4.fields[0]]);
                            }
                            else {
                                const copyOfStruct_5 = matchValue_5;
                                return (copyOfStruct_5.tag === 1) ? (new FSharpResult$2(/* Error */ 1, [copyOfStruct_5.fields[0]])) : (new FSharpResult$2(/* Ok */ 0, [ctor(copyOfStruct.fields[0], copyOfStruct_1.fields[0], copyOfStruct_2.fields[0], copyOfStruct_3.fields[0], copyOfStruct_4.fields[0], copyOfStruct_5.fields[0])]));
                            }
                        }
                    }
                }
            }
        },
    };
}

/**
 * Decode with seven decoders and combine their results.
 */
export function map7(ctor, d1, d2, d3, d4, d5, d6, d7) {
    return {
        Decode(helpers, value_1) {
            const matchValue = d1.Decode(helpers, value_1);
            const matchValue_1 = d2.Decode(helpers, value_1);
            const matchValue_2 = d3.Decode(helpers, value_1);
            const matchValue_3 = d4.Decode(helpers, value_1);
            const matchValue_4 = d5.Decode(helpers, value_1);
            const matchValue_5 = d6.Decode(helpers, value_1);
            const matchValue_6 = d7.Decode(helpers, value_1);
            const copyOfStruct = matchValue;
            if (copyOfStruct.tag === 1) {
                return new FSharpResult$2(/* Error */ 1, [copyOfStruct.fields[0]]);
            }
            else {
                const copyOfStruct_1 = matchValue_1;
                if (copyOfStruct_1.tag === 1) {
                    return new FSharpResult$2(/* Error */ 1, [copyOfStruct_1.fields[0]]);
                }
                else {
                    const copyOfStruct_2 = matchValue_2;
                    if (copyOfStruct_2.tag === 1) {
                        return new FSharpResult$2(/* Error */ 1, [copyOfStruct_2.fields[0]]);
                    }
                    else {
                        const copyOfStruct_3 = matchValue_3;
                        if (copyOfStruct_3.tag === 1) {
                            return new FSharpResult$2(/* Error */ 1, [copyOfStruct_3.fields[0]]);
                        }
                        else {
                            const copyOfStruct_4 = matchValue_4;
                            if (copyOfStruct_4.tag === 1) {
                                return new FSharpResult$2(/* Error */ 1, [copyOfStruct_4.fields[0]]);
                            }
                            else {
                                const copyOfStruct_5 = matchValue_5;
                                if (copyOfStruct_5.tag === 1) {
                                    return new FSharpResult$2(/* Error */ 1, [copyOfStruct_5.fields[0]]);
                                }
                                else {
                                    const copyOfStruct_6 = matchValue_6;
                                    return (copyOfStruct_6.tag === 1) ? (new FSharpResult$2(/* Error */ 1, [copyOfStruct_6.fields[0]])) : (new FSharpResult$2(/* Ok */ 0, [ctor(copyOfStruct.fields[0], copyOfStruct_1.fields[0], copyOfStruct_2.fields[0], copyOfStruct_3.fields[0], copyOfStruct_4.fields[0], copyOfStruct_5.fields[0], copyOfStruct_6.fields[0])]));
                                }
                            }
                        }
                    }
                }
            }
        },
    };
}

/**
 * Decode with eight decoders and combine their results.
 */
export function map8(ctor, d1, d2, d3, d4, d5, d6, d7, d8) {
    return {
        Decode(helpers, value_1) {
            const matchValue = d1.Decode(helpers, value_1);
            const matchValue_1 = d2.Decode(helpers, value_1);
            const matchValue_2 = d3.Decode(helpers, value_1);
            const matchValue_3 = d4.Decode(helpers, value_1);
            const matchValue_4 = d5.Decode(helpers, value_1);
            const matchValue_5 = d6.Decode(helpers, value_1);
            const matchValue_6 = d7.Decode(helpers, value_1);
            const matchValue_7 = d8.Decode(helpers, value_1);
            const copyOfStruct = matchValue;
            if (copyOfStruct.tag === 1) {
                return new FSharpResult$2(/* Error */ 1, [copyOfStruct.fields[0]]);
            }
            else {
                const copyOfStruct_1 = matchValue_1;
                if (copyOfStruct_1.tag === 1) {
                    return new FSharpResult$2(/* Error */ 1, [copyOfStruct_1.fields[0]]);
                }
                else {
                    const copyOfStruct_2 = matchValue_2;
                    if (copyOfStruct_2.tag === 1) {
                        return new FSharpResult$2(/* Error */ 1, [copyOfStruct_2.fields[0]]);
                    }
                    else {
                        const copyOfStruct_3 = matchValue_3;
                        if (copyOfStruct_3.tag === 1) {
                            return new FSharpResult$2(/* Error */ 1, [copyOfStruct_3.fields[0]]);
                        }
                        else {
                            const copyOfStruct_4 = matchValue_4;
                            if (copyOfStruct_4.tag === 1) {
                                return new FSharpResult$2(/* Error */ 1, [copyOfStruct_4.fields[0]]);
                            }
                            else {
                                const copyOfStruct_5 = matchValue_5;
                                if (copyOfStruct_5.tag === 1) {
                                    return new FSharpResult$2(/* Error */ 1, [copyOfStruct_5.fields[0]]);
                                }
                                else {
                                    const copyOfStruct_6 = matchValue_6;
                                    if (copyOfStruct_6.tag === 1) {
                                        return new FSharpResult$2(/* Error */ 1, [copyOfStruct_6.fields[0]]);
                                    }
                                    else {
                                        const copyOfStruct_7 = matchValue_7;
                                        return (copyOfStruct_7.tag === 1) ? (new FSharpResult$2(/* Error */ 1, [copyOfStruct_7.fields[0]])) : (new FSharpResult$2(/* Ok */ 0, [ctor(copyOfStruct.fields[0], copyOfStruct_1.fields[0], copyOfStruct_2.fields[0], copyOfStruct_3.fields[0], copyOfStruct_4.fields[0], copyOfStruct_5.fields[0], copyOfStruct_6.fields[0], copyOfStruct_7.fields[0])]));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        },
    };
}

/**
 * Decode <c>null</c> into <c>None</c>, and anything else with the given decoder.
 */
export function lossyOption(decoder) {
    return {
        Decode(helpers, value_1) {
            return helpers.isNullValue(value_1) ? (new FSharpResult$2(/* Ok */ 0, [undefined])) : Result_Map(some, decoder.Decode(helpers, value_1));
        },
    };
}

/**
 * Decode the object written by <see cref="M:Thoth.Json.Core.Encode.losslessOption"/> into an
 * option.
 */
export function losslessOption(decoder) {
    return andThen((typeName) => {
        if (typeName === "option") {
            return andThen((state) => {
                switch (state) {
                    case "none":
                        return succeed(undefined);
                    case "some":
                        return map(some, field("$value", decoder));
                    default:
                        return fail("Expecting a state field with value \'none\' or \'some\' but got " + state);
                }
            }, field("$case", string));
        }
        else {
            return fail("Expecting an Option type but got " + typeName);
        }
    }, field("$type", string));
}

/**
 * Apply one decoded value to a decoded function, for building a value field by field.
 */
export function andMap() {
    return (d) => ((d_1) => map2((arg, func) => func(arg), d, d_1));
}

function unwrapWith(errors, helpers, decoder, value_1) {
    const matchValue = decoder.Decode(helpers, value_1);
    if (matchValue.tag === 1) {
        void (errors.push(matchValue.fields[0]));
        return defaultOf();
    }
    else {
        return matchValue.fields[0];
    }
}

export class Getters$2 {
    constructor(helpers, value_1) {
        let _this, _this_1;
        this.errors = [];
        this.required = ((_this = this, {
            Field(fieldName, decoder) {
                return unwrapWith(_this.errors, helpers, field(fieldName, decoder), value_1);
            },
            At(fieldNames, decoder_1) {
                return unwrapWith(_this.errors, helpers, at(fieldNames, decoder_1), value_1);
            },
            Raw(decoder_2) {
                return unwrapWith(_this.errors, helpers, decoder_2, value_1);
            },
        }));
        this.optional = ((_this_1 = this, {
            Field(fieldName_1, decoder_3) {
                return unwrapWith(_this_1.errors, helpers, optional(fieldName_1, decoder_3), value_1);
            },
            At(fieldNames_1, decoder_4) {
                return unwrapWith(_this_1.errors, helpers, optionalAt(fieldNames_1, decoder_4), value_1);
            },
            Raw(decoder_5) {
                const matchValue = decoder_5.Decode(helpers, value_1);
                if (matchValue.tag === 1) {
                    const reason = matchValue.fields[0][1];
                    const error = matchValue.fields[0];
                    let matchResult, v_1;
                    switch (reason.tag) {
                        case 3:
                        case 4: {
                            matchResult = 1;
                            break;
                        }
                        case 5:
                        case 6:
                        case 7: {
                            matchResult = 2;
                            break;
                        }
                        case 1: {
                            matchResult = 0;
                            v_1 = reason.fields[1];
                            break;
                        }
                        case 2: {
                            matchResult = 0;
                            v_1 = reason.fields[1];
                            break;
                        }
                        default: {
                            matchResult = 0;
                            v_1 = reason.fields[1];
                        }
                    }
                    switch (matchResult) {
                        case 0:
                            if (helpers.isNullValue(v_1)) {
                                return undefined;
                            }
                            else {
                                void (_this_1.errors.push(error));
                                return defaultOf();
                            }
                        case 1:
                            return undefined;
                        default: {
                            void (_this_1.errors.push(error));
                            return defaultOf();
                        }
                    }
                }
                else {
                    return some(matchValue.fields[0]);
                }
            },
        }));
    }
    get Required() {
        const __ = this;
        return __.required;
    }
    get Optional() {
        const __ = this;
        return __.optional;
    }
}

export function Getters$2_$reflection(gen0, gen1) {
    return class_type("Thoth.Json.Core.Decode.Getters`2", [gen0, gen1], Getters$2);
}

export function Getters$2_$ctor_Z4BE6C149(helpers, value_1) {
    return new Getters$2(helpers, value_1);
}

export function Getters$2__get_Errors(__) {
    return toList(__.errors);
}

/**
 * Decode an object, reading each property through <c>get.Required</c> or <c>get.Optional</c>.
 */
export function object(builder) {
    return {
        Decode(helpers, value_1) {
            const getters = Getters$2_$ctor_Z4BE6C149(helpers, value_1);
            const result = builder(getters);
            const matchValue = Getters$2__get_Errors(getters);
            if (!isEmpty(matchValue)) {
                const errors = matchValue;
                return (length(errors) > 1) ? (new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadOneOf */ 7, [errors])]])) : (new FSharpResult$2(/* Error */ 1, [head_1(matchValue)]));
            }
            else {
                return new FSharpResult$2(/* Ok */ 0, [result]);
            }
        },
    };
}

/**
 * Decode a JSON array of 2 elements into a tuple.
 */
export function tuple2(decoder1, decoder2) {
    return andThen((v1) => andThen((v2) => succeed([v1, v2]), index(1, decoder2)), index(0, decoder1));
}

/**
 * Decode a JSON array of 3 elements into a tuple.
 */
export function tuple3(decoder1, decoder2, decoder3) {
    return andThen((v1) => andThen((v2) => andThen((v3) => succeed([v1, v2, v3]), index(2, decoder3)), index(1, decoder2)), index(0, decoder1));
}

/**
 * Decode a JSON array of 4 elements into a tuple.
 */
export function tuple4(decoder1, decoder2, decoder3, decoder4) {
    return andThen((v1) => andThen((v2) => andThen((v3) => andThen((v4) => succeed([v1, v2, v3, v4]), index(3, decoder4)), index(2, decoder3)), index(1, decoder2)), index(0, decoder1));
}

/**
 * Decode a JSON array of 5 elements into a tuple.
 */
export function tuple5(decoder1, decoder2, decoder3, decoder4, decoder5) {
    return andThen((v1) => andThen((v2) => andThen((v3) => andThen((v4) => andThen((v5) => succeed([v1, v2, v3, v4, v5]), index(4, decoder5)), index(3, decoder4)), index(2, decoder3)), index(1, decoder2)), index(0, decoder1));
}

/**
 * Decode a JSON array of 6 elements into a tuple.
 */
export function tuple6(decoder1, decoder2, decoder3, decoder4, decoder5, decoder6) {
    return andThen((v1) => andThen((v2) => andThen((v3) => andThen((v4) => andThen((v5) => andThen((v6) => succeed([v1, v2, v3, v4, v5, v6]), index(5, decoder6)), index(4, decoder5)), index(3, decoder4)), index(2, decoder3)), index(1, decoder2)), index(0, decoder1));
}

/**
 * Decode a JSON array of 7 elements into a tuple.
 */
export function tuple7(decoder1, decoder2, decoder3, decoder4, decoder5, decoder6, decoder7) {
    return andThen((v1) => andThen((v2) => andThen((v3) => andThen((v4) => andThen((v5) => andThen((v6) => andThen((v7) => succeed([v1, v2, v3, v4, v5, v6, v7]), index(6, decoder7)), index(5, decoder6)), index(4, decoder5)), index(3, decoder4)), index(2, decoder3)), index(1, decoder2)), index(0, decoder1));
}

/**
 * Decode a JSON array of 8 elements into a tuple.
 */
export function tuple8(decoder1, decoder2, decoder3, decoder4, decoder5, decoder6, decoder7, decoder8) {
    return andThen((v1) => andThen((v2) => andThen((v3) => andThen((v4) => andThen((v5) => andThen((v6) => andThen((v7) => andThen((v8) => succeed([v1, v2, v3, v4, v5, v6, v7, v8]), index(7, decoder8)), index(6, decoder7)), index(5, decoder6)), index(4, decoder5)), index(3, decoder4)), index(2, decoder3)), index(1, decoder2)), index(0, decoder1));
}

/**
 * Decode an object into a map keyed by the property names.
 */
export function dict(decoder) {
    return map((elements) => ofList(elements, {
        Compare: (x, y) => (comparePrimitives(x, y) | 0),
    }), keyValuePairs(decoder));
}

/**
 * Decode an array of <c>[ key, value ]</c> pairs into a map. Use it when the key is not a string.
 */
export function map$0027(keyDecoder, valueDecoder) {
    return map((elements) => ofSeq_1(elements, {
        Compare: (x, y) => (compare(x, y) | 0),
    }), array(tuple2(keyDecoder, valueDecoder)));
}

class FixDecoder$1 {
    constructor(make) {
        const this$ = new FSharpRef(defaultOf());
        this$.contents = this;
        this.self = make(this$.contents);
        this["init@1673"] = 1;
    }
    Decode(helpers, value_1) {
        const this$ = this;
        return this$.self.Decode(helpers, value_1);
    }
}

function FixDecoder$1_$reflection(gen0) {
    return class_type("Thoth.Json.Core.Decode.FixDecoder`1", [gen0], FixDecoder$1);
}

function FixDecoder$1_$ctor_Z2A44CE2A(make) {
    return new FixDecoder$1(make);
}

/**
 * Allow to build a decoder that can call itself
 */
export function fix(make) {
    return FixDecoder$1_$ctor_Z2A44CE2A(make);
}

/**
 * Turn <c>None</c> into a failure carrying the given message.
 */
export function requireSome(errorMessage, decoder) {
    return andThen((_arg) => {
        if (_arg == null) {
            return fail(errorMessage);
        }
        else {
            return succeed(value_7(_arg));
        }
    }, decoder);
}

/**
 * Turn a decoder of an option into one which fails on <c>None</c>.
 */
export function notNone(decoder) {
    return andThen((_arg) => {
        if (_arg == null) {
            return fail("Expecting a value but instead got: None");
        }
        else {
            return succeed(value_7(_arg));
        }
    }, decoder);
}

/**
 * The decoder half of a codec.
 */
export function codec(c) {
    return c.Decoder;
}

