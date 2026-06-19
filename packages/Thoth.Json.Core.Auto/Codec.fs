namespace Thoth.Json.Core.Auto

open Thoth.Json.Core

[<RequireQualifiedAccess>]
module Codec =

    type Auto private () =
        static member inline generateCodec
            (
                ?caseStrategy: CaseStrategy,
                ?extra: ExtraCoders,
                ?skipNullField: bool
            )
            : Codec<'t>
            =
            Codec.create
                (Encode.Auto.generateEncoder (
                    ?caseStrategy = caseStrategy,
                    ?extra = extra,
                    ?skipNullField = skipNullField
                ))
                (Decode.Auto.generateDecoder (
                    ?caseStrategy = caseStrategy,
                    ?extra = extra
                ))
