namespace Thoth.Json.Auto

open System
open System.Reflection
open FSharp.Reflection
open Thoth.Json.Core
open Thoth.Json.Auto

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
                    Encode.option enc

                static member Lazily<'t>(enc: Lazy<Encoder<'t>>) : Encoder<'t> =
                    Encode.lazily enc

                static member SeqOf<'t>(enc: Encoder<'t>) : Encoder<'t seq> =
                    fun xs -> xs |> Seq.map enc |> Seq.toArray |> Encode.array

                static member ListOf<'t>(enc: Encoder<'t>) : Encoder<'t list> =
                    fun xs -> xs |> List.map enc |> Encode.list

                static member MapOf<'k, 'v when 'k: comparison>
                    (
                        stringifyKey: 'k -> string,
                        enc: Encoder<'v>
                    )
                    : Encoder<Map<'k, 'v>>
                    =
                    fun m ->
                        [
                            for KeyValue(k, v) in m do
                                stringifyKey k, enc v
                        ]
                        |> Encode.object

                static member MapAsArrayOf<'k, 'v when 'k: comparison>
                    (
                        keyEncoder: Encoder<'k>,
                        valueEncoder: Encoder<'v>
                    )
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

#if FABLE_COMPILER
            let optionOf (innerType: Type) (enc: obj) : obj =
                Encode.option (unbox enc)
#else
            let private getGenericMethodDefinition (name: string) =
                typeof<EncodeHelpers>
                    .GetMethods(BindingFlags.Static ||| BindingFlags.NonPublic)
                |> Seq.filter (fun x -> x.Name = name)
                |> Seq.exactlyOne
                |> fun mi -> mi.GetGenericMethodDefinition()

            let private optionOfMethodDefinition =
                getGenericMethodDefinition "OptionOf"

            let optionOf (innerType: Type) (enc: obj) : obj =
                let methodInfo =
                    optionOfMethodDefinition.MakeGenericMethod(innerType)

                methodInfo.Invoke(null, [| enc |])
#endif

#if FABLE_COMPILER
            let seqOf (innerType: Type) (enc: obj) : obj =
                box (fun xs -> unbox xs |> Seq.map (unbox enc) |> Encode.seq)
#else
            let private seqOfMethodDefinition =
                getGenericMethodDefinition "SeqOf"

            let seqOf (innerType: Type) (enc: obj) : obj =
                let methodInfo =
                    seqOfMethodDefinition.MakeGenericMethod(innerType)

                methodInfo.Invoke(null, [| enc |])
#endif

#if FABLE_COMPILER
            let listOf (innerType: Type) (enc: obj) : obj =
                box (fun xs -> unbox xs |> List.map (unbox enc) |> Encode.list)
#else
            let private listOfMethodDefinition =
                getGenericMethodDefinition "ListOf"

            let listOf (innerType: Type) (enc: obj) : obj =
                let methodInfo =
                    listOfMethodDefinition.MakeGenericMethod(innerType)

                methodInfo.Invoke(null, [| enc |])
#endif

#if FABLE_COMPILER
            let mapOf
                (keyType: Type)
                (valueType: Type)
                (stringifyKey: obj)
                (enc: obj)
                : obj
                =
                (fun m ->
                    let stringifyKey = unbox stringifyKey
                    let enc = unbox enc

                    m
                    |> unbox
                    |> Map.toSeq
                    |> Seq.map (fun (k, v) -> stringifyKey k, enc v)
                    |> Map.ofSeq
                    |> Encode.dict
                )
                |> box
#else
            let private mapOfMethodDefinition =
                getGenericMethodDefinition "MapOf"

            let mapOf
                (keyType: Type)
                (valueType: Type)
                (stringifyKey: obj)
                (enc: obj)
                : obj
                =
                let methodInfo =
                    mapOfMethodDefinition.MakeGenericMethod(keyType, valueType)

                methodInfo.Invoke(
                    null,
                    [|
                        stringifyKey
                        enc
                    |]
                )
