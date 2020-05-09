module Tests.Builder

open Thoth.Json
open Thoth.Json.Builder
open Util.Testing

let tests: Test =
    testList "Thoth.Json.Builder" [
        testList "Basic Tests" [
            testCase "return works" <| fun _ ->
                let expected = Ok 42

                let decodeFortyTwo =
                    decoder { return 42 }

                let actual = Decode.fromString decodeFortyTwo "{}"

                equal expected actual


            testCase "return! works" <| fun _ ->
                let expected = Ok 42

                let decodeInt =
                    decoder { return! Decode.int }

                let actual = Decode.fromString decodeInt "42"

                equal expected actual


            testCase "let! works" <| fun _ ->
                let expected = Ok 42

                let decodeIntPlusOne =
                    decoder {
                        let! value = Decode.int
                        return value + 1 }

                let actual = Decode.fromString decodeIntPlusOne "41"

                equal expected actual


            testCase "zero works" <| fun _ ->
                let expected = Ok ()

                let validateFortyTwo =
                    decoder {
                        let! value = Decode.int
                        if value <> 42 then return! Decode.fail "Not 42!"
                    }

                let actual = Decode.fromString validateFortyTwo "42"

                equal expected actual


            testCase "multiple let! statements work" <| fun _ ->
                let expected = Ok (42, "EUR")

                let decodeMoney =
                    decoder {
                        let! value = Decode.int |> Decode.field "value"
                        let! currency = Decode.string |> Decode.field "currency"
                        return value, currency
                    }

                let actual = Decode.fromString decodeMoney """{ "value": 42, "currency": "EUR" }"""

                equal expected actual


            testCase "returns an error on invalid json" <| fun _ ->
                let expected =
                    Error(
                        """
Error at: `$.value`
Expecting an int but instead got: "forty two"
                        """.Trim())

                let decodeMoney =
                    decoder {
                        let! value = Decode.int |> Decode.field "value"
                        let! currency = Decode.string |> Decode.field "currency"
                        return value, currency
                    }

                let actual = Decode.fromString decodeMoney """{ "value": "forty two", "currency": "EUR" }"""

                equal expected actual
        ]
    ]
