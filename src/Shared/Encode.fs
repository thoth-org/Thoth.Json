#if THOTH_JSON
namespace Thoth.Json

open System.Text
open Thoth.Json.Parser

#endif

#if THOTH_JSON_FABLE
namespace Thoth.Json.Fable
#endif

#if THOTH_JSON_NEWTONSOFT
namespace Thoth.Json.Newtonsoft
#endif

[<RequireQualifiedAccess>]
module Encode =

    open System.Collections.Generic
    open System.Globalization

    #if THOTH_JSON
    open Fable.Core
    #endif

    #if THOTH_JSON_FABLE
    open Fable.Core
    open Fable.Core.JsInterop
    #endif

    #if THOTH_JSON_NEWTONSOFT
    open Newtonsoft.Json
    open Newtonsoft.Json.Linq
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
    #if THOTH_JSON
    let string (value : string) : JsonValue =
        // If the string is null we represent it using null
        // This is similar to what the other runtime do, THOTH_JSON parser is more strict by default
        // so we need mix a bit of the Parser logic with the decoder
        if value = null then
            Json.Null
        else
            value |> Json.String
    #endif

    #if THOTH_JSON_FABLE
    let inline string (value : string) : JsonValue =
        box value
    #endif

    #if THOTH_JSON_NEWTONSOFT
    let string (value : string) : JsonValue =
        JValue(value) :> JsonValue
    #endif

    #if THOTH_JSON
    let char (value : char) : JsonValue =
        (Operators.string value) |> Json.String
    #endif

    #if THOTH_JSON_FABLE
    let inline char (value : char) : JsonValue =
        box value
    #endif

    #if THOTH_JSON_NEWTONSOFT
    let char (value : char) : JsonValue =
        JValue(value) :> JsonValue
    #endif

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
        #if THOTH_JSON
        value.ToString() |> Json.String
        #endif

        #if THOTH_JSON_FABLE
        box (value.ToString())
        #endif

        #if THOTH_JSON_NEWTONSOFT
        value.ToString() |> string
        #endif

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
    #if THOTH_JSON
    let inline float (value : float) : JsonValue =
        value |> Json.Number
    #endif

    #if THOTH_JSON_FABLE
    let inline float (value : float) : JsonValue =
        box value
    #endif

    #if THOTH_JSON_NEWTONSOFT
    let float (value : float) : JsonValue =
        JValue(value) :> JsonValue
    #endif

    #if THOTH_JSON
    let float32 (value : float32) : JsonValue =
        // I think we are loosing some precision here
        Operators.float value |> Json.Number
    #endif

    #if THOTH_JSON_FABLE
    let inline float32 (value : float32) : JsonValue =
        box value
    #endif

    #if THOTH_JSON_NEWTONSOFT
    let float32 (value : float32) : JsonValue =
        JValue(value) :> JsonValue
    #endif

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
        #if THOTH_JSON
        value.ToString(CultureInfo.InvariantCulture) |> Json.String
        #endif

        #if THOTH_JSON_FABLE
        value.ToString() |> string
        #endif

        #if THOTH_JSON_NEWTONSOFT
        JValue(value.ToString(CultureInfo.InvariantCulture)) :> JsonValue
        #endif

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
        #if THOTH_JSON
        Json.Null
        #endif

        #if THOTH_JSON_FABLE
        box null
        #endif

        #if THOTH_JSON_NEWTONSOFT
        JValue.CreateNull() :> JsonValue
        #endif

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
    #if THOTH_JSON
    let bool (value : bool) : JsonValue =
        Json.Bool value
    #endif

    #if THOTH_JSON_FABLE
    let inline bool (value : bool) : JsonValue =
        box value
    #endif

    #if THOTH_JSON_NEWTONSOFT
    let bool (value : bool) : JsonValue =
        JValue(value) :> JsonValue
    #endif

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
        #if THOTH_JSON
        values
        |> Map.ofSeq
        |> Json.Object
        #endif

        #if THOTH_JSON_FABLE
        let o = obj()
        for (key, value) in values do
            o?(key) <- value
        box o
        #endif

        #if THOTH_JSON_NEWTONSOFT
        values
        |> Seq.map (fun (key, value) ->
            JProperty(key, value)
        )
        |> JObject :> JsonValue
        #endif

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
    #if THOTH_JSON
    let array (values : JsonValue array) : JsonValue =
        Json.Array values
    #endif

    #if THOTH_JSON_FABLE
    let inline array (values : JsonValue array) : JsonValue =
        box values
    #endif

    #if THOTH_JSON_NEWTONSOFT
    let array (values : JsonValue array) : JsonValue =
        JArray(values) :> JsonValue
    #endif

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
        #if THOTH_JSON
        values |> List.toArray |> Json.Array
        #endif

        #if THOTH_JSON_FABLE
        // Don't use List.toArray as it may create a typed array
        Helpers.arrayFrom values
        #endif

        #if THOTH_JSON_NEWTONSOFT
        JArray(values) :> JsonValue
        #endif

    let seq (values : JsonValue seq) : JsonValue =
        #if THOTH_JSON
        values |> Seq.toArray |> Json.Array
        #endif

        #if THOTH_JSON_FABLE
        Helpers.arrayFrom values
        #endif

        #if THOTH_JSON_NEWTONSOFT
        JArray(values) :> JsonValue
        #endif

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
        #if THOTH_JSON
        value.ToString(CultureInfo.InvariantCulture) |> Json.String
        #endif

        #if THOTH_JSON_FABLE
        box (value.ToString())
        #endif

        #if THOTH_JSON_NEWTONSOFT
        JValue(value.ToString(CultureInfo.InvariantCulture)) :> JsonValue
        #endif

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
        #if THOTH_JSON
        value.ToString("O", CultureInfo.InvariantCulture) |> string
        #endif

        #if THOTH_JSON_FABLE
        value.ToString("O", CultureInfo.InvariantCulture) |> string
        #endif

        #if THOTH_JSON_NEWTONSOFT
        JValue(value.ToString("O", CultureInfo.InvariantCulture)) :> JsonValue
        #endif

    let datetimeOffset (value : System.DateTimeOffset) : JsonValue =
        #if THOTH_JSON
        value.ToString("O", CultureInfo.InvariantCulture) |> string
        #endif

        #if THOTH_JSON_FABLE
        value.ToString("O", CultureInfo.InvariantCulture) |> string
        #endif

        #if THOTH_JSON_NEWTONSOFT
        JValue(value.ToString("O", CultureInfo.InvariantCulture)) :> JsonValue
        #endif

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
        #if THOTH_JSON
        value.ToString() |> string
        #endif

        #if THOTH_JSON_FABLE
        value.ToString() |> string
        #endif

        #if THOTH_JSON_NEWTONSOFT
        JValue(value.ToString()) :> JsonValue
        #endif

    let sbyte (value : sbyte) : JsonValue =
        #if THOTH_JSON
        value.ToString(CultureInfo.InvariantCulture) |> string
        #endif

        #if THOTH_JSON_FABLE
        box (value.ToString(CultureInfo.InvariantCulture))
        #endif

        #if THOTH_JSON_NEWTONSOFT
        JValue(value.ToString(CultureInfo.InvariantCulture)) :> JsonValue
        #endif

    let byte (value : byte) : JsonValue =
        #if THOTH_JSON
        value.ToString(CultureInfo.InvariantCulture) |> string
        #endif

        #if THOTH_JSON_FABLE
        box (value.ToString(CultureInfo.InvariantCulture))
        #endif

        #if THOTH_JSON_NEWTONSOFT
        JValue(value.ToString(CultureInfo.InvariantCulture)) :> JsonValue
        #endif

    let int16 (value : int16) : JsonValue =
        #if THOTH_JSON
        value.ToString(CultureInfo.InvariantCulture) |> string
        #endif

        #if THOTH_JSON_FABLE
        box (value.ToString(CultureInfo.InvariantCulture))
        #endif

        #if THOTH_JSON_NEWTONSOFT
        JValue(value.ToString(CultureInfo.InvariantCulture)) :> JsonValue
        #endif

    let uint16 (value : uint16) : JsonValue =
        #if THOTH_JSON
        value.ToString(CultureInfo.InvariantCulture) |> string
        #endif

        #if THOTH_JSON_FABLE
        box (value.ToString(CultureInfo.InvariantCulture))
        #endif

        #if THOTH_JSON_NEWTONSOFT
        JValue(value.ToString(CultureInfo.InvariantCulture)) :> JsonValue
        #endif

    let int (value : int) : JsonValue =
        #if THOTH_JSON
        Operators.float value |> Json.Number
        #endif

        #if THOTH_JSON_FABLE
        box value
        #endif

        #if THOTH_JSON_NEWTONSOFT
        JValue(value) :> JsonValue
        #endif

    let uint32 (value : uint32) : JsonValue =
        #if THOTH_JSON
        Operators.float value |> Json.Number
        #endif

        #if THOTH_JSON_FABLE
        box value
        #endif

        #if THOTH_JSON_NEWTONSOFT
        JValue(value) :> JsonValue
        #endif

    let int64 (value : int64) : JsonValue =
        #if THOTH_JSON
        value.ToString(CultureInfo.InvariantCulture) |> Json.String
        #endif

        #if THOTH_JSON_FABLE
        box (value.ToString(CultureInfo.InvariantCulture))
        #endif

        #if THOTH_JSON_NEWTONSOFT
        JValue(value.ToString(CultureInfo.InvariantCulture)) :> JsonValue
        #endif

    let uint64 (value : uint64) : JsonValue =
        #if THOTH_JSON
        value.ToString(CultureInfo.InvariantCulture) |> Json.String
        #endif

        #if THOTH_JSON_FABLE
        box (value.ToString(CultureInfo.InvariantCulture))
        #endif

        #if THOTH_JSON_NEWTONSOFT
        JValue(value.ToString(CultureInfo.InvariantCulture)) :> JsonValue
        #endif

    let unit () : JsonValue =
        #if THOTH_JSON
        Json.Null
        #endif

        #if THOTH_JSON_FABLE
        box null
        #endif

        #if THOTH_JSON_NEWTONSOFT
        JValue.CreateNull() :> JsonValue
        #endif

    let tuple2
            (enc1 : Encoder<'T1>)
            (enc2 : Encoder<'T2>)
            (v1, v2) : JsonValue =
        #if THOTH_JSON_FABLE
        box [| enc1 v1
               enc2 v2 |]
        #endif

        #if THOTH_JSON_NEWTONSOFT || THOTH_JSON
        [| enc1 v1
           enc2 v2 |] |> array
        #endif

    let tuple3
            (enc1 : Encoder<'T1>)
            (enc2 : Encoder<'T2>)
            (enc3 : Encoder<'T3>)
            (v1, v2, v3) : JsonValue =
        #if THOTH_JSON_FABLE
        box [| enc1 v1
               enc2 v2
               enc3 v3 |]
        #endif

        #if THOTH_JSON_NEWTONSOFT || THOTH_JSON
        [| enc1 v1
           enc2 v2
           enc3 v3 |] |> array
        #endif

    let tuple4
            (enc1 : Encoder<'T1>)
            (enc2 : Encoder<'T2>)
            (enc3 : Encoder<'T3>)
            (enc4 : Encoder<'T4>)
            (v1, v2, v3, v4) : JsonValue =
        #if THOTH_JSON_FABLE
        box [| enc1 v1
               enc2 v2
               enc3 v3
               enc4 v4 |]
        #endif

        #if THOTH_JSON_NEWTONSOFT || THOTH_JSON
        [| enc1 v1
           enc2 v2
           enc3 v3
           enc4 v4 |] |> array
        #endif

    let tuple5
            (enc1 : Encoder<'T1>)
            (enc2 : Encoder<'T2>)
            (enc3 : Encoder<'T3>)
            (enc4 : Encoder<'T4>)
            (enc5 : Encoder<'T5>)
            (v1, v2, v3, v4, v5) : JsonValue =
        #if THOTH_JSON_FABLE
        box [| enc1 v1
               enc2 v2
               enc3 v3
               enc4 v4
               enc5 v5 |]
        #endif

        #if THOTH_JSON_NEWTONSOFT || THOTH_JSON
        [| enc1 v1
           enc2 v2
           enc3 v3
           enc4 v4
           enc5 v5 |] |> array
        #endif

    let tuple6
            (enc1 : Encoder<'T1>)
            (enc2 : Encoder<'T2>)
            (enc3 : Encoder<'T3>)
            (enc4 : Encoder<'T4>)
            (enc5 : Encoder<'T5>)
            (enc6 : Encoder<'T6>)
            (v1, v2, v3, v4, v5, v6) : JsonValue =
        #if THOTH_JSON_FABLE
        box [|  enc1 v1
                enc2 v2
                enc3 v3
                enc4 v4
                enc5 v5
                enc6 v6 |]
        #endif

        #if THOTH_JSON_NEWTONSOFT || THOTH_JSON
        [| enc1 v1
           enc2 v2
           enc3 v3
           enc4 v4
           enc5 v5
           enc6 v6 |] |> array
        #endif

    let tuple7
            (enc1 : Encoder<'T1>)
            (enc2 : Encoder<'T2>)
            (enc3 : Encoder<'T3>)
            (enc4 : Encoder<'T4>)
            (enc5 : Encoder<'T5>)
            (enc6 : Encoder<'T6>)
            (enc7 : Encoder<'T7>)
            (v1, v2, v3, v4, v5, v6, v7) : JsonValue =
        #if THOTH_JSON_FABLE
        box [|  enc1 v1
                enc2 v2
                enc3 v3
                enc4 v4
                enc5 v5
                enc6 v6
                enc7 v7 |]
        #endif

        #if THOTH_JSON_NEWTONSOFT || THOTH_JSON
        [| enc1 v1
           enc2 v2
           enc3 v3
           enc4 v4
           enc5 v5
           enc6 v6
           enc7 v7 |] |> array
        #endif

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
        #if THOTH_JSON_FABLE
        box [|  enc1 v1
                enc2 v2
                enc3 v3
                enc4 v4
                enc5 v5
                enc6 v6
                enc7 v7
                enc8 v8 |]
        #endif

        #if THOTH_JSON_NEWTONSOFT || THOTH_JSON
        [| enc1 v1
           enc2 v2
           enc3 v3
           enc4 v4
           enc5 v5
           enc6 v6
           enc7 v7
           enc8 v8 |] |> array
        #endif

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
        #if THOTH_JSON
        let builder = new StringBuilder()
        let format =
            if space = 0 then
                JsonSaveOptions.DisableFormatting
            else
                JsonSaveOptions.Format

        value.Write(builder, format, space)

//        let bytes = Encoding.Default.GetBytes(builder.ToString())
//        Encoding.ASCII.GetString(bytes)
        builder.ToString()
        #endif

        #if THOTH_JSON_FABLE
        JS.JSON.stringify(value, !!null, space)
        #endif

        #if THOTH_JSON_NEWTONSOFT
        let format = if space = 0 then Formatting.None else Formatting.Indented
        use stream = new StringWriter(NewLine = "\n")
        use jsonWriter = new JsonTextWriter(
                            stream,
                            Formatting = format,
                            Indentation = space )

        value.WriteTo(jsonWriter)
        stream.ToString()
        #endif

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
    #if THOTH_JSON_FABLE
    open Fable.Core.DynamicExtensions
    #endif

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

                #if THOTH_JSON
                fun (source : obj) (res : Map<string, Json>) ->
                    let value = FSharpValue.GetRecordField(source, propertyInfo)
                    // Discard null value
                    if not skipNullField || (skipNullField && not (isNull value)) then
                        Map.add targetKey (encoder.Encode value) res
                    else
                        res
                #endif

                #if THOTH_JSON_FABLE
                fun (source : obj) (res : JsonValue) ->
                #endif

                #if THOTH_JSON_NEWTONSOFT
                fun (source : obj) (res : JObject) ->
                #endif

                #if THOTH_JSON_NEWTONSOFT || THOTH_JSON_FABLE
                    let value = FSharpValue.GetRecordField(source, propertyInfo)
                    // Discard null value
                    if not skipNullField || (skipNullField && not (isNull value)) then
                        res.[targetKey] <- encoder.Encode value
                    res
                #endif
            )

        boxEncoder(fun (value : obj) ->
            #if THOTH_JSON
            (Map.empty<string, Json>, setters)
            ||> Array.fold (fun res set ->
                set value res
            )
            |> Json.Object
            #endif

            #if THOTH_JSON_FABLE
            (JsonValue(), setters)
            ||> Array.fold (fun res set ->
                set value res
            )
            #endif

            #if THOTH_JSON_NEWTONSOFT
            (JObject(), setters)
            ||> Array.fold (fun res set ->
                set value res
            ) :> JsonValue
            #endif
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
                let fieldTypes = info.GetFields()
                // There is only one field so we can use a direct access to it
                let encoder : BoxedEncoder = autoEncoder extra caseStrategy skipNullField fieldTypes.[0].PropertyType
                encoder.Encode(fields.[0])

            | _ ->
                match fields.Length with
                | 0 ->
                    #if !NETFRAMEWORK && !THOTH_JSON_FABLE && !(THOTH_JSON && FABLE_COMPILER)
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
            #if THOTH_JSON
            let array = ResizeArray<Json>()
            #endif

            #if THOTH_JSON_FABLE
            let array = ResizeArray<obj>()
            #endif

            #if THOTH_JSON_NEWTONSOFT
            let array = JArray()
            #endif

            for element in elements :?> System.Collections.IEnumerable do
                array.Add(encoder.Encode element)

            #if THOTH_JSON
            array.ToArray() |> Json.Array
            #endif

            #if THOTH_JSON_FABLE || THOTH_JSON_NEWTONSOFT
            array :> JsonValue
            #endif
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
                #if THOTH_JSON && FABLE_COMPILER
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

                #if THOTH_JSON_FABLE
                let res = JsonValue()

                // This cast to Map<string, obj> seems to be fine for Fable
                // We use the same trick for non stringifiable key
                for KeyValue(key, value) in value :?> Map<string, obj> do
                    res.[toString key] <- valueEncoder.Encode value

                res
                #endif

                #if THOTH_JSON_NEWTONSOFT
                let res = JObject()
                let kvProps = typedefof<KeyValuePair<obj, obj>>.MakeGenericType(keyType, valueType).GetProperties()

                for kv in value :?> System.Collections.IEnumerable do
                    let k = kvProps.[0].GetValue(kv)
                    let v = kvProps.[1].GetValue(kv)
                    res.[toString k] <- valueEncoder.Encode v

                res :> JsonValue
                #endif

                #if THOTH_JSON && !FABLE_COMPILER
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

                #if THOTH_JSON && FABLE_COMPILER
                let res = ResizeArray<Json>()

                // This cast to Map<string, obj> seems to be fine for Fable
                // We use the same trick for non stringifiable key
                // TODO: Does it works for .NET runtime?
                for KeyValue(key, value) in value :?> Map<string, obj> do
                    res.Add(Json.Array [| keyEncoder.Encode key; valueEncoder.Encode value |])

                res.ToArray()
                |> Json.Array
                #endif

                #if THOTH_JSON_FABLE
                let res = ResizeArray<obj>()

                for KeyValue(key, value) in value :?> Map<string, obj> do
                    res.Add(ResizeArray<obj>([ keyEncoder.Encode key; valueEncoder.Encode value ]))

                res :> JsonValue
                #endif

                #if THOTH_JSON_NEWTONSOFT
                let res = JArray()
                let kvProps = typedefof<KeyValuePair<obj, obj>>.MakeGenericType(keyType, valueType).GetProperties()

                for kv in value :?> System.Collections.IEnumerable do
                    let k = kvProps.[0].GetValue(kv)
                    let v = kvProps.[1].GetValue(kv)

                    res.Add(JArray [| keyEncoder.Encode k; valueEncoder.Encode v |])

                res :> JsonValue
                #endif

                #if (THOTH_JSON && !FABLE_COMPILER)
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
            #if THOTH_JSON && FABLE_COMPILER
            // Evaluate lazily so we don't need to generate the encoder for null values
            // TODO: Adapt for running on .NET ?
            let encoder = lazy
                            ((autoEncoder extra caseStrategy skipNullField (t.GenericTypeArguments.[0])).Encode)
                            |> option
                            |> boxEncoder
            boxEncoder(fun (value: obj) ->
                if isNull value then
                    nil
                else
                    encoder.Value.Encode value
            )
            #endif

            #if THOTH_JSON_FABLE
            // Evaluate lazily so we don't need to generate the encoder for null values
            let encoder = lazy
                            ((autoEncoder extra caseStrategy skipNullField (t.GenericTypeArguments.[0])).Encode)
                            |> option
                            |> boxEncoder
            boxEncoder(fun (value: obj) ->
                if isNull value then
                    nil
                else
                    encoder.Value.Encode value
            )
            #endif

            #if THOTH_JSON_NEWTONSOFT || (THOTH_JSON && !FABLE_COMPILER)
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
                #if THOTH_JSON
                // If this is used only for Null value it should be ok
                boxEncoder(fun (v : obj) ->
                    Json.Null
                )
                #endif

                #if THOTH_JSON_FABLE
                boxEncoder id
                #endif

                #if THOTH_JSON_NEWTONSOFT
                boxEncoder(fun (v: obj) -> JValue(v) :> JsonValue)
                #endif
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

        #if THOTH_JSON_FABLE || (THOTH_JSON && FABLE_COMPILER)
        /// ATTENTION: Use this only when other arguments (caseStrategy, extra) don't change
        static member generateEncoderCached<'T>(?caseStrategy : CaseStrategy, ?extra: ExtraCoders, ?skipNullField: bool, [<Inject>] ?resolver : ITypeResolver<'T>): Encoder<'T> =
            let t = resolver.Value.ResolveType()
            Auto.LowLevel.generateEncoderCached(t, ?caseStrategy = caseStrategy, ?extra = extra, ?skipNullField=skipNullField)
        #endif

        #if THOTH_JSON_NEWTONSOFT || (THOTH_JSON && !FABLE_COMPILER)
        /// ATTENTION: Use this only when other arguments (caseStrategy, extra) don't change
        static member generateEncoderCached<'T>(?caseStrategy : CaseStrategy, ?extra: ExtraCoders, ?skipNullField: bool): Encoder<'T> =
            let t = typeof<'T>
            Auto.LowLevel.generateEncoderCached(t, ?caseStrategy = caseStrategy, ?extra = extra, ?skipNullField=skipNullField)
        #endif

        #if THOTH_JSON_FABLE || (THOTH_JSON && FABLE_COMPILER)
        static member generateEncoder<'T>(?caseStrategy : CaseStrategy, ?extra: ExtraCoders, ?skipNullField: bool, [<Inject>] ?resolver : ITypeResolver<'T>): Encoder<'T> =
            let caseStrategy = defaultArg caseStrategy PascalCase
            let skipNullField = defaultArg skipNullField true
            let t = resolver.Value.ResolveType()
            let encoderCrate = autoEncoder (makeExtra extra) caseStrategy skipNullField t
            fun (value: 'T) ->
                encoderCrate.Encode value
        #endif

        #if THOTH_JSON_NEWTONSOFT || (THOTH_JSON && !FABLE_COMPILER)
        static member generateEncoder<'T>(?caseStrategy : CaseStrategy, ?extra: ExtraCoders, ?skipNullField: bool): Encoder<'T> =
            let caseStrategy = defaultArg caseStrategy PascalCase
            let skipNullField = defaultArg skipNullField true
            let encoderCrate = autoEncoder (makeExtra extra) caseStrategy skipNullField typeof<'T>
            fun (value: 'T) ->
                encoderCrate.Encode value
        #endif

        #if THOTH_JSON_FABLE || (THOTH_JSON && FABLE_COMPILER)
        static member toString(space : int, value : 'T, ?caseStrategy : CaseStrategy, ?extra: ExtraCoders, ?skipNullField: bool, [<Inject>] ?resolver : ITypeResolver<'T>) : string =
            let encoder = Auto.generateEncoder(?caseStrategy=caseStrategy, ?extra=extra, ?skipNullField=skipNullField, ?resolver=resolver)
            encoder value |> toString space
        #endif

        #if THOTH_JSON_NEWTONSOFT || (THOTH_JSON && !FABLE_COMPILER)
        static member toString(space : int, value : 'T, ?caseStrategy : CaseStrategy, ?extra: ExtraCoders, ?skipNullField: bool) : string =
            let encoder = Auto.generateEncoder(?caseStrategy=caseStrategy, ?extra=extra, ?skipNullField=skipNullField)
            encoder value |> toString space
        #endif
