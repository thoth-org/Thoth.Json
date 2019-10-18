module Tests.ExtraCoders

open Thoth.Json
open Util.Testing
open System
open Tests.Types
open Fable.Core

type Data =
    { Id : int
      Text : string }

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
type CachedCoder =
    static member internal encode<'Data>(data:'Data, ?isCamelCase:bool, ?extra:ExtraCoders, [<Inject>]?dataResolver: ITypeResolver<'Data>) =
        let encode = Encode.Auto.generateEncoderCached<'Data>(?isCamelCase = isCamelCase, ?extra = extra, ?resolver = dataResolver)
        encode data |> Encode.toString 0 
        
    static member internal decode<'Response>(value:string, ?isCamelCase:bool, ?extra:ExtraCoders, [<Inject>]?responseResolver: ITypeResolver<'Response>) =
        let decoder = Decode.Auto.generateDecoderCached<'Response>(?isCamelCase = isCamelCase, ?extra = extra, ?resolver = responseResolver)
        Decode.unsafeFromString decoder value   

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
                    Encode.Auto.toString (0, data, isCamelCase = true)
                    |> fun json -> 
                    Decode.Auto.unsafeFromString (json, isCamelCase = true)
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
                    Encode.Auto.toString (0, data, isCamelCase = false)
                    |> fun json -> 
                    Decode.Auto.unsafeFromString (json, isCamelCase = false)
                    
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
                    CachedCoder.encode ( data, isCamelCase = true)
                    |> fun json -> 
                    CachedCoder.decode (json, isCamelCase = true)
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
                    CachedCoder.encode ( data, isCamelCase = false)
                    |> fun json -> 
                    CachedCoder.decode (json, isCamelCase = false)
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
                    CachedCoder.encode ( data, isCamelCase = true)
                    |> fun json -> 
                    Decode.Auto.unsafeFromString (json, isCamelCase = true)
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
                    CachedCoder.encode ( data,  isCamelCase = false)
                    |> fun json -> 
                    Decode.Auto.unsafeFromString (json, isCamelCase = false)
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
                    Encode.Auto.toString (0, data, isCamelCase = true)
                    |> fun json -> 
                    CachedCoder.decode (json, isCamelCase = true)
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
                    Encode.Auto.toString (0, data, isCamelCase = false)
                    |> fun json -> 
                    CachedCoder.decode (json, isCamelCase = false)
                equal expected actual
        ]
    ]
