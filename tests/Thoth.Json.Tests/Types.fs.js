import { FSharpException, Union, Record } from "./Thoth.Json.JavaScript.Tests/fable_modules/fable-library.4.5.0/Types.js";
import { bigint_type, char_type, unit_type, uint64_type, int64_type, uint32_type, uint16_type, int16_type, uint8_type, int8_type, class_type, array_type, option_type, list_type, tuple_type, bool_type, string_type, union_type, int32_type, record_type, float64_type } from "./Thoth.Json.JavaScript.Tests/fable_modules/fable-library.4.5.0/Reflection.js";
import { fail, andThen, tuple2, int, field, map, string, object } from "../packages/Thoth.Json.Core/Decode.fs.js";
import { Json } from "../packages/Thoth.Json.Core/Types.fs.js";
import { defaultOf } from "./Thoth.Json.JavaScript.Tests/fable_modules/fable-library.4.5.0/Util.js";

export class Record2 extends Record {
    constructor(a, b) {
        super();
        this.a = a;
        this.b = b;
    }
}

export function Record2_$reflection() {
    return record_type("Tests.Types.Record2", [], Record2, () => [["a", float64_type], ["b", float64_type]]);
}

export function Record2_Create(a, b) {
    return new Record2(a, b);
}

export class Record3 extends Record {
    constructor(a, b, c) {
        super();
        this.a = a;
        this.b = b;
        this.c = c;
    }
}

export function Record3_$reflection() {
    return record_type("Tests.Types.Record3", [], Record3, () => [["a", float64_type], ["b", float64_type], ["c", float64_type]]);
}

export function Record3_Create(a, b, c) {
    return new Record3(a, b, c);
}

export class Record4 extends Record {
    constructor(a, b, c, d) {
        super();
        this.a = a;
        this.b = b;
        this.c = c;
        this.d = d;
    }
}

export function Record4_$reflection() {
    return record_type("Tests.Types.Record4", [], Record4, () => [["a", float64_type], ["b", float64_type], ["c", float64_type], ["d", float64_type]]);
}

export function Record4_Create(a, b, c, d) {
    return new Record4(a, b, c, d);
}

export class Record5 extends Record {
    constructor(a, b, c, d, e) {
        super();
        this.a = a;
        this.b = b;
        this.c = c;
        this.d = d;
        this.e = e;
    }
}

export function Record5_$reflection() {
    return record_type("Tests.Types.Record5", [], Record5, () => [["a", float64_type], ["b", float64_type], ["c", float64_type], ["d", float64_type], ["e", float64_type]]);
}

export function Record5_Create(a, b, c, d, e) {
    return new Record5(a, b, c, d, e);
}

export class Record6 extends Record {
    constructor(a, b, c, d, e, f) {
        super();
        this.a = a;
        this.b = b;
        this.c = c;
        this.d = d;
        this.e = e;
        this.f = f;
    }
}

export function Record6_$reflection() {
    return record_type("Tests.Types.Record6", [], Record6, () => [["a", float64_type], ["b", float64_type], ["c", float64_type], ["d", float64_type], ["e", float64_type], ["f", float64_type]]);
}

export function Record6_Create(a, b, c, d, e, f) {
    return new Record6(a, b, c, d, e, f);
}

export class Record7 extends Record {
    constructor(a, b, c, d, e, f, g) {
        super();
        this.a = a;
        this.b = b;
        this.c = c;
        this.d = d;
        this.e = e;
        this.f = f;
        this.g = g;
    }
}

export function Record7_$reflection() {
    return record_type("Tests.Types.Record7", [], Record7, () => [["a", float64_type], ["b", float64_type], ["c", float64_type], ["d", float64_type], ["e", float64_type], ["f", float64_type], ["g", float64_type]]);
}

export function Record7_Create(a, b, c, d, e, f, g) {
    return new Record7(a, b, c, d, e, f, g);
}

export class Record8 extends Record {
    constructor(a, b, c, d, e, f, g, h) {
        super();
        this.a = a;
        this.b = b;
        this.c = c;
        this.d = d;
        this.e = e;
        this.f = f;
        this.g = g;
        this.h = h;
    }
}

export function Record8_$reflection() {
    return record_type("Tests.Types.Record8", [], Record8, () => [["a", float64_type], ["b", float64_type], ["c", float64_type], ["d", float64_type], ["e", float64_type], ["f", float64_type], ["g", float64_type], ["h", float64_type]]);
}

