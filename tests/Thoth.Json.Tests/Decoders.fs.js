import { Union, Record } from "../Thoth.Json.Tests.JavaScript/fable_modules/fable-library.4.5.0/Types.js";
import { union_type, string_type, record_type, float64_type, int32_type } from "../Thoth.Json.Tests.JavaScript/fable_modules/fable-library.4.5.0/Reflection.js";
import { FSharpChoice$2, FSharpResult$2 } from "../Thoth.Json.Tests.JavaScript/fable_modules/fable-library.4.5.0/Choice.js";
import { map8, map7, map6, map5, map4, map3, all, andThen as andThen_1, andMap, fail, succeed as succeed_1, optionalAt, optional, option as option_3, map as map_1, object, oneOf, map$0027, map2, dict, keyValuePairs, keys as keys_1, array, list as list_5, index, at, field, tuple8, tuple7, nil, tuple6, tuple5, tuple4, tuple3, tuple2, timespan, datetimeOffset, datetimeLocal, datetimeUtc, bigint, sbyte, byte, uint64, uint32, int64, uint16, int16, int, bool, char, string, unit, float } from "../../packages/Thoth.Json.Core/Decode.fs.js";
import { Data, Post, Person, MediumRecord, MyObj2, Shape, MyObj, Shape_get_DecoderRectangle, Shape_get_DecoderCircle, User_Create, User, Model, SmallRecord2, Record8_Create, Record8, Record7_Create, Record7, Record6_Create, Record6, Record5_Create, Record5, Record4_Create, Record4, Record3_Create, Record3, Record2, Record10, Record10_Create, Price, Record2_Create, SmallRecord_get_Decoder, SmallRecord, CustomException } from "./Types.fs.js";
import { empty, map, singleton, ofArray } from "../Thoth.Json.Tests.JavaScript/fable_modules/fable-library.4.5.0/List.js";
import { fromInt32 } from "../Thoth.Json.Tests.JavaScript/fable_modules/fable-library.4.5.0/BigInt.js";
import { toString, toUniversalTime, create } from "../Thoth.Json.Tests.JavaScript/fable_modules/fable-library.4.5.0/Date.js";
import { toConsole, printf, toText } from "../Thoth.Json.Tests.JavaScript/fable_modules/fable-library.4.5.0/String.js";
import { create as create_1 } from "../Thoth.Json.Tests.JavaScript/fable_modules/fable-library.4.5.0/DateOffset.js";
import { create as create_2, fromHours } from "../Thoth.Json.Tests.JavaScript/fable_modules/fable-library.4.5.0/TimeSpan.js";
import { uint16 as uint16_1, int16 as int16_1, uint32 as uint32_1, int as int_1, byte as byte_1, sbyte as sbyte_1, succeed, andThen } from "../../packages/Thoth.Json.Core/./Decode.fs.js";
import { stringHash, int32ToString, compareArrays, comparePrimitives, defaultOf } from "../Thoth.Json.Tests.JavaScript/fable_modules/fable-library.4.5.0/Util.js";
import { list as list_6 } from "../../packages/Thoth.Json.Core/Encode.fs.js";
import { Json } from "../../packages/Thoth.Json.Core/Types.fs.js";
import { ofList } from "../Thoth.Json.Tests.JavaScript/fable_modules/fable-library.4.5.0/Map.js";
import { List_except } from "../Thoth.Json.Tests.JavaScript/fable_modules/fable-library.4.5.0/Seq2.js";
import { defaultArg } from "../Thoth.Json.Tests.JavaScript/fable_modules/fable-library.4.5.0/Option.js";
import { head } from "../Thoth.Json.Tests.JavaScript/fable_modules/fable-library.4.5.0/Seq.js";

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
    return record_type("Thoth.Json.Tests.Decoders.RecordWithPrivateConstructor", [], RecordWithPrivateConstructor, () => [["Foo1", int32_type], ["Foo2", float64_type]]);
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
    return union_type("Thoth.Json.Tests.Decoders.UnionWithPrivateConstructor", [], UnionWithPrivateConstructor, () => [[["Item", string_type]], []]);
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
    return union_type("Thoth.Json.Tests.Decoders.UnionWithMultipleFields", [], UnionWithMultipleFields, () => [[["Item1", string_type], ["Item2", int32_type], ["Item3", float64_type]]]);
}

