namespace Thoth.Json.Auto

open System
open System.Collections.Generic
open FSharp.Reflection

[<AutoOpen>]
module internal Prelude =

    type FSharpValue with

        static member MakeList(elementType: Type, elements: seq<obj>) : obj =
            let objListType = typedefof<obj list>
            let listType = objListType.MakeGenericType(elementType)
            let ucis = FSharpType.GetUnionCases(listType)
            let emptyUci = ucis |> Seq.find (fun uci -> uci.Name = "Empty")
            let consUci = ucis |> Seq.find (fun uci -> uci.Name = "Cons")
            let empty = FSharpValue.MakeUnion(emptyUci, [||])

            elements
            |> Seq.rev
            |> Seq.fold
                (fun acc x ->
                    FSharpValue.MakeUnion(
                        consUci,
                        [|
                            x
                            acc
                        |]
                    )
                )
                empty

    [<RequireQualifiedAccess>]
    module Map =

        let merge (a: Map<'k, 'v>) (b: Map<'k, 'v>) =
            if a = b then
                a
            else
                seq {
                    for KeyValue(k, v) in a do
                        yield k, v

                    for KeyValue(k, v) in b do
                        yield k, v
                }
                |> Map.ofSeq

    [<RequireQualifiedAccess>]
    module Lazy =

#if FABLE_COMPILER
        let makeGeneric (ty: Type) (func: obj) : obj =
            let factory: unit -> obj = unbox func
            Lazy<obj>(factory) |> box
#else
        open System.Reflection

        type private LazyHelper =
            static member Create<'t>(factory: unit -> 't) : Lazy<'t> =
                Lazy<'t>(factory, true)

        let lazyCreateMethodDefinition =
            typeof<LazyHelper>
                .GetMethods(BindingFlags.Static ||| BindingFlags.NonPublic)
            |> Seq.filter (fun x -> x.Name = "Create")
            |> Seq.exactlyOne
            |> fun mi -> mi.GetGenericMethodDefinition()

        /// Creates a lazy value of the given type.
        /// ty is the type of 't
        /// func is a boxed value of unit -> 't
        let makeGeneric (ty: Type) (func: obj) =
            lazyCreateMethodDefinition
                .MakeGenericMethod([| ty |])
                .Invoke(null, [| func |])
#endif

    [<AutoOpen>]
    module internal ActivePatterns =

        let (|UnitType|_|) (ty: Type) =
            if ty.FullName = typeof<unit>.FullName then
                Some()
            else
                None

        let (|StringType|_|) (ty: Type) =
            if ty.FullName = typeof<string>.FullName then
                Some()
            else
                None

        let (|CharType|_|) (ty: Type) =
            if ty.FullName = typeof<char>.FullName then
                Some()
            else
                None

        let (|IntType|_|) (ty: Type) =
            if ty.FullName = typeof<int>.FullName then
                Some()
            else
                None

        let (|BoolType|_|) (ty: Type) =
            if ty.FullName = typeof<bool>.FullName then
                Some()
            else
                None

        let (|Int64Type|_|) (ty: Type) =
            if ty.FullName = typeof<int64>.FullName then
                Some()
            else
                None

        let (|SingleType|_|) (ty: Type) =
            if ty.FullName = typeof<float32>.FullName then
                Some()
            else
                None

        let (|DoubleType|_|) (ty: Type) =
            if ty.FullName = typeof<float>.FullName then
                Some()
            else
                None

        let (|DecimalType|_|) (ty: Type) =
            if ty.FullName = typeof<decimal>.FullName then
                Some()
            else
                None

        let (|ByteType|_|) (ty: Type) =
            if ty.FullName = typeof<byte>.FullName then
                Some()
            else
                None

        let (|SByteType|_|) (ty: Type) =
            if ty.FullName = typeof<sbyte>.FullName then
                Some()
            else
                None

        let (|UInt16Type|_|) (ty: Type) =
            if ty.FullName = typeof<uint16>.FullName then
                Some()
            else
                None

        let (|Int16Type|_|) (ty: Type) =
            if ty.FullName = typeof<int16>.FullName then
                Some()
            else
                None

        let (|UIntType|_|) (ty: Type) =
            if ty.FullName = typeof<uint>.FullName then
                Some()
            else
                None

        let (|UInt64Type|_|) (ty: Type) =
            if ty.FullName = typeof<uint64>.FullName then
                Some()
            else
                None

        let (|BigIntType|_|) (ty: Type) =
            if ty.FullName = typeof<bigint>.FullName then
                Some()
            else
                None

        let (|GuidType|_|) (ty: Type) =
            if ty.FullName = typeof<Guid>.FullName then
                Some()
            else
                None

        let (|TimeSpanType|_|) (ty: Type) =
            if ty.FullName = typeof<TimeSpan>.FullName then
                Some()
            else
                None

        let (|DateTimeType|_|) (ty: Type) =
            if ty.FullName = typeof<DateTime>.FullName then
                Some()
            else
                None

        let (|DateTimeOffsetType|_|) (ty: Type) =
            if ty.FullName = typeof<DateTimeOffset>.FullName then
                Some()
            else
                None

        let (|OptionType|_|) (ty: Type) =
            if
                ty.IsGenericType
                && ty.GetGenericTypeDefinition() = typedefof<obj option>
            then
                Some(ty.GetGenericArguments()[0])
            else
                None

        let (|ListType|_|) (ty: Type) =
            if
                ty.IsGenericType
                && ty.GetGenericTypeDefinition() = typedefof<obj list>
            then
                Some(ty.GetGenericArguments()[0])
            else
                None

        let (|MapType|_|) (ty: Type) =
            if
                ty.IsGenericType
                && ty.GetGenericTypeDefinition() = typedefof<Map<int, obj>>
            then
                Some(ty.GetGenericArguments()[0], ty.GetGenericArguments()[1])
            else
                None

        let (|ArrayType|_|) (ty: Type) =
