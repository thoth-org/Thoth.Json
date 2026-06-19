module Thoth.Json.Tests.Codec.AutoCodec

#if !NETFRAMEWORK
open Fable.Core
#endif

open Thoth.Json.Tests.Testing
open Fable.Pyxpecto

open Thoth.Json.Core.Auto

type Baz =
    {
        Baz: bool
        Bic: int option
    }

type Foo =
    {
        Bar: string
        Baz: Baz
        Qux: int list
    }

module Foo =

    let codec = Codec.Auto.generateCodec (CamelCase)

let tests (runner: TestRunner<'DecoderJsonValue, 'EncoderJsonValue>) =
    testList
        "Auto"
        [
            test "Auto.generateCodec works for simple case 1" {
                let expected =
                    {
                        Bar = "abc"
                        Baz =
                            {
                                Baz = true
                                Bic = Some 123
                            }
                        Qux =
                            [
                                2
                                4
                                8
                            ]
                    }

                let actual = roundTrip runner Foo.codec expected

                equal actual expected
            }
        ]
