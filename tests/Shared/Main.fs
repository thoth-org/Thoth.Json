module Tests.Main

#if THOTH_JSON_FABLE
open Thoth.Json.Fable
open Fable.Mocha
#endif

#if THOTH_JSON_NEWTONSOFT
open Thoth.Json.Newtonsoft
open Expecto
#endif

open Types

let quicktests =
    testList "QuickTest" [
        testList "Fake category" [
            testCase "QuickTest: #1" <| fun _ ->
                let expected = SingleCaseDUSimple "Maxime"
                let json =
                    """
[
    "SingleCaseDUSimple",
    "Maxime"
]
                    """.Trim()
                let actual = Decode.Auto.unsafeFromString(json)
                Expect.equal actual expected ""

        ]
    ]

[<EntryPoint>]
let main args =
    let allTests =
        testList "All" [
            Decoders.Manual.tests
            Decoders.Auto.tests
            Encoders.Manual.tests
            Encoders.Auto.tests
            // Uncomment this line if you want to use the quicktests useful
            // when prototyping or trying to reproduce an issue
            //quicktests
        ]

    #if FABLE_COMPILER
    Mocha.runTests allTests
    #else
    runTestsWithArgs defaultConfig args allTests
    #endif
