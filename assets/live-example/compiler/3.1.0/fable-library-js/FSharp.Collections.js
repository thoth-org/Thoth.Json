import { compare, physicalHash, equals as equals_1, structuralHash } from "./Util.js";
export function HashIdentity_FromFunctions(hasher, equals) {
    return {
        GetHashCode(x) {
            return hasher(x) | 0;
        },
        Equals(x_1, y) {
            return (x_1 == null) ? (y == null) : ((y == null) ? false : equals(x_1, y));
        },
    };
}
export function HashIdentity_Structural() {
    return {
        GetHashCode(x) {
            return structuralHash(x) | 0;
        },
        Equals(x_1, y) {
            return equals_1(x_1, y);
        },
    };
}
export function HashIdentity_Reference() {
    return {
        GetHashCode(x) {
            return physicalHash(x) | 0;
        },
        Equals(x_1, y) {
            return equals_1(x_1, y);
        },
    };
}
export function ComparisonIdentity_FromFunction(comparer) {
    return {
        Compare(x, y) {
            return ((x == null) ? ((y == null) ? 0 : -1) : ((y == null) ? 1 : comparer(x, y))) | 0;
        },
    };
}
export function ComparisonIdentity_Structural() {
    return {
        Compare(x, y) {
            return compare(x, y) | 0;
        },
    };
}
