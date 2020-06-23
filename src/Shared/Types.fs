#if THOTH_JSON_FABLE
namespace Thoth.Json.Fable
#endif

#if THOTH_JSON_NEWTONSOFT
namespace Thoth.Json.Newtonsoft
#endif

type ErrorReason =
    | BadPrimitive of string * JsonValue
    | BadPrimitiveExtra of string * JsonValue * string
    | BadType of string * JsonValue
    | BadField of string * JsonValue
    | BadPath of string * JsonValue * string
    | TooSmallArray of string * JsonValue
    | FailMessage of string
    | BadOneOf of DecoderError list

and DecoderError = string * ErrorReason

type Decoder<'T> = JsonValue -> Result<'T, DecoderError>

type Encoder<'T> = 'T -> JsonValue

module DecoderError =

    let prependPath (path: string) (err: DecoderError): DecoderError =
        let (oldPath, reason) = err
        (path + oldPath, reason)

    let inline prependPathToResult<'T> (path: string) (res: Result<'T, DecoderError>) =
        res |> Result.mapError(prependPath path)

    let genericMsg (msg : string) (value : JsonValue) (newLine : bool) =
        try
            "Expecting "
                + msg
                + " but instead got:"
                + (if newLine then "\n" else " ")
                + (Helpers.anyToString value)
        with
            | _ ->
                "Expecting "
                + msg
                + " but decoder failed. Couldn't report given value due to circular structure."
                + (if newLine then "\n" else " ")

    let rec errorToString (path : string, error) =
        match error with
        | BadPrimitive (msg, value) ->
            genericMsg msg value false
        | BadType (msg, value) ->
            genericMsg msg value true
        | BadPrimitiveExtra (msg, value, reason) ->
            genericMsg msg value false + "\nReason: " + reason
        | BadField (msg, value) ->
            genericMsg msg value true
        | BadPath (msg, value, fieldName) ->
            genericMsg msg value true + ("\nNode `" + fieldName + "` is unknown.")
        | TooSmallArray (msg, value) ->
            "Expecting " + msg + ".\n" + (Helpers.anyToString value)
        | BadOneOf messages ->
            let messages =
                messages
                |> List.map (fun (subPath, subError) ->
                    "Error at: `" + (path + subPath) + "`\n" + errorToString (subPath, subError)
                )
            "The following errors were found:\n\n" + String.concat "\n\n" messages
        | FailMessage msg ->
            "The following `failure` occurred with the decoder: " + msg

    let errorToStringWithPath (path : string, error) =
        match error with
        | BadOneOf _ ->
            errorToString (path, error)
        | _ ->
            "Error at: `" + path + "`\n" + errorToString (path, error)
