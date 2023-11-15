import { Union, Record } from "./fable_modules/fable-library.4.5.0/Types.js";
import { union_type, string_type, record_type, float64_type, int32_type } from "./fable_modules/fable-library.4.5.0/Reflection.js";

export const jsonRecord = "{ \"a\": 1.0,\n         \"b\": 2.0,\n         \"c\": 3.0,\n         \"d\": 4.0,\n         \"e\": 5.0,\n         \"f\": 6.0,\n         \"g\": 7.0,\n         \"h\": 8.0 }";

export const jsonRecordInvalid = "{ \"a\": \"invalid_a_field\",\n         \"b\": \"invalid_a_field\",\n         \"c\": \"invalid_a_field\",\n         \"d\": \"invalid_a_field\",\n         \"e\": \"invalid_a_field\",\n         \"f\": \"invalid_a_field\",\n         \"g\": \"invalid_a_field\",\n         \"h\": \"invalid_a_field\" }";

export class RecordWithPrivateConstructor extends Record {
    constructor(Foo1, Foo2) {
        super();
        this.Foo1 = (Foo1 | 0);
        this.Foo2 = Foo2;
    }
}

export function RecordWithPrivateConstructor_$reflection() {
    return record_type("Tests.Decoders.RecordWithPrivateConstructor", [], RecordWithPrivateConstructor, () => [["Foo1", int32_type], ["Foo2", float64_type]]);
}

export class UnionWithPrivateConstructor extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["Bar", "Baz"];
    }
}

export function UnionWithPrivateConstructor_$reflection() {
    return union_type("Tests.Decoders.UnionWithPrivateConstructor", [], UnionWithPrivateConstructor, () => [[["Item", string_type]], []]);
}

export class UnionWithMultipleFields extends Union {
    constructor(Item1, Item2, Item3) {
        super();
        this.tag = 0;
        this.fields = [Item1, Item2, Item3];
    }
    cases() {
        return ["Multi"];
    }
}

export function UnionWithMultipleFields_$reflection() {
    return union_type("Tests.Decoders.UnionWithMultipleFields", [], UnionWithMultipleFields, () => [[["Item1", string_type], ["Item2", int32_type], ["Item3", float64_type]]]);
}

export const tests = (() => {
    throw 1;
})();

