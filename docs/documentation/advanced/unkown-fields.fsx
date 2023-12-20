(***
layout: nacara/layouts/docs.njk
title: Unkown fields
***)

(*** hide ***)

#I "../../../src/bin/Debug/netstandard2.0"
#r "Thoth.Json.dll"

open Thoth.Json

(**

It can happens that you receive a JSON but you don't know which fields are going to be present.

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

- The `ts` field required
- There is an unkown number of `Rate` fields which consists of:
    - The key property which has the format:
        1. `sourceCurrency`
        2. `_`
        3. `targetCurrency`
    - The value should have a `rate` field which is a `decimal`

We are now going to write a decoder capable of handling such a JSON.

## Custom decoders

We need some specific decoder in order to work with our JSON.

*)

module Decode =

    (**

1. We need a decoder which ignore failed decoder

*)

    let ignoreFail (decoder: Decoder<'T>) : Decoder<'T option> =
        fun path token ->
            match decoder path token with
            | Ok x -> Ok(Some x)
            | Error _ -> Ok None

    (**

2. We need a decoder which decodes all the object fields and only keep the valid ones

*)

    let keyValueOptions
        (decoder: Decoder<'a option>)
        : Decoder<(string * 'a) list>
        =
        decoder
        |> Decode.keyValuePairs
        |> Decode.map (
            List.collect (fun (key, aOpt) ->
                match aOpt with
                | Some a -> [ key, a ]
                | None -> []
            )
        )

(**

## Define our domain types

First, we need a type to represent the time field.
*)


type Ts = | Ts of System.DateTime

module Ts =

    let decoder: Decoder<Ts> = Decode.datetimeUtc |> Decode.map Ts

(**

Then a type to represent a valid `Rate` field.

*)

type RateObject = | RateObject of decimal

module RateObject =

    let decoder: Decoder<RateObject> =
        Decode.field "rate" Decode.decimal |> Decode.map RateObject

(**


Now, we need a type to store all the information associated to a rate.

This type contains the name of the 2 currencies and the rate.

*)

type Rate =
    {
        SourceCurrency: string
        TargetCurrency: string
        Rate: decimal
    }

(**

We don't have a `Decoder<Rate>` because the informations required to build a `Rate` are not stored in a standard object.

Indeed, they are coming from both the field name and the associated value.

In order, to work with this JSON we are going to directly works on all the field of the JSON.

*)

module Rates =

    let decoder: Decoder<Rate list> =
        // 1. We retrieve all the valid RateObject fields and their associated name
        Decode.keyValueOptions (Decode.ignoreFail RateObject.decoder)
        // 2. Now that we have all the potential valid Rate fields
        // We need to verify if they have a valid name
        |> Decode.andThen (fun rateObjects ->
            rateObjects
            |> List.map (fun (fieldName, (RateObject rate)) ->

                // We consider the fieldName valid if it contains a `_`
                // The format is [sourceCurrency]_[targetCurrency]
                match fieldName.Split('_') with
                | [| sourceCurrency; targetCurrency |] ->
                    // The fieldName is valid, we can build the Rate record
                    Some
                        {
                            SourceCurrency = sourceCurrency
                            TargetCurrency = targetCurrency
                            Rate = rate
                        }
                // If the fieldName is invalid
                // Returns None, this will allow us to filter invalid fields without failing
                | _ -> None

            )
            // Only keep valid fields
            |> List.choose id
            // Return a decoder which succeeds
            |> Decode.succeed
        )

(**

## Compose everything

Now, we have everything we need to build our final type.

It consist in an object with the time and the list of rates retrieved from the JSON.

*)

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

(**

Here is our decoder tested against different JSONs and their result.

*)

let jsonWithError =
    """
{
    "ts": "2020-01-01T00:00:00Z",
    "EUR_PLN": { "rate": "4.55" },
    "GBP_PLN": { "error": "Rate is not available at the moment" },
    "USD_PLN": { "rate": "4.01" }
}
    """

Decode.fromString ExchangeRate.decoder jsonWithError

// Returns:
// Ok
//    {
//        Time = System.DateTime(2020, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)
//        Rates = [
//                {
//                    SourceCurrency = "EUR"
//                    TargetCurrency = "PLN"
//                    Rate = 4.55m
//                }
//                {
//                    SourceCurrency = "USD"
//                    TargetCurrency = "PLN"
//                    Rate = 4.01m
//                }
//            ]
//    }

(**
*)

let jsonEmptyRates =
    """
{
    "ts": "2020-01-01T00:00:00Z"
}
    """

Decode.fromString ExchangeRate.decoder jsonEmptyRates

// Returns:
// Ok
//    {
//        Time = System.DateTime(2020, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)
//        Rates = [ ]
//    }

(**
*)

let jsonMissingTime =
    """
{
    "EUR_PLN": { "rate": "4.55" }
}
    """

Decode.fromString ExchangeRate.decoder jsonMissingTime

// Returns:
// Error at: `$`
// Expecting an object with a field named `ts` but instead got:
// {
//     "EUR_PLN": {
//         "rate": "4.55"
//     }
// }
