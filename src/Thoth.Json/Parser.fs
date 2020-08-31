module Thoth.Json.Parser

open System
open System.Text
open System.Globalization


// This code has been heavily inspired by https://github.com/fsharp/FSharp.Data/blob/master/src/Json/JsonValue.fs
// I want to create a parser library but still lack the time and energy to do it

module internal UnicodeHelper =

    // used http://en.wikipedia.org/wiki/UTF-16#Code_points_U.2B010000_to_U.2B10FFFF as a guide below
    let getUnicodeSurrogatePair num =
        // only code points U+010000 to U+10FFFF supported
        // for conversion to UTF16 surrogate pair
        let codePoint = num - 0x010000u
        let HIGH_TEN_BIT_MASK = 0xFFC00u                     // 1111|1111|1100|0000|0000
        let LOW_TEN_BIT_MASK = 0x003FFu                      // 0000|0000|0011|1111|1111
        let leadSurrogate = (codePoint &&& HIGH_TEN_BIT_MASK >>> 10) + 0xD800u
        let trailSurrogate = (codePoint &&& LOW_TEN_BIT_MASK) + 0xDC00u
        char leadSurrogate, char trailSurrogate

[<RequireQualifiedAccess>]
type JsonSaveOptions =
    // Indent the Json
    | Format = 0
    // Don't format the Json and print it in one line in a compact way
    | DisableFormatting = 1

type StringBuilder with
    member this.Write(text : string) =
        this.Append(text) |> ignore

[<RequireQualifiedAccess>]
type Json =
    | Number of float
    | String of string
    | Bool of bool
    | Null
    | Array of Json array
    | Object of Map<string, Json>
    // Not a real JSON type
    // It is used to have a concrete type to represent when a value is missing
    // For example, when trying to access property "name" in object { "age": 25 }
    | Undefined

    member this.Write(builder : StringBuilder, saveOptions : JsonSaveOptions, baseIndentation : int) =
        let newLine =
            if saveOptions = JsonSaveOptions.Format then
                fun indentation plus ->
                    builder.AppendLine("") |> ignore
                    System.String(' ', indentation + plus)
                    |> builder.Write
                    |> ignore
            else
                fun _ _ -> ()

        let propSep =
            if saveOptions = JsonSaveOptions.Format then
                "\": "
            else
                "\":"

        let rec serialize indentation = function
            | Json.Null ->
                builder.Write "null"

            | Json.Bool b ->
                builder.Write(if b then "true" else "false")

            | Json.Number number ->
                builder.Write(string number)

            | Json.Number v when Double.IsInfinity v || Double.IsNaN v ->
                builder.Write "null"

            | Json.Number number ->
                builder.Write(string number)

            | Json.String s ->
                builder.Write "\""
                Json.JsonStringEncodeTo builder s
                builder.Write "\""

            | Json.Object properties ->
                builder.Write "{"
                let properties = Map.toArray properties
                for i = 0 to properties.Length - 1 do
                    let k,v = properties.[i]
                    if i > 0 then builder.Write ","
                    newLine indentation baseIndentation
                    builder.Write "\""
                    Json.JsonStringEncodeTo builder k
                    builder.Write propSep
                    serialize (indentation + baseIndentation) v
                if properties.Length <> 0 then
                    newLine indentation 0
                builder.Write "}"

            | Json.Undefined ->
                ()

            | Json.Array elements ->
                builder.Write "["
                for i = 0 to elements.Length - 1 do
                    if i > 0 then builder.Write ","
                    newLine indentation baseIndentation
                    serialize (indentation + baseIndentation) elements.[i]
                if elements.Length > 0 then
                    newLine indentation 0
                builder.Write "]"

        serialize 0 this

    // Encode characters that are not valid in JS string. The implementation is based
    // on https://github.com/mono/mono/blob/master/mcs/class/System.Web/System.Web/HttpUtility.cs
    static member internal JsonStringEncodeTo (builder : StringBuilder) (value:string) =
        if not (String.IsNullOrEmpty value) then
            for i = 0 to value.Length - 1 do
                let c = value.[i]
                let ci = int c

                if ci >= 0 && ci <= 7 || ci = 11 || ci >= 14 && ci <= 31 then
                    System.String.Format("\\u{0:x4}", ci)
                    |> builder.Write
                else
                    match c with
                    | '\b' ->
                        builder.Write "\\b"

                    | '\t' ->
                        builder.Write "\\t"

                    | '\n' ->
                        builder.Write "\\n"

                    | '\f' ->
                        builder.Write "\\f"

                    | '\r' ->
                        builder.Write "\\r"

                    | '"'  ->
                        builder.Write "\\\""

                    | '\\' ->
                        builder.Write "\\\\"

                    | _    ->
                        builder.Write (string c)

exception JsonParserError of string

