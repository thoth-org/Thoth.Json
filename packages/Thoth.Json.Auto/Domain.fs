namespace Thoth.Json.Auto

open System
open System.Collections.Generic
open Thoth.Json.Core

type CaseStyle =
    | SnakeCase
    | ScreamingSnakeCase
    | PascalCase
    | CamelCase
    | DotNetPascalCase
    | DotNetCamelCase

type TypeKey =
    private
    | TypeKey of string

    static member Create(t: Type) = TypeKey t.FullName

[<RequireQualifiedAccess>]
module TypeKey =

    let ofType (t: Type) = TypeKey.Create(t)

type Cache<'Value>() =
    let cache = Dictionary<string, 'Value>()

    member this.GetOrAdd(key, factory) =
        match cache.TryGetValue(key) with
        | true, x -> x
        | false, _ ->
            let x = factory ()
            cache.Add(key, x)
            x

type BoxedEncoder = obj

type BoxedDecoder = obj

[<NoComparison>]
type ExtraCoders =
    {
        Hash: string
        EncoderOverrides: Map<TypeKey, BoxedEncoder>
        DecoderOverrides: Map<TypeKey, BoxedDecoder>
    }

[<RequireQualifiedAccess>]
module Extra =

    let empty =
        {
            Hash = ""
            EncoderOverrides = Map.empty
            DecoderOverrides = Map.empty
        }

    let inline withCustom
        (encoder: Encoder<'t>)
        (decoder: Decoder<'t>)
        (opts: ExtraCoders)
        : ExtraCoders
        =
        let hash = Guid.NewGuid()
        let typeKey = TypeKey.ofType typeof<'t>

        {
            Hash = string hash
            EncoderOverrides = opts.EncoderOverrides |> Map.add typeKey encoder
            DecoderOverrides = opts.DecoderOverrides |> Map.add typeKey decoder
        }

    let inline withInt64 (extra: ExtraCoders) : ExtraCoders =
        withCustom Encode.int64 Decode.int64 extra

    let inline withUInt64 (extra: ExtraCoders) : ExtraCoders =
        withCustom Encode.uint64 Decode.uint64 extra

    let inline withDecimal (extra: ExtraCoders) : ExtraCoders =
        withCustom Encode.decimal Decode.decimal extra

    let inline withBigInt (extra: ExtraCoders) : ExtraCoders =
        withCustom Encode.bigint Decode.bigint extra
