module Thoth.Json.Tests.Testing

open Thoth.Json.Core
open Scriptorium.Nib.Assertion

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

let equal (actual: 'T) (expected: 'T) = assertThat actual (isEqualTo expected)

let notEqual (actual: 'T) (expected: 'T) =
    assertThat actual (isNotEqualTo expected)

let contains (expected: string) : Assertion<string> =
    assertion
        (fun (actual: string) -> actual.Contains expected)
        (fun actual -> $"given %A{actual} should contain %A{expected}")

let roundTrip (testRunner: TestRunner<_, _>) (codec: Codec<'t>) v =
    let encoded = v |> Encode.codec codec |> testRunner.Encode.toString 2

    // Uncomment for debugging purpose
    // printfn $"---\n%s{encoded}\n---"

    let decoded = encoded |> testRunner.Decode.fromString (Decode.codec codec)

    match decoded with
    | Ok value -> value
    | Error error -> failwith $"Decoding must succeed but got: {error}"
