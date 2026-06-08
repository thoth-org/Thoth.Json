namespace Thoth.Json.Core

open System

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Codec =

    let create (encoder: Encoder<'t>) (decoder: Decoder<'t>) =
        {
            Encoder = encoder
            Decoder = decoder
        }

    let encoder (c: Codec<'t>) = c.Encoder

    let decoder (c: Codec<'t>) = c.Decoder

    let string: Codec<string> = create Encode.string Decode.string

    let int: Codec<int> = create Encode.int Decode.int

    let bool: Codec<bool> = create Encode.bool Decode.bool

    let guid: Codec<Guid> = create Encode.guid Decode.guid

    let float: Codec<float> = create Encode.float Decode.float

    let float32: Codec<float32> = create Encode.float32 Decode.float32

    let decimal: Codec<decimal> = create Encode.decimal Decode.decimal

    let sbyte: Codec<sbyte> = create Encode.sbyte Decode.sbyte

    let byte: Codec<byte> = create Encode.byte Decode.byte

    let int16: Codec<int16> = create Encode.int16 Decode.int16

    let uint16: Codec<uint16> = create Encode.uint16 Decode.uint16

    let uint32: Codec<uint32> = create Encode.uint32 Decode.uint32

    let int64: Codec<int64> = create Encode.int64 Decode.int64

    let unit: Codec<unit> = create Encode.unit Decode.unit

#if !FABLE_COMPILER_PYTHON
    let datetimeOffset: Codec<DateTimeOffset> =
        create Encode.datetimeOffset Decode.datetimeOffset
#endif

    let bigint: Codec<bigint> = create Encode.bigint Decode.bigint

    let timespan: Codec<TimeSpan> = create Encode.timespan Decode.timespan

    [<RequireQualifiedAccess>]
    module Enum =

        let inline byte<'t when 't: enum<byte>> : Codec<'t> =
            create Encode.Enum.byte Decode.Enum.byte

        let inline sbyte<'t when 't: enum<sbyte>> : Codec<'t> =
            create Encode.Enum.sbyte Decode.Enum.sbyte

        let inline int16<'t when 't: enum<int16>> : Codec<'t> =
            create Encode.Enum.int16 Decode.Enum.int16

        let inline uint16<'t when 't: enum<uint16>> : Codec<'t> =
            create Encode.Enum.uint16 Decode.Enum.uint16

        let inline int<'t when 't: enum<int>> : Codec<'t> =
            create Encode.Enum.int Decode.Enum.int

        let inline uint32<'t when 't: enum<uint32>> : Codec<'t> =
            create Encode.Enum.uint32 Decode.Enum.uint32

    let map (f: 't -> 'u) (f': 'u -> 't) (codec: Codec<'t>) : Codec<'u> =
        create (f' >> codec.Encoder) (codec.Decoder |> Decode.map f)

    let option (x: Codec<'t>) : Codec<'t option> =
        create (Encode.lossyOption x.Encoder) (Decode.lossyOption x.Decoder)

    let lossyOption (x: Codec<'t>) : Codec<'t option> =
        create (Encode.lossyOption x.Encoder) (Decode.lossyOption x.Decoder)

    let losslessOption (x: Codec<'t>) : Codec<'t option> =
        create
            (Encode.losslessOption x.Encoder)
            (Decode.losslessOption x.Decoder)

    let list (x: Codec<'t>) : Codec<'t list> =
        create
            (fun xs -> xs |> List.map x.Encoder |> Encode.list)
            (Decode.list x.Decoder)

    let array (x: Codec<'t>) : Codec<'t array> =
        create
            (fun xs -> xs |> Array.map x.Encoder |> Encode.array)
            (Decode.array x.Decoder)

    let tuple2 (a: Codec<'a>) (b: Codec<'b>) : Codec<'a * 'b> =
        create
            (Encode.tuple2 a.Encoder b.Encoder)
            (Decode.tuple2 a.Decoder b.Decoder)

    let tuple3
        (a: Codec<'a>)
        (b: Codec<'b>)
        (c: Codec<'c>)
        : Codec<'a * 'b * 'c>
        =
        create
            (Encode.tuple3 a.Encoder b.Encoder c.Encoder)
            (Decode.tuple3 a.Decoder b.Decoder c.Decoder)

    let tuple4
        (a: Codec<'a>)
        (b: Codec<'b>)
        (c: Codec<'c>)
        (d: Codec<'d>)
        : Codec<'a * 'b * 'c * 'd>
        =
        create
            (Encode.tuple4 a.Encoder b.Encoder c.Encoder d.Encoder)
            (Decode.tuple4 a.Decoder b.Decoder c.Decoder d.Decoder)

    let tuple5
        (a: Codec<'a>)
        (b: Codec<'b>)
        (c: Codec<'c>)
        (d: Codec<'d>)
        (e: Codec<'e>)
        : Codec<'a * 'b * 'c * 'd * 'e>
        =
        create
            (Encode.tuple5 a.Encoder b.Encoder c.Encoder d.Encoder e.Encoder)
            (Decode.tuple5 a.Decoder b.Decoder c.Decoder d.Decoder e.Decoder)

    let tuple6
        (a: Codec<'a>)
        (b: Codec<'b>)
        (c: Codec<'c>)
        (d: Codec<'d>)
        (e: Codec<'e>)
        (f: Codec<'f>)
        : Codec<'a * 'b * 'c * 'd * 'e * 'f>
        =
        create
            (Encode.tuple6
                a.Encoder
                b.Encoder
                c.Encoder
                d.Encoder
                e.Encoder
                f.Encoder)
            (Decode.tuple6
                a.Decoder
                b.Decoder
                c.Decoder
                d.Decoder
                e.Decoder
                f.Decoder)

    let tuple7
        (a: Codec<'a>)
        (b: Codec<'b>)
        (c: Codec<'c>)
        (d: Codec<'d>)
        (e: Codec<'e>)
        (f: Codec<'f>)
        (g: Codec<'g>)
        : Codec<'a * 'b * 'c * 'd * 'e * 'f * 'g>
        =
        create
            (Encode.tuple7
                a.Encoder
                b.Encoder
                c.Encoder
                d.Encoder
                e.Encoder
                f.Encoder
                g.Encoder)
            (Decode.tuple7
                a.Decoder
                b.Decoder
                c.Decoder
                d.Decoder
                e.Decoder
                f.Decoder
                g.Decoder)

    let tuple8
        (a: Codec<'a>)
        (b: Codec<'b>)
        (c: Codec<'c>)
        (d: Codec<'d>)
        (e: Codec<'e>)
        (f: Codec<'f>)
        (g: Codec<'g>)
        (h: Codec<'h>)
        : Codec<'a * 'b * 'c * 'd * 'e * 'f * 'g * 'h>
        =
        create
            (Encode.tuple8
                a.Encoder
                b.Encoder
                c.Encoder
                d.Encoder
                e.Encoder
                f.Encoder
                g.Encoder
                h.Encoder)
            (Decode.tuple8
                a.Decoder
                b.Decoder
                c.Decoder
                d.Decoder
                e.Decoder
                f.Decoder
                g.Decoder
                h.Decoder)
