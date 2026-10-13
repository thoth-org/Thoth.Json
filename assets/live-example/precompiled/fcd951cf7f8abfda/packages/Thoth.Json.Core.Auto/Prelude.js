
import { getUnionCaseFields, getEnumUnderlyingType, isEnum, getTupleElements, isTuple, isUnion, getRecordElements, isRecord, getElementType, isArray, class_type, getGenerics, option_type, getGenericTypeDefinition, equals, isGenericType, fullName, makeUnion, name, list_type, obj_type, makeGenericType, getUnionCases } from "fable-library-js/Reflection.js";
import { filter, exists, singleton, collect, append, delay, reverse, fold, find } from "fable-library-js/Seq.js";
import { ofSeq } from "fable-library-js/Map.js";
import { comparePrimitives, structuralHash, Lazy, compare } from "fable-library-js/Util.js";
import { some } from "fable-library-js/Option.js";
import { collect as collect_1, head, map, contains, item } from "fable-library-js/Array.js";
import { empty as empty_1, add, contains as contains_1, ofArray, union } from "fable-library-js/Set.js";

export function Microsoft_FSharp_Reflection_FSharpValue__FSharpValue_MakeList_Static_Z38FAE0DE(elementType, elements) {
    const ucis = getUnionCases(makeGenericType(list_type(obj_type), [elementType]));
    const emptyUci = find((uci) => (name(uci) === "Empty"), ucis);
    const consUci = find((uci_1) => (name(uci_1) === "Cons"), ucis);
    return fold((acc, x) => makeUnion(consUci, [x, acc]), makeUnion(emptyUci, []), reverse(elements));
}

export function Map_merge(a, b) {
    if (a.Equals(b)) {
        return a;
    }
    else {
        return ofSeq(delay(() => append(collect((matchValue) => {
            const activePatternResult = matchValue;
            return singleton([activePatternResult[0], activePatternResult[1]]);
        }, a), delay(() => collect((matchValue_1) => {
            const activePatternResult_1 = matchValue_1;
            return singleton([activePatternResult_1[0], activePatternResult_1[1]]);
        }, b)))), {
            Compare: (x, y) => (compare(x, y) | 0),
        });
    }
}

export function Lazy_makeGeneric(ty, func) {
    return new Lazy(func);
}

export function ActivePatterns_$007CUnitType$007C_$007C(ty) {
    if (fullName(ty) === "Microsoft.FSharp.Core.Unit") {
        return some(undefined);
    }
    else {
        return undefined;
    }
}

export function ActivePatterns_$007CStringType$007C_$007C(ty) {
    if (fullName(ty) === "System.String") {
        return some(undefined);
    }
    else {
        return undefined;
    }
}

export function ActivePatterns_$007CCharType$007C_$007C(ty) {
    if (fullName(ty) === "System.Char") {
        return some(undefined);
    }
    else {
        return undefined;
    }
}

export function ActivePatterns_$007CIntType$007C_$007C(ty) {
    if (fullName(ty) === "System.Int32") {
        return some(undefined);
    }
    else {
        return undefined;
    }
}

export function ActivePatterns_$007CBoolType$007C_$007C(ty) {
    if (fullName(ty) === "System.Boolean") {
        return some(undefined);
    }
    else {
        return undefined;
    }
}

export function ActivePatterns_$007CInt64Type$007C_$007C(ty) {
    if (fullName(ty) === "System.Int64") {
        return some(undefined);
    }
    else {
        return undefined;
    }
}

export function ActivePatterns_$007CSingleType$007C_$007C(ty) {
    if (fullName(ty) === "System.Single") {
        return some(undefined);
    }
    else {
        return undefined;
    }
}

export function ActivePatterns_$007CDoubleType$007C_$007C(ty) {
    if (fullName(ty) === "System.Double") {
        return some(undefined);
    }
    else {
        return undefined;
    }
}

export function ActivePatterns_$007CDecimalType$007C_$007C(ty) {
    if (fullName(ty) === "System.Decimal") {
        return some(undefined);
    }
    else {
        return undefined;
    }
}

