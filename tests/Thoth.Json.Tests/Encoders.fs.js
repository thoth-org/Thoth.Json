import { Union, Record } from "./Thoth.Json.JavaScript.Tests/fable_modules/fable-library.4.5.0/Types.js";
import { union_type, string_type, record_type, float64_type, int32_type } from "./Thoth.Json.JavaScript.Tests/fable_modules/fable-library.4.5.0/Reflection.js";
import { Test_testCase, Test_testList } from "./Thoth.Json.JavaScript.Tests/fable_modules/Fable.Mocha.2.17.0/Mocha.fs.js";
import { defaultOf, int64ToString, comparePrimitives, assertEqual } from "./Thoth.Json.JavaScript.Tests/fable_modules/fable-library.4.5.0/Util.js";
import { toString } from "../packages/Thoth.Json.JavaScript/Encode.fs.js";
import { Json } from "../packages/Thoth.Json.Core/Types.fs.js";
import { option, tuple8, tuple7, tuple6, tuple5, tuple4, tuple3, tuple2, Enum_uint16, Enum_int16, Enum_uint32, Enum_int, Enum_byte, Enum_sbyte, map, dict, list } from "../packages/Thoth.Json.Core/Encode.fs.js";
import { singleton, ofArray } from "./Thoth.Json.JavaScript.Tests/fable_modules/fable-library.4.5.0/List.js";
import { ofList } from "./Thoth.Json.JavaScript.Tests/fable_modules/fable-library.4.5.0/Map.js";
import { fromInt32, toString as toString_1 } from "./Thoth.Json.JavaScript.Tests/fable_modules/fable-library.4.5.0/BigInt.js";
import { create, toString as toString_2 } from "./Thoth.Json.JavaScript.Tests/fable_modules/fable-library.4.5.0/Date.js";
import { create as create_1 } from "./Thoth.Json.JavaScript.Tests/fable_modules/fable-library.4.5.0/DateOffset.js";
import { create as create_2, fromHours } from "./Thoth.Json.JavaScript.Tests/fable_modules/fable-library.4.5.0/TimeSpan.js";
import { fromParts, toString as toString_3 } from "./Thoth.Json.JavaScript.Tests/fable_modules/fable-library.4.5.0/Decimal.js";
import { SmallRecord, SmallRecord_Encoder_Z4AB0BC7 } from "./Types.fs.js";

export class RecordWithPrivateConstructor extends Record {
    constructor(Foo1, Foo2) {
        super();
        this.Foo1 = (Foo1 | 0);
        this.Foo2 = Foo2;
    }
}

export function RecordWithPrivateConstructor_$reflection() {
    return record_type("Tests.Encoders.RecordWithPrivateConstructor", [], RecordWithPrivateConstructor, () => [["Foo1", int32_type], ["Foo2", float64_type]]);
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
    return union_type("Tests.Encoders.UnionWithPrivateConstructor", [], UnionWithPrivateConstructor, () => [[["Item", string_type]], []]);
}

