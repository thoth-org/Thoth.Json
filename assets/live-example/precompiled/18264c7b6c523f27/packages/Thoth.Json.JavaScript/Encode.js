
import { disposeSafe, getEnumerator, defaultOf } from "fable-library-js/Util.js";
import { codec as codec_1 } from "../Thoth.Json.Core/Encode.js";

export const helpers = {
    encodeString(value) {
        return value;
    },
    encodeChar(value_1) {
        return value_1;
    },
    encodeDecimalNumber(value_2) {
        return value_2;
    },
    encodeBool(value_3) {
        return value_3;
    },
    encodeNull() {
        return defaultOf();
    },
    encodeObject(values) {
        const o = {};
        const enumerator = getEnumerator(values);
        try {
            while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
                const forLoopVar = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
                o[forLoopVar[0]] = forLoopVar[1];
            }
        }
        finally {
            disposeSafe(enumerator);
        }
        return o;
    },
    encodeArray(values_1) {
        return Array.from(values_1);
    },
    encodeList(values_2) {
        return Array.from(values_2);
    },
    encodeSeq(values_3) {
        return Array.from(values_3);
    },
    encodeResizeArray(values_4) {
        return Array.from(values_4);
    },
    encodeSignedIntegralNumber(value_5) {
        return value_5;
    },
    encodeUnsignedIntegralNumber(value_6) {
        return value_6;
    },
};

/**
 * Write an encodable as a JSON string, indented by the given number of spaces. Pass <c>0</c>
 * for a single line.
 */
export function toString(space, value) {
    const json_1 = value.Encode(helpers);
    return JSON.stringify(json_1, undefined, space);
}

/**
 * Write a value as a JSON string with the encoder half of a codec.
 */
export function fromCodec(space, codec, value) {
    return toString(space, codec_1(codec)(value));
}