export function Record8_Create(a, b, c, d, e, f, g, h) {
    return new Record8(a, b, c, d, e, f, g, h);
}

export class MyUnion extends Union {
    constructor(Item) {
        super();
        this.tag = 0;
        this.fields = [Item];
    }
    cases() {
        return ["Foo"];
    }
}

export function MyUnion_$reflection() {
    return union_type("Tests.Types.MyUnion", [], MyUnion, () => [[["Item", int32_type]]]);
}

export class Record9 extends Record {
    constructor(a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, r, s) {
        super();
        this.a = (a | 0);
        this.b = b;
        this.c = c;
        this.d = d;
        this.e = e;
        this.f = f;
        this.g = g;
        this.h = h;
        this.i = (i | 0);
        this.j = j;
        this.k = (k | 0);
        this.l = l;
        this.m = m;
        this.n = n;
        this.o = o;
        this.p = p;
        this.r = r;
        this.s = s;
    }
}

export function Record9_$reflection() {
    return record_type("Tests.Types.Record9", [], Record9, () => [["a", int32_type], ["b", string_type], ["c", list_type(tuple_type(bool_type, int32_type))], ["d", array_type(option_type(MyUnion_$reflection()))], ["e", class_type("Microsoft.FSharp.Collections.FSharpMap`2", [string_type, Record2_$reflection()])], ["f", class_type("System.DateTime")], ["g", class_type("Microsoft.FSharp.Collections.FSharpSet`1", [Record2_$reflection()])], ["h", class_type("System.TimeSpan")], ["i", int8_type], ["j", uint8_type], ["k", int16_type], ["l", uint16_type], ["m", uint32_type], ["n", int64_type], ["o", uint64_type], ["p", unit_type], ["r", class_type("Microsoft.FSharp.Collections.FSharpMap`2", [Record2_$reflection(), string_type])], ["s", char_type]]);
}

export class Record10 extends Record {
    constructor(a, b, c, d, e, f, g, h, i, j, k) {
        super();
        this.a = (a | 0);
        this.b = (b | 0);
        this.c = (c | 0);
        this.d = (d | 0);
        this.e = (e | 0);
        this.f = (f | 0);
        this.g = (g | 0);
        this.h = (h | 0);
        this.i = (i | 0);
        this.j = (j | 0);
        this.k = (k | 0);
    }
}

export function Record10_$reflection() {
    return record_type("Tests.Types.Record10", [], Record10, () => [["a", int32_type], ["b", int32_type], ["c", int32_type], ["d", int32_type], ["e", int32_type], ["f", int32_type], ["g", int32_type], ["h", int32_type], ["i", int32_type], ["j", int32_type], ["k", int32_type]]);
}

export function Record10_Create(a, b, c, d, e, f, g, h, i, j, k) {
    return new Record10(a, b, c, d, e, f, g, h, i, j, k);
}

export class User extends Record {
    constructor(Id, Name, Email, Followers) {
        super();
        this.Id = (Id | 0);
        this.Name = Name;
        this.Email = Email;
        this.Followers = (Followers | 0);
    }
}

export function User_$reflection() {
    return record_type("Tests.Types.User", [], User, () => [["Id", int32_type], ["Name", string_type], ["Email", string_type], ["Followers", int32_type]]);
}

export function User_Create(id, name, email, followers) {
    return new User(id, name, email, followers);
}

export class SmallRecord extends Record {
    constructor(fieldA) {
        super();
        this.fieldA = fieldA;
    }
}

export function SmallRecord_$reflection() {
    return record_type("Tests.Types.SmallRecord", [], SmallRecord, () => [["fieldA", string_type]]);
}

export function SmallRecord_get_Decoder() {
    return object((get$) => {
        let objectArg;
        return new SmallRecord((objectArg = get$.Required, objectArg.Field("fieldA", string)));
    });
}

export function SmallRecord_Encoder_Z4AB0BC7(x) {
    return new Json(6, [[["fieldA", new Json(0, [x.fieldA])]]]);
}

export class MediumRecord extends Record {
    constructor(FieldA, FieldB, FieldC, FieldD) {
        super();
        this.FieldA = FieldA;
        this.FieldB = FieldB;
        this.FieldC = (FieldC | 0);
        this.FieldD = FieldD;
    }
}