export const tests = Test_testList("Thoth.Json.Encode", singleton(Test_testList("Basic", ofArray([Test_testCase("a string works", () => {
    assertEqual(toString(0, new Json(0, ["maxime"])), "\"maxime\"");
}), Test_testCase("a string with new line works", () => {
    assertEqual(toString(4, new Json(0, ["a\nb"])), "\"a\\nb\"");
}), Test_testCase("a string with new line character works", () => {
    assertEqual(toString(4, new Json(0, ["a\\nb"])), "\"a\\\\nb\"");
}), Test_testCase("a string with tab works", () => {
    assertEqual(toString(4, new Json(0, ["a\tb"])), "\"a\\tb\"");
}), Test_testCase("a string with tab character works", () => {
    assertEqual(toString(4, new Json(0, ["a\\tb"])), "\"a\\\\tb\"");
}), Test_testCase("a char works", () => {
    assertEqual(toString(0, new Json(1, ["a"])), "\"a\"");
}), Test_testCase("an int works", () => {
    assertEqual(toString(0, new Json(13, [1])), "1");
}), Test_testCase("a float works", () => {
    assertEqual(toString(0, new Json(2, [1.2])), "1.2");
}), Test_testCase("an array works", () => {
    assertEqual(toString(0, new Json(7, [[new Json(0, ["maxime"]), new Json(13, [2])]])), "[\"maxime\",2]");
}), Test_testCase("a list works", () => {
    assertEqual(toString(0, list(ofArray([new Json(0, ["maxime"]), new Json(13, [2])]))), "[\"maxime\",2]");
}), Test_testCase("a bool works", () => {
    assertEqual(toString(0, new Json(5, [false])), "false");
}), Test_testCase("a null works", () => {
    assertEqual(toString(0, new Json(4, [])), "null");
}), Test_testCase("unit works", () => {
    assertEqual(toString(0, new Json(15, [])), "null");
}), Test_testCase("an object works", () => {
    assertEqual(toString(0, new Json(6, [[["firstname", new Json(0, ["maxime"])], ["age", new Json(13, [25])]]])), "{\"firstname\":\"maxime\",\"age\":25}");
}), Test_testCase("a dict works", () => {
    assertEqual(toString(0, dict(ofList(ofArray([["a", new Json(13, [1])], ["b", new Json(13, [2])], ["c", new Json(13, [3])]]), {
        Compare: comparePrimitives,
    }))), "{\"a\":1,\"b\":2,\"c\":3}");
}), Test_testCase("a map works", () => {
    assertEqual(toString(0, map((value_33) => (new Json(0, [value_33])), (value_35) => (new Json(13, [value_35])), ofList(ofArray([["a", 1], ["b", 2], ["c", 3]]), {
        Compare: comparePrimitives,
    }))), "[[\"a\",1],[\"b\",2],[\"c\",3]]");
}), Test_testCase("a bigint works", () => {
    assertEqual(toString(0, new Json(0, [toString_1(fromInt32(12))])), "\"12\"");
}), Test_testCase("a datetime works", () => {
    assertEqual(toString(0, new Json(0, [toString_2(create(2018, 10, 1, 11, 12, 55, 0, 1), "O", {})])), "\"2018-10-01T11:12:55.000Z\"");
}), Test_testCase("a datetimeOffset works", () => {
    assertEqual(toString(0, new Json(0, [toString_2(create_1(2018, 7, 2, 12, 23, 45, 0, fromHours(2)), "O", {})])), "\"2018-07-02T12:23:45.000+02:00\"");
}), Test_testCase("a timeSpan works", () => {
    assertEqual(toString(0, new Json(8, [create_2(1, 2, 3, 4, 5)])), "\"1.02:03:04.0050000\"");
}), Test_testCase("a decimal works", () => {
    assertEqual(toString(0, new Json(0, [toString_3(fromParts(7833, 0, 0, false, 4))])), "\"0.7833\"");
}), Test_testCase("a guid works", () => {
    let copyOfStruct;
    assertEqual(toString(0, new Json(0, [(copyOfStruct = "1e5dee25-8558-4392-a9fb-aae03f81068f", copyOfStruct)])), "\"1e5dee25-8558-4392-a9fb-aae03f81068f\"");
}), Test_testCase("an byte works", () => {
    assertEqual(toString(0, new Json(10, [99])), "99");
}), Test_testCase("an sbyte works", () => {
    assertEqual(toString(0, new Json(9, [99])), "99");
}), Test_testCase("an int16 works", () => {
    assertEqual(toString(0, new Json(11, [99])), "99");
}), Test_testCase("an uint16 works", () => {
    assertEqual(toString(0, new Json(12, [99])), "99");
}), Test_testCase("an int64 works", () => {
    assertEqual(toString(0, new Json(0, [int64ToString(7923209n)])), "\"7923209\"");
}), Test_testCase("an uint64 works", () => {
    assertEqual(toString(0, new Json(0, [(7923209n).toString()])), "\"7923209\"");
}), Test_testCase("an enum<sbyte> works", () => {
    assertEqual(toString(0, Enum_sbyte(99)), "99");
}), Test_testCase("an enum<byte> works", () => {
    assertEqual(toString(0, Enum_byte(99)), "99");
}), Test_testCase("an enum<int> works", () => {
    assertEqual(toString(0, Enum_int(1)), "1");
}), Test_testCase("an enum<uint32> works", () => {
    assertEqual(toString(0, Enum_uint32(99)), "99");
}), Test_testCase("an enum<int16> works", () => {
    assertEqual(toString(0, Enum_int16(99)), "99");
}), Test_testCase("an enum<uint16> works", () => {
    assertEqual(toString(0, Enum_uint16(99)), "99");
}), Test_testCase("a tuple2 works", () => {
    assertEqual(toString(0, tuple2((value_80) => (new Json(13, [value_80])), (value_82) => (new Json(0, [value_82])), 1, "maxime")), "[1,\"maxime\"]");
}), Test_testCase("a tuple3 works", () => {
    assertEqual(toString(0, tuple3((value_85) => (new Json(13, [value_85])), (value_87) => (new Json(0, [value_87])), (value_89) => (new Json(2, [value_89])), 1, "maxime", 2.5)), "[1,\"maxime\",2.5]");
}), Test_testCase("a tuple4 works", () => {
    assertEqual(toString(0, tuple4((value_92) => (new Json(13, [value_92])), (value_94) => (new Json(0, [value_94])), (value_96) => (new Json(2, [value_96])), SmallRecord_Encoder_Z4AB0BC7, 1, "maxime", 2.5, new SmallRecord("test"))), "[1,\"maxime\",2.5,{\"fieldA\":\"test\"}]");
}), Test_testCase("a tuple5 works", () => {
    assertEqual(toString(0, tuple5((value_99) => (new Json(13, [value_99])), (value_101) => (new Json(0, [value_101])), (value_103) => (new Json(2, [value_103])), SmallRecord_Encoder_Z4AB0BC7, (value_105) => (new Json(0, [toString_2(value_105, "O", {})])), 1, "maxime", 2.5, new SmallRecord("test"), create(2018, 10, 1, 11, 12, 55, 0, 1))), "[1,\"maxime\",2.5,{\"fieldA\":\"test\"},\"2018-10-01T11:12:55.000Z\"]");
}), Test_testCase("a tuple6 works", () => {
    assertEqual(toString(0, tuple6((value_109) => (new Json(13, [value_109])), (value_111) => (new Json(0, [value_111])), (value_113) => (new Json(2, [value_113])), SmallRecord_Encoder_Z4AB0BC7, (value_115) => (new Json(5, [value_115])), (_arg_39) => (new Json(4, [])), 1, "maxime", 2.5, new SmallRecord("test"), false, defaultOf())), "[1,\"maxime\",2.5,{\"fieldA\":\"test\"},false,null]");
}), Test_testCase("a tuple7 works", () => {
    assertEqual(toString(0, tuple7((value_118) => (new Json(13, [value_118])), (value_120) => (new Json(0, [value_120])), (value_122) => (new Json(2, [value_122])), SmallRecord_Encoder_Z4AB0BC7, (value_124) => (new Json(5, [value_124])), (_arg_41) => (new Json(4, [])), (value_126) => (new Json(5, [value_126])), 1, "maxime", 2.5, new SmallRecord("test"), false, defaultOf(), true)), "[1,\"maxime\",2.5,{\"fieldA\":\"test\"},false,null,true]");
}), Test_testCase("a tuple8 works", () => {
    assertEqual(toString(0, tuple8((value_129) => (new Json(13, [value_129])), (value_131) => (new Json(0, [value_131])), (value_133) => (new Json(2, [value_133])), SmallRecord_Encoder_Z4AB0BC7, (value_135) => (new Json(5, [value_135])), (_arg_43) => (new Json(4, [])), (value_137) => (new Json(5, [value_137])), (value_139) => (new Json(13, [value_139])), 1, "maxime", 2.5, new SmallRecord("test"), false, defaultOf(), true, 98)), "[1,\"maxime\",2.5,{\"fieldA\":\"test\"},false,null,true,98]");
}), Test_testCase("using pretty space works", () => {
    assertEqual(toString(4, new Json(6, [[["firstname", new Json(0, ["maxime"])], ["age", new Json(13, [25])]]])), "{\n    \"firstname\": \"maxime\",\n    \"age\": 25\n}");
}), Test_testCase("complex structure works", () => {
    const expected_84 = "{\n    \"firstname\": \"maxime\",\n    \"age\": 25,\n    \"address\": {\n        \"street\": \"main road\",\n        \"city\": \"Bordeaux\"\n    }\n}";
    assertEqual(toString(4, new Json(6, [[["firstname", new Json(0, ["maxime"])], ["age", new Json(13, [25])], ["address", new Json(6, [[["street", new Json(0, ["main road"])], ["city", new Json(0, ["Bordeaux"])]]])]]])), expected_84);
}), Test_testCase("option with a value `Some ...` works", () => {
    assertEqual(toString(0, new Json(6, [[["id", new Json(13, [1])], ["operator", option((value_151) => (new Json(0, [value_151])))("maxime")]]])), "{\"id\":1,\"operator\":\"maxime\"}");
}), Test_testCase("option without a value `None` works", () => {
    assertEqual(toString(0, new Json(6, [[["id", new Json(13, [1])], ["operator", option((value_155) => (new Json(0, [value_155])))(void 0)]]])), "{\"id\":1,\"operator\":null}");
})]))));

