
import { Operators_IsNull } from "fable-library-js/FSharp.Core.js";
import { class_type } from "fable-library-js/Reflection.js";
import { errorToString, Helpers_prependPath, codec as codec_2, Advanced_fromValue } from "../Thoth.Json.Core/Decode.js";
import { FSharpResult$2 } from "fable-library-js/Result.js";
import { Exception } from "fable-library-js/Util.js";

export const DecodeModule_helpers = {
    isString(jsonValue) {
        return typeof jsonValue === "string";
    },
    isNumber(jsonValue_1) {
        return (typeof jsonValue_1) === "number";
    },
    isBoolean(jsonValue_2) {
        return typeof jsonValue_2 === "boolean";
    },
    isNullValue(jsonValue_3) {
        return Operators_IsNull(jsonValue_3);
    },
    isArray(jsonValue_4) {
        return Array.isArray(jsonValue_4);
    },
    isObject(jsonValue_5) {
        return jsonValue_5 === null ? false : (Object.getPrototypeOf(jsonValue_5 || false) === Object.prototype)
                ;
    },
    hasProperty(fieldName, jsonValue_6) {
        return jsonValue_6.hasOwnProperty(fieldName);
                    ;
    },
    isIntegralValue(jsonValue_7) {
        return isFinite(jsonValue_7) && Math.floor(jsonValue_7) === jsonValue_7
                    ;
    },
    asString(jsonValue_8) {
        return jsonValue_8;
    },
    asBoolean(jsonValue_9) {
        return jsonValue_9;
    },
    asArray(jsonValue_10) {
        return jsonValue_10;
    },
    asFloat(jsonValue_11) {
        return jsonValue_11;
    },
    asFloat32(jsonValue_12) {
        return jsonValue_12;
    },
    asInt(jsonValue_13) {
        return jsonValue_13 | 0;
    },
    getProperties(jsonValue_14) {
        return Object.keys(jsonValue_14);
    },
    getProperty(fieldName_1, jsonValue_15) {
        return jsonValue_15[fieldName_1];
    },
    anyToString(jsonValue_16) {
        return JSON.stringify(jsonValue_16, null, 4) + ''
                    ;
    },
    numberToString(jsonValue_17) {
        return String(jsonValue_17)
                    ;
    },
};

/**
 * Runs a decoder against JavaScript.
 */
export class Decode {
    constructor() {
    }
}

export function Decode_$reflection() {
    return class_type("Thoth.Json.JavaScript.Decode", undefined, Decode);
}

/**
 * Run a decoder against a <c>obj</c> which is already parsed.
 */
export function Decode_fromValue_1FBE35A8(decoder) {
    return (value) => Advanced_fromValue(DecodeModule_helpers, decoder, value);
}

/**
 * Run the decoder half of a codec against a <c>obj</c> which is already parsed.
 */
export function Decode_fromValue_Z2BB4EEE8(codec) {
    return Decode_fromValue_1FBE35A8(codec_2(codec));
}

/**
 * Parse a JSON string and run the decoder against it.
 */
export function Decode_fromString_1FBE35A8(decoder) {
    return (value) => {
        try {
            const json = JSON.parse(value);
            const matchValue = decoder.Decode(DecodeModule_helpers, json);
            if (matchValue.tag === 1) {
                let finalError;
                const tupledArg = matchValue.fields[0];
                finalError = Helpers_prependPath("$", tupledArg[0], tupledArg[1]);
                return new FSharpResult$2(/* Error */ 1, [errorToString(DecodeModule_helpers, finalError[0], finalError[1])]);
            }
            else {
                return new FSharpResult$2(/* Ok */ 0, [matchValue.fields[0]]);
            }
        }
        catch (matchValue_1) {
            if (matchValue_1 instanceof SyntaxError) {
                return new FSharpResult$2(/* Error */ 1, ["Given an invalid JSON: " + matchValue_1.message]);
            }
            else {
                throw matchValue_1;
            }
        }
    };
}

/**
 * Parse a JSON string and run the decoder half of a codec against it.
 */
export function Decode_fromString_Z2BB4EEE8(codec) {
    return Decode_fromString_1FBE35A8(codec_2(codec));
}

/**
 * Parse a JSON string and run the decoder, raising an exception carrying the message on
 * failure.
 */
export function Decode_unsafeFromString_1FBE35A8(decoder) {
    return (value) => {
        const matchValue = Decode_fromString_1FBE35A8(decoder)(value);
        if (matchValue.tag === 1) {
            throw new Exception(matchValue.fields[0]);
        }
        else {
            return matchValue.fields[0];
        }
    };
}

/**
 * Parse a JSON string and run the decoder half of a codec, raising an exception carrying the
 * message on failure.
 */
export function Decode_unsafeFromString_Z2BB4EEE8(codec) {
    return Decode_fromString_1FBE35A8(codec_2(codec));
}

