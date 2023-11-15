module Tests.Main

open Fable.Core
open Fable.Mocha
open Fable.Core.Testing

// let [<Global>] describe (name: string) (f: unit->unit) = jsNative
// let [<Global>] it (msg: string) (f: unit->unit) = jsNative

// let run () =
//     let tests = [ Tests.Decoders.tests
//                   Tests.Encoders.tests
//                   Tests.ExtraCoders.tests
//                   Tests.BackAndForth.tests
//                 ] :> Util.Testing.Test seq

//     for (moduleName, moduleTests) in tests do
//         describe moduleName <| fun () ->
//             for (name, tests) in moduleTests do
//                 describe name <| fun _ ->
//                     for (msg, test) in tests do
//                         it msg test

// run()

open Util.Testing

let allTests =
    testList "All" [
        Decoders.tests
    ]

[<EntryPoint>]
let main args =
    Mocha.runTests allTests
