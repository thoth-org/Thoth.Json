module Tests.Main

// #if FABLE_COMPILER
open Fable.Mocha
// #endif

// #if THOTH_JSON_NEWTONSOFT
// open Thoth.Json.Newtonsoft
// open Expecto
// #endif

let quicktests =
    testList "QuickTest" [
        testList "Fake category" [
            testCase "QuickTest: #1" <| fun _ ->
                printfn "add your code here"
        ]
    ]

[<EntryPoint>]
let main args =
    let allTests =
        testList "All" [
            Tests.Parser.tests
            // Uncomment this line if you want to use the quicktests useful
            // when prototyping or trying to reproduce an issue
            //quicktests
        ]

    #if FABLE_COMPILER
    Mocha.runTests allTests
    #else
    runTestsWithArgs defaultConfig args allTests
    #endif