#endif

#if FABLE_COMPILER
            let mapAsArrayOf
                (keyType: Type)
                (valueType: Type)
                (keyEncoder: obj)
                (valueEncoder: obj)
                : obj
                =
                (fun xs ->
                    let enc =
                        Encode.tuple2 (unbox keyEncoder) (unbox valueEncoder)

                    unbox xs |> Map.toList |> List.map enc |> Encode.list
                )
                |> box
#else
            let private mapAsArrayOfMethodDefinition =
                getGenericMethodDefinition "MapAsArrayOf"

            let mapAsArrayOf
                (keyType: Type)
                (valueType: Type)
                (keyEncoder: obj)
                (valueEncoder: obj)
                : obj
                =
                let methodInfo =
                    mapAsArrayOfMethodDefinition.MakeGenericMethod(
                        keyType,
                        valueType
                    )

                methodInfo.Invoke(
                    null,
                    [|
                        keyEncoder
                        valueEncoder
                    |]
                )
#endif

#if FABLE_COMPILER
            let setOf (innerType: Type) (enc: obj) : obj =
                box (fun xs ->
                    unbox xs
                    |> Seq.map (unbox enc)
                    |> Seq.toArray
                    |> Encode.array
                )
#else
            let private setOfMethodDefinition =
                getGenericMethodDefinition "SetOf"

            let setOf (innerType: Type) (enc: obj) : obj =
                let methodInfo =
                    setOfMethodDefinition.MakeGenericMethod(innerType)

                methodInfo.Invoke(null, [| enc |])
#endif

#if FABLE_COMPILER
            let arrayOf (innerType: Type) (enc: obj) : obj =
                EncodeHelpers.ArrayOf(unbox enc)
#else
            let private arrayOfMethodDefinition =
                getGenericMethodDefinition "ArrayOf"

            let arrayOf (innerType: Type) (enc: obj) : obj =
                let methodInfo =
                    arrayOfMethodDefinition.MakeGenericMethod(innerType)

                methodInfo.Invoke(null, [| enc |])
#endif

#if FABLE_COMPILER
            let lazily (innerType: Type) (enc: obj) : obj =
                Encode.lazily (unbox enc)
#else
            let private lazilyMethodDefinition =
                getGenericMethodDefinition "Lazily"

            let lazily (innerType: Type) (enc: obj) : obj =
                let methodInfo =
                    lazilyMethodDefinition.MakeGenericMethod(innerType)

                methodInfo.Invoke(null, [| enc |])
#endif

            module Enum =

#if FABLE_COMPILER
                let byte (innerType: Type) : obj = Encode.byte |> box
#else
                let private enumByteGenericMethodDefinition =
                    getGenericMethodDefinition "EnumByte"

                let byte (innerType: Type) : obj =
                    enumByteGenericMethodDefinition
                        .MakeGenericMethod([| innerType |])
                        .Invoke(null, [||])
#endif

#if FABLE_COMPILER
                let sbyte (innerType: Type) : obj = Encode.sbyte |> box
#else
                let private enumSbyteGenericMethodDefinition =
                    getGenericMethodDefinition "EnumSbyte"

                let sbyte (innerType: Type) : obj =
                    enumSbyteGenericMethodDefinition
                        .MakeGenericMethod([| innerType |])
                        .Invoke(null, [||])
#endif

#if FABLE_COMPILER
                let int16 (innerType: Type) : obj = Encode.int16 |> box
#else
                let private enumInt16GenericMethodDefinition =
                    getGenericMethodDefinition "EnumInt16"

                let int16 (innerType: Type) : obj =
                    enumInt16GenericMethodDefinition
                        .MakeGenericMethod([| innerType |])
                        .Invoke(null, [||])
#endif

#if FABLE_COMPILER
                let uint16 (innerType: Type) : obj = Encode.uint16 |> box
#else
                let private enumUint16GenericMethodDefinition =
                    getGenericMethodDefinition "EnumUint16"

                let uint16 (innerType: Type) : obj =
                    enumUint16GenericMethodDefinition
                        .MakeGenericMethod([| innerType |])
                        .Invoke(null, [||])
