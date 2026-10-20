
import { getTupleElements, getUnionCaseFields, fullName, string_type, name, getUnionFields, getRecordField, lambda_type, obj_type, class_type } from "fable-library-js/Reflection.js";
import { float, float32, dict, seq, Enum_uint32, Enum_int, Enum_uint16, Enum_int16, Enum_sbyte, Enum_byte, tuple2, list as list_1, lazily, losslessOption as losslessOption_1, lossyOption } from "../Thoth.Json.Core/Encode.js";
import { append, isEmpty, empty, singleton, collect, delay, toList, map, toArray } from "fable-library-js/Seq.js";
import { item, map as map_1 } from "fable-library-js/Array.js";
import { map as map_2 } from "fable-library-js/List.js";
import { add, tryFind, toList as toList_1, toSeq, ofSeq } from "fable-library-js/Map.js";
import { defaultOf, Exception, comparePrimitives } from "fable-library-js/Util.js";
import { Type_isRecursive, ActivePatterns_$007CListType$007C_$007C, ActivePatterns_$007CSeqType$007C_$007C, ActivePatterns_$007COptionType$007C_$007C, ActivePatterns_$007CDateTimeOffsetType$007C_$007C, ActivePatterns_$007CDateTimeType$007C_$007C, ActivePatterns_$007CUriType$007C_$007C, ActivePatterns_$007CTimeSpanType$007C_$007C, ActivePatterns_$007CDecimalType$007C_$007C, ActivePatterns_$007CDoubleType$007C_$007C, ActivePatterns_$007CSingleType$007C_$007C, ActivePatterns_$007CBigIntType$007C_$007C, ActivePatterns_$007CUInt64Type$007C_$007C, ActivePatterns_$007CInt64Type$007C_$007C, ActivePatterns_$007CBoolType$007C_$007C, ActivePatterns_$007CStringType$007C_$007C, ActivePatterns_$007CCharType$007C_$007C, ActivePatterns_$007CUnitType$007C_$007C, ActivePatterns_$007CGuidType$007C_$007C, ActivePatterns_$007CByteType$007C_$007C, ActivePatterns_$007CFSharpTupleType$007C_$007C, ActivePatterns_$007CFSharpUnionType$007C_$007C, ActivePatterns_$007CFSharpRecordType$007C_$007C, ActivePatterns_$007CArrayType$007C_$007C, ActivePatterns_$007CSetType$007C_$007C, ActivePatterns_$007CMapType$007C_$007C, ActivePatterns_$007CSByteType$007C_$007C, ActivePatterns_$007CInt16Type$007C_$007C, ActivePatterns_$007CUInt16Type$007C_$007C, ActivePatterns_$007CIntType$007C_$007C, ActivePatterns_$007CUIntType$007C_$007C, ActivePatterns_$007CEnumType$007C_$007C, Lazy_makeGeneric } from "./Prelude.js";
import { convertCase } from "./Casing.js";
import { Cache$2_$ctor, TypeKeyModule_ofType, CaseStrategy } from "./Domain.js";
import { toString } from "fable-library-js/BigInt.js";
import { toString as toString_1 } from "fable-library-js/TimeSpan.js";
import { toString as toString_2 } from "fable-library-js/Date.js";
import { concat } from "fable-library-js/String.js";
import { value as value_44 } from "fable-library-js/Option.js";
import { Operators_IsNull } from "fable-library-js/FSharp.Core.js";
import { rangeDouble } from "fable-library-js/Range.js";

export class Encode_Generic_EncodeHelpers {
    constructor() {
    }
}

export function Encode_Generic_EncodeHelpers_$reflection() {
    return class_type("Thoth.Json.Core.Auto.Encode.Encode.Generic.EncodeHelpers", undefined, Encode_Generic_EncodeHelpers);
}

export function Encode_Generic_EncodeHelpers_OptionOf_69F68B54(enc) {
    return lossyOption(enc);
}

