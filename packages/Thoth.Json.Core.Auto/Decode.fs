namespace Thoth.Json.Core.Auto

open System
open System.Collections.Concurrent
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

            static member OptionLossless<'t>
                (x: Decoder<'t>)
                : Decoder<'t option>
                =
                Decode.losslessOption x

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

            static member Error<'t>(typeName: string) : Decoder<'t> =
                { new Decoder<'t> with
                    member _.Decode(helpers, value) =
                        Error(
                            "",
                            BadType($"an extra coder for {typeName}", value)
                        )
                }

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

        let makeDecoderType (ty: Type) : Type =
            typedefof<Decoder<_>>.MakeGenericType([| ty |])

        [<RequireQualifiedAccess>]
        module internal Decode =

            [<RequireQualifiedAccess>]
            module Generic =

#if !FABLE_COMPILER
                let private makeCachedInvoker (name: string) =
                    let methodDef =
                        typeof<DecodeHelpers>
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
                        typeof<DecodeHelpers>
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

                let private invokeOption = makeCachedInvoker "Option"

                let private invokeOptionLossless =
                    makeCachedInvoker "OptionLossless"

                let private invokeList = makeCachedInvoker "List"
                let private invokeArray = makeCachedInvoker "Array"
                let private invokeSeq = makeCachedInvoker "Seq"
                let private invokeSet = makeCachedInvoker "Set"
                let private invokeDict = makeCachedInvoker "Dict"
                let private invokeMapAsArray = makeCachedInvoker2 "MapAsArray"
                let private invokeField = makeCachedInvoker "Field"
                let private invokeOptional = makeCachedInvoker "Optional"
                let private invokeIndex = makeCachedInvoker "Index"
                let private invokeSucceed = makeCachedInvoker "Succeed"
                let private invokeFail = makeCachedInvoker "Fail"
                let private invokeError = makeCachedInvoker "Error"
                let private invokeBind = makeCachedInvoker2 "Bind"
                let private invokeMap = makeCachedInvoker2 "Map"
                let private invokeZip = makeCachedInvoker2 "Zip"
                let private invokeEither = makeCachedInvoker "Either"
                let private invokeLazily = makeCachedInvoker "Lazily"
#endif

#if FABLE_COMPILER
                // Simple decoder wrappers - marked inline for performance
                let inline option
                    (lossless: bool)
                    (innerType: Type)
                    (decoder: obj)
                    : obj
                    =
                    if lossless then
                        Decode.losslessOption (unbox decoder) |> box
                    else
                        Decode.lossyOption (unbox decoder) |> box

                let inline list (innerType: Type) (decoder: obj) : obj =
                    Decode.list (unbox decoder) |> box

                let inline array (innerType: Type) (decoder: obj) : obj =
                    Decode.array (unbox decoder) |> box

                let inline seq (innerType: Type) (decoder: obj) : obj =
                    Decode.list (unbox decoder) |> box

                let inline set (innerType: Type) (decoder: obj) : obj =
                    Decode.list (unbox decoder) |> Decode.map Set.ofList |> box

                let inline dict (innerType: Type) (decoder: obj) : obj =
                    Decode.dict (unbox decoder) |> box

                let inline mapAsArray
                    (keyType: Type)
                    (valueType: Type)
                    (keyDecoder: obj)
                    (valueDecoder: obj)
                    : obj
                    =
                    Decode.map' (unbox keyDecoder) (unbox valueDecoder) |> box

                let inline field
                    (innerType: Type)
                    (name: string)
                    (decoder: obj)
                    : obj
                    =
                    Decode.field name (unbox decoder) |> box

                let inline optional
                    (innerType: Type)
                    (name: string)
                    (decoder: obj)
                    : obj
                    =
                    Decode.optional name (unbox decoder) |> box

                let inline index
                    (innerType: Type)
                    (i: int)
                    (decoder: obj)
                    : obj
                    =
                    Decode.index i (unbox decoder) |> box

                let inline succeed (innerType: Type) (x: obj) : obj =
                    Decode.succeed (unbox x) |> box

                let inline fail (innerType: Type) (x: string) : obj =
                    Decode.fail x |> box

                // Not inlined: creates an object, would increase code size
                let error (innerType: Type) (typeName: string) : obj =
                    { new Decoder<obj> with
                        member _.Decode(helpers, value) =
                            Error(
                                "",
                                BadType($"an extra coder for {typeName}", value)
                            )
                    }
                    |> box

                let inline bind
                    (fromType: Type)
                    (toType: Type)
                    (func: obj)
                    (decoder: obj)
                    : obj
                    =
                    Decode.andThen (unbox func) (unbox decoder) |> box

                let inline map
                    (fromType: Type)
                    (toType: Type)
                    (func: obj)
                    (decoder: obj)
                    : obj
                    =
                    Decode.map (unbox func) (unbox decoder) |> box

                let inline zip
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

                let inline either
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

                let inline lazily (innerType: Type) (x: obj) : obj =
                    Decode.lazily (unbox x) |> box
#else
                let option
                    (lossless: bool)
                    (innerType: Type)
                    (decoder: obj)
                    : obj
                    =
                    if lossless then
                        invokeOptionLossless innerType [| decoder |]
                    else
                        invokeOption innerType [| decoder |]

                let list (innerType: Type) (decoder: obj) : obj =
                    invokeList innerType [| decoder |]

                let array (innerType: Type) (decoder: obj) : obj =
                    invokeArray innerType [| decoder |]

                let seq (innerType: Type) (decoder: obj) : obj =
                    invokeSeq innerType [| decoder |]

                let set (innerType: Type) (decoder: obj) : obj =
                    invokeSet innerType [| decoder |]

                let dict (innerType: Type) (decoder: obj) : obj =
                    invokeDict innerType [| decoder |]

                let mapAsArray
                    (keyType: Type)
                    (valueType: Type)
                    (keyDecoder: obj)
                    (valueDecoder: obj)
                    : obj
                    =
                    invokeMapAsArray
                        keyType
                        valueType
                        [|
                            keyDecoder
                            valueDecoder
                        |]

                let field
                    (innerType: Type)
                    (name: string)
                    (decoder: obj)
                    : obj
                    =
                    invokeField
                        innerType
                        [|
                            name
                            decoder
                        |]

                let optional
                    (innerType: Type)
                    (name: string)
                    (decoder: obj)
                    : obj
                    =
                    invokeOptional
                        innerType
                        [|
                            name
                            decoder
                        |]

                let index (innerType: Type) (i: int) (decoder: obj) : obj =
                    invokeIndex
                        innerType
                        [|
                            i
                            decoder
                        |]

                let succeed (innerType: Type) (x: obj) : obj =
                    invokeSucceed innerType [| x |]

                let fail (innerType: Type) (x: string) : obj =
                    invokeFail innerType [| x |]

                let error (innerType: Type) (typeName: string) : obj =
                    invokeError innerType [| typeName |]

                let bind
                    (fromType: Type)
                    (toType: Type)
                    (func: obj)
                    (decoder: obj)
                    : obj
                    =
                    invokeBind
                        fromType
                        toType
                        [|
                            func
                            decoder
                        |]

                let map
                    (fromType: Type)
                    (toType: Type)
                    (func: obj)
                    (decoder: obj)
                    : obj
                    =
                    invokeMap
                        fromType
                        toType
                        [|
                            func
                            decoder
                        |]

                let zip
                    (leftType: Type)
                    (rightType: Type)
                    (leftDecoder: obj)
                    (rightDecoder: obj)
                    : obj
                    =
                    invokeZip
                        leftType
                        rightType
                        [|
                            leftDecoder
                            rightDecoder
                        |]

                let either
                    (innerType: Type)
                    (decoderA: obj)
                    (decoderB: obj)
                    : obj
                    =
                    invokeEither
                        innerType
                        [|
                            decoderA
                            decoderB
                        |]

                let lazily (innerType: Type) (x: obj) : obj =
                    invokeLazily innerType [| x |]
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

                    let byte (innerType: Type) : obj =
                        Decode.byte
                        |> Decode.andThen (checkEnumValue innerType)
                        |> box

                    let sbyte (innerType: Type) : obj =
                        Decode.sbyte
                        |> Decode.andThen (checkEnumValue innerType)
                        |> box

                    let int16 (innerType: Type) : obj =
                        Decode.int16
                        |> Decode.andThen (checkEnumValue innerType)
                        |> box

                    let uint16 (innerType: Type) : obj =
                        Decode.int16
                        |> Decode.andThen (checkEnumValue innerType)
                        |> box

                    let int (innerType: Type) : obj =
                        Decode.int
                        |> Decode.andThen (checkEnumValue innerType)
                        |> box

                    let uint32 (innerType: Type) : obj =
                        Decode.uint32
                        |> Decode.andThen (checkEnumValue innerType)
                        |> box
#else
                    let private invokeEnumByte = makeCachedInvoker "EnumByte"
                    let private invokeEnumSbyte = makeCachedInvoker "EnumSbyte"
                    let private invokeEnumInt16 = makeCachedInvoker "EnumInt16"

                    let private invokeEnumUint16 =
                        makeCachedInvoker "EnumUint16"

                    let private invokeEnumInt = makeCachedInvoker "EnumInt"

                    let private invokeEnumUint32 =
                        makeCachedInvoker "EnumUint32"

                    let byte (innerType: Type) : obj =
                        invokeEnumByte innerType [||]

                    let sbyte (innerType: Type) : obj =
                        invokeEnumSbyte innerType [||]

                    let int16 (innerType: Type) : obj =
                        invokeEnumInt16 innerType [||]

                    let uint16 (innerType: Type) : obj =
                        invokeEnumUint16 innerType [||]

                    let int (innerType: Type) : obj =
                        invokeEnumInt innerType [||]

                    let uint32 (innerType: Type) : obj =
                        invokeEnumUint32 innerType [||]
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

    // Platform helpers: create lazy self-references and typed mapping functions

#if FABLE_COMPILER
    let private makeLazySelf (ty: Type) (getSelf: unit -> obj) : obj =
        Lazy.makeGeneric (makeDecoderType ty) (fun _ -> getSelf ())

    let private makeMappingFunc
        (_fromType: Type)
        (_toType: Type)
        (funcImpl: obj -> obj)
        : obj
        =
        box funcImpl
#else
    let private makeLazySelf (ty: Type) (getSelf: unit -> obj) : obj =
        let decoderType = makeDecoderType ty

        Lazy.makeGeneric
            decoderType
            (FSharpValue.MakeFunction(
                FSharpType.MakeFunctionType(typeof<unit>, decoderType),
                fun _ -> getSelf ()
            ))

    let private makeMappingFunc
        (fromType: Type)
        (toType: Type)
        (funcImpl: obj -> obj)
        : obj
        =
        FSharpValue.MakeFunction(
            FSharpType.MakeFunctionType(fromType, toType),
            funcImpl
        )
#endif

    let private mergeDecoders (state: Type * obj) (next: Type * obj) =
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

    let rec generateDecoder
        (caseStyle: CaseStrategy option)
        (existingDecoders: Map<TypeKey, obj>)
        (isOptional: bool)
        (losslessOption: bool)
        (ty: Type)
        : obj
        =
        match Map.tryFind (TypeKey.ofType ty) existingDecoders with
        | Some x -> x
        | None ->
            let gen =
                generateDecoder caseStyle existingDecoders false losslessOption

            match ty with

            //             ///////////////////////////
            //                      IMPORTANT
            //             ///////////////////////////
            //
            // The following types are not enabled by default on purpose.
            //
            // This is done in order to minimize the impact on bundle size for Fable targets
            //
            // | Int64Type _ -> box Decode.int64
            // | DecimalType _ -> box Decode.decimal
            // | UInt64Type _ -> box Decode.uint64
            // | BigIntType _ -> box Decode.bigint
            | UnitType _ -> box Decode.unit
            | StringType _ -> box Decode.string
            | CharType _ -> box Decode.char
            | IntType _ -> box Decode.int
            | BoolType _ -> box Decode.bool
            | ByteType _ -> box Decode.byte
            | SByteType _ -> box Decode.sbyte
            | UInt16Type _ -> box Decode.uint16
            | SByteType _ -> box Decode.sbyte
            | Int16Type _ -> box Decode.int16
            | UIntType _ -> box Decode.uint32
            | SingleType _ -> box Decode.float32
            | DoubleType -> box Decode.float
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
                    losslessOption
                    inner
                    (generateDecoder
                        caseStyle
                        existingDecoders
                        true
                        losslessOption
                        inner)
            | ListType inner -> Decode.Generic.list inner (gen inner)
            | ArrayType inner -> Decode.Generic.array inner (gen inner)
            | SeqType inner -> Decode.Generic.seq inner (gen inner)
            | SetType inner -> Decode.Generic.set inner (gen inner)
            | MapType(StringType, valueType) ->
                Decode.Generic.dict valueType (gen valueType)
            | MapType(keyType, valueType) ->
                Decode.Generic.mapAsArray
                    keyType
                    valueType
                    (gen keyType)
                    (gen valueType)
            | FSharpRecordType _ ->
                genericRecordDecoder
                    caseStyle
                    existingDecoders
                    losslessOption
                    ty
            | FSharpUnionType _ ->
                genericUnionDecoder caseStyle existingDecoders losslessOption ty
            | FSharpTupleType _ ->
                genericTupleDecoder caseStyle existingDecoders losslessOption ty
            | EnumType(ByteType _) -> Decode.Generic.Enum.byte ty
            | EnumType(SByteType _) -> Decode.Generic.Enum.sbyte ty
            | EnumType(Int16Type _) -> Decode.Generic.Enum.int16 ty
            | EnumType(UInt16Type _) -> Decode.Generic.Enum.uint16 ty
            | EnumType(IntType _) -> Decode.Generic.Enum.int ty
            | EnumType(UIntType _) -> Decode.Generic.Enum.uint32 ty
            | unkown ->
                if isOptional then
                    // Return a runtime-error decoder instead of failing at generation time.
                    // This allows optional fields with unsupported types to work when the value is null,
                    // because decodeMaybeNull catches null values before the decoder runs.
                    Decode.Generic.error unkown unkown.FullName
                else
                    failwith
                        $"Cannot generate auto decoder for '{unkown.FullName}'. Please pass an extra decoder.\n\nDocumentation available at: https://thoth-org.github.io/Thoth.Json/documentation/auto/extra-coders.html#ready-to-use-extra-coders"

    and private genericRecordDecoder
        (caseStyle: CaseStrategy option)
        (existingDecoders: Map<TypeKey, obj>)
        (losslessOption: bool)
        (ty: Type)
        : obj
        =
        let mutable self = Unchecked.defaultof<_>

        let existingDecoders =
            if Type.isRecursive ty then
                let lazySelf =
                    Decode.Generic.lazily ty (makeLazySelf ty (fun () -> self))

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
                                    false
                                    losslessOption
                                    field.PropertyType)
                            |> Decode.Generic.map
                                typeof<unit option>
                                typeof<unit>
                                (makeMappingFunc
                                    typeof<unit option>
                                    typeof<unit>
                                    (fun o ->
                                        let maybeUnit: unit option = unbox o

                                        maybeUnit
                                        |> Option.defaultValue ()
                                        |> box
                                    ))
                        | OptionType innerType ->
                            if losslessOption then
                                // For lossless option encoding, the field value is already in the option format
                                // So we need to use a decoder for the option type itself
                                // We use Decode.Generic.either to handle both the lossless format and null/missing
                                Decode.Generic.either
                                    field.PropertyType
                                    (Decode.Generic.field
                                        field.PropertyType
                                        encodedFieldName
                                        (generateDecoder
                                            caseStyle
                                            existingDecoders
                                            false
                                            losslessOption
                                            field.PropertyType))
                                    (Decode.Generic.succeed
                                        field.PropertyType
                                        None)
                            else
                                Decode.Generic.optional
                                    innerType
                                    encodedFieldName
                                    (generateDecoder
                                        caseStyle
                                        existingDecoders
                                        true
                                        losslessOption
                                        innerType)
                        | _ ->
                            Decode.Generic.field
                                field.PropertyType
                                encodedFieldName
                                (generateDecoder
                                    caseStyle
                                    existingDecoders
                                    false
                                    losslessOption
                                    field.PropertyType)

                    field.PropertyType, decoder
            |]

        let tupleType, decoder = fieldDecoders |> Array.reduce mergeDecoders

        let tupleToRecord =
            makeMappingFunc
                tupleType
                ty
                (fun x ->
                    let values =
                        getNestedTupleFields x (Array.length recordFields)
#if FABLE_COMPILER_PYTHON
                    FSharpValue.MakeRecord(ty, values)
#else
                    FSharpValue.MakeRecord(
                        ty,
                        values,
                        allowAccessToPrivateRepresentation = true
                    )
#endif
                )

        let decoder = Decode.Generic.map tupleType ty tupleToRecord decoder

        self <- decoder

        decoder

    and private genericUnionDecoder
        (caseStyle: CaseStrategy option)
        (existingDecoders: Map<TypeKey, obj>)
        (losslessOption: bool)
        (ty: Type)
        : obj
        =
        let mutable self = Unchecked.defaultof<_>

        let existingDecoders =
            if Type.isRecursive ty then
                let lazySelf =
                    Decode.Generic.lazily ty (makeLazySelf ty (fun () -> self))

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
                            fun x ->
                                let x: string = unbox x

                                if x = case.Name then
                                    Decode.Generic.succeed ty caseObject
                                else
                                    Decode.Generic.fail
                                        ty
                                        $"Expected %s{case.Name} but found \"%s{x}\""

                        Decode.Generic.bind
                            typeof<string>
                            ty
                            (makeMappingFunc
                                typeof<string>
                                (makeDecoderType ty)
                                funcImpl)
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
                                                false
                                                losslessOption
                                                field.PropertyType)

                                    field.PropertyType, decoder
                            |]

                        let tupleType, decoder =
                            fieldDecoders |> Array.reduce mergeDecoders

                        let tupleToUnionCase =
                            makeMappingFunc
                                tupleType
                                ty
                                (fun x ->
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
                                        allowAccessToPrivateRepresentation =
                                            true
                                    )
