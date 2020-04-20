module Tests.ExtraCoders

open Thoth.Json
open Util.Testing
#if !NETFRAMEWORK
open Fable.Core
#endif

type Data =
    { Id : int
      Text : string }

type TestRecord =
    { Foo: string
      Bar: string
      Baz: Data }

let camelCaseCoder = Extra.withCustom
                        (fun (c:Data) ->
                            Encode.object
                                [ "id", Encode.int c.Id
                                  "text", Encode.string c.Text])
                        (Decode.object (fun get ->
                                { Id = get.Required.Field "id" Decode.int
                                  Text = get.Required.Field "text" Decode.string }))
                        Extra.empty

let pascalCaseCoder = Extra.withCustom
                        (fun (c:Data) ->
                            Encode.object
                                [ "Id", Encode.int c.Id
                                  "Text", Encode.string c.Text])
                        (Decode.object (fun get ->
                                { Id = get.Required.Field "Id" Decode.int
                                  Text = get.Required.Field "Text" Decode.string }))
                        Extra.empty
#if FABLE_COMPILER
type CachedCoder =
    static member internal encode<'Data>(data:'Data, ?caseStrategy:CaseStrategy, ?extra:ExtraCoders, [<Inject>]?dataResolver: ITypeResolver<'Data>) =
        let encode = Encode.Auto.generateEncoderCached<'Data>(?caseStrategy = caseStrategy, ?extra = extra, ?resolver = dataResolver)
        encode data |> Encode.toString 0

    static member internal decode<'Response>(value:string, ?caseStrategy:CaseStrategy, ?extra:ExtraCoders, [<Inject>]?responseResolver: ITypeResolver<'Response>) =
        let decoder = Decode.Auto.generateDecoderCached<'Response>(?caseStrategy = caseStrategy, ?extra = extra, ?resolver = responseResolver)
        Decode.unsafeFromString decoder value
#else
type CachedCoder =
    static member internal encode<'Data>(data:'Data, ?caseStrategy:CaseStrategy, ?extra:ExtraCoders) =
        let encode = Encode.Auto.generateEncoderCached<'Data>(?caseStrategy = caseStrategy, ?extra = extra)
        encode data |> Encode.toString 0

    static member internal decode<'Response>(value:string, ?caseStrategy:CaseStrategy, ?extra:ExtraCoders) =
        let decoder = Decode.Auto.generateDecoderCached<'Response>(?caseStrategy = caseStrategy, ?extra = extra)
        Decode.unsafeFromString decoder value
#endif

