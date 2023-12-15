import { Union, Record } from "../Thoth.Json.Tests.JavaScript/fable_modules/fable-library.4.5.0/Types.js";
import { union_type, string_type, record_type, float64_type, int32_type } from "../Thoth.Json.Tests.JavaScript/fable_modules/fable-library.4.5.0/Reflection.js";
import { Json } from "../../packages/Thoth.Json.Core/Types.fs.js";
import { option, tuple8, tuple7, tuple6, tuple5, tuple4, tuple3, tuple2, Enum_uint16, Enum_int16, Enum_uint32, Enum_int, Enum_byte, Enum_sbyte, map, dict, list } from "../../packages/Thoth.Json.Core/Encode.fs.js";
import { singleton, ofArray } from "../Thoth.Json.Tests.JavaScript/fable_modules/fable-library.4.5.0/List.js";
import { ofList } from "../Thoth.Json.Tests.JavaScript/fable_modules/fable-library.4.5.0/Map.js";
import { defaultOf, comparePrimitives } from "../Thoth.Json.Tests.JavaScript/fable_modules/fable-library.4.5.0/Util.js";
import { fromInt32, toString } from "../Thoth.Json.Tests.JavaScript/fable_modules/fable-library.4.5.0/BigInt.js";
import { create, toString as toString_1 } from "../Thoth.Json.Tests.JavaScript/fable_modules/fable-library.4.5.0/Date.js";
import { create as create_1 } from "../Thoth.Json.Tests.JavaScript/fable_modules/fable-library.4.5.0/DateOffset.js";
import { toString as toString_2, create as create_2, fromHours } from "../Thoth.Json.Tests.JavaScript/fable_modules/fable-library.4.5.0/TimeSpan.js";
import { fromParts, toString as toString_3 } from "../Thoth.Json.Tests.JavaScript/fable_modules/fable-library.4.5.0/Decimal.js";
import { SmallRecord, SmallRecord_Encoder_Z60B4A1D2 } from "./Types.fs.js";

export class RecordWithPrivateConstructor extends Record {
    constructor(Foo1, Foo2) {
        super();
        this.Foo1 = (Foo1 | 0);
        this.Foo2 = Foo2;
    }
}

export function RecordWithPrivateConstructor_$reflection() {
    return record_type("Thoth.Json.Tests.Encoders.RecordWithPrivateConstructor", [], RecordWithPrivateConstructor, () => [["Foo1", int32_type], ["Foo2", float64_type]]);
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
    return union_type("Thoth.Json.Tests.Encoders.UnionWithPrivateConstructor", [], UnionWithPrivateConstructor, () => [[["Item", string_type]], []]);
}