#if FABLE_COMPILER
            if ty.IsArray then
                Some(ty.GetElementType())
            else
                None
#else
            if ty.IsArray && ty.GetArrayRank() = 1 then
                Some(ty.GetElementType())
            else
                None
#endif

        let (|SeqType|_|) (ty: Type) =
            if
                ty.IsGenericType
                && ty.GetGenericTypeDefinition() = typedefof<obj seq>
            then
                Some(ty.GetGenericArguments()[0])
            else
                None

        let (|SetType|_|) (ty: Type) =
            if
                ty.IsGenericType
                && ty.GetGenericTypeDefinition() = typedefof<Set<int>>
            then
                Some(ty.GetGenericArguments()[0])
            else
                None

        let (|FSharpRecordType|_|) (ty: Type) =
#if FABLE_COMPILER_PYTHON
            if FSharpType.IsRecord(ty) then
                Some(FSharpType.GetRecordFields(ty))
            else
                None
#else
            if FSharpType.IsRecord(ty, true) then
                Some(
                    FSharpType.GetRecordFields(
                        ty,
                        allowAccessToPrivateRepresentation = true
                    )
                )
            else
                None
#endif

        let (|FSharpUnionType|_|) (ty: Type) =
#if FABLE_COMPILER_PYTHON
            if FSharpType.IsUnion(ty) then
                Some(FSharpType.GetUnionCases(ty))
            else
                None
#else
            if FSharpType.IsUnion(ty, true) then
                Some(
                    FSharpType.GetUnionCases(
                        ty,
                        allowAccessToPrivateRepresentation = true
                    )
                )
            else
                None
#endif

        let (|FSharpTupleType|_|) (ty: Type) =
            if FSharpType.IsTuple(ty) then
                Some(FSharpType.GetTupleElements(ty))
            else
                None

        let (|EnumType|_|) (ty: Type) =
            if ty.IsEnum then
                Some(ty.GetEnumUnderlyingType())
            else
                None

#if !FABLE_COMPILER
        let (|StringEnum|_|) (ty: Type) =
            ty.CustomAttributes
            |> Seq.tryPick (
                function
                | attr when
                    attr.AttributeType.FullName = "Fable.Core.StringEnumAttribute"
                    ->
                    Some attr
                | _ -> None
            )
#endif

#if !FABLE_COMPILER
        let (|CompiledName|_|) (caseInfo: UnionCaseInfo) =
            caseInfo.GetCustomAttributes()
            |> Seq.tryPick (
                function
                | :? CompiledNameAttribute as att -> Some att.CompiledName
                | _ -> None
            )
#endif

#if !FABLE_COMPILER
        let (|LowerFirst|Forward|)
            (args: IList<Reflection.CustomAttributeTypedArgument>)
            =
            args
            |> Seq.tryPick (
                function
                | rule when
                    rule.ArgumentType.FullName = typeof<Fable.Core.CaseRules>
                        .FullName
                    ->
                    Some rule
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
                | None -> LowerFirst
#endif

    [<RequireQualifiedAccess>]
    module Type =

        let isRecursive (ty: Type) =
            let rec loop (seen: Set<string>) (current: Type) =
                match current with
                | FSharpTupleType elementTypes ->
                    if Array.contains ty elementTypes then
                        true
                    else
                        let seenNext =
                            Set.union
                                seen
                                (elementTypes
                                 |> Array.map (fun ty -> ty.FullName)
                                 |> Set.ofArray)

                        elementTypes
                        |> Seq.filter (fun ty ->
                            not (Set.contains ty.FullName seen)
                        )
                        |> Seq.exists (fun ty -> loop seenNext ty)
                | current when
                    current.IsGenericType
                    && current.GetGenericTypeDefinition() = typedefof<obj list>
                    ->
                    let elementType =
                        current.GetGenericArguments() |> Array.head

                    if elementType = ty then
                        true
                    else
                        let seenNext = Set.add elementType.FullName seen

                        loop seenNext elementType
                | FSharpRecordType fields ->
                    let fieldTypes =
                        fields |> Array.map (fun pi -> pi.PropertyType)

                    if Array.contains ty fieldTypes then
                        true
                    else
                        let seenNext =
                            Set.union
                                seen
                                (fieldTypes
                                 |> Array.map (fun ty -> ty.FullName)
                                 |> Set.ofArray)

                        fieldTypes
                        |> Seq.filter (fun ty ->
                            not (Set.contains ty.FullName seen)
                        )
                        |> Seq.exists (fun ty -> loop seenNext ty)
                | FSharpUnionType fields ->
                    let fieldTypes =
                        fields
                        |> Array.collect (fun uci -> uci.GetFields())
                        |> Array.map (fun pi -> pi.PropertyType)

                    if Array.contains ty fieldTypes then
                        true
                    else
                        let seenNext =
                            Set.union
                                seen
                                (fieldTypes
                                 |> Array.map (fun ty -> ty.FullName)
                                 |> Set.ofArray)

                        fieldTypes
                        |> Seq.filter (fun ty ->
                            not (Set.contains ty.FullName seen)
                        )
                        |> Seq.exists (fun ty -> loop seenNext ty)
                | _ -> false

            loop Set.empty ty
