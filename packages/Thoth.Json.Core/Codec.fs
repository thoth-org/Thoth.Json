namespace Thoth.Json.Core

open System

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Codec =

    /// <summary>Pair an encoder and a decoder into a codec.</summary>
    let create (encoder: Encoder<'t>) (decoder: Decoder<'t>) =
        {
            Encoder = encoder
            Decoder = decoder
        }

    /// <summary>The encoder half of a codec.</summary>
    let encoder (c: Codec<'t>) = c.Encoder

    /// <summary>The decoder half of a codec.</summary>
    let decoder (c: Codec<'t>) = c.Decoder

    /// <summary>A codec for string, using the same representation as <c>Encode.string</c> and <c>Decode.string</c>.</summary>
    let string: Codec<string> = create Encode.string Decode.string

    /// <summary>A codec for char, using the same representation as <c>Encode.char</c> and <c>Decode.char</c>.</summary>
    let char: Codec<char> = create Encode.char Decode.char

    /// <summary>A codec for Guid, using the same representation as <c>Encode.guid</c> and <c>Decode.guid</c>.</summary>
    let guid: Codec<Guid> = create Encode.guid Decode.guid

    /// <summary>A codec for Uri, using the same representation as <c>Encode.uri</c> and <c>Decode.uri</c>.</summary>
    let uri: Codec<Uri> = create Encode.uri Decode.uri

    /// <summary>A codec for unit, using the same representation as <c>Encode.unit</c> and <c>Decode.unit</c>.</summary>
    let unit: Codec<unit> = create Encode.unit Decode.unit

    /// <summary>A codec for sbyte, using the same representation as <c>Encode.sbyte</c> and <c>Decode.sbyte</c>.</summary>
    let sbyte: Codec<sbyte> = create Encode.sbyte Decode.sbyte

    /// <summary>A codec for byte, using the same representation as <c>Encode.byte</c> and <c>Decode.byte</c>.</summary>
    let byte: Codec<byte> = create Encode.byte Decode.byte

    /// <summary>A codec for int16, using the same representation as <c>Encode.int16</c> and <c>Decode.int16</c>.</summary>
    let int16: Codec<int16> = create Encode.int16 Decode.int16

    /// <summary>A codec for uint16, using the same representation as <c>Encode.uint16</c> and <c>Decode.uint16</c>.</summary>
    let uint16: Codec<uint16> = create Encode.uint16 Decode.uint16

    /// <summary>A codec for int, using the same representation as <c>Encode.int</c> and <c>Decode.int</c>.</summary>
    let int: Codec<int> = create Encode.int Decode.int

    /// <summary>A codec for uint32, using the same representation as <c>Encode.uint32</c> and <c>Decode.uint32</c>.</summary>
    let uint32: Codec<uint32> = create Encode.uint32 Decode.uint32

    /// <summary>A codec for int64, using the same representation as <c>Encode.int64</c> and <c>Decode.int64</c>.</summary>
    let int64: Codec<int64> = create Encode.int64 Decode.int64

    /// <summary>A codec for uint64, using the same representation as <c>Encode.uint64</c> and <c>Decode.uint64</c>.</summary>
    let uint64: Codec<uint64> = create Encode.uint64 Decode.uint64

    /// <summary>A codec for bigint, using the same representation as <c>Encode.bigint</c> and <c>Decode.bigint</c>.</summary>
    let bigint: Codec<bigint> = create Encode.bigint Decode.bigint

    /// <summary>A codec for bool, using the same representation as <c>Encode.bool</c> and <c>Decode.bool</c>.</summary>
    let bool: Codec<bool> = create Encode.bool Decode.bool

    /// <summary>A codec for float, using the same representation as <c>Encode.float</c> and <c>Decode.float</c>.</summary>
    let float: Codec<float> = create Encode.float Decode.float

    /// <summary>A codec for float32, using the same representation as <c>Encode.float32</c> and <c>Decode.float32</c>.</summary>
    let float32: Codec<float32> = create Encode.float32 Decode.float32

    /// <summary>A codec for decimal, using the same representation as <c>Encode.decimal</c> and <c>Decode.decimal</c>.</summary>
    let decimal: Codec<decimal> = create Encode.decimal Decode.decimal

#if !FABLE_COMPILER_PYTHON
    /// <summary>A codec for DateTimeOffset, using the same representation as <c>Encode.datetimeOffset</c> and <c>Decode.datetimeOffset</c>.</summary>
    let datetimeOffset: Codec<DateTimeOffset> =
        create Encode.datetimeOffset Decode.datetimeOffset
#endif

    /// <summary>A codec for TimeSpan, using the same representation as <c>Encode.timespan</c> and <c>Decode.timespan</c>.</summary>
    let timespan: Codec<TimeSpan> = create Encode.timespan Decode.timespan

    /// <summary>
    /// A codec for the JSON tree itself, passing values through untouched.
    /// </summary>
    let value: Codec<Json> = create Encode.value Decode.value

    /// <summary>
    /// Encode any value as <c>null</c>, and decode <c>null</c> back to the given constant.
    /// </summary>
    /// <remarks>
    /// Every input encodes to the same <c>null</c>, so only the given value round-trips. Use it as a
    /// branch of <see cref="oneOf"/> to represent an absent value.
    /// </remarks>
    let nil (output: 'a) : Codec<'a> =
        create (fun _ -> Encode.nil) (Decode.nil output)

    [<RequireQualifiedAccess>]
    module Enum =

        /// <summary>A codec for an enum with an underlying byte.</summary>
        let inline byte<'t when 't: enum<byte>> : Codec<'t> =
            create Encode.Enum.byte Decode.Enum.byte

        /// <summary>A codec for an enum with an underlying sbyte.</summary>
        let inline sbyte<'t when 't: enum<sbyte>> : Codec<'t> =
            create Encode.Enum.sbyte Decode.Enum.sbyte

        /// <summary>A codec for an enum with an underlying int16.</summary>
        let inline int16<'t when 't: enum<int16>> : Codec<'t> =
            create Encode.Enum.int16 Decode.Enum.int16

        /// <summary>A codec for an enum with an underlying uint16.</summary>
        let inline uint16<'t when 't: enum<uint16>> : Codec<'t> =
            create Encode.Enum.uint16 Decode.Enum.uint16

        /// <summary>A codec for an enum with an underlying int.</summary>
        let inline int<'t when 't: enum<int>> : Codec<'t> =
            create Encode.Enum.int Decode.Enum.int

        /// <summary>A codec for an enum with an underlying uint32.</summary>
        let inline uint32<'t when 't: enum<uint32>> : Codec<'t> =
            create Encode.Enum.uint32 Decode.Enum.uint32

    /// <summary>
    /// Turn a codec into one for another type, given a conversion for each direction.
    /// </summary>
    let map
        (decoder: 't -> 'u)
        (encoder: 'u -> 't)
        (codec: Codec<'t>)
        : Codec<'u>
        =
        create (encoder >> codec.Encoder) (codec.Decoder |> Decode.map decoder)

    /// <summary>
    /// A codec for an option, writing <c>Some x</c> as <c>x</c> and <c>None</c> as <c>null</c>.
    /// </summary>
    /// <remarks>
    /// Lossy: a nested option does not round-trip. Use <see cref="losslessOption"/> when you need
    /// the distinction.
    /// </remarks>
    let lossyOption (x: Codec<'t>) : Codec<'t option> =
        create (Encode.lossyOption x.Encoder) (Decode.lossyOption x.Decoder)

    /// <summary>
    /// A codec for an option, writing an object which carries the case.
    /// </summary>
    /// <remarks>
    /// Round-trips at any nesting depth, at the cost of a heavier representation.
    /// </remarks>
    let losslessOption (x: Codec<'t>) : Codec<'t option> =
        create
            (Encode.losslessOption x.Encoder)
            (Decode.losslessOption x.Decoder)

    /// <summary>
    /// Decode with the first codec of the list that succeeds, and always encode with the first one.
    /// </summary>
    /// <remarks>
    /// A value read through a fallback codec is written back with the first one, so it is not a
    /// round-trip. Use it to accept an old representation while writing the new one.
    /// </remarks>
    let oneOf (codecs: Codec<'t> list) : Codec<'t> =
        match codecs with
        | [] ->
            invalidArg (nameof codecs) "Codec.oneOf requires at least one codec"
        | first :: _ ->
            create first.Encoder (Decode.oneOf (codecs |> List.map _.Decoder))

    /// <summary>
    /// Defer the construction of a codec until it is first used, for mutually recursive types.
    /// </summary>
    let lazily (codec: Lazy<Codec<'t>>) : Codec<'t> =
        create
            (Encode.lazily (lazy codec.Value.Encoder))
            (Decode.lazily (lazy codec.Value.Decoder))

    /// <summary>
    /// Build a codec which can refer to itself, for a recursive type such as a tree.
    /// </summary>
    /// <example>
    /// <code lang="fsharp">
    /// type Tree = Tree of value: int * children: Tree list
    ///
    /// let codec : Codec&lt;Tree&gt; =
    ///     Codec.fix (fun self ->
    ///         objectCodec {
    ///             let! value =
    ///                 Codec.field "value" (fun (Tree(v, _)) -> v) Codec.int
    ///
    ///             and! children =
    ///                 Codec.field
    ///                     "children"
    ///                     (fun (Tree(_, c)) -> c)
    ///                     (Codec.list self)
    ///
    ///             return Tree(value, children)
    ///         }
    ///     )
    /// </code>
    /// </example>
    let fix (make: Codec<'t> -> Codec<'t>) : Codec<'t> =
        let mutable self = Unchecked.defaultof<Codec<'t>>
        self <- make (lazily (lazy self))
        self

    /// <summary>A codec for a list, as a JSON array.</summary>
    let list (x: Codec<'t>) : Codec<'t list> =
        create (Encode.mapList x.Encoder) (Decode.list x.Decoder)

    /// <summary>A codec for an array, as a JSON array.</summary>
    let array (x: Codec<'t>) : Codec<'t array> =
        create (Encode.mapArray x.Encoder) (Decode.array x.Decoder)

    /// <summary>A codec for a sequence, as a JSON array.</summary>
    let seq (x: Codec<'t>) : Codec<'t seq> =
        create (Encode.mapSeq x.Encoder) (Decode.seq x.Decoder)

    /// <summary>A codec for a ResizeArray, as a JSON array.</summary>
    let resizeArray (x: Codec<'t>) : Codec<ResizeArray<'t>> =
        create (Encode.mapResizeArray x.Encoder) (Decode.resizeArray x.Decoder)

    /// <summary>A codec for a map keyed by string, as a JSON object.</summary>
    let dict (x: Codec<'t>) : Codec<Map<string, 't>> =
        create
            (Map.map (fun _ v -> x.Encoder v) >> Encode.dict)
            (Decode.dict x.Decoder)

    /// <summary>A codec for a map, as an array of <c>[ key, value ]</c> pairs. Use it when the key is not a string.</summary>
    let map'
        (key: Codec<'key>)
        (value: Codec<'value>)
        : Codec<Map<'key, 'value>>
        =
        create
            (Encode.map key.Encoder value.Encoder)
            (Decode.map' key.Decoder value.Decoder)

    /// <summary>A codec for a list of properties, as a JSON object.</summary>
    let keyValuePairs (x: Codec<'t>) : Codec<(string * 't) list> =
        create
            (List.map (fun (k, v) -> k, x.Encoder v) >> Encode.object)
            (Decode.keyValuePairs x.Decoder)

    /// <summary>
    /// Move a codec under a path of property names. Decoding reads the value there, encoding wraps
    /// it back in nested objects.
    /// </summary>
    let at (fieldNames: string list) (x: Codec<'t>) : Codec<'t> =
        create
            (fun v ->
                List.foldBack
                    (fun field child -> Encode.object [ field, child ])
                    fieldNames
                    (x.Encoder v)
            )
            (Decode.at fieldNames x.Decoder)

    /// <summary>A codec for a tuple of 2 elements, as a JSON array.</summary>
    let tuple2 (a: Codec<'a>) (b: Codec<'b>) : Codec<'a * 'b> =
        create
            (Encode.tuple2 a.Encoder b.Encoder)
            (Decode.tuple2 a.Decoder b.Decoder)

    /// <summary>A codec for a tuple of 3 elements, as a JSON array.</summary>
    let tuple3
        (a: Codec<'a>)
        (b: Codec<'b>)
        (c: Codec<'c>)
        : Codec<'a * 'b * 'c>
        =
        create
            (Encode.tuple3 a.Encoder b.Encoder c.Encoder)
            (Decode.tuple3 a.Decoder b.Decoder c.Decoder)

    /// <summary>A codec for a tuple of 4 elements, as a JSON array.</summary>
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

    /// <summary>A codec for a tuple of 5 elements, as a JSON array.</summary>
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

    /// <summary>A codec for a tuple of 6 elements, as a JSON array.</summary>
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

    /// <summary>A codec for a tuple of 7 elements, as a JSON array.</summary>
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

    /// <summary>A codec for a tuple of 8 elements, as a JSON array.</summary>
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
