[<RequireQualifiedAccess>]
module Thoth.Json.Extra

let empty: ExtraCoders = Map.empty

let inline withInt64 (extra: ExtraCoders): ExtraCoders =
    Map.add typeof<int64>.FullName (Encode.boxEncoder Encode.int64, Decode.boxDecoder Decode.int64) extra

let inline withUInt64 (extra: ExtraCoders): ExtraCoders =
    Map.add typeof<uint64>.FullName (Encode.boxEncoder Encode.uint64, Decode.boxDecoder Decode.uint64) extra

let inline withDecimal (extra: ExtraCoders): ExtraCoders =
    Map.add typeof<decimal>.FullName (Encode.boxEncoder Encode.decimal, Decode.boxDecoder Decode.decimal) extra

let inline withBigInt (extra: ExtraCoders): ExtraCoders =
    Map.add typeof<bigint>.FullName (Encode.boxEncoder Encode.bigint, Decode.boxDecoder Decode.bigint) extra

let inline withCustom (encoder: Encoder<'Value>) (decoder: Decoder<'Value>) (extra: ExtraCoders): ExtraCoders =
    Map.add typeof<'Value>.FullName (Encode.boxEncoder encoder, Decode.boxDecoder decoder) extra
