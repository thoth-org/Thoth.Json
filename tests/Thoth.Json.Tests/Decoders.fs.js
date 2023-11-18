import { Union, Record } from "./Thoth.Json.JavaScript.Tests/fable_modules/fable-library.4.5.0/Types.js";
import { union_type, string_type, record_type, float64_type, int32_type } from "./Thoth.Json.JavaScript.Tests/fable_modules/fable-library.4.5.0/Reflection.js";
import { Test_testCase, Test_testList } from "./Thoth.Json.JavaScript.Tests/fable_modules/Fable.Mocha.2.17.0/Mocha.fs.js";
import { stringHash, int32ToString, compareArrays, comparePrimitives, defaultOf, assertEqual } from "./Thoth.Json.JavaScript.Tests/fable_modules/fable-library.4.5.0/Util.js";
import { map8, map7, map6, map5, map4, map3, all, andThen as andThen_1, andMap, fail, succeed as succeed_1, optionalAt, optional, option as option_3, map, object, oneOf, map$0027, map2, dict, keyValuePairs, keys as keys_1, array, list as list_3, index, at, field, tuple8, tuple7, nil, tuple6, tuple5, tuple4, tuple3, tuple2, timespan, datetimeOffset, datetimeLocal, datetimeUtc, bigint, sbyte, byte, uint64, uint32, int64, uint16, int16, int, bool, char, string, unit, float, fromValue } from "../packages/Thoth.Json.Core/Decode.fs.js";
import { fromString, helpers } from "../packages/Thoth.Json.JavaScript/Decode.fs.js";
import { FSharpChoice$2, FSharpResult$2 } from "./Thoth.Json.JavaScript.Tests/fable_modules/fable-library.4.5.0/Choice.js";
import { Data, Post, Person, MediumRecord, MyObj2, Shape, MyObj, Shape_get_DecoderRectangle, Shape_get_DecoderCircle, User_Create, User, Model, SmallRecord2, Record8_Create, Record8, Record7_Create, Record7, Record6_Create, Record6, Record5_Create, Record5, Record4_Create, Record4, Record3_Create, Record3, Record2, Record10, Record10_Create, Price, Record2_Create, SmallRecord_get_Decoder, SmallRecord, CustomException } from "./Types.fs.js";
import { empty, map as map_1, singleton, ofArray } from "./Thoth.Json.JavaScript.Tests/fable_modules/fable-library.4.5.0/List.js";
import { fromInt32 } from "./Thoth.Json.JavaScript.Tests/fable_modules/fable-library.4.5.0/BigInt.js";
import { toString, toUniversalTime, create } from "./Thoth.Json.JavaScript.Tests/fable_modules/fable-library.4.5.0/Date.js";
import { toConsole, printf, toText } from "./Thoth.Json.JavaScript.Tests/fable_modules/fable-library.4.5.0/String.js";
import { create as create_1 } from "./Thoth.Json.JavaScript.Tests/fable_modules/fable-library.4.5.0/DateOffset.js";
import { create as create_2, fromHours } from "./Thoth.Json.JavaScript.Tests/fable_modules/fable-library.4.5.0/TimeSpan.js";
import { uint16 as uint16_1, int16 as int16_1, uint32 as uint32_1, int as int_1, byte as byte_1, sbyte as sbyte_1, succeed, andThen } from "../packages/Thoth.Json.Core/./Decode.fs.js";
import { ofList } from "./Thoth.Json.JavaScript.Tests/fable_modules/fable-library.4.5.0/Map.js";
import { List_except } from "./Thoth.Json.JavaScript.Tests/fable_modules/fable-library.4.5.0/Seq2.js";
import { defaultArg } from "./Thoth.Json.JavaScript.Tests/fable_modules/fable-library.4.5.0/Option.js";
import { head } from "./Thoth.Json.JavaScript.Tests/fable_modules/fable-library.4.5.0/Seq.js";

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
    assertEqual(fromValue(helpers, "$", float, b), new FSharpResult$2(1, ["Error at: `$`\nExpecting a float but decoder failed. Couldn\'t report given value due to circular structure. "]));
}), Test_testCase("invalid json", () => {
    assertEqual(fromString(float, "maxime"), new FSharpResult$2(1, ["Given an invalid JSON: Unexpected token \'m\', \"maxime\" is not valid JSON"]));
}), Test_testCase("invalid json #3 - Special case for Thoth.Json.Net", () => {
    const incorrectJson = "\n                {\n                \"Ab\": [\n                    \"RecordC\",\n                    {\n                    \"C1\": \"\",\n                    \"C2\": \"\",\n                ";
    assertEqual(fromString(float, incorrectJson), new FSharpResult$2(1, ["Given an invalid JSON: Expected double-quoted property name in JSON at position 172"]));
}), Test_testCase("user exceptions are not captured by the decoders", () => {
    const decoder = {
        Decode(_arg_4, _arg_5, _arg_6) {
            throw new CustomException();
        },
    };
    assertEqual((() => {
        try {
            fromString(decoder, "\"maxime\"");
            return false;
        }
        catch (matchValue) {
            if (matchValue instanceof CustomException) {
                return true;
            }
            else {
                throw matchValue;
            }
        }
    })(), true);
})])), Test_testList("Primitives", ofArray([Test_testCase("unit works", () => {
    assertEqual(fromString(unit, "null"), new FSharpResult$2(0, [void 0]));
}), Test_testCase("a string works", () => {
    assertEqual(fromString(string, "\"maxime\""), new FSharpResult$2(0, ["maxime"]));
}), Test_testCase("a string with new line works", () => {
    assertEqual(fromString(string, "\"a\\nb\""), new FSharpResult$2(0, ["a\nb"]));
}), Test_testCase("a string with new line character works", () => {
    assertEqual(fromString(string, "\"a\\\\nb\""), new FSharpResult$2(0, ["a\\nb"]));
}), Test_testCase("a string with tab works", () => {
    assertEqual(fromString(string, "\"a\\tb\""), new FSharpResult$2(0, ["a\tb"]));
}), Test_testCase("a string with tab character works", () => {
    assertEqual(fromString(string, "\"a\\\\tb\""), new FSharpResult$2(0, ["a\\tb"]));
}), Test_testCase("a char works", () => {
    assertEqual(fromString(char, "\"a\""), new FSharpResult$2(0, ["a"]));
}), Test_testCase("a char reports an error if there are more than 1 characters in the string", () => {
    const expected_22 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a single character string but instead got: \"ab\"\n                        ".trim()]);
    assertEqual(fromString(char, "\"ab\""), expected_22);
}), Test_testCase("a float works", () => {
    assertEqual(fromString(float, "1.2"), new FSharpResult$2(0, [1.2]));
}), Test_testCase("a float from int works", () => {
    assertEqual(fromString(float, "1"), new FSharpResult$2(0, [1]));
}), Test_testCase("a bool works", () => {
    assertEqual(fromString(bool, "true"), new FSharpResult$2(0, [true]));
}), Test_testCase("an invalid bool output an error", () => {
    assertEqual(fromString(bool, "2"), new FSharpResult$2(1, ["Error at: `$`\nExpecting a boolean but instead got: 2"]));
}), Test_testCase("an int works", () => {
    assertEqual(fromString(int, "25"), new FSharpResult$2(0, [25]));
}), Test_testCase("an invalid int [invalid range: too big] output an error", () => {
    assertEqual(fromString(int, "2147483648"), new FSharpResult$2(1, ["Error at: `$`\nExpecting an int but instead got: 2147483648\nReason: Value was either too large or too small for an int"]));
}), Test_testCase("an invalid int [invalid range: too small] output an error", () => {
    assertEqual(fromString(int, "-2147483649"), new FSharpResult$2(1, ["Error at: `$`\nExpecting an int but instead got: -2147483649\nReason: Value was either too large or too small for an int"]));
}), Test_testCase("an int16 works from number", () => {
    const expected_38 = new FSharpResult$2(0, [(25 + 0x8000 & 0xFFFF) - 0x8000]);
    assertEqual(fromString(int16, "25"), expected_38);
}), Test_testCase("an int16 works from string", () => {
    const expected_40 = new FSharpResult$2(0, [(-25 + 0x8000 & 0xFFFF) - 0x8000]);
    assertEqual(fromString(int16, "\"-25\""), expected_40);
}), Test_testCase("an int16 output an error if value is too big", () => {
    const expected_42 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an int16 but instead got: 32768\nReason: Value was either too large or too small for an int16\n                        ".trim()]);
    assertEqual(fromString(int16, "32768"), expected_42);
}), Test_testCase("an int16 output an error if value is too small", () => {
    const expected_44 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an int16 but instead got: -32769\nReason: Value was either too large or too small for an int16\n                        ".trim()]);
    assertEqual(fromString(int16, "-32769"), expected_44);
}), Test_testCase("an int16 output an error if incorrect string", () => {
    const expected_46 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an int16 but instead got: \"maxime\"\n                        ".trim()]);
    assertEqual(fromString(int16, "\"maxime\""), expected_46);
}), Test_testCase("an uint16 works from number", () => {
    const expected_48 = new FSharpResult$2(0, [25 & 0xFFFF]);
    assertEqual(fromString(uint16, "25"), expected_48);
}), Test_testCase("an uint16 works from string", () => {
    const expected_50 = new FSharpResult$2(0, [25 & 0xFFFF]);
    assertEqual(fromString(uint16, "\"25\""), expected_50);
}), Test_testCase("an uint16 output an error if value is too big", () => {
    const expected_52 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an uint16 but instead got: 65536\nReason: Value was either too large or too small for an uint16\n                        ".trim()]);
    assertEqual(fromString(uint16, "65536"), expected_52);
}), Test_testCase("an uint16 output an error if value is too small", () => {
    const expected_54 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an uint16 but instead got: -1\nReason: Value was either too large or too small for an uint16\n                        ".trim()]);
    assertEqual(fromString(uint16, "-1"), expected_54);
}), Test_testCase("an uint16 output an error if incorrect string", () => {
    const expected_56 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an uint16 but instead got: \"maxime\"\n                        ".trim()]);
    assertEqual(fromString(uint16, "\"maxime\""), expected_56);
}), Test_testCase("an int64 works from number", () => {
    assertEqual(fromString(int64, "1000"), new FSharpResult$2(0, [1000n]));
}), Test_testCase("an int64 works from string", () => {
    assertEqual(fromString(int64, "\"99\""), new FSharpResult$2(0, [99n]));
}), Test_testCase("an int64 works output an error if incorrect string", () => {
    const expected_62 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an int64 but instead got: \"maxime\"\n                        ".trim()]);
    assertEqual(fromString(int64, "\"maxime\""), expected_62);
}), Test_testCase("an uint32 works from number", () => {
    assertEqual(fromString(uint32, "1000"), new FSharpResult$2(0, [1000]));
}), Test_testCase("an uint32 works from string", () => {
    assertEqual(fromString(uint32, "\"1000\""), new FSharpResult$2(0, [1000]));
}), Test_testCase("an uint32 output an error if incorrect string", () => {
    const expected_68 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an uint32 but instead got: \"maxime\"\n                        ".trim()]);
    assertEqual(fromString(uint32, "\"maxime\""), expected_68);
}), Test_testCase("an uint64 works from number", () => {
    assertEqual(fromString(uint64, "1000"), new FSharpResult$2(0, [1000n]));
}), Test_testCase("an uint64 works from string", () => {
    assertEqual(fromString(uint64, "\"1000\""), new FSharpResult$2(0, [1000n]));
}), Test_testCase("an uint64 output an error if incorrect string", () => {
    const expected_74 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an uint64 but instead got: \"maxime\"\n                        ".trim()]);
    assertEqual(fromString(uint64, "\"maxime\""), expected_74);
}), Test_testCase("a byte works from number", () => {
    assertEqual(fromString(byte, "25"), new FSharpResult$2(0, [25]));
}), Test_testCase("a byte works from string", () => {
    assertEqual(fromString(byte, "\"25\""), new FSharpResult$2(0, [25]));
}), Test_testCase("a byte output an error if value is too big", () => {
    const expected_80 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a byte but instead got: 256\nReason: Value was either too large or too small for a byte\n                        ".trim()]);
    assertEqual(fromString(byte, "256"), expected_80);
}), Test_testCase("a byte output an error if value is too small", () => {
    const expected_82 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a byte but instead got: -1\nReason: Value was either too large or too small for a byte\n                        ".trim()]);
    assertEqual(fromString(byte, "-1"), expected_82);
}), Test_testCase("a byte output an error if incorrect string", () => {
    const expected_84 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a byte but instead got: \"maxime\"\n                        ".trim()]);
    assertEqual(fromString(byte, "\"maxime\""), expected_84);
}), Test_testCase("a sbyte works from number", () => {
    assertEqual(fromString(sbyte, "25"), new FSharpResult$2(0, [25]));
}), Test_testCase("a sbyte works from string", () => {
    assertEqual(fromString(sbyte, "\"-25\""), new FSharpResult$2(0, [-25]));
}), Test_testCase("a sbyte output an error if value is too big", () => {
    const expected_90 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a sbyte but instead got: 128\nReason: Value was either too large or too small for a sbyte\n                        ".trim()]);
    assertEqual(fromString(sbyte, "128"), expected_90);
}), Test_testCase("a sbyte output an error if value is too small", () => {
    const expected_92 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a sbyte but instead got: -129\nReason: Value was either too large or too small for a sbyte\n                        ".trim()]);
    assertEqual(fromString(sbyte, "-129"), expected_92);
}), Test_testCase("a sbyte output an error if incorrect string", () => {
    const expected_94 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a sbyte but instead got: \"maxime\"\n                        ".trim()]);
    assertEqual(fromString(sbyte, "\"maxime\""), expected_94);
}), Test_testCase("an bigint works from number", () => {
    const expected_96 = new FSharpResult$2(0, [fromInt32(12)]);
    assertEqual(fromString(bigint, "12"), expected_96);
}), Test_testCase("an bigint works from string", () => {
    const expected_98 = new FSharpResult$2(0, [fromInt32(12)]);
    assertEqual(fromString(bigint, "\"12\""), expected_98);
}), Test_testCase("an bigint output an error if invalid string", () => {
    const expected_100 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a bigint but instead got: \"maxime\"\n                        ".trim()]);
    assertEqual(fromString(bigint, "\"maxime\""), expected_100);
}), Test_testCase("a string representing a DateTime should be accepted as a string", () => {
    assertEqual(fromString(string, "\"2018-10-01T11:12:55.00Z\""), new FSharpResult$2(0, ["2018-10-01T11:12:55.00Z"]));
}), Test_testCase("a datetime works", () => {
    const expected_104 = create(2018, 10, 1, 11, 12, 55, 0, 1);
    assertEqual(fromString(datetimeUtc, "\"2018-10-01T11:12:55.00Z\""), new FSharpResult$2(0, [expected_104]));
}), Test_testCase("a non-UTC datetime works", () => {
    const expected_106 = create(2018, 10, 1, 11, 12, 55);
    assertEqual(fromString(datetimeLocal, "\"2018-10-01T11:12:55\""), new FSharpResult$2(0, [expected_106]));
}), Test_testCase("a datetime output an error if invalid string", () => {
    const expected_108 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a datetime but instead got: \"invalid_string\"\n                        ".trim()]);
    assertEqual(fromString(datetimeUtc, "\"invalid_string\""), expected_108);
}), Test_testCase("a datetime works with TimeZone", () => {
    let arg;
    const localDate = create(2018, 10, 1, 11, 12, 55, 0, 2);
    const expected_110 = new FSharpResult$2(0, [toUniversalTime(localDate)]);
    assertEqual(fromString(datetimeUtc, (arg = toString(localDate, "O"), toText(printf("\"%s\""))(arg))), expected_110);
}), Test_testCase("a datetimeOffset works", () => {
    const expected_112 = new FSharpResult$2(0, [create_1(2018, 7, 2, 12, 23, 45, 0, fromHours(2))]);
    assertEqual(fromString(datetimeOffset, "\"2018-07-02T12:23:45+02:00\""), expected_112);
}), Test_testCase("a datetimeOffset returns Error if invalid format", () => {
    const expected_114 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a datetimeoffset but instead got: \"NOT A DATETIMEOFFSET\"\n                        ".trim()]);
    assertEqual(fromString(datetimeOffset, "\"NOT A DATETIMEOFFSET\""), expected_114);
}), Test_testCase("a timespan works", () => {
    const expected_116 = new FSharpResult$2(0, [create_2(23, 45, 0)]);
    assertEqual(fromString(timespan, "\"23:45:00\""), expected_116);
}), Test_testCase("a timespan returns Error if invalid format", () => {
    const expected_118 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting a timespan but instead got: \"NOT A TimeSpan\"\n                        ".trim()]);
    assertEqual(fromString(timespan, "\"NOT A TimeSpan\""), expected_118);
}), Test_testCase("an enum<sbyte> works", () => {
    assertEqual(fromString(andThen(succeed, sbyte_1), "99"), new FSharpResult$2(0, [99]));
}), Test_testCase("an enum<byte> works", () => {
    assertEqual(fromString(andThen(succeed, byte_1), "99"), new FSharpResult$2(0, [99]));
}), Test_testCase("an enum<int> works", () => {
    assertEqual(fromString(andThen(succeed, int_1), "1"), new FSharpResult$2(0, [1]));
}), Test_testCase("an enum<uint32> works", () => {
    assertEqual(fromString(andThen(succeed, uint32_1), "99"), new FSharpResult$2(0, [99]));
}), Test_testCase("an enum<int16> works", () => {
    assertEqual(fromString(andThen(succeed, int16_1), "99"), new FSharpResult$2(0, [99]));
}), Test_testCase("an enum<uint16> works", () => {
    assertEqual(fromString(andThen(succeed, uint16_1), "99"), new FSharpResult$2(0, [99]));
})])), Test_testList("Tuples", ofArray([Test_testCase("tuple2 works", () => {
    assertEqual(fromString(tuple2(int, string), "[1, \"maxime\"]"), new FSharpResult$2(0, [[1, "maxime"]]));
}), Test_testCase("tuple3 works", () => {
    assertEqual(fromString(tuple3(int, string, float), "[1, \"maxime\", 2.5]"), new FSharpResult$2(0, [[1, "maxime", 2.5]]));
}), Test_testCase("tuple4 works", () => {
    const expected_136 = new FSharpResult$2(0, [[1, "maxime", 2.5, new SmallRecord("test")]]);
    assertEqual(fromString(tuple4(int, string, float, SmallRecord_get_Decoder()), "[1, \"maxime\", 2.5, { \"fieldA\" : \"test\" }]"), expected_136);
}), Test_testCase("tuple5 works", () => {
    const expected_138 = new FSharpResult$2(0, [[1, "maxime", 2.5, new SmallRecord("test"), false]]);
    assertEqual(fromString(tuple5(int, string, float, SmallRecord_get_Decoder(), bool), "[1, \"maxime\", 2.5, { \"fieldA\" : \"test\" }, false]"), expected_138);
}), Test_testCase("tuple6 works", () => {
    const expected_140 = new FSharpResult$2(0, [[1, "maxime", 2.5, new SmallRecord("test"), false, defaultOf()]]);
    assertEqual(fromString(tuple6(int, string, float, SmallRecord_get_Decoder(), bool, nil(defaultOf())), "[1, \"maxime\", 2.5, { \"fieldA\" : \"test\" }, false, null]"), expected_140);
}), Test_testCase("tuple7 works", () => {
    const expected_142 = new FSharpResult$2(0, [[1, "maxime", 2.5, new SmallRecord("test"), false, defaultOf(), 56]]);
    assertEqual(fromString(tuple7(int, string, float, SmallRecord_get_Decoder(), bool, nil(defaultOf()), int), "[1, \"maxime\", 2.5, { \"fieldA\" : \"test\" }, false, null, 56]"), expected_142);
}), Test_testCase("tuple8 works", () => {
    const expected_144 = new FSharpResult$2(0, [[1, "maxime", 2.5, new SmallRecord("test"), false, defaultOf(), true, 98]]);
    assertEqual(fromString(tuple8(int, string, float, SmallRecord_get_Decoder(), bool, nil(defaultOf()), bool, int), "[1, \"maxime\", 2.5, { \"fieldA\" : \"test\" }, false, null, true, 98]"), expected_144);
}), Test_testCase("tuple2 returns an error if invalid json", () => {
    const expected_146 = new FSharpResult$2(1, ["\nError at: `$.[1]`\nExpecting a string but instead got: false\n                        ".trim()]);
    assertEqual(fromString(tuple2(int, string), "[1, false, \"unused value\"]"), expected_146);
}), Test_testCase("tuple3 returns an error if invalid json", () => {
    const expected_148 = new FSharpResult$2(1, ["\nError at: `$.[2]`\nExpecting a float but instead got: false\n                        ".trim()]);
    assertEqual(fromString(tuple3(int, string, float), "[1, \"maxime\", false]"), expected_148);
}), Test_testCase("tuple4 returns an error if invalid json (missing index)", () => {
    const expected_150 = new FSharpResult$2(1, ["\nError at: `$.[3]`\nExpecting a longer array. Need index `3` but there are only `3` entries.\n[\n    1,\n    \"maxime\",\n    2.5\n]\n                        ".trim()]);
    assertEqual(fromString(tuple4(int, string, float, SmallRecord_get_Decoder()), "[1, \"maxime\", 2.5]"), expected_150);
}), Test_testCase("tuple4 returns an error if invalid json (error in the nested object)", () => {
    const expected_152 = new FSharpResult$2(1, ["\nError at: `$.[3].fieldA`\nExpecting a string but instead got: false\n                        ".trim()]);
    assertEqual(fromString(tuple4(int, string, float, SmallRecord_get_Decoder()), "[1, \"maxime\", 2.5, { \"fieldA\" : false }]"), expected_152);
}), Test_testCase("tuple5 returns an error if invalid json", () => {
    const expected_154 = new FSharpResult$2(1, ["\nError at: `$.[4]`\nExpecting a datetime but instead got: false\n                        ".trim()]);
    assertEqual(fromString(tuple5(int, string, float, SmallRecord_get_Decoder(), datetimeUtc), "[1, \"maxime\", 2.5, { \"fieldA\" : \"test\" }, false]"), expected_154);
}), Test_testCase("tuple6 returns an error if invalid json", () => {
    const expected_156 = new FSharpResult$2(1, ["\nError at: `$.[5]`\nExpecting null but instead got: false\n                        ".trim()]);
    assertEqual(fromString(tuple6(int, string, float, SmallRecord_get_Decoder(), datetimeUtc, nil(defaultOf())), "[1, \"maxime\", 2.5, { \"fieldA\" : \"test\" }, \"2018-10-01T11:12:55.00Z\", false]"), expected_156);
}), Test_testCase("tuple7 returns an error if invalid json", () => {
    const expected_158 = new FSharpResult$2(1, ["\nError at: `$.[6]`\nExpecting an int but instead got: false\n                        ".trim()]);
    assertEqual(fromString(tuple7(int, string, float, SmallRecord_get_Decoder(), datetimeUtc, nil(defaultOf()), int), "[1, \"maxime\", 2.5, { \"fieldA\" : \"test\" }, \"2018-10-01T11:12:55.00Z\", null, false]"), expected_158);
}), Test_testCase("tuple8 returns an error if invalid json", () => {
    const expected_160 = new FSharpResult$2(1, ["\nError at: `$.[7]`\nExpecting an int but instead got: \"maxime\"\n                        ".trim()]);
    assertEqual(fromString(tuple8(int, string, float, SmallRecord_get_Decoder(), datetimeUtc, nil(defaultOf()), int, int), "[1, \"maxime\", 2.5, { \"fieldA\" : \"test\" }, \"2018-10-01T11:12:55.00Z\", null, 56, \"maxime\"]"), expected_160);
})])), Test_testList("Object primitives", ofArray([Test_testCase("field works", () => {
    assertEqual(fromString(field("name", string), "{ \"name\": \"maxime\", \"age\": 25 }"), new FSharpResult$2(0, ["maxime"]));
}), Test_testCase("field output an error explaining why the value is considered invalid", () => {
    const expected_164 = new FSharpResult$2(1, ["\nError at: `$.name`\nExpecting an int but instead got: null\n                        ".trim()]);
    assertEqual(fromString(field("name", int), "{ \"name\": null, \"age\": 25 }"), expected_164);
}), Test_testCase("field output an error when field is missing", () => {
    const expected_166 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an object with a field named `height` but instead got:\n{\n    \"name\": \"maxime\",\n    \"age\": 25\n}\n                        ".trim()]);
    assertEqual(fromString(field("height", float), "{ \"name\": \"maxime\", \"age\": 25 }"), expected_166);
}), Test_testCase("at works", () => {
    assertEqual(fromString(at(ofArray(["user", "name"]), string), "{ \"user\": { \"name\": \"maxime\", \"age\": 25 } }"), new FSharpResult$2(0, ["maxime"]));
}), Test_testCase("at output an error if the path failed", () => {
    const expected_170 = new FSharpResult$2(1, ["\nError at: `$.user.firstname`\nExpecting an object with path `user.firstname` but instead got:\n{\n    \"user\": {\n        \"name\": \"maxime\",\n        \"age\": 25\n    }\n}\nNode `firstname` is unkown.\n                        ".trim()]);
    assertEqual(fromString(at(ofArray(["user", "firstname"]), string), "{ \"user\": { \"name\": \"maxime\", \"age\": 25 } }"), expected_170);
}), Test_testCase("at output an error explaining why the value is considered invalid", () => {
    const expected_172 = new FSharpResult$2(1, ["\nError at: `$.name`\nExpecting an int but instead got: null\n                        ".trim()]);
    assertEqual(fromString(at(singleton("name"), int), "{ \"name\": null, \"age\": 25 }"), expected_172);
}), Test_testCase("index works", () => {
    assertEqual(fromString(index(1, string), "[\"maxime\", \"alfonso\", \"steffen\"]"), new FSharpResult$2(0, ["alfonso"]));
}), Test_testCase("index output an error if array is to small", () => {
    const expected_176 = new FSharpResult$2(1, ["\nError at: `$.[5]`\nExpecting a longer array. Need index `5` but there are only `3` entries.\n[\n    \"maxime\",\n    \"alfonso\",\n    \"steffen\"\n]\n                        ".trim()]);
    assertEqual(fromString(index(5, string), "[\"maxime\", \"alfonso\", \"steffen\"]"), expected_176);
}), Test_testCase("index output an error if value isn\'t an array", () => {
    const expected_178 = new FSharpResult$2(1, ["\nError at: `$.[5]`\nExpecting an array but instead got: 1\n                        ".trim()]);
    assertEqual(fromString(index(5, string), "1"), expected_178);
})])), Test_testList("Data structure", ofArray([Test_testCase("list works", () => {
    assertEqual(fromString(list_3(int), "[1, 2, 3]"), new FSharpResult$2(0, [ofArray([1, 2, 3])]));
}), Test_testCase("an invalid list output an error", () => {
    assertEqual(fromString(list_3(int), "1"), new FSharpResult$2(1, ["Error at: `$`\nExpecting a list but instead got: 1"]));
}), Test_testCase("array works", () => {
    const expected_184 = new FSharpResult$2(0, [[1, 2, 3]]);
    assertEqual(fromString(array(int), "[1, 2, 3]"), expected_184);
}), Test_testCase("an invalid array output an error", () => {
    assertEqual(fromString(array(int), "1"), new FSharpResult$2(1, ["Error at: `$`\nExpecting an array but instead got: 1"]));
}), Test_testCase("keys works", () => {
    assertEqual(fromString(keys_1, "{ \"a\": 1, \"b\": 2, \"c\": 3 }"), new FSharpResult$2(0, [ofArray(["a", "b", "c"])]));
}), Test_testCase("keys returns an error for invalid objects", () => {
    assertEqual(fromString(keys_1, "1"), new FSharpResult$2(1, ["Error at: `$`\nExpecting an object but instead got: 1"]));
}), Test_testCase("keyValuePairs works", () => {
    assertEqual(fromString(keyValuePairs(int), "{ \"a\": 1, \"b\": 2, \"c\": 3 }"), new FSharpResult$2(0, [ofArray([["a", 1], ["b", 2], ["c", 3]])]));
}), Test_testCase("dict works", () => {
    const expected_194 = new FSharpResult$2(0, [ofList(ofArray([["a", 1], ["b", 2], ["c", 3]]), {
        Compare: comparePrimitives,
    })]);
    assertEqual(fromString(dict(int), "{ \"a\": 1, \"b\": 2, \"c\": 3 }"), expected_194);
}), Test_testCase("dict with custom decoder works", () => {
    const expected_196 = new FSharpResult$2(0, [ofList(ofArray([["a", Record2_Create(1, 1)], ["b", Record2_Create(2, 2)], ["c", Record2_Create(3, 3)]]), {
        Compare: comparePrimitives,
    })]);
    assertEqual(fromString(dict(map2(Record2_Create, field("a", float), field("b", float))), "\n{\n    \"a\":\n        {\n            \"a\": 1.0,\n            \"b\": 1.0\n        },\n    \"b\":\n        {\n            \"a\": 2.0,\n            \"b\": 2.0\n        },\n    \"c\":\n        {\n            \"a\": 3.0,\n            \"b\": 3.0\n        }\n}\n                        "), expected_196);
}), Test_testCase("an invalid dict output an error", () => {
    assertEqual(fromString(dict(int), "1"), new FSharpResult$2(1, ["Error at: `$`\nExpecting an object but instead got: 1"]));
}), Test_testCase("map\' works", () => {
    const expected_200 = new FSharpResult$2(0, [ofList(ofArray([[1, "x"], [2, "y"], [3, "z"]]), {
        Compare: comparePrimitives,
    })]);
    assertEqual(fromString(map$0027(int, string), "[ [ 1, \"x\" ], [ 2, \"y\" ], [ 3, \"z\" ] ]"), expected_200);
}), Test_testCase("map\' with custom key decoder works", () => {
    const expected_202 = new FSharpResult$2(0, [ofList(ofArray([[[1, 6], "a"], [[2, 7], "b"], [[3, 8], "c"]]), {
        Compare: compareArrays,
    })]);
    assertEqual(fromString(map$0027(map2((x_4, y_4) => [x_4, y_4], field("x", int), field("y", int)), string), "\n[\n    [\n        {\n            \"x\": 1,\n            \"y\": 6\n        },\n        \"a\"\n    ],\n    [\n        {\n            \"x\": 2,\n            \"y\": 7\n        },\n        \"b\"\n    ],\n    [\n        {\n            \"x\": 3,\n            \"y\": 8\n        },\n        \"c\"\n    ]\n]\n                        "), expected_202);
})])), Test_testList("Inconsistent structure", ofArray([Test_testCase("oneOf works", () => {
    assertEqual(fromString(list_3(oneOf(ofArray([int, nil(0)]))), "[1,2,null,4]"), new FSharpResult$2(0, [ofArray([1, 2, 0, 4])]));
}), Test_testCase("oneOf works in combination with object builders", () => {
    const expected_206 = new FSharpResult$2(0, [new FSharpChoice$2(1, [new SmallRecord("maxime")])]);
    const decoder1 = object((get$) => {
        let objectArg;
        return new SmallRecord((objectArg = get$.Required, objectArg.Field("name", string)));
    });
    assertEqual(fromString(oneOf(ofArray([map((Item) => (new FSharpChoice$2(0, [Item])), field("Foo", decoder1)), map((Item_1) => (new FSharpChoice$2(1, [Item_1])), field("Bar", decoder1))])), "{ \"Bar\": { \"name\": \"maxime\", \"age\": 25 } }"), expected_206);
}), Test_testCase("oneOf works with optional", () => {
    const decoder_7 = oneOf(ofArray([map((Item_2) => (new Price(0, [Item_2])), field("Normal", float)), map((Item_3) => (new Price(1, [Item_3])), field("Reduced", option_3(float))), map((_arg_108) => (new Price(2, [])), field("Zero", bool))]));
    assertEqual(fromString(decoder_7, "{\"Normal\": 4.5}"), new FSharpResult$2(0, [new Price(0, [4.5])]));
    assertEqual(fromString(decoder_7, "{\"Reduced\": 4.5}"), new FSharpResult$2(0, [new Price(1, [4.5])]));
    assertEqual(fromString(decoder_7, "{\"Reduced\": null}"), new FSharpResult$2(0, [new Price(1, [void 0])]));
    assertEqual(fromString(decoder_7, "{\"Zero\": true}"), new FSharpResult$2(0, [new Price(2, [])]));
}), Test_testCase("oneOf output errors if all case fails", () => {
    const expected_216 = new FSharpResult$2(1, ["\nThe following errors were found:\n\nError at: `$.[0]`\nExpecting a string but instead got: 1\n\nError at: `$.[0]`\nExpecting an object but instead got:\n1\n                        ".trim()]);
    assertEqual(fromString(list_3(oneOf(ofArray([string, field("test", string)]))), "[1,2,null,4]"), expected_216);
}), Test_testCase("optional works", () => {
    assertEqual(fromString(optional("name", string), "{ \"name\": \"maxime\", \"age\": 25, \"something_undefined\": null }"), new FSharpResult$2(0, ["maxime"]));
    const matchValue_1 = fromString(optional("name", int), "{ \"name\": \"maxime\", \"age\": 25, \"something_undefined\": null }");
    if (matchValue_1.tag === 0) {
        throw new Error("Expected type error for `name` field");
    }
    assertEqual(fromString(optional("height", int), "{ \"name\": \"maxime\", \"age\": 25, \"something_undefined\": null }"), new FSharpResult$2(0, [void 0]));
    assertEqual(fromString(optional("something_undefined", string), "{ \"name\": \"maxime\", \"age\": 25, \"something_undefined\": null }"), new FSharpResult$2(0, [void 0]));
}), Test_testCase("optional returns Error value if decoder fails", () => {
    const expected_221 = new FSharpResult$2(1, ["\nError at: `$.name`\nExpecting a string but instead got: 12\n                        ".trim()]);
    assertEqual(fromString(optional("name", string), "{ \"name\": 12, \"age\": 25 }"), expected_221);
}), Test_testCase("optionalAt works", () => {
    assertEqual(fromString(optionalAt(ofArray(["data", "name"]), string), "{ \"data\" : { \"name\": \"maxime\", \"age\": 25, \"something_undefined\": null } }"), new FSharpResult$2(0, ["maxime"]));
    const matchValue_2 = fromString(optionalAt(ofArray(["data", "name"]), int), "{ \"data\" : { \"name\": \"maxime\", \"age\": 25, \"something_undefined\": null } }");
    if (matchValue_2.tag === 0) {
        throw new Error("Expected type error for `name` field");
    }
    assertEqual(fromString(optionalAt(ofArray(["data", "height"]), int), "{ \"data\" : { \"name\": \"maxime\", \"age\": 25, \"something_undefined\": null } }"), new FSharpResult$2(0, [void 0]));
    assertEqual(fromString(optionalAt(ofArray(["data", "something_undefined"]), string), "{ \"data\" : { \"name\": \"maxime\", \"age\": 25, \"something_undefined\": null } }"), new FSharpResult$2(0, [void 0]));
    assertEqual(fromString(optionalAt(ofArray(["data", "something_undefined", "name"]), string), "{ \"data\" : { \"name\": \"maxime\", \"age\": 25, \"something_undefined\": null } }"), new FSharpResult$2(0, [void 0]));
}), Test_testCase("combining field and option decoders works", () => {
    assertEqual(fromString(field("name", option_3(string)), "{ \"name\": \"maxime\", \"age\": 25, \"something_undefined\": null }"), new FSharpResult$2(0, ["maxime"]));
    const matchValue_3 = fromString(field("name", option_3(int)), "{ \"name\": \"maxime\", \"age\": 25, \"something_undefined\": null }");
    if (matchValue_3.tag === 0) {
        throw new Error("Expected type error for `name` field #1");
    }
    else {
        assertEqual(matchValue_3.fields[0], "\nError at: `$.name`\nExpecting an int but instead got: \"maxime\"\n                        ".trim());
    }
    const matchValue_4 = fromString(field("this_field_do_not_exist", option_3(int)), "{ \"name\": \"maxime\", \"age\": 25, \"something_undefined\": null }");
    if (matchValue_4.tag === 0) {
        throw new Error("Expected type error for `name` field #2");
    }
    else {
        assertEqual(matchValue_4.fields[0], "\nError at: `$`\nExpecting an object with a field named `this_field_do_not_exist` but instead got:\n{\n    \"name\": \"maxime\",\n    \"age\": 25,\n    \"something_undefined\": null\n}\n                        ".trim());
    }
    const matchValue_5 = fromString(field("something_undefined", option_3(int)), "{ \"name\": \"maxime\", \"age\": 25, \"something_undefined\": null }");
    if (matchValue_5.tag === 0) {
        assertEqual(matchValue_5.fields[0], void 0);
    }
    else {
        throw new Error("`Decode.field \"something_undefined\" (Decode.option Decode.int)` test should pass");
    }
    assertEqual(fromString(option_3(field("name", string)), "{ \"name\": \"maxime\", \"age\": 25, \"something_undefined\": null }"), new FSharpResult$2(0, ["maxime"]));
    const matchValue_6 = fromString(option_3(field("name", int)), "{ \"name\": \"maxime\", \"age\": 25, \"something_undefined\": null }");
    if (matchValue_6.tag === 0) {
        throw new Error("Expected type error for `name` field #3");
    }
    else {
        assertEqual(matchValue_6.fields[0], "\nError at: `$.name`\nExpecting an int but instead got: \"maxime\"\n                        ".trim());
    }
    const matchValue_7 = fromString(option_3(field("this_field_do_not_exist", int)), "{ \"name\": \"maxime\", \"age\": 25, \"something_undefined\": null }");
    if (matchValue_7.tag === 0) {
        throw new Error("Expected type error for `name` field #4");
    }
    else {
        assertEqual(matchValue_7.fields[0], "\nError at: `$`\nExpecting an object with a field named `this_field_do_not_exist` but instead got:\n{\n    \"name\": \"maxime\",\n    \"age\": 25,\n    \"something_undefined\": null\n}\n                        ".trim());
    }
    const matchValue_8 = fromString(option_3(field("something_undefined", int)), "{ \"name\": \"maxime\", \"age\": 25, \"something_undefined\": null }");
    if (matchValue_8.tag === 0) {
        throw new Error("Expected type error for `name` field");
    }
    else {
        assertEqual(matchValue_8.fields[0], "\nError at: `$.something_undefined`\nExpecting an int but instead got: null\n                        ".trim());
    }
    const matchValue_9 = fromString(field("height", option_3(int)), "{ \"name\": \"maxime\", \"age\": 25, \"something_undefined\": null }");
    if (matchValue_9.tag === 0) {
        throw new Error("Expected type error for `height` field");
    }
    else {
        assertEqual(matchValue_9.fields[0], "\nError at: `$`\nExpecting an object with a field named `height` but instead got:\n{\n    \"name\": \"maxime\",\n    \"age\": 25,\n    \"something_undefined\": null\n}\n                        ".trim());
    }
    assertEqual(fromString(field("something_undefined", option_3(string)), "{ \"name\": \"maxime\", \"age\": 25, \"something_undefined\": null }"), new FSharpResult$2(0, [void 0]));
})])), Test_testList("Fancy decoding", ofArray([Test_testCase("null works (test on an int)", () => {
    assertEqual(fromString(nil(20), "null"), new FSharpResult$2(0, [20]));
}), Test_testCase("null works (test on a boolean)", () => {
    assertEqual(fromString(nil(false), "null"), new FSharpResult$2(0, [false]));
}), Test_testCase("succeed works", () => {
    assertEqual(fromString(succeed_1(7), "true"), new FSharpResult$2(0, [7]));
}), Test_testCase("succeed output an error if the JSON is invalid", () => {
    assertEqual(fromString(succeed_1(7), "maxime"), new FSharpResult$2(1, ["Given an invalid JSON: Unexpected token \'m\', \"maxime\" is not valid JSON"]));
}), Test_testCase("fail works", () => {
    const expected_251 = new FSharpResult$2(1, ["Error at: `$`\nThe following `failure` occurred with the decoder: Failing because it\'s fun"]);
    assertEqual(fromString(fail("Failing because it\'s fun"), "true"), expected_251);
}), Test_testCase("andMap works for any arity", () => {
    assertEqual(fromString(andMap()(field("k", int))(andMap()(field("j", int))(andMap()(field("i", int))(andMap()(field("h", int))(andMap()(field("g", int))(andMap()(field("f", int))(andMap()(field("e", int))(andMap()(field("d", int))(andMap()(field("c", int))(andMap()(field("b", int))(andMap()(field("a", int))(succeed_1((a_5) => ((b_5) => ((c) => ((d_5) => ((e) => ((f) => ((g) => ((h) => ((i) => ((j) => ((k) => Record10_Create(a_5, b_5, c, d_5, e, f, g, h, i, j, k))))))))))))))))))))))), "{\"a\": 1,\"b\": 2,\"c\": 3,\"d\": 4,\"e\": 5,\"f\": 6,\"g\": 7,\"h\": 8,\"i\": 9,\"j\": 10,\"k\": 11}"), new FSharpResult$2(0, [new Record10(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11)]));
}), Test_testCase("andThen works", () => {
    assertEqual(fromString(andThen_1((version) => {
        switch (version) {
            case 3:
                return succeed_1(1);
            case 4:
                return succeed_1(1);
            default:
                return fail(("Trying to decode info, but version " + int32ToString(version)) + "is not supported");
        }
    }, field("version", int)), "{ \"version\": 3, \"data\": 2 }"), new FSharpResult$2(0, [1]));
}), Test_testCase("andThen generate an error if an error occuered", () => {
    const expected_257 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an object with a field named `version` but instead got:\n{\n    \"info\": 3,\n    \"data\": 2\n}\n                        ".trim()]);
    assertEqual(fromString(andThen_1((version_1) => {
        switch (version_1) {
            case 3:
                return succeed_1(1);
            case 4:
                return succeed_1(1);
            default:
                return fail(("Trying to decode info, but version " + int32ToString(version_1)) + "is not supported");
        }
    }, field("version", int)), "{ \"info\": 3, \"data\": 2 }"), expected_257);
}), Test_testCase("all works", () => {
    assertEqual(fromString(all(ofArray([succeed_1(1), succeed_1(2), succeed_1(3)])), "{}"), new FSharpResult$2(0, [ofArray([1, 2, 3])]));
}), Test_testCase("combining Decode.all and Decode.keys works", () => {
    assertEqual(fromString(andThen_1((keys) => all(map_1((key) => field(key, int), List_except(["special_property"], keys, {
        Equals: (x_5, y_5) => (x_5 === y_5),
        GetHashCode: stringHash,
    }))), keys_1), "{ \"a\": 1, \"b\": 2, \"c\": 3 }"), new FSharpResult$2(0, [ofArray([1, 2, 3])]));
}), Test_testCase("all succeeds on empty lists", () => {
    assertEqual(fromString(all(empty()), "{}"), new FSharpResult$2(0, [empty()]));
}), Test_testCase("all fails when one decoder fails", () => {
    assertEqual(fromString(all(ofArray([succeed_1(1), int, succeed_1(3)])), "{}"), new FSharpResult$2(1, ["Error at: `$`\nExpecting an int but instead got: {}"]));
})])), Test_testList("Mapping", ofArray([Test_testCase("map works", () => {
    assertEqual(fromString(map((str) => str.length, string), "\"maxime\""), new FSharpResult$2(0, [6]));
}), Test_testCase("map2 works", () => {
    const expected_269 = new FSharpResult$2(0, [new Record2(1, 2)]);
    assertEqual(fromString(map2(Record2_Create, field("a", float), field("b", float)), jsonRecord), expected_269);
}), Test_testCase("map3 works", () => {
    const expected_271 = new FSharpResult$2(0, [new Record3(1, 2, 3)]);
    assertEqual(fromString(map3(Record3_Create, field("a", float), field("b", float), field("c", float)), jsonRecord), expected_271);
}), Test_testCase("map4 works", () => {
    const expected_273 = new FSharpResult$2(0, [new Record4(1, 2, 3, 4)]);
    assertEqual(fromString(map4(Record4_Create, field("a", float), field("b", float), field("c", float), field("d", float)), jsonRecord), expected_273);
}), Test_testCase("map5 works", () => {
    const expected_275 = new FSharpResult$2(0, [new Record5(1, 2, 3, 4, 5)]);
    assertEqual(fromString(map5(Record5_Create, field("a", float), field("b", float), field("c", float), field("d", float), field("e", float)), jsonRecord), expected_275);
}), Test_testCase("map6 works", () => {
    const expected_277 = new FSharpResult$2(0, [new Record6(1, 2, 3, 4, 5, 6)]);
    assertEqual(fromString(map6(Record6_Create, field("a", float), field("b", float), field("c", float), field("d", float), field("e", float), field("f", float)), jsonRecord), expected_277);
}), Test_testCase("map7 works", () => {
    const expected_279 = new FSharpResult$2(0, [new Record7(1, 2, 3, 4, 5, 6, 7)]);
    assertEqual(fromString(map7(Record7_Create, field("a", float), field("b", float), field("c", float), field("d", float), field("e", float), field("f", float), field("g", float)), jsonRecord), expected_279);
}), Test_testCase("map8 works", () => {
    const expected_281 = new FSharpResult$2(0, [new Record8(1, 2, 3, 4, 5, 6, 7, 8)]);
    assertEqual(fromString(map8(Record8_Create, field("a", float), field("b", float), field("c", float), field("d", float), field("e", float), field("f", float), field("g", float), field("h", float)), jsonRecord), expected_281);
}), Test_testCase("map2 generate an error if invalid", () => {
    assertEqual(fromString(map2(Record2_Create, field("a", float), field("b", float)), jsonRecordInvalid), new FSharpResult$2(1, ["Error at: `$.a`\nExpecting a float but instead got: \"invalid_a_field\""]));
})])), Test_testList("object builder", ofArray([Test_testCase("get.Required.Field works", () => {
    const expected_285 = new FSharpResult$2(0, [new SmallRecord("maxime")]);
    assertEqual(fromString(object((get$_1) => {
        let objectArg_1;
        return new SmallRecord((objectArg_1 = get$_1.Required, objectArg_1.Field("name", string)));
    }), "{ \"name\": \"maxime\", \"age\": 25 }"), expected_285);
}), Test_testCase("get.Required.Field returns Error if field is missing", () => {
    const expected_287 = new FSharpResult$2(1, ["\nError at: `$`\nExpecting an object with a field named `name` but instead got:\n{\n    \"age\": 25\n}\n                        ".trim()]);
    assertEqual(fromString(object((get$_2) => {
        let objectArg_2;
        return new SmallRecord((objectArg_2 = get$_2.Required, objectArg_2.Field("name", string)));
    }), "{ \"age\": 25 }"), expected_287);
}), Test_testCase("get.Required.Field returns Error if type is incorrect", () => {
    const expected_289 = new FSharpResult$2(1, ["\nError at: `$.name`\nExpecting a string but instead got: 12\n                        ".trim()]);
    assertEqual(fromString(object((get$_3) => {
        let objectArg_3;
        return new SmallRecord((objectArg_3 = get$_3.Required, objectArg_3.Field("name", string)));
    }), "{ \"name\": 12, \"age\": 25 }"), expected_289);
}), Test_testCase("get.Optional.Field works", () => {
    const expected_291 = new FSharpResult$2(0, [new SmallRecord2("maxime")]);
    assertEqual(fromString(object((get$_4) => {
        let objectArg_4;
        return new SmallRecord2((objectArg_4 = get$_4.Optional, objectArg_4.Field("name", string)));
    }), "{ \"name\": \"maxime\", \"age\": 25 }"), expected_291);
}), Test_testCase("get.Optional.Field returns None value if field is missing", () => {
    const expected_293 = new FSharpResult$2(0, [new SmallRecord2(void 0)]);
    assertEqual(fromString(object((get$_5) => {
        let objectArg_5;
        return new SmallRecord2((objectArg_5 = get$_5.Optional, objectArg_5.Field("name", string)));
    }), "{ \"age\": 25 }"), expected_293);
}), Test_testCase("get.Optional.Field returns None if field is null", () => {
    const expected_295 = new FSharpResult$2(0, [new SmallRecord2(void 0)]);
    assertEqual(fromString(object((get$_6) => {
        let objectArg_6;
        return new SmallRecord2((objectArg_6 = get$_6.Optional, objectArg_6.Field("name", string)));
    }), "{ \"name\": null, \"age\": 25 }"), expected_295);
}), Test_testCase("get.Optional.Field returns Error value if decoder fails", () => {
    const expected_297 = new FSharpResult$2(1, ["\nError at: `$.name`\nExpecting a string but instead got: 12\n                        ".trim()]);
    assertEqual(fromString(object((get$_7) => {
        let objectArg_7;
        return new SmallRecord2((objectArg_7 = get$_7.Optional, objectArg_7.Field("name", string)));
    }), "{ \"name\": 12, \"age\": 25 }"), expected_297);
}), Test_testCase("nested get.Optional.Field > get.Required.Field returns None if field is null", () => {
    const expected_299 = new FSharpResult$2(0, [new Model(void 0, 25)]);
    const userDecoder = object((get$_8) => {
        let objectArg_8, objectArg_9, objectArg_10;
        return new User((objectArg_8 = get$_8.Required, objectArg_8.Field("id", int)), (objectArg_9 = get$_8.Required, objectArg_9.Field("name", string)), (objectArg_10 = get$_8.Required, objectArg_10.Field("email", string)), 0);
    });
    assertEqual(fromString(object((get$_9) => {
        let objectArg_11, objectArg_12;
        return new Model((objectArg_11 = get$_9.Optional, objectArg_11.Field("user", userDecoder)), (objectArg_12 = get$_9.Required, objectArg_12.Field("field2", int)));
    }), "{ \"user\": null, \"field2\": 25 }"), expected_299);
}), Test_testCase("get.Optional.Field returns Error if type is incorrect", () => {
    const expected_301 = new FSharpResult$2(1, ["\nError at: `$.name`\nExpecting a string but instead got: 12\n                        ".trim()]);
    assertEqual(fromString(object((get$_10) => {
        let objectArg_13;
        return new SmallRecord2((objectArg_13 = get$_10.Optional, objectArg_13.Field("name", string)));
    }), "{ \"name\": 12, \"age\": 25 }"), expected_301);
}), Test_testCase("get.Required.At works", () => {
    const expected_303 = new FSharpResult$2(0, [new SmallRecord("maxime")]);
    assertEqual(fromString(object((get$_11) => {
        let objectArg_14;
        return new SmallRecord((objectArg_14 = get$_11.Required, objectArg_14.At(ofArray(["user", "name"]), string)));
    }), "{ \"user\": { \"name\": \"maxime\", \"age\": 25 } }"), expected_303);
}), Test_testCase("get.Required.At returns Error if non-object in path", () => {
    const expected_305 = new FSharpResult$2(1, ["\nError at: `$.user`\nExpecting an object but instead got:\n\"maxime\"\n                        ".trim()]);
    assertEqual(fromString(object((get$_12) => {
        let objectArg_15;
        return new SmallRecord((objectArg_15 = get$_12.Required, objectArg_15.At(ofArray(["user", "name"]), string)));
    }), "{ \"user\": \"maxime\" }"), expected_305);
}), Test_testCase("get.Required.At returns Error if field missing", () => {
    const expected_307 = new FSharpResult$2(1, ["\nError at: `$.user.firstname`\nExpecting an object with path `user.firstname` but instead got:\n{\n    \"user\": {\n        \"name\": \"maxime\",\n        \"age\": 25\n    }\n}\nNode `firstname` is unkown.\n                        ".trim()]);
    assertEqual(fromString(object((get$_13) => {
        let objectArg_16;
        return new SmallRecord((objectArg_16 = get$_13.Required, objectArg_16.At(ofArray(["user", "firstname"]), string)));
    }), "{ \"user\": { \"name\": \"maxime\", \"age\": 25 } }"), expected_307);
}), Test_testCase("get.Required.At returns Error if type is incorrect", () => {
    const expected_309 = new FSharpResult$2(1, ["\nError at: `$.user.name`\nExpecting a string but instead got: 12\n                        ".trim()]);
    assertEqual(fromString(object((get$_14) => {
        let objectArg_17;
        return new SmallRecord((objectArg_17 = get$_14.Required, objectArg_17.At(ofArray(["user", "name"]), string)));
    }), "{ \"user\": { \"name\": 12, \"age\": 25 } }"), expected_309);
}), Test_testCase("get.Optional.At works", () => {
    const expected_311 = new FSharpResult$2(0, [new SmallRecord2("maxime")]);
    assertEqual(fromString(object((get$_15) => {
        let objectArg_18;
        return new SmallRecord2((objectArg_18 = get$_15.Optional, objectArg_18.At(ofArray(["user", "name"]), string)));
    }), "{ \"user\": { \"name\": \"maxime\", \"age\": 25 } }"), expected_311);
}), Test_testCase("get.Optional.At returns \'type error\' if non-object in path", () => {
    const expected_313 = new FSharpResult$2(1, ["\nError at: `$.user`\nExpecting an object but instead got:\n\"maxime\"\n                        ".trim()]);
    assertEqual(fromString(object((get$_16) => {
        let objectArg_19;
        return new SmallRecord2((objectArg_19 = get$_16.Optional, objectArg_19.At(ofArray(["user", "name"]), string)));
    }), "{ \"user\": \"maxime\" }"), expected_313);
}), Test_testCase("get.Optional.At returns None if field missing", () => {
    const expected_315 = new FSharpResult$2(0, [new SmallRecord2(void 0)]);
    assertEqual(fromString(object((get$_17) => {
        let objectArg_20;
        return new SmallRecord2((objectArg_20 = get$_17.Optional, objectArg_20.At(ofArray(["user", "firstname"]), string)));
    }), "{ \"user\": { \"name\": \"maxime\", \"age\": 25 } }"), expected_315);
}), Test_testCase("get.Optional.At returns Error if type is incorrect", () => {
    const expected_317 = new FSharpResult$2(1, ["\nError at: `$.user.name`\nExpecting a string but instead got: 12\n                        ".trim()]);
    assertEqual(fromString(object((get$_18) => {
        let objectArg_21;
        return new SmallRecord2((objectArg_21 = get$_18.Optional, objectArg_21.At(ofArray(["user", "name"]), string)));
    }), "{ \"user\": { \"name\": 12, \"age\": 25 } }"), expected_317);
}), Test_testCase("complex object builder works", () => {
    const expected_319 = new FSharpResult$2(0, [User_Create(67, "", "user@mail.com", 0)]);
    assertEqual(fromString(object((get$_19) => {
        let objectArg_22, objectArg_23, objectArg_24;
        return new User((objectArg_22 = get$_19.Required, objectArg_22.Field("id", int)), defaultArg((objectArg_23 = get$_19.Optional, objectArg_23.Field("name", string)), ""), (objectArg_24 = get$_19.Required, objectArg_24.Field("email", string)), 0);
    }), "{ \"id\": 67, \"email\": \"user@mail.com\" }"), expected_319);
}), Test_testCase("get.Field.Raw works", () => {
    const shapeDecoder = andThen_1((_arg_154) => {
        switch (_arg_154) {
            case "circle":
                return Shape_get_DecoderCircle();
            case "rectangle":
                return Shape_get_DecoderRectangle();
            default:
                return fail(toText(printf("Unknown shape type %s"))(_arg_154));
        }
    }, field("shape", string));
    assertEqual(fromString(object((get$_20) => {
        let objectArg_25;
        return new MyObj((objectArg_25 = get$_20.Required, objectArg_25.Field("enabled", bool)), get$_20.Required.Raw(shapeDecoder));
    }), "{\n    \"enabled\": true,\n\t\"shape\": \"circle\",\n    \"radius\": 20\n}"), new FSharpResult$2(0, [new MyObj(true, new Shape(0, [20]))]));
}), Test_testCase("get.Field.Raw returns Error if a decoder fail", () => {
    const shapeDecoder_1 = andThen_1((_arg_156) => {
        switch (_arg_156) {
            case "circle":
                return Shape_get_DecoderCircle();
            case "rectangle":
                return Shape_get_DecoderRectangle();
            default:
                return fail(toText(printf("Unknown shape type %s"))(_arg_156));
        }
    }, field("shape", string));
    assertEqual(fromString(object((get$_21) => {
        let objectArg_26;
        return new MyObj((objectArg_26 = get$_21.Required, objectArg_26.Field("enabled", bool)), get$_21.Required.Raw(shapeDecoder_1));
    }), "{\n    \"enabled\": true,\n\t\"shape\": \"custom_shape\",\n    \"radius\": 20\n}"), new FSharpResult$2(1, ["Error at: `$`\nThe following `failure` occurred with the decoder: Unknown shape type custom_shape"]));
}), Test_testCase("get.Field.Raw returns Error if a field is missing in the \'raw decoder\'", () => {
    const shapeDecoder_2 = andThen_1((_arg_158) => {
        switch (_arg_158) {
            case "circle":
                return Shape_get_DecoderCircle();
            case "rectangle":
                return Shape_get_DecoderRectangle();
            default:
                return fail(toText(printf("Unknown shape type %s"))(_arg_158));
        }
    }, field("shape", string));
    assertEqual(fromString(object((get$_22) => {
        let objectArg_27;
        return new MyObj((objectArg_27 = get$_22.Required, objectArg_27.Field("enabled", bool)), get$_22.Required.Raw(shapeDecoder_2));
    }), "{\n    \"enabled\": true,\n\t\"shape\": \"circle\"\n}"), new FSharpResult$2(1, ["\nError at: `$`\nExpecting an object with a field named `radius` but instead got:\n{\n    \"enabled\": true,\n    \"shape\": \"circle\"\n}                   ".trim()]));
}), Test_testCase("get.Optional.Raw works", () => {
    const shapeDecoder_3 = andThen_1((_arg_160) => {
        switch (_arg_160) {
            case "circle":
                return Shape_get_DecoderCircle();
            case "rectangle":
                return Shape_get_DecoderRectangle();
            default:
                return fail(toText(printf("Unknown shape type %s"))(_arg_160));
        }
    }, field("shape", string));
    assertEqual(fromString(object((get$_23) => {
        let objectArg_28;
        return new MyObj2((objectArg_28 = get$_23.Required, objectArg_28.Field("enabled", bool)), get$_23.Optional.Raw(shapeDecoder_3));
    }), "{\n    \"enabled\": true,\n\t\"shape\": \"circle\",\n    \"radius\": 20\n}"), new FSharpResult$2(0, [new MyObj2(true, new Shape(0, [20]))]));
}), Test_testCase("get.Optional.Raw returns None if a field is missing", () => {
    const shapeDecoder_4 = andThen_1((_arg_162) => {
        switch (_arg_162) {
            case "circle":
                return Shape_get_DecoderCircle();
            case "rectangle":
                return Shape_get_DecoderRectangle();
            default:
                return fail(toText(printf("Unknown shape type %s"))(_arg_162));
        }
    }, field("shape", string));
    assertEqual(fromString(object((get$_24) => {
        let objectArg_29;
        return new MyObj2((objectArg_29 = get$_24.Required, objectArg_29.Field("enabled", bool)), get$_24.Optional.Raw(shapeDecoder_4));
    }), "{\n    \"enabled\": true,\n\t\"shape\": \"circle\"\n}"), new FSharpResult$2(0, [new MyObj2(true, void 0)]));
}), Test_testCase("get.Optional.Raw returns an Error if a decoder fail", () => {
    const shapeDecoder_5 = andThen_1((_arg_164) => {
        switch (_arg_164) {
            case "circle":
                return Shape_get_DecoderCircle();
            case "rectangle":
                return Shape_get_DecoderRectangle();
            default:
                return fail(toText(printf("Unknown shape type %s"))(_arg_164));
        }
    }, field("shape", string));
    assertEqual(fromString(object((get$_25) => {
        let objectArg_30;
        return new MyObj2((objectArg_30 = get$_25.Required, objectArg_30.Field("enabled", bool)), get$_25.Optional.Raw(shapeDecoder_5));
    }), "{\n    \"enabled\": true,\n\t\"shape\": \"invalid_shape\"\n}"), new FSharpResult$2(1, ["Error at: `$`\nThe following `failure` occurred with the decoder: Unknown shape type invalid_shape"]));
}), Test_testCase("get.Optional.Raw returns an Error if the type is invalid", () => {
    const shapeDecoder_6 = andThen_1((_arg_166) => {
        switch (_arg_166) {
            case "circle":
                return Shape_get_DecoderCircle();
            case "rectangle":
                return Shape_get_DecoderRectangle();
            default:
                return fail(toText(printf("Unknown shape type %s"))(_arg_166));
        }
    }, field("shape", string));
    assertEqual(fromString(object((get$_26) => {
        let objectArg_31;
        return new MyObj2((objectArg_31 = get$_26.Required, objectArg_31.Field("enabled", bool)), get$_26.Optional.Raw(shapeDecoder_6));
    }), "{\n    \"enabled\": true,\n\t\"shape\": \"circle\",\n    \"radius\": \"maxime\"\n}"), new FSharpResult$2(1, ["Error at: `$.radius`\nExpecting an int but instead got: \"maxime\""]));
}), Test_testCase("get.Optional.Raw returns None if a decoder fails with null", () => {
    const shapeDecoder_7 = andThen_1((_arg_168) => {
        switch (_arg_168) {
            case "circle":
                return Shape_get_DecoderCircle();
            case "rectangle":
                return Shape_get_DecoderRectangle();
            default:
                return fail(toText(printf("Unknown shape type %s"))(_arg_168));
        }
    }, field("shape", string));
    assertEqual(fromString(object((get$_27) => {
        let objectArg_32;
        return new MyObj2((objectArg_32 = get$_27.Required, objectArg_32.Field("enabled", bool)), get$_27.Optional.Raw(shapeDecoder_7));
    }), "{\n    \"enabled\": true,\n\t\"shape\": null\n}"), new FSharpResult$2(0, [new MyObj2(true, void 0)]));
}), Test_testCase("Object builders returns all the Errors", () => {
    const expected_337 = new FSharpResult$2(1, ["\nThe following errors were found:\n\nError at: `$`\nExpecting an object with a field named `missing_field_1` but instead got:\n{\n    \"age\": 25,\n    \"fieldC\": \"not_a_number\",\n    \"fieldD\": {\n        \"sub_field\": \"not_a_boolean\"\n    }\n}\n\nError at: `$.missing_field_2.sub_field`\nExpecting an object with path `missing_field_2.sub_field` but instead got:\n{\n    \"age\": 25,\n    \"fieldC\": \"not_a_number\",\n    \"fieldD\": {\n        \"sub_field\": \"not_a_boolean\"\n    }\n}\nNode `sub_field` is unkown.\n\nError at: `$.fieldC`\nExpecting an int but instead got: \"not_a_number\"\n\nError at: `$.fieldD.sub_field`\nExpecting a boolean but instead got: \"not_a_boolean\"\n                        ".trim()]);
    assertEqual(fromString(object((get$_28) => {
        let objectArg_33, objectArg_34, objectArg_35, objectArg_36;
        return new MediumRecord((objectArg_33 = get$_28.Required, objectArg_33.Field("missing_field_1", string)), (objectArg_34 = get$_28.Required, objectArg_34.At(ofArray(["missing_field_2", "sub_field"]), string)), defaultArg((objectArg_35 = get$_28.Optional, objectArg_35.Field("fieldC", int)), -1), defaultArg((objectArg_36 = get$_28.Optional, objectArg_36.At(ofArray(["fieldD", "sub_field"]), bool)), false));
    }), "{ \"age\": 25, \"fieldC\": \"not_a_number\", \"fieldD\": { \"sub_field\": \"not_a_boolean\" } }"), expected_337);
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
    assertEqual(fromString(object((get$_31) => {
        let objectArg_39, objectArg_40;
        return new Data((objectArg_39 = get$_31.Required, objectArg_39.Field("person", personDecoder)), (objectArg_40 = get$_31.Optional, objectArg_40.Field("post", postDecoder)));
    }), json_61), new FSharpResult$2(0, [new Data(new Person("maxime"), void 0)]));
})]))]));

