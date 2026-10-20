# Contributing

Thanks for your interest in RMediator! Bug reports, ideas and pull requests are welcome.

## Before you start

For anything beyond a small fix, please open an issue first so we can agree on the approach. RMediator intentionally stays small: features that belong to a bigger framework are likely to be declined.

## Build and test

You need the .NET 10 SDK (see `global.json`) and the .NET 8 and 9 runtimes, since the tests run on the three target frameworks.

```sh
dotnet build RMediator.slnx
dotnet test RMediator.slnx
```

The build is strict: warnings are errors and the code style defined in `.editorconfig` is enforced. Format your changes with:

```sh
dotnet format RMediator.slnx
```

## Pull requests

- Branch from `main` and keep the pull request focused on one change.
- Add or update tests. A bug fix comes with a test that fails without the fix.
- Update `README.md` and `CHANGELOG.md` (under `Unreleased`) when the behavior or the public API changes.
- Use short, imperative commit messages, prefixed with `feat:`, `fix:`, `chore:`, `ci:` or `docs:`.
- The CI must be green before a pull request is merged.

## Releases

Releases are published by the maintainer. Pushing a tag such as `v1.2.0` runs the CI, packs both packages with that version, pushes them to NuGet and creates the GitHub release.

## Code of conduct

Be respectful and constructive. Harassment and discrimination are not tolerated.
