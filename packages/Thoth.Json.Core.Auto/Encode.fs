namespace Thoth.Json.Core.Auto

open System
open System.Collections.Concurrent
open System.Reflection
open FSharp.Reflection
open Thoth.Json.Core
open Thoth.Json.Core.Auto

[<RequireQualifiedAccess>]
module Encode =

    [<RequireQualifiedAccess>]
    module internal Encode =

        [<RequireQualifiedAccess>]
        module Generic =

            type EncodeHelpers =
                static member OptionOf<'t>
                    (enc: Encoder<'t>)
                    : Encoder<'t option>
                    =
                    Encode.lossyOption enc

                static member OptionOfLossless<'t>
                    (enc: Encoder<'t>)
                    : Encoder<'t option>
                    =
                    Encode.losslessOption enc

                static member Lazily<'t>(enc: Lazy<Encoder<'t>>) : Encoder<'t> =
                    Encode.lazily enc

                static member SeqOf<'t>(enc: Encoder<'t>) : Encoder<'t seq> =
                    fun xs -> xs |> Seq.map enc |> Seq.toArray |> Encode.array

                static member ListOf<'t>(enc: Encoder<'t>) : Encoder<'t list> =
                    fun xs -> xs |> List.map enc |> Encode.list

                static member MapOf<'k, 'v when 'k: comparison>
                    (stringifyKey: 'k -> string, enc: Encoder<'v>)
                    : Encoder<Map<'k, 'v>>
                    =
                    fun m ->
                        [
                            for KeyValue(k, v) in m do
                                stringifyKey k, enc v
                        ]
                        |> Encode.object

                static member MapAsArrayOf<'k, 'v when 'k: comparison>
                    (keyEncoder: Encoder<'k>, valueEncoder: Encoder<'v>)
                    : Encoder<Map<'k, 'v>>
                    =
                    fun m ->
                        [|
                            for KeyValue(k, v) in m do
                                Encode.tuple2 keyEncoder valueEncoder (k, v)
                        |]
                        |> Encode.array

                static member SetOf<'t when 't: comparison>
                    (enc: Encoder<'t>)
                    : Encoder<Set<'t>>
                    =
                    fun xs -> xs |> Seq.map enc |> Seq.toArray |> Encode.array

                static member ArrayOf<'t>
                    (enc: Encoder<'t>)
                    : Encoder<'t array>
                    =
                    fun xs -> xs |> Array.map enc |> Encode.array

                static member EnumByte<'t when 't: enum<byte>>() : Encoder<'t> =
                    Encode.Enum.byte

                static member EnumSbyte<'t when 't: enum<sbyte>>
                    ()
                    : Encoder<'t>
                    =
                    Encode.Enum.sbyte

                static member EnumInt16<'t when 't: enum<int16>>
                    ()
                    : Encoder<'t>
                    =
                    Encode.Enum.int16

                static member EnumUint16<'t when 't: enum<uint16>>
                    ()
                    : Encoder<'t>
                    =
                    Encode.Enum.uint16

                static member EnumInt<'t when 't: enum<int>>() : Encoder<'t> =
                    Encode.Enum.int

                static member EnumUint32<'t when 't: enum<uint>>
                    ()
                    : Encoder<'t>
                    =
                    Encode.Enum.uint32

            // static member Field<'t, 'u>(picker : 't -> 'u, fieldEncoder : Encoder<'u>) : Encode.IFieldEncoder<'t> =
            //   Encode.field picker fieldEncoder

            // static member Object<'t>(fields : seq<string * Encode.IFieldEncoder<'t>>) : Encoder<'t> =
            //   Encode.object fields

            // static member Element<'t, 'u>(picker : 't -> 'u, elementEncoder : Encoder<'u>) : Encoder<'t> =
            //   Encode.element picker elementEncoder

            // static member FixedArray<'t>(elements : Encoder<'t> seq) : Encoder<'t> =
            //   Encode.fixedArray elements

            // static member Union<'t>(picker : 't -> Encode.ICase<'t>) : Encoder<'t> =
            //   Encode.union picker

            // static member Case<'t>(tag : string, data : Encode.ICaseData<'t> seq) : Encode.ICase<'t> =
            //   Encode.case tag data

#if !FABLE_COMPILER
            let private makeCachedInvoker (name: string) =
                let methodDef =
                    typeof<EncodeHelpers>
                        .GetMethods(
                            BindingFlags.Static ||| BindingFlags.NonPublic
                        )
                    |> Seq.filter (fun x -> x.Name = name)
                    |> Seq.exactlyOne
                    |> fun mi -> mi.GetGenericMethodDefinition()

                let cache = ConcurrentDictionary<Type, MethodInfo>()

                fun (typeArg: Type) (args: obj[]) ->
                    cache
                        .GetOrAdd(
                            typeArg,
                            fun t -> methodDef.MakeGenericMethod(t)
                        )
                        .Invoke(null, args)

            let private makeCachedInvoker2 (name: string) =
                let methodDef =
                    typeof<EncodeHelpers>
                        .GetMethods(
                            BindingFlags.Static ||| BindingFlags.NonPublic
                        )
                    |> Seq.filter (fun x -> x.Name = name)
                    |> Seq.exactlyOne
                    |> fun mi -> mi.GetGenericMethodDefinition()

                let cache =
                    ConcurrentDictionary<struct (Type * Type), MethodInfo>()

                fun (typeArg1: Type) (typeArg2: Type) (args: obj[]) ->
                    cache
                        .GetOrAdd(
                            struct (typeArg1, typeArg2),
                            fun struct (t1, t2) ->
                                methodDef.MakeGenericMethod(t1, t2)
                        )
                        .Invoke(null, args)

            // Pre-cache invokers for .NET
            let private invokeOptionOf = makeCachedInvoker "OptionOf"

            let private invokeOptionOfLossless =
                makeCachedInvoker "OptionOfLossless"

            let private invokeSeqOf = makeCachedInvoker "SeqOf"
            let private invokeListOf = makeCachedInvoker "ListOf"
            let private invokeMapOf = makeCachedInvoker2 "MapOf"
            let private invokeMapAsArrayOf = makeCachedInvoker2 "MapAsArrayOf"
            let private invokeSetOf = makeCachedInvoker "SetOf"
            let private invokeArrayOf = makeCachedInvoker "ArrayOf"
            let private invokeLazily = makeCachedInvoker "Lazily"
#endif

#if FABLE_COMPILER
            // Fable implementations - simple boxing
            let optionOf (lossless: bool) (innerType: Type) (enc: obj) : obj =
                if lossless then
                    Encode.losslessOption (unbox enc)
                else
                    Encode.lossyOption (unbox enc)

            let seqOf (innerType: Type) (enc: obj) : obj =
                box (fun xs -> unbox xs |> Seq.map (unbox enc) |> Encode.seq)

            let listOf (innerType: Type) (enc: obj) : obj =
                box (fun xs -> unbox xs |> List.map (unbox enc) |> Encode.list)

            let mapOf
                (keyType: Type)
                (valueType: Type)
                (stringifyKey: obj)
                (enc: obj)
                : obj
                =
                box (fun m ->
                    unbox m
                    |> Map.toSeq
                    |> Seq.map (fun (k, v) ->
                        (unbox stringifyKey) k, (unbox enc) v
                    )
                    |> Map.ofSeq
                    |> Encode.dict
                )

            let mapAsArrayOf
                (keyType: Type)
                (valueType: Type)
                (keyEncoder: obj)
                (valueEncoder: obj)
                : obj
                =
                box (fun xs ->
                    let enc =
                        Encode.tuple2 (unbox keyEncoder) (unbox valueEncoder)

                    unbox xs |> Map.toList |> List.map enc |> Encode.list
                )

            let setOf (innerType: Type) (enc: obj) : obj =
                box (fun xs ->
                    unbox xs
                    |> Seq.map (unbox enc)
                    |> Seq.toArray
                    |> Encode.array
                )

            let arrayOf (innerType: Type) (enc: obj) : obj =
                EncodeHelpers.ArrayOf(unbox enc)

            let lazily (innerType: Type) (enc: obj) : obj =
                Encode.lazily (unbox enc)
#else
            // .NET implementations using cached invokers
            let optionOf (lossless: bool) (innerType: Type) (enc: obj) : obj =
                if lossless then
                    invokeOptionOfLossless innerType [| enc |]
                else
                    invokeOptionOf innerType [| enc |]

            let seqOf (innerType: Type) (enc: obj) : obj =
                invokeSeqOf innerType [| enc |]

            let listOf (innerType: Type) (enc: obj) : obj =
                invokeListOf innerType [| enc |]

            let mapOf
                (keyType: Type)
                (valueType: Type)
                (stringifyKey: obj)
                (enc: obj)
                : obj
                =
                invokeMapOf
                    keyType
                    valueType
                    [|
                        stringifyKey
                        enc
                    |]

            let mapAsArrayOf
                (keyType: Type)
                (valueType: Type)
                (keyEncoder: obj)
                (valueEncoder: obj)
                : obj
                =
                invokeMapAsArrayOf
                    keyType
                    valueType
                    [|
                        keyEncoder
                        valueEncoder
                    |]

            let setOf (innerType: Type) (enc: obj) : obj =
                invokeSetOf innerType [| enc |]

            let arrayOf (innerType: Type) (enc: obj) : obj =
                invokeArrayOf innerType [| enc |]

            let lazily (innerType: Type) (enc: obj) : obj =
                invokeLazily innerType [| enc |]
#endif

            module Enum =

#if FABLE_COMPILER
                let byte (innerType: Type) : obj = Encode.byte |> box
#else
                let private invokeEnumByte = makeCachedInvoker "EnumByte"

                let byte (innerType: Type) : obj = invokeEnumByte innerType [||]
#endif

#if FABLE_COMPILER
                let sbyte (innerType: Type) : obj = Encode.sbyte |> box
#else
                let private invokeEnumSbyte = makeCachedInvoker "EnumSbyte"

                let sbyte (innerType: Type) : obj =
                    invokeEnumSbyte innerType [||]
#endif

#if FABLE_COMPILER
                let int16 (innerType: Type) : obj = Encode.int16 |> box
#else
                let private invokeEnumInt16 = makeCachedInvoker "EnumInt16"

                let int16 (innerType: Type) : obj =
                    invokeEnumInt16 innerType [||]
#endif

#if FABLE_COMPILER
                let uint16 (innerType: Type) : obj = Encode.uint16 |> box
#else
                let private invokeEnumUint16 = makeCachedInvoker "EnumUint16"

                let uint16 (innerType: Type) : obj =
                    invokeEnumUint16 innerType [||]
#endif

#if FABLE_COMPILER
                let int (innerType: Type) : obj = Encode.int |> box
#else
                let private invokeEnumInt = makeCachedInvoker "EnumInt"

                let int (innerType: Type) : obj = invokeEnumInt innerType [||]
#endif

#if FABLE_COMPILER
                let uint32 (innerType: Type) : obj = Encode.uint32 |> box
#else
                let private invokeEnumUint32 = makeCachedInvoker "EnumUint32"

                let uint32 (innerType: Type) : obj =
                    invokeEnumUint32 innerType [||]
#endif

    // let private fieldMethodDefinition = getGenericMethodDefinition "Field"

    // let field (objectType : Type) (fieldType : Type) (picker : obj) (fieldEncoder : obj) : obj =
    //   let methodInfo = fieldMethodDefinition.MakeGenericMethod(objectType, fieldType)
    //   methodInfo.Invoke(null, [| picker; fieldEncoder |])

    // let private objectMethodDefinition = getGenericMethodDefinition "Object"

    // let object (objectType : Type) (fields : obj) : obj =
    //   let methodInfo = objectMethodDefinition.MakeGenericMethod(objectType)
    //   methodInfo.Invoke(null, [| fields |])

    // let private elementMethodDefinition = getGenericMethodDefinition "Element"

    // let element (objectType : Type) (elementType : Type) (picker : obj) (elementEncoder : obj) : obj =
    //   let methodInfo = elementMethodDefinition.MakeGenericMethod(objectType, elementType)
    //   methodInfo.Invoke(null, [| picker; elementEncoder |])

    // let private fixedArrayMethodDefinition = getGenericMethodDefinition "FixedArray"

    // let fixedArray (objectType : Type) (elements : obj) : obj =
    //   let methodInfo = fixedArrayMethodDefinition.MakeGenericMethod(objectType)
    //   methodInfo.Invoke(null, [| elements |])

    // let private unionMethodDefinition = getGenericMethodDefinition "Union"

    // let union (objectType : Type) (picker : obj) : obj =
    //   let methodInfo = unionMethodDefinition.MakeGenericMethod(objectType)
    //   methodInfo.Invoke(null, [| picker |])

    // let private caseMethodDefinition = getGenericMethodDefinition "Case"

    // let case (objectType : Type) (tag : string) (data : obj) : obj =
    //   let methodInfo = caseMethodDefinition.MakeGenericMethod(objectType)
    //   methodInfo.Invoke(null, [| tag; data |])

    // let private makeFieldEncoderType (ty : Type) : Type =
    //   typedefof<Encode.IFieldEncoder<obj>>.MakeGenericType(ty)

    // let private makeEncodeCaseType (ty : Type) : Type =
    //   typedefof<Encode.ICase<obj>>.MakeGenericType(ty)

#if !FABLE_COMPILER
    let private makeEncoderType (ty: Type) : Type =
        FSharpType.MakeFunctionType(ty, typeof<IEncodable>)
    // typedefof<Encoder<obj>>.MakeGenericType(ty)
#endif

#if FABLE_COMPILER
    let private makeLazyEncoder (ty: Type) (getSelf: unit -> obj) : obj =
        Encode.Generic.lazily
            ty
            (Lazy.makeGeneric (typeof<obj -> obj>) (fun _ -> getSelf ()))

    let private wrapBoxedEncoder (encoder: obj) : obj -> IEncodable =
        unbox encoder

    let private wrapFinalEncoder (_ty: Type) (funcImpl: obj -> obj) : obj =
        box funcImpl

    let private makeFieldReader (pi: PropertyInfo) : obj -> obj =
        fun record -> FSharpValue.GetRecordField(record, pi)

    let private makeUnionTagReader (ty: Type) : obj -> int =
        fun o -> (FSharpValue.GetUnionFields(o, ty) |> fst).Tag

    let private makeUnionCaseReader
        (_unionCase: UnionCaseInfo)
        (ty: Type)
        : obj -> obj[]
        =
        fun o -> FSharpValue.GetUnionFields(o, ty) |> snd

    let private makeTupleReader (_ty: Type) : obj -> obj[] =
        fun o -> (unbox<ResizeArray<obj>> o).ToArray()

    let private getUnionCaseName
        (_ty: Type)
        (caseStyle: CaseStrategy option)
        (unionCase: UnionCaseInfo)
        : string
        =
        match caseStyle with
        | Some caseStyle ->
            Casing.convertCase DotNetPascalCase caseStyle unionCase.Name
        | None -> unionCase.Name
#else
    let private makeLazyEncoder (ty: Type) (getSelf: unit -> obj) : obj =
        let encoderType = makeEncoderType ty

        Encode.Generic.lazily
            ty
            (Lazy.makeGeneric
                encoderType
                (FSharpValue.MakeFunction(
                    FSharpType.MakeFunctionType(typeof<unit>, encoderType),
                    fun _ -> getSelf ()
                )))

    let private invokeMethodCache = ConcurrentDictionary<Type, MethodInfo>()

    let private wrapBoxedEncoder (encoder: obj) : obj -> IEncodable =
        let methodInfo =
            invokeMethodCache.GetOrAdd(
                encoder.GetType(),
                fun t ->
                    t.GetMethods()
                    |> Array.find (fun x ->
                        x.Name = "Invoke" && x.ReturnType = typeof<IEncodable>
                    )
            )

        fun value -> methodInfo.Invoke(encoder, [| value |]) :?> IEncodable

    let private wrapFinalEncoder (ty: Type) (funcImpl: obj -> obj) : obj =
        FSharpValue.MakeFunction(makeEncoderType ty, funcImpl)

    let private makeFieldReader (pi: PropertyInfo) : obj -> obj =
        FSharpValue.PreComputeRecordFieldReader(pi)

    let private makeUnionTagReader (ty: Type) : obj -> int =
        FSharpValue.PreComputeUnionTagReader(
            ty,
            allowAccessToPrivateRepresentation = true
        )

    let private makeUnionCaseReader
        (unionCase: UnionCaseInfo)
        (_ty: Type)
        : obj -> obj[]
        =
        FSharpValue.PreComputeUnionReader(
            unionCase,
            allowAccessToPrivateRepresentation = true
        )

    let private makeTupleReader (ty: Type) : obj -> obj[] =
        FSharpValue.PreComputeTupleReader(ty)

    let private getUnionCaseName
        (ty: Type)
        (caseStyle: CaseStrategy option)
        (unionCase: UnionCaseInfo)
        : string
        =
        let fromCase, name =
            match ty with
            | StringEnum ty ->
                match unionCase with
                | CompiledName name -> DotNetPascalCase, name
                | _ ->
                    match ty.ConstructorArguments with
                    | LowerFirst ->
                        let name =
                            unionCase.Name.[..0].ToLowerInvariant()
                            + unionCase.Name.[1..]

                        DotNetCamelCase, name
                    | Forward -> DotNetPascalCase, unionCase.Name
            | _ -> DotNetPascalCase, unionCase.Name

        match caseStyle with
        | Some caseStyle -> Casing.convertCase fromCase caseStyle name
        | None -> name
#endif

    let rec generateEncoder
        (caseStyle: CaseStrategy option)
        (existingEncoders: Map<TypeKey, BoxedEncoder>)
        (skipNullField: bool)
        (losslessOption: bool)
        (ty: Type)
        : BoxedEncoder
        =
        match Map.tryFind (TypeKey.ofType ty) existingEncoders with
        | Some x -> x
        | None ->
            let gen =
                generateEncoder
                    caseStyle
                    existingEncoders
                    skipNullField
                    losslessOption

            match ty with
            | UnitType _ -> box Encode.unit
            | IntType _ -> box Encode.int
            | CharType _ -> box Encode.char
            | StringType _ -> box Encode.string
            | BoolType _ -> box Encode.bool
            | ByteType _ -> box Encode.byte
            | SByteType _ -> box Encode.sbyte
            | UInt16Type _ -> box Encode.uint16
            | Int16Type _ -> box Encode.int16
            | Int64Type _ -> box Encode.int64
            | UIntType _ -> box Encode.uint32
            | UInt64Type _ -> box Encode.uint64
            | BigIntType _ -> box Encode.bigint
            | SingleType _ -> box Encode.float32
            | DoubleType _ -> box Encode.float
            | DecimalType _ -> box Encode.decimal
            | GuidType _ -> box (fun (g: Guid) -> Encode.guid g)
            | TimeSpanType _ -> box (fun (ts: TimeSpan) -> Encode.timespan ts)
            | DateTimeType _ -> box Encode.datetime
            | DateTimeOffsetType _ -> box Encode.datetimeOffset
            | OptionType innerType ->
                Encode.Generic.optionOf losslessOption innerType (gen innerType)
            | SeqType innerType ->
                Encode.Generic.seqOf innerType (gen innerType)
            | ListType innerType ->
                Encode.Generic.listOf innerType (gen innerType)
            | MapType(StringType, valueType) ->
                Encode.Generic.mapOf
                    typeof<string>
                    valueType
                    (fun (s: string) -> s)
                    (gen valueType)
            | MapType(GuidType, valueType) ->
                Encode.Generic.mapOf
                    typeof<Guid>
                    valueType
                    (fun (g: Guid) -> string g)
                    (gen valueType)
            | MapType(keyType, valueType) ->
                Encode.Generic.mapAsArrayOf
                    keyType
                    valueType
                    (gen keyType)
                    (gen valueType)
            | SetType innerType ->
                Encode.Generic.setOf innerType (gen innerType)
            | ArrayType innerType ->
                Encode.Generic.arrayOf innerType (gen innerType)
            | FSharpRecordType _ ->
                generateEncoderForRecord
                    caseStyle
                    existingEncoders
                    skipNullField
                    losslessOption
                    ty
            | FSharpUnionType _ ->
                generateEncoderForUnion
                    caseStyle
                    existingEncoders
                    skipNullField
                    losslessOption
                    ty
            | FSharpTupleType _ ->
                generateEncoderForTuple
                    caseStyle
                    existingEncoders
                    skipNullField
                    losslessOption
                    ty
            | EnumType(ByteType _) -> Encode.Generic.Enum.byte ty
            | EnumType(SByteType _) -> Encode.Generic.Enum.sbyte ty
            | EnumType(Int16Type _) -> Encode.Generic.Enum.int16 ty
            | EnumType(UInt16Type _) -> Encode.Generic.Enum.uint16 ty
            | EnumType(IntType _) -> Encode.Generic.Enum.int ty
            | EnumType(UIntType _) -> Encode.Generic.Enum.uint32 ty
            | _ ->
                failwith
                    $"Encoder generation failed, unsupported type '%s{ty.FullName}'"

    and generateEncoderForRecord
        (caseStyle: CaseStrategy option)
        (existingEncoders: Map<TypeKey, obj>)
        (skipNullField: bool)
        (losslessOption: bool)
        (ty: Type)
        : obj
        =
        let mutable self = Unchecked.defaultof<_>

        let existingEncoders =
            if Type.isRecursive ty then
                let lazySelf = makeLazyEncoder ty (fun () -> self)
                existingEncoders |> Map.add (TypeKey.ofType ty) lazySelf
            else
                existingEncoders

        let recordFieldsWithEncoders =
            [|
                let recordFields =
                    match ty with
                    | FSharpRecordType fields -> fields
                    | _ -> failwith $"Expected an F# record type"

                for pi in recordFields do
                    let fieldEncoder =
                        generateEncoder
                            caseStyle
                            existingEncoders
                            skipNullField
                            losslessOption
                            pi.PropertyType
                        |> wrapBoxedEncoder

                    let reader = makeFieldReader pi

                    let readAndEncode (record: obj) =
                        let value = reader record

                        if skipNullField && isNull value then
                            None
                        else
                            fieldEncoder value |> Some

                    pi.Name, readAndEncode
            |]

        let funcImpl: obj -> obj =
            fun o ->
                let fields =
                    [|
                        for fieldName, readAndEncode in recordFieldsWithEncoders do
                            let encodedFieldName =
                                match caseStyle with
                                | Some caseStyle ->
                                    Casing.convertCase
                                        DotNetPascalCase
                                        caseStyle
                                        fieldName
                                | None -> fieldName

                            match readAndEncode o with
                            | Some encoded -> encodedFieldName, encoded
                            | None -> ()
                    |]

                Encode.object fields

        let encoder = wrapFinalEncoder ty funcImpl

        self <- encoder

        encoder

    and generateEncoderForUnion
        (caseStyle: CaseStrategy option)
        (existingEncoders: Map<TypeKey, obj>)
        (skipNullField: bool)
        (losslessOption: bool)
        (ty: Type)
        : obj
        =
        let mutable self = Unchecked.defaultof<_>

        let existingEncoders =
            if Type.isRecursive ty then
                let lazySelf = makeLazyEncoder ty (fun () -> self)
                existingEncoders |> Map.add (TypeKey.ofType ty) lazySelf
            else
                existingEncoders

        let unionCases =
            match ty with
            | FSharpUnionType cases -> cases
            | _ ->
                failwith $"Expected an F# union type but found %s{ty.FullName}"

        let tagReader = makeUnionTagReader ty

        let caseEncoders =
            [|
                for unionCase in unionCases do
                    let encodedUnionCaseName =
                        getUnionCaseName ty caseStyle unionCase

                    let caseHasData =
                        unionCase.GetFields() |> Seq.isEmpty |> not

                    if caseHasData then
                        let caseReader = makeUnionCaseReader unionCase ty

                        let fieldEncoders =
                            [|
                                for pi in unionCase.GetFields() do
                                    generateEncoder
                                        caseStyle
                                        existingEncoders
                                        skipNullField
                                        losslessOption
                                        pi.PropertyType
                                    |> wrapBoxedEncoder
                            |]

                        let n = Array.length fieldEncoders - 1

                        fun o ->
                            let values = caseReader o

                            Encode.array
                                [|
                                    Encode.string encodedUnionCaseName

                                    for i = 0 to n do
                                        fieldEncoders[i]values[i]
                                |]
                    else
                        fun _ -> Encode.string encodedUnionCaseName
            |]

        let funcImpl: obj -> obj = fun o -> caseEncoders[tagReader o]o

        let encoder = wrapFinalEncoder ty funcImpl

        self <- encoder

        encoder

    and generateEncoderForTuple
        (caseStyle: CaseStrategy option)
        (existingEncoders: Map<TypeKey, obj>)
        (skipNullField: bool)
        (losslessOption: bool)
        (ty: Type)
        : obj
        =
        let reader = makeTupleReader ty

        let encoders =
            [|
                for elementType in FSharpType.GetTupleElements(ty) do
                    generateEncoder
                        caseStyle
                        existingEncoders
                        skipNullField
                        losslessOption
                        elementType
                    |> wrapBoxedEncoder
            |]

        let n = Array.length encoders - 1

        let funcImpl: obj -> obj =
            fun o ->
                let values = reader o

                Encode.array
                    [|
                        for i = 0 to n do
                            encoders[i]values[i]
                    |]

        wrapFinalEncoder ty funcImpl

    let inline autoWithOptions<'t>
        (caseStrategy: CaseStrategy option)
        (extra: ExtraCoders)
        (skipNullField: bool)
        (losslessOption: bool)
        : Encoder<'t>
        =
        let ty = typeof<'t>

        let encoder =
            generateEncoder
                caseStrategy
                extra.EncoderOverrides
                skipNullField
                losslessOption
                ty

        unbox encoder

#if !FABLE_COMPILER
    open System.Threading
#endif

    type private AutoCache =
        Cache<
            struct (string * CaseStrategy option * bool * bool * string),
            BoxedEncoder
         >

    type Auto =
#if FABLE_COMPILER
        static let instance = AutoCache()
#else
        static let instance = new ThreadLocal<_>(fun () -> AutoCache())
#endif

#if FABLE_COMPILER
        static member inline generateEncoder<'T>
#else
        static member generateEncoder<'T>
#endif
            (
                ?caseStrategy: CaseStrategy,
                ?extra: ExtraCoders,
                ?skipNullField: bool,
                ?losslessOption: bool
            )
            : Encoder<'T>
            =
            let extra = defaultArg extra Extra.empty
            let skipNullField = defaultArg skipNullField true
            let losslessOption = defaultArg losslessOption false

            autoWithOptions caseStrategy extra skipNullField losslessOption

#if FABLE_COMPILER
        static member inline generateEncoderCached<'T>
#else
        static member generateEncoderCached<'T>
#endif
            (
                ?caseStrategy: CaseStrategy,
                ?extra: ExtraCoders,
                ?skipNullField: bool,
                ?losslessOption: bool
            )
            : Encoder<'T>
            =
            let extra = defaultArg extra Extra.empty
            let skipNullField = defaultArg skipNullField true
            let losslessOption = defaultArg losslessOption false

            let t = typeof<'T>

            let key =
                struct (t.FullName,
                        caseStrategy,
                        skipNullField,
                        losslessOption,
                        extra.Hash)

#if FABLE_COMPILER
            let cache = instance
#else
            let cache = instance.Value
#endif

            cache.GetOrAdd(
                key,
                fun () ->
                    let enc: Encoder<'T> =
                        autoWithOptions
                            caseStrategy
                            extra
                            skipNullField
                            losslessOption

                    box enc
            )
            |> unbox
