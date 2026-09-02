---
title: Thoth.Json
description: JSON the simple and safe way
layout: bare
---

<div class="landing">

<div class="selling-point">
<div class="selling-point-header">
<h3>Quick and friendly feedback</h3>
<p>Thoth.Json reports helpful errors. Stop wasting time searching why your JSON is invalid.</p>
</div>
<div class="selling-point-showcase">

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
<h3>Build with small blocks</h3>
<p>Thoth.Json allows you to work on small functions and then combine them into a bigger one making it easy to handle complex data.</p>
</div>
<div class="selling-point-showcase">

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
<h3>Isomorphic support</h3>
<p>If you use F# on the server and client, you can directly use your F# types to define your JSON.</p>
</div>
<div class="selling-point-showcase">

```fs
type User =
    {
        Email: string
        Firstname: string
    }

let user =
    {
        Email = "maxime@mail.com"
        Firstname = "Maxime"
    }

// Transform your F# types to JSON
let userJson =
    user
    |> Encode.Auto.generateEncoder<User> ()
    |> Encode.toString 4

// Transform your JSON to F# types
let userFromJson =
    userJson
    |> Decode.fromString (Decode.Auto.generateDecoder<User> ())
```

</div>
</div>

<div class="selling-point is-fullwidth">
<div class="selling-point-header">
<h3>Extensible</h3>
<p>Extends Thoth.Json with your own decoders and encoders.</p>
</div>
<div class="selling-point-showcase">

```fs
module Encode =

    let timestamp (date: DateTime) =
        DateTimeOffset(date).ToUnixTimeSeconds() |> float |> Encode.float

module Decode =

    let timestamp: Decoder<DateTime> =
        { new Decoder<DateTime> with
            member _.Decode(helpers, value) =
                if helpers.isNumber value then
                    helpers.asFloat value
                    |> int64
                    |> DateTimeOffset.FromUnixTimeSeconds
                    |> _.DateTime
                    |> Ok
                else
                    Error("", BadPrimitive("a timestamp", value))
        }

// Example: decode an invalid JSON
Decode.fromString Decode.timestamp "\"2022-01-01\""

// Error at: `$`
// Expecting a timestamp but instead got:
// "2022-01-01"
```

</div>
</div>

</div>
