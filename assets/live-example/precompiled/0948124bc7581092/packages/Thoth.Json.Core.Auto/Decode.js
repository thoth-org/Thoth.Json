
import { makeTuple, getTupleElements, string_type, makeUnion, getUnionCaseFields, makeRecord, option_type, unit_type, name as name_4, tuple_type, getTupleField, fullName, isEnumDefined, obj_type, makeGenericType, class_type } from "fable-library-js/Reflection.js";
import { lazily, datetimeOffset, datetimeUtc, uri, timespan, guid, float, float32, uint16, bool, char, string, unit, uint32, int, int16, sbyte, byte, oneOf, map2, andThen, fail, succeed, index as index_1, optional, field as field_1, map$0027, dict, map, array as array_1, list, losslessOption as losslessOption_1, lossyOption } from "../Thoth.Json.Core/Decode.js";
import { toList, reduce, singleton, collect, delay, toArray, ofList } from "fable-library-js/Seq.js";
import { ofList as ofList_1 } from "fable-library-js/Set.js";
import { defaultOf, Exception, compare } from "fable-library-js/Util.js";
import { concat } from "fable-library-js/String.js";
import { ErrorReason$1 } from "../Thoth.Json.Core/Types.js";
import { FSharpResult$2 } from "fable-library-js/Result.js";
import { ofArray } from "fable-library-js/List.js";
import { indexed, setItem, fill } from "fable-library-js/Array.js";
import { Type_isRecursive, ActivePatterns_$007CSetType$007C_$007C, ActivePatterns_$007CSeqType$007C_$007C, ActivePatterns_$007CArrayType$007C_$007C, ActivePatterns_$007CListType$007C_$007C, ActivePatterns_$007COptionType$007C_$007C, ActivePatterns_$007CDateTimeOffsetType$007C_$007C, ActivePatterns_$007CDateTimeType$007C_$007C, ActivePatterns_$007CUriType$007C_$007C, ActivePatterns_$007CTimeSpanType$007C_$007C, ActivePatterns_$007CGuidType$007C_$007C, ActivePatterns_$007CFSharpTupleType$007C_$007C, ActivePatterns_$007CFSharpUnionType$007C_$007C, ActivePatterns_$007CFSharpRecordType$007C_$007C, ActivePatterns_$007CMapType$007C_$007C, ActivePatterns_$007CEnumType$007C_$007C, ActivePatterns_$007CDoubleType$007C_$007C, ActivePatterns_$007CSingleType$007C_$007C, ActivePatterns_$007CUIntType$007C_$007C, ActivePatterns_$007CInt16Type$007C_$007C, ActivePatterns_$007CUInt16Type$007C_$007C, ActivePatterns_$007CSByteType$007C_$007C, ActivePatterns_$007CByteType$007C_$007C, ActivePatterns_$007CBoolType$007C_$007C, ActivePatterns_$007CIntType$007C_$007C, ActivePatterns_$007CCharType$007C_$007C, ActivePatterns_$007CStringType$007C_$007C, ActivePatterns_$007CUnitType$007C_$007C, Lazy_makeGeneric } from "./Prelude.js";
import { add, tryFind } from "fable-library-js/Map.js";
import { Cache$2_$ctor, CaseStrategy, TypeKeyModule_ofType } from "./Domain.js";
import { defaultArg, value as value_10 } from "fable-library-js/Option.js";
import { convertCase } from "./Casing.js";

class Helpers_DecodeHelpers {
    constructor() {
    }
}

function Helpers_DecodeHelpers_$reflection() {
    return class_type("Thoth.Json.Core.Auto.Decode.Helpers.DecodeHelpers", undefined, Helpers_DecodeHelpers);
}

function Helpers_DecodeHelpers_Option_1FBE35A8(x) {
    return lossyOption(x);
}

function Helpers_DecodeHelpers_OptionLossless_1FBE35A8(x) {
    return losslessOption_1(x);
}

function Helpers_DecodeHelpers_List_1FBE35A8(x) {
    return list(x);
}

function Helpers_DecodeHelpers_Array_1FBE35A8(x) {
    return array_1(x);
}

