---
title: Unknown fields
---

It can happen that you receive a JSON but you don't know which fields are going to be present.

Consider the following JSON:

```json
{
    "ts": "2020-01-01T00:00:00Z",
    "EUR_PLN": { "rate": "4.55" },
    "GBP_PLN": { "error": "Rate is not available at the moment" },
    "USD_PLN": { "rate": "4.01" }
}
```

In this example, we know that:

- The `ts` field is required
- There is an unknown number of `Rate` fields which consist of:
    - The key property, which has the format:
        1. `baseCurrency`
        2. `_`
        3. `quoteCurrency`
    - The value, which should have a `rate` field holding a `decimal`

We are now going to write a decoder capable of handling such a JSON.

## Custom decoders

We need two decoders which are not part of Thoth.Json.

1. One that turns a failure into `None` instead of failing.
2. One that decodes all the object fields and keeps only the valid ones.

```fs
module Decode =

    let ignoreFail (decoder: Decoder<'T>) : Decoder<'T option> =
        { new Decoder<'T option> with
            member _.Decode(helpers, value) =
                match decoder.Decode(helpers, value) with
                | Ok x -> Ok(Some x)
                | Error _ -> Ok None
        }

    let keyValueOptions (decoder: Decoder<'a option>) : Decoder<(string * 'a) list> =
        decoder
        |> Decode.keyValuePairs
        |> Decode.map (
            List.collect (fun (key, value) ->
                match value with
                | Some value -> [ key, value ]
                | None -> []
            )
        )
```

## Define our domain types

First, a type to represent the time field, then one for a valid `Rate` field.

```fs
type Ts = Ts of System.DateTime

module Ts =

    let decoder: Decoder<Ts> = Decode.datetimeUtc |> Decode.map Ts

type RateObject = RateObject of decimal

module RateObject =

    let decoder: Decoder<RateObject> =
        Decode.field "rate" Decode.decimal |> Decode.map RateObject
```

Now a type to store all the information associated to a rate. It holds the name of the two
currencies and the rate.

```fs
type Rate =
    {
        BaseCurrency: string
        QuoteCurrency: string
        Rate: decimal
    }
```

There is no `Decoder<Rate>`, because the information required to build a `Rate` is not stored in a
standard object. It comes from both the field name and the associated value.

To work with this JSON we go through all the fields of the object.

```fs
module Rates =

    let decoder: Decoder<Rate list> =
        // 1. Retrieve all the valid RateObject fields and their associated name
        Decode.keyValueOptions (Decode.ignoreFail RateObject.decoder)
        // 2. Now that we have all the potentially valid Rate fields,
        // check whether they have a valid name
        |> Decode.andThen (fun rateObjects ->
            rateObjects
            |> List.map (fun (fieldName, RateObject rate) ->
                // The fieldName is valid if it contains a `_`
                // The format is [baseCurrency]_[quoteCurrency]
                match fieldName.Split('_') with
                | [| baseCurrency; quoteCurrency |] ->
                    Some
                        {
                            BaseCurrency = baseCurrency
                            QuoteCurrency = quoteCurrency
                            Rate = rate
                        }
                // Returning None filters the invalid fields out without failing
                | _ -> None
            )
            |> List.choose id
            |> Decode.succeed
        )
```

## Compose everything

The final type is an object with the time and the list of rates retrieved from the JSON.

Run the example against the three JSONs below to see what each one gives.

```fsharp live
open Thoth.Json.Core
open Thoth.Json.JavaScript

module Decode =

    let ignoreFail (decoder: Decoder<'T>) : Decoder<'T option> =
        { new Decoder<'T option> with
            member _.Decode(helpers, value) =
                match decoder.Decode(helpers, value) with
                | Ok x -> Ok(Some x)
                | Error _ -> Ok None
        }

    let keyValueOptions
        (decoder: Decoder<'a option>)
        : Decoder<(string * 'a) list>
        =
        decoder
        |> Decode.keyValuePairs
        |> Decode.map (
            List.collect (fun (key, value) ->
                match value with
                | Some value -> [ key, value ]
                | None -> []
            )
        )

type Ts = Ts of System.DateTime

module Ts =

    let decoder: Decoder<Ts> = Decode.datetimeUtc |> Decode.map Ts

type RateObject = RateObject of decimal

module RateObject =

    let decoder: Decoder<RateObject> =
        Decode.field "rate" Decode.decimal |> Decode.map RateObject

type Rate =
    {
        BaseCurrency: string
        QuoteCurrency: string
        Rate: decimal
    }

module Rates =

    let decoder: Decoder<Rate list> =
        Decode.keyValueOptions (Decode.ignoreFail RateObject.decoder)
        |> Decode.andThen (fun rateObjects ->
            rateObjects
            |> List.map (fun (fieldName, RateObject rate) ->
                match fieldName.Split('_') with
                | [| baseCurrency; quoteCurrency |] ->
                    Some
                        {
                            BaseCurrency = baseCurrency
                            QuoteCurrency = quoteCurrency
                            Rate = rate
                        }
                | _ -> None
            )
            |> List.choose id
            |> Decode.succeed
        )

type ExchangeRate =
    {
        Time: System.DateTime
        Rates: Rate list
    }

module ExchangeRate =

    let private ctor (Ts time: Ts) (rates: Rate list) =
        {
            Time = time
            Rates = rates
        }

    let decoder: Decoder<ExchangeRate> =
        Decode.map2 ctor (Decode.field "ts" Ts.decoder) Rates.decoder

let jsonWithError =
    """
{
    "ts": "2020-01-01T00:00:00Z",
    "EUR_PLN": { "rate": "4.55" },
    "GBP_PLN": { "error": "Rate is not available at the moment" },
    "USD_PLN": { "rate": "4.01" }
}
    """

let jsonEmptyRates =
    """
{
    "ts": "2020-01-01T00:00:00Z"
}
    """

let jsonMissingTime =
    """
{
    "EUR_PLN": { "rate": "4.55" }
}
    """

Decode.fromString ExchangeRate.decoder jsonWithError |> Docs.print

Decode.fromString ExchangeRate.decoder jsonEmptyRates |> Docs.print

Decode.fromString ExchangeRate.decoder jsonMissingTime |> Docs.print
```
