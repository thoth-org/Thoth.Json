module Thoth.Json.Tests.Testing

open Thoth.Json.Core
open Fable.Pyxpecto

type IEncode =
    abstract toString: int -> IEncodable -> string

type IDecode<'JsonValue> =
    abstract fromValue<'T> : Decoder<'T> -> ('JsonValue -> Result<'T, string>)
    abstract fromString<'T> : Decoder<'T> -> string -> Result<'T, string>
    abstract unsafeFromString<'T> : Decoder<'T> -> string -> 'T

[<AbstractClass>]
type TestRunner<'DecoderJsonValue, 'EncoderJsonValue>() =
    abstract Encode: IEncode with get
    abstract Decode: IDecode<'DecoderJsonValue> with get
    abstract EncoderHelpers: IEncoderHelpers<'EncoderJsonValue> with get
    abstract DecoderHelpers: IDecoderHelpers<'DecoderJsonValue> with get

    abstract MapEncoderValueToDecoderValue:
        'EncoderJsonValue -> 'DecoderJsonValue

let equal (actual: 'T) (expected: 'T) = Expect.equal actual expected ""
