
import { isNegativeInfinity, isPositiveInfinity } from "fable-library-js/Double.js";
import { map as map_1 } from "fable-library-js/List.js";
import { map as map_2 } from "fable-library-js/Seq.js";
import { disposeSafe, getEnumerator } from "fable-library-js/Util.js";
import { map as map_3 } from "fable-library-js/Array.js";
import { toList, toSeq } from "fable-library-js/Map.js";
import { value as value_6 } from "fable-library-js/Option.js";

/**
 * Encode a float as a number.
 */
export function float(value_1) {
    return {
        Encode(helpers) {
            return Number.isNaN(value_1) ? helpers.encodeString("NaN") : (isPositiveInfinity(value_1) ? helpers.encodeString("Infinity") : (isNegativeInfinity(value_1) ? helpers.encodeString("-Infinity") : helpers.encodeDecimalNumber(value_1)));
        },
    };
}

/**
 * Encode a float32 as a number.
 */
export function float32(value_1) {
    return float(value_1);
}

/**
 * Encode a list as a JSON array.
 */
export function list(values) {
    return {
        Encode(helpers) {
            const arg = map_1((v) => v.Encode(helpers), values);
            return helpers.encodeList(arg);
        },
    };
}

/**
 * Encode a sequence as a JSON array.
 */
export function seq(values) {
    return {
        Encode(helpers) {
            const arg = map_2((v) => v.Encode(helpers), values);
            return helpers.encodeSeq(arg);
        },
    };
}

/**
 * Encode a ResizeArray as a JSON array.
 */
export function resizeArray(values) {
    return {
        Encode(helpers) {
            const result = [];
            let enumerator = getEnumerator(values);
            try {
                while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
                    const v = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
                    void (result.push(v.Encode(helpers)));
                }
            }
            finally {
                disposeSafe(enumerator);
            }
            return helpers.encodeResizeArray(result);
        },
    };
}

/**
 * Encode each element of a list with the given encoder.
 */
export function mapList(encoder, values) {
    return list(map_1(encoder, values));
}

/**
 * Encode each element of an array with the given encoder.
 */
export function mapArray(encoder, values) {
    const values_1 = map_3(encoder, values);
    return {
        Encode(helpers) {
            const arg = map_3((v) => v.Encode(helpers), values_1);
            return helpers.encodeArray(arg);
        },
    };
}

/**
 * Encode each element of a sequence with the given encoder.
 */
export function mapSeq(encoder, values) {
    return seq(map_2(encoder, values));
}

/**
 * Encode each element of a ResizeArray with the given encoder.
 */
export function mapResizeArray(encoder, values) {
    let collection;
    return resizeArray((collection = map_2(encoder, values), Array.from(collection)));
}

/**
 * Encode a map as an object, one property per key.
 */
export function dict(values) {
    const values_1 = toSeq(values);
    return {
        Encode(helpers) {
            const arg = map_2((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers)], values_1);
            return helpers.encodeObject(arg);
        },
    };
}

/**
 * Encode a tuple of 2 elements as a JSON array.
 */
export function tuple2(enc1, enc2, v1, v2) {
    const values = [enc1(v1), enc2(v2)];
    return {
        Encode(helpers) {
            const arg = map_3((v) => v.Encode(helpers), values);
            return helpers.encodeArray(arg);
        },
    };
}

/**
 * Encode a tuple of 3 elements as a JSON array.
 */
export function tuple3(enc1, enc2, enc3, v1, v2, v3) {
    const values = [enc1(v1), enc2(v2), enc3(v3)];
    return {
        Encode(helpers) {
            const arg = map_3((v) => v.Encode(helpers), values);
            return helpers.encodeArray(arg);
        },
    };
}

/**
 * Encode a tuple of 4 elements as a JSON array.
 */
export function tuple4(enc1, enc2, enc3, enc4, v1, v2, v3, v4) {
    const values = [enc1(v1), enc2(v2), enc3(v3), enc4(v4)];
    return {
        Encode(helpers) {
            const arg = map_3((v) => v.Encode(helpers), values);
            return helpers.encodeArray(arg);
        },
    };
}

/**
 * Encode a tuple of 5 elements as a JSON array.
 */
export function tuple5(enc1, enc2, enc3, enc4, enc5, v1, v2, v3, v4, v5) {
    const values = [enc1(v1), enc2(v2), enc3(v3), enc4(v4), enc5(v5)];
    return {
        Encode(helpers) {
            const arg = map_3((v) => v.Encode(helpers), values);
            return helpers.encodeArray(arg);
        },
    };
}

/**
 * Encode a tuple of 6 elements as a JSON array.
 */
export function tuple6(enc1, enc2, enc3, enc4, enc5, enc6, v1, v2, v3, v4, v5, v6) {
    const values = [enc1(v1), enc2(v2), enc3(v3), enc4(v4), enc5(v5), enc6(v6)];
    return {
        Encode(helpers) {
            const arg = map_3((v) => v.Encode(helpers), values);
            return helpers.encodeArray(arg);
        },
    };
}

/**
 * Encode a tuple of 7 elements as a JSON array.
 */