#endif
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

                        let unitToUnionCaseDecoder: obj -> obj = fun _ -> dec

                        Decode.Generic.bind
                            typeof<unit>
                            ty
                            (makeMappingFunc
                                typeof<unit>
                                (makeDecoderType ty)
                                unitToUnionCaseDecoder)
                            prefix
            ]

        let decoder = alternatives |> Seq.reduce (Decode.Generic.either ty)

        self <- decoder

        decoder

    and private genericTupleDecoder
        (caseStyle: CaseStrategy option)
        (existingDecoders: Map<TypeKey, obj>)
        (losslessOption: bool)
        (ty: Type)
        =
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
                                false
                                losslessOption
                                elementType)

                    elementType, decoder
            |]

        let tupleType, decoder = elementDecoders |> Array.reduce mergeDecoders

        let nestedTuplesToTuple =
            makeMappingFunc
                tupleType
                ty
                (fun x ->
                    let values = getNestedTupleFields x (Array.length elements)
                    FSharpValue.MakeTuple(values, ty)
                )

        Decode.Generic.map tupleType ty nestedTuplesToTuple decoder

    let inline autoWithOptions<'t>
        (caseStrategy: CaseStrategy option)
        (extra: ExtraCoders)
        (losslessOption: bool)
        : Decoder<'t>
        =
        let ty = typeof<'t>

        let decoder =
            generateDecoder
                caseStrategy
                extra.DecoderOverrides
                false
                losslessOption
                ty

        unbox decoder

