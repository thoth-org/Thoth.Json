# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## 3.0.0 - 2019-04-17
### Changed
* Release stable version

## 3.0.0-beta-005 - 2019-04-02
### Changed
* Make Decode.at be consistant and reports the exact error

## 3.0.0-beta-004 - 2019-04-02
### Fixed
* Fix Decode.oneOf in combination with object builder (by @alfonsogarciacaro)
* Make `Decode.field` consistant and report the exact error

### Changed
* Make Decode.object output 1 error or all the errors if there are severals
* Improve BadOneOf errors readibility

## 3.0.0-beta-003 - 2019-03-17
### Changed
* Upgrade to `Fable.Core` v3-beta

## 3.0.0-beta-002 - 2019-01-11
### Added
* Adding `TimeSpan` support (by @rfrerebe)

## 3.0.0-beta-001 - 2018-12-10
### Added
* Add `Set` support in auto coders (by @alfonsogarciacaro)
* Use reflection for auto encoders just as auto decoders. This will help keep the JSON representatin in synx between manual and auto coders (by @alfonsogarciacaro)
* Add `extra` support to auto coders. So people can now override/extends auto coders capabilities (by @alfonsogarciacaro)

### Fixed
* Fix #103: Optional Records fail with Auto Decoder in Internet Explorer (by @SCullman)

### Changed
* `Decode.datetime` always outputs universal time (by @alfonsogarciacaro)
* If a coder is missing, auto coders will fail on generation phase instead of coder evaluation phase (by @alfonsogarciacaro)
* By default `int64` - `uint64` - `bigint` - `decimal` support is being disabled from auto coders to reduce bundle size (by @alfonsogarciacaro)

### Removed
* Mark `Decode.unwrap` as private. It's now only used internally for object builder. This will encourage people to use `Decode.fromValue`.

## 2.5.0 - 2018-11-08
### Added
* Make auto decoder support record/unions with private constructors

## 2.4.0 - 2018-11-07
### Added
* Make auto decoder succeeds on Class marked as optional

## 2.3.0 - 2018-10-18
### Added
* Added CultureInfo.InvariantCulture to all Encoder functions where it was possible (by @draganjovanovic1)

### Fixed
* Fix #59: Make autodecoder support optional fields when missing from JSON
* Fix #51: Add support for `Raw` decoder in object builders


## 2.2.0 - 2018-10-11
### Added
* Re-add optional and optionalAt related to #51

### Fixed
* Fix Encode.Auto
* Fix decoding of optional fields (by @eugene-g)

### Changed
* Various improvements for Primitive types improvements  (by @draganjovanovic1)


## 2.1.0 - 2018-10-03
### Fixed
* Fix nested object builder (ex: get.Optional.Field > get.Required.Field)
* Fix exception handling

## 2.0.0 - 2018-10-01
### Added
* Stable release for Fable 2

## 2.0.0-beta-005 - 2018-08-31
### Changed
* Make `Encode.Value` an alias of `obj` instead of an empty interface

## 2.0.0-beta-004 - 2018-08-03
### Added
* Add Encoders for all the equivalent Decoders

## 2.0.0-beta-003 - 2018-07-16
### Changed
* Make auto decoder safe by default

## 2.0.0-beta-002 - 2018-07-12
### Fixed

* Fix `Decode.decodeString` signature

## 2.0.0-beta-001 - 2018-07-11
### Added

* Support auto decoders and encoders
* Add object builder style for the decoders

### Changed

* Support Fable 2
* Better error, by now tracking the path

### Deprecated

* Mark `Encode.encode`, `Decode.decodeString`, `Decode.decodeValue` as obsoletes

### Removed

* Remove pipeline style for the decoders

## 1.1.0 - 2018-06-08
### Fixed

* Ensure that `field` `at` `optional` `optionalAt` works with object

## 1.0.0 - 2018-04-17
### Added

* Initial release