let tests : Test =
    testList "Thoth.Json.ExtraCoder" [
        testList "Basic Tests (uncached)" [
            testCase "coder in camelCase works" <| fun _ ->
                let data = {Id = 1; Text ="Text"}
                let expected = data

                let actual =
                    Encode.Auto.toString (0, data, extra = camelCaseCoder)
                    |> fun json ->
                    Decode.Auto.unsafeFromString (json,extra = camelCaseCoder)

                equal expected actual

            testCase "auto coder are working in camelCase" <| fun _ ->
                let data = {Id = 1; Text ="Text"}
                let expected = data

                let actual =
                    Encode.Auto.toString (0, data, caseStrategy = CamelCase)
                    |> fun json ->
                    Decode.Auto.unsafeFromString (json, caseStrategy = CamelCase)
                equal expected actual

            testCase "coder in PasalCase works'" <| fun _ ->
                let data = {Id = 1; Text ="Text"}
                let expected = data

                let actual =
                    Encode.Auto.toString (0, data, extra = pascalCaseCoder)
                    |> fun json ->
                    Decode.Auto.unsafeFromString (json, extra = pascalCaseCoder)
                equal expected actual

            testCase "auto coder are working in PascalCase" <| fun _ ->
                let data = {Id = 1; Text ="Text"}
                let expected = data

                let actual =
                    Encode.Auto.toString (0, data, caseStrategy = PascalCase)
                    |> fun json ->
                    Decode.Auto.unsafeFromString (json, caseStrategy = PascalCase)

                equal expected actual
        ]
        testList "Cached Coders" [
            testCase "coder in camelCase works" <| fun _ ->
                let data = {Id = 1; Text ="Text"}
                let expected = data
                let actual =
                    CachedCoder.encode ( data, extra = camelCaseCoder)
                    |> fun json ->
                    CachedCoder.decode (json, extra = camelCaseCoder)
                equal expected actual

            testCase "auto coder are working in camelCase" <| fun _ ->
                let data = {Id = 1; Text ="Text"}
                let expected = data
                let actual =
                    CachedCoder.encode ( data, caseStrategy = CamelCase)
                    |> fun json ->
                    CachedCoder.decode (json, caseStrategy = CamelCase)
                equal expected actual

            testCase "coder in PascalCase works" <| fun _ ->
                let data = {Id = 1; Text ="Text"}
                let expected = data
                let actual =
                    CachedCoder.encode ( data, extra = pascalCaseCoder)
                    |> fun json ->
                    CachedCoder.decode (json, extra = pascalCaseCoder)
                equal expected actual

            testCase "auto coder are working in PascalCase" <| fun _ ->
                let data = {Id = 1; Text ="Text"}
                let expected = data
                let actual =
                    CachedCoder.encode ( data, caseStrategy = PascalCase)
                    |> fun json ->
                    CachedCoder.decode (json, caseStrategy = PascalCase)
                equal expected actual
        ]
        testList "Cached encoding/ normal decoding" [
            testCase "coder in camelCase works" <| fun _ ->
                let data = {Id = 1; Text ="Text"}
                let expected = data
                let actual =
                    CachedCoder.encode ( data, extra = camelCaseCoder)
                    |> fun json ->
                    Decode.Auto.unsafeFromString (json, extra = camelCaseCoder)
                equal expected actual

            testCase "auto coder are working in camelCase" <| fun _ ->
                let data = {Id = 1; Text ="Text"}
                let expected = data
                let actual =
                    CachedCoder.encode ( data, caseStrategy = CamelCase)
                    |> fun json ->
                    Decode.Auto.unsafeFromString (json, caseStrategy = CamelCase)
                equal expected actual

            testCase "coder in PascalCase works" <| fun _ ->
                let data = {Id = 1; Text ="Text"}
                let expected = data
                let actual =
                    CachedCoder.encode ( data, extra = pascalCaseCoder)
                    |> fun json ->
                    Decode.Auto.unsafeFromString (json, extra = pascalCaseCoder)
                equal expected actual

            testCase "auto coder are working in PascalCase" <| fun _ ->
                let data = {Id = 1; Text ="Text"}
                let expected = data
                let actual =
                    CachedCoder.encode ( data,  caseStrategy = PascalCase)
                    |> fun json ->
                    Decode.Auto.unsafeFromString (json, caseStrategy = PascalCase)
                equal expected actual
        ]
        testList "Normal encoding/ cached decoding" [
            testCase "coder in camelCase works" <| fun _ ->
                let data = {Id = 1; Text ="Text"}
                let expected = data
                let actual =
                    Encode.Auto.toString (0, data, extra = camelCaseCoder)
                    |> fun json ->
                    CachedCoder.decode (json, extra = camelCaseCoder)
                equal expected actual

            testCase "auto coder are working in camelCase" <| fun _ ->
                let data = {Id = 1; Text ="Text"}
                let expected = data
                let actual =
                    Encode.Auto.toString (0, data, caseStrategy = CamelCase)
                    |> fun json ->
                    CachedCoder.decode (json, caseStrategy = CamelCase)
                equal expected actual

            testCase "coder in PascalCase works" <| fun _ ->
                let data = {Id = 1; Text ="Text"}
                let expected = data
                let actual =
                    Encode.Auto.toString (0, data, extra = pascalCaseCoder)
                    |> fun json ->
                    CachedCoder.decode (json, extra = pascalCaseCoder)
                equal expected actual

            testCase "auto coder are working in PascalCase" <| fun _ ->
                let data = {Id = 1; Text ="Text"}
                let expected = data
                let actual =
                    Encode.Auto.toString (0, data, caseStrategy = PascalCase)
                    |> fun json ->
                    CachedCoder.decode (json, caseStrategy = PascalCase)
                equal expected actual

            testCase "custom field encoders work" <| fun _ ->
                let data = {Id = 1; Text ="Text"}
                let wrapper = { Foo = "foo"
                                Bar = "bar"
                                Baz = data }

                let fieldEncoder (o: obj) =
                    match o with
                    | :? string as s when s = "bar" -> IgnoreField
                    | _ -> UseAutoEncoder

                let extra =
                    Extra.empty
                    |> Extra.withCustomFieldEncoder<TestRecord> "Foo" fieldEncoder
                    |> Extra.withCustomFieldEncoder<TestRecord> "Bar" fieldEncoder
                    |> Extra.withCustomFieldEncoder<TestRecord> "Baz" (fun _ -> Encode.int 5 |> UseJsonValue)

                Encode.Auto.toString (0, wrapper, CamelCase, extra)
                |> equal """{"foo":"foo","baz":5}"""

            testCase "custom field decoders work" <| fun _ ->
                let json = """{"foo":"oof","baz":5}"""
                let expected = { Foo = "foo"
                                 Bar = "bar"
                                 Baz = {Id = 1; Text ="Text"} }

                let fieldDecoder path = function
                    | Some v ->
                        match Decode.string path v with
                        | Ok s -> UseOk(s.ToCharArray() |> Seq.rev |> Seq.toArray |> System.String)
                        | Error e -> UseError e
                    | None -> UseOk "bar"

                let extra =
                    Extra.empty
                    |> Extra.withCustomFieldDecoder<TestRecord> "Foo" fieldDecoder
                    |> Extra.withCustomFieldDecoder<TestRecord> "Bar" fieldDecoder
                    |> Extra.withCustomFieldDecoder<TestRecord> "Baz" (fun _ _ -> UseOk {Id = 1; Text ="Text"})

                Decode.Auto.fromString (json, CamelCase, extra)
                |> equal (Ok expected)
        ]
    ]
