
import { Record } from "fable-library-js/Types.js";
import { record_type, lambda_type, list_type, tuple_type, class_type, string_type } from "fable-library-js/Reflection.js";
import { empty, singleton, append } from "fable-library-js/List.js";
import { optional, field, map2 } from "./Decode.js";
import { create, map } from "./Codec.js";
import { map as map_1 } from "fable-library-js/Seq.js";
import { value } from "fable-library-js/Option.js";

/**
 * The fields declared so far by an <c>objectCodec</c> block.
 */
export class ObjectCodecFieldSet$2 extends Record {
    constructor(Values, Decoder, Picker) {
        super();
        this.Values = Values;
        this.Decoder = Decoder;
        this.Picker = Picker;
    }
}

export function ObjectCodecFieldSet$2_$reflection(gen0, gen1) {
    return record_type("Thoth.Json.Core.ObjectCodecComputationExpression.ObjectCodecFieldSet`2", [gen0, gen1], ObjectCodecFieldSet$2, () => [["Values", lambda_type(gen0, list_type(tuple_type(string_type, class_type("Thoth.Json.Core.IEncodable"))))], ["Decoder", class_type("Thoth.Json.Core.Decoder`1", [gen0])], ["Picker", lambda_type(gen1, gen0)]]);
}

export function ObjectCodecFieldSet_zip(a, b) {
    return new ObjectCodecFieldSet$2((tupledArg) => append(a.Values(tupledArg[0]), b.Values(tupledArg[1])), map2((i_1, j_1) => [i_1, j_1], a.Decoder, b.Decoder), (u) => [a.Picker(u), b.Picker(u)]);
}

export function ObjectCodecFieldSet_complete(f, m) {
    return map(f, m.Picker, create((t) => {
        const values = m.Values(t);
        return {
            Encode(helpers) {
                const arg = map_1((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers)], values);
                return helpers.encodeObject(arg);
            },
        };
    }, m.Decoder));
}

/**
 * The builder behind <c>objectCodec</c>.
 */
export class ObjectCodecBuilder {
    constructor() {
    }
}

export function ObjectCodecBuilder_$reflection() {
    return class_type("Thoth.Json.Core.ObjectCodecComputationExpression.ObjectCodecBuilder", undefined, ObjectCodecBuilder);
}

export function ObjectCodecBuilder_$ctor() {
    return new ObjectCodecBuilder();
}

export function ObjectCodecBuilder__MergeSources_5532C640(this$, a, b) {
    return ObjectCodecFieldSet_zip(a, b);
}

export function ObjectCodecBuilder__BindReturn_Z2BDEB988(this$, m, f) {
    return ObjectCodecFieldSet_complete(f, m);
}

export const objectCodec = ObjectCodecBuilder_$ctor();

/**
 * A required property of an <c>objectCodec</c> block: its name, how to read it out of the
 * value, and the codec for it.
 */
export function Codec_field(fieldName, picker, fieldCodec) {
    return new ObjectCodecFieldSet$2((i) => singleton([fieldName, fieldCodec.Encoder(i)]), field(fieldName, fieldCodec.Decoder), picker);
}

/**
 * An optional property of an <c>objectCodec</c> block. It is left out when the value is
 * <c>None</c>, and its absence decodes back to <c>None</c>.
 */
export function Codec_optional(fieldName, picker, fieldCodec) {
    return new ObjectCodecFieldSet$2((_arg) => ((_arg == null) ? empty() : singleton([fieldName, fieldCodec.Encoder(value(_arg))])), optional(fieldName, fieldCodec.Decoder), picker);
}

