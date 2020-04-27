namespace Thoth.Json
open System.Text.RegularExpressions

type JsonValue = obj

type ErrorReason =
    | BadPrimitive of string * JsonValue
    | BadPrimitiveExtra of string * JsonValue * string
    | BadType of string * JsonValue
    | BadField of string * JsonValue
    | BadPath of string * JsonValue * string
    | TooSmallArray of string * JsonValue
    | FailMessage of string
    | BadOneOf of string list

type CaseStrategy =
    | PascalCase
    | CamelCase
    | SnakeCase

type DecoderError = string * ErrorReason

type Decoder<'T> = string -> JsonValue -> Result<'T, DecoderError>

type Encoder<'T> = 'T -> JsonValue

type BoxedDecoder = Decoder<obj>

type BoxedEncoder = Encoder<obj>

type FieldDecoderResult =
    | UseOk of obj
    | UseError of DecoderError
    | UseAutoDecoder

type FieldDecoder = string -> JsonValue option -> FieldDecoderResult

type FieldEncoderResult =
    | UseJsonValue of JsonValue
    | IgnoreField
    | UseAutoEncoder

type FieldEncoder = obj -> FieldEncoderResult

type ExtraCoders =
    { Hash: string
      Coders: Map<string, BoxedEncoder * BoxedDecoder>
      FieldDecoders: Map<string, Map<string, FieldDecoder>>
      FieldEncoders: Map<string, Map<string, FieldEncoder>> }

module internal Util =
    open System.Collections.Generic

    type Cache<'Value>() =
        let cache = Dictionary<string, 'Value>()
        member __.GetOrAdd(key, factory) =
            match cache.TryGetValue(key) with
            | true, x -> x
            | false, _ ->
                let x = factory()
                cache.Add(key, x)
                x

    // Tree shaking will remove this if not used
    // so no need to make them lazy in Fable
    let CachedEncoders = Cache<BoxedEncoder>()
    let CachedDecoders = Cache<BoxedDecoder>()

    /// If used from .NET the type resolver won't be injected,
    /// throw a more informative error than just a null reference.
    let inline resolveType (resolver: Fable.Core.ITypeResolver<'T> option): System.Type =
#if !FABLE_COMPILER
        failwith "Thoth.Json is only compatible with Fable, use Thoth.Json.Net"
#else
        resolver.Value.ResolveType()
#endif


    module Casing =
        let lowerFirst (str : string) = str.[..0].ToLowerInvariant() + str.[1..]
        let convert caseStrategy fieldName =
            match caseStrategy with
            | CamelCase -> lowerFirst fieldName
            | SnakeCase -> Regex.Replace(lowerFirst fieldName, "[A-Z]","_$0").ToLowerInvariant()
            | PascalCase -> fieldName
