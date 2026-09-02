
import { Codec$1 } from "./Types.js";
import { tuple8 as tuple8_2, tuple7 as tuple7_2, tuple6 as tuple6_2, tuple5 as tuple5_2, tuple4 as tuple4_2, tuple3 as tuple3_2, tuple2 as tuple2_2, at as at_1, keyValuePairs as keyValuePairs_1, map$0027 as map$0027_1, dict as dict_2, resizeArray as resizeArray_1, seq as seq_1, array as array_1, list as list_2, lazily as lazily_2, oneOf as oneOf_1, losslessOption as losslessOption_2, lossyOption as lossyOption_2, map as map_1, nil as nil_1, value as value_5, timespan as timespan_1, datetimeOffset as datetimeOffset_1, decimal as decimal_1, float32 as float32_2, float as float_1, bool as bool_1, bigint as bigint_1, uint64 as uint64_1, int64 as int64_1, uint32 as uint32_1, int as int_1, uint16 as uint16_1, int16 as int16_1, byte as byte_1, sbyte as sbyte_1, unit as unit_1, uri as uri_1, guid as guid_1, char as char_1, string as string_1 } from "./Decode.js";
import { toString } from "fable-library-js/BigInt.js";
import { tuple8 as tuple8_1, tuple7 as tuple7_1, tuple6 as tuple6_1, tuple5 as tuple5_1, tuple4 as tuple4_1, tuple3 as tuple3_1, tuple2 as tuple2_1, map as map_4, dict as dict_1, mapResizeArray, mapSeq, mapArray, mapList, lazily as lazily_1, losslessOption as losslessOption_1, lossyOption as lossyOption_1, value as value_4, float32 as float32_1 } from "./Encode.js";
import { toString as toString_1 } from "fable-library-js/Date.js";
import { toString as toString_2 } from "fable-library-js/TimeSpan.js";
import { foldBack, map as map_2, head, isEmpty } from "fable-library-js/List.js";
import { defaultOf, Lazy, Exception } from "fable-library-js/Util.js";
import { map as map_3 } from "fable-library-js/Map.js";
import { map as map_5 } from "fable-library-js/Seq.js";

/**
 * Pair an encoder and a decoder into a codec.
 */
export function create(encoder_1, decoder_1) {
    return new Codec$1(encoder_1, decoder_1);
}

/**
 * The encoder half of a codec.
 */
export function encoder(c) {
    return c.Encoder;
}

/**
 * The decoder half of a codec.
 */
export function decoder(c) {
    return c.Decoder;
}

export const string = create((value_1) => ({
    Encode(helpers) {
        return helpers.encodeString(value_1);
    },
}), string_1);

export const char = create((value_1) => ({
    Encode(helpers) {
        return helpers.encodeChar(value_1);
    },
}), char_1);

export const guid = create((value_1) => {
    let value_1_1;
    let copyOfStruct = value_1;
    value_1_1 = copyOfStruct;
    return {
        Encode(helpers) {
            return helpers.encodeString(value_1_1);
        },
    };
}, guid_1);

export const uri = create((value_1) => {
    const value_1_1 = value_1.originalString;
    return {
        Encode(helpers) {
            return helpers.encodeString(value_1_1);
        },
    };
}, uri_1);

export const unit = create(() => ({
    Encode(helpers) {
        return helpers.encodeNull();
    },
}), unit_1);

export const sbyte = create((value_1) => ({
    Encode(helpers) {
        return helpers.encodeSignedIntegralNumber(value_1);
    },
}), sbyte_1);

export const byte = create((value_1) => ({
    Encode(helpers) {
        return helpers.encodeUnsignedIntegralNumber(value_1);
    },
}), byte_1);

export const int16 = create((value_1) => ({
    Encode(helpers) {
        return helpers.encodeSignedIntegralNumber(value_1);
    },
}), int16_1);

export const uint16 = create((value_1) => ({
    Encode(helpers) {
        return helpers.encodeUnsignedIntegralNumber(value_1);
    },
}), uint16_1);

export const int = create((value_1) => ({
    Encode(helpers) {
        return helpers.encodeSignedIntegralNumber(value_1);
    },
}), int_1);

export const uint32 = create((value_1) => ({
    Encode(helpers) {
        return helpers.encodeUnsignedIntegralNumber(value_1);
    },
}), uint32_1);

export const int64 = create((value_1) => {
    const value_1_1 = String(value_1);
    return {
        Encode(helpers) {
            return helpers.encodeString(value_1_1);
        },
    };
}, int64_1);

