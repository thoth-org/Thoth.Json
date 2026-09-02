
import { Record, Union } from "fable-library-js/Types.js";
import { record_type, lambda_type, class_type, bool_type, float64_type, union_type, list_type, tuple_type, string_type } from "fable-library-js/Reflection.js";

/**
 * Why a decoder failed, and the value it failed on.
 */
export class ErrorReason$1 extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["BadPrimitive", "BadPrimitiveExtra", "BadType", "BadField", "BadPath", "TooSmallArray", "FailMessage", "BadOneOf"];
    }
}

export function ErrorReason$1_$reflection(gen0) {
    return union_type("Thoth.Json.Core.ErrorReason`1", [gen0], ErrorReason$1, () => [[["Item1", string_type], ["Item2", gen0]], [["Item1", string_type], ["Item2", gen0], ["Item3", string_type]], [["Item1", string_type], ["Item2", gen0]], [["Item1", string_type], ["Item2", gen0]], [["Item1", string_type], ["Item2", gen0], ["Item3", string_type]], [["Item1", string_type], ["Item2", gen0]], [["Item", string_type]], [["Item", list_type(tuple_type(string_type, ErrorReason$1_$reflection(gen0)))]]]);
}

/**
 * A JSON value
 */
export class Json extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["String", "Number", "Null", "Boolean", "Object", "Array"];
    }
    static Null = new Json(2, []);
}

export function Json_$reflection() {
    return union_type("Thoth.Json.Core.Json", [], Json, () => [[["Item", string_type]], [["Item", float64_type]], [], [["Item", bool_type]], [["Item", list_type(tuple_type(string_type, Json_$reflection()))]], [["Item", list_type(Json_$reflection())]]]);
}

/**
 * An encoder and a decoder for the same type.
 */
export class Codec$1 extends Record {
    constructor(Encoder, Decoder) {
        super();
        this.Encoder = Encoder;
        this.Decoder = Decoder;
    }
}

export function Codec$1_$reflection(gen0) {
    return record_type("Thoth.Json.Core.Codec`1", [gen0], Codec$1, () => [["Encoder", lambda_type(gen0, class_type("Thoth.Json.Core.IEncodable"))], ["Decoder", class_type("Thoth.Json.Core.Decoder`1", [gen0])]]);
}

