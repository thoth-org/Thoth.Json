namespace Thoth.Json.Core

[<AutoOpen>]
module ObjectCodecComputationExpression =

    [<NoComparison>]
    [<NoEquality>]
    type ObjectCodecFieldSet<'t, 'u> =
        {
            Values: 't -> (string * IEncodable) list
            Decoder: Decoder<'t>
            Picker: 'u -> 't
        }

    [<RequireQualifiedAccess>]
    module ObjectCodecFieldSet =

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

    let objectCodec = ObjectCodecBuilder()

    [<RequireQualifiedAccess>]
    module Codec =

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