#endif

#if FABLE_COMPILER
                let int (innerType: Type) : obj = Encode.int |> box
#else
                let private enumIntGenericMethodDefinition =
                    getGenericMethodDefinition "EnumInt"

                let int (innerType: Type) : obj =
                    enumIntGenericMethodDefinition
                        .MakeGenericMethod([| innerType |])
                        .Invoke(null, [||])
#endif

#if FABLE_COMPILER
                let uint32 (innerType: Type) : obj = Encode.uint32 |> box
#else
                let private enumUint32GenericMethodDefinition =
                    getGenericMethodDefinition "EnumUint32"

                let uint32 (innerType: Type) : obj =
                    enumUint32GenericMethodDefinition
                        .MakeGenericMethod([| innerType |])
                        .Invoke(null, [||])
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

    let rec generateEncoder
        (caseStyle: CaseStyle option)
        (existingEncoders: Map<TypeKey, BoxedEncoder>)
        (skipNullField: bool)
        (ty: Type)
        : BoxedEncoder
        =
        match Map.tryFind (TypeKey.ofType ty) existingEncoders with
        | Some x -> x
        | None ->
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
                let innerEncoder =
                    generateEncoder
                        caseStyle
                        existingEncoders
                        skipNullField
                        innerType

                Encode.Generic.optionOf innerType innerEncoder
            | SeqType innerType ->
                let innerEncoder =
                    generateEncoder
                        caseStyle
                        existingEncoders
                        skipNullField
                        innerType

                Encode.Generic.seqOf innerType innerEncoder
            | ListType innerType ->
                let innerEncoder =
                    generateEncoder
                        caseStyle
                        existingEncoders
                        skipNullField
                        innerType

                Encode.Generic.listOf innerType innerEncoder
            | MapType(StringType, valueType) ->
                let valueEncoder =
                    generateEncoder
                        caseStyle
                        existingEncoders
                        skipNullField
                        valueType

                let stringifyKey = fun (s: string) -> s

                Encode.Generic.mapOf
                    typeof<string>
                    valueType
                    stringifyKey
                    valueEncoder
            | MapType(GuidType, valueType) ->
                let valueEncoder =
                    generateEncoder
                        caseStyle
                        existingEncoders
                        skipNullField
                        valueType

                let stringifyKey = fun (g: Guid) -> string g

                Encode.Generic.mapOf
                    typeof<Guid>
                    valueType
                    stringifyKey
                    valueEncoder
            | MapType(keyType, valueType) ->
                let keyEncoder =
                    generateEncoder
                        caseStyle
                        existingEncoders
                        skipNullField
                        keyType

                let valueEncoder =
                    generateEncoder
                        caseStyle
                        existingEncoders
                        skipNullField
                        valueType

                Encode.Generic.mapAsArrayOf
                    keyType
                    valueType
                    keyEncoder
                    valueEncoder
            | SetType innerType ->
                let innerEncoder =
                    generateEncoder
                        caseStyle
                        existingEncoders
                        skipNullField
                        innerType

                Encode.Generic.setOf innerType innerEncoder
            | ArrayType innerType ->
                let innerEncoder =
                    generateEncoder
                        caseStyle
                        existingEncoders
                        skipNullField
                        innerType

                Encode.Generic.arrayOf innerType innerEncoder
            | FSharpRecordType _ ->
                generateEncoderForRecord
                    caseStyle
                    existingEncoders
                    skipNullField
                    ty
            | FSharpUnionType _ ->
                generateEncoderForUnion
                    caseStyle
                    existingEncoders
                    skipNullField
                    ty
            | FSharpTupleType _ ->
                generateEncoderForTuple
                    caseStyle
                    existingEncoders
                    skipNullField
                    ty
            | EnumType(ByteType _) -> Encode.Generic.Enum.byte ty
            | EnumType(SByteType _) -> Encode.Generic.Enum.sbyte ty
            | EnumType(Int16Type _) -> Encode.Generic.Enum.int16 ty
            | EnumType(UInt16Type _) -> Encode.Generic.Enum.uint16 ty
            | EnumType(IntType _) -> Encode.Generic.Enum.int ty
            | EnumType(UIntType _) -> Encode.Generic.Enum.uint32 ty
            | _ -> failwith $"Unsupported type %s{ty.FullName}"

    and generateEncoderForRecord
        (caseStyle: CaseStyle option)
        (existingEncoders: Map<TypeKey, obj>)
        (skipNullField: bool)
        (ty: Type)
        : obj
        =
