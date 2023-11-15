import { Union, Record } from "./fable_modules/fable-library.4.5.0/Types.js";
import { union_type, string_type, record_type, float64_type, int32_type } from "./fable_modules/fable-library.4.5.0/Reflection.js";
import { Test_testCase, Test_testList } from "./fable_modules/Fable.Mocha.2.17.0/Mocha.fs.js";
import { FSharpChoice$2, FSharpResult$2 } from "./fable_modules/fable-library.4.5.0/Choice.js";
import { map8, map7, map6, map5, map4, map3, all, andThen as andThen_1, andMap, fail, succeed as succeed_1, optionalAt, optional, option as option_3, map, object, oneOf, map$0027, map2, dict, keyValuePairs, keys as keys_1, array, list as list_3, index, at, field, tuple8, tuple7, nil, tuple6, tuple5, tuple4, tuple3, tuple2, timespan, datetimeOffset, datetimeLocal, datetimeUtc, bigint, sbyte, byte, uint64, uint32, int64, uint16, int16, int, bool, char, string, unit, float, fromValue } from "../packages/Thoth.Json.Core/Decode.fs.js";
import { fromString, helpers } from "../packages/Thoth.Json.JavaScript/Decode.fs.js";
import { stringHash, int32ToString, compareArrays, comparePrimitives, defaultOf, assertEqual } from "./fable_modules/fable-library.4.5.0/Util.js";
import { Data, Post, Person, MediumRecord, MyObj2, Shape, MyObj, Shape_get_DecoderRectangle, Shape_get_DecoderCircle, User_Create, User, Model, SmallRecord2, Record8_Create, Record8, Record7_Create, Record7, Record6_Create, Record6, Record5_Create, Record5, Record4_Create, Record4, Record3_Create, Record3, Record2, Record10, Record10_Create, Price, Record2_Create, SmallRecord_get_Decoder, SmallRecord, CustomException } from "./Types.fs.js";
import { empty, map as map_1, singleton, ofArray } from "./fable_modules/fable-library.4.5.0/List.js";
import { fromInt32 } from "./fable_modules/fable-library.4.5.0/BigInt.js";
import { toString, toUniversalTime, create } from "./fable_modules/fable-library.4.5.0/Date.js";
import { toConsole, printf, toText } from "./fable_modules/fable-library.4.5.0/String.js";
import { create as create_1 } from "./fable_modules/fable-library.4.5.0/DateOffset.js";
import { create as create_2, fromHours } from "./fable_modules/fable-library.4.5.0/TimeSpan.js";
import { uint16 as uint16_1, int16 as int16_1, uint32 as uint32_1, int as int_1, byte as byte_1, sbyte as sbyte_1, succeed, andThen } from "../packages/Thoth.Json.Core/./Decode.fs.js";
import { ofList } from "./fable_modules/fable-library.4.5.0/Map.js";
import { List_except } from "./fable_modules/fable-library.4.5.0/Seq2.js";
import { defaultArg } from "./fable_modules/fable-library.4.5.0/Option.js";
import { head } from "./fable_modules/fable-library.4.5.0/Seq.js";

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

