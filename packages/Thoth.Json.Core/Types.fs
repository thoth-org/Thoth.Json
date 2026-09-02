namespace Thoth.Json.Core

/// <summary>
/// Reads and tests a JSON value of the runtime's own type.
/// </summary>
/// <remarks>
/// Implemented once per runtime package. A decoder goes through it so that the same decoder works
/// on every runtime.
/// </remarks>
type IDecoderHelpers<'JsonValue> =
    /// <summary>Whether the value is a JSON string.</summary>
    abstract isString: 'JsonValue -> bool
    /// <summary>Whether the value is a JSON number.</summary>
    abstract isNumber: 'JsonValue -> bool
    /// <summary>Whether the value is a JSON boolean.</summary>
    abstract isBoolean: 'JsonValue -> bool
    /// <summary>Whether the value is <c>null</c>.</summary>
    abstract isNullValue: 'JsonValue -> bool
    /// <summary>Whether the value is a JSON array.</summary>
    abstract isArray: 'JsonValue -> bool
    /// <summary>Whether the value is a JSON object.</summary>
    abstract isObject: 'JsonValue -> bool
    /// <summary>Whether the object has a property under the given name.</summary>
    abstract hasProperty: string -> 'JsonValue -> bool
    /// <summary>Whether the number has no fractional part.</summary>
    abstract isIntegralValue: 'JsonValue -> bool
    /// <summary>The value as a string.</summary>
    abstract asString: 'JsonValue -> string
    /// <summary>The value as a float.</summary>
    abstract asFloat: 'JsonValue -> float
    /// <summary>The value as a float32.</summary>
    abstract asFloat32: 'JsonValue -> float32
    /// <summary>The value as an int.</summary>
    abstract asInt: 'JsonValue -> int
    /// <summary>The value as a boolean.</summary>
    abstract asBoolean: 'JsonValue -> bool
    /// <summary>The elements of the array.</summary>
    abstract asArray: 'JsonValue -> 'JsonValue[]
    /// <summary>The value of the object property under the given name.</summary>
    abstract getProperty: string * 'JsonValue -> 'JsonValue
    /// <summary>The property names of the object.</summary>
    abstract getProperties: 'JsonValue -> string seq
    /// <summary>The value as JSON text, for the error messages.</summary>
    abstract anyToString: 'JsonValue -> string
    /// <summary>The number as text, without the runtime's own formatting.</summary>
    abstract numberToString: 'JsonValue -> string

/// <summary>
/// Builds a JSON value of the runtime's own type.
/// </summary>
/// <remarks>
/// Implemented once per runtime package. An <see cref="T:Thoth.Json.Core.IEncodable"/> goes through
/// it so that the same encoder works on every runtime.
/// </remarks>
type IEncoderHelpers<'JsonValue> =
    /// <summary>A JSON string.</summary>
    abstract encodeString: string -> 'JsonValue
    /// <summary>A JSON string holding a single character.</summary>
    abstract encodeChar: char -> 'JsonValue
    /// <summary>A JSON number which may have a fractional part.</summary>
    abstract encodeDecimalNumber: float -> 'JsonValue
    /// <summary>A JSON boolean.</summary>
    abstract encodeBool: bool -> 'JsonValue
    /// <summary>A JSON <c>null</c>.</summary>
    abstract encodeNull: unit -> 'JsonValue
    /// <summary>A JSON object from the given properties.</summary>
    abstract encodeObject: (string * 'JsonValue) seq -> 'JsonValue
    /// <summary>A JSON array from an array.</summary>
    abstract encodeArray: 'JsonValue array -> 'JsonValue
    /// <summary>A JSON array from a list.</summary>
    abstract encodeList: 'JsonValue list -> 'JsonValue
    /// <summary>A JSON array from a sequence.</summary>
    abstract encodeSeq: 'JsonValue seq -> 'JsonValue
    /// <summary>A JSON array from a ResizeArray.</summary>
    abstract encodeResizeArray: ResizeArray<'JsonValue> -> 'JsonValue
    // See https://github.com/thoth-org/Thoth.Json/issues/187 for more information
    // about why we make a distinction between signed and unsigned integral numbers
    // when encoding them.
    /// <summary>A JSON number holding a signed whole number.</summary>
    abstract encodeSignedIntegralNumber: int32 -> 'JsonValue
    /// <summary>A JSON number holding an unsigned whole number.</summary>
    abstract encodeUnsignedIntegralNumber: uint32 -> 'JsonValue

/// <summary>
/// Why a decoder failed, and the value it failed on.
/// </summary>
type ErrorReason<'JsonValue> =
    /// <summary>A value of the wrong shape, reported as what was expected.</summary>
    | BadPrimitive of string * 'JsonValue
    /// <summary>The same, followed by an extra message.</summary>
    | BadPrimitiveExtra of string * 'JsonValue * string
    /// <summary>A value which is not of the expected type.</summary>
    | BadType of string * 'JsonValue
    /// <summary>An object missing an expected field.</summary>
    | BadField of string * 'JsonValue
    /// <summary>An object missing a value at an expected path.</summary>
    | BadPath of string * 'JsonValue * string
    /// <summary>An array with fewer elements than expected.</summary>
    | TooSmallArray of string * 'JsonValue
    /// <summary>A decoder which failed with a message of its own.</summary>
    | FailMessage of string
    /// <summary>Every decoder of a <c>oneOf</c> failed.</summary>
    | BadOneOf of DecoderError<'JsonValue> list

/// <summary>
/// A decoding failure: the JSONPath where it happened, and why.
/// </summary>
and DecoderError<'JsonValue> = string * ErrorReason<'JsonValue>

/// <summary>
/// Turns a JSON value into a <c>'T</c>, or explains why it can't.
/// </summary>
/// <remarks>
/// The method is generic over the JSON type, so a decoder is written once and runs on every runtime.
/// </remarks>
type Decoder<'T> =
    abstract member Decode<'JsonValue> :
        helpers: IDecoderHelpers<'JsonValue> * value: 'JsonValue ->
            Result<'T, DecoderError<'JsonValue>>

/// <summary>
/// A JSON value
/// </summary>
/// <remarks>
/// Although theoretically numbers are arbitrary prevision in JSON,
/// here they are representated as `float`, which is in line with most implementations.
/// </remarks>
[<RequireQualifiedAccess; NoComparison>]
type Json =
    | String of string
    | Number of float
    | Null
    | Boolean of bool
    | Object of (string * Json) list
    | Array of Json list

/// <summary>
/// A value which knows how to write itself as JSON.
/// </summary>
/// <remarks>
/// The method is generic over the JSON type, so what an encoder builds can be written by every
/// runtime.
/// </remarks>
type IEncodable =
    abstract member Encode<'JsonValue> :
        helpers: IEncoderHelpers<'JsonValue> -> 'JsonValue

/// <summary>
/// Turns a <c>'T</c> into something a runtime can write as JSON.
/// </summary>
type Encoder<'T> = 'T -> IEncodable

/// <summary>
/// An encoder and a decoder for the same type.
/// </summary>
/// <remarks>
/// Building both from one description keeps them in step. See
/// <c>objectCodec</c> and <c>variantCodec</c>.
/// </remarks>
[<NoComparison>]
[<NoEquality>]
type Codec<'t> =
    {
        Encoder: Encoder<'t>
        Decoder: Decoder<'t>
    }
