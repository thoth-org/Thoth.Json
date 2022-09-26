---
layout: navbar-only
title: Thoth.Json
---

<div class="container mt-5" date-disable-copy-button="true">
    <!-- <section class="section">
        <h2 class="title is-2 has-text-primary has-text-centered">
            Thoth.Json
        </h2>
        <p class="content is-size-5 has-text-centered">
            JSON the simple and safe way
        </p>
    </section> -->
    <section class="section selling-points">

<div class="selling-point">
    <div class="selling-point-header">
        <h4 class="title has-text-primary">
            Quick and friendly feedback
        </h4>
        <p class="content is-size-5">
            Thoth.Json reports helpful errors. Stop wasting time searching why your JSON is invalid.
        </p>
    </div>
    <div class="selling-point-showcase content">

```json
Error at: `$.user.firstname`
Expecting an object with path `user.firstname` but instead got:
{
    "user": {
        "name": "maxime",
        "age": 25
    }
}
Node `firstname` is unknown.
```
</div>
</div>

<div class="selling-point is-fullwidth">
    <div class="selling-point-header">
        <h4 class="title has-text-primary">
            Build with small blocks
        </h4>
        <p class="content is-size-5">
            Thoth.Json allows you to work on small functions and then combine them into a bigger one making it easy to handle complex data.
        </p>
    </div>
    <div class="selling-point-showcase content">

```fs
module Author =
    let decoder =
        Decode.object (fun get ->
            {
                Name = get.Required.Field "name" Decode.string
                Profile = get.Required.Field "profile" Decode.string
            }
        )

module Post =
    let decoder =
        Decode.object (fun get ->
            {
                Title = get.Required.Field "title" Decode.string
                Abstract = get.Required.Field "abstract" Decode.string
                Date = get.Required.Field "date" Decode.datetime
                Author = get.Required.Field "author" Author.decoder
            }
        )
```
</div>
</div>

<div class="selling-point">
    <div class="selling-point-header">
        <h4 class="title has-text-primary">
            Isomorphic support
        </h4>
        <p class="content is-size-5">
            If you use F# on the server and client, you can directly use your F# types to define your JSON.
        </p>
    </div>
    <div class="selling-point-showcase content">

```fs
type User =
    {
        Email : string
        Firstname : string
    }

let user =
    {
        Email = "maxime@mail.com"
        Firstname = "Maxime"
    }

// Transform your F# types to JSON
let userJson =
    Encode.Auto.toString(4, user)

// Transform your JSON to F# types
let userFromJson =
    Decode.Auto.fromString<User> userJson
```
</div>
</div>


<div class="selling-point is-fullwidth">
    <div class="selling-point-header">
        <h4 class="title has-text-primary">
            Extensible
        </h4>
        <p class="content is-size-5">
            Extends Thoth.Json with your own decoders and encoders.
        </p>
    </div>
    <div class="selling-point-showcase content">

```fs
module Encode =

    let timestamp (date : DateTime) =
        DateTimeOffset(date).ToUnixTimeSeconds()
        |> box

module Decode =

    let timestamp : Decoder<DateTime> =
        fun path value ->
            if Decode.Helpers.isNumber value then
                let value : int64 = unbox value
                let datetime =
                    DateTimeOffset
                        .FromUnixTimeSeconds(value)
                        .DateTime
                Ok datetime

            else
                (path, BadPrimitive("a timestamp", value)) |> Error

// Example: decode an invalid JSON
Decode.fromString Decode.timestamp "\"2022-01-01\""

// Error at: `$`
// Expecting a number but instead got:
// "2022-01-01"
```
</div>
</div>

</div>
