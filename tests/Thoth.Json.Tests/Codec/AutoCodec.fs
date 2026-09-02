module Thoth.Json.Tests.Codec.AutoCodec

#if !NETFRAMEWORK
open Fable.Core
#endif

open Thoth.Json.Tests.Testing
open Fable.Pyxpecto

open Thoth.Json.Core
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

type Nested =
    {
        Data: int option option
    }

module Foo =

    let codec = Codec.Auto.generateCodec (CamelCase)

module Nested =

    let codec: Codec<Nested> = Codec.Auto.generateCodec (losslessOption = true)

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

            test
                "Auto.generateCodec round-trips nested options when losslessOption is set" {
                for expected in
                    [
                        {
                            Data = Some None
                        }
                        {
                            Data = Some(Some 123)
                        }
                        {
                            Data = None
                        }
                    ] do
                    let actual = roundTrip runner Nested.codec expected

                    equal actual expected
            }
        ]