export function tuple7(enc1, enc2, enc3, enc4, enc5, enc6, enc7, v1, v2, v3, v4, v5, v6, v7) {
    const values = [enc1(v1), enc2(v2), enc3(v3), enc4(v4), enc5(v5), enc6(v6), enc7(v7)];
    return {
        Encode(helpers) {
            const arg = map_3((v) => v.Encode(helpers), values);
            return helpers.encodeArray(arg);
        },
    };
}

/**
 * Encode a tuple of 8 elements as a JSON array.
 */
export function tuple8(enc1, enc2, enc3, enc4, enc5, enc6, enc7, enc8, v1, v2, v3, v4, v5, v6, v7, v8) {
    const values = [enc1(v1), enc2(v2), enc3(v3), enc4(v4), enc5(v5), enc6(v6), enc7(v7), enc8(v8)];
    return {
        Encode(helpers) {
            const arg = map_3((v) => v.Encode(helpers), values);
            return helpers.encodeArray(arg);
        },
    };
}

/**
 * Encode a map as an array of <c>[ key, value ]</c> pairs. Use it when the key is not a string.
 */
export function map(keyEncoder, valueEncoder, values) {
    return list(map_1((tupledArg) => tuple2(keyEncoder, valueEncoder, tupledArg[0], tupledArg[1]), toList(values)));
}

/**
 * Defer the construction of an encoder until it is first used, for recursive types.
 */
export function lazily(enc, x) {
    return enc.Value(x);
}

/**
 * Encode an enum with an underlying byte as a number.
 */
export function Enum_byte(value_1) {
    return {
        Encode(helpers) {
            return helpers.encodeUnsignedIntegralNumber(value_1);
        },
    };
}

/**
 * Encode an enum with an underlying sbyte as a number.
 */
export function Enum_sbyte(value_1) {
    return {
        Encode(helpers) {
            return helpers.encodeSignedIntegralNumber(value_1);
        },
    };
}

/**
 * Encode an enum with an underlying int16 as a number.
 */
export function Enum_int16(value_1) {
    return {
        Encode(helpers) {
            return helpers.encodeSignedIntegralNumber(value_1);
        },
    };
}

/**
 * Encode an enum with an underlying uint16 as a number.
 */
export function Enum_uint16(value_1) {
    return {
        Encode(helpers) {
            return helpers.encodeUnsignedIntegralNumber(value_1);
        },
    };
}

/**
 * Encode an enum with an underlying int as a number.
 */
export function Enum_int(value_1) {
    return {
        Encode(helpers) {
            return helpers.encodeSignedIntegralNumber(value_1);
        },
    };
}

/**
 * Encode an enum with an underlying uint32 as a number.
 */
export function Enum_uint32(value_1) {
    return {
        Encode(helpers) {
            return helpers.encodeUnsignedIntegralNumber(value_1);
        },
    };
}

/**
 * Encode <c>Some x</c> as <c>x</c>, and <c>None</c> as <c>null</c>.
 */
export function lossyOption(encoder) {
    return (arg) => {
        let option_3;
        const option_1 = arg;
        option_3 = ((option_1 != null) ? encoder(value_6(option_1)) : undefined);
        return (option_3 != null) ? option_3 : {
            Encode(helpers) {
                return helpers.encodeNull();
            },
        };
    };
}

/**
 * Encode an option as an object carrying the case, so a nested option round-trips.
 */
export function losslessOption(encoder, value_1) {
    if (value_1 == null) {
        const values_1 = [["$type", {
            Encode(helpers_3) {
                return helpers_3.encodeString("option");
            },
        }], ["$case", {
            Encode(helpers_4) {
                return helpers_4.encodeString("none");
            },
        }]];
        return {
            Encode(helpers_5) {
                const arg_1 = map_2((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_5)], values_1);
                return helpers_5.encodeObject(arg_1);
            },
        };
    }
    else {
        const v = value_6(value_1);
        const values = [["$type", {
            Encode(helpers) {
                return helpers.encodeString("option");
            },
        }], ["$case", {
            Encode(helpers_1) {
                return helpers_1.encodeString("some");
            },
        }], ["$value", encoder(v)]];
        return {
            Encode(helpers_2) {
                const arg = map_2((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers_2)], values);
                return helpers_2.encodeObject(arg);
            },
        };
    }
}

/**
 * Encode a <see cref="T:Thoth.Json.Core.Json"/> value as it stands.
 */
export function value(json) {
    switch (json.tag) {
        case 3:
            return {
                Encode(helpers_1) {
                    return helpers_1.encodeBool(json.fields[0]);
                },
            };
        case 0:
            return {
                Encode(helpers_2) {
                    return helpers_2.encodeString(json.fields[0]);
                },
            };
        case 1:
            return float(json.fields[0]);
        case 5:
            return list(map_1(value, json.fields[0]));
        case 4: {
            const values = map_2((tupledArg) => [tupledArg[0], value(tupledArg[1])], json.fields[0]);
            return {
                Encode(helpers_3) {
                    const arg = map_2((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_3)], values);
                    return helpers_3.encodeObject(arg);
                },
            };
        }
        default:
            return {
                Encode(helpers) {
                    return helpers.encodeNull();
                },
            };
    }
}

/**
 * The encoder half of a codec.
 */
export function codec(c) {
    return c.Encoder;
}

