namespace Thoth.Json.Fable

open Fable.Core
open Fable.Core.JsInterop

type JsonValue = obj

module Helpers =
    [<Emit("typeof $0")>]
    let jsTypeof (_ : JsonValue) : string = jsNative

    [<Emit("$0 instanceof SyntaxError")>]
    let isSyntaxError (_ : JsonValue) : bool = jsNative

    let inline getField (fieldName: string) (o: JsonValue) = o?(fieldName)
    let inline isString (o: JsonValue) : bool = o :? string

    let inline isBoolean (o: JsonValue) : bool = o :? bool

    let inline isNumber (o: JsonValue) : bool = jsTypeof o = "number"

    let inline isArray (o: JsonValue) : bool = JS.Constructors.Array.isArray(o)

    [<Emit("$0 === null ? false : (Object.getPrototypeOf($0 || false) === Object.prototype)")>]
    let isObject (_ : JsonValue) : bool = jsNative

    let inline isNaN (o: JsonValue) : bool = JS.Constructors.Number.isNaN(!!o)

    let inline isNullValue (o: JsonValue): bool = isNull o

    /// is the value an integer? This returns false for 1.1, NaN, Infinite, ...
    [<Emit("isFinite($0) && Math.floor($0) === $0")>]
    let isIntegralValue (_: JsonValue) : bool = jsNative

    [<Emit("$1 <= $0 && $0 < $2")>]
    let isBetweenInclusive(_v: JsonValue, _min: obj, _max: obj) = jsNative

    [<Emit("isFinite($0) && !($0 % 1)")>]
    let isIntFinite (_: JsonValue) : bool = jsNative

    let isUndefined (o: JsonValue): bool = jsTypeof o = "undefined"

    [<Emit("JSON.stringify($0, null, 4) + ''")>]
    let anyToString (_: JsonValue) : string = jsNative

    let inline isFunction (o: JsonValue) : bool = jsTypeof o = "function"

    let inline objectKeys (o: JsonValue) : string seq = upcast JS.Constructors.Object.keys(o)
    let inline asBool (o: JsonValue): bool = unbox o
    let inline asInt (o: JsonValue): int = unbox o
    let inline asFloat (o: JsonValue): float = unbox o
    let inline asFloat32 (o: JsonValue): float32 = unbox o
    let inline asString (o: JsonValue): string = unbox o
    let inline asArray (o: JsonValue): JsonValue[] = unbox o

    [<Emit("Array.from($0)")>]
    let arrayFrom(x: JsonValue seq): JsonValue = jsNative
