# Development and Verification

## Prerequisites

- .NET 8 SDK
- Git

The interactive workbench uses the GUI.SVG source pinned as a Git submodule. Initialize it after cloning:

```sh
git submodule update --init --recursive
```

A fresh clone can use `git clone --recurse-submodules https://github.com/TrentBest/StoryBuilder.git`.

## Build and test

From the repository root:

```sh
dotnet build src/StoryBuilder.Core/StoryBuilder.Core.csproj --configuration Release
dotnet build src/StoryBuilder.FSM/StoryBuilder.FSM.csproj --configuration Release
dotnet build src/StoryBuilder.MicroBundle/StoryBuilder.MicroBundle.csproj --configuration Release
dotnet build src/StoryBuilder.Persistence/StoryBuilder.Persistence.csproj --configuration Release
dotnet build samples/StoryBuilder.Quickstart/StoryBuilder.Quickstart.csproj --configuration Release
dotnet build samples/StoryBuilder.WebDemo/StoryBuilder.WebDemo.csproj --configuration Release
dotnet test tests/StoryBuilder.Core.Tests/StoryBuilder.Core.Tests.csproj --configuration Release
```

The suite covers low-friction character drafts, synonym search and edits, session-driven re-indexing, scene narration/reordering, workspace identity and boundaries, FSM_API lifecycle transitions, FSM_COS composition, binary workspace round trips and malformed-payload rejection, and GUI.SVG output generated from real StoryBuilder records.

## Architecture guardrails

- Keep the domain core host-neutral and dependency-light.
- Keep implementation records separate from fictional canon.
- Preserve surface wording and attribution in vocabulary indexing.
- Do not turn suggested character traits into facts without author action.
- Keep scene narration editable and grounded in recorded beats.
- Keep the GUI.SVG dependency source-pinned; do not require an unpublished NuGet package.

## Publishing

No publishing or release workflow is enabled by this development guide. Package publication requires explicit authorization from the repository owner.
