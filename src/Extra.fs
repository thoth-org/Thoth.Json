[<RequireQualifiedAccess>]
module Thoth.Json.Extra

open Fable.Core

#if !FABLE_REPL_LIB
let empty: ExtraCoders =
    { Hash = ""
      Coders = Map.empty }

let inline internal withCustomAndKey (encoder: Encoder<'Value>) (decoder: Decoder<'Value>)
           (extra: ExtraCoders): ExtraCoders =
    { extra with
          Hash = System.Guid.NewGuid().ToString()
          Coders =
              extra.Coders |> Map.add typeof<'Value>.FullName (Encode.boxEncoder encoder, Decode.boxDecoder decoder) }

let inline withCustom (encoder: Encoder<'Value>) (decoder: Decoder<'Value>) (extra: ExtraCoders): ExtraCoders =
    withCustomAndKey encoder decoder extra

let inline withInt64 (extra: ExtraCoders): ExtraCoders = withCustomAndKey Encode.int64 Decode.int64 extra

let inline withUInt64 (extra: ExtraCoders): ExtraCoders =
    withCustomAndKey Encode.uint64 Decode.uint64 extra

let inline withDecimal (extra: ExtraCoders): ExtraCoders =
    withCustomAndKey Encode.decimal Decode.decimal extra

let inline withBigInt (extra: ExtraCoders): ExtraCoders =
    withCustomAndKey Encode.bigint Decode.bigint extra
#endif