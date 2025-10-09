namespace Thoth.Json.Auto

open System
open System.Reflection
open FSharp.Reflection
open Thoth.Json.Core

[<RequireQualifiedAccess>]
module Decode =

    [<AutoOpen>]
    module private Helpers =

        type DecodeHelpers =
            static member inline Lazily<'t>
                (x: Lazy<Decoder<'t>>)
                : Decoder<'t>
                =
                Decode.lazily x

            static member Option<'t>(x: Decoder<'t>) : Decoder<'t option> =
                Decode.lossyOption x

            static member List<'t>(x: Decoder<'t>) : Decoder<'t list> =
                Decode.list x

            static member Array<'t>(x: Decoder<'t>) : Decoder<'t array> =
                Decode.array x

            static member Seq<'t>(x: Decoder<'t>) : Decoder<'t seq> =
                Decode.list x |> Decode.map Seq.ofList

            static member Set<'t when 't: comparison>
                (x: Decoder<'t>)
                : Decoder<Set<'t>>
                =
                Decode.list x |> Decode.map Set.ofList

            static member Dict<'t>(x: Decoder<'t>) : Decoder<Map<string, 't>> =
                Decode.dict x

            static member MapAsArray<'k, 'v when 'k: comparison>
                (keyDecoder: Decoder<'k>, valueDecoder: Decoder<'v>)
                : Decoder<Map<'k, 'v>>
                =
                Decode.map' keyDecoder valueDecoder

            static member Field<'t>
                (name: string, x: Decoder<'t>)
                : Decoder<'t>
                =
                Decode.field name x

            static member Optional<'t>
                (name: string, x: Decoder<'t>)
                : Decoder<'t option>
                =
                Decode.optional name x

            static member Index<'t>(index: int, x: Decoder<'t>) : Decoder<'t> =
                Decode.index index x

            static member Succeed<'t>(x: 't) : Decoder<'t> = Decode.succeed x

            static member Fail<'t>(x: string) : Decoder<'t> = Decode.fail x

            static member Bind<'t, 'u>
                (f: 't -> Decoder<'u>, x: Decoder<'t>)
                : Decoder<'u>
                =
                Decode.andThen f x

            static member Map<'t, 'u>
                (f: 't -> 'u, x: Decoder<'t>)
                : Decoder<'u>
                =
                Decode.map f x

            static member Zip<'a, 'b>
                (x: Decoder<'a>, y: Decoder<'b>)
                : Decoder<'a * 'b>
                =
                Decode.map2 (fun x y -> x, y) x y

            static member Either<'t>
                (x: Decoder<'t>, y: Decoder<'t>)
                : Decoder<'t>
                =
                Decode.oneOf
                    [
                        x
                        y
                    ]

            static member inline EnumByte<'t when 't: enum<byte>>
                ()
                : Decoder<'t>
                =
                Decode.Enum.byte

            static member inline EnumSbyte<'t when 't: enum<sbyte>>
                ()
                : Decoder<'t>
                =
                Decode.Enum.sbyte

            static member inline EnumInt16<'t when 't: enum<int16>>
                ()
                : Decoder<'t>
                =
                Decode.Enum.int16

            static member inline EnumUint16<'t when 't: enum<uint16>>
                ()
                : Decoder<'t>
                =
                Decode.Enum.uint16

            static member inline EnumInt<'t when 't: enum<int>>
                ()
                : Decoder<'t>
                =
                Decode.Enum.int

            static member inline EnumUint32<'t when 't: enum<uint32>>
                ()
                : Decoder<'t>
                =
                Decode.Enum.uint32

#if !FABLE_COMPILER
        let getGenericMethodDefinition (name: string) : MethodInfo =
            typeof<DecodeHelpers>
                .GetMethods(BindingFlags.Static ||| BindingFlags.NonPublic)
            |> Seq.filter (fun x -> x.Name = name)
            |> Seq.exactlyOne
            |> fun mi -> mi.GetGenericMethodDefinition()
#endif

        let makeDecoderType (ty: Type) : Type =
            typedefof<Decoder<_>>.MakeGenericType([| ty |])

        [<RequireQualifiedAccess>]
        module internal Decode =

            [<RequireQualifiedAccess>]
            module Generic =

#if FABLE_COMPILER
                let option (innerType: Type) (decoder: obj) : obj =
                    Decode.lossyOption (unbox decoder) |> box
#else
                let private optionGenericMethodDefinition =
                    getGenericMethodDefinition "Option"

                let option (innerType: Type) (decoder: obj) : obj =
                    optionGenericMethodDefinition
                        .MakeGenericMethod([| innerType |])
                        .Invoke(null, [| decoder |])
#endif

#if FABLE_COMPILER
                let seq (innerType: Type) (decoder: obj) : obj =
                    Decode.list (unbox decoder) |> box
#else
                let private seqGenericMethodDefinition =
                    getGenericMethodDefinition "Seq"

                let seq (innerType: Type) (decoder: obj) : obj =
                    seqGenericMethodDefinition
                        .MakeGenericMethod([| innerType |])
                        .Invoke(null, [| decoder |])
#endif

#if FABLE_COMPILER
                let set (innerType: Type) (decoder: obj) : obj =
                    Decode.list (unbox decoder) |> Decode.map Set.ofList |> box
#else
                let private setGenericMethodDefinition =
                    getGenericMethodDefinition "Set"

                let set (innerType: Type) (decoder: obj) : obj =
                    setGenericMethodDefinition
                        .MakeGenericMethod([| innerType |])
                        .Invoke(null, [| decoder |])
#endif

#if FABLE_COMPILER
                let list (innerType: Type) (decoder: obj) : obj =
                    Decode.list (unbox decoder) |> box
#else
                let private listGenericMethodDefinition =
                    getGenericMethodDefinition "List"

                let list (innerType: Type) (decoder: obj) : obj =
                    listGenericMethodDefinition
                        .MakeGenericMethod([| innerType |])
                        .Invoke(null, [| decoder |])
#endif

#if FABLE_COMPILER
                let array (innerType: Type) (decoder: obj) : obj =
                    Decode.array (unbox decoder) |> box
#else
                let private arrayGenericMethodDefinition =
                    getGenericMethodDefinition "Array"

                let array (innerType: Type) (decoder: obj) : obj =
                    arrayGenericMethodDefinition
                        .MakeGenericMethod([| innerType |])
                        .Invoke(null, [| decoder |])
#endif

#if FABLE_COMPILER
                let dict (innerType: Type) (decoder: obj) : obj =
                    Decode.dict (unbox decoder) |> box
#else
                let private dictGenericMethodDefinition =
                    getGenericMethodDefinition "Dict"

                let dict (innerType: Type) (decoder: obj) : obj =
                    dictGenericMethodDefinition
                        .MakeGenericMethod([| innerType |])
                        .Invoke(null, [| decoder |])
#endif

#if FABLE_COMPILER
                let mapAsArray
                    (keyType: Type)
                    (valueType: Type)
                    (keyDecoder: obj)
                    (valueDecoder: obj)
                    : obj
                    =
                    Decode.map' (unbox keyDecoder) (unbox valueDecoder) |> box
#else
                let private mapAsArrayMethodDefinition =
                    getGenericMethodDefinition "MapAsArray"

                let mapAsArray
                    (keyType: Type)
                    (valueType: Type)
                    (keyDecoder: obj)
                    (valueDecoder: obj)
                    : obj
                    =
                    mapAsArrayMethodDefinition
                        .MakeGenericMethod(
                            [|
                                keyType
                                valueType
                            |]
                        )
                        .Invoke(
                            null,
                            [|
                                keyDecoder
                                valueDecoder
                            |]
                        )
#endif

#if FABLE_COMPILER
                let map
                    (fromType: Type)
                    (toType: Type)
                    (func: obj)
                    (decoder: obj)
                    : obj
                    =
                    Decode.map (unbox func) (unbox decoder) |> box
#else
                let private mapGenericMethodDefinition =
                    getGenericMethodDefinition "Map"

                let map
                    (fromType: Type)
                    (toType: Type)
                    (func: obj)
                    (decoder: obj)
                    : obj
                    =
                    mapGenericMethodDefinition
                        .MakeGenericMethod(
                            [|
                                fromType
                                toType
                            |]
                        )
                        .Invoke(
                            null,
                            [|
                                func
                                decoder
                            |]
                        )
#endif

#if FABLE_COMPILER
                let zip
                    (leftType: Type)
                    (rightType: Type)
                    (leftDecoder: obj)
                    (rightDecoder: obj)
                    : obj
                    =
                    Decode.map2
                        (fun x y -> x, y)
                        (unbox leftDecoder)
                        (unbox rightDecoder)
                    |> box
#else
                let private zipGenericMethodDefinition =
                    getGenericMethodDefinition "Zip"

                let zip
                    (leftType: Type)
                    (rightType: Type)
                    (leftDecoder: obj)
                    (rightDecoder: obj)
                    : obj
                    =
                    zipGenericMethodDefinition
                        .MakeGenericMethod(
                            [|
                                leftType
                                rightType
                            |]
                        )
                        .Invoke(
                            null,
                            [|
                                leftDecoder
                                rightDecoder
                            |]
                        )
#endif

#if FABLE_COMPILER
                let lazily (innerType: Type) (x: obj) : obj =
                    Decode.lazily (unbox x) |> box
#else
                let private lazilyGenericMethodDefinition =
                    getGenericMethodDefinition "Lazily"

                let lazily (innerType: Type) (x: obj) : obj =
                    lazilyGenericMethodDefinition
                        .MakeGenericMethod([| innerType |])
                        .Invoke(null, [| x |])
#endif

#if FABLE_COMPILER
                let succeed (innerType: Type) (x: obj) : obj =
                    Decode.succeed (unbox x) |> box
#else
                let private succeedGenericMethodDefinition =
                    getGenericMethodDefinition "Succeed"

                let succeed (innerType: Type) (x: obj) : obj =
                    succeedGenericMethodDefinition
                        .MakeGenericMethod([| innerType |])
                        .Invoke(null, [| x |])
#endif

#if FABLE_COMPILER
                let fail (innerType: Type) (x: string) : obj =
                    Decode.fail x |> box
#else
                let private failGenericMethodDefinition =
                    getGenericMethodDefinition "Fail"

                let fail (innerType: Type) (x: string) : obj =
                    failGenericMethodDefinition
                        .MakeGenericMethod([| innerType |])
                        .Invoke(null, [| x |])
#endif

#if FABLE_COMPILER
                let index (innerType: Type) (index: int) (decoder: obj) : obj =
                    Decode.index index (unbox decoder) |> box
#else
                let private indexGenericMethodDefinition =
                    getGenericMethodDefinition "Index"

                let index (innerType: Type) (index: int) (decoder: obj) : obj =
                    indexGenericMethodDefinition
                        .MakeGenericMethod([| innerType |])
                        .Invoke(
                            null,
                            [|
                                index
                                decoder
                            |]
                        )
#endif

#if FABLE_COMPILER
                let field
                    (innerType: Type)
                    (name: string)
                    (decoder: obj)
                    : obj
                    =
                    Decode.field name (unbox decoder) |> box
#else
                let private fieldGenericMethodDefinition =
                    getGenericMethodDefinition "Field"

                let field
                    (innerType: Type)
                    (name: string)
                    (decoder: obj)
                    : obj
                    =
                    fieldGenericMethodDefinition
                        .MakeGenericMethod([| innerType |])
                        .Invoke(
                            null,
                            [|
                                name
                                decoder
                            |]
                        )
#endif

#if FABLE_COMPILER
                let optional
                    (innerType: Type)
                    (name: string)
                    (decoder: obj)
                    : obj
                    =
                    Decode.optional name (unbox decoder) |> box
#else
                let private optionalGenericMethodDefinition =
                    getGenericMethodDefinition "Optional"

                let optional
                    (innerType: Type)
                    (name: string)
                    (decoder: obj)
                    : obj
                    =
                    optionalGenericMethodDefinition
                        .MakeGenericMethod([| innerType |])
                        .Invoke(
                            null,
                            [|
                                name
                                decoder
                            |]
                        )
#endif

#if FABLE_COMPILER
                let bind
                    (fromType: Type)
                    (toType: Type)
                    (func: obj)
                    (decoder: obj)
                    : obj
                    =
                    Decode.andThen (unbox func) (unbox decoder) |> box
#else
                let private bindGenericMethodDefinition =
                    getGenericMethodDefinition "Bind"

                let bind
                    (fromType: Type)
                    (toType: Type)
                    (func: obj)
                    (decoder: obj)
                    : obj
                    =
                    bindGenericMethodDefinition
                        .MakeGenericMethod(
                            [|
                                fromType
                                toType
                            |]
                        )
                        .Invoke(
                            null,
                            [|
                                func
                                decoder
                            |]
                        )
#endif

#if FABLE_COMPILER
                let either
                    (innerType: Type)
                    (decoderA: obj)
                    (decoderB: obj)
                    : obj
                    =
                    Decode.oneOf
                        [
                            unbox decoderA
                            unbox decoderB
                        ]
                    |> box
#else
                let private eitherGenericMethodDefinition =
                    getGenericMethodDefinition "Either"

                let either
                    (innerType: Type)
                    (decoderA: obj)
                    (decoderB: obj)
                    : obj
                    =
                    eitherGenericMethodDefinition
                        .MakeGenericMethod([| innerType |])
                        .Invoke(
                            null,
                            [|
                                decoderA
                                decoderB
                            |]
                        )
#endif

                module Enum =
#if FABLE_COMPILER
                    let checkEnumValue (innerType: Type) =
                        (fun value ->
                            if System.Enum.IsDefined(innerType, value) then
                                value |> Decode.succeed
                            else
                                { new Decoder<_> with
                                    member _.Decode<'JsonValue>
                                        (_, value: 'JsonValue)
                                        =
                                        ("",
                                         BadPrimitiveExtra(
                                             innerType.FullName,
                                             value,
                                             "Unkown value provided for the enum"
                                         ))
                                        |> Error
                                }
                        )
#endif

#if FABLE_COMPILER
                    let byte (innerType: Type) : obj =
                        Decode.byte
                        |> Decode.andThen (checkEnumValue innerType)
                        |> box
#else
                    let private enumByteGenericMethodDefinition =
                        getGenericMethodDefinition "EnumByte"

                    let byte (innerType: Type) : obj =
                        enumByteGenericMethodDefinition
                            .MakeGenericMethod([| innerType |])
                            .Invoke(null, [||])
#endif

#if FABLE_COMPILER
                    let sbyte (innerType: Type) : obj =
                        Decode.sbyte
                        |> Decode.andThen (checkEnumValue innerType)
                        |> box
#else
                    let private enumSbyteGenericMethodDefinition =
                        getGenericMethodDefinition "EnumSbyte"

                    let sbyte (innerType: Type) : obj =
                        enumSbyteGenericMethodDefinition
                            .MakeGenericMethod([| innerType |])
                            .Invoke(null, [||])
#endif

#if FABLE_COMPILER
                    let int16 (innerType: Type) : obj =
                        Decode.int16
                        |> Decode.andThen (checkEnumValue innerType)
                        |> box
#else
                    let private enumInt16GenericMethodDefinition =
                        getGenericMethodDefinition "EnumInt16"

                    let int16 (innerType: Type) : obj =
                        enumInt16GenericMethodDefinition
                            .MakeGenericMethod([| innerType |])
                            .Invoke(null, [||])
#endif

#if FABLE_COMPILER
                    let uint16 (innerType: Type) : obj =
                        Decode.int16
                        |> Decode.andThen (checkEnumValue innerType)
                        |> box
#else
                    let private enumUint16GenericMethodDefinition =
                        getGenericMethodDefinition "EnumUint16"

                    let uint16 (innerType: Type) : obj =
                        enumUint16GenericMethodDefinition
                            .MakeGenericMethod([| innerType |])
                            .Invoke(null, [||])
#endif

#if FABLE_COMPILER
                    let int (innerType: Type) : obj =
                        Decode.int
                        |> Decode.andThen (checkEnumValue innerType)
                        |> box
#else
                    let private enumIntGenericMethodDefinition =
                        getGenericMethodDefinition "EnumInt"

                    let int (innerType: Type) : obj =
                        enumIntGenericMethodDefinition
                            .MakeGenericMethod([| innerType |])
                            .Invoke(null, [||])
#endif

#if FABLE_COMPILER
                    let uint32 (innerType: Type) : obj =
                        Decode.uint32
                        |> Decode.andThen (checkEnumValue innerType)
                        |> box
#else
                    let private enumUint32GenericMethodDefinition =
                        getGenericMethodDefinition "EnumUint32"

                    let uint32 (innerType: Type) : obj =
                        enumUint32GenericMethodDefinition
                            .MakeGenericMethod([| innerType |])
                            .Invoke(null, [||])
#endif

        // Unpacks tuples encoded like this:
        //   "a"
        //   ("a", "b")
        //   (("a", "b"), "c")
        //   ((("a", "b"), "c"), "d")
        let getNestedTupleFields (tuple: obj) (length: int) =
            if length = 1 then
                [| tuple |]
            else
                let result = Array.zeroCreate length

                let mutable x = tuple
                let mutable i = length - 1

                while i > 0 do
                    result[i] <- FSharpValue.GetTupleField(x, 1)

                    i <- i - 1

                    if i = 0 then
                        result[i] <- FSharpValue.GetTupleField(x, 0)
                    else
                        x <- FSharpValue.GetTupleField(x, 0)

                result

    let rec generateDecoder
        (caseStyle: CaseStyle option)
        (existingDecoders: Map<TypeKey, obj>)
        (ty: Type)
        : obj
        =
        match Map.tryFind (TypeKey.ofType ty) existingDecoders with
        | Some x -> x
        | None ->
            match ty with
            | UnitType _ -> box Decode.unit
            | StringType _ -> box Decode.string
            | CharType _ -> box Decode.char
            | IntType _ -> box Decode.int
            | BoolType _ -> box Decode.bool
            | Int64Type _ -> box Decode.int64
            | DecimalType _ -> box Decode.decimal
            | ByteType _ -> box Decode.byte
            | SByteType _ -> box Decode.sbyte
            | UInt16Type _ -> box Decode.uint16
            | SByteType _ -> box Decode.sbyte
            | Int16Type _ -> box Decode.int16
            | UIntType _ -> box Decode.uint32
            | UInt64Type _ -> box Decode.uint64
            | SingleType _ -> box Decode.float32
            | DoubleType -> box Decode.float
            | BigIntType _ -> box Decode.bigint
            | GuidType _ -> box Decode.guid
            | TimeSpanType _ -> box Decode.timespan
            | DateTimeType _ ->
#if FABLE_COMPILER_PYTHON
                box Decode.datetimeLocal
#else
                box Decode.datetimeUtc
#endif
#if !FABLE_COMPILER_PYTHON
            | DateTimeOffsetType _ -> box Decode.datetimeOffset
#endif
            | OptionType inner ->
                Decode.Generic.option
                    inner
                    (generateDecoder caseStyle existingDecoders inner)
            | ListType inner ->
                Decode.Generic.list
                    inner
                    (generateDecoder caseStyle existingDecoders inner)
            | ArrayType inner ->
                Decode.Generic.array
                    inner
                    (generateDecoder caseStyle existingDecoders inner)
            | SeqType inner ->
                Decode.Generic.seq
                    inner
                    (generateDecoder caseStyle existingDecoders inner)
            | SetType inner ->
                Decode.Generic.set
                    inner
                    (generateDecoder caseStyle existingDecoders inner)
            | MapType(StringType, valueType) ->
                Decode.Generic.dict
                    valueType
                    (generateDecoder caseStyle existingDecoders valueType)
            | MapType(keyType, valueType) ->
                let keyDecoder =
                    generateDecoder caseStyle existingDecoders keyType

                let valueDecoder =
                    generateDecoder caseStyle existingDecoders valueType

                Decode.Generic.mapAsArray
                    keyType
                    valueType
                    keyDecoder
                    valueDecoder
            | FSharpRecordType _ ->
                genericRecordDecoder caseStyle existingDecoders ty
            | FSharpUnionType _ ->
                genericUnionDecoder caseStyle existingDecoders ty
            | FSharpTupleType _ ->
                genericTupleDecoder caseStyle existingDecoders ty
            | EnumType(ByteType _) -> Decode.Generic.Enum.byte ty
            | EnumType(SByteType _) -> Decode.Generic.Enum.sbyte ty
            | EnumType(Int16Type _) -> Decode.Generic.Enum.int16 ty
            | EnumType(UInt16Type _) -> Decode.Generic.Enum.uint16 ty
            | EnumType(IntType _) -> Decode.Generic.Enum.int ty
            | EnumType(UIntType _) -> Decode.Generic.Enum.uint32 ty
            | x -> failwith $"Unsupported type %s{x.FullName}"

    and private genericRecordDecoder
        (caseStyle: CaseStyle option)
        (existingDecoders: Map<TypeKey, obj>)
        (ty: Type)
        : obj
        =
#if FABLE_COMPILER
        let mutable self = Unchecked.defaultof<_>

        let existingDecoders =
            if Type.isRecursive ty then
                let lazySelf =
                    Decode.Generic.lazily
                        ty
                        (Lazy.makeGeneric (makeDecoderType ty) (fun _ -> self))

                existingDecoders |> Map.add (TypeKey.ofType ty) lazySelf
            else
                existingDecoders

        let recordFields =
            match ty with
            | FSharpRecordType fields -> fields
            | _ -> failwith $"Expected an F# record type"

        let fieldDecoders =
            [|
                for field in recordFields do
                    let encodedFieldName =
                        match caseStyle with
                        | Some caseStyle ->
                            Casing.convertCase
                                DotNetPascalCase
                                caseStyle
                                field.Name
                        | None -> field.Name

                    let decoder =
                        match field.PropertyType with
                        | UnitType _ ->
                            Decode.Generic.optional
                                field.PropertyType
                                encodedFieldName
                                (generateDecoder
                                    caseStyle
                                    existingDecoders
                                    field.PropertyType)
                            |> Decode.Generic.map
                                typeof<unit option>
                                typeof<unit>
                                (fun (o: obj) ->
                                    let maybeUnit = unbox o
                                    maybeUnit |> Option.defaultValue () |> box
                                )
                        | OptionType innerType ->
                            Decode.Generic.optional
                                innerType
                                encodedFieldName
                                (generateDecoder
                                    caseStyle
                                    existingDecoders
                                    innerType)
                        | _ ->
                            Decode.Generic.field
                                field.PropertyType
                                encodedFieldName
                                (generateDecoder
                                    caseStyle
                                    existingDecoders
                                    field.PropertyType)

                    field.PropertyType, decoder
            |]

        let rec mergeDecoders (state: Type * obj) (next: Type * obj) =
            let aggregateType, aggregateDecoder = state
            let fieldType, nextDecoder = next

            let nextType =
                FSharpType.MakeTupleType(
                    [|
                        aggregateType
                        fieldType
                    |]
                )

            let nextDecoder =
                Decode.Generic.zip
                    aggregateType
                    fieldType
                    aggregateDecoder
                    nextDecoder

            nextType, nextDecoder

        let tupleType, decoder = fieldDecoders |> Array.reduce mergeDecoders // There will always be at least one field

        let tupleToRecord: obj -> obj =
            fun x ->
                let values = getNestedTupleFields x (Array.length recordFields)
#if FABLE_COMPILER_PYTHON
                FSharpValue.MakeRecord(ty, values)
#else
                FSharpValue.MakeRecord(
                    ty,
                    values,
                    allowAccessToPrivateRepresentation = true
                )
#endif

        let decoder =
            Decode.Generic.map tupleType ty (box tupleToRecord) decoder

        self <- decoder

        decoder
#else
        let mutable self = Unchecked.defaultof<_>

        let existingDecoders =
            if Type.isRecursive ty then
                let lazySelf =
                    Decode.Generic.lazily
                        ty
                        (Lazy.makeGeneric
                            (makeDecoderType ty)
                            (FSharpValue.MakeFunction(
                                FSharpType.MakeFunctionType(
                                    typeof<unit>,
                                    makeDecoderType ty
                                ),
                                (fun _ -> self)
                            )))

                existingDecoders |> Map.add (TypeKey.ofType ty) lazySelf
            else
                existingDecoders

        let recordFields =
            match ty with
            | FSharpRecordType fields -> fields
            | _ -> failwith $"Expected an F# record type"

        let fieldDecoders =
            [|
                for field in recordFields do
                    let encodedFieldName =
                        match caseStyle with
                        | Some caseStyle ->
                            Casing.convertCase
                                DotNetPascalCase
                                caseStyle
                                field.Name
                        | None -> field.Name

                    let decoder =
                        match field.PropertyType with
                        | UnitType _ ->
                            Decode.Generic.optional
                                field.PropertyType
                                encodedFieldName
                                (generateDecoder
                                    caseStyle
                                    existingDecoders
                                    field.PropertyType)
                            |> Decode.Generic.map
                                typeof<unit option>
                                typeof<unit>
                                (Option.defaultValue ())
                        | OptionType innerType ->
                            Decode.Generic.optional
                                innerType
                                encodedFieldName
                                (generateDecoder
                                    caseStyle
                                    existingDecoders
                                    innerType)
                        | _ ->
                            Decode.Generic.field
                                field.PropertyType
                                encodedFieldName
                                (generateDecoder
                                    caseStyle
                                    existingDecoders
                                    field.PropertyType)

                    field.PropertyType, decoder
            |]

        let rec mergeDecoders (state: Type * obj) (next: Type * obj) =
            let aggregateType, aggregateDecoder = state
            let fieldType, nextDecoder = next

            let nextType =
                FSharpType.MakeTupleType(
                    [|
                        aggregateType
                        fieldType
                    |]
                )

            let nextDecoder =
                Decode.Generic.zip
                    aggregateType
                    fieldType
                    aggregateDecoder
                    nextDecoder

            nextType, nextDecoder

        let tupleType, decoder = fieldDecoders |> Array.reduce mergeDecoders // There will always be at least one field

        let tupleToRecordType = FSharpType.MakeFunctionType(tupleType, ty)

        let tupleToRecordImpl: obj -> obj =
            fun x ->
                let values = getNestedTupleFields x (Array.length recordFields)
#if FABLE_COMPILER_PYTHON
                FSharpValue.MakeRecord(ty, values)
#else
                FSharpValue.MakeRecord(
                    ty,
                    values,
                    allowAccessToPrivateRepresentation = true
                )
#endif

        let tupleToRecord: obj =
            FSharpValue.MakeFunction(tupleToRecordType, tupleToRecordImpl)

        let decoder = Decode.Generic.map tupleType ty tupleToRecord decoder

        self <- decoder

        decoder
#endif

    and private genericUnionDecoder
        (caseStyle: CaseStyle option)
        (existingDecoders: Map<TypeKey, obj>)
        (ty: Type)
        : obj
        =
#if FABLE_COMPILER
        let mutable self = Unchecked.defaultof<_>

        let existingDecoders =
            if Type.isRecursive ty then
                let lazySelf =
                    Decode.Generic.lazily
                        ty
                        (Lazy.makeGeneric (makeDecoderType ty) ((fun _ -> self)))

                existingDecoders |> Map.add (TypeKey.ofType ty) lazySelf
            else
                existingDecoders

        let unionCases =
            match ty with
            | FSharpUnionType cases -> cases
            | _ ->
                failwith $"Expected an F# union type but found %s{ty.FullName}"

        let alternatives =
            [
                for case in unionCases do
                    let caseFields = case.GetFields()

                    if Array.isEmpty caseFields then
#if FABLE_COMPILER_PYTHON
                        let caseObject = FSharpValue.MakeUnion(case, [||])
#else
                        let caseObject =
                            FSharpValue.MakeUnion(
                                case,
                                [||],
                                allowAccessToPrivateRepresentation = true
                            )
#endif

                        let funcImpl: obj -> obj =
                            (fun x ->
                                let x = unbox x

                                if x = case.Name then
                                    Decode.Generic.succeed ty caseObject
                                else
                                    Decode.Generic.fail
                                        ty
                                        $"Expected %s{case.Name} but found \"%s{x}\""
                            )

                        Decode.Generic.bind
                            typeof<string>
                            ty
                            funcImpl
                            Decode.string
                    else
                        let fieldDecoders =
                            [|
                                for index, field in Array.indexed caseFields do
                                    let decoder =
                                        Decode.Generic.index
                                            field.PropertyType
                                            (index + 1)
                                            (generateDecoder
                                                caseStyle
                                                existingDecoders
                                                field.PropertyType)

                                    field.PropertyType, decoder
                            |]

                        let rec mergeDecoders
                            (state: Type * obj)
                            (next: Type * obj)
                            =
                            let aggregateType, aggregateDecoder = state
                            let fieldType, nextDecoder = next

                            let nextType =
                                FSharpType.MakeTupleType(
                                    [|
                                        aggregateType
                                        fieldType
                                    |]
                                )

                            let nextDecoder =
                                Decode.Generic.zip
                                    aggregateType
                                    fieldType
                                    aggregateDecoder
                                    nextDecoder

                            nextType, nextDecoder

                        let tupleType, decoder =
                            fieldDecoders |> Array.reduce mergeDecoders // There will always be at least one field

                        let tupleToUnionCase: obj -> obj =
                            fun (x: obj) ->
                                let values =
                                    getNestedTupleFields
                                        x
                                        (Array.length caseFields)
#if FABLE_COMPILER_PYTHON
                                FSharpValue.MakeUnion(case, values)
#else
                                FSharpValue.MakeUnion(
                                    case,
                                    values,
                                    allowAccessToPrivateRepresentation = true
                                )
#endif

                        let prefix =
                            Decode.index 0 Decode.string
                            |> Decode.andThen (fun x ->
                                if x = case.Name then
                                    Decode.succeed ()
                                else
                                    Decode.fail
                                        $"Expected %s{case.Name} but found \"%s{x}\""
                            )

                        let dec =
                            Decode.Generic.map
                                tupleType
                                ty
                                (box tupleToUnionCase)
                                decoder

                        let unitToUnionCaseImpl: obj -> obj = fun _ -> dec

                        Decode.Generic.bind
                            typeof<unit>
                            ty
                            (box unitToUnionCaseImpl)
                            prefix
            ]

        let decoder = alternatives |> Seq.reduce (Decode.Generic.either ty)

        self <- decoder

        decoder
#else
        let mutable self = Unchecked.defaultof<_>

        let existingDecoders =
            if Type.isRecursive ty then
                let lazySelf =
                    Decode.Generic.lazily
                        ty
                        (Lazy.makeGeneric
                            (makeDecoderType ty)
                            (FSharpValue.MakeFunction(
                                FSharpType.MakeFunctionType(
                                    typeof<unit>,
                                    makeDecoderType ty
                                ),
                                (fun _ -> self)
                            )))

                existingDecoders |> Map.add (TypeKey.ofType ty) lazySelf
            else
                existingDecoders

        let unionCases =
            match ty with
            | FSharpUnionType cases -> cases
            | _ ->
                failwith $"Expected an F# union type but found %s{ty.FullName}"

        let alternatives =
            [
                for case in unionCases do
                    let caseFields = case.GetFields()

                    if Array.isEmpty caseFields then
                        let funcType =
                            FSharpType.MakeFunctionType(
                                typeof<string>,
                                makeDecoderType ty
                            )

                        let caseObject =
#if FABLE_COMPILER_PYTHON
                            FSharpValue.MakeUnion(case, [||])
#else
                            FSharpValue.MakeUnion(
                                case,
                                [||],
                                allowAccessToPrivateRepresentation = true
                            )
#endif

                        let funcImpl: obj -> obj =
                            (fun x ->
                                let x = unbox x

                                if x = case.Name then
                                    Decode.Generic.succeed ty caseObject
                                else
                                    Decode.Generic.fail
                                        ty
                                        $"Expected %s{case.Name} but found \"%s{x}\""
                            )

                        let func = FSharpValue.MakeFunction(funcType, funcImpl)

                        Decode.Generic.bind typeof<string> ty func Decode.string
                    else
                        let fieldDecoders =
                            [|
                                for index, field in Array.indexed caseFields do
                                    let decoder =
                                        Decode.Generic.index
                                            field.PropertyType
                                            (index + 1)
                                            (generateDecoder
                                                caseStyle
                                                existingDecoders
                                                field.PropertyType)

                                    field.PropertyType, decoder
                            |]

                        let rec mergeDecoders
                            (state: Type * obj)
                            (next: Type * obj)
                            =
                            let aggregateType, aggregateDecoder = state
                            let fieldType, nextDecoder = next

                            let nextType =
                                FSharpType.MakeTupleType(
                                    [|
                                        aggregateType
                                        fieldType
                                    |]
                                )

                            let nextDecoder =
                                Decode.Generic.zip
                                    aggregateType
                                    fieldType
                                    aggregateDecoder
                                    nextDecoder

                            nextType, nextDecoder

                        let tupleType, decoder =
                            fieldDecoders |> Array.reduce mergeDecoders // There will always be at least one field

                        let tupleToUnionCaseType =
                            FSharpType.MakeFunctionType(tupleType, ty)

                        let tupleToUnionCaseImpl: obj -> obj =
                            fun x ->
                                let values =
                                    getNestedTupleFields
                                        x
                                        (Array.length caseFields)
#if FABLE_COMPILER_PYTHON
                                FSharpValue.MakeUnion(case, values)
#else
                                FSharpValue.MakeUnion(
                                    case,
                                    values,
                                    allowAccessToPrivateRepresentation = true
                                )
#endif

                        let tupleToUnionCase: obj =
                            FSharpValue.MakeFunction(
                                tupleToUnionCaseType,
                                tupleToUnionCaseImpl
                            )

                        let prefix =
                            Decode.index 0 Decode.string
                            |> Decode.andThen (fun x ->
                                if x = case.Name then
                                    Decode.succeed ()
                                else
                                    Decode.fail
                                        $"Expected %s{case.Name} but found \"%s{x}\""
                            )

                        let dec =
                            Decode.Generic.map
                                tupleType
                                ty
                                tupleToUnionCase
                                decoder

                        let unitToUnionCaseDecoderType =
                            FSharpType.MakeFunctionType(
                                typeof<unit>,
                                makeDecoderType ty
                            )

                        let unitToUnionCaseDecoderImpl: obj -> obj =
                            fun _ -> dec

                        let unitToUnionCaseImpl =
                            FSharpValue.MakeFunction(
                                unitToUnionCaseDecoderType,
                                unitToUnionCaseDecoderImpl
                            )

                        Decode.Generic.bind
                            typeof<unit>
                            ty
                            unitToUnionCaseImpl
                            prefix
            ]

        let decoder = alternatives |> Seq.reduce (Decode.Generic.either ty)

        self <- decoder

        decoder
#endif

    and private genericTupleDecoder
        (caseStyle: CaseStyle option)
        (existingDecoders: Map<TypeKey, obj>)
        (ty: Type)
        =
#if FABLE_COMPILER
        let elements = FSharpType.GetTupleElements(ty)

        let elementDecoders =
            [|
                for index, elementType in Array.indexed elements do
                    let decoder =
                        Decode.Generic.index
                            elementType
                            index
                            (generateDecoder
                                caseStyle
                                existingDecoders
                                elementType)

                    elementType, decoder
            |]

        let rec mergeDecoders (state: Type * obj) (next: Type * obj) =
            let aggregateType, aggregateDecoder = state
            let fieldType, nextDecoder = next

            let nextType =
                FSharpType.MakeTupleType(
                    [|
                        aggregateType
                        fieldType
                    |]
                )

            let nextDecoder =
                Decode.Generic.zip
                    aggregateType
                    fieldType
                    aggregateDecoder
                    nextDecoder

            nextType, nextDecoder

        let tupleType, decoder = elementDecoders |> Array.reduce mergeDecoders // There will always be at least one element

        let nestedTuplesToTupleImpl: obj -> obj =
            fun x ->
                let values = getNestedTupleFields x (Array.length elements)
                FSharpValue.MakeTuple(values, ty)

        let nestedTuplesToTuple: obj = box nestedTuplesToTupleImpl

        Decode.Generic.map tupleType ty nestedTuplesToTuple decoder
#else
        let elements = FSharpType.GetTupleElements(ty)

        let elementDecoders =
            [|
                for index, elementType in Array.indexed elements do
                    let decoder =
                        Decode.Generic.index
                            elementType
                            index
                            (generateDecoder
                                caseStyle
                                existingDecoders
                                elementType)

                    elementType, decoder
            |]

        let rec mergeDecoders (state: Type * obj) (next: Type * obj) =
            let aggregateType, aggregateDecoder = state
            let fieldType, nextDecoder = next

            let nextType =
                FSharpType.MakeTupleType(
                    [|
                        aggregateType
                        fieldType
                    |]
                )

            let nextDecoder =
                Decode.Generic.zip
                    aggregateType
                    fieldType
                    aggregateDecoder
                    nextDecoder

            nextType, nextDecoder

        let tupleType, decoder = elementDecoders |> Array.reduce mergeDecoders // There will always be at least one element

        let nestedTuplesToTupleType = FSharpType.MakeFunctionType(tupleType, ty)

        let nestedTuplesToTupleImpl: obj -> obj =
            fun x ->
                let values = getNestedTupleFields x (Array.length elements)
                FSharpValue.MakeTuple(values, ty)

        let nestedTuplesToTuple: obj =
            FSharpValue.MakeFunction(
                nestedTuplesToTupleType,
                nestedTuplesToTupleImpl
            )

        Decode.Generic.map tupleType ty nestedTuplesToTuple decoder
#endif

    let inline autoWithOptions<'t>
        (caseStrategy: CaseStyle option)
        (extra: ExtraCoders)
        : Decoder<'t>
        =
        let ty = typeof<'t>
        let decoder = generateDecoder caseStrategy extra.DecoderOverrides ty
        unbox decoder

#if !FABLE_COMPILER
    open System.Threading
#endif

    type Auto =
#if FABLE_COMPILER
        static let instance = Cache<BoxedDecoder>()
#else
        static let instance =
            new ThreadLocal<_>(fun () -> Cache<BoxedDecoder>())
#endif

#if FABLE_COMPILER
        static member inline generateDecoder
            (?caseStrategy: CaseStyle, ?extra: ExtraCoders)
            =
#else
        static member generateDecoder
            (?caseStrategy: CaseStyle, ?extra: ExtraCoders)
            =
#endif
            let extra = defaultArg extra Extra.empty
            autoWithOptions caseStrategy extra

#if FABLE_COMPILER
        static member inline generateDecoderCached<'T>
#else
        static member generateDecoderCached<'T>
#endif
            (?caseStrategy: CaseStyle, ?extra: ExtraCoders)
            : Decoder<'T>
            =
            let extra = defaultArg extra Extra.empty

            let t = typeof<'T>

            let key =
                t.FullName
                |> (+) (Operators.string caseStrategy)
                |> (+) extra.Hash

#if FABLE_COMPILER
            let cache = instance
#else
            let cache = instance.Value
#endif

            cache.GetOrAdd(
                key,
                fun () ->
                    let dec: Decoder<'T> = autoWithOptions caseStrategy extra
                    box dec
            )
            |> unbox
