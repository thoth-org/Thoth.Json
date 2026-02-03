namespace Thoth.Json.Core

open System.Numerics

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
    abstract encodeObject: (string * 'JsonValue) seq -> 'JsonValue
    abstract encodeArray: 'JsonValue array -> 'JsonValue
    abstract encodeList: 'JsonValue list -> 'JsonValue
    abstract encodeSeq: 'JsonValue seq -> 'JsonValue
    abstract encodeResizeArray: ResizeArray<'JsonValue> -> 'JsonValue
    // See https://github.com/thoth-org/Thoth.Json/issues/187 for more information
    // about why we make a distinction between signed and unsigned integral numbers
    // when encoding them.
    abstract encodeSignedIntegralNumber: int32 -> 'JsonValue
    abstract encodeUnsignedIntegralNumber: uint32 -> 'JsonValue

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
/// Numbers are representated as `string` so that the consumer can decide on precision
/// </remarks>
[<RequireQualifiedAccess; NoComparison>]
type Json =
    | String of string
    | Number of string
    | Null
    | Boolean of bool
    | Object of (string * Json) list
    | Array of Json list

type IEncodable =
    abstract member Encode<'JsonValue> :
        helpers: IEncoderHelpers<'JsonValue> -> 'JsonValue

type Encoder<'T> = 'T -> IEncodable