export function ActivePatterns_$007CByteType$007C_$007C(ty) {
    if (fullName(ty) === "System.Byte") {
        return some(undefined);
    }
    else {
        return undefined;
    }
}

export function ActivePatterns_$007CSByteType$007C_$007C(ty) {
    if (fullName(ty) === "System.SByte") {
        return some(undefined);
    }
    else {
        return undefined;
    }
}

export function ActivePatterns_$007CUInt16Type$007C_$007C(ty) {
    if (fullName(ty) === "System.UInt16") {
        return some(undefined);
    }
    else {
        return undefined;
    }
}

export function ActivePatterns_$007CInt16Type$007C_$007C(ty) {
    if (fullName(ty) === "System.Int16") {
        return some(undefined);
    }
    else {
        return undefined;
    }
}

export function ActivePatterns_$007CUIntType$007C_$007C(ty) {
    if (fullName(ty) === "System.UInt32") {
        return some(undefined);
    }
    else {
        return undefined;
    }
}

export function ActivePatterns_$007CUInt64Type$007C_$007C(ty) {
    if (fullName(ty) === "System.UInt64") {
        return some(undefined);
    }
    else {
        return undefined;
    }
}

export function ActivePatterns_$007CBigIntType$007C_$007C(ty) {
    if (fullName(ty) === "System.Numerics.BigInteger") {
        return some(undefined);
    }
    else {
        return undefined;
    }
}

export function ActivePatterns_$007CGuidType$007C_$007C(ty) {
    if (fullName(ty) === "System.Guid") {
        return some(undefined);
    }
    else {
        return undefined;
    }
}

export function ActivePatterns_$007CTimeSpanType$007C_$007C(ty) {
    if (fullName(ty) === "System.TimeSpan") {
        return some(undefined);
    }
    else {
        return undefined;
    }
}

export function ActivePatterns_$007CUriType$007C_$007C(ty) {
    if (fullName(ty) === "System.Uri") {
        return some(undefined);
    }
    else {
        return undefined;
    }
}

export function ActivePatterns_$007CDateTimeType$007C_$007C(ty) {
    if (fullName(ty) === "System.DateTime") {
        return some(undefined);
    }
    else {
        return undefined;
    }
}

export function ActivePatterns_$007CDateTimeOffsetType$007C_$007C(ty) {
    if (fullName(ty) === "System.DateTimeOffset") {
        return some(undefined);
    }
    else {
        return undefined;
    }
}

export function ActivePatterns_$007COptionType$007C_$007C(ty) {
    if (isGenericType(ty) && equals(getGenericTypeDefinition(ty), option_type(obj_type))) {
        return item(0, getGenerics(ty));
    }
    else {
        return undefined;
    }
}

export function ActivePatterns_$007CListType$007C_$007C(ty) {
    if (isGenericType(ty) && equals(getGenericTypeDefinition(ty), list_type(obj_type))) {
        return item(0, getGenerics(ty));
    }
    else {
        return undefined;
    }
}

export function ActivePatterns_$007CMapType$007C_$007C(ty) {
    if (isGenericType(ty) && equals(getGenericTypeDefinition(ty), class_type("Microsoft.FSharp.Collections.FSharpMap`2", [obj_type, obj_type]))) {
        return [item(0, getGenerics(ty)), item(1, getGenerics(ty))];
    }
    else {
        return undefined;
    }
}

export function ActivePatterns_$007CArrayType$007C_$007C(ty) {
    if (isArray(ty)) {
        return getElementType(ty);
    }
    else {
        return undefined;
    }
}

export function ActivePatterns_$007CSeqType$007C_$007C(ty) {
    if (isGenericType(ty) && equals(getGenericTypeDefinition(ty), class_type("System.Collections.Generic.IEnumerable`1", [obj_type]))) {
        return item(0, getGenerics(ty));
    }
    else {
        return undefined;
    }
}

export function ActivePatterns_$007CSetType$007C_$007C(ty) {
    if (isGenericType(ty) && equals(getGenericTypeDefinition(ty), class_type("Microsoft.FSharp.Collections.FSharpSet`1", [obj_type]))) {
        return item(0, getGenerics(ty));
    }
    else {
        return undefined;
    }
}