#if !FABLE_COMPILER
    open System.Threading
#endif

    type private AutoCache =
        Cache<
            struct (string * CaseStrategy option * bool * string),
            BoxedDecoder
         >

    type Auto =
#if FABLE_COMPILER
        static let instance = AutoCache()
#else
        static let instance = new ThreadLocal<_>(fun () -> AutoCache())
#endif

#if FABLE_COMPILER
        static member inline generateDecoder
            (
                ?caseStrategy: CaseStrategy,
                ?extra: ExtraCoders,
                ?losslessOption: bool
            )
            =
#else
        static member generateDecoder
            (
                ?caseStrategy: CaseStrategy,
                ?extra: ExtraCoders,
                ?losslessOption: bool
            )
            =
#endif
            let extra = defaultArg extra Extra.empty
            let losslessOption = defaultArg losslessOption false
            autoWithOptions caseStrategy extra losslessOption

#if FABLE_COMPILER
        static member inline generateDecoderCached<'T>
#else
        static member generateDecoderCached<'T>
#endif
            (
                ?caseStrategy: CaseStrategy,
                ?extra: ExtraCoders,
                ?losslessOption: bool
            )
            : Decoder<'T>
            =
            let extra = defaultArg extra Extra.empty
            let losslessOption = defaultArg losslessOption false

            let t = typeof<'T>

            let key =
                struct (t.FullName, caseStrategy, losslessOption, extra.Hash)

#if FABLE_COMPILER
            let cache = instance
#else
            let cache = instance.Value
#endif

            cache.GetOrAdd(
                key,
                fun () ->
                    let dec: Decoder<'T> =
                        autoWithOptions caseStrategy extra losslessOption

                    box dec
            )
            |> unbox
