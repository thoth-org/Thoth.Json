module Thoth.Json.DotNet.Benchmark

open System
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open BenchmarkDotNet.Order
open Thoth.Json
open Thoth.Json.Core

type User =
    {
        Gender: string
        FullName: string
        Email: string
        CellPhone: string
        OfficePhone: string
        Age: int
        Birthday: DateTime
        Picture: string
    }

    static member Decoder =
        Decode.object (fun get ->
            {
                Gender = get.Required.Field "Gender" Decode.string
                FullName = get.Required.Field "FullName" Decode.string
                Email = get.Required.Field "Email" Decode.string
                CellPhone = get.Required.Field "CellPhone" Decode.string
                OfficePhone = get.Required.Field "OfficePhone" Decode.string
                Age = get.Required.Field "Age" Decode.int
                Birthday = get.Required.Field "Birthday" Decode.datetimeUtc
                Picture = get.Required.Field "Picture" Decode.string
            }
        )

let userJson =
    """{
    "Gender": "male",
    "FullName": "Kaladin Stormblessed",
    "Email": "coskun.sadiklar@example.com",
    "Age": 77,
    "CellPhone": "(555)-555-5555",
    "OfficePhone": "(555)-555-5555",
    "Birthday": "1947-07-30T14:54:27.372Z",
    "Picture": "https://randomuser.me/api/portraits/men/95.jpg"
}"""

[<Orderer(SummaryOrderPolicy.FastestToSlowest)>]
type Benchmarks() =
    [<Benchmark(Description = "Thoth.Json.Newtonsoft")>]
    member _.ThothJsonNewtonsoft() =
        Newtonsoft.Decode.fromString User.Decoder userJson

    [<Benchmark(Description = "Thoth.Json.System.Text.Json")>]
    member _.ThothJsonSystemTextJson() =
        System.Text.Json.Decode.fromString User.Decoder userJson

    [<Benchmark(Description = "Newtonsoft")>]
    member _.Newtonsoft() =
        Newtonsoft.Json.JsonConvert.DeserializeObject<User>(userJson)

    [<Benchmark(Baseline = true, Description = "System.Text.Json")>]
    member _.SystemTextJson() =
        System.Text.Json.JsonSerializer.Deserialize<User>(userJson)

BenchmarkRunner.Run<Benchmarks>() |> ignore
