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
    open System.Text.RegularExpressions

    #if THOTH_JSON_FABLE
    open Fable.Core
    open Fable.Core.JsInterop
    #else
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
    let inline string (value : string) : JsonValue =
        #if THOTH_JSON_FABLE
        box value
        #endif

        #if THOTH_JSON_NEWTONSOFT
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
    let inline float (value : float) : JsonValue =
        #if THOTH_JSON_FABLE
        box value
        #endif

        #if THOTH_JSON_NEWTONSOFT
        JValue(value) :> JsonValue
        #endif

    let inline float32 (value : float32) : JsonValue =
        #if THOTH_JSON_FABLE
        box value
        #endif

        #if THOTH_JSON_NEWTONSOFT
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
    let inline bool (value : bool) : JsonValue =
        #if THOTH_JSON_FABLE
        box value
        #endif

        #if THOTH_JSON_NEWTONSOFT
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
    let inline array (values : JsonValue array) : JsonValue =
        #if THOTH_JSON_FABLE
        box values
        #endif

        #if THOTH_JSON_NEWTONSOFT
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
        #if THOTH_JSON_FABLE
        // Don't use List.toArray as it may create a typed array
        Helpers.arrayFrom values
        #endif

        #if THOTH_JSON_NEWTONSOFT
        JArray(values) :> JsonValue
        #endif

    let seq (values : JsonValue seq) : JsonValue =
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
        #if THOTH_JSON_FABLE
        value.ToString("O", CultureInfo.InvariantCulture) |> string
        #endif

        #if THOTH_JSON_NEWTONSOFT
        JValue(value.ToString("O", CultureInfo.InvariantCulture)) :> JsonValue
        #endif

    let datetimeOffset (value : System.DateTimeOffset) : JsonValue =
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
        #if THOTH_JSON_FABLE
        value.ToString() |> string
        #endif

        #if THOTH_JSON_NEWTONSOFT
        JValue(value.ToString()) :> JsonValue
        #endif

    let sbyte (value : sbyte) : JsonValue =
        #if THOTH_JSON_FABLE
        box (value.ToString(CultureInfo.InvariantCulture))
        #endif

        #if THOTH_JSON_NEWTONSOFT
        JValue(value.ToString(CultureInfo.InvariantCulture)) :> JsonValue
        #endif

    let byte (value : byte) : JsonValue =
        #if THOTH_JSON_FABLE
        box (value.ToString(CultureInfo.InvariantCulture))
        #endif

        #if THOTH_JSON_NEWTONSOFT
        JValue(value.ToString(CultureInfo.InvariantCulture)) :> JsonValue
        #endif

    let int16 (value : int16) : JsonValue =
        #if THOTH_JSON_FABLE
        box (value.ToString(CultureInfo.InvariantCulture))
        #endif

        #if THOTH_JSON_NEWTONSOFT
        JValue(value.ToString(CultureInfo.InvariantCulture)) :> JsonValue
        #endif

    let uint16 (value : uint16) : JsonValue =
        #if THOTH_JSON_FABLE
        box (value.ToString(CultureInfo.InvariantCulture))
        #endif

        #if THOTH_JSON_NEWTONSOFT
        JValue(value.ToString(CultureInfo.InvariantCulture)) :> JsonValue
        #endif

    let inline int (value : int) : JsonValue =
        #if THOTH_JSON_FABLE
        box value
        #endif

        #if THOTH_JSON_NEWTONSOFT
        JValue(value) :> JsonValue
        #endif

    let inline uint32 (value : uint32) : JsonValue =
        #if THOTH_JSON_FABLE
        box value
        #endif

        #if THOTH_JSON_NEWTONSOFT
        JValue(value) :> JsonValue
        #endif

    let int64 (value : int64) : JsonValue =
        #if THOTH_JSON_FABLE
        box (value.ToString(CultureInfo.InvariantCulture))
        #endif

        #if THOTH_JSON_NEWTONSOFT
        JValue(value.ToString(CultureInfo.InvariantCulture)) :> JsonValue
        #endif

    let uint64 (value : uint64) : JsonValue =
        #if THOTH_JSON_FABLE
        box (value.ToString(CultureInfo.InvariantCulture))
        #endif

        #if THOTH_JSON_NEWTONSOFT
        JValue(value.ToString(CultureInfo.InvariantCulture)) :> JsonValue
        #endif

    let unit () : JsonValue =
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

        #if THOTH_JSON_NEWTONSOFT
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

        #if THOTH_JSON_NEWTONSOFT
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

        #if THOTH_JSON_NEWTONSOFT
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

        #if THOTH_JSON_NEWTONSOFT
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

        #if THOTH_JSON_NEWTONSOFT
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

        #if THOTH_JSON_NEWTONSOFT
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

        #if THOTH_JSON_NEWTONSOFT
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