type private JsonParser(jsonText : string) =

    let mutable position = 0
    let text = jsonText

    let buffer = StringBuilder() // Pre-allocate buffers for strings

    let skipWhiteSpace () =
        while position < text.Length && Char.IsWhiteSpace text.[position] do
            position <- position + 1

    let isNumChar (c : char) =
        Char.IsDigit c || c = '.' || c = 'e' || c = 'E' || c = '+' || c = '-'

    let throw () =
        let msg =
            "Invalid JSON starting at character "
                + string position
                + ", snippet =\n-----\n"
                + (jsonText.[(max 0 (position-10))..(min (jsonText.Length-1) (position+10))])
                + "\n-----\njson =\n-----\n"
                + (if jsonText.Length > 1000 then jsonText.Substring(0, 1000) else jsonText)
                + "\n-----"

        raise (JsonParserError msg)

    let ensure (condition : bool) =
        if not condition then
            throw()

    // Recursive descent parser for JSON that uses global mutable index
    let rec parseValue() =
        skipWhiteSpace()
        ensure(position < text.Length)
        match text.[position] with
        | '"' ->
            Json.String(parseString())

        | '-' ->
            parseNumber()

        | c when Char.IsDigit(c) ->
            parseNumber()

        | '{' ->
            parseObject()

        | '[' ->
            parseArray()
        // If a value start with 't' we know that it can only be the boolean value true
        | 't' ->
            parseLiteral("true", Json.Bool true)
        // If a value start with 'f' we know that it can only be the boolean value false
        | 'f' ->
            parseLiteral("false", Json.Bool false)
        // If a value start with 'n' we know that it can only be 'null'
        | 'n' ->
            parseLiteral("null", Json.Null)

        | _ -> throw()

    and parseString () =
        // Make sure this is a valid start for a string
        ensure(position < text.Length && text.[position] = '"')
        position <- position + 1

        while position < text.Length && text.[position] <> '"' do
            if text.[position] = '\\' then
                ensure(position + 1 < text.Length)

                match text.[position + 1] with
                | 'b' ->
                    buffer.Append('\b') |> ignore

                | 'f' ->
                    buffer.Append('\f') |> ignore

                | 'n' ->
                    buffer.Append('\n') |> ignore

                | 't' ->
                    buffer.Append('\t') |> ignore

                | 'r' ->
                    buffer.Append('\r') |> ignore

                | '\\' ->
                    buffer.Append('\\') |> ignore

                | '/' ->
                    buffer.Append('/') |> ignore

                | '"' ->
                    buffer.Append('"') |> ignore

                | 'u' ->
                    ensure(position + 5 < text.Length)
                    let hexDigit d =
                        if d >= '0' && d <= '9' then int32 d - int32 '0'
                        elif d >= 'a' && d <= 'f' then int32 d - int32 'a' + 10
                        elif d >= 'A' && d <= 'F' then int32 d - int32 'A' + 10
                        else failwith "hexDigit"

                    let unicodeChar (s:string) =
                        if s.Length <> 4 then failwith "unicodeChar";
                        char (hexDigit s.[0] * 4096 + hexDigit s.[1] * 256 + hexDigit s.[2] * 16 + hexDigit s.[3])

                    let ch = unicodeChar (text.Substring(position + 2, 4))

                    buffer.Append(ch) |> ignore
                    position <- position + 4  // the \ and u will also be skipped past further below

                | 'U' ->
                    ensure(position + 9 < text.Length)
                    let unicodeChar (s:string) =
                        if s.Length <> 8 then failwith "unicodeChar";
                        if s.[0..1] <> "00" then failwith "unicodeChar";
                        UnicodeHelper.getUnicodeSurrogatePair <| UInt32.Parse(text, NumberStyles.HexNumber)
                    let lead, trail = unicodeChar (text.Substring(position + 2, 8))
                    buffer.Append(lead) |> ignore
                    buffer.Append(trail) |> ignore
                    position <- position + 8  // the \ and u will also be skipped past further below

                | _ -> throw()
                position <- position + 2  // skip past \ and next char
            else
                buffer.Append(text.[position]) |> ignore
                position <- position + 1

        ensure(position < text.Length && text.[position] = '"')
        position <- position + 1
        let str = buffer.ToString()
        buffer.Clear() |> ignore
        str

    and parseNumber () =
        let start = position
        while position < text.Length && (isNumChar text.[position]) do
            position <- position + 1

        let len = position - start
        let subText = text.Substring(start, len)

        match Double.TryParse subText with
        | true, value ->
            Json.Number value

        | _ ->
            throw()

    and parsePair () =
        let key = parseString()
        skipWhiteSpace()
        ensure(position < text.Length && text.[position] = ':')
        position <- position + 1
        skipWhiteSpace()
        key, parseValue()

    and parseObject () =
        ensure(position < text.Length && text.[position] = '{')
        position <- position + 1
        skipWhiteSpace()
        let pairs = ResizeArray<_>()

        if position < text.Length && text.[position] = '"' then
            pairs.Add(parsePair())
            skipWhiteSpace()

            while position < text.Length && text.[position] = ',' do
                position <- position + 1
                skipWhiteSpace()
                pairs.Add(parsePair())
                skipWhiteSpace()

        ensure(position < text.Length && text.[position] = '}')
        position <- position + 1

        pairs.ToArray()
        |> Map.ofArray
        |> Json.Object

    and parseArray () =
        ensure(position < text.Length && text.[position] = '[')
        position <- position + 1
        skipWhiteSpace()
        let values = ResizeArray<_>()

        if position < text.Length && text.[position] <> ']' then
            values.Add(parseValue())
            skipWhiteSpace()

            while position < text.Length && text.[position] = ',' do
                position <- position + 1
                skipWhiteSpace()
                values.Add(parseValue())
                skipWhiteSpace()

        ensure(position < text.Length && text.[position] = ']')
        position <- position + 1

        Json.Array(values.ToArray())

    and parseLiteral(expected : string, result : Json) =
        // Check that the string is long enough
        ensure(position + expected.Length <= text.Length)
        // Check that each char as the expected value
        for charRank in 0 .. expected.Length - 1 do
            ensure(text.[position + charRank] = expected.[charRank])

        position <- position + expected.Length
        result

    member this.Parse() =
        let value = parseValue()
        skipWhiteSpace()
        // Check that we consume all the JSON string
        // If not then the JSON is not valid
        if position <> text.Length then
            throw()
        value

let parse (jsonText : string) =
    JsonParser(jsonText).Parse()