export function Encode_Generic_EncodeHelpers_OptionOfLossless_69F68B54(enc) {
    return (value) => losslessOption_1(enc, value);
}

export function Encode_Generic_EncodeHelpers_Lazily_7F4AF886(enc) {
    return (x) => lazily(enc, x);
}

export function Encode_Generic_EncodeHelpers_SeqOf_69F68B54(enc) {
    return (xs) => {
        const values = toArray(map(enc, xs));
        return {
            Encode(helpers) {
                const arg = map_1((v) => v.Encode(helpers), values);
                return helpers.encodeArray(arg);
            },
        };
    };
}

export function Encode_Generic_EncodeHelpers_ListOf_69F68B54(enc) {
    return (xs) => list_1(map_2(enc, xs));
}

export function Encode_Generic_EncodeHelpers_MapOf_Z223C77DB(stringifyKey, enc) {
    return (m) => {
        const values = toList(delay(() => collect((matchValue) => {
            const activePatternResult = matchValue;
            return singleton([stringifyKey(activePatternResult[0]), enc(activePatternResult[1])]);
        }, m)));
        return {
            Encode(helpers) {
                const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers)], values);
                return helpers.encodeObject(arg);
            },
        };
    };
}

export function Encode_Generic_EncodeHelpers_MapAsArrayOf_Z629B8BA7(keyEncoder, valueEncoder) {
    return (m) => {
        const values = toArray(delay(() => collect((matchValue) => {
            const activePatternResult = matchValue;
            return singleton(tuple2(keyEncoder, valueEncoder, activePatternResult[0], activePatternResult[1]));
        }, m)));
        return {
            Encode(helpers) {
                const arg = map_1((v_1) => v_1.Encode(helpers), values);
                return helpers.encodeArray(arg);
            },
        };
    };
}

export function Encode_Generic_EncodeHelpers_SetOf_4578D16D(enc) {
    return (xs) => {
        const values = toArray(map(enc, xs));
        return {
            Encode(helpers) {
                const arg = map_1((v) => v.Encode(helpers), values);
                return helpers.encodeArray(arg);
            },
        };
    };
}

export function Encode_Generic_EncodeHelpers_ArrayOf_69F68B54(enc) {
    return (xs) => {
        const values = map_1(enc, xs);
        return {
            Encode(helpers) {
                const arg = map_1((v) => v.Encode(helpers), values);
                return helpers.encodeArray(arg);
            },
        };
    };
}

export function Encode_Generic_EncodeHelpers_EnumByte() {
    return Enum_byte;
}

export function Encode_Generic_EncodeHelpers_EnumSbyte() {
    return Enum_sbyte;
}

export function Encode_Generic_EncodeHelpers_EnumInt16() {
    return Enum_int16;
}

export function Encode_Generic_EncodeHelpers_EnumUint16() {
    return Enum_uint16;
}

export function Encode_Generic_EncodeHelpers_EnumInt() {
    return Enum_int;
}

export function Encode_Generic_EncodeHelpers_EnumUint32() {
    return Enum_uint32;
}

export function Encode_Generic_optionOf(lossless, innerType, enc) {
    if (lossless) {
        return (value) => losslessOption_1(enc, value);
    }
    else {
        return lossyOption(enc);
    }
}

export function Encode_Generic_seqOf(innerType, enc) {
    return (xs) => seq(map(enc, xs));
}

export function Encode_Generic_listOf(innerType, enc) {
    return (xs) => list_1(map_2(enc, xs));
}

export function Encode_Generic_mapOf(keyType, valueType, stringifyKey, enc) {
    return (m) => dict(ofSeq(map((tupledArg) => [stringifyKey(tupledArg[0]), enc(tupledArg[1])], toSeq(m)), {
        Compare: (x, y) => (comparePrimitives(x, y) | 0),
    }));
}

