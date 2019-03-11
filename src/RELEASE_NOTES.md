#### 3.0.0-beta-002

* Adding `TimeSpan` support (by @rfrerebe)

#### 3.0.0-beta-001

* Fix #103: Optional Records fail with Auto Decoder in Internet Explorer (by @SCullman)
* Add `Set` support in auto coders (by @alfonsogarciacaro)
* Use reflection for auto encoders just as auto decoders. This will help keep the JSON representatin in synx between manual and auto coders (by @alfonsogarciacaro)
* `Decode.datetime` always outputs universal time (by @alfonsogarciacaro)
* Add `extra` support to auto coders. So people can now override/extends auto coders capabilities (by @alfonsogarciacaro)
* If a coder is missing, auto coders will fail on generation phase instead of coder evaluation phase (by @alfonsogarciacaro)
* By default `int64` - `uint64` - `bigint` - `decimal` support is being disabled from auto coders to reduce bundle size (by @alfonsogarciacaro)
* Mark `Decode.unwrap` as private. It's now only used internally for object builder. This will encourage people to use `Decode.fromValue`.

#### 2.5.0

* Make auto decoder support record/unions with private constructors

#### 2.4.0

* Make auto decoder succeeds on Class marked as optional

#### 2.3.0

* Fix #59: Make autodecoder support optional fields when missing from JSON
* Fix #51: Add support for `Raw` decoder in object builders
* Added CultureInfo.InvariantCulture to all Encoder functions where it was possible (by @draganjovanovic1)

#### 2.2.0

* Re-add optional and optionalAt related to #51
* Fix Encode.Auto
* Various improvements for Primitive types improvements  (by @draganjovanovic1)
* Fix decoding of optional fields (by @eugene-g)

#### 2.1.0

* Fix nested object builder (ex: get.Optional.Field > get.Required.Field)
* Fix exception handling

#### 2.0.0

* Stable release for Fable 2

#### 2.0.0-beta-005

* Make `Encode.Value` an alias of `obj` instead of an empty interface

#### 2.0.0-beta-004

* Add Encoders for all the equivalent Decoders

#### 2.0.0-beta-003

* Make auto decoder safe by default

#### 2.0.0-beta-002

* Fix `Decode.decodeString` signature

#### 2.0.0-beta-001

* Mark `Encode.encode`, `Decode.decodeString`, `Decode.decodeValue` as obsoletes
* Support Fable 2
* Support auto decoders and encoders
* Remove pipeline style for the decoders
* Add object builder style for the decoders
* Better error, by now tracking the path

#### 1.1.0

* Ensure that `field` `at` `optional` `optionalAt` works with object

#### 1.0.0

* Initial release