#if FABLE_COMPILER
        let mutable self = Unchecked.defaultof<_>

        let existingEncoders =
            if Type.isRecursive ty then
                let lazySelf =
                    Encode.Generic.lazily
                        ty
                        (Lazy.makeGeneric (typeof<obj -> obj>) ((fun _ -> self)))

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
                    let fieldEncoder: obj -> IEncodable =
                        generateEncoder
                            caseStyle
                            existingEncoders
                            skipNullField
                            pi.PropertyType
                        |> unbox

                    let reader =
                        fun (record: obj) ->
                            FSharpValue.GetRecordField(record, pi)

                    let readAndEncode (record: obj) =
                        let value = reader record

                        if skipNullField && isNull value then
                            None
                        else
                            fieldEncoder value |> Some

                    pi.Name, readAndEncode
            |]

        let encoder: obj -> obj =
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

        self <- box encoder

        box encoder
#else
        let mutable self = Unchecked.defaultof<_>

        let existingEncoders =
            if Type.isRecursive ty then
                let lazySelf =
                    Encode.Generic.lazily
                        ty
                        (Lazy.makeGeneric
                            (makeEncoderType ty)
                            (FSharpValue.MakeFunction(
                                FSharpType.MakeFunctionType(
                                    typeof<unit>,
                                    makeEncoderType ty
                                ),
                                (fun _ -> self)
                            )))

                existingEncoders |> Map.add (TypeKey.ofType ty) lazySelf
            else
                existingEncoders

        let funcType = makeEncoderType ty

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
                            pi.PropertyType

                    let invokeMethodInfo =
                        fieldEncoder.GetType().GetMethods()
                        |> Array.find (fun x ->
                            x.Name = "Invoke"
                            && x.ReturnType = typedefof<IEncodable>
                        )

                    let reader = FSharpValue.PreComputeRecordFieldReader(pi)

                    let readAndEncode (record: obj) =
                        let value = reader record

                        if skipNullField && isNull value then
                            None
                        else
                            invokeMethodInfo.Invoke(fieldEncoder, [| value |])
                            :?> IEncodable
                            |> Some

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

        let encoder = FSharpValue.MakeFunction(funcType, funcImpl)

        self <- encoder

        encoder
#endif

    and generateEncoderForUnion
        (caseStyle: CaseStyle option)
        (existingEncoders: Map<TypeKey, obj>)
        (skipNullField: bool)
        (ty: Type)
        : obj
        =
