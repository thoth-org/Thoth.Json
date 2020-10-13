module Tests.Main

#if FABLE_COMPILER
open Thoth.Json
open Fable.Mocha
#endif

#if !FABLE_COMPILER
open Thoth.Json
open Expecto
#endif

let quicktests =
    testList "QuickTest" [
        testList "Fake category" [
            testCase "QuickTest: #1" <| fun _ ->
                let value = Some 5
                let json = Encode.Auto.toString(4, value)
                let res = Decode.Auto.unsafeFromString<int option>(json)
                Expect.equal res value ""
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
            quicktests
        ]

    #if FABLE_COMPILER
    Mocha.runTests allTests
    #else
    runTestsWithArgs defaultConfig args allTests
    #endif
