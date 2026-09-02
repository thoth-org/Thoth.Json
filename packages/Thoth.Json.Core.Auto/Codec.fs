namespace Thoth.Json.Core.Auto

open Thoth.Json.Core

[<RequireQualifiedAccess>]
module Codec =

    /// <summary>
    /// Codecs generated from F# types.
    /// </summary>
    type Auto private () =
        /// <summary>
        /// The codec for <c>'t</c>, generated from the type.
        /// </summary>
        static member inline generateCodec
            (
                ?caseStrategy: CaseStrategy,
                ?extra: ExtraCoders,
                ?skipNullField: bool,
                ?losslessOption: bool
            )
            : Codec<'t>
            =
            Codec.create
                (Encode.Auto.generateEncoder (
                    ?caseStrategy = caseStrategy,
                    ?extra = extra,
                    ?skipNullField = skipNullField,
                    ?losslessOption = losslessOption
                ))
                (Decode.Auto.generateDecoder (
                    ?caseStrategy = caseStrategy,
                    ?extra = extra,
                    ?losslessOption = losslessOption
                ))
