module Tests.Main

open Fable.Core

let [<Global>] describe (name: string) (f: unit->unit) = jsNative
let [<Global>] it (msg: string) (f: unit->unit) = jsNative

open Thoth.Json
open Util.Testing
open Fable.Core.Experimental

let quicktests : Test =
    testList "QuickTest" [
        testList "Fake category" [
            testCase "QuickTest: #1" <| fun _ ->
                ()
        ]
    ]

let run () =
    let tests =
        [
            Tests.Decoders.tests
            Tests.Encoders.tests
            Tests.ExtraCoders.tests
            // Uncomment this line if you want to use the quicktests useful
            // when prototyping or trying to reproduce an issue
            // quicktests
        ] :> Util.Testing.Test seq

    for (moduleName, moduleTests) in tests do
        describe moduleName <| fun () ->
            for (name, tests) in moduleTests do
                describe name <| fun _ ->
                    for (msg, test) in tests do
                        it msg test

run()
