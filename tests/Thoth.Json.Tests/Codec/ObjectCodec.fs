module Thoth.Json.Tests.Codec.ObjectCodec

open System

#if !NETFRAMEWORK
open Fable.Core
#endif

open Thoth.Json.Tests.Testing
open Fable.Pyxpecto

open Thoth.Json.Core

type FooBar =
    {
        Foo: int
        Bar: string
    }

type Qux =
    {
        Qux: int
        FooBar: FooBar
    }

type Large =
    {
        Foo: int
        Bar: string
        Qux: bool
        Baz: Guid
    }

type Baz =
    {
        Baz: string option
    }

module Codec =

    let fooBar: Codec<FooBar> =
        objectCodec {
            let! foo = Codec.field "foo" (fun x -> x.Foo) Codec.int
            and! bar = Codec.field "bar" (fun x -> x.Bar) Codec.string

            return
                {
                    Foo = foo
                    Bar = bar
                }
        }

    let qux: Codec<Qux> =
        objectCodec {
            let! qux = Codec.field "qux" (fun x -> x.Qux) Codec.int
            and! fooBar = Codec.field "fooBar" (fun x -> x.FooBar) fooBar

            return
                {
                    Qux = qux
                    FooBar = fooBar
                }
        }

    let large: Codec<Large> =
        objectCodec {
            let! foo = Codec.field "foo" (fun x -> x.Foo) Codec.int
            and! bar = Codec.field "bar" (fun x -> x.Bar) Codec.string
            and! qux = Codec.field "qux" (fun x -> x.Qux) Codec.bool
            and! baz = Codec.field "baz" (fun x -> x.Baz) Codec.guid

            return
                {
                    Foo = foo
                    Bar = bar
                    Qux = qux
                    Baz = baz
                }
        }

    let baz: Codec<Baz> =
        objectCodec {
            let! baz = Codec.optional "baz" (fun x -> x.Baz) Codec.string

            return
                {
                    Baz = baz
                }
        }

let tests (runner: TestRunner<'DecoderJsonValue, 'EncoderJsonValue>) =
    testList
        "ObjectCodec"
        [
            test "objectCodec works for simple case 1" {
                let expected =
                    {
                        Foo = 123
                        Bar = "abc"
                    }

                let actual = roundTrip runner Codec.fooBar expected

                equal actual expected
            }

            test "objectCodec works for simple case 2" {
                let expected =
                    {
                        Qux = 101
                        FooBar =
                            {
                                Foo = 456
                                Bar = "def"
                            }
                    }

                let actual = roundTrip runner Codec.qux expected

                equal actual expected
            }

            test "objectCodec works for simple case 3" {
                let expected =
                    {
                        Foo = 101
                        Bar = "def"
                        Qux = true
                        Baz = Guid.Parse "3739a1b7-ee2f-4cab-9597-94fbf7a3766e"
                    }

                let actual = roundTrip runner Codec.large expected

                equal actual expected
            }

            test "objectCodec optional field works" {
                let withValue =
                    {
                        Baz = Some "abc"
                    }

                let withoutValue =
                    {
                        Baz = None
                    }

                let actualWithValue = roundTrip runner Codec.baz withValue

                let actualWithoutValue = roundTrip runner Codec.baz withoutValue

                equal actualWithValue withValue

                equal actualWithoutValue withoutValue
            }
        ]
