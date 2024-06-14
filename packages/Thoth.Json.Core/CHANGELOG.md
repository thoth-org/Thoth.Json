# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Unreleased

### Changed

* Rework encoder API to not need a custom DU ([GH-188](https://github.com/thoth-org/Thoth.Json/pull/188/))
* Rework object encoder helper to support JSON backends not allowing mutating a JSON object
* Move `Decode.fromValue` to `Decode.Advanced.fromValue`

### Fixed

* Encoding negative integers should keep their sign ([GH-187](https://github.com/thoth-org/Thoth.Json/issues/187))

## 0.2.1 - 2023-12-12

### Fixed

* Do not publish Fable compiled files (`**\*.fs.js`)

## 0.2.0 - 2023-12-12

### Changed

* Disable `Encode.datetimeOffset` for Python as it seems like the implementation of `DateTimeOffset.ToString("O")` is not complete in Fable

## 0.1.0 - 2023-12-12

### Added

* Build path only if the decoder fails (small perfermance improvement) ([GH-43](https://github.com/thoth-org/Thoth.Json/issues/43))
* Port API from Thoth.Json