export const uint64 = create((value_1) => {
    const value_1_1 = String(value_1);
    return {
        Encode(helpers) {
            return helpers.encodeString(value_1_1);
        },
    };
}, uint64_1);

export const bigint = create((value_1) => {
    const value_1_1 = toString(value_1);
    return {
        Encode(helpers) {
            return helpers.encodeString(value_1_1);
        },
    };
}, bigint_1);

export const bool = create((value_1) => ({
    Encode(helpers) {
        return helpers.encodeBool(value_1);
    },
}), bool_1);

export const float = create((value_1) => ({
    Encode(helpers) {
        return helpers.encodeDecimalNumber(value_1);
    },
}), float_1);

export const float32 = create(float32_1, float32_2);

export const decimal = create((value_1) => {
    const value_1_1 = value_1.toString();
    return {
        Encode(helpers) {
            return helpers.encodeString(value_1_1);
        },
    };
}, decimal_1);

export const datetimeOffset = create((value_1) => {
    const value_1_1 = toString_1(value_1, "O", {});
    return {
        Encode(helpers) {
            return helpers.encodeString(value_1_1);
        },
    };
}, datetimeOffset_1);

export const timespan = create((value_1) => {
    let value_1_1;
    let copyOfStruct = value_1;
    value_1_1 = toString_2(copyOfStruct);
    return {
        Encode(helpers) {
            return helpers.encodeString(value_1_1);
        },
    };
}, timespan_1);

export const value = create(value_4, value_5);

/**
 * Encode any value as <c>null</c>, and decode <c>null</c> back to the given constant.
 */
export function nil(output) {
    return create((_arg) => ({
        Encode(helpers) {
            return helpers.encodeNull();
        },
    }), nil_1(output));
}

/**
 * Turn a codec into one for another type, given a conversion for each direction.
 */
export function map(decoder_1, encoder_1, codec) {
    return create((arg) => codec.Encoder(encoder_1(arg)), map_1(decoder_1, codec.Decoder));
}

/**
 * A codec for an option, writing <c>Some x</c> as <c>x</c> and <c>None</c> as <c>null</c>.
 */
export function lossyOption(x) {
    return create(lossyOption_1(x.Encoder), lossyOption_2(x.Decoder));
}

/**
 * A codec for an option, writing an object which carries the case.
 */
export function losslessOption(x) {
    return create((value_1) => losslessOption_1(x.Encoder, value_1), losslessOption_2(x.Decoder));
}

/**
 * Decode with the first codec of the list that succeeds, and always encode with the first one.
 */
export function oneOf(codecs) {
    if (!isEmpty(codecs)) {
        return create(head(codecs).Encoder, oneOf_1(map_2((_arg) => _arg.Decoder, codecs)));
    }
    else {
        throw new Exception("Codec.oneOf requires at least one codec (Parameter \'codecs\')");
    }
}

/**
 * Defer the construction of a codec until it is first used, for mutually recursive types.
 */
export function lazily(codec) {
    let enc;
    return create((enc = (new Lazy(() => codec.Value.Encoder)), (x) => lazily_1(enc, x)), lazily_2(new Lazy(() => codec.Value.Decoder)));
}

/**
 * Build a codec which can refer to itself, for a recursive type such as a tree.
 */
export function fix(make) {
    let self = defaultOf();
    self = make(lazily(new Lazy(() => self)));
    return self;
}

/**
 * A codec for a list, as a JSON array.
 */
export function list(x) {
    return create((values) => mapList(x.Encoder, values), list_2(x.Decoder));
}

/**
 * A codec for an array, as a JSON array.
 */
export function array(x) {
    return create((values) => mapArray(x.Encoder, values), array_1(x.Decoder));
}

/**
 * A codec for a sequence, as a JSON array.
 */
export function seq(x) {
    return create((values) => mapSeq(x.Encoder, values), seq_1(x.Decoder));
}

/**
 * A codec for a ResizeArray, as a JSON array.
 */
export function resizeArray(x) {
    return create((values) => mapResizeArray(x.Encoder, values), resizeArray_1(x.Decoder));
}

/**
 * A codec for a map keyed by string, as a JSON object.
 */
export function dict(x) {
    return create((arg) => dict_1(map_3((_arg, v) => x.Encoder(v), arg)), dict_2(x.Decoder));
}

