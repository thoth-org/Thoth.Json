
import { Record, FSharpRef, Union } from "fable-library-js/Types.js";
import { record_type, obj_type, class_type, getGenericTypeDefinition, fullName, isGenericType, string_type, union_type } from "fable-library-js/Reflection.js";
import { Dictionary } from "fable-library-js/MutableMap.js";
import { compare, defaultOf, structuralHash, equals } from "fable-library-js/Util.js";
import { addToDict, tryGetValue } from "fable-library-js/MapUtil.js";
import { empty } from "fable-library-js/Map.js";

/**
 * How the auto API renames record fields and union cases.
 */
export class CaseStrategy extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["SnakeCase", "ScreamingSnakeCase", "PascalCase", "CamelCase", "DotNetPascalCase", "DotNetCamelCase"];
    }
    static SnakeCase = new CaseStrategy(0, []);
    static ScreamingSnakeCase = new CaseStrategy(1, []);
    static PascalCase = new CaseStrategy(2, []);
    static CamelCase = new CaseStrategy(3, []);
    static DotNetPascalCase = new CaseStrategy(4, []);
    static DotNetCamelCase = new CaseStrategy(5, []);
}

export function CaseStrategy_$reflection() {
    return union_type("Thoth.Json.Core.Auto.CaseStrategy", [], CaseStrategy, () => [[], [], [], [], [], []]);
}

/**
 * The identity of a type in an <see cref="T:Thoth.Json.Core.Auto.ExtraCoders"/>.
 */
export class TypeKey extends Union {
    constructor(Item) {
        super();
        this.tag = 0;
        this.fields = [Item];
    }
    cases() {
        return ["TypeKey"];
    }
}

export function TypeKey_$reflection() {
    return union_type("Thoth.Json.Core.Auto.TypeKey", [], TypeKey, () => [[["Item", string_type]]]);
}

export function TypeKey_Create_24524716(t) {
    if (isGenericType(t)) {
        return new TypeKey(fullName(getGenericTypeDefinition(t)));
    }
    else {
        return new TypeKey(fullName(t));
    }
}

/**
 * The key of the given type.
 */
export function TypeKeyModule_ofType(t) {
    return TypeKey_Create_24524716(t);
}

/**
 * Holds the coders already generated, so a type is walked once.
 */
export class Cache$2 {
    constructor() {
        this.cache = (new Dictionary([], {
            Equals: equals,
            GetHashCode: (x) => (structuralHash(x) | 0),
        }));
    }
}

export function Cache$2_$reflection(gen0, gen1) {
    return class_type("Thoth.Json.Core.Auto.Cache`2", [gen0, gen1], Cache$2);
}

export function Cache$2_$ctor() {
    return new Cache$2();
}

/**
 * The cached value for the key, generating it on the first ask.
 */
export function Cache$2__GetOrAdd_9927FCB(this$, key, factory) {
    let matchValue;
    let outArg = defaultOf();
    matchValue = [tryGetValue(this$.cache, key, new FSharpRef(() => outArg, (v) => {
        outArg = v;
    })), outArg];
    if (matchValue[0]) {
        return matchValue[1];
    }
    else {
        const x_1 = factory();
        addToDict(this$.cache, key, x_1);
        return x_1;
    }
}

/**
 * Coders the auto API uses instead of the ones it would generate.
 */
export class ExtraCoders extends Record {
    constructor(Hash, EncoderOverrides, DecoderOverrides) {
        super();
        this.Hash = Hash;
        this.EncoderOverrides = EncoderOverrides;
        this.DecoderOverrides = DecoderOverrides;
    }
}

export function ExtraCoders_$reflection() {
    return record_type("Thoth.Json.Core.Auto.ExtraCoders", [], ExtraCoders, () => [["Hash", string_type], ["EncoderOverrides", class_type("Microsoft.FSharp.Collections.FSharpMap`2", [TypeKey_$reflection(), obj_type])], ["DecoderOverrides", class_type("Microsoft.FSharp.Collections.FSharpMap`2", [TypeKey_$reflection(), obj_type])]]);
}

export const Extra_empty = new ExtraCoders("", empty({
    Compare: (x, y) => (compare(x, y) | 0),
}), empty({
    Compare: (x_1, y_1) => (compare(x_1, y_1) | 0),
}));

