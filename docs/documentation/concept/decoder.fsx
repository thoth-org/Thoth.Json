(***
layout: nacara/layouts/docs.njk
title: Decoder
***)

(*** hide ***)

#I "../../src/bin/Debug/netstandard2.0"
#r "Thoth.Json.dll"

open Thoth.Json

(**

A decoder is a function that takes a JSON value and transforms it into an F# value.

**Example of a successful decoder:**

*)

Decode.fromString Decode.int "1"

(**
Returns:

```fs
Ok 1
```

**Example of a failed decoder:**

In case of an error, a decoder returns an helpful errors explaining:

1. Where the error happened in the JSON using the JSONPath syntax.

    You can use tools like [JSONPath Online Evaluator](http://jsonpath.com/) to explore your JSON

    <span/>

2. What was expected

3. What was received

*)

Decode.fromString Decode.int "\"maxime\""

(**
Returns:

```json
Error at: `$`
Expecting an int but instead got: "maxime"
```
*)
