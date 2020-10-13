namespace Thoth.Json

type CaseStrategy =
    | PascalCase
    | CamelCase
    | SnakeCase

type ErrorReason =
    | BadPrimitive of string * JsonValue
    | BadPrimitiveExtra of string * JsonValue * string
    | BadType of string * JsonValue
    | BadField of string * JsonValue
    | BadPath of string * JsonValue * string
    | TooSmallArray of string * JsonValue
    | FailMessage of string
    | BadOneOf of DecoderError list

and DecoderError = string * ErrorReason

type Decoder<'T> = JsonValue -> Result<'T, DecoderError>

type Encoder<'T> = 'T -> JsonValue

module DecoderError =

    let prependPath (path: string) (err: DecoderError): DecoderError =
        let (oldPath, reason) = err
        (path + oldPath, reason)

    let inline prependPathToResult<'T> (path: string) (res: Result<'T, DecoderError>) =
        res |> Result.mapError(prependPath path)

    let genericMsg (msg : string) (value : JsonValue) (newLine : bool) =
        try
            "Expecting "
                + msg
                + " but instead got:"
                + (if newLine then "\n" else " ")
                + (Helpers.anyToString value)
        with
            | _ ->
                "Expecting "
                + msg
                + " but decoder failed. Couldn't report given value due to circular structure."
                + (if newLine then "\n" else " ")

    let rec errorToString (path : string, error) =
        match error with
        | BadPrimitive (msg, value) ->
            genericMsg msg value false
        | BadType (msg, value) ->
            genericMsg msg value true
        | BadPrimitiveExtra (msg, value, reason) ->
            genericMsg msg value false + "\nReason: " + reason
        | BadField (msg, value) ->
            genericMsg msg value true
        | BadPath (msg, value, fieldName) ->
            genericMsg msg value true + ("\nNode `" + fieldName + "` is unknown.")
        | TooSmallArray (msg, value) ->
            "Expecting " + msg + ".\n" + (Helpers.anyToString value)
        | BadOneOf messages ->
            let messages =
                messages
                |> List.map (fun (subPath, subError) ->
                    "Error at: `" + (path + subPath) + "`\n" + errorToString (subPath, subError)
                )
            "The following errors were found:\n\n" + String.concat "\n\n" messages
        | FailMessage msg ->
            "The following `failure` occurred with the decoder: " + msg

    let errorToStringWithPath (path : string, error) =
        match error with
        | BadOneOf _ ->
            errorToString (path, error)
        | _ ->
            "Error at: `" + path + "`\n" + errorToString (path, error)

[<AbstractClass>]
type BoxedDecoder() =
    abstract Decode : value : JsonValue -> Result<obj, DecoderError>
    member this.BoxedDecoder : Decoder<obj> =
        fun value -> this.Decode(value)

[<AbstractClass>]
type BoxedEncoder() =
    abstract Encode : value : obj -> JsonValue
    member this.BoxedEncoder : Encoder<obj> =
        this.Encode

type ExtraCoders =
    {
        Hash : string
        Coders : Map<string, BoxedEncoder * BoxedDecoder>
    }

module internal Cache =

    #if THOTH_JSON_FABLE || (THOTH_JSON && FABLE_COMPILER)
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
    let Encoders = lazy Cache<BoxedEncoder>()
    let Decoders = lazy Cache<BoxedDecoder>()
    #endif

    #if THOTH_JSON_NEWTONSOFT || (THOTH_JSON && !FABLE_COMPILER)
    open System.Collections.Concurrent

    type Cache<'Value>() =
        let cache = ConcurrentDictionary<string, 'Value>()
        member __.GetOrAdd(key: string, factory: string->'Value) =
            cache.GetOrAdd(key, factory)

    let Encoders = lazy Cache<BoxedEncoder>()
    let Decoders = lazy Cache<BoxedDecoder>()
    #endif

module Util =

    open System.Text.RegularExpressions
    open FSharp.Reflection
    open System.Collections.Generic

    module Casing =
        let lowerFirst (str : string) = str.[..0].ToLowerInvariant() + str.[1..]
        let convert caseStrategy fieldName =
            match caseStrategy with
            | CamelCase -> lowerFirst fieldName
            | SnakeCase -> Regex.Replace(lowerFirst fieldName, "[A-Z]","_$0").ToLowerInvariant()
            | PascalCase -> fieldName

    #if !NETFRAMEWORK && !THOTH_JSON_FABLE && !(THOTH_JSON && FABLE_COMPILER)
    let (|StringEnum|_|) (typ : System.Type) =
        typ.CustomAttributes
        |> Seq.tryPick (function
            | attr when attr.AttributeType.FullName = typeof<Fable.Core.StringEnumAttribute>.FullName -> Some attr
            | _ -> None
        )

    let (|CompiledName|_|) (caseInfo : UnionCaseInfo) =
        caseInfo.GetCustomAttributes()
        |> Seq.tryPick (function
            | :? CompiledNameAttribute as att -> Some att.CompiledName
            | _ -> None)

    let (|LowerFirst|Forward|) (args : IList<System.Reflection.CustomAttributeTypedArgument>) =
        args
        |> Seq.tryPick (function
            | rule when rule.ArgumentType.FullName = typeof<Fable.Core.CaseRules>.FullName -> Some rule
            | _ -> None
        )
        |> function
        | Some rule ->
            match rule.Value with
            | :? int as value ->
                match value with
                | 0 -> Forward
                | 1 -> LowerFirst
                | _ -> LowerFirst // should not happen
            | _ -> LowerFirst // should not happen
        | None ->
            LowerFirst
    #endif
