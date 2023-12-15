namespace Thoth.Json.Core

type IDecoderHelpers<'JsonValue> =
    abstract isString: 'JsonValue -> bool
    abstract isNumber: 'JsonValue -> bool
    abstract isBoolean: 'JsonValue -> bool
    abstract isNullValue: 'JsonValue -> bool
    abstract isArray: 'JsonValue -> bool
    abstract isObject: 'JsonValue -> bool
    abstract hasProperty: string -> 'JsonValue -> bool
    abstract isIntegralValue: 'JsonValue -> bool
    abstract asString: 'JsonValue -> string
    abstract asFloat: 'JsonValue -> float
    abstract asFloat32: 'JsonValue -> float32
    abstract asInt: 'JsonValue -> int
    abstract asBoolean: 'JsonValue -> bool
    abstract asArray: 'JsonValue -> 'JsonValue[]
    abstract getProperty: string * 'JsonValue -> 'JsonValue
    abstract getProperties: 'JsonValue -> string seq
    abstract anyToString: 'JsonValue -> string

type IEncoderHelpers<'JsonValue> =
    abstract encodeString: string -> 'JsonValue
    abstract encodeChar: char -> 'JsonValue
    abstract encodeDecimalNumber: float -> 'JsonValue
    abstract encodeBool: bool -> 'JsonValue
    abstract encodeNull: unit -> 'JsonValue
    abstract createEmptyObject: unit -> 'JsonValue
    abstract setPropertyOnObject: 'JsonValue * string * 'JsonValue -> unit
    abstract encodeArray: 'JsonValue array -> 'JsonValue
    abstract encodeList: 'JsonValue list -> 'JsonValue
    abstract encodeSeq: 'JsonValue seq -> 'JsonValue
    abstract encodeIntegralNumber: uint32 -> 'JsonValue


type ErrorReason<'JsonValue> =
    | BadPrimitive of string * 'JsonValue
    | BadPrimitiveExtra of string * 'JsonValue * string
    | BadType of string * 'JsonValue
    | BadField of string * 'JsonValue
    | BadPath of string * 'JsonValue * string
    | TooSmallArray of string * 'JsonValue
    | FailMessage of string
    | BadOneOf of DecoderError<'JsonValue> list

and DecoderError<'JsonValue> = string * ErrorReason<'JsonValue>

type Decoder<'T> =
    abstract member Decode<'JsonValue> :
        helpers: IDecoderHelpers<'JsonValue> * value: 'JsonValue ->
            Result<'T, DecoderError<'JsonValue>>

/// <summary>
/// A JSON value
/// </summary>
/// <remarks>
/// Some types don't have a direct representation in this DU,
/// this to make sure we represent them in the same way between the different
/// backends.
///
/// For example, <c>decimal</c> use <c>string</c> as the underlying type.
/// </remarks>
[<RequireQualifiedAccess; NoComparison>]
type Json =
    | String of string
    | Char of char
    | DecimalNumber of float
    | Null
    | Boolean of bool
    | Object of (string * Json) seq
    | Array of Json[]
    // Thoth.Json as an abritrary limit to the size of numbers
    | IntegralNumber of uint32
    | Unit

type Encoder<'T> = 'T -> Json
