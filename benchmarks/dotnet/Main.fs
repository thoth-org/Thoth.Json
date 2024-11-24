module Thoth.Json.DotNet.Benchmark

open System
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
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
            let firstname = get.Required.Field "firstname" Decode.string
            let lastname = get.Required.Field "lastname" Decode.string

            {
                Gender = get.Required.Field "gender" Decode.string
                FullName = firstname + " " + lastname
                Email = get.Required.Field "email" Decode.string
                CellPhone = get.Required.Field "cell" Decode.string
                OfficePhone = get.Required.Field "phone" Decode.string
                Age = get.Required.Field "age" Decode.int
                Birthday = get.Required.Field "dob" Decode.datetimeUtc
                Picture = get.Required.Field "picture" Decode.string
            }
        )

let userJson =
    """{
    "gender": "male",
    "firstname": "Kaladin",
    "lastname": "Stormblessed",
    "email": "coskun.sadiklar@example.com",
    "age": 77,
    "dob": "1947-07-30T14:54:27.372Z",
    "picture": "https://randomuser.me/api/portraits/men/95.jpg"
}"""

type Benchmarks() =
    [<Benchmark(Description = "Thoth.Json.Newtonsoft")>]
    member this.ThothJsonNewtonsoft() =
        Newtonsoft.Decode.fromString User.Decoder userJson

    [<Benchmark(Description = "Thoth.Json.System.Text.Json")>]
    member this.ThothJsonSystemTextJson() =
        System.Text.Json.Decode.fromString User.Decoder userJson

    [<Benchmark(Description = "Newtonsoft")>]
    member this.Newtonsoft() =
        Newtonsoft.Json.JsonConvert.DeserializeObject<User>(userJson)

    [<Benchmark(Baseline = true, Description = "System.Text.Json")>]
    member this.SystemTextJson() =
        System.Text.Json.JsonSerializer.Deserialize<User>(userJson)

BenchmarkRunner.Run<Benchmarks>() |> ignore
