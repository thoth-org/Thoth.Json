/**
 * F# Quotation runtime support for Fable JS/TS target.
 *
 * Provides class-based representations of F# quotation AST nodes
 * and pattern matching helpers compatible with Fable's option convention
 * (undefined = no match, value/tuple = match).
 */
import { ofArray } from "./List.js";
// ===================================================================
// Var: represents an F# quotation variable
// ===================================================================
export class Var {
    constructor(name, type_, isMutable) {
        this.Name = name;
        this.Type = type_;
        this.IsMutable = isMutable;
    }
}
export function mkQuotVar(name, type_, isMutable = false) {
    return new Var(name, type_, isMutable);
}
export function varGetName(v) { return v.Name; }
export function varGetType(v) { return v.Type; }
export function varGetIsMutable(v) { return v.IsMutable; }
// ===================================================================
// Expr nodes: tagged classes for each quotation expression kind
// ===================================================================
export class ExprValue {
    constructor(value, type) {
        this.tag = "Value";
        this.value = value;
        this.type = type;
    }
    toJSON() { return ["Value", this.value, this.type]; }
}
export class ExprVarExpr {
    constructor(var_) {
        this.tag = "Var";
        this.var_ = var_;
    }
    toJSON() { return ["Var", this.var_]; }
}
export class ExprLambda {
    constructor(var_, body) {
        this.tag = "Lambda";
        this.var_ = var_;
        this.body = body;
    }
    toJSON() { return ["Lambda", this.var_, this.body]; }
}
export class ExprApplication {
    constructor(func, arg) {
        this.tag = "Application";
        this.func = func;
        this.arg = arg;
    }
    toJSON() { return ["Application", this.func, this.arg]; }
}
export class ExprLet {
    constructor(var_, value, body) {
        this.tag = "Let";
        this.var_ = var_;
        this.value = value;
        this.body = body;
    }
    toJSON() { return ["Let", this.var_, this.value, this.body]; }
}
export class ExprIfThenElse {
    constructor(guard, thenExpr, elseExpr) {
        this.tag = "IfThenElse";
        this.guard = guard;
        this.thenExpr = thenExpr;
        this.elseExpr = elseExpr;
    }
    toJSON() { return ["IfThenElse", this.guard, this.thenExpr, this.elseExpr]; }
}
export class ExprCall {
    constructor(instance, method, args, declaringType = "") {
        this.tag = "Call";
        this.instance = instance;
        this.method = method;
        this.args = args;
        this.declaringType = declaringType;
    }
    toJSON() { return ["Call", this.instance, this.method, this.args]; }
}
export class ExprSequential {
    constructor(first, second) {
        this.tag = "Sequential";
        this.first = first;
        this.second = second;
    }
    toJSON() { return ["Sequential", this.first, this.second]; }
}
export class ExprNewTuple {
    constructor(elements) {
        this.tag = "NewTuple";
        this.elements = elements;
    }
    toJSON() { return ["NewTuple", this.elements]; }
}
export class ExprNewUnion {
    constructor(typeName, unionTag, fields) {
        this.tag = "NewUnion";
        this.typeName = typeName;
        this.unionTag = unionTag;
        this.fields = fields;
    }
    toJSON() { return ["NewUnion", this.typeName, this.unionTag, this.fields]; }
}
export class ExprNewRecord {
    constructor(fieldNames, values) {
        this.tag = "NewRecord";
        this.fieldNames = fieldNames;
        this.values = values;
    }
    toJSON() { return ["NewRecord", this.fieldNames, this.values]; }
}
export class ExprNewList {
    constructor(head, tail) {
        this.tag = "NewList";
        this.head = head;
        this.tail = tail;
    }
    toJSON() { return ["NewList", this.head, this.tail]; }
}
export class ExprTupleGet {
    constructor(expr, index) {
        this.tag = "TupleGet";
        this.expr = expr;
        this.index = index;
    }
    toJSON() { return ["TupleGet", this.expr, this.index]; }
}
export class ExprUnionTag {
    constructor(expr) {
        this.tag = "UnionTag";
        this.expr = expr;
    }
    toJSON() { return ["UnionTag", this.expr]; }
}
export class ExprUnionField {
    constructor(expr, fieldIndex) {
        this.tag = "UnionField";
        this.expr = expr;
        this.fieldIndex = fieldIndex;
    }
    toJSON() { return ["UnionField", this.expr, this.fieldIndex]; }
}
export class ExprFieldGet {
    constructor(expr, fieldName) {
        this.tag = "FieldGet";
        this.expr = expr;
        this.fieldName = fieldName;
    }
    toJSON() { return ["FieldGet", this.expr, this.fieldName]; }
}
export class ExprFieldSet {
    constructor(expr, fieldName, value) {
        this.tag = "FieldSet";
        this.expr = expr;
        this.fieldName = fieldName;
        this.value = value;
    }
    toJSON() { return ["FieldSet", this.expr, this.fieldName, this.value]; }
}
export class ExprVarSet {
    constructor(target, value) {
        this.tag = "VarSet";
        this.target = target;
        this.value = value;
    }
    toJSON() { return ["VarSet", this.target, this.value]; }
}
// ===================================================================
// Constructors (called by QuotationEmitter.fs)
// ===================================================================
export function mkValue(value, type) {
    return new ExprValue(value, type);
}
export function mkNull(type) {
    // A null-value node (no instance / null literal / empty option or list).
    return new ExprValue(null, type);
}
export function mkVar(v) {
    return new ExprVarExpr(v);
}
export function mkLambda(v, body) {
    return new ExprLambda(v, body);
}
export function mkApplication(func, arg) {
    return new ExprApplication(func, arg);
}
export function mkLet(v, value, body) {
    return new ExprLet(v, value, body);
}
export function mkIfThenElse(guard, thenExpr, elseExpr) {
    return new ExprIfThenElse(guard, thenExpr, elseExpr);
}
export function mkCall(instance, method, args, declaringType = "") {
    return new ExprCall(instance, method, args, declaringType);
}
export function mkSequential(first, second) {
    return new ExprSequential(first, second);
}
export function mkNewTuple(elements) {
    return new ExprNewTuple(elements);
}
export function mkTupleGet(expr, index) {
    return new ExprTupleGet(expr, index);
}
export function mkUnionTag(expr) {
    return new ExprUnionTag(expr);
}
export function mkUnionField(expr, fieldIndex) {
    return new ExprUnionField(expr, fieldIndex);
}
export function mkFieldGet(expr, fieldName) {
    return new ExprFieldGet(expr, fieldName);
}
export function mkFieldSet(expr, fieldName, value) {
    return new ExprFieldSet(expr, fieldName, value);
}
export function mkVarSet(target, value) {
    return new ExprVarSet(target, value);
}
export function mkNewUnion(typeName, tag, fields) {
    return new ExprNewUnion(typeName, tag, fields);
}
export function mkNewRecord(fieldNames, values) {
    return new ExprNewRecord(fieldNames, values);
}
export function mkNewList(head, tail) {
    return new ExprNewList(head, tail);
}
// ===================================================================
// Accessors
// ===================================================================
export function getType(expr) {
    if (expr instanceof ExprValue)
        return expr.type;
    if (expr instanceof ExprLambda)
        return expr.var_.Type;
    return "obj";
}
// ===================================================================
// Pattern match helpers
// Returns undefined (no match) or a value/tuple (match), following
// Fable's option convention for active patterns.
// ===================================================================
export function isValue(expr) {
    if (expr instanceof ExprValue)
        return [expr.value, expr.type];
    return undefined;
}
export function isVar(expr) {
    if (expr instanceof ExprVarExpr)
        return expr.var_;
    return undefined;
}
export function isLambda(expr) {
    if (expr instanceof ExprLambda)
        return [expr.var_, expr.body];
    return undefined;
}
export function isApplication(expr) {
    if (expr instanceof ExprApplication)
        return [expr.func, expr.arg];
    return undefined;
}
export function isLet(expr) {
    if (expr instanceof ExprLet)
        return [expr.var_, expr.value, expr.body];
    return undefined;
}
export function isIfThenElse(expr) {
    if (expr instanceof ExprIfThenElse)
        return [expr.guard, expr.thenExpr, expr.elseExpr];
    return undefined;
}
export function isCall(expr) {
    if (expr instanceof ExprCall) {
        // "novalue" is the "no instance" sentinel, distinct from a genuine quoted null ("null").
        const instance = (expr.instance instanceof ExprValue && expr.instance.type === "novalue") ? null : expr.instance;
        // Must be a real FSharpList (not the raw array), same reasoning as isFieldGet below.
        return [instance, expr.method, ofArray(expr.args)];
    }
    return undefined;
}
export function callDeclaringType(expr) {
    return expr instanceof ExprCall ? expr.declaringType : "";
}
export function isSequential(expr) {
    if (expr instanceof ExprSequential)
        return [expr.first, expr.second];
    return undefined;
}
export function isNewTuple(expr) {
    if (expr instanceof ExprNewTuple)
        return expr.elements;
    return undefined;
}
export function isNewUnionCase(expr) {
    if (expr instanceof ExprNewUnion)
        return [expr.typeName, expr.fields];
    return undefined;
}
export function isNewRecord(expr) {
    if (expr instanceof ExprNewRecord)
        return [expr.fieldNames, expr.values];
    return undefined;
}
export function isTupleGet(expr) {
    if (expr instanceof ExprTupleGet)
        return [expr.expr, expr.index];
    return undefined;
}
export function isFieldGet(expr) {
    // Third element mirrors PropertyGet's indexer-args slot, always empty here (never indexed).
    // "novalue" is the "no instance" sentinel, distinct from a genuine quoted null ("null").
    if (expr instanceof ExprFieldGet) {
        const instance = (expr.expr instanceof ExprValue && expr.expr.type === "novalue") ? null : expr.expr;
        return [instance, expr.fieldName, []];
    }
    return undefined;
}
// ===================================================================
// Evaluation
// ===================================================================
const OPERATORS = {
    "op_Addition": (a, b) => a + b,
    "op_Subtraction": (a, b) => a - b,
    "op_Multiply": (a, b) => a * b,
    "op_Division": (a, b) => a / b,
    "op_Modulus": (a, b) => a % b,
    "op_Exponentiation": (a, b) => a ** b,
    "op_UnaryNegation": (a) => -a,
    "op_UnaryPlus": (a) => +a,
    "op_LogicalNot": (a) => !a,
    "op_BitwiseOr": (a, b) => a | b,
    "op_BitwiseAnd": (a, b) => a & b,
    "op_ExclusiveOr": (a, b) => a ^ b,
    "op_LeftShift": (a, b) => a << b,
    "op_RightShift": (a, b) => a >> b,
    "op_Equality": (a, b) => a === b,
    "op_Inequality": (a, b) => a !== b,
    "op_LessThan": (a, b) => a < b,
    "op_LessThanOrEqual": (a, b) => a <= b,
    "op_GreaterThan": (a, b) => a > b,
    "op_GreaterThanOrEqual": (a, b) => a >= b,
    "op_BooleanAnd": (a, b) => a && b,
    "op_BooleanOr": (a, b) => a || b,
};
export function evaluate(expr, env) {
    if (env == null)
        env = new Map();
    if (expr instanceof ExprValue)
        return expr.value;
    if (expr instanceof ExprVarExpr) {
        if (env.has(expr.var_.Name))
            return env.get(expr.var_.Name);
        throw new Error(`Unbound variable: ${expr.var_.Name}`);
    }
    if (expr instanceof ExprLambda) {
        const capturedEnv = new Map(env);
        return (arg) => {
            const newEnv = new Map(capturedEnv);
            newEnv.set(expr.var_.Name, arg);
            return evaluate(expr.body, newEnv);
        };
    }
    if (expr instanceof ExprApplication) {
        return evaluate(expr.func, env)(evaluate(expr.arg, env));
    }
    if (expr instanceof ExprLet) {
        const newEnv = new Map(env);
        newEnv.set(expr.var_.Name, evaluate(expr.value, env));
        return evaluate(expr.body, newEnv);
    }
    if (expr instanceof ExprIfThenElse) {
        return evaluate(expr.guard, env) ? evaluate(expr.thenExpr, env) : evaluate(expr.elseExpr, env);
    }
    if (expr instanceof ExprSequential) {
        evaluate(expr.first, env);
        return evaluate(expr.second, env);
    }
    if (expr instanceof ExprNewTuple) {
        return expr.elements.map(e => evaluate(e, env));
    }
    if (expr instanceof ExprCall) {
        const evaluatedArgs = expr.args.map((a) => evaluate(a, env));
        if (expr.method in OPERATORS)
            return OPERATORS[expr.method](...evaluatedArgs);
        throw new Error(`Unknown method: ${expr.method}`);
    }
    if (expr instanceof ExprTupleGet) {
        return evaluate(expr.expr, env)[expr.index];
    }
    if (expr instanceof ExprNewUnion) {
        return [expr.unionTag, ...expr.fields.map((f) => evaluate(f, env))];
    }
    if (expr instanceof ExprNewRecord) {
        const result = {};
        for (let i = 0; i < expr.fieldNames.length; i++) {
            result[expr.fieldNames[i]] = evaluate(expr.values[i], env);
        }
        return result;
    }
    if (expr instanceof ExprNewList) {
        return [evaluate(expr.head, env), ...evaluate(expr.tail, env)];
    }
    if (expr instanceof ExprVarSet) {
        if (expr.target instanceof ExprVarExpr) {
            env.set(expr.target.var_.Name, evaluate(expr.value, env));
            return undefined;
        }
        throw new Error("VarSet target must be a variable");
    }
    if (expr instanceof ExprFieldGet) {
        const obj = evaluate(expr.expr, env);
        return obj[expr.fieldName];
    }
    throw new Error(`Cannot evaluate expression: ${expr.tag}`);
}
// ===================================================================
// FSharpExpr instance methods
// ===================================================================
const OP_SYMBOLS = {
    "op_Addition": "+",
    "op_Subtraction": "-",
    "op_Multiply": "*",
    "op_Division": "/",
    "op_Modulus": "%",
    "op_Exponentiation": "**",
    "op_Equality": "=",
    "op_Inequality": "<>",
    "op_LessThan": "<",
    "op_LessThanOrEqual": "<=",
    "op_GreaterThan": ">",
    "op_GreaterThanOrEqual": ">=",
    "op_BooleanAnd": "&&",
    "op_BooleanOr": "||",
    "op_UnaryNegation": "-",
    "op_LogicalNot": "not",
};
export function exprToString(expr) {
    if (expr instanceof ExprValue) {
        if (expr.type === "string")
            return `"${expr.value}"`;
        if (expr.type === "unit")
            return "()";
        if (expr.type === "bool")
            return expr.value ? "true" : "false";
        return String(expr.value);
    }
    if (expr instanceof ExprVarExpr)
        return expr.var_.Name;
    if (expr instanceof ExprLambda)
        return `fun ${expr.var_.Name} -> ${exprToString(expr.body)}`;
    if (expr instanceof ExprApplication)
        return `${exprToString(expr.func)} ${exprToString(expr.arg)}`;
    if (expr instanceof ExprLet)
        return `let ${expr.var_.Name} = ${exprToString(expr.value)} in ${exprToString(expr.body)}`;
    if (expr instanceof ExprIfThenElse)
        return `if ${exprToString(expr.guard)} then ${exprToString(expr.thenExpr)} else ${exprToString(expr.elseExpr)}`;
    if (expr instanceof ExprCall) {
        if (expr.method in OP_SYMBOLS && expr.args.length === 2) {
            return `(${exprToString(expr.args[0])} ${OP_SYMBOLS[expr.method]} ${exprToString(expr.args[1])})`;
        }
        if (expr.method in OP_SYMBOLS && expr.args.length === 1) {
            return `${OP_SYMBOLS[expr.method]}${exprToString(expr.args[0])}`;
        }
        return `${expr.method}(${expr.args.map(exprToString).join(", ")})`;
    }
    if (expr instanceof ExprSequential)
        return `${exprToString(expr.first)}; ${exprToString(expr.second)}`;
    if (expr instanceof ExprNewTuple)
        return `(${expr.elements.map(exprToString).join(", ")})`;
    if (expr instanceof ExprTupleGet)
        return `Item${expr.index + 1}(${exprToString(expr.expr)})`;
    if (expr instanceof ExprFieldGet)
        return `${exprToString(expr.expr)}.${expr.fieldName}`;
    return `<${expr.tag}>`;
}
export function getFreeVars(expr) {
    const free = [];
    const seen = new Set();
    function walk(e, bound) {
        if (e instanceof ExprVarExpr) {
            if (!bound.has(e.var_.Name) && !seen.has(e.var_.Name)) {
                free.push(e.var_);
                seen.add(e.var_.Name);
            }
        }
        else if (e instanceof ExprLambda) {
            walk(e.body, new Set([...bound, e.var_.Name]));
        }
        else if (e instanceof ExprLet) {
            walk(e.value, bound);
            walk(e.body, new Set([...bound, e.var_.Name]));
        }
        else if (e instanceof ExprApplication) {
            walk(e.func, bound);
            walk(e.arg, bound);
        }
        else if (e instanceof ExprIfThenElse) {
            walk(e.guard, bound);
            walk(e.thenExpr, bound);
            walk(e.elseExpr, bound);
        }
        else if (e instanceof ExprCall) {
            for (const a of e.args)
                walk(a, bound);
        }
        else if (e instanceof ExprSequential) {
            walk(e.first, bound);
            walk(e.second, bound);
        }
        else if (e instanceof ExprNewTuple) {
            for (const el of e.elements)
                walk(el, bound);
        }
        else if (e instanceof ExprTupleGet) {
            walk(e.expr, bound);
        }
        else if (e instanceof ExprNewUnion) {
            for (const f of e.fields)
                walk(f, bound);
        }
        else if (e instanceof ExprNewRecord) {
            for (const v of e.values)
                walk(v, bound);
        }
        else if (e instanceof ExprNewList) {
            walk(e.head, bound);
            walk(e.tail, bound);
        }
        else if (e instanceof ExprUnionTag) {
            walk(e.expr, bound);
        }
        else if (e instanceof ExprUnionField) {
            walk(e.expr, bound);
        }
        else if (e instanceof ExprFieldGet) {
            walk(e.expr, bound);
        }
        else if (e instanceof ExprFieldSet) {
            walk(e.expr, bound);
            walk(e.value, bound);
        }
        else if (e instanceof ExprVarSet) {
            walk(e.target, bound);
            walk(e.value, bound);
        }
    }
    walk(expr, new Set());
    return free;
}
export function substitute(expr, fn) {
    function sub(e) {
        if (e instanceof ExprVarExpr) {
            const result = fn(e.var_);
            return result !== undefined ? result : e;
        }
        if (e instanceof ExprLambda)
            return new ExprLambda(e.var_, sub(e.body));
        if (e instanceof ExprLet)
            return new ExprLet(e.var_, sub(e.value), sub(e.body));
        if (e instanceof ExprApplication)
            return new ExprApplication(sub(e.func), sub(e.arg));
        if (e instanceof ExprIfThenElse)
            return new ExprIfThenElse(sub(e.guard), sub(e.thenExpr), sub(e.elseExpr));
        if (e instanceof ExprCall) {
            const newInst = e.instance != null ? sub(e.instance) : e.instance;
            return new ExprCall(newInst, e.method, e.args.map(sub), e.declaringType);
        }
        if (e instanceof ExprSequential)
            return new ExprSequential(sub(e.first), sub(e.second));
        if (e instanceof ExprNewTuple)
            return new ExprNewTuple(e.elements.map(sub));
        if (e instanceof ExprTupleGet)
            return new ExprTupleGet(sub(e.expr), e.index);
        if (e instanceof ExprNewUnion)
            return new ExprNewUnion(e.typeName, e.unionTag, e.fields.map(sub));
        if (e instanceof ExprNewRecord)
            return new ExprNewRecord(e.fieldNames, e.values.map(sub));
        if (e instanceof ExprNewList)
            return new ExprNewList(sub(e.head), sub(e.tail));
        if (e instanceof ExprUnionTag)
            return new ExprUnionTag(sub(e.expr));
        if (e instanceof ExprUnionField)
            return new ExprUnionField(sub(e.expr), e.fieldIndex);
        if (e instanceof ExprFieldGet)
            return new ExprFieldGet(sub(e.expr), e.fieldName);
        if (e instanceof ExprFieldSet)
            return new ExprFieldSet(sub(e.expr), e.fieldName, sub(e.value));
        if (e instanceof ExprVarSet)
            return new ExprVarSet(sub(e.target), sub(e.value));
        return e;
    }
    return sub(expr);
}
// ===================================================================
// JSON deserialization
// Reconstructs Expr/Var from the toJSON() array format.
// ===================================================================
function varFromJSON(json) {
    return new Var(json.Name, json.Type, json.IsMutable);
}
export function exprFromJSON(json) {
    if (!Array.isArray(json))
        return new ExprValue(json, typeof json);
    const [tag, ...fields] = json;
    switch (tag) {
        case "Value": return new ExprValue(fields[0], fields[1]);
        case "Var": return new ExprVarExpr(varFromJSON(fields[0]));
        case "Lambda": return new ExprLambda(varFromJSON(fields[0]), exprFromJSON(fields[1]));
        case "Application": return new ExprApplication(exprFromJSON(fields[0]), exprFromJSON(fields[1]));
        case "Let": return new ExprLet(varFromJSON(fields[0]), exprFromJSON(fields[1]), exprFromJSON(fields[2]));
        case "IfThenElse": return new ExprIfThenElse(exprFromJSON(fields[0]), exprFromJSON(fields[1]), exprFromJSON(fields[2]));
        case "Call": return new ExprCall(fields[0] != null ? exprFromJSON(fields[0]) : null, fields[1], fields[2].map(exprFromJSON));
        case "Sequential": return new ExprSequential(exprFromJSON(fields[0]), exprFromJSON(fields[1]));
        case "NewTuple": return new ExprNewTuple(fields[0].map(exprFromJSON));
        case "TupleGet": return new ExprTupleGet(exprFromJSON(fields[0]), fields[1]);
        case "NewUnion": return new ExprNewUnion(fields[0], fields[1], fields[2].map(exprFromJSON));
        case "UnionTag": return new ExprUnionTag(exprFromJSON(fields[0]));
        case "UnionField": return new ExprUnionField(exprFromJSON(fields[0]), fields[1]);
        case "NewRecord": return new ExprNewRecord(fields[0], fields[1].map(exprFromJSON));
        case "FieldGet": return new ExprFieldGet(exprFromJSON(fields[0]), fields[1]);
        case "FieldSet": return new ExprFieldSet(exprFromJSON(fields[0]), fields[1], exprFromJSON(fields[2]));
        case "VarSet": return new ExprVarSet(exprFromJSON(fields[0]), exprFromJSON(fields[1]));
        case "NewList": return new ExprNewList(exprFromJSON(fields[0]), exprFromJSON(fields[1]));
        default: return new ExprValue(json, "unknown");
    }
}
