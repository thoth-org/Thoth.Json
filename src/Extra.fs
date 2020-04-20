[<RequireQualifiedAccess>]
module Thoth.Json.Extra

let empty: ExtraCoders =
    { Hash = ""
      Coders = Map.empty
      FieldDecoders = Map.empty
      FieldEncoders = Map.empty }

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

let __withCustomFieldDecoderAndKey typeFullName (fieldName: string) (fieldDecoder: FieldDecoder) (extra: ExtraCoders) =
    { extra with Hash = System.Guid.NewGuid().ToString()
                 FieldDecoders =
                    Map.tryFind typeFullName extra.FieldDecoders
                    |> Option.defaultValue Map.empty
                    |> Map.add fieldName fieldDecoder
                    |> fun m -> Map.add typeFullName m extra.FieldDecoders }

let inline withCustomFieldDecoder<'T> (fieldName: string) (fieldDecoder: FieldDecoder) (extra: ExtraCoders) =
    __withCustomFieldDecoderAndKey typeof<'T>.FullName fieldName fieldDecoder extra

let __withCustomFieldEncoderAndKey typeFullName (fieldName: string) (fieldEncoder: FieldEncoder) (extra: ExtraCoders) =
    { extra with Hash = System.Guid.NewGuid().ToString()
                 FieldEncoders =
                    Map.tryFind typeFullName extra.FieldEncoders
                    |> Option.defaultValue Map.empty
                    |> Map.add fieldName fieldEncoder
                    |> fun m -> Map.add typeFullName m extra.FieldEncoders }

let inline withCustomFieldEncoder<'T> (fieldName: string) (fieldEncoder: FieldEncoder) (extra: ExtraCoders) =
    __withCustomFieldEncoderAndKey typeof<'T>.FullName fieldName fieldEncoder extra