export function MediumRecord_$reflection() {
    return record_type("Tests.Types.MediumRecord", [], MediumRecord, () => [["FieldA", string_type], ["FieldB", string_type], ["FieldC", int32_type], ["FieldD", bool_type]]);
}

export class SmallRecord2 extends Record {
    constructor(optionalField) {
        super();
        this.optionalField = optionalField;
    }
}

export function SmallRecord2_$reflection() {
    return record_type("Tests.Types.SmallRecord2", [], SmallRecord2, () => [["optionalField", option_type(string_type)]]);
}

export class Model extends Record {
    constructor(User, Field2) {
        super();
        this.User = User;
        this.Field2 = (Field2 | 0);
    }
}

export function Model_$reflection() {
    return record_type("Tests.Types.Model", [], Model, () => [["User", option_type(User_$reflection())], ["Field2", int32_type]]);
}

export class MyList$1 extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["Nil", "Cons"];
    }
}

export function MyList$1_$reflection(gen0) {
    return union_type("Tests.Types.MyList`1", [gen0], MyList$1, () => [[], [["Item1", gen0], ["Item2", MyList$1_$reflection(gen0)]]]);
}

export class TestMaybeRecord extends Record {
    constructor(Maybe, Must) {
        super();
        this.Maybe = Maybe;
        this.Must = Must;
    }
}

export function TestMaybeRecord_$reflection() {
    return record_type("Tests.Types.TestMaybeRecord", [], TestMaybeRecord, () => [["Maybe", option_type(string_type)], ["Must", string_type]]);
}

export class BaseClass {
    constructor() {
    }
}

export function BaseClass_$reflection() {
    return class_type("Tests.Types.BaseClass", void 0, BaseClass);
}

export class RecordWithOptionalClass extends Record {
    constructor(MaybeClass, Must) {
        super();
        this.MaybeClass = MaybeClass;
        this.Must = Must;
    }
}

export function RecordWithOptionalClass_$reflection() {
    return record_type("Tests.Types.RecordWithOptionalClass", [], RecordWithOptionalClass, () => [["MaybeClass", option_type(BaseClass_$reflection())], ["Must", string_type]]);
}

export class RecordWithRequiredClass extends Record {
    constructor(Class, Must) {
        super();
        this.Class = Class;
        this.Must = Must;
    }
}

export function RecordWithRequiredClass_$reflection() {
    return record_type("Tests.Types.RecordWithRequiredClass", [], RecordWithRequiredClass, () => [["Class", BaseClass_$reflection()], ["Must", string_type]]);
}

export class Shape extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["Circle", "Rectangle"];
    }
}

export function Shape_$reflection() {
    return union_type("Tests.Types.Shape", [], Shape, () => [[["radius", int32_type]], [["width", int32_type], ["height", int32_type]]]);
}

export function Shape_get_DecoderCircle() {
    return map((radius) => (new Shape(0, [radius])), field("radius", int));
}

export function Shape_get_DecoderRectangle() {
    return map((tupledArg) => (new Shape(1, [tupledArg[0], tupledArg[1]])), tuple2(field("width", int), field("height", int)));
}

export class MyObj extends Record {
    constructor(Enabled, Shape) {
        super();
        this.Enabled = Enabled;
        this.Shape = Shape;
    }
}

export function MyObj_$reflection() {
    return record_type("Tests.Types.MyObj", [], MyObj, () => [["Enabled", bool_type], ["Shape", Shape_$reflection()]]);
}

export class MyObj2 extends Record {
    constructor(Enabled, Shape) {
        super();
        this.Enabled = Enabled;
        this.Shape = Shape;
    }
}

export function MyObj2_$reflection() {
    return record_type("Tests.Types.MyObj2", [], MyObj2, () => [["Enabled", bool_type], ["Shape", option_type(Shape_$reflection())]]);
}

export class CustomException extends FSharpException {
    constructor() {
        super();
    }
}

export function CustomException_$reflection() {
    return class_type("Tests.Types.CustomException", void 0, CustomException, class_type("System.Exception"));
}

export class BigIntRecord extends Record {
    constructor(bigintField) {
        super();
        this.bigintField = bigintField;
    }
}

export function BigIntRecord_$reflection() {
    return record_type("Tests.Types.BigIntRecord", [], BigIntRecord, () => [["bigintField", bigint_type]]);
}

export class ChildType extends Record {
    constructor(ChildField) {
        super();
        this.ChildField = ChildField;
    }
}

