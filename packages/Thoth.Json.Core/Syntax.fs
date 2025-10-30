namespace Thoth.Json.Core

[<AutoOpen>]
module Syntax =

    type DecoderBuilder internal () =
        member inline this.Bind(m, f) = Decode.andThen f m

        member this.BindReturn(m, f) = Decode.map f m

        member this.Bind2Return(d1, d2, ctor: struct ('a * 'b) -> 't) =
            Decode.map2 (fun a b -> ctor struct (a, b)) d1 d2

        member this.Bind3Return(d1, d2, d3, ctor: struct ('a * 'b * 'c) -> 't) =
            Decode.map3 (fun a b c -> ctor struct (a, b, c)) d1 d2 d3

        member this.Bind4Return
            (d1, d2, d3, d4, ctor: struct ('a * 'b * 'c * 'd) -> 't)
            =
            Decode.map4 (fun a b c d -> ctor struct (a, b, c, d)) d1 d2 d3 d4

        member this.Bind5Return
            (d1, d2, d3, d4, d5, ctor: struct ('a * 'b * 'c * 'd * 'e) -> 't)
            =
            Decode.map5
                (fun a b c d e -> ctor struct (a, b, c, d, e))
                d1
                d2
                d3
                d4
                d5

        member this.Bind6Return
            (
                d1,
                d2,
                d3,
                d4,
                d5,
                d6,
                ctor: struct ('a * 'b * 'c * 'd * 'e * 'f) -> 't
            )
            =
            Decode.map6
                (fun a b c d e f -> ctor struct (a, b, c, d, e, f))
                d1
                d2
                d3
                d4
                d5
                d6

        member this.Bind7Return
            (
                d1,
                d2,
                d3,
                d4,
                d5,
                d6,
                d7,
                ctor: struct ('a * 'b * 'c * 'd * 'e * 'f * 'g) -> 't
            )
            =
            Decode.map7
                (fun a b c d e f g -> ctor struct (a, b, c, d, e, f, g))
                d1
                d2
                d3
                d4
                d5
                d6
                d7

        member this.Bind8Return
            (
                d1,
                d2,
                d3,
                d4,
                d5,
                d6,
                d7,
                d8,
                ctor: struct ('a * 'b * 'c * 'd * 'e * 'f * 'g * 'h) -> 't
            )
            =
            Decode.map8
                (fun a b c d e f g h -> ctor struct (a, b, c, d, e, f, g, h))
                d1
                d2
                d3
                d4
                d5
                d6
                d7
                d8

        member this.MergeSources(d1, d2) =
            Decode.map2 (fun a b -> struct (a, b)) d1 d2

        member this.Return(x) = Decode.succeed x

        member this.ReturnFrom(x: Decoder<'a>) = x

    let decoder = DecoderBuilder()
