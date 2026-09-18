
import { Record, Union } from "fable-library-js/Types.js";
import { record_type, class_type, union_type, string_type } from "fable-library-js/Reflection.js";
import { containsKey, tryFind, toSeq, ofSeq } from "fable-library-js/Map.js";
import { map, tryExactlyOne, filter, append, delay, toList } from "fable-library-js/Seq.js";
import { Exception, comparePrimitives } from "fable-library-js/Util.js";
import { map as map_1, string, index, keys as keys_1, field, andThen, fail } from "./Decode.js";
import { join, concat } from "fable-library-js/String.js";
import { create } from "./Codec.js";
import { tuple2 } from "./Encode.js";

/**
 * How a <c>variantCodec</c> writes the case a value belongs to.
 */
export class VariantEncoding extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["TagAndValue", "OnTag", "Tuple"];
    }
    static OnTag = new VariantEncoding(1, []);
    static Tuple = new VariantEncoding(2, []);
}

export function VariantEncoding_$reflection() {
    return union_type("Thoth.Json.Core.VariantCodecBuilder.VariantEncoding", [], VariantEncoding, () => [[["tagName", string_type], ["valueName", string_type]], [], []]);
}

/**
 * The cases declared so far by a <c>variantCodec</c> block.
 */
export class VariantCase$2 extends Record {
    constructor(Value, Decoders) {
        super();
        this.Value = Value;
        this.Decoders = Decoders;
    }
}

export function VariantCase$2_$reflection(gen0, gen1) {
    return record_type("Thoth.Json.Core.VariantCodecBuilder.VariantCase`2", [gen0, gen1], VariantCase$2, () => [["Value", gen0], ["Decoders", class_type("Microsoft.FSharp.Collections.FSharpMap`2", [string_type, class_type("Thoth.Json.Core.Decoder`1", [gen1])])]]);
}

export function VariantCase_zip(a, b) {
    return new VariantCase$2([a.Value, b.Value], ofSeq(toList(delay(() => append(toSeq(a.Decoders), delay(() => toSeq(b.Decoders))))), {
        Compare: (x, y) => (comparePrimitives(x, y) | 0),
    }));
}

export function VariantCase_complete(variantEncoding, f, x) {
    const decodeForTag = (tag, descend) => {
        const matchValue = tryFind(tag, x.Decoders);
        if (matchValue == null) {
            return fail(concat("The tag \"", tag, "\" was not recognized"));
        }
        else {
            return descend(matchValue);
        }
    };
    return create((v) => f(x.Value, v, variantEncoding), (variantEncoding.tag === 1) ? andThen((keys) => {
        const recognizedKeys = filter((key) => containsKey(key, x.Decoders), keys);
        const matchValue_1 = tryExactlyOne(recognizedKeys);
        if (matchValue_1 == null) {
            return fail(concat("Expected exactly one object key but found: ", join(", ", map((x_1) => concat("\"", x_1, "\""), recognizedKeys))));
        }
        else {
            const tag_2 = matchValue_1;
            return decodeForTag(tag_2, (decoder_3) => field(tag_2, decoder_3));
        }
    }, keys_1) : ((variantEncoding.tag === 2) ? andThen((tag_3) => decodeForTag(tag_3, (decoder_5) => index(1, decoder_5)), index(0, string)) : andThen((tag_1) => decodeForTag(tag_1, (decoder_1) => field(variantEncoding.fields[1], decoder_1)), field(variantEncoding.fields[0], string))));
}

/**
 * The builder behind <c>variantCodec</c> and <c>variantCodecWithTag</c>.
 */
export class VariantCodecBuilder {
    constructor(variantEncoding) {
        this.variantEncoding = variantEncoding;
    }
}

export function VariantCodecBuilder_$reflection() {
    return class_type("Thoth.Json.Core.VariantCodecBuilder.VariantCodecBuilder", undefined, VariantCodecBuilder);
}

export function VariantCodecBuilder_$ctor_4C6A4545(variantEncoding) {
    return new VariantCodecBuilder(variantEncoding);
}

export function VariantCodecBuilder__MergeSources_59582FC0(this$, a, b) {
    return VariantCase_zip(a, b);
}

export function VariantCodecBuilder__BindReturn_2681A3BE(this$, x, f) {
    return VariantCase_complete(this$.variantEncoding, f, x);
}

/**
 * Build a codec for a union, writing the tag and the value under the given property names.
 */
export function variantCodecWithTag(tagName, valueName) {
    if (tagName === valueName) {
        throw new Exception("value name must be distinct from tag name (Parameter \'valueName\')");
    }
    return VariantCodecBuilder_$ctor_4C6A4545(new VariantEncoding(/* TagAndValue */ 0, [tagName, valueName]));
}

export const variantCodecTuple = VariantCodecBuilder_$ctor_4C6A4545(VariantEncoding.Tuple);

export const variantCodec = VariantCodecBuilder_$ctor_4C6A4545(VariantEncoding.OnTag);

/**
 * A case of a <c>variantCodec</c> block: the tag it is written under, its constructor, and
 * the codec for its fields.
 */
export function Codec_case(tag, caseConstructor, caseCodec) {
    return new VariantCase$2((t) => ((_arg) => {
        switch (_arg.tag) {
            case 0: {
                const values_1 = [[_arg.fields[0], {
                    Encode(helpers_1) {
                        return helpers_1.encodeString(tag);
                    },
                }], [_arg.fields[1], caseCodec.Encoder(t)]];
                return {
                    Encode(helpers_2) {
                        const arg_1 = map((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_2)], values_1);
                        return helpers_2.encodeObject(arg_1);
                    },
                };
            }
            case 2:
                return tuple2((value_1) => ({
                    Encode(helpers_3) {
                        return helpers_3.encodeString(value_1);
                    },
                }), caseCodec.Encoder, tag, t);
            default: {
                const values = [[tag, caseCodec.Encoder(t)]];
                return {
                    Encode(helpers) {
                        const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers)], values);
                        return helpers.encodeObject(arg);
                    },
                };
            }
        }
    }), ofSeq([[tag, map_1(caseConstructor, caseCodec.Decoder)]], {
        Compare: (x, y) => (comparePrimitives(x, y) | 0),
    }));
}

