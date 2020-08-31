module Tests.Main

#if THOTH_JSON && FABLE_COMPILER
open Thoth.Json
open Fable.Mocha
#endif

#if THOTH_JSON && !FABLE_COMPILER
open Thoth.Json
open Expecto
#endif

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
                let expected =
                    {
                        FeedName = "Ars"
                        Content = "<div><figure class=\"intro-image intro-left\"><img src=\"https://cdn.arstechnica.net/wp-content/uploads/2019/05/qualcomm-enforcer-800x450.jpg\"></figure><div>"
                    }

                let articleJson =
                    """
                {
                  "FeedName": "Ars",
                  "Content": "<div><figure class=\"intro-image intro-left\"><img src=\"https://cdn.arstechnica.net/wp-content/uploads/2019/05/qualcomm-enforcer-800x450.jpg\"></figure><div>"
                }
                    """
                let actual : TestStringWithHTML = Decode.Auto.unsafeFromString(articleJson)
                Expect.equal actual expected ""
                ()
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
//            // Uncomment this line if you want to use the quicktests useful
//            // when prototyping or trying to reproduce an issue
//            quicktests
        ]

    #if FABLE_COMPILER
    Mocha.runTests allTests
    #else
    runTestsWithArgs defaultConfig args allTests
    #endif
