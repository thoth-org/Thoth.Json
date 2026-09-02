namespace Thoth.Json.Core

[<AutoOpen>]
module ObjectCodecComputationExpression =

    /// <summary>
    /// The fields declared so far by an <c>objectCodec</c> block.
    /// </summary>
    [<NoComparison>]
    [<NoEquality>]
    type ObjectCodecFieldSet<'t, 'u> =
        {
            Values: 't -> (string * IEncodable) list
            Decoder: Decoder<'t>
            Picker: 'u -> 't
        }

    [<RequireQualifiedAccess>]
    module internal ObjectCodecFieldSet =

        let zip
            (a: ObjectCodecFieldSet<'a, 'u>)
            (b: ObjectCodecFieldSet<'b, 'u>)
            : ObjectCodecFieldSet<'a * 'b, 'u>
            =
            {
                Values = fun (i, j) -> a.Values i @ b.Values j
                Decoder = Decode.map2 (fun i j -> i, j) a.Decoder b.Decoder
                Picker = fun u -> a.Picker u, b.Picker u
            }

        let complete
            (f: 't -> 'u)
            (m: ObjectCodecFieldSet<'t, 'u>)
            : Codec<'u>
            =
            let codec =
                Codec.create (fun t -> m.Values t |> Encode.object) m.Decoder

            Codec.map f m.Picker codec

    /// <summary>
    /// The builder behind <c>objectCodec</c>.
    /// </summary>
    type ObjectCodecBuilder internal () =
        member this.MergeSources
            (a: ObjectCodecFieldSet<'a, 'u>, b: ObjectCodecFieldSet<'b, 'u>)
            : ObjectCodecFieldSet<'a * 'b, 'u>
            =
            ObjectCodecFieldSet.zip a b

        member this.BindReturn
            (m: ObjectCodecFieldSet<'t, 'u>, f: 't -> 'u)
            : Codec<'u>
            =
            ObjectCodecFieldSet.complete f m

    /// <summary>
    /// Build a codec for a record, one <see cref="M:Thoth.Json.Core.Codec.field"/> per property.
    /// </summary>
    /// <example>
    /// <code lang="fsharp">
    /// let codec : Codec&lt;Point&gt; =
    ///     objectCodec {
    ///         let! x = Codec.field "x" _.X Codec.int
    ///         and! y = Codec.field "y" _.Y Codec.int
    ///
    ///         return { X = x; Y = y }
    ///     }
    /// </code>
    /// </example>
    let objectCodec = ObjectCodecBuilder()

    [<RequireQualifiedAccess>]
    module Codec =

        /// <summary>
        /// A required property of an <c>objectCodec</c> block: its name, how to read it out of the
        /// value, and the codec for it.
        /// </summary>
        let field
            (fieldName: string)
            (picker: 'u -> 't)
            (fieldCodec: Codec<'t>)
            : ObjectCodecFieldSet<'t, 'u>
            =
            {
                Values = fun i -> [ fieldName, fieldCodec.Encoder i ]
                Decoder = Decode.field fieldName fieldCodec.Decoder
                Picker = picker
            }

        /// <summary>
        /// An optional property of an <c>objectCodec</c> block. It is left out when the value is
        /// <c>None</c>, and its absence decodes back to <c>None</c>.
        /// </summary>
        let optional
            (fieldName: string)
            (picker: 'u -> 't option)
            (fieldCodec: Codec<'t>)
            : ObjectCodecFieldSet<'t option, 'u>
            =
            {
                Values =
                    function
                    | Some i -> [ fieldName, fieldCodec.Encoder i ]
                    | None -> []
                Decoder = Decode.optional fieldName fieldCodec.Decoder
                Picker = picker
            }
