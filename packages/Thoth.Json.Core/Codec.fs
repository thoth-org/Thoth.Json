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

    let char: Codec<char> = create Encode.char Decode.char

    let guid: Codec<Guid> = create Encode.guid Decode.guid

    let uri: Codec<Uri> = create Encode.uri Decode.uri

    let unit: Codec<unit> = create Encode.unit Decode.unit

    let sbyte: Codec<sbyte> = create Encode.sbyte Decode.sbyte

    let byte: Codec<byte> = create Encode.byte Decode.byte

    let int16: Codec<int16> = create Encode.int16 Decode.int16

    let uint16: Codec<uint16> = create Encode.uint16 Decode.uint16

    let int: Codec<int> = create Encode.int Decode.int

    let uint32: Codec<uint32> = create Encode.uint32 Decode.uint32

    let int64: Codec<int64> = create Encode.int64 Decode.int64

    let uint64: Codec<uint64> = create Encode.uint64 Decode.uint64

    let bigint: Codec<bigint> = create Encode.bigint Decode.bigint

    let bool: Codec<bool> = create Encode.bool Decode.bool

    let float: Codec<float> = create Encode.float Decode.float

    let float32: Codec<float32> = create Encode.float32 Decode.float32

    let decimal: Codec<decimal> = create Encode.decimal Decode.decimal

#if !FABLE_COMPILER_PYTHON
    let datetimeOffset: Codec<DateTimeOffset> =
        create Encode.datetimeOffset Decode.datetimeOffset
#endif

    let timespan: Codec<TimeSpan> = create Encode.timespan Decode.timespan

    /// <summary>
    /// A codec for the raw JSON AST, passing values through untouched.
    /// </summary>
    let value: Codec<Json> = create Encode.value Decode.value

    /// <summary>
    /// Encode any value as <c>null</c> and decode <c>null</c> back to the
    /// given constant. Useful as a branch of <see cref="oneOf"/> to represent
    /// a nullable/absent value.
    /// </summary>
    /// <remarks>
    /// This is a constant codec: every input encodes to the same <c>null</c>,
    /// so it only round-trips the provided value.
    /// </remarks>
    let nil (output: 'a) : Codec<'a> =
        create (fun _ -> Encode.nil) (Decode.nil output)

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

    let map
        (decoder: 't -> 'u)
        (encoder: 'u -> 't)
        (codec: Codec<'t>)
        : Codec<'u>
        =
        create (encoder >> codec.Encoder) (codec.Decoder |> Decode.map decoder)

    let lossyOption (x: Codec<'t>) : Codec<'t option> =
        create (Encode.lossyOption x.Encoder) (Decode.lossyOption x.Decoder)

    let losslessOption (x: Codec<'t>) : Codec<'t option> =
        create
            (Encode.losslessOption x.Encoder)
            (Decode.losslessOption x.Decoder)

    /// <summary>
    /// Build a codec that decodes any of the given representations, trying
    /// them in order, and encodes using the <c>first</c> codec of the list.
    ///
    /// This is the building block for lenient decoding and versioning:
    /// read several formats, always write the canonical (first) one.
    /// </summary>
    /// <remarks>
    /// Unlike the other combinators this is not a strict isomorphism:
    /// <c>decode (encode x)</c> still holds, but a value decoded from a
    /// fallback representation will be re-encoded using the first codec.
    /// </remarks>
    let oneOf (codecs: Codec<'t> list) : Codec<'t> =
        match codecs with
        | [] ->
            invalidArg (nameof codecs) "Codec.oneOf requires at least one codec"
        | first :: _ ->
            create first.Encoder (Decode.oneOf (codecs |> List.map _.Decoder))

    /// <summary>
    /// Defer the construction of a codec until it is first used. Combine with
    /// <see cref="fix"/> (or a recursive binding) to describe recursive types.
    /// </summary>
    let lazily (codec: Lazy<Codec<'t>>) : Codec<'t> =
        create
            (Encode.lazily (lazy codec.Value.Encoder))
            (Decode.lazily (lazy codec.Value.Decoder))

    /// <summary>
    /// Build a codec that can refer to itself, for recursive types such as
    /// trees or linked lists.
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

    let list (x: Codec<'t>) : Codec<'t list> =
        create
            (fun xs -> xs |> List.map x.Encoder |> Encode.list)
            (Decode.list x.Decoder)

    let array (x: Codec<'t>) : Codec<'t array> =
        create
            (fun xs -> xs |> Array.map x.Encoder |> Encode.array)
            (Decode.array x.Decoder)

    let seq (x: Codec<'t>) : Codec<'t seq> =
        create (Seq.map x.Encoder >> Encode.seq) (Decode.seq x.Decoder)

    let resizeArray (x: Codec<'t>) : Codec<ResizeArray<'t>> =
        create
            (Seq.map x.Encoder >> ResizeArray >> Encode.resizeArray)
            (Decode.resizeArray x.Decoder)

    let dict (x: Codec<'t>) : Codec<Map<string, 't>> =
        create
            (Map.map (fun _ v -> x.Encoder v) >> Encode.dict)
            (Decode.dict x.Decoder)

    let map'
        (key: Codec<'key>)
        (value: Codec<'value>)
        : Codec<Map<'key, 'value>>
        =
        create
            (Encode.map key.Encoder value.Encoder)
            (Decode.map' key.Decoder value.Decoder)

    let keyValuePairs (x: Codec<'t>) : Codec<(string * 't) list> =
        create
            (List.map (fun (k, v) -> k, x.Encoder v) >> Encode.object)
            (Decode.keyValuePairs x.Decoder)

    /// <summary>
    /// Focus a codec on a nested path: decode reads the value at
    /// <paramref name="fieldNames"/>, encode wraps it back in nested objects.
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
