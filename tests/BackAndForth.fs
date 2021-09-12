module Tests.BackAndForth

open Thoth.Json
open Util.Testing
open System
open Tests.Types

let tests : Test =

    testList "Thoth.Json - Back and Forth" [

        testList "StringEnum" [

            testCase "works with default rule" <| fun _ ->
                let expected = FirstPerson
                let json = Encode.Auto.toString(0, expected)
                let actual = Decode.Auto.unsafeFromString<Camera>(json)

                equal expected actual

            testCase "works with CaseRules.LowerFirst rule" <| fun _ ->
                let expected = VueJs
                let json = Encode.Auto.toString(0, expected)
                let actual = Decode.Auto.unsafeFromString<Framework>(json)

                equal expected actual

            testCase "works with CaseRules.None rule" <| fun _ ->
                let expected = Fsharp
                let json = Encode.Auto.toString(0, expected)
                let actual = Decode.Auto.unsafeFromString<Language>(json)

                equal expected actual

            testCase "works for CompiledName case" <| fun _ ->
                let expected = Csharp
                let json = Encode.Auto.toString(0, expected)
                let actual = Decode.Auto.unsafeFromString<Language>(json)

                equal expected actual
        ]
    ]
