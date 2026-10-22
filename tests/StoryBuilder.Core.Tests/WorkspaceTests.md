# WorkspaceTests

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

These tests protect two foundational boundaries: implementation records and fictional claims occupy distinct collections, one workspace collection cannot contain duplicate record identities, and participant choices stay proposals while their words are indexed with attribution.

## Verification

Run from the repository root:

```sh
dotnet test tests/StoryBuilder.Core.Tests/StoryBuilder.Core.Tests.csproj --configuration Release
```

The tests are authored and wired into CI. Their pass/fail state must be taken from the actual workflow run, not inferred from their presence.
