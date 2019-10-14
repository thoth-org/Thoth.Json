[<RequireQualifiedAccess>]
module Thoth.Json.Extra

let empty: ExtraCoders = 
    { Hash = System.Guid.Empty
      Coders =  Map.empty }

let inline withCustom (encoder: Encoder<'Value>) (decoder: Decoder<'Value>) (extra: ExtraCoders): ExtraCoders =
    { extra with 
          Hash = System.Guid.NewGuid()
          Coders = extra.Coders 
                   |> Map.add typeof<'Value>.FullName (Encode.boxEncoder encoder, Decode.boxDecoder decoder) }

let inline withInt64 (extra: ExtraCoders): ExtraCoders =
    withCustom Encode.int64 Decode.int64 extra
 
let inline withUInt64 (extra: ExtraCoders): ExtraCoders =
    withCustom Encode.uint64 Decode.uint64 extra

let inline withDecimal (extra: ExtraCoders): ExtraCoders =
    withCustom Encode.decimal Decode.decimal extra

let inline withBigInt (extra: ExtraCoders): ExtraCoders =
    withCustom Encode.bigint Decode.bigint extra
