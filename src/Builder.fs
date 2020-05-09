module Thoth.Json.Builder

type DecoderBuilder() =
    member __.Bind(decoder: 'a Decoder, f: 'a -> 'b Decoder): 'b Decoder = Decode.andThen f decoder

    member __.Return(value: 'a): 'a Decoder = Decode.succeed value

    member __.ReturnFrom(decoder: 'a Decoder): 'a Decoder = decoder

    member __.Zero(): unit Decoder = __.Return()

let decoder = DecoderBuilder()