export const tests = Test_testList("Thoth.Json.Decode", ofArray([Test_testList("Errors", ofArray([Test_testCase("circular structure are supported when reporting error", () => {
    const a = {};
    const b = {};
    a.child = b;
    b.child = a;
    const expected = new FSharpResult$2(1, ["Error at: `$`\nExpecting a float but decoder failed. Couldn\'t report given value due to circular structure. "]);
    const actual = fromValue(helpers, "$", float, b);
    assertEqual(actual, expected);
}), Test_testCase("invalid json", () => {
    const expected_2 = new FSharpResult$2(1, ["Given an invalid JSON: Unexpected token \'m\', \"maxime\" is not valid JSON"]);
    const actual_2 = fromString(float, "maxime");
    assertEqual(actual_2, expected_2);
}), Test_testCase("invalid json #3 - Special case for Thoth.Json.Net", () => {
    const expected_4 = new FSharpResult$2(1, ["Given an invalid JSON: Expected double-quoted property name in JSON at position 172"]);
    const incorrectJson = "\n                {\n                \"Ab\": [\n                    \"RecordC\",\n                    {\n                    \"C1\": \"\",\n                    \"C2\": \"\",\n                ";
    const actual_4 = fromString(float, incorrectJson);
    assertEqual(actual_4, expected_4);
}), Test_testCase("user exceptions are not captured by the decoders", () => {
    const expected_6 = true;
    const decoder = {
        Decode(_arg_4, _arg_5, _arg_6) {
            throw new CustomException();
        },
    };
    let actual_6;
    try {
        fromString(decoder, "\"maxime\"");
        actual_6 = false;
    }
    catch (matchValue) {
        if (matchValue instanceof CustomException) {
            actual_6 = true;
        }
        else {
            throw matchValue;
        }
    }
    assertEqual(actual_6, expected_6);
})])), Test_testList("Primitives", ofArray([Test_testCase("unit works", () => {
    const expected_8 = new FSharpResult$2(0, [void 0]);
    const actual_8 = fromString(unit, "null");
    assertEqual(actual_8, expected_8);
}), Test_testCase("a string works", () => {
    const expected_10 = new FSharpResult$2(0, ["maxime"]);
    const actual_10 = fromString(string, "\"maxime\"");
    assertEqual(actual_10, expected_10);
}), Test_testCase("a string with new line works", () => {
    const expected_12 = new FSharpResult$2(0, ["a\nb"]);
    const actual_12 = fromString(string, "\"a\\nb\"");
    assertEqual(actual_12, expected_12);
}), Test_testCase("a string with new line character works", () => {
    const expected_14 = new FSharpResult$2(0, ["a\\nb"]);
    const actual_14 = fromString(string, "\"a\\\\nb\"");
    assertEqual(actual_14, expected_14);
}), Test_testCase("a string with tab works", () => {
    const expected_16 = new FSharpResult$2(0, ["a\tb"]);
    const actual_16 = fromString(string, "\"a\\tb\"");
    assertEqual(actual_16, expected_16);
}), Test_testCase("a string with tab character works", () => {
    const expected_18 = new FSharpResult$2(0, ["a\\tb"]);
    const actual_18 = fromString(string, "\"a\\\\tb\"");
    assertEqual(actual_18, expected_18);
}), Test_testCase("a char works", () => {
    const expected_20 = new FSharpResult$2(0, ["a"]);
    const actual_20 = fromString(char, "\"a\"");
    assertEqual(actual_20, expected_20);
}), Test_testCase("a char reports an error if there are more than 1 characters in the string", () => {
    const expected_22 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a single character string but instead got: \"ab\"\n                        ".trim()]);
    const actual_22 = fromString(char, "\"ab\"");
    assertEqual(actual_22, expected_22);
}), Test_testCase("a float works", () => {
    const expected_24 = new FSharpResult$2(0, [1.2]);
    const actual_24 = fromString(float, "1.2");
    assertEqual(actual_24, expected_24);
}), Test_testCase("a float from int works", () => {
    const expected_26 = new FSharpResult$2(0, [1]);
    const actual_26 = fromString(float, "1");
    assertEqual(actual_26, expected_26);
}), Test_testCase("a bool works", () => {
    const expected_28 = new FSharpResult$2(0, [true]);
    const actual_28 = fromString(bool, "true");
    assertEqual(actual_28, expected_28);
}), Test_testCase("an invalid bool output an error", () => {
    const expected_30 = new FSharpResult$2(1, ["Error at: `$`\nExpecting a boolean but instead got: 2"]);
    const actual_30 = fromString(bool, "2");
    assertEqual(actual_30, expected_30);
}), Test_testCase("an int works", () => {
    const expected_32 = new FSharpResult$2(0, [25]);
    const actual_32 = fromString(int, "25");
    assertEqual(actual_32, expected_32);
}), Test_testCase("an invalid int [invalid range: too big] output an error", () => {
    const expected_34 = new FSharpResult$2(1, ["Error at: `$`\nExpecting an int but instead got: 2147483648\nReason: Value was either too large or too small for an int"]);
    const actual_34 = fromString(int, "2147483648");
    assertEqual(actual_34, expected_34);
}), Test_testCase("an invalid int [invalid range: too small] output an error", () => {
    const expected_36 = new FSharpResult$2(1, ["Error at: `$`\nExpecting an int but instead got: -2147483649\nReason: Value was either too large or too small for an int"]);
    const actual_36 = fromString(int, "-2147483649");
    assertEqual(actual_36, expected_36);
}), Test_testCase("an int16 works from number", () => {
    const expected_38 = new FSharpResult$2(0, [(25 + 0x8000 & 0xFFFF) - 0x8000]);
    const actual_38 = fromString(int16, "25");
    assertEqual(actual_38, expected_38);
}), Test_testCase("an int16 works from string", () => {
    const expected_40 = new FSharpResult$2(0, [(-25 + 0x8000 & 0xFFFF) - 0x8000]);
    const actual_40 = fromString(int16, "\"-25\"");
    assertEqual(actual_40, expected_40);
}), Test_testCase("an int16 output an error if value is too big", () => {
    const expected_42 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an int16 but instead got: 32768\nReason: Value was either too large or too small for an int16\n                        ".trim()]);
    const actual_42 = fromString(int16, "32768");
    assertEqual(actual_42, expected_42);
}), Test_testCase("an int16 output an error if value is too small", () => {
    const expected_44 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an int16 but instead got: -32769\nReason: Value was either too large or too small for an int16\n                        ".trim()]);
    const actual_44 = fromString(int16, "-32769");
    assertEqual(actual_44, expected_44);
}), Test_testCase("an int16 output an error if incorrect string", () => {
    const expected_46 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an int16 but instead got: \"maxime\"\n                        ".trim()]);
    const actual_46 = fromString(int16, "\"maxime\"");
    assertEqual(actual_46, expected_46);
}), Test_testCase("an uint16 works from number", () => {
    const expected_48 = new FSharpResult$2(0, [25 & 0xFFFF]);
    const actual_48 = fromString(uint16, "25");
    assertEqual(actual_48, expected_48);
}), Test_testCase("an uint16 works from string", () => {
    const expected_50 = new FSharpResult$2(0, [25 & 0xFFFF]);
    const actual_50 = fromString(uint16, "\"25\"");
    assertEqual(actual_50, expected_50);
}), Test_testCase("an uint16 output an error if value is too big", () => {
    const expected_52 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an uint16 but instead got: 65536\nReason: Value was either too large or too small for an uint16\n                        ".trim()]);
    const actual_52 = fromString(uint16, "65536");
    assertEqual(actual_52, expected_52);
}), Test_testCase("an uint16 output an error if value is too small", () => {
    const expected_54 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an uint16 but instead got: -1\nReason: Value was either too large or too small for an uint16\n                        ".trim()]);
    const actual_54 = fromString(uint16, "-1");
    assertEqual(actual_54, expected_54);
}), Test_testCase("an uint16 output an error if incorrect string", () => {
    const expected_56 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an uint16 but instead got: \"maxime\"\n                        ".trim()]);
    const actual_56 = fromString(uint16, "\"maxime\"");
    assertEqual(actual_56, expected_56);
}), Test_testCase("an int64 works from number", () => {
    const expected_58 = new FSharpResult$2(0, [1000n]);
    const actual_58 = fromString(int64, "1000");
    assertEqual(actual_58, expected_58);
}), Test_testCase("an int64 works from string", () => {
    const expected_60 = new FSharpResult$2(0, [99n]);
    const actual_60 = fromString(int64, "\"99\"");
    assertEqual(actual_60, expected_60);
}), Test_testCase("an int64 works output an error if incorrect string", () => {
    const expected_62 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an int64 but instead got: \"maxime\"\n                        ".trim()]);
    const actual_62 = fromString(int64, "\"maxime\"");
    assertEqual(actual_62, expected_62);
}), Test_testCase("an uint32 works from number", () => {
    const expected_64 = new FSharpResult$2(0, [1000]);
    const actual_64 = fromString(uint32, "1000");
    assertEqual(actual_64, expected_64);
}), Test_testCase("an uint32 works from string", () => {
    const expected_66 = new FSharpResult$2(0, [1000]);
    const actual_66 = fromString(uint32, "\"1000\"");
    assertEqual(actual_66, expected_66);
}), Test_testCase("an uint32 output an error if incorrect string", () => {
    const expected_68 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an uint32 but instead got: \"maxime\"\n                        ".trim()]);
    const actual_68 = fromString(uint32, "\"maxime\"");
    assertEqual(actual_68, expected_68);
}), Test_testCase("an uint64 works from number", () => {
    const expected_70 = new FSharpResult$2(0, [1000n]);
    const actual_70 = fromString(uint64, "1000");
    assertEqual(actual_70, expected_70);
}), Test_testCase("an uint64 works from string", () => {
    const expected_72 = new FSharpResult$2(0, [1000n]);
    const actual_72 = fromString(uint64, "\"1000\"");
    assertEqual(actual_72, expected_72);
}), Test_testCase("an uint64 output an error if incorrect string", () => {
    const expected_74 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an uint64 but instead got: \"maxime\"\n                        ".trim()]);
    const actual_74 = fromString(uint64, "\"maxime\"");
    assertEqual(actual_74, expected_74);
}), Test_testCase("a byte works from number", () => {
    const expected_76 = new FSharpResult$2(0, [25]);
    const actual_76 = fromString(byte, "25");
    assertEqual(actual_76, expected_76);
}), Test_testCase("a byte works from string", () => {
    const expected_78 = new FSharpResult$2(0, [25]);
    const actual_78 = fromString(byte, "\"25\"");
    assertEqual(actual_78, expected_78);
}), Test_testCase("a byte output an error if value is too big", () => {
    const expected_80 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a byte but instead got: 256\nReason: Value was either too large or too small for a byte\n                        ".trim()]);
    const actual_80 = fromString(byte, "256");
    assertEqual(actual_80, expected_80);
}), Test_testCase("a byte output an error if value is too small", () => {
    const expected_82 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a byte but instead got: -1\nReason: Value was either too large or too small for a byte\n                        ".trim()]);
    const actual_82 = fromString(byte, "-1");
    assertEqual(actual_82, expected_82);
}), Test_testCase("a byte output an error if incorrect string", () => {
    const expected_84 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a byte but instead got: \"maxime\"\n                        ".trim()]);
    const actual_84 = fromString(byte, "\"maxime\"");
    assertEqual(actual_84, expected_84);
}), Test_testCase("a sbyte works from number", () => {
    const expected_86 = new FSharpResult$2(0, [25]);
    const actual_86 = fromString(sbyte, "25");
    assertEqual(actual_86, expected_86);
}), Test_testCase("a sbyte works from string", () => {
    const expected_88 = new FSharpResult$2(0, [-25]);
    const actual_88 = fromString(sbyte, "\"-25\"");
    assertEqual(actual_88, expected_88);
}), Test_testCase("a sbyte output an error if value is too big", () => {
    const expected_90 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a sbyte but instead got: 128\nReason: Value was either too large or too small for a sbyte\n                        ".trim()]);
    const actual_90 = fromString(sbyte, "128");
    assertEqual(actual_90, expected_90);
}), Test_testCase("a sbyte output an error if value is too small", () => {
    const expected_92 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a sbyte but instead got: -129\nReason: Value was either too large or too small for a sbyte\n                        ".trim()]);
    const actual_92 = fromString(sbyte, "-129");
    assertEqual(actual_92, expected_92);
}), Test_testCase("a sbyte output an error if incorrect string", () => {
    const expected_94 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a sbyte but instead got: \"maxime\"\n                        ".trim()]);
    const actual_94 = fromString(sbyte, "\"maxime\"");
    assertEqual(actual_94, expected_94);
}), Test_testCase("an bigint works from number", () => {
    const expected_96 = new FSharpResult$2(0, [fromInt32(12)]);
    const actual_96 = fromString(bigint, "12");
    assertEqual(actual_96, expected_96);
}), Test_testCase("an bigint works from string", () => {
    const expected_98 = new FSharpResult$2(0, [fromInt32(12)]);
    const actual_98 = fromString(bigint, "\"12\"");
    assertEqual(actual_98, expected_98);
}), Test_testCase("an bigint output an error if invalid string", () => {
    const expected_100 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a bigint but instead got: \"maxime\"\n                        ".trim()]);
    const actual_100 = fromString(bigint, "\"maxime\"");
    assertEqual(actual_100, expected_100);
}), Test_testCase("a string representing a DateTime should be accepted as a string", () => {
    const expected_102 = "2018-10-01T11:12:55.00Z";
    const actual_102 = fromString(string, "\"2018-10-01T11:12:55.00Z\"");
    assertEqual(actual_102, new FSharpResult$2(0, [expected_102]));
}), Test_testCase("a datetime works", () => {
    const expected_104 = create(2018, 10, 1, 11, 12, 55, 0, 1);
    const actual_104 = fromString(datetimeUtc, "\"2018-10-01T11:12:55.00Z\"");
    assertEqual(actual_104, new FSharpResult$2(0, [expected_104]));
}), Test_testCase("a non-UTC datetime works", () => {
    const expected_106 = create(2018, 10, 1, 11, 12, 55);
    const actual_106 = fromString(datetimeLocal, "\"2018-10-01T11:12:55\"");
    assertEqual(actual_106, new FSharpResult$2(0, [expected_106]));
}), Test_testCase("a datetime output an error if invalid string", () => {
    const expected_108 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a datetime but instead got: \"invalid_string\"\n                        ".trim()]);
    const actual_108 = fromString(datetimeUtc, "\"invalid_string\"");
    assertEqual(actual_108, expected_108);
}), Test_testCase("a datetime works with TimeZone", () => {
    const localDate = create(2018, 10, 1, 11, 12, 55, 0, 2);
    const expected_110 = new FSharpResult$2(0, [toUniversalTime(localDate)]);
    let json;
    const arg = toString(localDate, "O");
    json = toText(printf("\"%s\""))(arg);
    const actual_110 = fromString(datetimeUtc, json);
    assertEqual(actual_110, expected_110);
}), Test_testCase("a datetimeOffset works", () => {
    const expected_112 = new FSharpResult$2(0, [create_1(2018, 7, 2, 12, 23, 45, 0, fromHours(2))]);
    const json_1 = "\"2018-07-02T12:23:45+02:00\"";
    const actual_112 = fromString(datetimeOffset, json_1);
    assertEqual(actual_112, expected_112);
}), Test_testCase("a datetimeOffset returns Error if invalid format", () => {
    const expected_114 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a datetimeoffset but instead got: \"NOT A DATETIMEOFFSET\"\n                        ".trim()]);
    const json_2 = "\"NOT A DATETIMEOFFSET\"";
    const actual_114 = fromString(datetimeOffset, json_2);
    assertEqual(actual_114, expected_114);
}), Test_testCase("a timespan works", () => {
    const expected_116 = new FSharpResult$2(0, [create_2(23, 45, 0)]);
    const json_3 = "\"23:45:00\"";
    const actual_116 = fromString(timespan, json_3);
    assertEqual(actual_116, expected_116);
}), Test_testCase("a timespan returns Error if invalid format", () => {
    const expected_118 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a timespan but instead got: \"NOT A TimeSpan\"\n                        ".trim()]);
    const json_4 = "\"NOT A TimeSpan\"";
    const actual_118 = fromString(timespan, json_4);
    assertEqual(actual_118, expected_118);
}), Test_testCase("an enum<sbyte> works", () => {
    const expected_120 = new FSharpResult$2(0, [99]);
    const actual_120 = fromString(andThen(succeed, sbyte_1), "99");
    assertEqual(actual_120, expected_120);
}), Test_testCase("an enum<byte> works", () => {
    const expected_122 = new FSharpResult$2(0, [99]);
    const actual_122 = fromString(andThen(succeed, byte_1), "99");
    assertEqual(actual_122, expected_122);
}), Test_testCase("an enum<int> works", () => {
    const expected_124 = new FSharpResult$2(0, [1]);
    const actual_124 = fromString(andThen(succeed, int_1), "1");
    assertEqual(actual_124, expected_124);
}), Test_testCase("an enum<uint32> works", () => {
    const expected_126 = new FSharpResult$2(0, [99]);
    const actual_126 = fromString(andThen(succeed, uint32_1), "99");
    assertEqual(actual_126, expected_126);
}), Test_testCase("an enum<int16> works", () => {
    const expected_128 = new FSharpResult$2(0, [99]);
    const actual_128 = fromString(andThen(succeed, int16_1), "99");
    assertEqual(actual_128, expected_128);
}), Test_testCase("an enum<uint16> works", () => {
    const expected_130 = new FSharpResult$2(0, [99]);
    const actual_130 = fromString(andThen(succeed, uint16_1), "99");
    assertEqual(actual_130, expected_130);
})])), Test_testList("Tuples", ofArray([Test_testCase("tuple2 works", () => {
    const json_5 = "[1, \"maxime\"]";
    const expected_132 = new FSharpResult$2(0, [[1, "maxime"]]);
    const actual_132 = fromString(tuple2(int, string), json_5);
    assertEqual(actual_132, expected_132);
}), Test_testCase("tuple3 works", () => {
    const json_6 = "[1, \"maxime\", 2.5]";
    const expected_134 = new FSharpResult$2(0, [[1, "maxime", 2.5]]);
    const actual_134 = fromString(tuple3(int, string, float), json_6);
    assertEqual(actual_134, expected_134);
}), Test_testCase("tuple4 works", () => {
    const json_7 = "[1, \"maxime\", 2.5, { \"fieldA\" : \"test\" }]";
    const expected_136 = new FSharpResult$2(0, [[1, "maxime", 2.5, new SmallRecord("test")]]);
    const actual_136 = fromString(tuple4(int, string, float, SmallRecord_get_Decoder()), json_7);
    assertEqual(actual_136, expected_136);
}), Test_testCase("tuple5 works", () => {
    const json_8 = "[1, \"maxime\", 2.5, { \"fieldA\" : \"test\" }, false]";
    const expected_138 = new FSharpResult$2(0, [[1, "maxime", 2.5, new SmallRecord("test"), false]]);
    const actual_138 = fromString(tuple5(int, string, float, SmallRecord_get_Decoder(), bool), json_8);
    assertEqual(actual_138, expected_138);
}), Test_testCase("tuple6 works", () => {
    const json_9 = "[1, \"maxime\", 2.5, { \"fieldA\" : \"test\" }, false, null]";
    const expected_140 = new FSharpResult$2(0, [[1, "maxime", 2.5, new SmallRecord("test"), false, defaultOf()]]);
    const actual_140 = fromString(tuple6(int, string, float, SmallRecord_get_Decoder(), bool, nil(defaultOf())), json_9);
    assertEqual(actual_140, expected_140);
}), Test_testCase("tuple7 works", () => {
    const json_10 = "[1, \"maxime\", 2.5, { \"fieldA\" : \"test\" }, false, null, 56]";
    const expected_142 = new FSharpResult$2(0, [[1, "maxime", 2.5, new SmallRecord("test"), false, defaultOf(), 56]]);
    const actual_142 = fromString(tuple7(int, string, float, SmallRecord_get_Decoder(), bool, nil(defaultOf()), int), json_10);
    assertEqual(actual_142, expected_142);
}), Test_testCase("tuple8 works", () => {
    const json_11 = "[1, \"maxime\", 2.5, { \"fieldA\" : \"test\" }, false, null, true, 98]";
    const expected_144 = new FSharpResult$2(0, [[1, "maxime", 2.5, new SmallRecord("test"), false, defaultOf(), true, 98]]);
    const actual_144 = fromString(tuple8(int, string, float, SmallRecord_get_Decoder(), bool, nil(defaultOf()), bool, int), json_11);
    assertEqual(actual_144, expected_144);
}), Test_testCase("tuple2 returns an error if invalid json", () => {
    const json_12 = "[1, false, \"unused value\"]";
    const expected_146 = new FSharpResult$2(1, ["\nError at: `$.[1]`\nExpecting a string but instead got: false\n                        ".trim()]);
    const actual_146 = fromString(tuple2(int, string), json_12);
    assertEqual(actual_146, expected_146);
}), Test_testCase("tuple3 returns an error if invalid json", () => {
    const json_13 = "[1, \"maxime\", false]";
    const expected_148 = new FSharpResult$2(1, ["\nError at: `$.[2]`\nExpecting a float but instead got: false\n                        ".trim()]);
    const actual_148 = fromString(tuple3(int, string, float), json_13);
    assertEqual(actual_148, expected_148);
}), Test_testCase("tuple4 returns an error if invalid json (missing index)", () => {
    const json_14 = "[1, \"maxime\", 2.5]";
    const expected_150 = new FSharpResult$2(1, ["\nError at: `$.[3]`\nExpecting a longer array. Need index `3` but there are only `3` entries.\n[\n    1,\n    \"maxime\",\n    2.5\n]\n                        ".trim()]);
    const actual_150 = fromString(tuple4(int, string, float, SmallRecord_get_Decoder()), json_14);
    assertEqual(actual_150, expected_150);
}), Test_testCase("tuple4 returns an error if invalid json (error in the nested object)", () => {
    const json_15 = "[1, \"maxime\", 2.5, { \"fieldA\" : false }]";
    const expected_152 = new FSharpResult$2(1, ["\nError at: `$.[3].fieldA`\nExpecting a string but instead got: false\n                        ".trim()]);
    const actual_152 = fromString(tuple4(int, string, float, SmallRecord_get_Decoder()), json_15);
    assertEqual(actual_152, expected_152);
}), Test_testCase("tuple5 returns an error if invalid json", () => {
    const json_16 = "[1, \"maxime\", 2.5, { \"fieldA\" : \"test\" }, false]";
    const expected_154 = new FSharpResult$2(1, ["\nError at: `$.[4]`\nExpecting a datetime but instead got: false\n                        ".trim()]);
    const actual_154 = fromString(tuple5(int, string, float, SmallRecord_get_Decoder(), datetimeUtc), json_16);
    assertEqual(actual_154, expected_154);
}), Test_testCase("tuple6 returns an error if invalid json", () => {
    const json_17 = "[1, \"maxime\", 2.5, { \"fieldA\" : \"test\" }, \"2018-10-01T11:12:55.00Z\", false]";
    const expected_156 = new FSharpResult$2(1, ["\nError at: `$.[5]`\nExpecting null but instead got: false\n                        ".trim()]);
    const actual_156 = fromString(tuple6(int, string, float, SmallRecord_get_Decoder(), datetimeUtc, nil(defaultOf())), json_17);
    assertEqual(actual_156, expected_156);
}), Test_testCase("tuple7 returns an error if invalid json", () => {
    const json_18 = "[1, \"maxime\", 2.5, { \"fieldA\" : \"test\" }, \"2018-10-01T11:12:55.00Z\", null, false]";
    const expected_158 = new FSharpResult$2(1, ["\nError at: `$.[6]`\nExpecting an int but instead got: false\n                        ".trim()]);
    const actual_158 = fromString(tuple7(int, string, float, SmallRecord_get_Decoder(), datetimeUtc, nil(defaultOf()), int), json_18);
    assertEqual(actual_158, expected_158);
}), Test_testCase("tuple8 returns an error if invalid json", () => {
    const json_19 = "[1, \"maxime\", 2.5, { \"fieldA\" : \"test\" }, \"2018-10-01T11:12:55.00Z\", null, 56, \"maxime\"]";
    const expected_160 = new FSharpResult$2(1, ["\nError at: `$.[7]`\nExpecting an int but instead got: \"maxime\"\n                        ".trim()]);
    const actual_160 = fromString(tuple8(int, string, float, SmallRecord_get_Decoder(), datetimeUtc, nil(defaultOf()), int, int), json_19);
    assertEqual(actual_160, expected_160);
})])), Test_testList("Object primitives", ofArray([Test_testCase("field works", () => {
    const json_20 = "{ \"name\": \"maxime\", \"age\": 25 }";
    const expected_162 = new FSharpResult$2(0, ["maxime"]);
    const actual_162 = fromString(field("name", string), json_20);
    assertEqual(actual_162, expected_162);
}), Test_testCase("field output an error explaining why the value is considered invalid", () => {
    const json_21 = "{ \"name\": null, \"age\": 25 }";
    const expected_164 = new FSharpResult$2(1, ["\nError at: `$.name`\nExpecting an int but instead got: null\n                        ".trim()]);
    const actual_164 = fromString(field("name", int), json_21);
    assertEqual(actual_164, expected_164);
}), Test_testCase("field output an error when field is missing", () => {
    const json_22 = "{ \"name\": \"maxime\", \"age\": 25 }";
    const expected_166 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an object with a field named `height` but instead got:\n{\n    \"name\": \"maxime\",\n    \"age\": 25\n}\n                        ".trim()]);
    const actual_166 = fromString(field("height", float), json_22);
    assertEqual(actual_166, expected_166);
}), Test_testCase("at works", () => {
    const json_23 = "{ \"user\": { \"name\": \"maxime\", \"age\": 25 } }";
    const expected_168 = new FSharpResult$2(0, ["maxime"]);
    const actual_168 = fromString(at(ofArray(["user", "name"]), string), json_23);
    assertEqual(actual_168, expected_168);
}), Test_testCase("at output an error if the path failed", () => {
    const json_24 = "{ \"user\": { \"name\": \"maxime\", \"age\": 25 } }";
    const expected_170 = new FSharpResult$2(1, ["\nError at: `$.user.firstname`\nExpecting an object with path `user.firstname` but instead got:\n{\n    \"user\": {\n        \"name\": \"maxime\",\n        \"age\": 25\n    }\n}\nNode `firstname` is unkown.\n                        ".trim()]);
    const actual_170 = fromString(at(ofArray(["user", "firstname"]), string), json_24);
    assertEqual(actual_170, expected_170);
}), Test_testCase("at output an error explaining why the value is considered invalid", () => {
    const json_25 = "{ \"name\": null, \"age\": 25 }";
    const expected_172 = new FSharpResult$2(1, ["\nError at: `$.name`\nExpecting an int but instead got: null\n                        ".trim()]);
    const actual_172 = fromString(at(singleton("name"), int), json_25);
    assertEqual(actual_172, expected_172);
}), Test_testCase("index works", () => {
    const json_26 = "[\"maxime\", \"alfonso\", \"steffen\"]";
    const expected_174 = new FSharpResult$2(0, ["alfonso"]);
    const actual_174 = fromString(index(1, string), json_26);
    assertEqual(actual_174, expected_174);
}), Test_testCase("index output an error if array is to small", () => {
    const json_27 = "[\"maxime\", \"alfonso\", \"steffen\"]";
    const expected_176 = new FSharpResult$2(1, ["\nError at: `$.[5]`\nExpecting a longer array. Need index `5` but there are only `3` entries.\n[\n    \"maxime\",\n    \"alfonso\",\n    \"steffen\"\n]\n                        ".trim()]);
    const actual_176 = fromString(index(5, string), json_27);
    assertEqual(actual_176, expected_176);
}), Test_testCase("index output an error if value isn\'t an array", () => {
    const json_28 = "1";
    const expected_178 = new FSharpResult$2(1, ["\nError at: `$.[5]`\nExpecting an array but instead got: 1\n                        ".trim()]);
    const actual_178 = fromString(index(5, string), json_28);
    assertEqual(actual_178, expected_178);
})])), Test_testList("Data structure", ofArray([Test_testCase("list works", () => {
    const expected_180 = new FSharpResult$2(0, [ofArray([1, 2, 3])]);
    const actual_180 = fromString(list_3(int), "[1, 2, 3]");
    assertEqual(actual_180, expected_180);
}), Test_testCase("an invalid list output an error", () => {
    const expected_182 = new FSharpResult$2(1, ["Error at: `$`\nExpecting a list but instead got: 1"]);
    const actual_182 = fromString(list_3(int), "1");
    assertEqual(actual_182, expected_182);
}), Test_testCase("array works", () => {
    const expected_184 = new FSharpResult$2(0, [[1, 2, 3]]);
    const actual_184 = fromString(array(int), "[1, 2, 3]");
    assertEqual(actual_184, expected_184);
}), Test_testCase("an invalid array output an error", () => {
    const expected_186 = new FSharpResult$2(1, ["Error at: `$`\nExpecting an array but instead got: 1"]);
    const actual_186 = fromString(array(int), "1");
    assertEqual(actual_186, expected_186);
}), Test_testCase("keys works", () => {
    const expected_188 = new FSharpResult$2(0, [ofArray(["a", "b", "c"])]);
    const actual_188 = fromString(keys_1, "{ \"a\": 1, \"b\": 2, \"c\": 3 }");
    assertEqual(actual_188, expected_188);
}), Test_testCase("keys returns an error for invalid objects", () => {
    const expected_190 = new FSharpResult$2(1, ["Error at: `$`\nExpecting an object but instead got: 1"]);
    const actual_190 = fromString(keys_1, "1");
    assertEqual(actual_190, expected_190);
}), Test_testCase("keyValuePairs works", () => {
    const expected_192 = new FSharpResult$2(0, [ofArray([["a", 1], ["b", 2], ["c", 3]])]);
    const actual_192 = fromString(keyValuePairs(int), "{ \"a\": 1, \"b\": 2, \"c\": 3 }");
    assertEqual(actual_192, expected_192);
}), Test_testCase("dict works", () => {
    const expected_194 = new FSharpResult$2(0, [ofList(ofArray([["a", 1], ["b", 2], ["c", 3]]), {
        Compare: comparePrimitives,
    })]);
    const actual_194 = fromString(dict(int), "{ \"a\": 1, \"b\": 2, \"c\": 3 }");
    assertEqual(actual_194, expected_194);
}), Test_testCase("dict with custom decoder works", () => {
    const expected_196 = new FSharpResult$2(0, [ofList(ofArray([["a", Record2_Create(1, 1)], ["b", Record2_Create(2, 2)], ["c", Record2_Create(3, 3)]]), {
        Compare: comparePrimitives,
    })]);
    const decodePoint = map2(Record2_Create, field("a", float), field("b", float));
    const actual_196 = fromString(dict(decodePoint), "\n{\n    \"a\":\n        {\n            \"a\": 1.0,\n            \"b\": 1.0\n        },\n    \"b\":\n        {\n            \"a\": 2.0,\n            \"b\": 2.0\n        },\n    \"c\":\n        {\n            \"a\": 3.0,\n            \"b\": 3.0\n        }\n}\n                        ");
    assertEqual(actual_196, expected_196);
}), Test_testCase("an invalid dict output an error", () => {
    const expected_198 = new FSharpResult$2(1, ["Error at: `$`\nExpecting an object but instead got: 1"]);
    const actual_198 = fromString(dict(int), "1");
    assertEqual(actual_198, expected_198);
}), Test_testCase("map\' works", () => {
    const expected_200 = new FSharpResult$2(0, [ofList(ofArray([[1, "x"], [2, "y"], [3, "z"]]), {
        Compare: comparePrimitives,
    })]);
    const actual_200 = fromString(map$0027(int, string), "[ [ 1, \"x\" ], [ 2, \"y\" ], [ 3, \"z\" ] ]");
    assertEqual(actual_200, expected_200);
}), Test_testCase("map\' with custom key decoder works", () => {
    const expected_202 = new FSharpResult$2(0, [ofList(ofArray([[[1, 6], "a"], [[2, 7], "b"], [[3, 8], "c"]]), {
        Compare: compareArrays,
    })]);
    const decodePoint_1 = map2((x_4, y_4) => [x_4, y_4], field("x", int), field("y", int));
    const actual_202 = fromString(map$0027(decodePoint_1, string), "\n[\n    [\n        {\n            \"x\": 1,\n            \"y\": 6\n        },\n        \"a\"\n    ],\n    [\n        {\n            \"x\": 2,\n            \"y\": 7\n        },\n        \"b\"\n    ],\n    [\n        {\n            \"x\": 3,\n            \"y\": 8\n        },\n        \"c\"\n    ]\n]\n                        ");
    assertEqual(actual_202, expected_202);
})])), Test_testList("Inconsistent structure", ofArray([Test_testCase("oneOf works", () => {
    const expected_204 = new FSharpResult$2(0, [ofArray([1, 2, 0, 4])]);
    const badInt = oneOf(ofArray([int, nil(0)]));
    const actual_204 = fromString(list_3(badInt), "[1,2,null,4]");
    assertEqual(actual_204, expected_204);
}), Test_testCase("oneOf works in combination with object builders", () => {
    const json_29 = "{ \"Bar\": { \"name\": \"maxime\", \"age\": 25 } }";
    const expected_206 = new FSharpResult$2(0, [new FSharpChoice$2(1, [new SmallRecord("maxime")])]);
    const decoder1 = object((get$) => {
        let objectArg;
        return new SmallRecord((objectArg = get$.Required, objectArg.Field("name", string)));
    });
    const decoder2 = oneOf(ofArray([map((Item) => (new FSharpChoice$2(0, [Item])), field("Foo", decoder1)), map((Item_1) => (new FSharpChoice$2(1, [Item_1])), field("Bar", decoder1))]));
    const actual_206 = fromString(decoder2, json_29);
    assertEqual(actual_206, expected_206);
}), Test_testCase("oneOf works with optional", () => {
    const decoder_7 = oneOf(ofArray([map((Item_2) => (new Price(0, [Item_2])), field("Normal", float)), map((Item_3) => (new Price(1, [Item_3])), field("Reduced", option_3(float))), map((_arg_108) => (new Price(2, [])), field("Zero", bool))]));
    assertEqual(fromString(decoder_7, "{\"Normal\": 4.5}"), new FSharpResult$2(0, [new Price(0, [4.5])]));
    assertEqual(fromString(decoder_7, "{\"Reduced\": 4.5}"), new FSharpResult$2(0, [new Price(1, [4.5])]));
    assertEqual(fromString(decoder_7, "{\"Reduced\": null}"), new FSharpResult$2(0, [new Price(1, [void 0])]));
    assertEqual(fromString(decoder_7, "{\"Zero\": true}"), new FSharpResult$2(0, [new Price(2, [])]));
}), Test_testCase("oneOf output errors if all case fails", () => {
    const expected_216 = new FSharpResult$2(1, ["\nThe following errors were found:\n\nError at: `$.[0]`\nExpecting a string but instead got: 1\n\nError at: `$.[0]`\nExpecting an object but instead got:\n1\n                        ".trim()]);
    const badInt_1 = oneOf(ofArray([string, field("test", string)]));
    const actual_216 = fromString(list_3(badInt_1), "[1,2,null,4]");
    assertEqual(actual_216, expected_216);
}), Test_testCase("optional works", () => {
    const json_30 = "{ \"name\": \"maxime\", \"age\": 25, \"something_undefined\": null }";
    const expectedValid = new FSharpResult$2(0, ["maxime"]);
    const actualValid = fromString(optional("name", string), json_30);
    assertEqual(actualValid, expectedValid);
    const matchValue_1 = fromString(optional("name", int), json_30);
    if (matchValue_1.tag === 0) {
        throw new Error("Expected type error for `name` field");
    }
    const expectedMissingField = new FSharpResult$2(0, [void 0]);
    const actualMissingField = fromString(optional("height", int), json_30);
    assertEqual(actualMissingField, expectedMissingField);
    const expectedUndefinedField = new FSharpResult$2(0, [void 0]);
    const actualUndefinedField = fromString(optional("something_undefined", string), json_30);
    assertEqual(actualUndefinedField, expectedUndefinedField);
}), Test_testCase("optional returns Error value if decoder fails", () => {
    const json_31 = "{ \"name\": 12, \"age\": 25 }";
    const expected_221 = new FSharpResult$2(1, ["\nError at: `$.name`\nExpecting a string but instead got: 12\n                        ".trim()]);
    const actual_221 = fromString(optional("name", string), json_31);
    assertEqual(actual_221, expected_221);
}), Test_testCase("optionalAt works", () => {
    const json_32 = "{ \"data\" : { \"name\": \"maxime\", \"age\": 25, \"something_undefined\": null } }";
    const expectedValid_1 = new FSharpResult$2(0, ["maxime"]);
    const actualValid_1 = fromString(optionalAt(ofArray(["data", "name"]), string), json_32);
    assertEqual(actualValid_1, expectedValid_1);
    const matchValue_2 = fromString(optionalAt(ofArray(["data", "name"]), int), json_32);
    if (matchValue_2.tag === 0) {
        throw new Error("Expected type error for `name` field");
    }
    const expectedMissingField_1 = new FSharpResult$2(0, [void 0]);
    const actualMissingField_1 = fromString(optionalAt(ofArray(["data", "height"]), int), json_32);
    assertEqual(actualMissingField_1, expectedMissingField_1);
    const expectedUndefinedField_1 = new FSharpResult$2(0, [void 0]);
    const actualUndefinedField_1 = fromString(optionalAt(ofArray(["data", "something_undefined"]), string), json_32);
    assertEqual(actualUndefinedField_1, expectedUndefinedField_1);
    const expectedUndefinedField_2 = new FSharpResult$2(0, [void 0]);
    const actualUndefinedField_2 = fromString(optionalAt(ofArray(["data", "something_undefined", "name"]), string), json_32);
    assertEqual(actualUndefinedField_2, expectedUndefinedField_2);
}), Test_testCase("combining field and option decoders works", () => {
    const json_33 = "{ \"name\": \"maxime\", \"age\": 25, \"something_undefined\": null }";
    const expectedValid_2 = new FSharpResult$2(0, ["maxime"]);
    const actualValid_2 = fromString(field("name", option_3(string)), json_33);
    assertEqual(actualValid_2, expectedValid_2);
    const matchValue_3 = fromString(field("name", option_3(int)), json_33);
    if (matchValue_3.tag === 0) {
        throw new Error("Expected type error for `name` field #1");
    }
    else {
        const msg = matchValue_3.fields[0];
        const expected_228 = "\nError at: `$.name`\nExpecting an int but instead got: \"maxime\"\n                        ".trim();
        assertEqual(msg, expected_228);
    }
    const matchValue_4 = fromString(field("this_field_do_not_exist", option_3(int)), json_33);
    if (matchValue_4.tag === 0) {
        throw new Error("Expected type error for `name` field #2");
    }
    else {
        const msg_1 = matchValue_4.fields[0];
        const expected_230 = "\nError at: `$`\nExpecting an object with a field named `this_field_do_not_exist` but instead got:\n{\n    \"name\": \"maxime\",\n    \"age\": 25,\n    \"something_undefined\": null\n}\n                        ".trim();
        assertEqual(msg_1, expected_230);
    }
    const matchValue_5 = fromString(field("something_undefined", option_3(int)), json_33);
    if (matchValue_5.tag === 0) {
        const result = matchValue_5.fields[0];
        assertEqual(result, void 0);
    }
    else {
        throw new Error("`Decode.field \"something_undefined\" (Decode.option Decode.int)` test should pass");
    }
    const expectedValid2 = new FSharpResult$2(0, ["maxime"]);
    const actualValid2 = fromString(option_3(field("name", string)), json_33);
    assertEqual(actualValid2, expectedValid2);
    const matchValue_6 = fromString(option_3(field("name", int)), json_33);
    if (matchValue_6.tag === 0) {
        throw new Error("Expected type error for `name` field #3");
    }
    else {
        const msg_2 = matchValue_6.fields[0];
        const expected_234 = "\nError at: `$.name`\nExpecting an int but instead got: \"maxime\"\n                        ".trim();
        assertEqual(msg_2, expected_234);
    }
    const matchValue_7 = fromString(option_3(field("this_field_do_not_exist", int)), json_33);
    if (matchValue_7.tag === 0) {
        throw new Error("Expected type error for `name` field #4");
    }
    else {
        const msg_3 = matchValue_7.fields[0];
        const expected_236 = "\nError at: `$`\nExpecting an object with a field named `this_field_do_not_exist` but instead got:\n{\n    \"name\": \"maxime\",\n    \"age\": 25,\n    \"something_undefined\": null\n}\n                        ".trim();
        assertEqual(msg_3, expected_236);
    }
    const matchValue_8 = fromString(option_3(field("something_undefined", int)), json_33);
    if (matchValue_8.tag === 0) {
        throw new Error("Expected type error for `name` field");
    }
    else {
        const msg_4 = matchValue_8.fields[0];
        const expected_238 = "\nError at: `$.something_undefined`\nExpecting an int but instead got: null\n                        ".trim();
        assertEqual(msg_4, expected_238);
    }
    const matchValue_9 = fromString(field("height", option_3(int)), json_33);
    if (matchValue_9.tag === 0) {
        throw new Error("Expected type error for `height` field");
    }
    else {
        const msg_5 = matchValue_9.fields[0];
        const expected_240 = "\nError at: `$`\nExpecting an object with a field named `height` but instead got:\n{\n    \"name\": \"maxime\",\n    \"age\": 25,\n    \"something_undefined\": null\n}\n                        ".trim();
        assertEqual(msg_5, expected_240);
    }
    const expectedUndefinedField_3 = new FSharpResult$2(0, [void 0]);
    const actualUndefinedField_3 = fromString(field("something_undefined", option_3(string)), json_33);
    assertEqual(actualUndefinedField_3, expectedUndefinedField_3);
})])), Test_testList("Fancy decoding", ofArray([Test_testCase("null works (test on an int)", () => {
    const expected_243 = new FSharpResult$2(0, [20]);
    const actual_237 = fromString(nil(20), "null");
    assertEqual(actual_237, expected_243);
}), Test_testCase("null works (test on a boolean)", () => {
    const expected_245 = new FSharpResult$2(0, [false]);
    const actual_239 = fromString(nil(false), "null");
    assertEqual(actual_239, expected_245);
}), Test_testCase("succeed works", () => {
    const expected_247 = new FSharpResult$2(0, [7]);
    const actual_241 = fromString(succeed_1(7), "true");
    assertEqual(actual_241, expected_247);
}), Test_testCase("succeed output an error if the JSON is invalid", () => {
    const expected_249 = new FSharpResult$2(1, ["Given an invalid JSON: Unexpected token \'m\', \"maxime\" is not valid JSON"]);
    const actual_243 = fromString(succeed_1(7), "maxime");
    assertEqual(actual_243, expected_249);
}), Test_testCase("fail works", () => {
    const msg_6 = "Failing because it\'s fun";
    const expected_251 = new FSharpResult$2(1, ["Error at: `$`\nThe following `failure` occurred with the decoder: " + msg_6]);
    const actual_245 = fromString(fail(msg_6), "true");
    assertEqual(actual_245, expected_251);
}), Test_testCase("andMap works for any arity", () => {
    const json_34 = "{\"a\": 1,\"b\": 2,\"c\": 3,\"d\": 4,\"e\": 5,\"f\": 6,\"g\": 7,\"h\": 8,\"i\": 9,\"j\": 10,\"k\": 11}";
    const decodeRecord10 = andMap()(field("k", int))(andMap()(field("j", int))(andMap()(field("i", int))(andMap()(field("h", int))(andMap()(field("g", int))(andMap()(field("f", int))(andMap()(field("e", int))(andMap()(field("d", int))(andMap()(field("c", int))(andMap()(field("b", int))(andMap()(field("a", int))(succeed_1((a_5) => ((b_5) => ((c) => ((d_5) => ((e) => ((f) => ((g) => ((h) => ((i) => ((j) => ((k) => Record10_Create(a_5, b_5, c, d_5, e, f, g, h, i, j, k)))))))))))))))))))))));
    const actual_247 = fromString(decodeRecord10, json_34);
    const expected_253 = new FSharpResult$2(0, [new Record10(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11)]);
    assertEqual(actual_247, expected_253);
}), Test_testCase("andThen works", () => {
    const expected_255 = new FSharpResult$2(0, [1]);
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
    const actual_249 = fromString(info, "{ \"version\": 3, \"data\": 2 }");
    assertEqual(actual_249, expected_255);
}), Test_testCase("andThen generate an error if an error occuered", () => {
    const expected_257 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an object with a field named `version` but instead got:\n{\n    \"info\": 3,\n    \"data\": 2\n}\n                        ".trim()]);
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
    const actual_251 = fromString(info_1, "{ \"info\": 3, \"data\": 2 }");
    assertEqual(actual_251, expected_257);
}), Test_testCase("all works", () => {
    const expected_259 = new FSharpResult$2(0, [ofArray([1, 2, 3])]);
    const decodeAll = all(ofArray([succeed_1(1), succeed_1(2), succeed_1(3)]));
    const actual_253 = fromString(decodeAll, "{}");
    assertEqual(actual_253, expected_259);
}), Test_testCase("combining Decode.all and Decode.keys works", () => {
    const expected_261 = new FSharpResult$2(0, [ofArray([1, 2, 3])]);
    const decoder_11 = andThen_1((keys) => all(map_1((key) => field(key, int), List_except(["special_property"], keys, {
        Equals: (x_5, y_5) => (x_5 === y_5),
        GetHashCode: stringHash,
    }))), keys_1);
    const actual_255 = fromString(decoder_11, "{ \"a\": 1, \"b\": 2, \"c\": 3 }");
    assertEqual(actual_255, expected_261);
}), Test_testCase("all succeeds on empty lists", () => {
    const expected_263 = new FSharpResult$2(0, [empty()]);
    const decodeNone = all(empty());
    const actual_257 = fromString(decodeNone, "{}");
    assertEqual(actual_257, expected_263);
}), Test_testCase("all fails when one decoder fails", () => {
    const expected_265 = new FSharpResult$2(1, ["Error at: `$`\nExpecting an int but instead got: {}"]);
    const decodeAll_1 = all(ofArray([succeed_1(1), int, succeed_1(3)]));
    const actual_259 = fromString(decodeAll_1, "{}");
    assertEqual(actual_259, expected_265);
})])), Test_testList("Mapping", ofArray([Test_testCase("map works", () => {
    const expected_267 = new FSharpResult$2(0, [6]);
    const stringLength = map((str) => str.length, string);
    const actual_261 = fromString(stringLength, "\"maxime\"");
    assertEqual(actual_261, expected_267);
}), Test_testCase("map2 works", () => {
    const expected_269 = new FSharpResult$2(0, [new Record2(1, 2)]);
    const decodePoint_2 = map2(Record2_Create, field("a", float), field("b", float));
    const actual_263 = fromString(decodePoint_2, jsonRecord);
    assertEqual(actual_263, expected_269);
}), Test_testCase("map3 works", () => {
    const expected_271 = new FSharpResult$2(0, [new Record3(1, 2, 3)]);
    const decodePoint_3 = map3(Record3_Create, field("a", float), field("b", float), field("c", float));
    const actual_265 = fromString(decodePoint_3, jsonRecord);
    assertEqual(actual_265, expected_271);
}), Test_testCase("map4 works", () => {
    const expected_273 = new FSharpResult$2(0, [new Record4(1, 2, 3, 4)]);
    const decodePoint_4 = map4(Record4_Create, field("a", float), field("b", float), field("c", float), field("d", float));
    const actual_267 = fromString(decodePoint_4, jsonRecord);
    assertEqual(actual_267, expected_273);
}), Test_testCase("map5 works", () => {
    const expected_275 = new FSharpResult$2(0, [new Record5(1, 2, 3, 4, 5)]);
    const decodePoint_5 = map5(Record5_Create, field("a", float), field("b", float), field("c", float), field("d", float), field("e", float));
    const actual_269 = fromString(decodePoint_5, jsonRecord);
    assertEqual(actual_269, expected_275);
}), Test_testCase("map6 works", () => {
    const expected_277 = new FSharpResult$2(0, [new Record6(1, 2, 3, 4, 5, 6)]);
    const decodePoint_6 = map6(Record6_Create, field("a", float), field("b", float), field("c", float), field("d", float), field("e", float), field("f", float));
    const actual_271 = fromString(decodePoint_6, jsonRecord);
    assertEqual(actual_271, expected_277);
}), Test_testCase("map7 works", () => {
    const expected_279 = new FSharpResult$2(0, [new Record7(1, 2, 3, 4, 5, 6, 7)]);
    const decodePoint_7 = map7(Record7_Create, field("a", float), field("b", float), field("c", float), field("d", float), field("e", float), field("f", float), field("g", float));
    const actual_273 = fromString(decodePoint_7, jsonRecord);
    assertEqual(actual_273, expected_279);
}), Test_testCase("map8 works", () => {
    const expected_281 = new FSharpResult$2(0, [new Record8(1, 2, 3, 4, 5, 6, 7, 8)]);
    const decodePoint_8 = map8(Record8_Create, field("a", float), field("b", float), field("c", float), field("d", float), field("e", float), field("f", float), field("g", float), field("h", float));
    const actual_275 = fromString(decodePoint_8, jsonRecord);
    assertEqual(actual_275, expected_281);
}), Test_testCase("map2 generate an error if invalid", () => {
    const expected_283 = new FSharpResult$2(1, ["Error at: `$.a`\nExpecting a float but instead got: \"invalid_a_field\""]);
    const decodePoint_9 = map2(Record2_Create, field("a", float), field("b", float));
    const actual_277 = fromString(decodePoint_9, jsonRecordInvalid);
    assertEqual(actual_277, expected_283);
})])), Test_testList("object builder", ofArray([Test_testCase("get.Required.Field works", () => {
    const json_35 = "{ \"name\": \"maxime\", \"age\": 25 }";
    const expected_285 = new FSharpResult$2(0, [new SmallRecord("maxime")]);
    const decoder_12 = object((get$_1) => {
        let objectArg_1;
        return new SmallRecord((objectArg_1 = get$_1.Required, objectArg_1.Field("name", string)));
    });
    const actual_279 = fromString(decoder_12, json_35);
    assertEqual(actual_279, expected_285);
}), Test_testCase("get.Required.Field returns Error if field is missing", () => {
    const json_36 = "{ \"age\": 25 }";
    const expected_287 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an object with a field named `name` but instead got:\n{\n    \"age\": 25\n}\n                        ".trim()]);
    const decoder_13 = object((get$_2) => {
        let objectArg_2;
        return new SmallRecord((objectArg_2 = get$_2.Required, objectArg_2.Field("name", string)));
    });
    const actual_281 = fromString(decoder_13, json_36);
    assertEqual(actual_281, expected_287);
}), Test_testCase("get.Required.Field returns Error if type is incorrect", () => {
    const json_37 = "{ \"name\": 12, \"age\": 25 }";
    const expected_289 = new FSharpResult$2(1, ["\nError at: `$.name`\nExpecting a string but instead got: 12\n                        ".trim()]);
    const decoder_14 = object((get$_3) => {
        let objectArg_3;
        return new SmallRecord((objectArg_3 = get$_3.Required, objectArg_3.Field("name", string)));
    });
    const actual_283 = fromString(decoder_14, json_37);
    assertEqual(actual_283, expected_289);
}), Test_testCase("get.Optional.Field works", () => {
    const json_38 = "{ \"name\": \"maxime\", \"age\": 25 }";
    const expected_291 = new FSharpResult$2(0, [new SmallRecord2("maxime")]);
    const decoder_15 = object((get$_4) => {
        let objectArg_4;
        return new SmallRecord2((objectArg_4 = get$_4.Optional, objectArg_4.Field("name", string)));
    });
    const actual_285 = fromString(decoder_15, json_38);
    assertEqual(actual_285, expected_291);
}), Test_testCase("get.Optional.Field returns None value if field is missing", () => {
    const json_39 = "{ \"age\": 25 }";
    const expected_293 = new FSharpResult$2(0, [new SmallRecord2(void 0)]);
    const decoder_16 = object((get$_5) => {
        let objectArg_5;
        return new SmallRecord2((objectArg_5 = get$_5.Optional, objectArg_5.Field("name", string)));
    });
    const actual_287 = fromString(decoder_16, json_39);
    assertEqual(actual_287, expected_293);
}), Test_testCase("get.Optional.Field returns None if field is null", () => {
    const json_40 = "{ \"name\": null, \"age\": 25 }";
    const expected_295 = new FSharpResult$2(0, [new SmallRecord2(void 0)]);
    const decoder_17 = object((get$_6) => {
        let objectArg_6;
        return new SmallRecord2((objectArg_6 = get$_6.Optional, objectArg_6.Field("name", string)));
    });
    const actual_289 = fromString(decoder_17, json_40);
    assertEqual(actual_289, expected_295);
}), Test_testCase("get.Optional.Field returns Error value if decoder fails", () => {
    const json_41 = "{ \"name\": 12, \"age\": 25 }";
    const expected_297 = new FSharpResult$2(1, ["\nError at: `$.name`\nExpecting a string but instead got: 12\n                        ".trim()]);
    const decoder_18 = object((get$_7) => {
        let objectArg_7;
        return new SmallRecord2((objectArg_7 = get$_7.Optional, objectArg_7.Field("name", string)));
    });
    const actual_291 = fromString(decoder_18, json_41);
    assertEqual(actual_291, expected_297);
}), Test_testCase("nested get.Optional.Field > get.Required.Field returns None if field is null", () => {
    const json_42 = "{ \"user\": null, \"field2\": 25 }";
    const expected_299 = new FSharpResult$2(0, [new Model(void 0, 25)]);
    const userDecoder = object((get$_8) => {
        let objectArg_8, objectArg_9, objectArg_10;
        return new User((objectArg_8 = get$_8.Required, objectArg_8.Field("id", int)), (objectArg_9 = get$_8.Required, objectArg_9.Field("name", string)), (objectArg_10 = get$_8.Required, objectArg_10.Field("email", string)), 0);
    });
    const decoder_19 = object((get$_9) => {
        let objectArg_11, objectArg_12;
        return new Model((objectArg_11 = get$_9.Optional, objectArg_11.Field("user", userDecoder)), (objectArg_12 = get$_9.Required, objectArg_12.Field("field2", int)));
    });
    const actual_293 = fromString(decoder_19, json_42);
    assertEqual(actual_293, expected_299);
}), Test_testCase("get.Optional.Field returns Error if type is incorrect", () => {
    const json_43 = "{ \"name\": 12, \"age\": 25 }";
    const expected_301 = new FSharpResult$2(1, ["\nError at: `$.name`\nExpecting a string but instead got: 12\n                        ".trim()]);
    const decoder_20 = object((get$_10) => {
        let objectArg_13;
        return new SmallRecord2((objectArg_13 = get$_10.Optional, objectArg_13.Field("name", string)));
    });
    const actual_295 = fromString(decoder_20, json_43);
    assertEqual(actual_295, expected_301);
}), Test_testCase("get.Required.At works", () => {
    const json_44 = "{ \"user\": { \"name\": \"maxime\", \"age\": 25 } }";
    const expected_303 = new FSharpResult$2(0, [new SmallRecord("maxime")]);
    const decoder_21 = object((get$_11) => {
        let objectArg_14;
        return new SmallRecord((objectArg_14 = get$_11.Required, objectArg_14.At(ofArray(["user", "name"]), string)));
    });
    const actual_297 = fromString(decoder_21, json_44);
    assertEqual(actual_297, expected_303);
}), Test_testCase("get.Required.At returns Error if non-object in path", () => {
    const json_45 = "{ \"user\": \"maxime\" }";
    const expected_305 = new FSharpResult$2(1, ["\nError at: `$.user`\nExpecting an object but instead got:\n\"maxime\"\n                        ".trim()]);
    const decoder_22 = object((get$_12) => {
        let objectArg_15;
        return new SmallRecord((objectArg_15 = get$_12.Required, objectArg_15.At(ofArray(["user", "name"]), string)));
    });
    const actual_299 = fromString(decoder_22, json_45);
    assertEqual(actual_299, expected_305);
}), Test_testCase("get.Required.At returns Error if field missing", () => {
    const json_46 = "{ \"user\": { \"name\": \"maxime\", \"age\": 25 } }";
    const expected_307 = new FSharpResult$2(1, ["\nError at: `$.user.firstname`\nExpecting an object with path `user.firstname` but instead got:\n{\n    \"user\": {\n        \"name\": \"maxime\",\n        \"age\": 25\n    }\n}\nNode `firstname` is unkown.\n                        ".trim()]);
    const decoder_23 = object((get$_13) => {
        let objectArg_16;
        return new SmallRecord((objectArg_16 = get$_13.Required, objectArg_16.At(ofArray(["user", "firstname"]), string)));
    });
    const actual_301 = fromString(decoder_23, json_46);
    assertEqual(actual_301, expected_307);
}), Test_testCase("get.Required.At returns Error if type is incorrect", () => {
    const json_47 = "{ \"user\": { \"name\": 12, \"age\": 25 } }";
    const expected_309 = new FSharpResult$2(1, ["\nError at: `$.user.name`\nExpecting a string but instead got: 12\n                        ".trim()]);
    const decoder_24 = object((get$_14) => {
        let objectArg_17;
        return new SmallRecord((objectArg_17 = get$_14.Required, objectArg_17.At(ofArray(["user", "name"]), string)));
    });
    const actual_303 = fromString(decoder_24, json_47);
    assertEqual(actual_303, expected_309);
}), Test_testCase("get.Optional.At works", () => {
    const json_48 = "{ \"user\": { \"name\": \"maxime\", \"age\": 25 } }";
    const expected_311 = new FSharpResult$2(0, [new SmallRecord2("maxime")]);
    const decoder_25 = object((get$_15) => {
        let objectArg_18;
        return new SmallRecord2((objectArg_18 = get$_15.Optional, objectArg_18.At(ofArray(["user", "name"]), string)));
    });
    const actual_305 = fromString(decoder_25, json_48);
    assertEqual(actual_305, expected_311);
}), Test_testCase("get.Optional.At returns \'type error\' if non-object in path", () => {
    const json_49 = "{ \"user\": \"maxime\" }";
    const expected_313 = new FSharpResult$2(1, ["\nError at: `$.user`\nExpecting an object but instead got:\n\"maxime\"\n                        ".trim()]);
    const decoder_26 = object((get$_16) => {
        let objectArg_19;
        return new SmallRecord2((objectArg_19 = get$_16.Optional, objectArg_19.At(ofArray(["user", "name"]), string)));
    });
    const actual_307 = fromString(decoder_26, json_49);
    assertEqual(actual_307, expected_313);
}), Test_testCase("get.Optional.At returns None if field missing", () => {
    const json_50 = "{ \"user\": { \"name\": \"maxime\", \"age\": 25 } }";
    const expected_315 = new FSharpResult$2(0, [new SmallRecord2(void 0)]);
    const decoder_27 = object((get$_17) => {
        let objectArg_20;
        return new SmallRecord2((objectArg_20 = get$_17.Optional, objectArg_20.At(ofArray(["user", "firstname"]), string)));
    });
    const actual_309 = fromString(decoder_27, json_50);
    assertEqual(actual_309, expected_315);
}), Test_testCase("get.Optional.At returns Error if type is incorrect", () => {
    const json_51 = "{ \"user\": { \"name\": 12, \"age\": 25 } }";
    const expected_317 = new FSharpResult$2(1, ["\nError at: `$.user.name`\nExpecting a string but instead got: 12\n                        ".trim()]);
    const decoder_28 = object((get$_18) => {
        let objectArg_21;
        return new SmallRecord2((objectArg_21 = get$_18.Optional, objectArg_21.At(ofArray(["user", "name"]), string)));
    });
    const actual_311 = fromString(decoder_28, json_51);
    assertEqual(actual_311, expected_317);
}), Test_testCase("complex object builder works", () => {
    const expected_319 = new FSharpResult$2(0, [User_Create(67, "", "user@mail.com", 0)]);
    const userDecoder_1 = object((get$_19) => {
        let objectArg_22, objectArg_23, objectArg_24;
        return new User((objectArg_22 = get$_19.Required, objectArg_22.Field("id", int)), defaultArg((objectArg_23 = get$_19.Optional, objectArg_23.Field("name", string)), ""), (objectArg_24 = get$_19.Required, objectArg_24.Field("email", string)), 0);
    });
    const actual_313 = fromString(userDecoder_1, "{ \"id\": 67, \"email\": \"user@mail.com\" }");
    assertEqual(actual_313, expected_319);
}), Test_testCase("get.Field.Raw works", () => {
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
        let objectArg_25;
        return new MyObj((objectArg_25 = get$_20.Required, objectArg_25.Field("enabled", bool)), get$_20.Required.Raw(shapeDecoder));
    });
    const actual_315 = fromString(decoder_30, json_52);
    const expected_321 = new FSharpResult$2(0, [new MyObj(true, new Shape(0, [20]))]);
    assertEqual(actual_315, expected_321);
}), Test_testCase("get.Field.Raw returns Error if a decoder fail", () => {
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
        let objectArg_26;
        return new MyObj((objectArg_26 = get$_21.Required, objectArg_26.Field("enabled", bool)), get$_21.Required.Raw(shapeDecoder_1));
    });
    const actual_317 = fromString(decoder_32, json_53);
    const expected_323 = new FSharpResult$2(1, ["Error at: `$`\nThe following `failure` occurred with the decoder: Unknown shape type custom_shape"]);
    assertEqual(actual_317, expected_323);
}), Test_testCase("get.Field.Raw returns Error if a field is missing in the \'raw decoder\'", () => {
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
        let objectArg_27;
        return new MyObj((objectArg_27 = get$_22.Required, objectArg_27.Field("enabled", bool)), get$_22.Required.Raw(shapeDecoder_2));
    });
    const actual_319 = fromString(decoder_34, json_54);
    const expected_325 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an object with a field named `radius` but instead got:\n{\n    \"enabled\": true,\n    \"shape\": \"circle\"\n}                   ".trim()]);
    assertEqual(actual_319, expected_325);
}), Test_testCase("get.Optional.Raw works", () => {
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
        let objectArg_28;
        return new MyObj2((objectArg_28 = get$_23.Required, objectArg_28.Field("enabled", bool)), get$_23.Optional.Raw(shapeDecoder_3));
    });
    const actual_321 = fromString(decoder_36, json_55);
    const expected_327 = new FSharpResult$2(0, [new MyObj2(true, new Shape(0, [20]))]);
    assertEqual(actual_321, expected_327);
}), Test_testCase("get.Optional.Raw returns None if a field is missing", () => {
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
        let objectArg_29;
        return new MyObj2((objectArg_29 = get$_24.Required, objectArg_29.Field("enabled", bool)), get$_24.Optional.Raw(shapeDecoder_4));
    });
    const actual_323 = fromString(decoder_38, json_56);
    const expected_329 = new FSharpResult$2(0, [new MyObj2(true, void 0)]);
    assertEqual(actual_323, expected_329);
}), Test_testCase("get.Optional.Raw returns an Error if a decoder fail", () => {
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
        let objectArg_30;
        return new MyObj2((objectArg_30 = get$_25.Required, objectArg_30.Field("enabled", bool)), get$_25.Optional.Raw(shapeDecoder_5));
    });
    const actual_325 = fromString(decoder_40, json_57);
    const expected_331 = new FSharpResult$2(1, ["Error at: `$`\nThe following `failure` occurred with the decoder: Unknown shape type invalid_shape"]);
    assertEqual(actual_325, expected_331);
}), Test_testCase("get.Optional.Raw returns an Error if the type is invalid", () => {
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
        let objectArg_31;
        return new MyObj2((objectArg_31 = get$_26.Required, objectArg_31.Field("enabled", bool)), get$_26.Optional.Raw(shapeDecoder_6));
    });
    const actual_327 = fromString(decoder_42, json_58);
    const expected_333 = new FSharpResult$2(1, ["Error at: `$.radius`\nExpecting an int but instead got: \"maxime\""]);
    assertEqual(actual_327, expected_333);
}), Test_testCase("get.Optional.Raw returns None if a decoder fails with null", () => {
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
        let objectArg_32;
        return new MyObj2((objectArg_32 = get$_27.Required, objectArg_32.Field("enabled", bool)), get$_27.Optional.Raw(shapeDecoder_7));
    });
    const actual_329 = fromString(decoder_44, json_59);
    const expected_335 = new FSharpResult$2(0, [new MyObj2(true, void 0)]);
    assertEqual(actual_329, expected_335);
}), Test_testCase("Object builders returns all the Errors", () => {
    const json_60 = "{ \"age\": 25, \"fieldC\": \"not_a_number\", \"fieldD\": { \"sub_field\": \"not_a_boolean\" } }";
    const expected_337 = new FSharpResult$2(1, ["\nThe following errors were found:\n\nError at: `$`\nExpecting an object with a field named `missing_field_1` but instead got:\n{\n    \"age\": 25,\n    \"fieldC\": \"not_a_number\",\n    \"fieldD\": {\n        \"sub_field\": \"not_a_boolean\"\n    }\n}\n\nError at: `$.missing_field_2.sub_field`\nExpecting an object with path `missing_field_2.sub_field` but instead got:\n{\n    \"age\": 25,\n    \"fieldC\": \"not_a_number\",\n    \"fieldD\": {\n        \"sub_field\": \"not_a_boolean\"\n    }\n}\nNode `sub_field` is unkown.\n\nError at: `$.fieldC`\nExpecting an int but instead got: \"not_a_number\"\n\nError at: `$.fieldD.sub_field`\nExpecting a boolean but instead got: \"not_a_boolean\"\n                        ".trim()]);
    const decoder_45 = object((get$_28) => {
        let objectArg_33, objectArg_34, objectArg_35, objectArg_36;
        return new MediumRecord((objectArg_33 = get$_28.Required, objectArg_33.Field("missing_field_1", string)), (objectArg_34 = get$_28.Required, objectArg_34.At(ofArray(["missing_field_2", "sub_field"]), string)), defaultArg((objectArg_35 = get$_28.Optional, objectArg_35.Field("fieldC", int)), -1), defaultArg((objectArg_36 = get$_28.Optional, objectArg_36.At(ofArray(["fieldD", "sub_field"]), bool)), false));
    });
    const actual_331 = fromString(decoder_45, json_60);
    assertEqual(actual_331, expected_337);
}), Test_testCase("Test", () => {
    const json_61 = "\n                    {\n                        \"person\": {\n                            \"name\": \"maxime\"\n                        },\n                        \"post\": null\n                    }\n                    ";
    const personDecoder = object((get$_29) => {
        let objectArg_37;
        return new Person((objectArg_37 = get$_29.Required, objectArg_37.Field("name", string)));
    });
    const postDecoder = object((get$_30) => {
        let title;
        const objectArg_38 = get$_30.Required;
        title = objectArg_38.Field("title", string);
        const arg_87 = head(title.split(""));
        toConsole(printf("Title: %A"))(arg_87);
        return new Post(title);
    });
    const dataDecoder = object((get$_31) => {
        let objectArg_39, objectArg_40;
        return new Data((objectArg_39 = get$_31.Required, objectArg_39.Field("person", personDecoder)), (objectArg_40 = get$_31.Optional, objectArg_40.Field("post", postDecoder)));
    });
    const actual_333 = fromString(dataDecoder, json_61);
    const expected_339 = new FSharpResult$2(0, [new Data(new Person("maxime"), void 0)]);
    assertEqual(actual_333, expected_339);
})]))]));