/**
 * A codec for a map, as an array of <c>[ key, value ]</c> pairs. Use it when the key is not a string.
 */
export function map$0027(key, value_1) {
    return create((values) => map_4(key.Encoder, value_1.Encoder, values), map$0027_1(key.Decoder, value_1.Decoder));
}

/**
 * A codec for a list of properties, as a JSON object.
 */
export function keyValuePairs(x) {
    return create((arg_1) => {
        const values = map_2((tupledArg) => [tupledArg[0], x.Encoder(tupledArg[1])], arg_1);
        return {
            Encode(helpers) {
                const arg = map_5((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers)], values);
                return helpers.encodeObject(arg);
            },
        };
    }, keyValuePairs_1(x.Decoder));
}

/**
 * Move a codec under a path of property names. Decoding reads the value there, encoding wraps
 * it back in nested objects.
 */
export function at(fieldNames, x) {
    return create((v) => foldBack((field, child) => ({
        Encode(helpers) {
            const arg = map_5((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers)], [[field, child]]);
            return helpers.encodeObject(arg);
        },
    }), fieldNames, x.Encoder(v)), at_1(fieldNames, x.Decoder));
}

/**
 * A codec for a tuple of 2 elements, as a JSON array.
 */
export function tuple2(a, b) {
    return create((tupledArg) => tuple2_1(a.Encoder, b.Encoder, tupledArg[0], tupledArg[1]), tuple2_2(a.Decoder, b.Decoder));
}

/**
 * A codec for a tuple of 3 elements, as a JSON array.
 */
export function tuple3(a, b, c) {
    return create((tupledArg) => tuple3_1(a.Encoder, b.Encoder, c.Encoder, tupledArg[0], tupledArg[1], tupledArg[2]), tuple3_2(a.Decoder, b.Decoder, c.Decoder));
}

/**
 * A codec for a tuple of 4 elements, as a JSON array.
 */
export function tuple4(a, b, c, d) {
    return create((tupledArg) => tuple4_1(a.Encoder, b.Encoder, c.Encoder, d.Encoder, tupledArg[0], tupledArg[1], tupledArg[2], tupledArg[3]), tuple4_2(a.Decoder, b.Decoder, c.Decoder, d.Decoder));
}

/**
 * A codec for a tuple of 5 elements, as a JSON array.
 */
export function tuple5(a, b, c, d, e) {
    return create((tupledArg) => tuple5_1(a.Encoder, b.Encoder, c.Encoder, d.Encoder, e.Encoder, tupledArg[0], tupledArg[1], tupledArg[2], tupledArg[3], tupledArg[4]), tuple5_2(a.Decoder, b.Decoder, c.Decoder, d.Decoder, e.Decoder));
}

/**
 * A codec for a tuple of 6 elements, as a JSON array.
 */
export function tuple6(a, b, c, d, e, f) {
    return create((tupledArg) => tuple6_1(a.Encoder, b.Encoder, c.Encoder, d.Encoder, e.Encoder, f.Encoder, tupledArg[0], tupledArg[1], tupledArg[2], tupledArg[3], tupledArg[4], tupledArg[5]), tuple6_2(a.Decoder, b.Decoder, c.Decoder, d.Decoder, e.Decoder, f.Decoder));
}

/**
 * A codec for a tuple of 7 elements, as a JSON array.
 */
export function tuple7(a, b, c, d, e, f, g) {
    return create((tupledArg) => tuple7_1(a.Encoder, b.Encoder, c.Encoder, d.Encoder, e.Encoder, f.Encoder, g.Encoder, tupledArg[0], tupledArg[1], tupledArg[2], tupledArg[3], tupledArg[4], tupledArg[5], tupledArg[6]), tuple7_2(a.Decoder, b.Decoder, c.Decoder, d.Decoder, e.Decoder, f.Decoder, g.Decoder));
}

/**
 * A codec for a tuple of 8 elements, as a JSON array.
 */
export function tuple8(a, b, c, d, e, f, g, h) {
    return create((tupledArg) => tuple8_1(a.Encoder, b.Encoder, c.Encoder, d.Encoder, e.Encoder, f.Encoder, g.Encoder, h.Encoder, tupledArg[0], tupledArg[1], tupledArg[2], tupledArg[3], tupledArg[4], tupledArg[5], tupledArg[6], tupledArg[7]), tuple8_2(a.Decoder, b.Decoder, c.Decoder, d.Decoder, e.Decoder, f.Decoder, g.Decoder, h.Decoder));
}

