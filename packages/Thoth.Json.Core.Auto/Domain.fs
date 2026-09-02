namespace Thoth.Json.Core.Auto

open System
open System.Collections.Generic
open Thoth.Json.Core

/// <summary>
/// How the auto API renames record fields and union cases.
/// </summary>
type CaseStrategy =
    | SnakeCase
    | ScreamingSnakeCase
    | PascalCase
    | CamelCase
    | DotNetPascalCase
    | DotNetCamelCase

/// <summary>
/// The identity of a type in an <see cref="T:Thoth.Json.Core.Auto.ExtraCoders"/>.
/// </summary>
/// <remarks>
/// A generic type is keyed by its definition, so <c>Node&lt;int&gt;</c> and <c>Node&lt;string&gt;</c>
/// share a key.
/// </remarks>
type TypeKey =
    private
    | TypeKey of string

    static member Create(t: Type) =
        // For generic types, use the generic type definition name
        // so that CstNode<'a> and CstNode<int> map to the same key
        if t.IsGenericType then
            let genericTypeDef = t.GetGenericTypeDefinition()
            TypeKey genericTypeDef.FullName
        else
            TypeKey t.FullName

[<RequireQualifiedAccess>]
module TypeKey =

    /// <summary>The key of the given type.</summary>
    let ofType (t: Type) = TypeKey.Create(t)

/// <summary>
/// Holds the coders already generated, so a type is walked once.
/// </summary>
type Cache<'Key, 'Value when 'Key: equality>() =
    let cache = Dictionary<'Key, 'Value>()

    /// <summary>The cached value for the key, generating it on the first ask.</summary>
    member this.GetOrAdd(key, factory) =
        match cache.TryGetValue(key) with
        | true, x -> x
        | false, _ ->
            let x = factory ()
            cache.Add(key, x)
            x

/// <summary>An encoder whose type is not known statically.</summary>
type BoxedEncoder = obj

/// <summary>A decoder whose type is not known statically.</summary>
type BoxedDecoder = obj

/// <summary>
/// Coders the auto API uses instead of the ones it would generate.
/// </summary>
/// <remarks>
/// Build one with <c>Extra.empty</c> and the <c>Extra.with*</c> functions, then pass it as the
/// <c>extra</c> argument. Build it once and share it: the coder cache is keyed on its identity.
/// </remarks>
[<NoComparison>]
type ExtraCoders =
    {
        Hash: string
        EncoderOverrides: Map<TypeKey, BoxedEncoder>
        DecoderOverrides: Map<TypeKey, BoxedDecoder>
    }

[<RequireQualifiedAccess>]
module Extra =

    /// <summary>No extra coders.</summary>
    let empty =
        {
            Hash = ""
            EncoderOverrides = Map.empty
            DecoderOverrides = Map.empty
        }

    /// <summary>
    /// Use the given coders for <c>'t</c>, whether or not the auto API can generate them.
    /// </summary>
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

    /// <summary>Add support for int64.</summary>
    let inline withInt64 (extra: ExtraCoders) : ExtraCoders =
        withCustom Encode.int64 Decode.int64 extra

    /// <summary>Add support for uint64.</summary>
    let inline withUInt64 (extra: ExtraCoders) : ExtraCoders =
        withCustom Encode.uint64 Decode.uint64 extra

    /// <summary>Add support for decimal.</summary>
    let inline withDecimal (extra: ExtraCoders) : ExtraCoders =
        withCustom Encode.decimal Decode.decimal extra

    /// <summary>Add support for bigint.</summary>
    let inline withBigInt (extra: ExtraCoders) : ExtraCoders =
        withCustom Encode.bigint Decode.bigint extra