function Helpers_DecodeHelpers_Seq_1FBE35A8(x) {
    return map(ofList, list(x));
}

function Helpers_DecodeHelpers_Set_14E49357(x) {
    return map((elements) => ofList_1(elements, {
        Compare: (x_1, y) => (compare(x_1, y) | 0),
    }), list(x));
}

function Helpers_DecodeHelpers_Dict_1FBE35A8(x) {
    return dict(x);
}

function Helpers_DecodeHelpers_MapAsArray_Z51373461(keyDecoder, valueDecoder) {
    return map$0027(keyDecoder, valueDecoder);
}

function Helpers_DecodeHelpers_Field_55ED3633(name, x) {
    return field_1(name, x);
}

function Helpers_DecodeHelpers_Optional_55ED3633(name, x) {
    return optional(name, x);
}

function Helpers_DecodeHelpers_Index_7ACC4474(index, x) {
    return index_1(index, x);
}

function Helpers_DecodeHelpers_Succeed_1505(x) {
    return succeed(x);
}

function Helpers_DecodeHelpers_Fail_Z721C83C5(x) {
    return fail(x);
}

function Helpers_DecodeHelpers_Error_Z721C83C5(typeName) {
    return {
        Decode(helpers, value) {
            return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadType */ 2, [concat("an extra coder for ", typeName), value])]]);
        },
    };
}

function Helpers_DecodeHelpers_Bind_54E8D813(f, x) {
    return andThen(f, x);
}

function Helpers_DecodeHelpers_Map_1A12FD3E(f, x) {
    return map(f, x);
}

function Helpers_DecodeHelpers_Zip_83ADF00(x, y) {
    return map2((x_1, y_1) => [x_1, y_1], x, y);
}

function Helpers_DecodeHelpers_Either_83ADF00(x, y) {
    return oneOf(ofArray([x, y]));
}

function Helpers_makeDecoderType(ty) {
    return makeGenericType(class_type("Thoth.Json.Core.Decoder`1", [obj_type]), [ty]);
}

function Helpers_Decode_Generic_error(innerType, typeName) {
    return {
        Decode(helpers, value) {
            return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadType */ 2, [concat("an extra coder for ", typeName), value])]]);
        },
    };
}

function Helpers_Decode_Generic_Enum_checkEnumValue(innerType, value) {
    if (isEnumDefined(innerType, value)) {
        return succeed(value);
    }
    else {
        return {
            Decode(_arg, value_1) {
                return new FSharpResult$2(/* Error */ 1, [["", new ErrorReason$1(/* BadPrimitiveExtra */ 1, [fullName(innerType), value_1, "Unknown value provided for the enum"])]]);
            },
        };
    }
}

function Helpers_Decode_Generic_Enum_byte(innerType) {
    return andThen((value) => Helpers_Decode_Generic_Enum_checkEnumValue(innerType, value), byte);
}

function Helpers_Decode_Generic_Enum_sbyte(innerType) {
    return andThen((value) => Helpers_Decode_Generic_Enum_checkEnumValue(innerType, value), sbyte);
}

function Helpers_Decode_Generic_Enum_int16(innerType) {
    return andThen((value) => Helpers_Decode_Generic_Enum_checkEnumValue(innerType, value), int16);
}

function Helpers_Decode_Generic_Enum_uint16(innerType) {
    return andThen((value) => Helpers_Decode_Generic_Enum_checkEnumValue(innerType, value), int16);
}

function Helpers_Decode_Generic_Enum_int(innerType) {
    return andThen((value) => Helpers_Decode_Generic_Enum_checkEnumValue(innerType, value), int);
}

function Helpers_Decode_Generic_Enum_uint32(innerType) {
    return andThen((value) => Helpers_Decode_Generic_Enum_checkEnumValue(innerType, value), uint32);
}

function Helpers_getNestedTupleFields(tuple, length) {
    if (length === 1) {
        return [tuple];
    }
    else {
        const result = fill(new Array(length), 0, length, null);
        let x = tuple;
        let i = length - 1;
        while (i > 0) {
            setItem(result, i, getTupleField(x, 1));
            i = ((i - 1) | 0);
            if (i === 0) {
                setItem(result, i, getTupleField(x, 0));
            }
            else {
                x = getTupleField(x, 0);
            }
        }
        return result;
    }
}