export function ChildType_$reflection() {
    return record_type("Tests.Types.ChildType", [], ChildType, () => [["ChildField", string_type]]);
}

export function ChildType_get_Decoder() {
    return map((x) => (new ChildType(x)), string);
}

export class ParentRecord extends Record {
    constructor(ParentField) {
        super();
        this.ParentField = ParentField;
    }
}

export function ParentRecord_$reflection() {
    return record_type("Tests.Types.ParentRecord", [], ParentRecord, () => [["ParentField", ChildType_$reflection()]]);
}

export class Price extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["Normal", "Reduced", "Zero"];
    }
}

export function Price_$reflection() {
    return union_type("Tests.Types.Price", [], Price, () => [[["Item", float64_type]], [["Item", option_type(float64_type)]], []]);
}

export class RecordWithStrangeType extends Record {
    constructor(Id, Thread) {
        super();
        this.Id = (Id | 0);
        this.Thread = Thread;
    }
}

export function RecordWithStrangeType_$reflection() {
    return record_type("Tests.Types.RecordWithStrangeType", [], RecordWithStrangeType, () => [["Id", int32_type], ["Thread", option_type(class_type("System.Threading.Thread"))]]);
}

export class UserCaseSensitive extends Record {
    constructor(Id, Name, Email, followers) {
        super();
        this.Id = (Id | 0);
        this.Name = Name;
        this.Email = Email;
        this.followers = (followers | 0);
    }
}

export function UserCaseSensitive_$reflection() {
    return record_type("Tests.Types.UserCaseSensitive", [], UserCaseSensitive, () => [["Id", int32_type], ["Name", string_type], ["Email", string_type], ["followers", int32_type]]);
}

export class RecordWithInterface extends Record {
    constructor(Id, Interface) {
        super();
        this.Id = (Id | 0);
        this.Interface = Interface;
    }
}

export function RecordWithInterface_$reflection() {
    return record_type("Tests.Types.RecordWithInterface", [], RecordWithInterface, () => [["Id", int32_type], ["Interface", option_type(class_type("Tests.Types.IAmAnInterface"))]]);
}

export class MyRecType extends Record {
    constructor(Name, Children) {
        super();
        this.Name = Name;
        this.Children = Children;
    }
}

export function MyRecType_$reflection() {
    return record_type("Tests.Types.MyRecType", [], MyRecType, () => [["Name", string_type], ["Children", list_type(MyRecType_$reflection())]]);
}

export class TestStringWithHTML extends Record {
    constructor(FeedName, Content) {
        super();
        this.FeedName = FeedName;
        this.Content = Content;
    }
}

export function TestStringWithHTML_$reflection() {
    return record_type("Tests.Types.TestStringWithHTML", [], TestStringWithHTML, () => [["FeedName", string_type], ["Content", string_type]]);
}

export class RecordForCharacterCase extends Record {
    constructor(One, TwoPart, ThreePartField) {
        super();
        this.One = (One | 0);
        this.TwoPart = (TwoPart | 0);
        this.ThreePartField = (ThreePartField | 0);
    }
}

export function RecordForCharacterCase_$reflection() {
    return record_type("Tests.Types.RecordForCharacterCase", [], RecordForCharacterCase, () => [["One", int32_type], ["TwoPart", int32_type], ["ThreePartField", int32_type]]);
}

export function IntAsRecord_encode(value) {
    return defaultOf();
}

export const IntAsRecord_decode = andThen((typ) => {
    if (typ === "int") {
        return field("value", int);
    }
    else {
        return fail("Invalid type");
    }
}, field("type", string));

export class Person extends Record {
    constructor(Name) {
        super();
        this.Name = Name;
    }
}

export function Person_$reflection() {
    return record_type("Tests.Types.Person", [], Person, () => [["Name", string_type]]);
}

export class Post extends Record {
    constructor(Title) {
        super();
        this.Title = Title;
    }
}

export function Post_$reflection() {
    return record_type("Tests.Types.Post", [], Post, () => [["Title", string_type]]);
}

export class Data extends Record {
    constructor(Person, Post) {
        super();
        this.Person = Person;
        this.Post = Post;
    }
}

export function Data_$reflection() {
    return record_type("Tests.Types.Data", [], Data, () => [["Person", Person_$reflection()], ["Post", option_type(Post_$reflection())]]);
}