export function ActivePatterns_$007CFSharpRecordType$007C_$007C(ty) {
    if (isRecord(ty)) {
        return getRecordElements(ty);
    }
    else {
        return undefined;
    }
}

export function ActivePatterns_$007CFSharpUnionType$007C_$007C(ty) {
    if (isUnion(ty)) {
        return getUnionCases(ty);
    }
    else {
        return undefined;
    }
}

export function ActivePatterns_$007CFSharpTupleType$007C_$007C(ty) {
    if (isTuple(ty)) {
        return getTupleElements(ty);
    }
    else {
        return undefined;
    }
}

export function ActivePatterns_$007CEnumType$007C_$007C(ty) {
    if (isEnum(ty)) {
        return getEnumUnderlyingType(ty);
    }
    else {
        return undefined;
    }
}

export function Type_isRecursive(ty) {
    const loop = (seen_mut, current_mut) => {
        loop:
        while (true) {
            const seen = seen_mut, current = current_mut;
            let current_1;
            const activePatternResult = ActivePatterns_$007CFSharpTupleType$007C_$007C(current);
            if (activePatternResult != null) {
                const elementTypes = activePatternResult;
                if (contains(ty, elementTypes, {
                    Equals: equals,
                    GetHashCode: (x) => (structuralHash(x) | 0),
                })) {
                    return true;
                }
                else {
                    const seenNext = union(seen, ofArray(map(fullName, elementTypes), {
                        Compare: (x_1, y_1) => (comparePrimitives(x_1, y_1) | 0),
                    }));
                    return exists((ty_3) => loop(seenNext, ty_3), filter((ty_2) => !contains_1(fullName(ty_2), seen), elementTypes));
                }
            }
            else if ((current_1 = current, isGenericType(current_1) && equals(getGenericTypeDefinition(current_1), list_type(obj_type)))) {
                const elementType = head(getGenerics(current));
                if (equals(elementType, ty)) {
                    return true;
                }
                else {
                    seen_mut = add(fullName(elementType), seen);
                    current_mut = elementType;
                    continue loop;
                }
            }
            else {
                const activePatternResult_1 = ActivePatterns_$007CFSharpRecordType$007C_$007C(current);
                if (activePatternResult_1 != null) {
                    const fields = activePatternResult_1;
                    const fieldTypes = map((pi) => pi[1], fields);
                    if (contains(ty, fieldTypes, {
                        Equals: equals,
                        GetHashCode: (x_2) => (structuralHash(x_2) | 0),
                    })) {
                        return true;
                    }
                    else {
                        const seenNext_2 = union(seen, ofArray(map(fullName, fieldTypes), {
                            Compare: (x_3, y_3) => (comparePrimitives(x_3, y_3) | 0),
                        }));
                        return exists((ty_6) => loop(seenNext_2, ty_6), filter((ty_5) => !contains_1(fullName(ty_5), seen), fieldTypes));
                    }
                }
                else {
                    const activePatternResult_2 = ActivePatterns_$007CFSharpUnionType$007C_$007C(current);
                    if (activePatternResult_2 != null) {
                        const fields_1 = activePatternResult_2;
                        const fieldTypes_1 = map((pi_1) => pi_1[1], collect_1(getUnionCaseFields, fields_1));
                        if (contains(ty, fieldTypes_1, {
                            Equals: equals,
                            GetHashCode: (x_4) => (structuralHash(x_4) | 0),
                        })) {
                            return true;
                        }
                        else {
                            const seenNext_3 = union(seen, ofArray(map(fullName, fieldTypes_1), {
                                Compare: (x_5, y_5) => (comparePrimitives(x_5, y_5) | 0),
                            }));
                            return exists((ty_9) => loop(seenNext_3, ty_9), filter((ty_8) => !contains_1(fullName(ty_8), seen), fieldTypes_1));
                        }
                    }
                    else {
                        return false;
                    }
                }
            }
            break;
        }
    };
    return loop(empty_1({
        Compare: (x_6, y_6) => (comparePrimitives(x_6, y_6) | 0),
    }), ty);
}