#if FABLE_COMPILER
        let mutable self = Unchecked.defaultof<_>

        let existingEncoders =
            if Type.isRecursive ty then
                let lazySelf =
                    Encode.Generic.lazily
                        ty
                        (Lazy.makeGeneric (typeof<obj -> obj>) ((fun _ -> self)))

                existingEncoders |> Map.add (TypeKey.ofType ty) lazySelf
            else
                existingEncoders

        let unionCases =
            match ty with
            | FSharpUnionType cases -> cases
            | _ ->
                failwith $"Expected an F# union type but found %s{ty.FullName}"

        let caseEncoders =
            [|
                for unionCase in unionCases do
                    let name =
                        match ty with
#if !FABLE_COMPILER
#endif
                        | _ -> unionCase.Name

                    let encodedUnionCaseName =
                        match caseStyle with
                        | Some caseStyle ->
                            Casing.convertCase
                                DotNetPascalCase
                                caseStyle
                                unionCase.Name
                        | None -> unionCase.Name

                    let caseHasData =
                        unionCase.GetFields() |> Seq.isEmpty |> not

                    if caseHasData then
                        let fieldEncoders =
                            [|
                                for pi in unionCase.GetFields() do
                                    let encoder: obj -> IEncodable =
                                        generateEncoder
                                            caseStyle
                                            existingEncoders
                                            skipNullField
                                            pi.PropertyType
                                        |> unbox

                                    encoder
                            |]

                        let n = Array.length fieldEncoders - 1

                        fun o ->
                            let _, values = FSharpValue.GetUnionFields(o, ty)

                            Encode.array
                                [|
                                    Encode.string encodedUnionCaseName

                                    for i = 0 to n do
                                        let value = values[i]

                                        let encoder: obj -> IEncodable =
                                            unbox fieldEncoders[i]

                                        encoder value
                                |]
                    else
                        fun _ -> Encode.string encodedUnionCaseName
            |]

        let encoder: obj -> obj =
            fun o ->
                let caseInfo, _ = FSharpValue.GetUnionFields(o, ty)
                let tag = caseInfo.Tag
                let caseEncoder = caseEncoders[tag]

                caseEncoder o

        self <- encoder

        encoder
#else
        let mutable self = Unchecked.defaultof<_>

        let funcType = makeEncoderType ty

        let existingEncoders =
            if Type.isRecursive ty then
                let lazySelf =
                    Encode.Generic.lazily
                        ty
                        (Lazy.makeGeneric
                            (makeEncoderType ty)
                            (FSharpValue.MakeFunction(
                                FSharpType.MakeFunctionType(
                                    typeof<unit>,
                                    makeEncoderType ty
                                ),
                                (fun _ -> self)
                            )))

                existingEncoders |> Map.add (TypeKey.ofType ty) lazySelf
            else
                existingEncoders

        let unionCases =
            match ty with
            | FSharpUnionType cases -> cases
            | _ ->
                failwith $"Expected an F# union type but found %s{ty.FullName}"

        let tagReader =
            FSharpValue.PreComputeUnionTagReader(
                ty,
                allowAccessToPrivateRepresentation = true
            )

        let caseEncoders =
            [|
                for unionCase in unionCases do
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

                    let encodedUnionCaseName =
                        match caseStyle with
                        | Some caseStyle ->
                            Casing.convertCase fromCase caseStyle name
                        | None -> name

                    let caseHasData =
                        unionCase.GetFields() |> Seq.isEmpty |> not

                    if caseHasData then
                        let unionReader =
                            FSharpValue.PreComputeUnionReader(
                                unionCase,
                                allowAccessToPrivateRepresentation = true
                            )

                        let fieldEncoders =
                            [|
                                for pi in unionCase.GetFields() do
                                    let encoder =
                                        generateEncoder
                                            caseStyle
                                            existingEncoders
                                            skipNullField
                                            pi.PropertyType

                                    let invokeMethodInfo =
                                        encoder.GetType().GetMethods()
                                        |> Array.find (fun x ->
                                            x.Name = "Invoke"
                                            && x.ReturnType = typeof<IEncodable>
                                        )

                                    fun o ->
                                        invokeMethodInfo.Invoke(
                                            encoder,
                                            [| o |]
                                        )
                                        :?> IEncodable
                            |]

                        let n = Array.length fieldEncoders - 1

                        fun o ->
                            let values = unionReader o

                            Encode.array
                                [|
                                    Encode.string encodedUnionCaseName

                                    for i = 0 to n do
                                        let value = values[i]
                                        let encoder = fieldEncoders[i]

                                        encoder value
                                |]
                    else
                        fun _ -> Encode.string encodedUnionCaseName
            |]

        let funcImpl: obj -> obj =
            fun o ->
                let tag = tagReader o
                let caseEncoder = caseEncoders[tag]

                caseEncoder o

        let encoder = FSharpValue.MakeFunction(funcType, funcImpl)

        self <- encoder

        encoder
