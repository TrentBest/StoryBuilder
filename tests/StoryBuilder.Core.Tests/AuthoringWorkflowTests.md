# AuthoringWorkflowTests

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

These tests cover the first authoring contracts: character drafts remain useful with minimal input, suggested traits are not silently applied, explicit synonym search preserves surface words and attribution, synonyms can be added after words have been recorded, workspace edits refresh word occurrences, and scene narration/reordering follows recorded beats.

## Verification

Run from the repository root:

```sh
dotnet test tests/StoryBuilder.Core.Tests/StoryBuilder.Core.Tests.csproj --configuration Release
```

The tests are wired into CI. Their pass/fail state must be taken from the actual workflow run, not inferred from their presence.
