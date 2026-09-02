
import { class_type } from "fable-library-js/Reflection.js";
import { succeed, map8, map7, map6, map5, map4, map3, map2, map } from "./Decode.js";

/**
 * The builder behind <c>decoder</c>.
 */
export class DecoderBuilder {
    constructor() {
    }
}

export function DecoderBuilder_$reflection() {
    return class_type("Thoth.Json.Core.DecoderCE.DecoderBuilder", undefined, DecoderBuilder);
}

export function DecoderBuilder_$ctor() {
    return new DecoderBuilder();
}

export function DecoderBuilder__BindReturn_6BCFE77E(_, m, f) {
    return map(f, m);
}

export function DecoderBuilder__Bind2Return_Z12312F75(_, d1, d2, ctor) {
    return map2((a, b) => ctor([a, b]), d1, d2);
}

export function DecoderBuilder__Bind3Return_B877D49(_, d1, d2, d3, ctor) {
    return map3((a, b, c) => ctor([a, b, c]), d1, d2, d3);
}

export function DecoderBuilder__Bind4Return_Z70E81EB5(_, d1, d2, d3, d4, ctor) {
    return map4((a, b, c, d) => ctor([a, b, c, d]), d1, d2, d3, d4);
}

export function DecoderBuilder__Bind5Return_1A977209(_, d1, d2, d3, d4, d5, ctor) {
    return map5((a, b, c, d, e) => ctor([a, b, c, d, e]), d1, d2, d3, d4, d5);
}

export function DecoderBuilder__Bind6Return_Z1E2849F5(_, d1, d2, d3, d4, d5, d6, ctor) {
    return map6((a, b, c, d, e, f) => ctor([a, b, c, d, e, f]), d1, d2, d3, d4, d5, d6);
}

export function DecoderBuilder__Bind7Return_Z16900C37(_, d1, d2, d3, d4, d5, d6, d7, ctor) {
    return map7((a, b, c, d, e, f, g) => ctor([a, b, c, d, e, f, g]), d1, d2, d3, d4, d5, d6, d7);
}

export function DecoderBuilder__Bind8Return_512F4ECB(_, d1, d2, d3, d4, d5, d6, d7, d8, ctor) {
    return map8((a, b, c, d, e, f, g, h) => ctor([a, b, c, d, e, f, g, h]), d1, d2, d3, d4, d5, d6, d7, d8);
}

export function DecoderBuilder__MergeSources_83ADF00(_, d1, d2) {
    return map2((a, b) => [a, b], d1, d2);
}

export function DecoderBuilder__Return_1505(_, x) {
    return succeed(x);
}

export function DecoderBuilder__ReturnFrom_1FBE35A8(_, x) {
    return x;
}

export const decoder = DecoderBuilder_$ctor();