#endif

    and generateEncoderForTuple
        (caseStyle: CaseStyle option)
        (existingEncoders: Map<TypeKey, obj>)
        (skipNullField: bool)
        (ty: Type)
        : obj
        =
#if FABLE_COMPILER
        let encoders =
            [|
                for elementType in FSharpType.GetTupleElements(ty) do
                    let elementEncoder =
                        generateEncoder
                            caseStyle
                            existingEncoders
                            skipNullField
                            elementType

                    box elementEncoder
            |]

        let funcImpl: obj -> obj =
            fun o ->
                let values: ResizeArray<obj> = unbox o

                Encode.array
                    [|
                        for i = 0 to Array.length encoders - 1 do
                            let value = unbox values[i]
                            let encode: obj -> IEncodable = unbox encoders[i]

                            encode value
                    |]

        box funcImpl
#else
        let funcType = makeEncoderType ty

        let reader = FSharpValue.PreComputeTupleReader(ty)

        let encoders =
            [|
                for elementType in FSharpType.GetTupleElements(ty) do
                    let elementEncoder =
                        generateEncoder
                            caseStyle
                            existingEncoders
                            skipNullField
                            elementType

                    let invokeMethodInfo =
                        elementEncoder.GetType().GetMethods()
                        |> Array.find (fun x ->
                            x.Name = "Invoke"
                            && x.ReturnType = typedefof<IEncodable>
                        )

                    let encode (value: obj) =
                        invokeMethodInfo.Invoke(elementEncoder, [| value |])
                        :?> IEncodable

                    encode
            |]

        let n = Array.length encoders - 1

        let funcImpl: obj -> obj =
            fun o ->
                let values = reader o

                let elements =
                    [|
                        for i = 0 to n do
                            let value = values[i]
                            let encode = encoders[i]

                            encode value
                    |]

                Encode.array elements

        FSharpValue.MakeFunction(funcType, funcImpl)
#endif

    let inline autoWithOptions<'t>
        (caseStrategy: CaseStyle option)
        (extra: ExtraCoders)
        (skipNullField: bool)
        : Encoder<'t>
        =
        let ty = typeof<'t>

        let encoder =
            generateEncoder caseStrategy extra.EncoderOverrides skipNullField ty

        unbox encoder

#if !FABLE_COMPILER
    open System.Threading
#endif

    type Auto =
#if FABLE_COMPILER
        static let instance = Cache<BoxedEncoder>()
#else
        static let instance =
            new ThreadLocal<_>(fun () -> Cache<BoxedEncoder>())
#endif

#if FABLE_COMPILER
        static member inline generateEncoder<'T>
#else
        static member generateEncoder<'T>
#endif
            (
                ?caseStrategy: CaseStyle,
                ?extra: ExtraCoders,
                ?skipNullField: bool
            )
            : Encoder<'T>
            =
            let extra = defaultArg extra Extra.empty
            let skipNullField = defaultArg skipNullField true

            autoWithOptions caseStrategy extra skipNullField

#if FABLE_COMPILER
        static member inline generateEncoderCached<'T>
#else
        static member generateEncoderCached<'T>
#endif
            (
                ?caseStrategy: CaseStyle,
                ?extra: ExtraCoders,
                ?skipNullField: bool
            )
            : Encoder<'T>
            =
            let extra = defaultArg extra Extra.empty
            let skipNullField = defaultArg skipNullField true

            let t = typeof<'T>

            let key =
                t.FullName
                |> (+) (Operators.string caseStrategy)
                |> (+) (Operators.string skipNullField)
                |> (+) extra.Hash

#if FABLE_COMPILER
            let cache = instance
#else
            let cache = instance.Value
#endif

            cache.GetOrAdd(
                key,
                fun () ->
                    let enc: Encoder<'T> =
                        autoWithOptions caseStrategy extra skipNullField

                    box enc
            )
            |> unbox
