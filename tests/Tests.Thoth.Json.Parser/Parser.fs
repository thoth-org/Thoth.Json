module Tests.Parser

open Thoth.Json.Parser

open Fable.Mocha

// String with all the special char to espace in JSON
// "Text with special character /\"\'\b\f\t\r\n."
// Text with special character /"'
// .
// After escaping.
// Text with special character \/\"'\b\f\t\r\n.
// See: https://www.tutorialspoint.com/json_simple/json_simple_escape_characters.htm

let tests =
    testList "Thoth.Json.Parser" [
        testCase "Test #1" <| fun _ ->
            let json = "null"
            let expected = Json.Null
            let actual =
                parse(json)

            Expect.equal actual expected ""

        testCase "Test #2" <| fun _ ->
            let json = "\"maxime\""
            let expected = Json.String "maxime"
            let actual =
                parse(json)

            Expect.equal actual expected ""

    ]