export function Encode_Generic_mapAsArrayOf(keyType, valueType, keyEncoder, valueEncoder) {
    return (xs) => list_1(map_2((tupledArg) => tuple2(keyEncoder, valueEncoder, tupledArg[0], tupledArg[1]), toList_1(xs)));
}

export function Encode_Generic_setOf(innerType, enc) {
    return (xs) => {
        const values = toArray(map(enc, xs));
        return {
            Encode(helpers) {
                const arg = map_1((v) => v.Encode(helpers), values);
                return helpers.encodeArray(arg);
            },
        };
    };
}

export function Encode_Generic_arrayOf(innerType, enc) {
    return Encode_Generic_EncodeHelpers_ArrayOf_69F68B54(enc);
}

export function Encode_Generic_lazily(innerType, enc) {
    return (x) => lazily(enc, x);
}

export function Encode_Generic_Enum_byte(innerType) {
    return (value) => ({
        Encode(helpers) {
            return helpers.encodeUnsignedIntegralNumber(value);
        },
    });
}

export function Encode_Generic_Enum_sbyte(innerType) {
    return (value) => ({
        Encode(helpers) {
            return helpers.encodeSignedIntegralNumber(value);
        },
    });
}

export function Encode_Generic_Enum_int16(innerType) {
    return (value) => ({
        Encode(helpers) {
            return helpers.encodeSignedIntegralNumber(value);
        },
    });
}

export function Encode_Generic_Enum_uint16(innerType) {
    return (value) => ({
        Encode(helpers) {
            return helpers.encodeUnsignedIntegralNumber(value);
        },
    });
}

export function Encode_Generic_Enum_int(innerType) {
    return (value) => ({
        Encode(helpers) {
            return helpers.encodeSignedIntegralNumber(value);
        },
    });
}

export function Encode_Generic_Enum_uint32(innerType) {
    return (value) => ({
        Encode(helpers) {
            return helpers.encodeUnsignedIntegralNumber(value);
        },
    });
}

function makeLazyEncoder(ty, getSelf) {
    return Encode_Generic_lazily(ty, Lazy_makeGeneric(lambda_type(obj_type, obj_type), (_arg) => getSelf()));
}

function wrapBoxedEncoder(encoder) {
    return encoder;
}

function wrapFinalEncoder(_ty, funcImpl) {
    return funcImpl;
}

function makeFieldReader(pi, record) {
    return getRecordField(record, pi);
}

function makeUnionTagReader(ty, o) {
    return getUnionFields(o, ty)[0].tag | 0;
}

function makeUnionCaseReader(_unionCase, ty, o) {
    return getUnionFields(o, ty)[1];
}

function makeTupleReader(_ty, o) {
    return o.slice();
}

function getUnionCaseName(_ty, caseStyle, unionCase) {
    if (caseStyle == null) {
        return name(unionCase);
    }
    else {
        return convertCase(CaseStrategy.DotNetPascalCase, caseStyle, name(unionCase));
    }
}

/**
 * The encoder for a type, built by walking it with reflection.
 */
