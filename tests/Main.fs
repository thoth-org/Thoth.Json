module Tests.Main

#if FABLE_COMPILER
open Thoth.Json
open Fable.Mocha
#endif

#if !FABLE_COMPILER
open Thoth.Json
open Expecto
#endif

open Types

type Temp = { Name : string; Age : int }

type SeveralArgumentDus2 = SeveralArgs2 of int * Temp

type SingleNoArguments = SingleNoArguments

type SeveralArgumentDus3 =
    | SeveralArgs3 of int * Temp
    | CustomTest1 of string

//let x = CustomTest1
//
//let y = SeveralArgs3
//
//let inline testDuEncoder<'DU, 'Args> (ctor : 'Args -> 'DU) (args : 'Args) : 'DU =
//    ctor args
//
//let v1 = testDuEncoder SeveralArgs3 (1, { Name = "maxime"; Age = 11 })
//
//let inline testDuEncoder2<'DU, 'Args> (dctor : 'DU -> 'Args option) (v : 'DU) : 'Args option =
//    dctor v
//
//let dctorTest =
//    testDuEncoder2
//        (function SeveralArgs3 (args1, args2) -> Some (args1, args2) | _ -> None  )
//        (SeveralArgs3 (1, { Name = "maxime"; Age = 11 }))

let quicktests =
    testList "QuickTest" [
        testList "Fake category" [
            testCase "QuickTest: #1" <| fun _ ->
                let value = SeveralArgs (10, {| Name = "maxime"; Age = 28 |})
                let json = Encode.Auto.toString(4, value)
                printfn "%A" json

                let value = SeveralArgs2 (10, { Name = "maxime"; Age = 28 })
                let json = Encode.Auto.toString(4, value)
                printfn "%A" json

                let value = Foo 11
                let json = Encode.Auto.toString(4, value)
                printfn "%A" json

                let value = SingleNoArguments
                let json = Encode.Auto.toString(4, value)
                printfn "%A" json

                let value = SeveralArgs3 (10, { Name = "maxime"; Age = 28 })
                let json = Encode.Auto.toString(4, value)
                printfn "%A" json

//                let value = FakeOption.Some 10
//                let json = Encode.Auto.toString(4, value)
//                printfn "%A" json
//
//                let value = FakeOption.None
//                let json = Encode.Auto.toString(4, value)
//                printfn "%A" json

//                let value = CustomTest1 "maxi"
//                let json = Encode.Auto.toString(4, value)
//                printfn "%A" json

                                // Check Some case of primitive type
                let realOptionSome : Option<_> = Some "maxime"
                let fakeOptionSome : Fake.FakeOption<_> = Fake.FakeOption.Some "maxime"

                let realOptionSomeJson = Encode.Auto.toString(0, realOptionSome)
                let fakeOptionSomeJson = Encode.Auto.toString(0, fakeOptionSome)

                Expect.equal realOptionSomeJson fakeOptionSomeJson ""

                // Check None case
                let realOptionNone : Option<_> = None
                let fakeOptionNone : Fake.FakeOption<_> = Fake.FakeOption.None

                let realOptionNoneJson = Encode.Auto.toString(0, realOptionNone, skipNullField = false)
                let fakeOptionNoneJson = Encode.Auto.toString(0, fakeOptionNone, skipNullField = false)

                Expect.equal realOptionNoneJson fakeOptionNoneJson ""

                // Check Some case of complex type
                let realOptionSomeComplex : Option<_> = Some {| Firstname = "maxime"; Age = 28 |}
                let fakeOptionSomeComplex : Fake.FakeOption<_> = Fake.FakeOption.Some {| Firstname = "maxime"; Age = 28 |}

                let realOptionSomeComplexJson = Encode.Auto.toString(0, realOptionSomeComplex)
                let fakeOptionSomeComplexJson = Encode.Auto.toString(0, fakeOptionSomeComplex)

                Expect.equal realOptionSomeComplexJson fakeOptionSomeComplexJson ""
                ()
        ]
    ]

[<EntryPoint>]
let main args =
    let allTests =
        testList "All" [
//            Decoders.Manual.tests
//            Decoders.Auto.tests
//            Encoders.Manual.tests
//            Encoders.Auto.tests
//            // Uncomment this line if you want to use the quicktests useful
//            // when prototyping or trying to reproduce an issue
            quicktests
        ]

    #if FABLE_COMPILER
    Mocha.runTests allTests
    #else
    runTestsWithArgs defaultConfig args allTests
    #endif
