import { disposeSafe, defaultOf, Exception, structuralHash, equals } from "./Util.js";
import { HashIdentity_Structural, ComparisonIdentity_Structural } from "./FSharp.Collections.js";
import { nonNullValue } from "./Option.js";
import { NullReferenceException_$ctor, ArgumentNullException_$ctor_Z721C83C5 } from "./System.js";
import { FSharpChoice$2_Choice2Of2, FSharpChoice$2_Choice1Of2 } from "./Choice.js";
import { StringBuilder__Append_Z721C83C5 } from "./System.Text.js";
export const LanguagePrimitives_GenericEqualityComparer = {
    Equals(x, y) {
        return equals(x, y);
    },
    GetHashCode(x_1) {
        return structuralHash(x_1) | 0;
    },
};
export const LanguagePrimitives_GenericEqualityERComparer = {
    Equals(x, y) {
        return equals(x, y);
    },
    GetHashCode(x_1) {
        return structuralHash(x_1) | 0;
    },
};
export function LanguagePrimitives_FastGenericComparer() {
    return ComparisonIdentity_Structural();
}
export function LanguagePrimitives_FastGenericComparerFromTable() {
    return ComparisonIdentity_Structural();
}
export function LanguagePrimitives_FastGenericEqualityComparer() {
    return HashIdentity_Structural();
}
export function LanguagePrimitives_FastGenericEqualityComparerFromTable() {
    return HashIdentity_Structural();
}
export function Operators_Failure(message) {
    return new Exception(message);
}
export function Operators_FailurePattern(exn) {
    return exn.message;
}
export function Operators_NullArg(argumentName) {
    throw ArgumentNullException_$ctor_Z721C83C5(argumentName);
}
export function Operators_Using(resource, action) {
    try {
        return action(resource);
    }
    finally {
        if (equals(resource, defaultOf())) {
        }
        else {
            let copyOfStruct = resource;
            disposeSafe(copyOfStruct);
        }
    }
}
export function Operators_Lock(_lockObj, action) {
    return action();
}
export function Operators_IsNull(value) {
    if (equals(value, defaultOf())) {
        return true;
    }
    else {
        return false;
    }
}
export function Operators_IsNotNull(value) {
    if (equals(value, defaultOf())) {
        return false;
    }
    else {
        return true;
    }
}
export function Operators_IsNullV(value) {
    return !(value != null);
}
export function Operators_NonNull(value) {
    if (equals(value, defaultOf())) {
        throw NullReferenceException_$ctor();
    }
    else {
        return value;
    }
}
export function Operators_NonNullV(value) {
    if (value != null) {
        return nonNullValue(value);
    }
    else {
        throw NullReferenceException_$ctor();
    }
}
export function Operators_NullMatchPattern(value) {
    if (equals(value, defaultOf())) {
        return FSharpChoice$2_Choice1Of2(undefined);
    }
    else {
        return FSharpChoice$2_Choice2Of2(value);
    }
}
export function Operators_NullValueMatchPattern(value) {
    if (value != null) {
        return FSharpChoice$2_Choice2Of2(nonNullValue(value));
    }
    else {
        return FSharpChoice$2_Choice1Of2(undefined);
    }
}
export function Operators_NonNullQuickPattern(value) {
    if (equals(value, defaultOf())) {
        throw NullReferenceException_$ctor();
    }
    else {
        return value;
    }
}
export function Operators_NonNullQuickValuePattern(value) {
    if (value != null) {
        return nonNullValue(value);
    }
    else {
        throw NullReferenceException_$ctor();
    }
}
export function Operators_WithNull(value) {
    return value;
}
export function Operators_WithNullV(value) {
    return value;
}
export function Operators_NullV() {
    return null;
}
export function Operators_NullArgCheck(argumentName, value) {
    if (equals(value, defaultOf())) {
        throw ArgumentNullException_$ctor_Z721C83C5(argumentName);
    }
    else {
        return value;
    }
}
export function ExtraTopLevelOperators_LazyPattern(input) {
    return input.Value;
}
export function PrintfModule_PrintFormatToStringBuilderThen(continuation, builder, format) {
    return format.cont((s) => {
        StringBuilder__Append_Z721C83C5(builder, s);
        return continuation();
    });
}
export function PrintfModule_PrintFormatToStringBuilder(builder, format) {
    return PrintfModule_PrintFormatToStringBuilderThen(() => {
    }, builder, format);
}