export function generateEncoder(caseStyle, existingEncoders, skipNullField, losslessOption, ty) {
    const matchValue = tryFind(TypeKeyModule_ofType(ty), existingEncoders);
    if (matchValue == null) {
        const gen = (ty_1) => generateEncoder(caseStyle, existingEncoders, skipNullField, losslessOption, ty_1);
        let matchResult, innerType, innerType_1, innerType_2, valueType;
        if (ActivePatterns_$007CUnitType$007C_$007C(ty) != null) {
            matchResult = 0;
        }
        else if (ActivePatterns_$007CIntType$007C_$007C(ty) != null) {
            matchResult = 1;
        }
        else if (ActivePatterns_$007CCharType$007C_$007C(ty) != null) {
            matchResult = 2;
        }
        else if (ActivePatterns_$007CStringType$007C_$007C(ty) != null) {
            matchResult = 3;
        }
        else if (ActivePatterns_$007CBoolType$007C_$007C(ty) != null) {
            matchResult = 4;
        }
        else if (ActivePatterns_$007CByteType$007C_$007C(ty) != null) {
            matchResult = 5;
        }
        else if (ActivePatterns_$007CSByteType$007C_$007C(ty) != null) {
            matchResult = 6;
        }
        else if (ActivePatterns_$007CUInt16Type$007C_$007C(ty) != null) {
            matchResult = 7;
        }
        else if (ActivePatterns_$007CInt16Type$007C_$007C(ty) != null) {
            matchResult = 8;
        }
        else if (ActivePatterns_$007CInt64Type$007C_$007C(ty) != null) {
            matchResult = 9;
        }
        else if (ActivePatterns_$007CUIntType$007C_$007C(ty) != null) {
            matchResult = 10;
        }
        else if (ActivePatterns_$007CUInt64Type$007C_$007C(ty) != null) {
            matchResult = 11;
        }
        else if (ActivePatterns_$007CBigIntType$007C_$007C(ty) != null) {
            matchResult = 12;
        }
        else if (ActivePatterns_$007CSingleType$007C_$007C(ty) != null) {
            matchResult = 13;
        }
        else if (ActivePatterns_$007CDoubleType$007C_$007C(ty) != null) {
            matchResult = 14;
        }
        else if (ActivePatterns_$007CDecimalType$007C_$007C(ty) != null) {
            matchResult = 15;
        }
        else if (ActivePatterns_$007CGuidType$007C_$007C(ty) != null) {
            matchResult = 16;
        }
        else if (ActivePatterns_$007CTimeSpanType$007C_$007C(ty) != null) {
            matchResult = 17;
        }
        else if (ActivePatterns_$007CUriType$007C_$007C(ty) != null) {
            matchResult = 18;
        }
        else if (ActivePatterns_$007CDateTimeType$007C_$007C(ty) != null) {
            matchResult = 19;
        }
        else if (ActivePatterns_$007CDateTimeOffsetType$007C_$007C(ty) != null) {
            matchResult = 20;
        }
        else {
            const activePatternResult_21 = ActivePatterns_$007COptionType$007C_$007C(ty);
            if (activePatternResult_21 != null) {
                matchResult = 21;
                innerType = activePatternResult_21;
            }
            else {
                const activePatternResult_22 = ActivePatterns_$007CSeqType$007C_$007C(ty);
                if (activePatternResult_22 != null) {
                    matchResult = 22;
                    innerType_1 = activePatternResult_22;
                }
                else {
                    const activePatternResult_23 = ActivePatterns_$007CListType$007C_$007C(ty);
                    if (activePatternResult_23 != null) {
                        matchResult = 23;
                        innerType_2 = activePatternResult_23;
                    }
                    else {
                        const activePatternResult_24 = ActivePatterns_$007CMapType$007C_$007C(ty);
                        if (activePatternResult_24 != null) {
                            if (ActivePatterns_$007CStringType$007C_$007C(activePatternResult_24[0]) != null) {
                                matchResult = 24;
                                valueType = activePatternResult_24[1];
                            }
                            else {
                                matchResult = 25;
                            }
                        }
                        else {
                            matchResult = 25;
                        }
                    }
                }
            }
        }
        switch (matchResult) {
            case 0:
                return () => ({
                    Encode(helpers) {
                        return helpers.encodeNull();
                    },
                });
            case 1:
                return (value) => ({
                    Encode(helpers_1) {
                        return helpers_1.encodeSignedIntegralNumber(value);
                    },
                });
            case 2:
                return (value_2) => ({
                    Encode(helpers_2) {
                        return helpers_2.encodeChar(value_2);
                    },
                });
            case 3:
                return (value_4) => ({
                    Encode(helpers_3) {
                        return helpers_3.encodeString(value_4);
                    },
                });
            case 4:
                return (value_6) => ({
                    Encode(helpers_4) {
                        return helpers_4.encodeBool(value_6);
                    },
                });
            case 5:
                return (value_8) => ({
                    Encode(helpers_5) {
                        return helpers_5.encodeUnsignedIntegralNumber(value_8);
                    },
                });
            case 6:
                return (value_10) => ({
                    Encode(helpers_6) {
                        return helpers_6.encodeSignedIntegralNumber(value_10);
                    },
                });
            case 7:
                return (value_12) => ({
                    Encode(helpers_7) {
                        return helpers_7.encodeUnsignedIntegralNumber(value_12);
                    },
                });
            case 8:
                return (value_14) => ({
                    Encode(helpers_8) {
                        return helpers_8.encodeSignedIntegralNumber(value_14);
                    },
                });
            case 9:
                return (value_16) => {
                    const value_1_1 = String(value_16);
                    return {
                        Encode(helpers_9) {
                            return helpers_9.encodeString(value_1_1);
                        },
                    };
                };
            case 10:
                return (value_19) => ({
                    Encode(helpers_10) {
                        return helpers_10.encodeUnsignedIntegralNumber(value_19);
                    },
                });
            case 11:
                return (value_21) => {
                    const value_1_2 = String(value_21);
                    return {
                        Encode(helpers_11) {
                            return helpers_11.encodeString(value_1_2);
                        },
                    };
                };
            case 12:
                return (value_24) => {
                    const value_1_3 = toString(value_24);
                    return {
                        Encode(helpers_12) {
                            return helpers_12.encodeString(value_1_3);
                        },
                    };
                };
            case 13:
                return float32;
            case 14:
                return float;
            case 15:
                return (value_29) => {
                    const value_1_4 = value_29.toString();
                    return {
                        Encode(helpers_13) {
                            return helpers_13.encodeString(value_1_4);
                        },
                    };
                };
            case 16:
                return (g) => {
                    let value_1_5;
                    let copyOfStruct = g;
                    value_1_5 = copyOfStruct;
                    return {
                        Encode(helpers_14) {
                            return helpers_14.encodeString(value_1_5);
                        },
                    };
                };
            case 17:
                return (ts) => {
                    let value_1_6;
                    let copyOfStruct_1 = ts;
                    value_1_6 = toString_1(copyOfStruct_1);
                    return {
                        Encode(helpers_15) {
                            return helpers_15.encodeString(value_1_6);
                        },
                    };
                };
            case 18:
                return (u) => {
                    const value_1_7 = u.originalString;
                    return {
                        Encode(helpers_16) {
                            return helpers_16.encodeString(value_1_7);
                        },
                    };
                };
            case 19:
                return (value_38) => {
                    const value_1_8 = toString_2(value_38, "O", {});
                    return {
                        Encode(helpers_17) {
                            return helpers_17.encodeString(value_1_8);
                        },
                    };
                };
            case 20:
                return (value_41) => {
                    const value_1_9 = toString_2(value_41, "O", {});
                    return {
                        Encode(helpers_18) {
                            return helpers_18.encodeString(value_1_9);
                        },
                    };
                };
            case 21:
                return Encode_Generic_optionOf(losslessOption, innerType, gen(innerType));
            case 22:
                return Encode_Generic_seqOf(innerType_1, gen(innerType_1));
            case 23:
                return Encode_Generic_listOf(innerType_2, gen(innerType_2));
            case 24:
                return Encode_Generic_mapOf(string_type, valueType, (s) => s, gen(valueType));
            default: {
                let matchResult_1, valueType_1;
                const activePatternResult_26 = ActivePatterns_$007CMapType$007C_$007C(ty);
                if (activePatternResult_26 != null) {
                    if (ActivePatterns_$007CGuidType$007C_$007C(activePatternResult_26[0]) != null) {
                        matchResult_1 = 0;
                        valueType_1 = activePatternResult_26[1];
                    }
                    else {
                        matchResult_1 = 1;
                    }
                }
                else {
                    matchResult_1 = 1;
                }
                switch (matchResult_1) {
                    case 0:
                        return Encode_Generic_mapOf(class_type("System.Guid"), valueType_1, (g_1) => g_1, gen(valueType_1));
                    default: {
                        let matchResult_2, keyType, valueType_2, innerType_3, innerType_4;
                        const activePatternResult_28 = ActivePatterns_$007CMapType$007C_$007C(ty);
                        if (activePatternResult_28 != null) {
                            matchResult_2 = 0;
                            keyType = activePatternResult_28[0];
                            valueType_2 = activePatternResult_28[1];
                        }
                        else {
                            const activePatternResult_29 = ActivePatterns_$007CSetType$007C_$007C(ty);
                            if (activePatternResult_29 != null) {
                                matchResult_2 = 1;
                                innerType_3 = activePatternResult_29;
                            }
                            else {
                                const activePatternResult_30 = ActivePatterns_$007CArrayType$007C_$007C(ty);
                                if (activePatternResult_30 != null) {
                                    matchResult_2 = 2;
                                    innerType_4 = activePatternResult_30;
                                }
                                else if (ActivePatterns_$007CFSharpRecordType$007C_$007C(ty) != null) {
                                    matchResult_2 = 3;
                                }
                                else if (ActivePatterns_$007CFSharpUnionType$007C_$007C(ty) != null) {
                                    matchResult_2 = 4;
                                }
                                else if (ActivePatterns_$007CFSharpTupleType$007C_$007C(ty) != null) {
                                    matchResult_2 = 5;
                                }
                                else {
                                    const activePatternResult_34 = ActivePatterns_$007CEnumType$007C_$007C(ty);
                                    if (activePatternResult_34 != null) {
                                        if (ActivePatterns_$007CByteType$007C_$007C(activePatternResult_34) != null) {
                                            matchResult_2 = 6;
                                        }
                                        else {
                                            matchResult_2 = 7;
                                        }
                                    }
                                    else {
                                        matchResult_2 = 7;
                                    }
                                }
                            }
                        }
                        switch (matchResult_2) {
                            case 0:
                                return Encode_Generic_mapAsArrayOf(keyType, valueType_2, gen(keyType), gen(valueType_2));
                            case 1:
                                return Encode_Generic_setOf(innerType_3, gen(innerType_3));
                            case 2:
                                return Encode_Generic_arrayOf(innerType_4, gen(innerType_4));
                            case 3:
                                return generateEncoderForRecord(caseStyle, existingEncoders, skipNullField, losslessOption, ty);
                            case 4:
                                return generateEncoderForUnion(caseStyle, existingEncoders, skipNullField, losslessOption, ty);
                            case 5:
                                return generateEncoderForTuple(caseStyle, existingEncoders, skipNullField, losslessOption, ty);
                            case 6:
                                return Encode_Generic_Enum_byte(ty);
                            default: {
                                let matchResult_3;
                                const activePatternResult_36 = ActivePatterns_$007CEnumType$007C_$007C(ty);
                                if (activePatternResult_36 != null) {
                                    if (ActivePatterns_$007CSByteType$007C_$007C(activePatternResult_36) != null) {
                                        matchResult_3 = 0;
                                    }
                                    else {
                                        matchResult_3 = 1;
                                    }
                                }
                                else {
                                    matchResult_3 = 1;
                                }
                                switch (matchResult_3) {
                                    case 0:
                                        return Encode_Generic_Enum_sbyte(ty);
                                    default: {
                                        let matchResult_4;
                                        const activePatternResult_38 = ActivePatterns_$007CEnumType$007C_$007C(ty);
                                        if (activePatternResult_38 != null) {
                                            if (ActivePatterns_$007CInt16Type$007C_$007C(activePatternResult_38) != null) {
                                                matchResult_4 = 0;
                                            }
                                            else {
                                                matchResult_4 = 1;
                                            }
                                        }
                                        else {
                                            matchResult_4 = 1;
                                        }
                                        switch (matchResult_4) {
                                            case 0:
                                                return Encode_Generic_Enum_int16(ty);
                                            default: {
                                                let matchResult_5;
                                                const activePatternResult_40 = ActivePatterns_$007CEnumType$007C_$007C(ty);
                                                if (activePatternResult_40 != null) {
                                                    if (ActivePatterns_$007CUInt16Type$007C_$007C(activePatternResult_40) != null) {
                                                        matchResult_5 = 0;
                                                    }
                                                    else {
                                                        matchResult_5 = 1;
                                                    }
                                                }
                                                else {
                                                    matchResult_5 = 1;
                                                }
                                                switch (matchResult_5) {
                                                    case 0:
                                                        return Encode_Generic_Enum_uint16(ty);
                                                    default: {
                                                        let matchResult_6;
                                                        const activePatternResult_42 = ActivePatterns_$007CEnumType$007C_$007C(ty);
                                                        if (activePatternResult_42 != null) {
                                                            if (ActivePatterns_$007CIntType$007C_$007C(activePatternResult_42) != null) {
                                                                matchResult_6 = 0;
                                                            }
                                                            else {
                                                                matchResult_6 = 1;
                                                            }
                                                        }
                                                        else {
                                                            matchResult_6 = 1;
                                                        }
                                                        switch (matchResult_6) {
                                                            case 0:
                                                                return Encode_Generic_Enum_int(ty);
                                                            default: {
                                                                let matchResult_7;
                                                                const activePatternResult_44 = ActivePatterns_$007CEnumType$007C_$007C(ty);
                                                                if (activePatternResult_44 != null) {
                                                                    if (ActivePatterns_$007CUIntType$007C_$007C(activePatternResult_44) != null) {
                                                                        matchResult_7 = 0;
                                                                    }
                                                                    else {
                                                                        matchResult_7 = 1;
                                                                    }
                                                                }
                                                                else {
                                                                    matchResult_7 = 1;
                                                                }
                                                                switch (matchResult_7) {
                                                                    case 0:
                                                                        return Encode_Generic_Enum_uint32(ty);
                                                                    default:
                                                                        throw new Exception(concat("Encoder generation failed, unsupported type \'", fullName(ty), "\'"));
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    else {
        return value_44(matchValue);
    }
}

export function generateEncoderForRecord(caseStyle, existingEncoders, skipNullField, losslessOption, ty) {
    let self = defaultOf();
    let existingEncoders_1;
    if (Type_isRecursive(ty)) {
        const lazySelf = makeLazyEncoder(ty, () => self);
        existingEncoders_1 = add(TypeKeyModule_ofType(ty), lazySelf, existingEncoders);
    }
    else {
        existingEncoders_1 = existingEncoders;
    }
    const recordFieldsWithEncoders = toArray(delay(() => {
        let activePatternResult, fields;
        return collect((pi) => {
            const fieldEncoder = wrapBoxedEncoder(generateEncoder(caseStyle, existingEncoders_1, skipNullField, losslessOption, pi[1]));
            return singleton([name(pi), (record_1) => {
                const value = makeFieldReader(pi, record_1);
                if (skipNullField && Operators_IsNull(value)) {
                    return undefined;
                }
                else {
                    return fieldEncoder(value);
                }
            }]);
        }, (activePatternResult = ActivePatterns_$007CFSharpRecordType$007C_$007C(ty), (activePatternResult != null) ? ((fields = activePatternResult, fields)) : (() => {
            throw new Exception("Expected an F# record type");
        })()));
    }));
    const encoder_1 = wrapFinalEncoder(ty, (o) => {
        const fields_1 = toArray(delay(() => collect((matchValue) => {
            const fieldName = matchValue[0];
            const encodedFieldName = (caseStyle == null) ? fieldName : convertCase(CaseStrategy.DotNetPascalCase, caseStyle, fieldName);
            const matchValue_1 = matchValue[1](o);
            if (matchValue_1 == null) {
                return empty();
            }
            else {
                return singleton([encodedFieldName, matchValue_1]);
            }
        }, recordFieldsWithEncoders)));
        return {
            Encode(helpers) {
                const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers)], fields_1);
                return helpers.encodeObject(arg);
            },
        };
    });
    self = encoder_1;
    return encoder_1;
}

export function generateEncoderForUnion(caseStyle, existingEncoders, skipNullField, losslessOption, ty) {
    let self = defaultOf();
    let existingEncoders_1;
    if (Type_isRecursive(ty)) {
        const lazySelf = makeLazyEncoder(ty, () => self);
        existingEncoders_1 = add(TypeKeyModule_ofType(ty), lazySelf, existingEncoders);
    }
    else {
        existingEncoders_1 = existingEncoders;
    }
    let unionCases;
    const activePatternResult = ActivePatterns_$007CFSharpUnionType$007C_$007C(ty);
    if (activePatternResult != null) {
        const cases = activePatternResult;
        unionCases = cases;
    }
    else {
        throw new Exception(concat("Expected an F# union type but found ", fullName(ty)));
    }
    const caseEncoders = toArray(delay(() => collect((unionCase) => {
        const encodedUnionCaseName = getUnionCaseName(ty, caseStyle, unionCase);
        if (!isEmpty(getUnionCaseFields(unionCase))) {
            const fieldEncoders = toArray(delay(() => map((pi) => wrapBoxedEncoder(generateEncoder(caseStyle, existingEncoders_1, skipNullField, losslessOption, pi[1])), getUnionCaseFields(unionCase))));
            const n = (fieldEncoders.length - 1) | 0;
            return singleton((o_2) => {
                const values = makeUnionCaseReader(unionCase, ty, o_2);
                const values_1 = toArray(delay(() => append(singleton({
                    Encode(helpers) {
                        return helpers.encodeString(encodedUnionCaseName);
                    },
                }), delay(() => map((i) => item(i, fieldEncoders)(item(i, values)), rangeDouble(0, 1, n))))));
                return {
                    Encode(helpers_1) {
                        const arg = map_1((v) => v.Encode(helpers_1), values_1);
                        return helpers_1.encodeArray(arg);
                    },
                };
            });
        }
        else {
            return singleton((_arg) => ({
                Encode(helpers_2) {
                    return helpers_2.encodeString(encodedUnionCaseName);
                },
            }));
        }
    }, unionCases)));
    const encoder_1 = wrapFinalEncoder(ty, (o_3) => item(makeUnionTagReader(ty, o_3), caseEncoders)(o_3));
    self = encoder_1;
    return encoder_1;
}

export function generateEncoderForTuple(caseStyle, existingEncoders, skipNullField, losslessOption, ty) {
    const encoders = toArray(delay(() => map((elementType) => wrapBoxedEncoder(generateEncoder(caseStyle, existingEncoders, skipNullField, losslessOption, elementType)), getTupleElements(ty))));
    const n = (encoders.length - 1) | 0;
    return wrapFinalEncoder(ty, (o_1) => {
        let values_1;
        const values = makeTupleReader(ty, o_1);
        return (values_1 = toArray(delay(() => map((i) => item(i, encoders)(item(i, values)), rangeDouble(0, 1, n)))), {
            Encode(helpers) {
                const arg = map_1((v) => v.Encode(helpers), values_1);
                return helpers.encodeArray(arg);
            },
        });
    });
}

/**
 * Encoders generated from F# types.
 */
export class Auto {
    constructor() {
    }
}

export function Auto_$reflection() {
    return class_type("Thoth.Json.Core.Auto.Encode.Auto", undefined, Auto);
}

(() => {
    Auto.instance = Cache$2_$ctor();
})();

