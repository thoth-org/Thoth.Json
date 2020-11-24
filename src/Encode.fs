namespace Thoth.Json

open System.Text
open Thoth.Json.Parser

[<RequireQualifiedAccess>]
module Encode =

    open System.Collections.Generic
    open System.Globalization

    open Fable.Core

    #if FABLE_COMPILER
    open Fable.Core
    open Fable.Core.JsInterop
    #endif

    #if !FABLE_COMPILER
    open System.IO
    #endif

    ///**Description**
    /// Encode a string
    ///
    ///**Parameters**
    ///  * `value` - parameter of type `string`
    ///
    ///**Output Type**
    ///  * `Value`
    ///
    ///**Exceptions**
    ///
    let string (value : string) : JsonValue =
        // If the string is null we represent it using null
        // This is similar to what the other runtime do, THOTH_JSON parser is more strict by default
        // so we need mix a bit of the Parser logic with the decoder
        if value = null then
            Json.Null
        else
            value |> Json.String

    let char (value : char) : JsonValue =
        (Operators.string value) |> Json.String

    ///**Description**
    /// Encode a GUID
    ///
    ///**Parameters**
    ///  * `value` - parameter of type `System.Guid`
    ///
    ///**Output Type**
    ///  * `Value`
    ///
    ///**Exceptions**
    ///
    let guid (value : System.Guid) : JsonValue =
        value.ToString() |> Json.String

    ///**Description**
    /// Encode a Float. `Infinity` and `NaN` are encoded as `null`.
    ///
    ///**Parameters**
    ///  * `value` - parameter of type `float`
    ///
    ///**Output Type**
    ///  * `Value`
    ///
    ///**Exceptions**
    ///
    let inline float (value : float) : JsonValue =
        value |> Json.Number

    let float32 (value : float32) : JsonValue =
        // I think we are loosing some precision here
        Operators.float value |> Json.Number

    ///**Description**
    /// Encode a Decimal.
    ///
    ///**Parameters**
    ///  * `value` - parameter of type `decimal`
    ///
    ///**Output Type**
    ///  * `Value`
    ///
    ///**Exceptions**
    ///
    let decimal (value : decimal) : JsonValue =
        value.ToString(CultureInfo.InvariantCulture) |> Json.String

    ///**Description**
    /// Encode null
    ///
    ///**Parameters**
    ///
    ///**Output Type**
    ///  * `Value`
    ///
    ///**Exceptions**
    ///
    let nil : JsonValue =
        Json.Null

    ///**Description**
    /// Encode a bool
    ///**Parameters**
    ///  * `value` - parameter of type `bool`
    ///
    ///**Output Type**
    ///  * `Value`
    ///
    ///**Exceptions**
    ///
    let bool (value : bool) : JsonValue =
        Json.Bool value

    ///**Description**
    /// Encode an object
    ///
    ///**Parameters**
    ///  * `values` - parameter of type `(string * Value) list`
    ///
    ///**Output Type**
    ///  * `Value`
    ///
    ///**Exceptions**
    ///
    let object (values : (string * JsonValue) seq) : JsonValue =
        values
        |> Map.ofSeq
        |> Json.Object

    ///**Description**
    /// Encode an array
    ///
    ///**Parameters**
    ///  * `values` - parameter of type `Value array`
    ///
    ///**Output Type**
    ///  * `Value`
    ///
    ///**Exceptions**
    ///
    let array (values : JsonValue array) : JsonValue =
        Json.Array values

    ///**Description**
    /// Encode a list
    ///**Parameters**
    ///  * `values` - parameter of type `Value list`
    ///
    ///**Output Type**
    ///  * `Value`
    ///
    ///**Exceptions**
    ///
    let list (values : JsonValue list) : JsonValue =
        values |> List.toArray |> Json.Array

    let seq (values : JsonValue seq) : JsonValue =
        values |> Seq.toArray |> Json.Array

    ///**Description**
    /// Encode a dictionary
    ///**Parameters**
    ///  * `values` - parameter of type `Map<string, Value>`
    ///
    ///**Output Type**
    ///  * `Value`
    ///
    ///**Exceptions**
    ///
    let dict (values : Map<string, JsonValue>) : JsonValue =
        values
        |> Map.toList
        |> object

    let bigint (value : bigint) : JsonValue =
        value.ToString(CultureInfo.InvariantCulture) |> Json.String

    /// **Description**
    ///
    /// The DateTime is always encoded using UTC representation
    ///
    /// **Parameters**
    ///   * `value` - parameter of type `System.DateTime`
    ///
    /// **Output Type**
    ///   * `Value`
    ///
    /// **Exceptions**
    ///
    let datetime (value : System.DateTime) : JsonValue =
        value.ToString("O", CultureInfo.InvariantCulture) |> string

    let datetimeOffset (value : System.DateTimeOffset) : JsonValue =
        value.ToString("O", CultureInfo.InvariantCulture) |> string

    /// **Description**
    /// Encode a timespan
    /// **Parameters**
    ///   * `value` - parameter of type `System.TimeSpan`
    ///
    /// **Output Type**
    ///   * `Value`
    ///
    /// **Exceptions**
    ///
    let timespan (value : System.TimeSpan) : JsonValue =
        value.ToString() |> string

    let sbyte (value : sbyte) : JsonValue =
        value.ToString(CultureInfo.InvariantCulture) |> string

    let byte (value : byte) : JsonValue =
        value.ToString(CultureInfo.InvariantCulture) |> string

    let int16 (value : int16) : JsonValue =
        value.ToString(CultureInfo.InvariantCulture) |> string

    let uint16 (value : uint16) : JsonValue =
        value.ToString(CultureInfo.InvariantCulture) |> string

    let int (value : int) : JsonValue =
        Operators.float value |> Json.Number

    let uint32 (value : uint32) : JsonValue =
        Operators.float value |> Json.Number

    let int64 (value : int64) : JsonValue =
        value.ToString(CultureInfo.InvariantCulture) |> Json.String

    let uint64 (value : uint64) : JsonValue =
        value.ToString(CultureInfo.InvariantCulture) |> Json.String

    let unit () : JsonValue =
        Json.Null

    let tuple2
            (enc1 : Encoder<'T1>)
            (enc2 : Encoder<'T2>)
            (v1, v2) : JsonValue =
        [| enc1 v1
           enc2 v2 |] |> array

    let tuple3
            (enc1 : Encoder<'T1>)
            (enc2 : Encoder<'T2>)
            (enc3 : Encoder<'T3>)
            (v1, v2, v3) : JsonValue =
        [| enc1 v1
           enc2 v2
           enc3 v3 |] |> array

    let tuple4
            (enc1 : Encoder<'T1>)
            (enc2 : Encoder<'T2>)
            (enc3 : Encoder<'T3>)
            (enc4 : Encoder<'T4>)
            (v1, v2, v3, v4) : JsonValue =
        [| enc1 v1
           enc2 v2
           enc3 v3
           enc4 v4 |] |> array

    let tuple5
            (enc1 : Encoder<'T1>)
            (enc2 : Encoder<'T2>)
            (enc3 : Encoder<'T3>)
            (enc4 : Encoder<'T4>)
            (enc5 : Encoder<'T5>)
            (v1, v2, v3, v4, v5) : JsonValue =
        [| enc1 v1
           enc2 v2
           enc3 v3
           enc4 v4
           enc5 v5 |] |> array

    let tuple6
            (enc1 : Encoder<'T1>)
            (enc2 : Encoder<'T2>)
            (enc3 : Encoder<'T3>)
            (enc4 : Encoder<'T4>)
            (enc5 : Encoder<'T5>)
            (enc6 : Encoder<'T6>)
            (v1, v2, v3, v4, v5, v6) : JsonValue =
        [| enc1 v1
           enc2 v2
           enc3 v3
           enc4 v4
           enc5 v5
           enc6 v6 |] |> array

    let tuple7
            (enc1 : Encoder<'T1>)
            (enc2 : Encoder<'T2>)
            (enc3 : Encoder<'T3>)
            (enc4 : Encoder<'T4>)
            (enc5 : Encoder<'T5>)
            (enc6 : Encoder<'T6>)
            (enc7 : Encoder<'T7>)
            (v1, v2, v3, v4, v5, v6, v7) : JsonValue =
        [| enc1 v1
           enc2 v2
           enc3 v3
           enc4 v4
           enc5 v5
           enc6 v6
           enc7 v7 |] |> array

    let tuple8
            (enc1 : Encoder<'T1>)
            (enc2 : Encoder<'T2>)
            (enc3 : Encoder<'T3>)
            (enc4 : Encoder<'T4>)
            (enc5 : Encoder<'T5>)
            (enc6 : Encoder<'T6>)
            (enc7 : Encoder<'T7>)
            (enc8 : Encoder<'T8>)
            (v1, v2, v3, v4, v5, v6, v7, v8) : JsonValue =
        [| enc1 v1
           enc2 v2
           enc3 v3
           enc4 v4
           enc5 v5
           enc6 v6
           enc7 v7
           enc8 v8 |] |> array

    ////////////
    // Enum ///
    /////////

    module Enum =

        let byte<'TEnum when 'TEnum : enum<byte>> (value : 'TEnum) : JsonValue =
            LanguagePrimitives.EnumToValue value
            |> byte

        let sbyte<'TEnum when 'TEnum : enum<sbyte>> (value : 'TEnum) : JsonValue =
            LanguagePrimitives.EnumToValue value
            |> sbyte

        let int16<'TEnum when 'TEnum : enum<int16>> (value : 'TEnum) : JsonValue =
            LanguagePrimitives.EnumToValue value
            |> int16

        let uint16<'TEnum when 'TEnum : enum<uint16>> (value : 'TEnum) : JsonValue =
            LanguagePrimitives.EnumToValue value
            |> uint16

        let int<'TEnum when 'TEnum : enum<int>> (value : 'TEnum) : JsonValue =
            LanguagePrimitives.EnumToValue value
            |> int

        let uint32<'TEnum when 'TEnum : enum<uint32>> (value : 'TEnum) : JsonValue =
            LanguagePrimitives.EnumToValue value
            |> uint32

    ///**Description**
    /// Convert a `Value` into a prettified string.
    ///**Parameters**
    ///  * `space` - parameter of type `int` - Amount of indentation
    ///  * `value` - parameter of type `obj` - Value to convert
    ///
    ///**Output Type**
    ///  * `string`
    ///
    ///**Exceptions**
    ///
    let toString (space: int) (value: JsonValue) : string =
        let builder = new StringBuilder()
        let format =
            if space = 0 then
                JsonSaveOptions.DisableFormatting
            else
                JsonSaveOptions.Format

        value.Write(builder, format, space)

        builder.ToString()

    ///**Description**
    /// Encode an option
    ///**Parameters**
    ///  * `encoder` - parameter of type `'a -> Value`
    ///
    ///**Output Type**
    ///  * `'a option -> Value`
    ///
    ///**Exceptions**
    ///
    let option (encoder : 'a -> JsonValue) (value : 'a option) =
        // Replicate the behaviour of Encode.Auto when dealing with DUs
        match value with
        | Some value ->
            tuple2
                string
                encoder
                ("Some", value)

        | None ->
            string "None"

    //////////////////
    // Reflection ///
    ////////////////

    open FSharp.Reflection

    type private EncoderCrate<'T>(enc: Encoder<'T>) =
        inherit BoxedEncoder()
        override __.Encode(value: obj): JsonValue =
            enc (unbox value)
        member __.UnboxedEncoder = enc

    let boxEncoder (d: Encoder<'T>): BoxedEncoder =
        EncoderCrate(d) :> BoxedEncoder

    let unboxEncoder<'T> (d: BoxedEncoder): Encoder<'T> =
        (d :?> EncoderCrate<'T>).UnboxedEncoder

    let private (|StringifiableType|_|) (t: System.Type): (obj->string) option =
        let fullName = t.FullName
        if fullName = typeof<string>.FullName then
            Some unbox
        elif fullName = typeof<System.Guid>.FullName then
            Some(fun (v: obj) -> (v :?> System.Guid).ToString())
        else None

    let rec inline private handleRecord (extra : Map<string, ref<BoxedEncoder>>)
                                (caseStrategy : CaseStrategy)
                                (skipNullField : bool)
                                (t : System.Type) : BoxedEncoder =
        let setters =
            FSharpType.GetRecordFields(t, allowAccessToPrivateRepresentation = true)
            |> Array.map (fun propertyInfo ->
                let targetKey = Util.Casing.convert caseStrategy propertyInfo.Name
                let encoder : BoxedEncoder = autoEncoder extra caseStrategy skipNullField propertyInfo.PropertyType

                fun (source : obj) (res : Map<string, Json>) ->
                    let value = FSharpValue.GetRecordField(source, propertyInfo)
                    // Discard null value
                    if not skipNullField || (skipNullField && not (isNull value)) then
                        Map.add targetKey (encoder.Encode value) res
                    else
                        res
            )

        boxEncoder(fun (value : obj) ->
            (Map.empty<string, Json>, setters)
            ||> Array.fold (fun res set ->
                set value res
            )
            |> Json.Object
        )

    and inline private handleUnion (extra : Map<string, ref<BoxedEncoder>>)
                            (caseStrategy : CaseStrategy)
                            (skipNullField : bool)
                            (t : System.Type) : BoxedEncoder =
        boxEncoder(fun (value : obj) ->
            let unionCasesInfo = FSharpType.GetUnionCases(t, allowAccessToPrivateRepresentation = true)
            let info, fields = FSharpValue.GetUnionFields(value, t, allowAccessToPrivateRepresentation = true)

            match unionCasesInfo.Length with
            | 1 ->
                match fields.Length with
                // If there is only one unionCaseInfo and no argument to it output the name of the Case
                // Type: type SingleNoArguments = SingleNoArguments
                // Value: let value = SingleNoArguments
                // JSON: "SingleNoArguments"
                | 0 ->
                    string info.Name

                // If there is only one unionCaseInfo and only one argument output the argument JSON representation directly
                // Type: type Email = Email of string
                // Value: let value = Email "mail@test.com"
                // JSON: "mail@test.com"
                | 1 ->
                    let fieldTypes = info.GetFields()
                    let encoder : BoxedEncoder = autoEncoder extra caseStrategy skipNullField fieldTypes.[0].PropertyType
                    encoder.Encode(fields.[0])

                // If there is only one unionCaseInfo and several arguments output the arguments represented as a tuple
                // Type: type User = User of string * {| Role : string; Level : int |}
                // Value: let value = User ("maxime", {| Role = "Admin"; Level = 0 |}
                // JSON: [ "maxime", { Role = "Admin", Level = 0 } ]
                | length ->
                    let fieldTypes = info.GetFields()
                    let res = Array.zeroCreate(length)
                    for i = 0 to length - 1 do
                        let encoder : BoxedEncoder = autoEncoder extra caseStrategy skipNullField fieldTypes.[i].PropertyType
                        res.[i] <- encoder.Encode(fields.[i])
                    array res

            | _ ->
                match fields.Length with
                | 0 ->
                    #if !NETFRAMEWORK && !FABLE_COMPILER
                    match t with
                    // Replicate Fable behaviour when using StringEnum
                    | Util.StringEnum t ->
                        match info with
                        | Util.CompiledName name -> string name
                        | _ ->
                            match t.ConstructorArguments with
                            | Util.LowerFirst ->
                                let name = info.Name.[..0].ToLowerInvariant() + info.Name.[1..]
                                string name
                            | Util.Forward -> string info.Name

                    | _ -> string info.Name
                    #else
                    string info.Name
                    #endif

                | length ->
                    let fieldTypes = info.GetFields()
                    let res = Array.zeroCreate(length + 1)
                    res.[0] <- string info.Name
                    for i = 1 to length do
                        let encoder : BoxedEncoder = autoEncoder extra caseStrategy skipNullField fieldTypes.[i-1].PropertyType
                        res.[i] <- encoder.Encode(fields.[i-1])
                    array res
        )

    and inline private handleRecordAndUnion (extra : Map<string, ref<BoxedEncoder>>)
                            (caseStrategy : CaseStrategy)
                            (skipNullField : bool)
                            (t : System.Type) : BoxedEncoder =
        // Add the encoder to extra in case one of the fields is recursive
        let encoderRef = ref Unchecked.defaultof<_>
        let extra = extra |> Map.add t.FullName encoderRef

        let encoder =
            if FSharpType.IsRecord(t, allowAccessToPrivateRepresentation = true) then
                handleRecord extra caseStrategy skipNullField t
            else if FSharpType.IsUnion(t, allowAccessToPrivateRepresentation = true) then
                handleUnion extra caseStrategy skipNullField t
            else
                failwithf "Cannot generate auto encoder for %s. Please pass an extra coder." t.FullName

        encoderRef := encoder
        encoder

    and inline private handleGenericSeq (encoder : BoxedEncoder) =
        boxEncoder(fun (elements : obj) ->
            let array = ResizeArray<Json>()

            for element in elements :?> System.Collections.IEnumerable do
                array.Add(encoder.Encode element)

            array.ToArray() |> Json.Array
        )

    and inline private handleEnum (t : System.Type) (fullName : string) =
        let enumType = System.Enum.GetUnderlyingType(t).FullName
        if enumType = typeof<sbyte>.FullName then
            boxEncoder sbyte
        else if enumType = typeof<byte>.FullName then
            boxEncoder byte
        else if enumType = typeof<int16>.FullName then
            boxEncoder int16
        else if enumType = typeof<uint16>.FullName then
            boxEncoder uint16
        else if enumType = typeof<int>.FullName then
            boxEncoder int
        else if enumType = typeof<uint32>.FullName then
            boxEncoder uint32
        else
            failwithf
                """Cannot generate auto encoder for %s.
Only the following enum type are supported:
- sbyte
- byte
- int16
- uint16
- int
- uint32
If you can't use one of these types, please pass add a new extra coder.
                """
                fullName

    and inline private handleTuple (extra : Map<string, ref<BoxedEncoder>>)
                (caseStrategy : CaseStrategy)
                (skipNullField : bool)
                (t : System.Type) : BoxedEncoder =

        let encoders : BoxedEncoder array =
            FSharpType.GetTupleElements(t)
            |> Array.map (fun typ ->
                autoEncoder extra caseStrategy skipNullField typ
            )

        boxEncoder(fun (value : obj) ->
            FSharpValue.GetTupleFields(value)
            |> Array.mapi (fun i fieldValue ->
                encoders.[i].Encode fieldValue
            )
            |> seq
        )

    and inline private handleMapOrDict (extra : Map<string, ref<BoxedEncoder>>)
                                        (caseStrategy : CaseStrategy)
                                        (skipNullField : bool)
                                        (t : System.Type) : BoxedEncoder =
        let keyType = t.GenericTypeArguments.[0]
        let valueType = t.GenericTypeArguments.[1]
        let valueEncoder : BoxedEncoder =
            valueType
            |> autoEncoder extra caseStrategy skipNullField

        match keyType with
        | StringifiableType toString ->
            boxEncoder(fun (value : obj) ->
                #if FABLE_COMPILER
                let res = ResizeArray<(string * Json)>()

                // This cast to Map<string, obj> seems to be fine for Fable
                // We use the same trick for non stringifiable key
                // TODO: Does it works for .NET runtime?
                for KeyValue(key, value) in value :?> Map<string, obj> do
                    res.Add(toString key, valueEncoder.Encode value)

                res.ToArray()
                |> Map.ofArray
                |> Json.Object
                #endif

                #if !FABLE_COMPILER
                let res = ResizeArray()
                let kvProps = typedefof<KeyValuePair<obj, obj>>.MakeGenericType(keyType, valueType).GetProperties()

                for kv in value :?> System.Collections.IEnumerable do
                    let k = kvProps.[0].GetValue(kv)
                    let v = kvProps.[1].GetValue(kv)
                    res.Add(toString k, valueEncoder.Encode v)

                res
                |> Map.ofSeq
                |> Json.Object
                #endif
            )

        | _ ->
            boxEncoder(fun (value : obj) ->
                let keyEncoder : BoxedEncoder =
                    keyType
                    |> autoEncoder extra caseStrategy skipNullField

                #if FABLE_COMPILER
                let res = ResizeArray<Json>()

                // This cast to Map<string, obj> seems to be fine for Fable
                // We use the same trick for non stringifiable key
                for KeyValue(key, value) in value :?> Map<string, obj> do
                    res.Add(Json.Array [| keyEncoder.Encode key; valueEncoder.Encode value |])

                res.ToArray()
                |> Json.Array
                #endif

                #if !FABLE_COMPILER
                let res = ResizeArray<Json>()
                let kvProps = typedefof<KeyValuePair<obj, obj>>.MakeGenericType(keyType, valueType).GetProperties()

                for kv in value :?> System.Collections.IEnumerable do
                    let k = kvProps.[0].GetValue(kv)
                    let v = kvProps.[1].GetValue(kv)

                    res.Add(Json.Array [| keyEncoder.Encode k; valueEncoder.Encode v |])

                res.ToArray()
                |> Json.Array
                #endif
            )

    and inline private handleGeneric (extra : Map<string, ref<BoxedEncoder>>)
                                (caseStrategy : CaseStrategy)
                                (skipNullField : bool)
                                (t : System.Type) : BoxedEncoder =
        let fullName = t.GetGenericTypeDefinition().FullName

        if fullName = typedefof<obj option>.FullName then
            #if FABLE_COMPILER
            // Evaluate lazily so we don't need to generate the encoder for null values
            let encoder = lazy
                            ((autoEncoder extra caseStrategy skipNullField (t.GenericTypeArguments.[0])).Encode)
                            |> option
                            |> boxEncoder
            boxEncoder(fun (value: obj) ->
                if skipNullField && isNull value then
                    nil
                else
                    encoder.Value.Encode value
            )
            #endif

            #if !FABLE_COMPILER
            // Evaluate lazily so we don't need to generate the encoder for null values
            let encoder = lazy autoEncoder extra caseStrategy skipNullField t.GenericTypeArguments.[0]
            boxEncoder(fun (value: obj) ->
                if isNull value then
                    nil
                else
                    let _, fields = FSharpValue.GetUnionFields(value, t, allowAccessToPrivateRepresentation=true)
                    encoder.Value.Encode fields.[0]
            )
            #endif

        else if fullName = typedefof<obj list>.FullName
                    || fullName = typedefof<Set<string>>.FullName
                    || fullName = typedefof<HashSet<string>>.FullName
                    || fullName = typedefof<obj seq>.FullName then
            t.GenericTypeArguments.[0]
            |> autoEncoder extra caseStrategy skipNullField
            |> handleGenericSeq

        else if fullName = typedefof<Map<string, obj>>.FullName
                    || fullName = typedefof<Dictionary<string, obj>>.FullName then
            handleMapOrDict extra caseStrategy skipNullField t
        else
            handleRecordAndUnion extra caseStrategy skipNullField t

    and private autoEncoder (extra : Map<string, ref<BoxedEncoder>>)
                            (caseStrategy : CaseStrategy)
                            (skipNullField : bool)
                            (t : System.Type) : BoxedEncoder =
        let fullName = t.FullName
        match Map.tryFind fullName extra with
        | Some encoderRef ->
            boxEncoder(fun v -> encoderRef.contents.BoxedEncoder v)

        | None ->
            if t.IsArray then
                t.GetElementType()
                |> autoEncoder extra caseStrategy skipNullField
                |> handleGenericSeq
            else if t.IsEnum then
                handleEnum t fullName
            else if FSharpType.IsTuple(t) then
                handleTuple extra caseStrategy skipNullField t
            else if t.IsGenericType then
                handleGeneric extra caseStrategy skipNullField t
            else if fullName = typeof<bool>.FullName then
                boxEncoder bool
            else if fullName = typeof<unit>.FullName then
                boxEncoder unit
            else if fullName = typeof<string>.FullName then
                boxEncoder string
            else if fullName = typeof<char>.FullName then
                boxEncoder char
            else if fullName = typeof<sbyte>.FullName then
                boxEncoder sbyte
            else if fullName = typeof<byte>.FullName then
                boxEncoder byte
            else if fullName = typeof<int16>.FullName then
                boxEncoder int16
            else if fullName = typeof<uint16>.FullName then
                boxEncoder uint16
            else if fullName = typeof<int>.FullName then
                boxEncoder int
            else if fullName = typeof<uint32>.FullName then
                boxEncoder uint32
            else if fullName = typeof<float>.FullName then
                boxEncoder float
            else if fullName = typeof<float32>.FullName then
                boxEncoder float32
            else if fullName = typeof<System.DateTime>.FullName then
                boxEncoder datetime
            else if fullName = typeof<System.DateTimeOffset>.FullName then
                boxEncoder datetimeOffset
            else if fullName = typeof<System.TimeSpan>.FullName then
                boxEncoder timespan
            else if fullName = typeof<System.Guid>.FullName then
                boxEncoder guid
            // Allows to encode null values
            else if fullName = typeof<obj>.FullName then
                // If this is used only for Null value it should be ok
                boxEncoder(fun (v : obj) ->
                    Json.Null
                )
            else
                handleRecordAndUnion extra caseStrategy skipNullField t

    let private makeExtra (extra: ExtraCoders option) =
        match extra with
        | None -> Map.empty
        | Some e -> Map.map (fun _ (enc,_) -> ref enc) e.Coders

    module Auto =

        /// The goal of this API is to provide better interop when consuming Thoth.Json.Net from a C# project
        type LowLevel =
            /// ATTENTION: Use this only when other arguments (isCamelCase, extra) don't change
            static member generateEncoderCached<'T> (t: System.Type, ?caseStrategy : CaseStrategy, ?extra: ExtraCoders, ?skipNullField: bool): Encoder<'T> =
                let caseStrategy = defaultArg caseStrategy PascalCase
                let skipNullField = defaultArg skipNullField true

                let key =
                    t.FullName
                    |> (+) (Operators.string caseStrategy)
                    |> (+) (extra |> Option.map (fun e -> e.Hash) |> Option.defaultValue "")

                let encoderCrate =
                    Cache.Encoders.Value.GetOrAdd(key, fun _ ->
                        autoEncoder (makeExtra extra) caseStrategy skipNullField t)

                fun (value: 'T) ->
                    encoderCrate.Encode value

    type Auto =

        #if FABLE_COMPILER
        /// ATTENTION: Use this only when other arguments (caseStrategy, extra) don't change
        static member generateEncoderCached<'T>(?caseStrategy : CaseStrategy, ?extra: ExtraCoders, ?skipNullField: bool, [<Inject>] ?resolver : ITypeResolver<'T>): Encoder<'T> =
            let t = resolver.Value.ResolveType()
            Auto.LowLevel.generateEncoderCached(t, ?caseStrategy = caseStrategy, ?extra = extra, ?skipNullField=skipNullField)
        #endif

        #if !FABLE_COMPILER
        /// ATTENTION: Use this only when other arguments (caseStrategy, extra) don't change
        static member generateEncoderCached<'T>(?caseStrategy : CaseStrategy, ?extra: ExtraCoders, ?skipNullField: bool): Encoder<'T> =
            let t = typeof<'T>
            Auto.LowLevel.generateEncoderCached(t, ?caseStrategy = caseStrategy, ?extra = extra, ?skipNullField=skipNullField)
        #endif

        #if FABLE_COMPILER
        static member generateEncoder<'T>(?caseStrategy : CaseStrategy, ?extra: ExtraCoders, ?skipNullField: bool, [<Inject>] ?resolver : ITypeResolver<'T>): Encoder<'T> =
            let caseStrategy = defaultArg caseStrategy PascalCase
            let skipNullField = defaultArg skipNullField true
            let t = resolver.Value.ResolveType()
            let encoderCrate = autoEncoder (makeExtra extra) caseStrategy skipNullField t
            fun (value: 'T) ->
                encoderCrate.Encode value
        #endif

        #if !FABLE_COMPILER
        static member generateEncoder<'T>(?caseStrategy : CaseStrategy, ?extra: ExtraCoders, ?skipNullField: bool): Encoder<'T> =
            let caseStrategy = defaultArg caseStrategy PascalCase
            let skipNullField = defaultArg skipNullField true
            let encoderCrate = autoEncoder (makeExtra extra) caseStrategy skipNullField typeof<'T>
            fun (value: 'T) ->
                encoderCrate.Encode value
        #endif

        #if FABLE_COMPILER
        static member toString(space : int, value : 'T, ?caseStrategy : CaseStrategy, ?extra: ExtraCoders, ?skipNullField: bool, [<Inject>] ?resolver : ITypeResolver<'T>) : string =
            let encoder = Auto.generateEncoder(?caseStrategy=caseStrategy, ?extra=extra, ?skipNullField=skipNullField, ?resolver=resolver)
            encoder value |> toString space
        #endif

        #if !FABLE_COMPILER
        static member toString(space : int, value : 'T, ?caseStrategy : CaseStrategy, ?extra: ExtraCoders, ?skipNullField: bool) : string =
            let encoder = Auto.generateEncoder(?caseStrategy=caseStrategy, ?extra=extra, ?skipNullField=skipNullField)
            encoder value |> toString space
        #endif