function makeLazySelf(ty, getSelf) {
    return Lazy_makeGeneric(Helpers_makeDecoderType(ty), (_arg) => getSelf());
}

function makeMappingFunc(_fromType, _toType, funcImpl) {
    return funcImpl;
}

function mergeDecoders(state_, state__1, next_, next__1) {
    const state = [state_, state__1];
    const next = [next_, next__1];
    return [tuple_type(state[0], next[0]), map2((x, y) => [x, y], state[1], next[1])];
}

/**
 * The decoder for a type, built by walking it with reflection.
 */
export function generateDecoder(caseStyle, existingDecoders, isOptional, losslessOption, ty) {
    const matchValue = tryFind(TypeKeyModule_ofType(ty), existingDecoders);
    if (matchValue == null) {
        const gen = (ty_1) => generateDecoder(caseStyle, existingDecoders, false, losslessOption, ty_1);
        if (ActivePatterns_$007CUnitType$007C_$007C(ty) != null) {
            return unit;
        }
        else if (ActivePatterns_$007CStringType$007C_$007C(ty) != null) {
            return string;
        }
        else if (ActivePatterns_$007CCharType$007C_$007C(ty) != null) {
            return char;
        }
        else if (ActivePatterns_$007CIntType$007C_$007C(ty) != null) {
            return int;
        }
        else if (ActivePatterns_$007CBoolType$007C_$007C(ty) != null) {
            return bool;
        }
        else if (ActivePatterns_$007CByteType$007C_$007C(ty) != null) {
            return byte;
        }
        else if (ActivePatterns_$007CSByteType$007C_$007C(ty) != null) {
            return sbyte;
        }
        else if (ActivePatterns_$007CUInt16Type$007C_$007C(ty) != null) {
            return uint16;
        }
        else if (ActivePatterns_$007CInt16Type$007C_$007C(ty) != null) {
            return int16;
        }
        else if (ActivePatterns_$007CUIntType$007C_$007C(ty) != null) {
            return uint32;
        }
        else if (ActivePatterns_$007CSingleType$007C_$007C(ty) != null) {
            return float32;
        }
        else if (ActivePatterns_$007CDoubleType$007C_$007C(ty) != null) {
            return float;
        }
        else {
            let matchResult, inner, inner_1, inner_2, inner_3, inner_4, valueType;
            if (ActivePatterns_$007CGuidType$007C_$007C(ty) != null) {
                matchResult = 0;
            }
            else if (ActivePatterns_$007CTimeSpanType$007C_$007C(ty) != null) {
                matchResult = 1;
            }
            else if (ActivePatterns_$007CUriType$007C_$007C(ty) != null) {
                matchResult = 2;
            }
            else if (ActivePatterns_$007CDateTimeType$007C_$007C(ty) != null) {
                matchResult = 3;
            }
            else if (ActivePatterns_$007CDateTimeOffsetType$007C_$007C(ty) != null) {
                matchResult = 4;
            }
            else {
                const activePatternResult_17 = ActivePatterns_$007COptionType$007C_$007C(ty);
                if (activePatternResult_17 != null) {
                    matchResult = 5;
                    inner = activePatternResult_17;
                }
                else {
                    const activePatternResult_18 = ActivePatterns_$007CListType$007C_$007C(ty);
                    if (activePatternResult_18 != null) {
                        matchResult = 6;
                        inner_1 = activePatternResult_18;
                    }
                    else {
                        const activePatternResult_19 = ActivePatterns_$007CArrayType$007C_$007C(ty);
                        if (activePatternResult_19 != null) {
                            matchResult = 7;
                            inner_2 = activePatternResult_19;
                        }
                        else {
                            const activePatternResult_20 = ActivePatterns_$007CSeqType$007C_$007C(ty);
                            if (activePatternResult_20 != null) {
                                matchResult = 8;
                                inner_3 = activePatternResult_20;
                            }
                            else {
                                const activePatternResult_21 = ActivePatterns_$007CSetType$007C_$007C(ty);
                                if (activePatternResult_21 != null) {
                                    matchResult = 9;
                                    inner_4 = activePatternResult_21;
                                }
                                else {
                                    const activePatternResult_22 = ActivePatterns_$007CMapType$007C_$007C(ty);
                                    if (activePatternResult_22 != null) {
                                        if (ActivePatterns_$007CStringType$007C_$007C(activePatternResult_22[0]) != null) {
                                            matchResult = 10;
                                            valueType = activePatternResult_22[1];
                                        }
                                        else {
                                            matchResult = 11;
                                        }
                                    }
                                    else {
                                        matchResult = 11;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            switch (matchResult) {
                case 0:
                    return guid;
                case 1:
                    return timespan;
                case 2:
                    return uri;
                case 3:
                    return datetimeUtc;
                case 4:
                    return datetimeOffset;
                case 5: {
                    const decoder = generateDecoder(caseStyle, existingDecoders, true, losslessOption, inner);
                    if (losslessOption) {
                        return losslessOption_1(decoder);
                    }
                    else {
                        return lossyOption(decoder);
                    }
                }
                case 6:
                    return list(gen(inner_1));
                case 7:
                    return array_1(gen(inner_2));
                case 8:
                    return list(gen(inner_3));
                case 9:
                    return map((elements) => ofList_1(elements, {
                        Compare: (x_1, y) => (compare(x_1, y) | 0),
                    }), list(gen(inner_4)));
                case 10:
                    return dict(gen(valueType));
                default: {
                    let matchResult_1, keyType, valueType_1;
                    const activePatternResult_24 = ActivePatterns_$007CMapType$007C_$007C(ty);
                    if (activePatternResult_24 != null) {
                        matchResult_1 = 0;
                        keyType = activePatternResult_24[0];
                        valueType_1 = activePatternResult_24[1];
                    }
                    else if (ActivePatterns_$007CFSharpRecordType$007C_$007C(ty) != null) {
                        matchResult_1 = 1;
                    }
                    else if (ActivePatterns_$007CFSharpUnionType$007C_$007C(ty) != null) {
                        matchResult_1 = 2;
                    }
                    else if (ActivePatterns_$007CFSharpTupleType$007C_$007C(ty) != null) {
                        matchResult_1 = 3;
                    }
                    else {
                        const activePatternResult_28 = ActivePatterns_$007CEnumType$007C_$007C(ty);
                        if (activePatternResult_28 != null) {
                            if (ActivePatterns_$007CByteType$007C_$007C(activePatternResult_28) != null) {
                                matchResult_1 = 4;
                            }
                            else {
                                matchResult_1 = 5;
                            }
                        }
                        else {
                            matchResult_1 = 5;
                        }
                    }
                    switch (matchResult_1) {
                        case 0:
                            return map$0027(gen(keyType), gen(valueType_1));
                        case 1:
                            return genericRecordDecoder(caseStyle, existingDecoders, losslessOption, ty);
                        case 2:
                            return genericUnionDecoder(caseStyle, existingDecoders, losslessOption, ty);
                        case 3:
                            return genericTupleDecoder(caseStyle, existingDecoders, losslessOption, ty);
                        case 4:
                            return Helpers_Decode_Generic_Enum_byte(ty);
                        default: {
                            let matchResult_2;
                            const activePatternResult_30 = ActivePatterns_$007CEnumType$007C_$007C(ty);
                            if (activePatternResult_30 != null) {
                                if (ActivePatterns_$007CSByteType$007C_$007C(activePatternResult_30) != null) {
                                    matchResult_2 = 0;
                                }
                                else {
                                    matchResult_2 = 1;
                                }
                            }
                            else {
                                matchResult_2 = 1;
                            }
                            switch (matchResult_2) {
                                case 0:
                                    return Helpers_Decode_Generic_Enum_sbyte(ty);
                                default: {
                                    let matchResult_3;
                                    const activePatternResult_32 = ActivePatterns_$007CEnumType$007C_$007C(ty);
                                    if (activePatternResult_32 != null) {
                                        if (ActivePatterns_$007CInt16Type$007C_$007C(activePatternResult_32) != null) {
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
                                            return Helpers_Decode_Generic_Enum_int16(ty);
                                        default: {
                                            let matchResult_4;
                                            const activePatternResult_34 = ActivePatterns_$007CEnumType$007C_$007C(ty);
                                            if (activePatternResult_34 != null) {
                                                if (ActivePatterns_$007CUInt16Type$007C_$007C(activePatternResult_34) != null) {
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
                                                    return Helpers_Decode_Generic_Enum_uint16(ty);
                                                default: {
                                                    let matchResult_5;
                                                    const activePatternResult_36 = ActivePatterns_$007CEnumType$007C_$007C(ty);
                                                    if (activePatternResult_36 != null) {
                                                        if (ActivePatterns_$007CIntType$007C_$007C(activePatternResult_36) != null) {
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
                                                            return Helpers_Decode_Generic_Enum_int(ty);
                                                        default: {
                                                            let matchResult_6;
                                                            const activePatternResult_38 = ActivePatterns_$007CEnumType$007C_$007C(ty);
                                                            if (activePatternResult_38 != null) {
                                                                if (ActivePatterns_$007CUIntType$007C_$007C(activePatternResult_38) != null) {
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
                                                                    return Helpers_Decode_Generic_Enum_uint32(ty);
                                                                default: {
                                                                    const unknown = ty;
                                                                    if (isOptional) {
                                                                        return Helpers_Decode_Generic_error(unknown, fullName(unknown));
                                                                    }
                                                                    else {
                                                                        throw new Exception(concat("Cannot generate auto decoder for \'", fullName(unknown), "\'. Please pass an extra decoder.\n\nDocumentation available at: https://thoth-org.github.io/Thoth.Json/documentation/auto/extra-coders/#ready-to-use-extra-coders"));
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
    }
    else {
        return value_10(matchValue);
    }
}

function genericRecordDecoder(caseStyle, existingDecoders, losslessOption, ty) {
    let self = defaultOf();
    let existingDecoders_1;
    if (Type_isRecursive(ty)) {
        const lazySelf = lazily(makeLazySelf(ty, () => self));
        existingDecoders_1 = add(TypeKeyModule_ofType(ty), lazySelf, existingDecoders);
    }
    else {
        existingDecoders_1 = existingDecoders;
    }
    let recordFields;
    const activePatternResult = ActivePatterns_$007CFSharpRecordType$007C_$007C(ty);
    if (activePatternResult != null) {
        const fields = activePatternResult;
        recordFields = fields;
    }
    else {
        throw new Exception("Expected an F# record type");
    }
    let patternInput;
    const array = toArray(delay(() => collect((field) => {
        const encodedFieldName = (caseStyle == null) ? name_4(field) : convertCase(CaseStrategy.DotNetPascalCase, caseStyle, name_4(field));
        let decoder_6;
        const matchValue = field[1];
        if (ActivePatterns_$007CUnitType$007C_$007C(matchValue) != null) {
            let decoder_1;
            field[1];
            decoder_1 = optional(encodedFieldName, generateDecoder(caseStyle, existingDecoders_1, false, losslessOption, field[1]));
            decoder_6 = map(makeMappingFunc(option_type(unit_type), unit_type, (o) => {
                defaultArg(o, undefined);
                return undefined;
            }), decoder_1);
        }
        else {
            const activePatternResult_2 = ActivePatterns_$007COptionType$007C_$007C(matchValue);
            if (activePatternResult_2 != null) {
                const innerType_2 = activePatternResult_2;
                if (losslessOption) {
                    field[1];
                    decoder_6 = oneOf(ofArray([(field[1], field_1(encodedFieldName, generateDecoder(caseStyle, existingDecoders_1, false, losslessOption, field[1]))), (field[1], succeed(undefined))]));
                }
                else {
                    decoder_6 = optional(encodedFieldName, generateDecoder(caseStyle, existingDecoders_1, true, losslessOption, innerType_2));
                }
            }
            else {
                field[1];
                decoder_6 = field_1(encodedFieldName, generateDecoder(caseStyle, existingDecoders_1, false, losslessOption, field[1]));
            }
        }
        return singleton([field[1], decoder_6]);
    }, recordFields)));
    patternInput = array.reduce((tupledArg, tupledArg_1) => mergeDecoders(tupledArg[0], tupledArg[1], tupledArg_1[0], tupledArg_1[1]));
    const decoder_9 = map(makeMappingFunc(patternInput[0], ty, (x_2) => makeRecord(ty, Helpers_getNestedTupleFields(x_2, recordFields.length))), patternInput[1]);
    self = decoder_9;
    return decoder_9;
}

function genericUnionDecoder(caseStyle, existingDecoders, losslessOption, ty) {
    let self = defaultOf();
    let existingDecoders_1;
    if (Type_isRecursive(ty)) {
        const lazySelf = lazily(makeLazySelf(ty, () => self));
        existingDecoders_1 = add(TypeKeyModule_ofType(ty), lazySelf, existingDecoders);
    }
    else {
        existingDecoders_1 = existingDecoders;
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
    const decoder_7 = reduce((decoderA, decoderB) => oneOf(ofArray([decoderA, decoderB])), toList(delay(() => collect((case$) => {
        const caseFields = getUnionCaseFields(case$);
        if (caseFields.length === 0) {
            const caseObject = makeUnion(case$, []);
            return singleton(andThen(makeMappingFunc(string_type, Helpers_makeDecoderType(ty), (x_1) => {
                const x_2 = x_1;
                if (x_2 === name_4(case$)) {
                    return succeed(caseObject);
                }
                else {
                    return fail(`Expected ${name_4(case$)} but found "${x_2}"`);
                }
            }), string));
        }
        else {
            let patternInput;
            const array = toArray(delay(() => collect((matchValue) => {
                const field = matchValue[1];
                let decoder_2;
                field[1];
                decoder_2 = index_1(matchValue[0] + 1, generateDecoder(caseStyle, existingDecoders_1, false, losslessOption, field[1]));
                return singleton([field[1], decoder_2]);
            }, indexed(caseFields))));
            patternInput = array.reduce((tupledArg, tupledArg_1) => mergeDecoders(tupledArg[0], tupledArg[1], tupledArg_1[0], tupledArg_1[1]));
            const tupleToUnionCase = makeMappingFunc(patternInput[0], ty, (x_5) => makeUnion(case$, Helpers_getNestedTupleFields(x_5, caseFields.length)));
            const prefix = andThen((x_6) => {
                if (x_6 === name_4(case$)) {
                    return succeed(undefined);
                }
                else {
                    return fail(`Expected ${name_4(case$)} but found "${x_6}"`);
                }
            }, index_1(0, string));
            const dec = map(tupleToUnionCase, patternInput[1]);
            return singleton(andThen(makeMappingFunc(unit_type, Helpers_makeDecoderType(ty), (_arg) => dec), prefix));
        }
    }, unionCases))));
    self = decoder_7;
    return decoder_7;
}

function genericTupleDecoder(caseStyle, existingDecoders, losslessOption, ty) {
    const elements = getTupleElements(ty);
    let patternInput;
    const array = toArray(delay(() => collect((matchValue) => {
        const elementType = matchValue[1];
        return singleton([elementType, index_1(matchValue[0], generateDecoder(caseStyle, existingDecoders, false, losslessOption, elementType))]);
    }, indexed(elements))));
    patternInput = array.reduce((tupledArg, tupledArg_1) => mergeDecoders(tupledArg[0], tupledArg[1], tupledArg_1[0], tupledArg_1[1]));
    return map(makeMappingFunc(patternInput[0], ty, (x) => makeTuple(Helpers_getNestedTupleFields(x, elements.length), ty)), patternInput[1]);
}

/**
 * Decoders generated from F# types.
 */
export class Auto {
    constructor() {
    }
}

export function Auto_$reflection() {
    return class_type("Thoth.Json.Core.Auto.Decode.Auto", undefined, Auto);
}

(() => {
    Auto.instance = Cache$2_$ctor();
})();