export function tests(runner) {
    return runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testList"]()("Thoth.Json.Encode")(singleton(runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testList"]()("Basic")(ofArray([runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a string works")(() => {
        const expected = "\"maxime\"";
        let actual;
        const objectArg = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual = objectArg.toString(0, new Json(0, ["maxime"]));
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected, actual);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a string with new line works")(() => {
        const expected_1 = "\"a\\nb\"";
        let actual_1;
        const objectArg_1 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_1 = objectArg_1.toString(4, new Json(0, ["a\nb"]));
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_1, actual_1);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a string with new line character works")(() => {
        const expected_2 = "\"a\\\\nb\"";
        let actual_2;
        const objectArg_2 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_2 = objectArg_2.toString(4, new Json(0, ["a\\nb"]));
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_2, actual_2);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a string with tab works")(() => {
        const expected_3 = "\"a\\tb\"";
        let actual_3;
        const objectArg_3 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_3 = objectArg_3.toString(4, new Json(0, ["a\tb"]));
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_3, actual_3);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a string with tab character works")(() => {
        const expected_4 = "\"a\\\\tb\"";
        let actual_4;
        const objectArg_4 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_4 = objectArg_4.toString(4, new Json(0, ["a\\tb"]));
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_4, actual_4);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a char works")(() => {
        const expected_5 = "\"a\"";
        let actual_5;
        const objectArg_5 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_5 = objectArg_5.toString(0, new Json(1, ["a"]));
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_5, actual_5);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an int works")(() => {
        const expected_6 = "1";
        let actual_6;
        const arg_25 = new Json(7, [1 >>> 0]);
        const objectArg_6 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_6 = objectArg_6.toString(0, arg_25);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_6, actual_6);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a float works")(() => {
        const expected_7 = "1.2";
        let actual_7;
        const objectArg_7 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_7 = objectArg_7.toString(0, new Json(2, [1.2]));
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_7, actual_7);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an array works")(() => {
        const expected_8 = "[\"maxime\",2]";
        let actual_8;
        const arg_33 = new Json(6, [[new Json(0, ["maxime"]), new Json(7, [2 >>> 0])]]);
        const objectArg_8 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_8 = objectArg_8.toString(0, arg_33);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_8, actual_8);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a list works")(() => {
        const expected_9 = "[\"maxime\",2]";
        let actual_9;
        const arg_37 = list(ofArray([new Json(0, ["maxime"]), new Json(7, [2 >>> 0])]));
        const objectArg_9 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_9 = objectArg_9.toString(0, arg_37);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_9, actual_9);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a bool works")(() => {
        const expected_10 = "false";
        let actual_10;
        const objectArg_10 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_10 = objectArg_10.toString(0, new Json(4, [false]));
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_10, actual_10);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a null works")(() => {
        const expected_11 = "null";
        let actual_11;
        const objectArg_11 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_11 = objectArg_11.toString(0, new Json(3, []));
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_11, actual_11);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("unit works")(() => {
        const expected_12 = "null";
        let actual_12;
        const objectArg_12 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_12 = objectArg_12.toString(0, new Json(8, []));
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_12, actual_12);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an object works")(() => {
        const expected_13 = "{\"firstname\":\"maxime\",\"age\":25}";
        let actual_13;
        const arg_53 = new Json(5, [[["firstname", new Json(0, ["maxime"])], ["age", new Json(7, [25 >>> 0])]]]);
        const objectArg_13 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_13 = objectArg_13.toString(0, arg_53);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_13, actual_13);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a dict works")(() => {
        const expected_14 = "{\"a\":1,\"b\":2,\"c\":3}";
        let actual_14;
        const arg_57 = dict(ofList(ofArray([["a", new Json(7, [1 >>> 0])], ["b", new Json(7, [2 >>> 0])], ["c", new Json(7, [3 >>> 0])]]), {
            Compare: comparePrimitives,
        }));
        const objectArg_14 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_14 = objectArg_14.toString(0, arg_57);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_14, actual_14);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a map works")(() => {
        const expected_15 = "[[\"a\",1],[\"b\",2],[\"c\",3]]";
        let actual_15;
        const arg_61 = map((value_18) => (new Json(0, [value_18])), (value_20) => (new Json(7, [value_20 >>> 0])), ofList(ofArray([["a", 1], ["b", 2], ["c", 3]]), {
            Compare: comparePrimitives,
        }));
        const objectArg_15 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_15 = objectArg_15.toString(0, arg_61);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_15, actual_15);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a bigint works")(() => {
        const expected_16 = "\"12\"";
        let actual_16;
        const arg_65 = new Json(0, [toString(fromInt32(12))]);
        const objectArg_16 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_16 = objectArg_16.toString(0, arg_65);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_16, actual_16);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a datetime works")(() => {
        const expected_17 = "\"2018-10-01T11:12:55.000Z\"";
        let actual_17;
        const arg_69 = new Json(0, [toString_1(create(2018, 10, 1, 11, 12, 55, 0, 1), "O", {})]);
        const objectArg_17 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_17 = objectArg_17.toString(0, arg_69);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_17, actual_17);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a datetimeOffset works")(() => {
        const expected_18 = "\"2018-07-02T12:23:45.000+02:00\"";
        let actual_18;
        const arg_73 = new Json(0, [toString_1(create_1(2018, 7, 2, 12, 23, 45, 0, fromHours(2)), "O", {})]);
        const objectArg_18 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_18 = objectArg_18.toString(0, arg_73);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_18, actual_18);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a timeSpan works")(() => {
        let copyOfStruct;
        const expected_19 = "\"1.02:03:04.0050000\"";
        let actual_19;
        const arg_77 = new Json(0, [(copyOfStruct = create_2(1, 2, 3, 4, 5), toString_2(copyOfStruct))]);
        const objectArg_19 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_19 = objectArg_19.toString(0, arg_77);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_19, actual_19);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a decimal works")(() => {
        const expected_20 = "\"0.7833\"";
        let actual_20;
        const arg_81 = new Json(0, [toString_3(fromParts(7833, 0, 0, false, 4))]);
        const objectArg_20 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_20 = objectArg_20.toString(0, arg_81);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_20, actual_20);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a guid works")(() => {
        let copyOfStruct_1;
        const expected_21 = "\"1e5dee25-8558-4392-a9fb-aae03f81068f\"";
        let actual_21;
        const arg_85 = new Json(0, [(copyOfStruct_1 = "1e5dee25-8558-4392-a9fb-aae03f81068f", copyOfStruct_1)]);
        const objectArg_21 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_21 = objectArg_21.toString(0, arg_85);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_21, actual_21);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an byte works")(() => {
        const expected_22 = "99";
        let actual_22;
        const objectArg_22 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_22 = objectArg_22.toString(0, new Json(7, [99]));
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_22, actual_22);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an sbyte works")(() => {
        const expected_23 = "99";
        let actual_23;
        const arg_93 = new Json(7, [99 >>> 0]);
        const objectArg_23 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_23 = objectArg_23.toString(0, arg_93);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_23, actual_23);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an int16 works")(() => {
        const expected_24 = "99";
        let actual_24;
        const arg_97 = new Json(7, [99 >>> 0]);
        const objectArg_24 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_24 = objectArg_24.toString(0, arg_97);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_24, actual_24);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an uint16 works")(() => {
        const expected_25 = "99";
        let actual_25;
        const objectArg_25 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_25 = objectArg_25.toString(0, new Json(7, [99]));
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_25, actual_25);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an int64 works")(() => {
        const expected_26 = "\"7923209\"";
        let actual_26;
        const arg_105 = new Json(0, [String(7923209n)]);
        const objectArg_26 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_26 = objectArg_26.toString(0, arg_105);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_26, actual_26);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an uint64 works")(() => {
        const expected_27 = "\"7923209\"";
        let actual_27;
        const arg_109 = new Json(0, [String(7923209n)]);
        const objectArg_27 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_27 = objectArg_27.toString(0, arg_109);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_27, actual_27);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an enum<sbyte> works")(() => {
        const expected_28 = "99";
        let actual_28;
        const arg_113 = Enum_sbyte(99);
        const objectArg_28 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_28 = objectArg_28.toString(0, arg_113);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_28, actual_28);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an enum<byte> works")(() => {
        const expected_29 = "99";
        let actual_29;
        const arg_117 = Enum_byte(99);
        const objectArg_29 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_29 = objectArg_29.toString(0, arg_117);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_29, actual_29);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an enum<int> works")(() => {
        const expected_30 = "1";
        let actual_30;
        const arg_121 = Enum_int(1);
        const objectArg_30 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_30 = objectArg_30.toString(0, arg_121);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_30, actual_30);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an enum<uint32> works")(() => {
        const expected_31 = "99";
        let actual_31;
        const arg_125 = Enum_uint32(99);
        const objectArg_31 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_31 = objectArg_31.toString(0, arg_125);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_31, actual_31);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an enum<int16> works")(() => {
        const expected_32 = "99";
        let actual_32;
        const arg_129 = Enum_int16(99);
        const objectArg_32 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_32 = objectArg_32.toString(0, arg_129);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_32, actual_32);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an enum<uint16> works")(() => {
        const expected_33 = "99";
        let actual_33;
        const arg_133 = Enum_uint16(99);
        const objectArg_33 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_33 = objectArg_33.toString(0, arg_133);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_33, actual_33);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a tuple2 works")(() => {
        const expected_34 = "[1,\"maxime\"]";
        let actual_34;
        const arg_137 = tuple2((value_53) => (new Json(7, [value_53 >>> 0])), (value_55) => (new Json(0, [value_55])), 1, "maxime");
        const objectArg_34 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_34 = objectArg_34.toString(0, arg_137);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_34, actual_34);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a tuple3 works")(() => {
        const expected_35 = "[1,\"maxime\",2.5]";
        let actual_35;
        const arg_141 = tuple3((value_57) => (new Json(7, [value_57 >>> 0])), (value_59) => (new Json(0, [value_59])), (value_61) => (new Json(2, [value_61])), 1, "maxime", 2.5);
        const objectArg_35 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_35 = objectArg_35.toString(0, arg_141);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_35, actual_35);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a tuple4 works")(() => {
        const expected_36 = "[1,\"maxime\",2.5,{\"fieldA\":\"test\"}]";
        let actual_36;
        const arg_145 = tuple4((value_63) => (new Json(7, [value_63 >>> 0])), (value_65) => (new Json(0, [value_65])), (value_67) => (new Json(2, [value_67])), SmallRecord_Encoder_Z60B4A1D2, 1, "maxime", 2.5, new SmallRecord("test"));
        const objectArg_36 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_36 = objectArg_36.toString(0, arg_145);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_36, actual_36);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a tuple5 works")(() => {
        const expected_37 = "[1,\"maxime\",2.5,{\"fieldA\":\"test\"},\"2018-10-01T11:12:55.000Z\"]";
        let actual_37;
        const arg_149 = tuple5((value_69) => (new Json(7, [value_69 >>> 0])), (value_71) => (new Json(0, [value_71])), (value_73) => (new Json(2, [value_73])), SmallRecord_Encoder_Z60B4A1D2, (value_75) => (new Json(0, [toString_1(value_75, "O", {})])), 1, "maxime", 2.5, new SmallRecord("test"), create(2018, 10, 1, 11, 12, 55, 0, 1));
        const objectArg_37 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_37 = objectArg_37.toString(0, arg_149);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_37, actual_37);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a tuple6 works")(() => {
        const expected_38 = "[1,\"maxime\",2.5,{\"fieldA\":\"test\"},false,null]";
        let actual_38;
        const arg_153 = tuple6((value_78) => (new Json(7, [value_78 >>> 0])), (value_80) => (new Json(0, [value_80])), (value_82) => (new Json(2, [value_82])), SmallRecord_Encoder_Z60B4A1D2, (value_84) => (new Json(4, [value_84])), (_arg_39) => (new Json(3, [])), 1, "maxime", 2.5, new SmallRecord("test"), false, defaultOf());
        const objectArg_38 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_38 = objectArg_38.toString(0, arg_153);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_38, actual_38);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a tuple7 works")(() => {
        const expected_39 = "[1,\"maxime\",2.5,{\"fieldA\":\"test\"},false,null,true]";
        let actual_39;
        const arg_157 = tuple7((value_86) => (new Json(7, [value_86 >>> 0])), (value_88) => (new Json(0, [value_88])), (value_90) => (new Json(2, [value_90])), SmallRecord_Encoder_Z60B4A1D2, (value_92) => (new Json(4, [value_92])), (_arg_41) => (new Json(3, [])), (value_94) => (new Json(4, [value_94])), 1, "maxime", 2.5, new SmallRecord("test"), false, defaultOf(), true);
        const objectArg_39 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_39 = objectArg_39.toString(0, arg_157);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_39, actual_39);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a tuple8 works")(() => {
        const expected_40 = "[1,\"maxime\",2.5,{\"fieldA\":\"test\"},false,null,true,98]";
        let actual_40;
        const arg_161 = tuple8((value_96) => (new Json(7, [value_96 >>> 0])), (value_98) => (new Json(0, [value_98])), (value_100) => (new Json(2, [value_100])), SmallRecord_Encoder_Z60B4A1D2, (value_102) => (new Json(4, [value_102])), (_arg_43) => (new Json(3, [])), (value_104) => (new Json(4, [value_104])), (value_106) => (new Json(7, [value_106 >>> 0])), 1, "maxime", 2.5, new SmallRecord("test"), false, defaultOf(), true, 98);
        const objectArg_40 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_40 = objectArg_40.toString(0, arg_161);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_40, actual_40);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("using pretty space works")(() => {
        const expected_41 = "{\n    \"firstname\": \"maxime\",\n    \"age\": 25\n}";
        let actual_41;
        const arg_165 = new Json(5, [[["firstname", new Json(0, ["maxime"])], ["age", new Json(7, [25 >>> 0])]]]);
        const objectArg_41 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_41 = objectArg_41.toString(4, arg_165);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_41, actual_41);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("complex structure works")(() => {
        const expected_42 = "{\n    \"firstname\": \"maxime\",\n    \"age\": 25,\n    \"address\": {\n        \"street\": \"main road\",\n        \"city\": \"Bordeaux\"\n    }\n}";
        let actual_42;
        const arg_169 = new Json(5, [[["firstname", new Json(0, ["maxime"])], ["age", new Json(7, [25 >>> 0])], ["address", new Json(5, [[["street", new Json(0, ["main road"])], ["city", new Json(0, ["Bordeaux"])]]])]]]);
        const objectArg_42 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_42 = objectArg_42.toString(4, arg_169);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_42, actual_42);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("option with a value `Some ...` works")(() => {
        const expected_43 = "{\"id\":1,\"operator\":\"maxime\"}";
        let actual_43;
        const arg_173 = new Json(5, [[["id", new Json(7, [1 >>> 0])], ["operator", option((value_115) => (new Json(0, [value_115])))("maxime")]]]);
        const objectArg_43 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_43 = objectArg_43.toString(0, arg_173);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_43, actual_43);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("option without a value `None` works")(() => {
        const expected_44 = "{\"id\":1,\"operator\":null}";
        let actual_44;
        const arg_177 = new Json(5, [[["id", new Json(7, [1 >>> 0])], ["operator", option((value_118) => (new Json(0, [value_118])))(void 0)]]]);
        const objectArg_44 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        actual_44 = objectArg_44.toString(0, arg_177);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_44, actual_44);
    })]))));
}