export function tests(runner) {
    return runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testList"]()("Thoth.Json.Decode")(ofArray([runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testList"]()("Errors")(ofArray([runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("invalid json")(() => {
        const expected = new FSharpResult$2(1, ["Given an invalid JSON: Unexpected token \'m\', \"maxime\" is not valid JSON"]);
        let actual;
        const objectArg = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual = objectArg.fromString(float, "maxime");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected, actual);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("invalid json #3 - Special case for Thoth.Json.Net")(() => {
        const expected_1 = new FSharpResult$2(1, ["Given an invalid JSON: Expected double-quoted property name in JSON at position 172"]);
        const incorrectJson = "\n                {\n                \"Ab\": [\n                    \"RecordC\",\n                    {\n                    \"C1\": \"\",\n                    \"C2\": \"\",\n                ";
        let actual_1;
        const objectArg_1 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_1 = objectArg_1.fromString(float, incorrectJson);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_1, actual_1);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("user exceptions are not captured by the decoders")(() => {
        let objectArg_2;
        const expected_2 = true;
        const decoder = {
            Decode(_arg_3, _arg_4) {
                throw new CustomException();
            },
        };
        let actual_2;
        try {
            (objectArg_2 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"](), objectArg_2.fromString(decoder, "\"maxime\""));
            actual_2 = false;
        }
        catch (matchValue) {
            if (matchValue instanceof CustomException) {
                actual_2 = true;
            }
            else {
                throw matchValue;
            }
        }
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_2, actual_2);
    })])), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testList"]()("Primitives")(ofArray([runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("unit works")(() => {
        const expected_3 = new FSharpResult$2(0, [void 0]);
        let actual_3;
        const objectArg_3 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_3 = objectArg_3.fromString(unit, "null");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_3, actual_3);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a string works")(() => {
        const expected_4 = new FSharpResult$2(0, ["maxime"]);
        let actual_4;
        const objectArg_4 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_4 = objectArg_4.fromString(string, "\"maxime\"");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_4, actual_4);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a string with new line works")(() => {
        const expected_5 = new FSharpResult$2(0, ["a\nb"]);
        let actual_5;
        const objectArg_5 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_5 = objectArg_5.fromString(string, "\"a\\nb\"");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_5, actual_5);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a string with new line character works")(() => {
        const expected_6 = new FSharpResult$2(0, ["a\\nb"]);
        let actual_6;
        const objectArg_6 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_6 = objectArg_6.fromString(string, "\"a\\\\nb\"");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_6, actual_6);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a string with tab works")(() => {
        const expected_7 = new FSharpResult$2(0, ["a\tb"]);
        let actual_7;
        const objectArg_7 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_7 = objectArg_7.fromString(string, "\"a\\tb\"");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_7, actual_7);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a string with tab character works")(() => {
        const expected_8 = new FSharpResult$2(0, ["a\\tb"]);
        let actual_8;
        const objectArg_8 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_8 = objectArg_8.fromString(string, "\"a\\\\tb\"");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_8, actual_8);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a char works")(() => {
        const expected_9 = new FSharpResult$2(0, ["a"]);
        let actual_9;
        const objectArg_9 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_9 = objectArg_9.fromString(char, "\"a\"");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_9, actual_9);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a char reports an error if there are more than 1 characters in the string")(() => {
        const expected_10 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a single character string but instead got: \"ab\"\n                        ".trim()]);
        let actual_10;
        const objectArg_10 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_10 = objectArg_10.fromString(char, "\"ab\"");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_10, actual_10);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a float works")(() => {
        const expected_11 = new FSharpResult$2(0, [1.2]);
        let actual_11;
        const objectArg_11 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_11 = objectArg_11.fromString(float, "1.2");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_11, actual_11);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a float from int works")(() => {
        const expected_12 = new FSharpResult$2(0, [1]);
        let actual_12;
        const objectArg_12 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_12 = objectArg_12.fromString(float, "1");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_12, actual_12);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a bool works")(() => {
        const expected_13 = new FSharpResult$2(0, [true]);
        let actual_13;
        const objectArg_13 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_13 = objectArg_13.fromString(bool, "true");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_13, actual_13);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an invalid bool output an error")(() => {
        const expected_14 = new FSharpResult$2(1, ["Error at: `$`\nExpecting a boolean but instead got: 2"]);
        let actual_14;
        const objectArg_14 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_14 = objectArg_14.fromString(bool, "2");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_14, actual_14);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an int works")(() => {
        const expected_15 = new FSharpResult$2(0, [25]);
        let actual_15;
        const objectArg_15 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_15 = objectArg_15.fromString(int, "25");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_15, actual_15);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an invalid int [invalid range: too big] output an error")(() => {
        const expected_16 = new FSharpResult$2(1, ["Error at: `$`\nExpecting an int but instead got: 2147483648\nReason: Value was either too large or too small for an int"]);
        let actual_16;
        const objectArg_16 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_16 = objectArg_16.fromString(int, "2147483648");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_16, actual_16);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an invalid int [invalid range: too small] output an error")(() => {
        const expected_17 = new FSharpResult$2(1, ["Error at: `$`\nExpecting an int but instead got: -2147483649\nReason: Value was either too large or too small for an int"]);
        let actual_17;
        const objectArg_17 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_17 = objectArg_17.fromString(int, "-2147483649");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_17, actual_17);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an int16 works from number")(() => {
        const expected_18 = new FSharpResult$2(0, [(25 + 0x8000 & 0xFFFF) - 0x8000]);
        let actual_18;
        const objectArg_18 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_18 = objectArg_18.fromString(int16, "25");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_18, actual_18);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an int16 works from string")(() => {
        const expected_19 = new FSharpResult$2(0, [(-25 + 0x8000 & 0xFFFF) - 0x8000]);
        let actual_19;
        const objectArg_19 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_19 = objectArg_19.fromString(int16, "\"-25\"");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_19, actual_19);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an int16 output an error if value is too big")(() => {
        const expected_20 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an int16 but instead got: 32768\nReason: Value was either too large or too small for an int16\n                        ".trim()]);
        let actual_20;
        const objectArg_20 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_20 = objectArg_20.fromString(int16, "32768");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_20, actual_20);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an int16 output an error if value is too small")(() => {
        const expected_21 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an int16 but instead got: -32769\nReason: Value was either too large or too small for an int16\n                        ".trim()]);
        let actual_21;
        const objectArg_21 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_21 = objectArg_21.fromString(int16, "-32769");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_21, actual_21);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an int16 output an error if incorrect string")(() => {
        const expected_22 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an int16 but instead got: \"maxime\"\n                        ".trim()]);
        let actual_22;
        const objectArg_22 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_22 = objectArg_22.fromString(int16, "\"maxime\"");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_22, actual_22);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an uint16 works from number")(() => {
        const expected_23 = new FSharpResult$2(0, [25 & 0xFFFF]);
        let actual_23;
        const objectArg_23 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_23 = objectArg_23.fromString(uint16, "25");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_23, actual_23);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an uint16 works from string")(() => {
        const expected_24 = new FSharpResult$2(0, [25 & 0xFFFF]);
        let actual_24;
        const objectArg_24 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_24 = objectArg_24.fromString(uint16, "\"25\"");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_24, actual_24);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an uint16 output an error if value is too big")(() => {
        const expected_25 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an uint16 but instead got: 65536\nReason: Value was either too large or too small for an uint16\n                        ".trim()]);
        let actual_25;
        const objectArg_25 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_25 = objectArg_25.fromString(uint16, "65536");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_25, actual_25);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an uint16 output an error if value is too small")(() => {
        const expected_26 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an uint16 but instead got: -1\nReason: Value was either too large or too small for an uint16\n                        ".trim()]);
        let actual_26;
        const objectArg_26 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_26 = objectArg_26.fromString(uint16, "-1");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_26, actual_26);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an uint16 output an error if incorrect string")(() => {
        const expected_27 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an uint16 but instead got: \"maxime\"\n                        ".trim()]);
        let actual_27;
        const objectArg_27 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_27 = objectArg_27.fromString(uint16, "\"maxime\"");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_27, actual_27);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an int64 works from number")(() => {
        const expected_28 = new FSharpResult$2(0, [1000n]);
        let actual_28;
        const objectArg_28 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_28 = objectArg_28.fromString(int64, "1000");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_28, actual_28);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an int64 works from string")(() => {
        const expected_29 = new FSharpResult$2(0, [99n]);
        let actual_29;
        const objectArg_29 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_29 = objectArg_29.fromString(int64, "\"99\"");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_29, actual_29);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an int64 works output an error if incorrect string")(() => {
        const expected_30 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an int64 but instead got: \"maxime\"\n                        ".trim()]);
        let actual_30;
        const objectArg_30 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_30 = objectArg_30.fromString(int64, "\"maxime\"");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_30, actual_30);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an uint32 works from number")(() => {
        const expected_31 = new FSharpResult$2(0, [1000]);
        let actual_31;
        const objectArg_31 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_31 = objectArg_31.fromString(uint32, "1000");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_31, actual_31);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an uint32 works from string")(() => {
        const expected_32 = new FSharpResult$2(0, [1000]);
        let actual_32;
        const objectArg_32 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_32 = objectArg_32.fromString(uint32, "\"1000\"");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_32, actual_32);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an uint32 output an error if incorrect string")(() => {
        const expected_33 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an uint32 but instead got: \"maxime\"\n                        ".trim()]);
        let actual_33;
        const objectArg_33 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_33 = objectArg_33.fromString(uint32, "\"maxime\"");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_33, actual_33);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an uint64 works from number")(() => {
        const expected_34 = new FSharpResult$2(0, [1000n]);
        let actual_34;
        const objectArg_34 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_34 = objectArg_34.fromString(uint64, "1000");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_34, actual_34);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an uint64 works from string")(() => {
        const expected_35 = new FSharpResult$2(0, [1000n]);
        let actual_35;
        const objectArg_35 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_35 = objectArg_35.fromString(uint64, "\"1000\"");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_35, actual_35);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an uint64 output an error if incorrect string")(() => {
        const expected_36 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an uint64 but instead got: \"maxime\"\n                        ".trim()]);
        let actual_36;
        const objectArg_36 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_36 = objectArg_36.fromString(uint64, "\"maxime\"");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_36, actual_36);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a byte works from number")(() => {
        const expected_37 = new FSharpResult$2(0, [25]);
        let actual_37;
        const objectArg_37 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_37 = objectArg_37.fromString(byte, "25");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_37, actual_37);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a byte works from string")(() => {
        const expected_38 = new FSharpResult$2(0, [25]);
        let actual_38;
        const objectArg_38 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_38 = objectArg_38.fromString(byte, "\"25\"");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_38, actual_38);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a byte output an error if value is too big")(() => {
        const expected_39 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a byte but instead got: 256\nReason: Value was either too large or too small for a byte\n                        ".trim()]);
        let actual_39;
        const objectArg_39 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_39 = objectArg_39.fromString(byte, "256");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_39, actual_39);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a byte output an error if value is too small")(() => {
        const expected_40 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a byte but instead got: -1\nReason: Value was either too large or too small for a byte\n                        ".trim()]);
        let actual_40;
        const objectArg_40 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_40 = objectArg_40.fromString(byte, "-1");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_40, actual_40);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a byte output an error if incorrect string")(() => {
        const expected_41 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a byte but instead got: \"maxime\"\n                        ".trim()]);
        let actual_41;
        const objectArg_41 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_41 = objectArg_41.fromString(byte, "\"maxime\"");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_41, actual_41);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a sbyte works from number")(() => {
        const expected_42 = new FSharpResult$2(0, [25]);
        let actual_42;
        const objectArg_42 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_42 = objectArg_42.fromString(sbyte, "25");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_42, actual_42);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a sbyte works from string")(() => {
        const expected_43 = new FSharpResult$2(0, [-25]);
        let actual_43;
        const objectArg_43 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_43 = objectArg_43.fromString(sbyte, "\"-25\"");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_43, actual_43);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a sbyte output an error if value is too big")(() => {
        const expected_44 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a sbyte but instead got: 128\nReason: Value was either too large or too small for a sbyte\n                        ".trim()]);
        let actual_44;
        const objectArg_44 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_44 = objectArg_44.fromString(sbyte, "128");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_44, actual_44);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a sbyte output an error if value is too small")(() => {
        const expected_45 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a sbyte but instead got: -129\nReason: Value was either too large or too small for a sbyte\n                        ".trim()]);
        let actual_45;
        const objectArg_45 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_45 = objectArg_45.fromString(sbyte, "-129");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_45, actual_45);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a sbyte output an error if incorrect string")(() => {
        const expected_46 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a sbyte but instead got: \"maxime\"\n                        ".trim()]);
        let actual_46;
        const objectArg_46 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_46 = objectArg_46.fromString(sbyte, "\"maxime\"");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_46, actual_46);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an bigint works from number")(() => {
        const expected_47 = new FSharpResult$2(0, [fromInt32(12)]);
        let actual_47;
        const objectArg_47 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_47 = objectArg_47.fromString(bigint, "12");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_47, actual_47);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an bigint works from string")(() => {
        const expected_48 = new FSharpResult$2(0, [fromInt32(12)]);
        let actual_48;
        const objectArg_48 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_48 = objectArg_48.fromString(bigint, "\"12\"");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_48, actual_48);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an bigint output an error if invalid string")(() => {
        const expected_49 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a bigint but instead got: \"maxime\"\n                        ".trim()]);
        let actual_49;
        const objectArg_49 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_49 = objectArg_49.fromString(bigint, "\"maxime\"");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_49, actual_49);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a string representing a DateTime should be accepted as a string")(() => {
        const expected_50 = "2018-10-01T11:12:55.00Z";
        let actual_50;
        const objectArg_50 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_50 = objectArg_50.fromString(string, "\"2018-10-01T11:12:55.00Z\"");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](new FSharpResult$2(0, [expected_50]), actual_50);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a datetime works")(() => {
        const expected_51 = create(2018, 10, 1, 11, 12, 55, 0, 1);
        let actual_51;
        const objectArg_51 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_51 = objectArg_51.fromString(datetimeUtc, "\"2018-10-01T11:12:55.00Z\"");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](new FSharpResult$2(0, [expected_51]), actual_51);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a non-UTC datetime works")(() => {
        const expected_52 = create(2018, 10, 1, 11, 12, 55);
        let actual_52;
        const objectArg_52 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_52 = objectArg_52.fromString(datetimeLocal, "\"2018-10-01T11:12:55\"");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](new FSharpResult$2(0, [expected_52]), actual_52);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a datetime output an error if invalid string")(() => {
        const expected_53 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a datetime but instead got: \"invalid_string\"\n                        ".trim()]);
        let actual_53;
        const objectArg_53 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_53 = objectArg_53.fromString(datetimeUtc, "\"invalid_string\"");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_53, actual_53);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a datetime works with TimeZone")(() => {
        const localDate = create(2018, 10, 1, 11, 12, 55, 0, 2);
        const expected_54 = new FSharpResult$2(0, [toUniversalTime(localDate)]);
        let json;
        const arg_216 = toString(localDate, "O");
        json = toText(printf("\"%s\""))(arg_216);
        let actual_54;
        const objectArg_54 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_54 = objectArg_54.fromString(datetimeUtc, json);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_54, actual_54);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a datetimeOffset works")(() => {
        const expected_55 = new FSharpResult$2(0, [create_1(2018, 7, 2, 12, 23, 45, 0, fromHours(2))]);
        const json_1 = "\"2018-07-02T12:23:45+02:00\"";
        let actual_55;
        const objectArg_55 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_55 = objectArg_55.fromString(datetimeOffset, json_1);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_55, actual_55);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a datetimeOffset returns Error if invalid format")(() => {
        const expected_56 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a datetimeoffset but instead got: \"NOT A DATETIMEOFFSET\"\n                        ".trim()]);
        const json_2 = "\"NOT A DATETIMEOFFSET\"";
        let actual_56;
        const objectArg_56 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_56 = objectArg_56.fromString(datetimeOffset, json_2);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_56, actual_56);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a timespan works")(() => {
        const expected_57 = new FSharpResult$2(0, [create_2(23, 45, 0)]);
        const json_3 = "\"23:45:00\"";
        let actual_57;
        const objectArg_57 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_57 = objectArg_57.fromString(timespan, json_3);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_57, actual_57);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("a timespan returns Error if invalid format")(() => {
        const expected_58 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a timespan but instead got: \"NOT A TimeSpan\"\n                        ".trim()]);
        const json_4 = "\"NOT A TimeSpan\"";
        let actual_58;
        const objectArg_58 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_58 = objectArg_58.fromString(timespan, json_4);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_58, actual_58);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an enum<sbyte> works")(() => {
        const expected_59 = new FSharpResult$2(0, [99]);
        let actual_59;
        const arg_237 = andThen(succeed, sbyte_1);
        const objectArg_59 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_59 = objectArg_59.fromString(arg_237, "99");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_59, actual_59);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an enum<byte> works")(() => {
        const expected_60 = new FSharpResult$2(0, [99]);
        let actual_60;
        const arg_241 = andThen(succeed, byte_1);
        const objectArg_60 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_60 = objectArg_60.fromString(arg_241, "99");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_60, actual_60);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an enum<int> works")(() => {
        const expected_61 = new FSharpResult$2(0, [1]);
        let actual_61;
        const arg_245 = andThen(succeed, int_1);
        const objectArg_61 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_61 = objectArg_61.fromString(arg_245, "1");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_61, actual_61);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an enum<uint32> works")(() => {
        const expected_62 = new FSharpResult$2(0, [99]);
        let actual_62;
        const arg_249 = andThen(succeed, uint32_1);
        const objectArg_62 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_62 = objectArg_62.fromString(arg_249, "99");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_62, actual_62);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an enum<int16> works")(() => {
        const expected_63 = new FSharpResult$2(0, [99]);
        let actual_63;
        const arg_253 = andThen(succeed, int16_1);
        const objectArg_63 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_63 = objectArg_63.fromString(arg_253, "99");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_63, actual_63);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an enum<uint16> works")(() => {
        const expected_64 = new FSharpResult$2(0, [99]);
        let actual_64;
        const arg_257 = andThen(succeed, uint16_1);
        const objectArg_64 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_64 = objectArg_64.fromString(arg_257, "99");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_64, actual_64);
    })])), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testList"]()("Tuples")(ofArray([runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("tuple2 works")(() => {
        const json_5 = "[1, \"maxime\"]";
        const expected_65 = new FSharpResult$2(0, [[1, "maxime"]]);
        let actual_65;
        const arg_261 = tuple2(int, string);
        const objectArg_65 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_65 = objectArg_65.fromString(arg_261, json_5);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_65, actual_65);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("tuple3 works")(() => {
        const json_6 = "[1, \"maxime\", 2.5]";
        const expected_66 = new FSharpResult$2(0, [[1, "maxime", 2.5]]);
        let actual_66;
        const arg_265 = tuple3(int, string, float);
        const objectArg_66 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_66 = objectArg_66.fromString(arg_265, json_6);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_66, actual_66);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("tuple4 works")(() => {
        const json_7 = "[1, \"maxime\", 2.5, { \"fieldA\" : \"test\" }]";
        const expected_67 = new FSharpResult$2(0, [[1, "maxime", 2.5, new SmallRecord("test")]]);
        let actual_67;
        const arg_269 = tuple4(int, string, float, SmallRecord_get_Decoder());
        const objectArg_67 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_67 = objectArg_67.fromString(arg_269, json_7);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_67, actual_67);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("tuple5 works")(() => {
        const json_8 = "[1, \"maxime\", 2.5, { \"fieldA\" : \"test\" }, false]";
        const expected_68 = new FSharpResult$2(0, [[1, "maxime", 2.5, new SmallRecord("test"), false]]);
        let actual_68;
        const arg_273 = tuple5(int, string, float, SmallRecord_get_Decoder(), bool);
        const objectArg_68 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_68 = objectArg_68.fromString(arg_273, json_8);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_68, actual_68);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("tuple6 works")(() => {
        const json_9 = "[1, \"maxime\", 2.5, { \"fieldA\" : \"test\" }, false, null]";
        const expected_69 = new FSharpResult$2(0, [[1, "maxime", 2.5, new SmallRecord("test"), false, defaultOf()]]);
        let actual_69;
        const arg_277 = tuple6(int, string, float, SmallRecord_get_Decoder(), bool, nil(defaultOf()));
        const objectArg_69 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_69 = objectArg_69.fromString(arg_277, json_9);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_69, actual_69);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("tuple7 works")(() => {
        const json_10 = "[1, \"maxime\", 2.5, { \"fieldA\" : \"test\" }, false, null, 56]";
        const expected_70 = new FSharpResult$2(0, [[1, "maxime", 2.5, new SmallRecord("test"), false, defaultOf(), 56]]);
        let actual_70;
        const arg_281 = tuple7(int, string, float, SmallRecord_get_Decoder(), bool, nil(defaultOf()), int);
        const objectArg_70 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_70 = objectArg_70.fromString(arg_281, json_10);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_70, actual_70);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("tuple8 works")(() => {
        const json_11 = "[1, \"maxime\", 2.5, { \"fieldA\" : \"test\" }, false, null, true, 98]";
        const expected_71 = new FSharpResult$2(0, [[1, "maxime", 2.5, new SmallRecord("test"), false, defaultOf(), true, 98]]);
        let actual_71;
        const arg_285 = tuple8(int, string, float, SmallRecord_get_Decoder(), bool, nil(defaultOf()), bool, int);
        const objectArg_71 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_71 = objectArg_71.fromString(arg_285, json_11);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_71, actual_71);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("tuple2 returns an error if invalid json")(() => {
        const json_12 = "[1, false, \"unused value\"]";
        const expected_72 = new FSharpResult$2(1, ["\nError at: `$.[1]`\nExpecting a string but instead got: false\n                        ".trim()]);
        let actual_72;
        const arg_289 = tuple2(int, string);
        const objectArg_72 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_72 = objectArg_72.fromString(arg_289, json_12);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_72, actual_72);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("tuple3 returns an error if invalid json")(() => {
        const json_13 = "[1, \"maxime\", false]";
        const expected_73 = new FSharpResult$2(1, ["\nError at: `$.[2]`\nExpecting a float but instead got: false\n                        ".trim()]);
        let actual_73;
        const arg_293 = tuple3(int, string, float);
        const objectArg_73 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_73 = objectArg_73.fromString(arg_293, json_13);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_73, actual_73);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("tuple4 returns an error if invalid json (missing index)")(() => {
        const json_14 = "[1, \"maxime\", 2.5]";
        const expected_74 = new FSharpResult$2(1, ["\nError at: `$.[3]`\nExpecting a longer array. Need index `3` but there are only `3` entries.\n[\n    1,\n    \"maxime\",\n    2.5\n]\n                        ".trim()]);
        let actual_74;
        const arg_297 = tuple4(int, string, float, SmallRecord_get_Decoder());
        const objectArg_74 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_74 = objectArg_74.fromString(arg_297, json_14);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_74, actual_74);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("tuple4 returns an error if invalid json (error in the nested object)")(() => {
        const json_15 = "[1, \"maxime\", 2.5, { \"fieldA\" : false }]";
        const expected_75 = new FSharpResult$2(1, ["\nError at: `$.[3].fieldA`\nExpecting a string but instead got: false\n                        ".trim()]);
        let actual_75;
        const arg_301 = tuple4(int, string, float, SmallRecord_get_Decoder());
        const objectArg_75 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_75 = objectArg_75.fromString(arg_301, json_15);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_75, actual_75);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("tuple5 returns an error if invalid json")(() => {
        const json_16 = "[1, \"maxime\", 2.5, { \"fieldA\" : \"test\" }, false]";
        const expected_76 = new FSharpResult$2(1, ["\nError at: `$.[4]`\nExpecting a datetime but instead got: false\n                        ".trim()]);
        let actual_76;
        const arg_305 = tuple5(int, string, float, SmallRecord_get_Decoder(), datetimeUtc);
        const objectArg_76 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_76 = objectArg_76.fromString(arg_305, json_16);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_76, actual_76);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("tuple6 returns an error if invalid json")(() => {
        const json_17 = "[1, \"maxime\", 2.5, { \"fieldA\" : \"test\" }, \"2018-10-01T11:12:55.00Z\", false]";
        const expected_77 = new FSharpResult$2(1, ["\nError at: `$.[5]`\nExpecting null but instead got: false\n                        ".trim()]);
        let actual_77;
        const arg_309 = tuple6(int, string, float, SmallRecord_get_Decoder(), datetimeUtc, nil(defaultOf()));
        const objectArg_77 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_77 = objectArg_77.fromString(arg_309, json_17);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_77, actual_77);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("tuple7 returns an error if invalid json")(() => {
        const json_18 = "[1, \"maxime\", 2.5, { \"fieldA\" : \"test\" }, \"2018-10-01T11:12:55.00Z\", null, false]";
        const expected_78 = new FSharpResult$2(1, ["\nError at: `$.[6]`\nExpecting an int but instead got: false\n                        ".trim()]);
        let actual_78;
        const arg_313 = tuple7(int, string, float, SmallRecord_get_Decoder(), datetimeUtc, nil(defaultOf()), int);
        const objectArg_78 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_78 = objectArg_78.fromString(arg_313, json_18);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_78, actual_78);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("tuple8 returns an error if invalid json")(() => {
        const json_19 = "[1, \"maxime\", 2.5, { \"fieldA\" : \"test\" }, \"2018-10-01T11:12:55.00Z\", null, 56, \"maxime\"]";
        const expected_79 = new FSharpResult$2(1, ["\nError at: `$.[7]`\nExpecting an int but instead got: \"maxime\"\n                        ".trim()]);
        let actual_79;
        const arg_317 = tuple8(int, string, float, SmallRecord_get_Decoder(), datetimeUtc, nil(defaultOf()), int, int);
        const objectArg_79 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_79 = objectArg_79.fromString(arg_317, json_19);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_79, actual_79);
    })])), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testList"]()("Object primitives")(ofArray([runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("field works")(() => {
        const json_20 = "{ \"name\": \"maxime\", \"age\": 25 }";
        const expected_80 = new FSharpResult$2(0, ["maxime"]);
        let actual_80;
        const arg_321 = field("name", string);
        const objectArg_80 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_80 = objectArg_80.fromString(arg_321, json_20);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_80, actual_80);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("field output an error explaining why the value is considered invalid")(() => {
        const json_21 = "{ \"name\": null, \"age\": 25 }";
        const expected_81 = new FSharpResult$2(1, ["\nError at: `$.name`\nExpecting an int but instead got: null\n                        ".trim()]);
        let actual_81;
        const arg_325 = field("name", int);
        const objectArg_81 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_81 = objectArg_81.fromString(arg_325, json_21);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_81, actual_81);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("field output an error when field is missing")(() => {
        const json_22 = "{ \"name\": \"maxime\", \"age\": 25 }";
        const expected_82 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an object with a field named `height` but instead got:\n{\n    \"name\": \"maxime\",\n    \"age\": 25\n}\n                        ".trim()]);
        let actual_82;
        const arg_329 = field("height", float);
        const objectArg_82 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_82 = objectArg_82.fromString(arg_329, json_22);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_82, actual_82);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("at works")(() => {
        const json_23 = "{ \"user\": { \"name\": \"maxime\", \"age\": 25 } }";
        const expected_83 = new FSharpResult$2(0, ["maxime"]);
        let actual_83;
        const arg_333 = at(ofArray(["user", "name"]), string);
        const objectArg_83 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_83 = objectArg_83.fromString(arg_333, json_23);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_83, actual_83);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("at output an error if the path failed")(() => {
        const json_24 = "{ \"user\": { \"name\": \"maxime\", \"age\": 25 } }";
        const expected_84 = new FSharpResult$2(1, ["\nError at: `$.user.firstname`\nExpecting an object with path `user.firstname` but instead got:\n{\n    \"user\": {\n        \"name\": \"maxime\",\n        \"age\": 25\n    }\n}\nNode `firstname` is unkown.\n                        ".trim()]);
        let actual_84;
        const arg_337 = at(ofArray(["user", "firstname"]), string);
        const objectArg_84 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_84 = objectArg_84.fromString(arg_337, json_24);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_84, actual_84);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("at output an error explaining why the value is considered invalid")(() => {
        const json_25 = "{ \"name\": null, \"age\": 25 }";
        const expected_85 = new FSharpResult$2(1, ["\nError at: `$.name`\nExpecting an int but instead got: null\n                        ".trim()]);
        let actual_85;
        const arg_341 = at(singleton("name"), int);
        const objectArg_85 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_85 = objectArg_85.fromString(arg_341, json_25);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_85, actual_85);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("index works")(() => {
        const json_26 = "[\"maxime\", \"alfonso\", \"steffen\"]";
        const expected_86 = new FSharpResult$2(0, ["alfonso"]);
        let actual_86;
        const arg_345 = index(1, string);
        const objectArg_86 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_86 = objectArg_86.fromString(arg_345, json_26);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_86, actual_86);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("index output an error if array is to small")(() => {
        const json_27 = "[\"maxime\", \"alfonso\", \"steffen\"]";
        const expected_87 = new FSharpResult$2(1, ["\nError at: `$.[5]`\nExpecting a longer array. Need index `5` but there are only `3` entries.\n[\n    \"maxime\",\n    \"alfonso\",\n    \"steffen\"\n]\n                        ".trim()]);
        let actual_87;
        const arg_349 = index(5, string);
        const objectArg_87 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_87 = objectArg_87.fromString(arg_349, json_27);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_87, actual_87);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("index output an error if value isn\'t an array")(() => {
        const json_28 = "1";
        const expected_88 = new FSharpResult$2(1, ["\nError at: `$.[5]`\nExpecting an array but instead got: 1\n                        ".trim()]);
        let actual_88;
        const arg_353 = index(5, string);
        const objectArg_88 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_88 = objectArg_88.fromString(arg_353, json_28);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_88, actual_88);
    })])), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testList"]()("Data structure")(ofArray([runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("list works")(() => {
        const expected_89 = new FSharpResult$2(0, [ofArray([1, 2, 3])]);
        let actual_89;
        const arg_357 = list_5(int);
        const objectArg_89 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_89 = objectArg_89.fromString(arg_357, "[1, 2, 3]");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_89, actual_89);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("nested lists work")(() => {
        let _arg_93;
        const arg_363 = list_5(list_5(string));
        let arg_364;
        const arg_362 = list_6(map((d) => list_6(map((value_7) => (new Json(0, [value_7])), d)), singleton(singleton("maxime2"))));
        const objectArg_90 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Encode"]();
        arg_364 = objectArg_90.toString(4, arg_362);
        const objectArg_91 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        _arg_93 = objectArg_91.fromString(arg_363, arg_364);
        if (_arg_93.tag === 1) {
            const er = _arg_93.fields[0];
            throw new Error(er);
        }
        else {
            const v = _arg_93.fields[0];
            runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](singleton(singleton("maxime2")), v);
        }
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an invalid list output an error")(() => {
        const expected_90 = new FSharpResult$2(1, ["Error at: `$`\nExpecting a list but instead got: 1"]);
        let actual_90;
        const arg_367 = list_5(int);
        const objectArg_92 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_90 = objectArg_92.fromString(arg_367, "1");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_90, actual_90);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("array works")(() => {
        const expected_91 = new FSharpResult$2(0, [[1, 2, 3]]);
        let actual_91;
        const arg_371 = array(int);
        const objectArg_93 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_91 = objectArg_93.fromString(arg_371, "[1, 2, 3]");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_91, actual_91);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an invalid array output an error")(() => {
        const expected_92 = new FSharpResult$2(1, ["Error at: `$`\nExpecting an array but instead got: 1"]);
        let actual_92;
        const arg_375 = array(int);
        const objectArg_94 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_92 = objectArg_94.fromString(arg_375, "1");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_92, actual_92);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("keys works")(() => {
        const expected_93 = new FSharpResult$2(0, [ofArray(["a", "b", "c"])]);
        let actual_93;
        const objectArg_95 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_93 = objectArg_95.fromString(keys_1, "{ \"a\": 1, \"b\": 2, \"c\": 3 }");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_93, actual_93);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("keys returns an error for invalid objects")(() => {
        const expected_94 = new FSharpResult$2(1, ["Error at: `$`\nExpecting an object but instead got: 1"]);
        let actual_94;
        const objectArg_96 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_94 = objectArg_96.fromString(keys_1, "1");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_94, actual_94);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("keyValuePairs works")(() => {
        const expected_95 = new FSharpResult$2(0, [ofArray([["a", 1], ["b", 2], ["c", 3]])]);
        let actual_95;
        const arg_387 = keyValuePairs(int);
        const objectArg_97 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_95 = objectArg_97.fromString(arg_387, "{ \"a\": 1, \"b\": 2, \"c\": 3 }");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_95, actual_95);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("dict works")(() => {
        const expected_96 = new FSharpResult$2(0, [ofList(ofArray([["a", 1], ["b", 2], ["c", 3]]), {
            Compare: comparePrimitives,
        })]);
        let actual_96;
        const arg_391 = dict(int);
        const objectArg_98 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_96 = objectArg_98.fromString(arg_391, "{ \"a\": 1, \"b\": 2, \"c\": 3 }");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_96, actual_96);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("dict with custom decoder works")(() => {
        const expected_97 = new FSharpResult$2(0, [ofList(ofArray([["a", Record2_Create(1, 1)], ["b", Record2_Create(2, 2)], ["c", Record2_Create(3, 3)]]), {
            Compare: comparePrimitives,
        })]);
        const decodePoint = map2(Record2_Create, field("a", float), field("b", float));
        let actual_97;
        const arg_395 = dict(decodePoint);
        const arg_396 = "\n{\n    \"a\":\n        {\n            \"a\": 1.0,\n            \"b\": 1.0\n        },\n    \"b\":\n        {\n            \"a\": 2.0,\n            \"b\": 2.0\n        },\n    \"c\":\n        {\n            \"a\": 3.0,\n            \"b\": 3.0\n        }\n}\n                        ";
        const objectArg_99 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_97 = objectArg_99.fromString(arg_395, arg_396);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_97, actual_97);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("an invalid dict output an error")(() => {
        const expected_98 = new FSharpResult$2(1, ["Error at: `$`\nExpecting an object but instead got: 1"]);
        let actual_98;
        const arg_399 = dict(int);
        const objectArg_100 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_98 = objectArg_100.fromString(arg_399, "1");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_98, actual_98);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("map\' works")(() => {
        const expected_99 = new FSharpResult$2(0, [ofList(ofArray([[1, "x"], [2, "y"], [3, "z"]]), {
            Compare: comparePrimitives,
        })]);
        let actual_99;
        const arg_403 = map$0027(int, string);
        const objectArg_101 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_99 = objectArg_101.fromString(arg_403, "[ [ 1, \"x\" ], [ 2, \"y\" ], [ 3, \"z\" ] ]");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_99, actual_99);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("map\' with custom key decoder works")(() => {
        const expected_100 = new FSharpResult$2(0, [ofList(ofArray([[[1, 6], "a"], [[2, 7], "b"], [[3, 8], "c"]]), {
            Compare: compareArrays,
        })]);
        const decodePoint_1 = map2((x_4, y_4) => [x_4, y_4], field("x", int), field("y", int));
        let actual_100;
        const arg_407 = map$0027(decodePoint_1, string);
        const arg_408 = "\n[\n    [\n        {\n            \"x\": 1,\n            \"y\": 6\n        },\n        \"a\"\n    ],\n    [\n        {\n            \"x\": 2,\n            \"y\": 7\n        },\n        \"b\"\n    ],\n    [\n        {\n            \"x\": 3,\n            \"y\": 8\n        },\n        \"c\"\n    ]\n]\n                        ";
        const objectArg_102 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_100 = objectArg_102.fromString(arg_407, arg_408);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_100, actual_100);
    })])), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testList"]()("Inconsistent structure")(ofArray([runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("oneOf works")(() => {
        const expected_101 = new FSharpResult$2(0, [ofArray([1, 2, 0, 4])]);
        const badInt = oneOf(ofArray([int, nil(0)]));
        let actual_101;
        const arg_411 = list_5(badInt);
        const objectArg_103 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_101 = objectArg_103.fromString(arg_411, "[1,2,null,4]");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_101, actual_101);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("oneOf works in combination with object builders")(() => {
        const json_29 = "{ \"Bar\": { \"name\": \"maxime\", \"age\": 25 } }";
        const expected_102 = new FSharpResult$2(0, [new FSharpChoice$2(1, [new SmallRecord("maxime")])]);
        const decoder1 = object((get$) => {
            let objectArg_104;
            return new SmallRecord((objectArg_104 = get$.Required, objectArg_104.Field("name", string)));
        });
        const decoder2 = oneOf(ofArray([map_1((Item) => (new FSharpChoice$2(0, [Item])), field("Foo", decoder1)), map_1((Item_1) => (new FSharpChoice$2(1, [Item_1])), field("Bar", decoder1))]));
        let actual_102;
        const objectArg_105 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_102 = objectArg_105.fromString(decoder2, json_29);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_102, actual_102);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("oneOf works with optional")(() => {
        const decoder_7 = oneOf(ofArray([map_1((Item_2) => (new Price(0, [Item_2])), field("Normal", float)), map_1((Item_3) => (new Price(1, [Item_3])), field("Reduced", option_3(float))), map_1((_arg_108) => (new Price(2, [])), field("Zero", bool))]));
        let arg_424;
        const objectArg_106 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        arg_424 = objectArg_106.fromString(decoder_7, "{\"Normal\": 4.5}");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](new FSharpResult$2(0, [new Price(0, [4.5])]), arg_424);
        let arg_428;
        const objectArg_107 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        arg_428 = objectArg_107.fromString(decoder_7, "{\"Reduced\": 4.5}");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](new FSharpResult$2(0, [new Price(1, [4.5])]), arg_428);
        let arg_432;
        const objectArg_108 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        arg_432 = objectArg_108.fromString(decoder_7, "{\"Reduced\": null}");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](new FSharpResult$2(0, [new Price(1, [void 0])]), arg_432);
        let arg_436;
        const objectArg_109 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        arg_436 = objectArg_109.fromString(decoder_7, "{\"Zero\": true}");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](new FSharpResult$2(0, [new Price(2, [])]), arg_436);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("oneOf output errors if all case fails")(() => {
        const expected_103 = new FSharpResult$2(1, ["\nThe following errors were found:\n\nError at: `$.[0]`\nExpecting a string but instead got: 1\n\nError at: `$.[0]`\nExpecting an object but instead got:\n1\n                        ".trim()]);
        const badInt_1 = oneOf(ofArray([string, field("test", string)]));
        let actual_103;
        const arg_437 = list_5(badInt_1);
        const objectArg_110 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_103 = objectArg_110.fromString(arg_437, "[1,2,null,4]");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_103, actual_103);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("optional works")(() => {
        const json_30 = "{ \"name\": \"maxime\", \"age\": 25, \"something_undefined\": null }";
        const expectedValid = new FSharpResult$2(0, ["maxime"]);
        let actualValid;
        const arg_441 = optional("name", string);
        const objectArg_111 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actualValid = objectArg_111.fromString(arg_441, json_30);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expectedValid, actualValid);
        let matchValue_1;
        const arg_445 = optional("name", int);
        const objectArg_112 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        matchValue_1 = objectArg_112.fromString(arg_445, json_30);
        if (matchValue_1.tag === 0) {
            throw new Error("Expected type error for `name` field");
        }
        const expectedMissingField = new FSharpResult$2(0, [void 0]);
        let actualMissingField;
        const arg_447 = optional("height", int);
        const objectArg_113 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actualMissingField = objectArg_113.fromString(arg_447, json_30);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expectedMissingField, actualMissingField);
        const expectedUndefinedField = new FSharpResult$2(0, [void 0]);
        let actualUndefinedField;
        const arg_451 = optional("something_undefined", string);
        const objectArg_114 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actualUndefinedField = objectArg_114.fromString(arg_451, json_30);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expectedUndefinedField, actualUndefinedField);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("optional returns Error value if decoder fails")(() => {
        const json_31 = "{ \"name\": 12, \"age\": 25 }";
        const expected_104 = new FSharpResult$2(1, ["\nError at: `$.name`\nExpecting a string but instead got: 12\n                        ".trim()]);
        let actual_104;
        const arg_455 = optional("name", string);
        const objectArg_115 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_104 = objectArg_115.fromString(arg_455, json_31);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_104, actual_104);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("optionalAt works")(() => {
        const json_32 = "{ \"data\" : { \"name\": \"maxime\", \"age\": 25, \"something_undefined\": null } }";
        const expectedValid_1 = new FSharpResult$2(0, ["maxime"]);
        let actualValid_1;
        const arg_459 = optionalAt(ofArray(["data", "name"]), string);
        const objectArg_116 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actualValid_1 = objectArg_116.fromString(arg_459, json_32);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expectedValid_1, actualValid_1);
        let matchValue_2;
        const arg_463 = optionalAt(ofArray(["data", "name"]), int);
        const objectArg_117 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        matchValue_2 = objectArg_117.fromString(arg_463, json_32);
        if (matchValue_2.tag === 0) {
            throw new Error("Expected type error for `name` field");
        }
        const expectedMissingField_1 = new FSharpResult$2(0, [void 0]);
        let actualMissingField_1;
        const arg_465 = optionalAt(ofArray(["data", "height"]), int);
        const objectArg_118 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actualMissingField_1 = objectArg_118.fromString(arg_465, json_32);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expectedMissingField_1, actualMissingField_1);
        const expectedUndefinedField_1 = new FSharpResult$2(0, [void 0]);
        let actualUndefinedField_1;
        const arg_469 = optionalAt(ofArray(["data", "something_undefined"]), string);
        const objectArg_119 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actualUndefinedField_1 = objectArg_119.fromString(arg_469, json_32);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expectedUndefinedField_1, actualUndefinedField_1);
        const expectedUndefinedField_2 = new FSharpResult$2(0, [void 0]);
        let actualUndefinedField_2;
        const arg_473 = optionalAt(ofArray(["data", "something_undefined", "name"]), string);
        const objectArg_120 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actualUndefinedField_2 = objectArg_120.fromString(arg_473, json_32);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expectedUndefinedField_2, actualUndefinedField_2);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("combining field and option decoders works")(() => {
        const json_33 = "{ \"name\": \"maxime\", \"age\": 25, \"something_undefined\": null }";
        const expectedValid_2 = new FSharpResult$2(0, ["maxime"]);
        let actualValid_2;
        const arg_477 = field("name", option_3(string));
        const objectArg_121 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actualValid_2 = objectArg_121.fromString(arg_477, json_33);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expectedValid_2, actualValid_2);
        let matchValue_3;
        const arg_481 = field("name", option_3(int));
        const objectArg_122 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        matchValue_3 = objectArg_122.fromString(arg_481, json_33);
        if (matchValue_3.tag === 0) {
            throw new Error("Expected type error for `name` field #1");
        }
        else {
            const msg = matchValue_3.fields[0];
            const expected_105 = "\nError at: `$.name`\nExpecting an int but instead got: \"maxime\"\n                        ".trim();
            runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_105, msg);
        }
        let matchValue_4;
        const arg_485 = field("this_field_do_not_exist", option_3(int));
        const objectArg_123 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        matchValue_4 = objectArg_123.fromString(arg_485, json_33);
        if (matchValue_4.tag === 0) {
            throw new Error("Expected type error for `name` field #2");
        }
        else {
            const msg_1 = matchValue_4.fields[0];
            const expected_106 = "\nError at: `$`\nExpecting an object with a field named `this_field_do_not_exist` but instead got:\n{\n    \"name\": \"maxime\",\n    \"age\": 25,\n    \"something_undefined\": null\n}\n                        ".trim();
            runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_106, msg_1);
        }
        let matchValue_5;
        const arg_489 = field("something_undefined", option_3(int));
        const objectArg_124 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        matchValue_5 = objectArg_124.fromString(arg_489, json_33);
        if (matchValue_5.tag === 0) {
            const result = matchValue_5.fields[0];
            runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](void 0, result);
        }
        else {
            throw new Error("`Decode.field \"something_undefined\" (Decode.option Decode.int)` test should pass");
        }
        const expectedValid2 = new FSharpResult$2(0, ["maxime"]);
        let actualValid2;
        const arg_493 = option_3(field("name", string));
        const objectArg_125 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actualValid2 = objectArg_125.fromString(arg_493, json_33);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expectedValid2, actualValid2);
        let matchValue_6;
        const arg_497 = option_3(field("name", int));
        const objectArg_126 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        matchValue_6 = objectArg_126.fromString(arg_497, json_33);
        if (matchValue_6.tag === 0) {
            throw new Error("Expected type error for `name` field #3");
        }
        else {
            const msg_2 = matchValue_6.fields[0];
            const expected_107 = "\nError at: `$.name`\nExpecting an int but instead got: \"maxime\"\n                        ".trim();
            runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_107, msg_2);
        }
        let matchValue_7;
        const arg_501 = option_3(field("this_field_do_not_exist", int));
        const objectArg_127 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        matchValue_7 = objectArg_127.fromString(arg_501, json_33);
        if (matchValue_7.tag === 0) {
            throw new Error("Expected type error for `name` field #4");
        }
        else {
            const msg_3 = matchValue_7.fields[0];
            const expected_108 = "\nError at: `$`\nExpecting an object with a field named `this_field_do_not_exist` but instead got:\n{\n    \"name\": \"maxime\",\n    \"age\": 25,\n    \"something_undefined\": null\n}\n                        ".trim();
            runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_108, msg_3);
        }
        let matchValue_8;
        const arg_505 = option_3(field("something_undefined", int));
        const objectArg_128 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        matchValue_8 = objectArg_128.fromString(arg_505, json_33);
        if (matchValue_8.tag === 0) {
            throw new Error("Expected type error for `name` field");
        }
        else {
            const msg_4 = matchValue_8.fields[0];
            const expected_109 = "\nError at: `$.something_undefined`\nExpecting an int but instead got: null\n                        ".trim();
            runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_109, msg_4);
        }
        let matchValue_9;
        const arg_509 = field("height", option_3(int));
        const objectArg_129 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        matchValue_9 = objectArg_129.fromString(arg_509, json_33);
        if (matchValue_9.tag === 0) {
            throw new Error("Expected type error for `height` field");
        }
        else {
            const msg_5 = matchValue_9.fields[0];
            const expected_110 = "\nError at: `$`\nExpecting an object with a field named `height` but instead got:\n{\n    \"name\": \"maxime\",\n    \"age\": 25,\n    \"something_undefined\": null\n}\n                        ".trim();
            runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_110, msg_5);
        }
        const expectedUndefinedField_3 = new FSharpResult$2(0, [void 0]);
        let actualUndefinedField_3;
        const arg_513 = field("something_undefined", option_3(string));
        const objectArg_130 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actualUndefinedField_3 = objectArg_130.fromString(arg_513, json_33);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expectedUndefinedField_3, actualUndefinedField_3);
    })])), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testList"]()("Fancy decoding")(ofArray([runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("null works (test on an int)")(() => {
        const expected_111 = new FSharpResult$2(0, [20]);
        let actual_105;
        const arg_517 = nil(20);
        const objectArg_131 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_105 = objectArg_131.fromString(arg_517, "null");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_111, actual_105);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("null works (test on a boolean)")(() => {
        const expected_112 = new FSharpResult$2(0, [false]);
        let actual_106;
        const arg_521 = nil(false);
        const objectArg_132 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_106 = objectArg_132.fromString(arg_521, "null");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_112, actual_106);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("succeed works")(() => {
        const expected_113 = new FSharpResult$2(0, [7]);
        let actual_107;
        const arg_525 = succeed_1(7);
        const objectArg_133 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_107 = objectArg_133.fromString(arg_525, "true");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_113, actual_107);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("succeed output an error if the JSON is invalid")(() => {
        const expected_114 = new FSharpResult$2(1, ["Given an invalid JSON: Unexpected token \'m\', \"maxime\" is not valid JSON"]);
        let actual_108;
        const arg_529 = succeed_1(7);
        const objectArg_134 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_108 = objectArg_134.fromString(arg_529, "maxime");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_114, actual_108);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("fail works")(() => {
        const msg_6 = "Failing because it\'s fun";
        const expected_115 = new FSharpResult$2(1, ["Error at: `$`\nThe following `failure` occurred with the decoder: " + msg_6]);
        let actual_109;
        const arg_533 = fail(msg_6);
        const objectArg_135 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_109 = objectArg_135.fromString(arg_533, "true");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_115, actual_109);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("andMap works for any arity")(() => {
        const json_34 = "{\"a\": 1,\"b\": 2,\"c\": 3,\"d\": 4,\"e\": 5,\"f\": 6,\"g\": 7,\"h\": 8,\"i\": 9,\"j\": 10,\"k\": 11}";
        const decodeRecord10 = andMap()(field("k", int))(andMap()(field("j", int))(andMap()(field("i", int))(andMap()(field("h", int))(andMap()(field("g", int))(andMap()(field("f", int))(andMap()(field("e", int))(andMap()(field("d", int))(andMap()(field("c", int))(andMap()(field("b", int))(andMap()(field("a", int))(succeed_1((a_4) => ((b_4) => ((c) => ((d_6) => ((e) => ((f) => ((g) => ((h) => ((i) => ((j) => ((k) => Record10_Create(a_4, b_4, c, d_6, e, f, g, h, i, j, k)))))))))))))))))))))));
        let actual_110;
        const objectArg_136 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_110 = objectArg_136.fromString(decodeRecord10, json_34);
        const expected_116 = new FSharpResult$2(0, [new Record10(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11)]);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_116, actual_110);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("andThen works")(() => {
        const expected_117 = new FSharpResult$2(0, [1]);
        const infoHelp = (version) => {
            switch (version) {
                case 3:
                    return succeed_1(1);
                case 4:
                    return succeed_1(1);
                default:
                    return fail(("Trying to decode info, but version " + int32ToString(version)) + "is not supported");
            }
        };
        const info = andThen_1(infoHelp, field("version", int));
        let actual_111;
        const objectArg_137 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_111 = objectArg_137.fromString(info, "{ \"version\": 3, \"data\": 2 }");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_117, actual_111);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("andThen generate an error if an error occuered")(() => {
        const expected_118 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an object with a field named `version` but instead got:\n{\n    \"info\": 3,\n    \"data\": 2\n}\n                        ".trim()]);
        const infoHelp_1 = (version_1) => {
            switch (version_1) {
                case 3:
                    return succeed_1(1);
                case 4:
                    return succeed_1(1);
                default:
                    return fail(("Trying to decode info, but version " + int32ToString(version_1)) + "is not supported");
            }
        };
        const info_1 = andThen_1(infoHelp_1, field("version", int));
        let actual_112;
        const objectArg_138 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_112 = objectArg_138.fromString(info_1, "{ \"info\": 3, \"data\": 2 }");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_118, actual_112);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("all works")(() => {
        const expected_119 = new FSharpResult$2(0, [ofArray([1, 2, 3])]);
        const decodeAll = all(ofArray([succeed_1(1), succeed_1(2), succeed_1(3)]));
        let actual_113;
        const objectArg_139 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_113 = objectArg_139.fromString(decodeAll, "{}");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_119, actual_113);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("combining Decode.all and Decode.keys works")(() => {
        const expected_120 = new FSharpResult$2(0, [ofArray([1, 2, 3])]);
        const decoder_11 = andThen_1((keys) => all(map((key) => field(key, int), List_except(["special_property"], keys, {
            Equals: (x_5, y_5) => (x_5 === y_5),
            GetHashCode: stringHash,
        }))), keys_1);
        let actual_114;
        const objectArg_140 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_114 = objectArg_140.fromString(decoder_11, "{ \"a\": 1, \"b\": 2, \"c\": 3 }");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_120, actual_114);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("all succeeds on empty lists")(() => {
        const expected_121 = new FSharpResult$2(0, [empty()]);
        const decodeNone = all(empty());
        let actual_115;
        const objectArg_141 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_115 = objectArg_141.fromString(decodeNone, "{}");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_121, actual_115);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("all fails when one decoder fails")(() => {
        const expected_122 = new FSharpResult$2(1, ["Error at: `$`\nExpecting an int but instead got: {}"]);
        const decodeAll_1 = all(ofArray([succeed_1(1), int, succeed_1(3)]));
        let actual_116;
        const objectArg_142 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_116 = objectArg_142.fromString(decodeAll_1, "{}");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_122, actual_116);
    })])), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testList"]()("Mapping")(ofArray([runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("map works")(() => {
        const expected_123 = new FSharpResult$2(0, [6]);
        const stringLength = map_1((str) => str.length, string);
        let actual_117;
        const objectArg_143 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_117 = objectArg_143.fromString(stringLength, "\"maxime\"");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_123, actual_117);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("map2 works")(() => {
        const expected_124 = new FSharpResult$2(0, [new Record2(1, 2)]);
        const decodePoint_2 = map2(Record2_Create, field("a", float), field("b", float));
        let actual_118;
        const objectArg_144 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_118 = objectArg_144.fromString(decodePoint_2, jsonRecord);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_124, actual_118);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("map3 works")(() => {
        const expected_125 = new FSharpResult$2(0, [new Record3(1, 2, 3)]);
        const decodePoint_3 = map3(Record3_Create, field("a", float), field("b", float), field("c", float));
        let actual_119;
        const objectArg_145 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_119 = objectArg_145.fromString(decodePoint_3, jsonRecord);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_125, actual_119);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("map4 works")(() => {
        const expected_126 = new FSharpResult$2(0, [new Record4(1, 2, 3, 4)]);
        const decodePoint_4 = map4(Record4_Create, field("a", float), field("b", float), field("c", float), field("d", float));
        let actual_120;
        const objectArg_146 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_120 = objectArg_146.fromString(decodePoint_4, jsonRecord);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_126, actual_120);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("map5 works")(() => {
        const expected_127 = new FSharpResult$2(0, [new Record5(1, 2, 3, 4, 5)]);
        const decodePoint_5 = map5(Record5_Create, field("a", float), field("b", float), field("c", float), field("d", float), field("e", float));
        let actual_121;
        const objectArg_147 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_121 = objectArg_147.fromString(decodePoint_5, jsonRecord);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_127, actual_121);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("map6 works")(() => {
        const expected_128 = new FSharpResult$2(0, [new Record6(1, 2, 3, 4, 5, 6)]);
        const decodePoint_6 = map6(Record6_Create, field("a", float), field("b", float), field("c", float), field("d", float), field("e", float), field("f", float));
        let actual_122;
        const objectArg_148 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_122 = objectArg_148.fromString(decodePoint_6, jsonRecord);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_128, actual_122);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("map7 works")(() => {
        const expected_129 = new FSharpResult$2(0, [new Record7(1, 2, 3, 4, 5, 6, 7)]);
        const decodePoint_7 = map7(Record7_Create, field("a", float), field("b", float), field("c", float), field("d", float), field("e", float), field("f", float), field("g", float));
        let actual_123;
        const objectArg_149 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_123 = objectArg_149.fromString(decodePoint_7, jsonRecord);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_129, actual_123);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("map8 works")(() => {
        const expected_130 = new FSharpResult$2(0, [new Record8(1, 2, 3, 4, 5, 6, 7, 8)]);
        const decodePoint_8 = map8(Record8_Create, field("a", float), field("b", float), field("c", float), field("d", float), field("e", float), field("f", float), field("g", float), field("h", float));
        let actual_124;
        const objectArg_150 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_124 = objectArg_150.fromString(decodePoint_8, jsonRecord);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_130, actual_124);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("map2 generate an error if invalid")(() => {
        const expected_131 = new FSharpResult$2(1, ["Error at: `$.a`\nExpecting a float but instead got: \"invalid_a_field\""]);
        const decodePoint_9 = map2(Record2_Create, field("a", float), field("b", float));
        let actual_125;
        const objectArg_151 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_125 = objectArg_151.fromString(decodePoint_9, jsonRecordInvalid);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_131, actual_125);
    })])), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testList"]()("object builder")(ofArray([runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("get.Required.Field works")(() => {
        const json_35 = "{ \"name\": \"maxime\", \"age\": 25 }";
        const expected_132 = new FSharpResult$2(0, [new SmallRecord("maxime")]);
        const decoder_12 = object((get$_1) => {
            let objectArg_152;
            return new SmallRecord((objectArg_152 = get$_1.Required, objectArg_152.Field("name", string)));
        });
        let actual_126;
        const objectArg_153 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_126 = objectArg_153.fromString(decoder_12, json_35);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_132, actual_126);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("get.Required.Field returns Error if field is missing")(() => {
        const json_36 = "{ \"age\": 25 }";
        const expected_133 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an object with a field named `name` but instead got:\n{\n    \"age\": 25\n}\n                        ".trim()]);
        const decoder_13 = object((get$_2) => {
            let objectArg_154;
            return new SmallRecord((objectArg_154 = get$_2.Required, objectArg_154.Field("name", string)));
        });
        let actual_127;
        const objectArg_155 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_127 = objectArg_155.fromString(decoder_13, json_36);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_133, actual_127);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("get.Required.Field returns Error if type is incorrect")(() => {
        const json_37 = "{ \"name\": 12, \"age\": 25 }";
        const expected_134 = new FSharpResult$2(1, ["\nError at: `$.name`\nExpecting a string but instead got: 12\n                        ".trim()]);
        const decoder_14 = object((get$_3) => {
            let objectArg_156;
            return new SmallRecord((objectArg_156 = get$_3.Required, objectArg_156.Field("name", string)));
        });
        let actual_128;
        const objectArg_157 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_128 = objectArg_157.fromString(decoder_14, json_37);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_134, actual_128);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("get.Optional.Field works")(() => {
        const json_38 = "{ \"name\": \"maxime\", \"age\": 25 }";
        const expected_135 = new FSharpResult$2(0, [new SmallRecord2("maxime")]);
        const decoder_15 = object((get$_4) => {
            let objectArg_158;
            return new SmallRecord2((objectArg_158 = get$_4.Optional, objectArg_158.Field("name", string)));
        });
        let actual_129;
        const objectArg_159 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_129 = objectArg_159.fromString(decoder_15, json_38);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_135, actual_129);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("get.Optional.Field returns None value if field is missing")(() => {
        const json_39 = "{ \"age\": 25 }";
        const expected_136 = new FSharpResult$2(0, [new SmallRecord2(void 0)]);
        const decoder_16 = object((get$_5) => {
            let objectArg_160;
            return new SmallRecord2((objectArg_160 = get$_5.Optional, objectArg_160.Field("name", string)));
        });
        let actual_130;
        const objectArg_161 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_130 = objectArg_161.fromString(decoder_16, json_39);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_136, actual_130);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("get.Optional.Field returns None if field is null")(() => {
        const json_40 = "{ \"name\": null, \"age\": 25 }";
        const expected_137 = new FSharpResult$2(0, [new SmallRecord2(void 0)]);
        const decoder_17 = object((get$_6) => {
            let objectArg_162;
            return new SmallRecord2((objectArg_162 = get$_6.Optional, objectArg_162.Field("name", string)));
        });
        let actual_131;
        const objectArg_163 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_131 = objectArg_163.fromString(decoder_17, json_40);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_137, actual_131);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("get.Optional.Field returns Error value if decoder fails")(() => {
        const json_41 = "{ \"name\": 12, \"age\": 25 }";
        const expected_138 = new FSharpResult$2(1, ["\nError at: `$.name`\nExpecting a string but instead got: 12\n                        ".trim()]);
        const decoder_18 = object((get$_7) => {
            let objectArg_164;
            return new SmallRecord2((objectArg_164 = get$_7.Optional, objectArg_164.Field("name", string)));
        });
        let actual_132;
        const objectArg_165 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_132 = objectArg_165.fromString(decoder_18, json_41);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_138, actual_132);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("nested get.Optional.Field > get.Required.Field returns None if field is null")(() => {
        const json_42 = "{ \"user\": null, \"field2\": 25 }";
        const expected_139 = new FSharpResult$2(0, [new Model(void 0, 25)]);
        const userDecoder = object((get$_8) => {
            let objectArg_166, objectArg_167, objectArg_168;
            return new User((objectArg_166 = get$_8.Required, objectArg_166.Field("id", int)), (objectArg_167 = get$_8.Required, objectArg_167.Field("name", string)), (objectArg_168 = get$_8.Required, objectArg_168.Field("email", string)), 0);
        });
        const decoder_19 = object((get$_9) => {
            let objectArg_169, objectArg_170;
            return new Model((objectArg_169 = get$_9.Optional, objectArg_169.Field("user", userDecoder)), (objectArg_170 = get$_9.Required, objectArg_170.Field("field2", int)));
        });
        let actual_133;
        const objectArg_171 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_133 = objectArg_171.fromString(decoder_19, json_42);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_139, actual_133);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("get.Optional.Field returns Error if type is incorrect")(() => {
        const json_43 = "{ \"name\": 12, \"age\": 25 }";
        const expected_140 = new FSharpResult$2(1, ["\nError at: `$.name`\nExpecting a string but instead got: 12\n                        ".trim()]);
        const decoder_20 = object((get$_10) => {
            let objectArg_172;
            return new SmallRecord2((objectArg_172 = get$_10.Optional, objectArg_172.Field("name", string)));
        });
        let actual_134;
        const objectArg_173 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_134 = objectArg_173.fromString(decoder_20, json_43);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_140, actual_134);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("get.Required.At works")(() => {
        const json_44 = "{ \"user\": { \"name\": \"maxime\", \"age\": 25 } }";
        const expected_141 = new FSharpResult$2(0, [new SmallRecord("maxime")]);
        const decoder_21 = object((get$_11) => {
            let objectArg_174;
            return new SmallRecord((objectArg_174 = get$_11.Required, objectArg_174.At(ofArray(["user", "name"]), string)));
        });
        let actual_135;
        const objectArg_175 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_135 = objectArg_175.fromString(decoder_21, json_44);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_141, actual_135);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("get.Required.At returns Error if non-object in path")(() => {
        const json_45 = "{ \"user\": \"maxime\" }";
        const expected_142 = new FSharpResult$2(1, ["\nError at: `$.user`\nExpecting an object but instead got:\n\"maxime\"\n                        ".trim()]);
        const decoder_22 = object((get$_12) => {
            let objectArg_176;
            return new SmallRecord((objectArg_176 = get$_12.Required, objectArg_176.At(ofArray(["user", "name"]), string)));
        });
        let actual_136;
        const objectArg_177 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_136 = objectArg_177.fromString(decoder_22, json_45);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_142, actual_136);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("get.Required.At returns Error if field missing")(() => {
        const json_46 = "{ \"user\": { \"name\": \"maxime\", \"age\": 25 } }";
        const expected_143 = new FSharpResult$2(1, ["\nError at: `$.user.firstname`\nExpecting an object with path `user.firstname` but instead got:\n{\n    \"user\": {\n        \"name\": \"maxime\",\n        \"age\": 25\n    }\n}\nNode `firstname` is unkown.\n                        ".trim()]);
        const decoder_23 = object((get$_13) => {
            let objectArg_178;
            return new SmallRecord((objectArg_178 = get$_13.Required, objectArg_178.At(ofArray(["user", "firstname"]), string)));
        });
        let actual_137;
        const objectArg_179 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_137 = objectArg_179.fromString(decoder_23, json_46);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_143, actual_137);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("get.Required.At returns Error if type is incorrect")(() => {
        const json_47 = "{ \"user\": { \"name\": 12, \"age\": 25 } }";
        const expected_144 = new FSharpResult$2(1, ["\nError at: `$.user.name`\nExpecting a string but instead got: 12\n                        ".trim()]);
        const decoder_24 = object((get$_14) => {
            let objectArg_180;
            return new SmallRecord((objectArg_180 = get$_14.Required, objectArg_180.At(ofArray(["user", "name"]), string)));
        });
        let actual_138;
        const objectArg_181 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_138 = objectArg_181.fromString(decoder_24, json_47);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_144, actual_138);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("get.Optional.At works")(() => {
        const json_48 = "{ \"user\": { \"name\": \"maxime\", \"age\": 25 } }";
        const expected_145 = new FSharpResult$2(0, [new SmallRecord2("maxime")]);
        const decoder_25 = object((get$_15) => {
            let objectArg_182;
            return new SmallRecord2((objectArg_182 = get$_15.Optional, objectArg_182.At(ofArray(["user", "name"]), string)));
        });
        let actual_139;
        const objectArg_183 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_139 = objectArg_183.fromString(decoder_25, json_48);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_145, actual_139);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("get.Optional.At returns \'type error\' if non-object in path")(() => {
        const json_49 = "{ \"user\": \"maxime\" }";
        const expected_146 = new FSharpResult$2(1, ["\nError at: `$.user`\nExpecting an object but instead got:\n\"maxime\"\n                        ".trim()]);
        const decoder_26 = object((get$_16) => {
            let objectArg_184;
            return new SmallRecord2((objectArg_184 = get$_16.Optional, objectArg_184.At(ofArray(["user", "name"]), string)));
        });
        let actual_140;
        const objectArg_185 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_140 = objectArg_185.fromString(decoder_26, json_49);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_146, actual_140);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("get.Optional.At returns None if field missing")(() => {
        const json_50 = "{ \"user\": { \"name\": \"maxime\", \"age\": 25 } }";
        const expected_147 = new FSharpResult$2(0, [new SmallRecord2(void 0)]);
        const decoder_27 = object((get$_17) => {
            let objectArg_186;
            return new SmallRecord2((objectArg_186 = get$_17.Optional, objectArg_186.At(ofArray(["user", "firstname"]), string)));
        });
        let actual_141;
        const objectArg_187 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_141 = objectArg_187.fromString(decoder_27, json_50);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_147, actual_141);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("get.Optional.At returns Error if type is incorrect")(() => {
        const json_51 = "{ \"user\": { \"name\": 12, \"age\": 25 } }";
        const expected_148 = new FSharpResult$2(1, ["\nError at: `$.user.name`\nExpecting a string but instead got: 12\n                        ".trim()]);
        const decoder_28 = object((get$_18) => {
            let objectArg_188;
            return new SmallRecord2((objectArg_188 = get$_18.Optional, objectArg_188.At(ofArray(["user", "name"]), string)));
        });
        let actual_142;
        const objectArg_189 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_142 = objectArg_189.fromString(decoder_28, json_51);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_148, actual_142);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("complex object builder works")(() => {
        const expected_149 = new FSharpResult$2(0, [User_Create(67, "", "user@mail.com", 0)]);
        const userDecoder_1 = object((get$_19) => {
            let objectArg_190, objectArg_191, objectArg_192;
            return new User((objectArg_190 = get$_19.Required, objectArg_190.Field("id", int)), defaultArg((objectArg_191 = get$_19.Optional, objectArg_191.Field("name", string)), ""), (objectArg_192 = get$_19.Required, objectArg_192.Field("email", string)), 0);
        });
        let actual_143;
        const objectArg_193 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_143 = objectArg_193.fromString(userDecoder_1, "{ \"id\": 67, \"email\": \"user@mail.com\" }");
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_149, actual_143);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("get.Field.Raw works")(() => {
        const json_52 = "{\n    \"enabled\": true,\n\t\"shape\": \"circle\",\n    \"radius\": 20\n}";
        const shapeDecoder = andThen_1((_arg_154) => {
            switch (_arg_154) {
                case "circle":
                    return Shape_get_DecoderCircle();
                case "rectangle":
                    return Shape_get_DecoderRectangle();
                default: {
                    const shape = _arg_154;
                    return fail(toText(printf("Unknown shape type %s"))(shape));
                }
            }
        }, field("shape", string));
        const decoder_30 = object((get$_20) => {
            let objectArg_194;
            return new MyObj((objectArg_194 = get$_20.Required, objectArg_194.Field("enabled", bool)), get$_20.Required.Raw(shapeDecoder));
        });
        let actual_144;
        const objectArg_195 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_144 = objectArg_195.fromString(decoder_30, json_52);
        const expected_150 = new FSharpResult$2(0, [new MyObj(true, new Shape(0, [20]))]);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_150, actual_144);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("get.Field.Raw returns Error if a decoder fail")(() => {
        const json_53 = "{\n    \"enabled\": true,\n\t\"shape\": \"custom_shape\",\n    \"radius\": 20\n}";
        const shapeDecoder_1 = andThen_1((_arg_156) => {
            switch (_arg_156) {
                case "circle":
                    return Shape_get_DecoderCircle();
                case "rectangle":
                    return Shape_get_DecoderRectangle();
                default: {
                    const shape_1 = _arg_156;
                    return fail(toText(printf("Unknown shape type %s"))(shape_1));
                }
            }
        }, field("shape", string));
        const decoder_32 = object((get$_21) => {
            let objectArg_196;
            return new MyObj((objectArg_196 = get$_21.Required, objectArg_196.Field("enabled", bool)), get$_21.Required.Raw(shapeDecoder_1));
        });
        let actual_145;
        const objectArg_197 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_145 = objectArg_197.fromString(decoder_32, json_53);
        const expected_151 = new FSharpResult$2(1, ["Error at: `$`\nThe following `failure` occurred with the decoder: Unknown shape type custom_shape"]);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_151, actual_145);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("get.Field.Raw returns Error if a field is missing in the \'raw decoder\'")(() => {
        const json_54 = "{\n    \"enabled\": true,\n\t\"shape\": \"circle\"\n}";
        const shapeDecoder_2 = andThen_1((_arg_158) => {
            switch (_arg_158) {
                case "circle":
                    return Shape_get_DecoderCircle();
                case "rectangle":
                    return Shape_get_DecoderRectangle();
                default: {
                    const shape_2 = _arg_158;
                    return fail(toText(printf("Unknown shape type %s"))(shape_2));
                }
            }
        }, field("shape", string));
        const decoder_34 = object((get$_22) => {
            let objectArg_198;
            return new MyObj((objectArg_198 = get$_22.Required, objectArg_198.Field("enabled", bool)), get$_22.Required.Raw(shapeDecoder_2));
        });
        let actual_146;
        const objectArg_199 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_146 = objectArg_199.fromString(decoder_34, json_54);
        const expected_152 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an object with a field named `radius` but instead got:\n{\n    \"enabled\": true,\n    \"shape\": \"circle\"\n}                   ".trim()]);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_152, actual_146);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("get.Optional.Raw works")(() => {
        const json_55 = "{\n    \"enabled\": true,\n\t\"shape\": \"circle\",\n    \"radius\": 20\n}";
        const shapeDecoder_3 = andThen_1((_arg_160) => {
            switch (_arg_160) {
                case "circle":
                    return Shape_get_DecoderCircle();
                case "rectangle":
                    return Shape_get_DecoderRectangle();
                default: {
                    const shape_3 = _arg_160;
                    return fail(toText(printf("Unknown shape type %s"))(shape_3));
                }
            }
        }, field("shape", string));
        const decoder_36 = object((get$_23) => {
            let objectArg_200;
            return new MyObj2((objectArg_200 = get$_23.Required, objectArg_200.Field("enabled", bool)), get$_23.Optional.Raw(shapeDecoder_3));
        });
        let actual_147;
        const objectArg_201 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_147 = objectArg_201.fromString(decoder_36, json_55);
        const expected_153 = new FSharpResult$2(0, [new MyObj2(true, new Shape(0, [20]))]);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_153, actual_147);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("get.Optional.Raw returns None if a field is missing")(() => {
        const json_56 = "{\n    \"enabled\": true,\n\t\"shape\": \"circle\"\n}";
        const shapeDecoder_4 = andThen_1((_arg_162) => {
            switch (_arg_162) {
                case "circle":
                    return Shape_get_DecoderCircle();
                case "rectangle":
                    return Shape_get_DecoderRectangle();
                default: {
                    const shape_4 = _arg_162;
                    return fail(toText(printf("Unknown shape type %s"))(shape_4));
                }
            }
        }, field("shape", string));
        const decoder_38 = object((get$_24) => {
            let objectArg_202;
            return new MyObj2((objectArg_202 = get$_24.Required, objectArg_202.Field("enabled", bool)), get$_24.Optional.Raw(shapeDecoder_4));
        });
        let actual_148;
        const objectArg_203 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_148 = objectArg_203.fromString(decoder_38, json_56);
        const expected_154 = new FSharpResult$2(0, [new MyObj2(true, void 0)]);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_154, actual_148);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("get.Optional.Raw returns an Error if a decoder fail")(() => {
        const json_57 = "{\n    \"enabled\": true,\n\t\"shape\": \"invalid_shape\"\n}";
        const shapeDecoder_5 = andThen_1((_arg_164) => {
            switch (_arg_164) {
                case "circle":
                    return Shape_get_DecoderCircle();
                case "rectangle":
                    return Shape_get_DecoderRectangle();
                default: {
                    const shape_5 = _arg_164;
                    return fail(toText(printf("Unknown shape type %s"))(shape_5));
                }
            }
        }, field("shape", string));
        const decoder_40 = object((get$_25) => {
            let objectArg_204;
            return new MyObj2((objectArg_204 = get$_25.Required, objectArg_204.Field("enabled", bool)), get$_25.Optional.Raw(shapeDecoder_5));
        });
        let actual_149;
        const objectArg_205 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_149 = objectArg_205.fromString(decoder_40, json_57);
        const expected_155 = new FSharpResult$2(1, ["Error at: `$`\nThe following `failure` occurred with the decoder: Unknown shape type invalid_shape"]);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_155, actual_149);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("get.Optional.Raw returns an Error if the type is invalid")(() => {
        const json_58 = "{\n    \"enabled\": true,\n\t\"shape\": \"circle\",\n    \"radius\": \"maxime\"\n}";
        const shapeDecoder_6 = andThen_1((_arg_166) => {
            switch (_arg_166) {
                case "circle":
                    return Shape_get_DecoderCircle();
                case "rectangle":
                    return Shape_get_DecoderRectangle();
                default: {
                    const shape_6 = _arg_166;
                    return fail(toText(printf("Unknown shape type %s"))(shape_6));
                }
            }
        }, field("shape", string));
        const decoder_42 = object((get$_26) => {
            let objectArg_206;
            return new MyObj2((objectArg_206 = get$_26.Required, objectArg_206.Field("enabled", bool)), get$_26.Optional.Raw(shapeDecoder_6));
        });
        let actual_150;
        const objectArg_207 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_150 = objectArg_207.fromString(decoder_42, json_58);
        const expected_156 = new FSharpResult$2(1, ["Error at: `$.radius`\nExpecting an int but instead got: \"maxime\""]);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_156, actual_150);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("get.Optional.Raw returns None if a decoder fails with null")(() => {
        const json_59 = "{\n    \"enabled\": true,\n\t\"shape\": null\n}";
        const shapeDecoder_7 = andThen_1((_arg_168) => {
            switch (_arg_168) {
                case "circle":
                    return Shape_get_DecoderCircle();
                case "rectangle":
                    return Shape_get_DecoderRectangle();
                default: {
                    const shape_7 = _arg_168;
                    return fail(toText(printf("Unknown shape type %s"))(shape_7));
                }
            }
        }, field("shape", string));
        const decoder_44 = object((get$_27) => {
            let objectArg_208;
            return new MyObj2((objectArg_208 = get$_27.Required, objectArg_208.Field("enabled", bool)), get$_27.Optional.Raw(shapeDecoder_7));
        });
        let actual_151;
        const objectArg_209 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_151 = objectArg_209.fromString(decoder_44, json_59);
        const expected_157 = new FSharpResult$2(0, [new MyObj2(true, void 0)]);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_157, actual_151);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("Object builders returns all the Errors")(() => {
        const json_60 = "{ \"age\": 25, \"fieldC\": \"not_a_number\", \"fieldD\": { \"sub_field\": \"not_a_boolean\" } }";
        const expected_158 = new FSharpResult$2(1, ["\nThe following errors were found:\n\nError at: `$`\nExpecting an object with a field named `missing_field_1` but instead got:\n{\n    \"age\": 25,\n    \"fieldC\": \"not_a_number\",\n    \"fieldD\": {\n        \"sub_field\": \"not_a_boolean\"\n    }\n}\n\nError at: `$.missing_field_2.sub_field`\nExpecting an object with path `missing_field_2.sub_field` but instead got:\n{\n    \"age\": 25,\n    \"fieldC\": \"not_a_number\",\n    \"fieldD\": {\n        \"sub_field\": \"not_a_boolean\"\n    }\n}\nNode `sub_field` is unkown.\n\nError at: `$.fieldC`\nExpecting an int but instead got: \"not_a_number\"\n\nError at: `$.fieldD.sub_field`\nExpecting a boolean but instead got: \"not_a_boolean\"\n                        ".trim()]);
        const decoder_45 = object((get$_28) => {
            let objectArg_210, objectArg_211, objectArg_212, objectArg_213;
            return new MediumRecord((objectArg_210 = get$_28.Required, objectArg_210.Field("missing_field_1", string)), (objectArg_211 = get$_28.Required, objectArg_211.At(ofArray(["missing_field_2", "sub_field"]), string)), defaultArg((objectArg_212 = get$_28.Optional, objectArg_212.Field("fieldC", int)), -1), defaultArg((objectArg_213 = get$_28.Optional, objectArg_213.At(ofArray(["fieldD", "sub_field"]), bool)), false));
        });
        let actual_152;
        const objectArg_214 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_152 = objectArg_214.fromString(decoder_45, json_60);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_158, actual_152);
    }), runner["Thoth.Json.Tests.Testing.TestRunner`2.get_testCase"]()("Test")(() => {
        const json_61 = "\n                    {\n                        \"person\": {\n                            \"name\": \"maxime\"\n                        },\n                        \"post\": null\n                    }\n                    ";
        const personDecoder = object((get$_29) => {
            let objectArg_215;
            return new Person((objectArg_215 = get$_29.Required, objectArg_215.Field("name", string)));
        });
        const postDecoder = object((get$_30) => {
            let title;
            const objectArg_216 = get$_30.Required;
            title = objectArg_216.Field("title", string);
            const arg_793 = head(title.split(""));
            toConsole(printf("Title: %A"))(arg_793);
            return new Post(title);
        });
        const dataDecoder = object((get$_31) => {
            let objectArg_217, objectArg_218;
            return new Data((objectArg_217 = get$_31.Required, objectArg_217.Field("person", personDecoder)), (objectArg_218 = get$_31.Optional, objectArg_218.Field("post", postDecoder)));
        });
        let actual_153;
        const objectArg_219 = runner["Thoth.Json.Tests.Testing.TestRunner`2.get_Decode"]();
        actual_153 = objectArg_219.fromString(dataDecoder, json_61);
        const expected_159 = new FSharpResult$2(0, [new Data(new Person("maxime"), void 0)]);
        runner["Thoth.Json.Tests.Testing.TestRunner`2.equal"](expected_159, actual_153);
    })]))]));
}

