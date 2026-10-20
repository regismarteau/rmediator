# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [2.0.1] - 2026-10-20

### Changed

- `RMediator.DependencyInjection` depends on the `Microsoft.Extensions.DependencyInjection` version matching each target framework (8.0.1, 9.0.0, 10.0.0) instead of 10.x everywhere.
- The public API is documented with XML comments, and package validation is enabled.

## [2.0.0] - 2026-10-20

### Added

- Support for .NET 8 and .NET 9, in addition to .NET 10.
- Symbol packages (`.snupkg`) and SourceLink for step-through debugging.
- Tag-driven release workflow and CI on pull requests.
- Dispatching a request without handler, or with several handlers, now fails with a message that says which case it is and lists the competing handlers.

### Changed

- **Breaking:** `Dispatch` and `Publish` take a `CancellationToken cancellationToken = default` instead of a nullable `CancellationToken?`. Callers that omit the token are unaffected; passing `null` no longer compiles, and the change is binary-incompatible with 1.0.x.
- **Breaking:** `ConfigureServicesOptionsBuilder.Build` is now internal.
- Handler and middleware lookups and the reflection used to invoke handlers are cached, which reduces allocations per dispatch.
- `RMediator.Abstractions` and `RMediator.DependencyInjection` now share the same version, taken from the release tag.

### Fixed

- Abstract classes and interfaces implementing handler interfaces are ignored when scanning assemblies instead of making the service provider fail to build.

## [1.0.1]

### Fixed

- Domain events are awaited, handler exceptions are no longer wrapped in `TargetInvocationException`, and middlewares of a base type are applied to its sub-types.

## [1.0.0]

### Added

- Initial release: commands, queries, domain events and middleware pipeline.

[Unreleased]: https://github.com/regismarteau/rmediator/compare/v2.0.1...HEAD
[2.0.1]: https://github.com/regismarteau/rmediator/compare/v2.0.0...v2.0.1
[2.0.0]: https://github.com/regismarteau/rmediator/compare/v1.0.1...v2.0.0
