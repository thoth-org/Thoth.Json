namespace Thoth.Json
open System.Text.RegularExpressions

[<RequireQualifiedAccess>]
module Encode =

    open System.Collections.Generic
    open System.Globalization
    open Fable.Core
    open Fable.Core.JsInterop

    [<Emit("Array.from($0)")>]
    let private arrayFrom(x: JsonValue seq): JsonValue = jsNative

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
    let inline string (value : string) : JsonValue =
        box value

    let inline char (value : char) : JsonValue =
        box value

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
        box (value.ToString())

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
        box value

    let inline float32 (value : float32) : JsonValue =
        box value

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
        value.ToString() |> string

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
        box null

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
    let inline bool (value : bool) : JsonValue =
        box value

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
        let o = obj()
        for (key, value) in values do
            o?(key) <- value
        box o

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
    let inline array (values : JsonValue array) : JsonValue =
        box values

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
        // Don't use List.toArray as it may create a typed array
        arrayFrom values

    let seq (values : JsonValue seq) : JsonValue =
        arrayFrom values

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
        box (value.ToString())

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

    let inline sbyte (value : sbyte) : JsonValue =
        box value

    let inline byte (value : byte) : JsonValue =
        box value

    let inline int16 (value : int16) : JsonValue =
        box value

    let inline uint16 (value : uint16) : JsonValue =
        box value

    let inline int (value : int) : JsonValue =
        box value

    let inline uint32 (value : uint32) : JsonValue =
        box value

    let int64 (value : int64) : JsonValue =
        box (value.ToString(CultureInfo.InvariantCulture))

    let uint64 (value : uint64) : JsonValue =
        box (value.ToString(CultureInfo.InvariantCulture))

    let unit () : JsonValue =
        box null

    let tuple2
            (enc1 : Encoder<'T1>)
            (enc2 : Encoder<'T2>)
            (v1, v2) : JsonValue =
        box [| enc1 v1
               enc2 v2 |]

    let tuple3
            (enc1 : Encoder<'T1>)
            (enc2 : Encoder<'T2>)
            (enc3 : Encoder<'T3>)
            (v1, v2, v3) : JsonValue =
        box [| enc1 v1
               enc2 v2
               enc3 v3 |]

    let tuple4
            (enc1 : Encoder<'T1>)
            (enc2 : Encoder<'T2>)
            (enc3 : Encoder<'T3>)
            (enc4 : Encoder<'T4>)
            (v1, v2, v3, v4) : JsonValue =
        box [| enc1 v1
               enc2 v2
               enc3 v3
               enc4 v4 |]

    let tuple5
            (enc1 : Encoder<'T1>)
            (enc2 : Encoder<'T2>)
            (enc3 : Encoder<'T3>)
            (enc4 : Encoder<'T4>)
            (enc5 : Encoder<'T5>)
            (v1, v2, v3, v4, v5) : JsonValue =
        box [| enc1 v1
               enc2 v2
               enc3 v3
               enc4 v4
               enc5 v5 |]

    let tuple6
            (enc1 : Encoder<'T1>)
            (enc2 : Encoder<'T2>)
            (enc3 : Encoder<'T3>)
            (enc4 : Encoder<'T4>)
            (enc5 : Encoder<'T5>)
            (enc6 : Encoder<'T6>)
            (v1, v2, v3, v4, v5, v6) : JsonValue =
        box [| enc1 v1
               enc2 v2
               enc3 v3
               enc4 v4
               enc5 v5
               enc6 v6 |]

    let tuple7
            (enc1 : Encoder<'T1>)
            (enc2 : Encoder<'T2>)
            (enc3 : Encoder<'T3>)
            (enc4 : Encoder<'T4>)
            (enc5 : Encoder<'T5>)
            (enc6 : Encoder<'T6>)
            (enc7 : Encoder<'T7>)
            (v1, v2, v3, v4, v5, v6, v7) : JsonValue =
        box [| enc1 v1
               enc2 v2
               enc3 v3
               enc4 v4
               enc5 v5
               enc6 v6
               enc7 v7 |]

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
        box [| enc1 v1
               enc2 v2
               enc3 v3
               enc4 v4
               enc5 v5
               enc6 v6
               enc7 v7
               enc8 v8 |]

    let map (keyEncoder : Encoder<'key>) (valueEncoder : Encoder<'value>) (values : Map<'key, 'value>) : JsonValue =
        values
        |> Map.toList
        |> List.map (tuple2 keyEncoder valueEncoder)
        |> list

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
       JS.JSON.stringify(value, space=space)

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
    let option (encoder : 'a -> JsonValue) =
        Option.map encoder >> Option.defaultWith (fun _ -> nil)

    //////////////////
    // Reflection ///
    ////////////////

    open FSharp.Reflection
    open Fable.Core.DynamicExtensions

    // As generics are erased by Fable, let's just do an unsafe cast for performance
    let inline boxEncoder (d: Encoder<'T>): BoxedEncoder =
        !!d

    let inline unboxEncoder (d: BoxedEncoder): Encoder<'T> =
        !!d

    let rec private autoEncodeRecordsAndUnions extra (caseStrategy : CaseStrategy) (skipNullField : bool) (t: System.Type) : BoxedEncoder =
        // Add the encoder to extra in case one of the fields is recursive
        let encoderRef = ref Unchecked.defaultof<_>
        let extra =
            // As of 3.7.17 Fable assigns empty name to anonymous record, we shouldn't add them to the map to avoid conflicts.
            // Anonymous records cannot be recursive anyways, see #144
            match t.FullName with
            | "" -> extra
            | fullName -> extra |> Map.add fullName encoderRef
        let encoder =
            if FSharpType.IsRecord(t, allowAccessToPrivateRepresentation=true) then
                let setters =
                    FSharpType.GetRecordFields(t, allowAccessToPrivateRepresentation=true)
                    |> Array.map (fun fi ->
                        let targetKey = Util.Casing.convert caseStrategy fi.Name
                        let encode = autoEncoder extra caseStrategy skipNullField fi.PropertyType
                        fun (source: obj) (target: JsonValue) ->
                            let value = FSharpValue.GetRecordField(source, fi)
                            if not skipNullField || (skipNullField && not (isNull value)) then // Discard null fields
                                target.[targetKey] <- encode value
                            target)
                fun (source: obj) ->
                    (JsonValue(), setters) ||> Seq.fold (fun target set -> set source target)
            elif FSharpType.IsUnion(t, allowAccessToPrivateRepresentation=true) then
                fun (value: obj) ->
                    let info, fields = FSharpValue.GetUnionFields(value, t, allowAccessToPrivateRepresentation=true)
                    match fields.Length with
                    | 0 -> string info.Name
                    | len ->
                        let fieldTypes = info.GetFields()
                        let target = Array.zeroCreate<JsonValue> (len + 1)
                        target.[0] <- string info.Name
                        for i = 1 to len do
                            let encode = autoEncoder extra caseStrategy skipNullField fieldTypes.[i-1].PropertyType
                            target.[i] <- encode fields.[i-1]
                        array target
            else
                // Don't use failwithf here, for some reason F#/Fable compiles it as a function
                // when the return type is a function too, so it doesn't fail immediately
                sprintf """Cannot generate auto encoder for %s. Please pass an extra encoder.

Documentation available at: https://thoth-org.github.io/Thoth.Json/documentation/auto/extra-coders.html#ready-to-use-extra-coders""" t.FullName
                |> failwith
        encoderRef.Value <- encoder
        encoder

    and private autoEncoder (extra: Map<string, ref<BoxedEncoder>>) caseStrategy (skipNullField : bool) (t: System.Type) : BoxedEncoder =
      let fullname = t.FullName
      match Map.tryFind fullname extra with
      | Some encoderRef -> fun v -> encoderRef.contents v
      | None ->
        if t.IsArray then
            let encoder = t.GetElementType() |> autoEncoder extra caseStrategy skipNullField
            fun (value: obj) ->
                value :?> obj seq |> Seq.map encoder |> seq
        elif t.IsEnum then
            let enumType = System.Enum.GetUnderlyingType(t).FullName
            if enumType = typeof<sbyte>.FullName then
                boxEncoder sbyte
            elif enumType = typeof<byte>.FullName then
                boxEncoder byte
            elif enumType = typeof<int16>.FullName then
                boxEncoder int16
            elif enumType = typeof<uint16>.FullName then
                boxEncoder uint16
            elif enumType = typeof<int>.FullName then
                boxEncoder int
            elif enumType = typeof<uint32>.FullName then
                boxEncoder uint32
            else
                failwithf
                    """Cannot generate auto encoder for %s.
Thoth.Json.Net only support the following enum types:
- sbyte
- byte
- int16
- uint16
- int
- uint32

If you can't use one of these types, please pass an extra encoder.

Documentation available at: https://thoth-org.github.io/Thoth.Json/documentation/auto/extra-coders.html#ready-to-use-extra-coders
                    """ t.FullName
        elif t.IsGenericType then
            if FSharpType.IsTuple(t) then
                let encoders =
                    FSharpType.GetTupleElements(t)
                    |> Array.map (autoEncoder extra caseStrategy skipNullField)
                fun (value: obj) ->
                    FSharpValue.GetTupleFields(value)
                    |> Seq.mapi (fun i x -> encoders.[i] x) |> seq
            else
                let fullname = t.GetGenericTypeDefinition().FullName
                if fullname = typedefof<obj option>.FullName then
                    // Evaluate lazily so we don't need to generate the encoder for null values
                    let encoder = lazy
                                    t.GenericTypeArguments.[0]
                                    |> autoEncoder extra caseStrategy skipNullField
                                    |> option
                                    |> boxEncoder
                    boxEncoder(fun (value: obj) ->
                        if isNull value then nil
                        else encoder.Value value)
                elif fullname = typedefof<obj list>.FullName
                    || fullname = typedefof<Set<string>>.FullName
                    || fullname = typedefof<obj seq>.FullName then
                    let encoder = t.GenericTypeArguments.[0] |> autoEncoder extra caseStrategy skipNullField
                    fun (value: obj) ->
                        value :?> obj seq |> Seq.map encoder |> seq
                elif fullname = typedefof< Map<string, obj> >.FullName then
                    let keyType = t.GenericTypeArguments.[0]
                    let valueEncoder = t.GenericTypeArguments.[1] |> autoEncoder extra caseStrategy skipNullField
                    if keyType.FullName = typeof<string>.FullName
                        || keyType.FullName = typeof<System.Guid>.FullName then
                        fun value ->
                            // Fable compiles Guids as strings so this works, but maybe we should make the conversion explicit
                            // (see dotnet version) in case Fable implementation of Guids change
                            (JsonValue(), value :?> Map<string, obj>)
                            ||> Seq.fold (fun target (KeyValue(k,v)) ->
                                target.[k] <- valueEncoder v
                                target)
                    else
                        let keyEncoder = keyType |> autoEncoder extra caseStrategy skipNullField
                        fun value ->
                            value :?> Map<string, obj> |> Seq.map (fun (KeyValue(k,v)) ->
                                array [|keyEncoder k; valueEncoder v|]) |> seq
                else
                    autoEncodeRecordsAndUnions extra caseStrategy skipNullField t
        else
            if fullname = typeof<bool>.FullName then
                boxEncoder bool
            elif fullname = typeof<unit>.FullName then
                boxEncoder unit
            elif fullname = typeof<string>.FullName then
                boxEncoder string
            elif fullname = typeof<char>.FullName then
                boxEncoder char
            elif fullname = typeof<sbyte>.FullName then
                boxEncoder sbyte
            elif fullname = typeof<byte>.FullName then
                boxEncoder byte
            elif fullname = typeof<int16>.FullName then
                boxEncoder int16
            elif fullname = typeof<uint16>.FullName then
                boxEncoder uint16
            elif fullname = typeof<int>.FullName then
                boxEncoder int
            elif fullname = typeof<uint32>.FullName then
                boxEncoder uint32
            elif fullname = typeof<float>.FullName then
                boxEncoder float
            elif fullname = typeof<float32>.FullName then
                boxEncoder float32
            // These number types require extra libraries in Fable. To prevent penalizing
            // all users, extra encoders (withInt64, etc) must be passed when they're needed.

            // elif fullname = typeof<int64>.FullName then
            //     boxEncoder int64
            // elif fullname = typeof<uint64>.FullName then
            //     boxEncoder uint64
            // elif fullname = typeof<bigint>.FullName then
            //     boxEncoder bigint
            // elif fullname = typeof<decimal>.FullName then
            //     boxEncoder decimal
            elif fullname = typeof<System.DateTime>.FullName then
                boxEncoder datetime
            elif fullname = typeof<System.DateTimeOffset>.FullName then
                boxEncoder datetimeOffset
            elif fullname = typeof<System.TimeSpan>.FullName then
                boxEncoder timespan
            elif fullname = typeof<System.Guid>.FullName then
                boxEncoder guid
            elif fullname = typeof<obj>.FullName then
                boxEncoder id
            else
                autoEncodeRecordsAndUnions extra caseStrategy skipNullField t

    let private makeExtra (extra: ExtraCoders option) =
        match extra with
        | None -> Map.empty
        | Some e -> Map.map (fun _ (enc,_) -> ref enc) e.Coders

    type Auto =
        static member generateBoxedEncoderCached(t: System.Type, ?caseStrategy : CaseStrategy, ?extra: ExtraCoders, ?skipNullField: bool): BoxedEncoder =
            let caseStrategy = defaultArg caseStrategy PascalCase
            let skipNullField = defaultArg skipNullField true

            let key =
                t.FullName
                |> (+) (Operators.string caseStrategy)
                |> (+) (extra |> Option.map (fun e -> e.Hash) |> Option.defaultValue "")

            Util.CachedEncoders.GetOrAdd(key , fun _ ->
                autoEncoder (makeExtra extra) caseStrategy skipNullField t)

        static member inline generateEncoderCached<'T>(?caseStrategy : CaseStrategy, ?extra: ExtraCoders, ?skipNullField: bool): Encoder<'T> =
            Auto.generateBoxedEncoderCached(typeof<'T>, ?caseStrategy=caseStrategy, ?extra=extra) |> unboxEncoder

        static member generateBoxedEncoder(t: System.Type, ?caseStrategy : CaseStrategy, ?extra: ExtraCoders, ?skipNullField: bool): BoxedEncoder =
            let caseStrategy = defaultArg caseStrategy PascalCase
            let skipNullField = defaultArg skipNullField true
            autoEncoder (makeExtra extra) caseStrategy skipNullField t

        static member inline generateEncoder<'T>(?caseStrategy : CaseStrategy, ?extra: ExtraCoders, ?skipNullField: bool): Encoder<'T> =
            Auto.generateBoxedEncoder(typeof<'T>, ?caseStrategy=caseStrategy, ?extra=extra, ?skipNullField=skipNullField) |> unboxEncoder

        static member inline toString(space : int, value : 'T, ?caseStrategy : CaseStrategy, ?extra: ExtraCoders, ?skipNullField: bool) : string =
            let encoder = Auto.generateEncoder(?caseStrategy=caseStrategy, ?extra=extra, ?skipNullField=skipNullField)
            encoder value |> toString space

        static member inline toString(value : 'T, ?caseStrategy : CaseStrategy, ?extra: ExtraCoders, ?skipNullField: bool) : string =
            Auto.toString(0, value, ?caseStrategy=caseStrategy, ?extra=extra, ?skipNullField=skipNullField)

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
    [<System.Obsolete("Please use toString instead")>]
    let encode (space: int) (value: JsonValue) : string = toString space value
