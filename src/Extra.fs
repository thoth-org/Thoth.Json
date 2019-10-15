[<RequireQualifiedAccess>]
module Thoth.Json.Extra

open Fable.Core

let empty: ExtraCoders =
    { Hash = ""
      Coders = Map.empty }

[<StringEnum>]
type private InternalCoder =
    | Int64
    | Uint64
    | Decimal
    | Bigint

type private Key =
    | Coder of InternalCoder
    | NewCoder

let inline private withCustomAndKey (key: Key) (encoder: Encoder<'Value>) (decoder: Decoder<'Value>)
           (extra: ExtraCoders): ExtraCoders =
    { extra with
          Hash =
              match key with
              | Coder k -> extra.Hash + key.ToString()
              | NewCoder -> System.Guid.NewGuid().ToString()
          Coders =
              extra.Coders |> Map.add typeof<'Value>.FullName (Encode.boxEncoder encoder, Decode.boxDecoder decoder) }

let inline withCustom (encoder: Encoder<'Value>) (decoder: Decoder<'Value>) (extra: ExtraCoders): ExtraCoders =
    withCustomAndKey NewCoder encoder decoder extra

let inline withInt64 (extra: ExtraCoders): ExtraCoders = withCustomAndKey (Coder Int64) Encode.int64 Decode.int64 extra

let inline withUInt64 (extra: ExtraCoders): ExtraCoders =
    withCustomAndKey (Coder Uint64) Encode.uint64 Decode.uint64 extra

let inline withDecimal (extra: ExtraCoders): ExtraCoders =
    withCustomAndKey (Coder Decimal) Encode.decimal Decode.decimal extra

let inline withBigInt (extra: ExtraCoders): ExtraCoders =
    withCustomAndKey (Coder Bigint) Encode.bigint Decode.bigint extra
