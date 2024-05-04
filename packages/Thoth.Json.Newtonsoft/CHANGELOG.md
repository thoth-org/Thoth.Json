# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Unreleased

### Changed

* Rework encoder API to not need a custom DU ([GH-188](https://github.com/thoth-org/Thoth.Json/pull/188/))

### Fixed

* Encoding negative integers should keep their sign ([GH-187](https://github.com/thoth-org/Thoth.Json/issues/187))

### Added

* `Decode.unsafeFromString`

## 0.1.0 - 2023-12-12

### Added

* Initial release
