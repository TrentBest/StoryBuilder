# Development and Verification

## Prerequisites

- .NET 8 SDK
- Git

## Build and test the domain core

From the repository root:

```sh
dotnet build src/StoryBuilder.Core/StoryBuilder.Core.csproj --configuration Release
dotnet build src/StoryBuilder.FSM/StoryBuilder.FSM.csproj --configuration Release
dotnet build src/StoryBuilder.MicroBundle/StoryBuilder.MicroBundle.csproj --configuration Release\ndotnet build src/StoryBuilder.Persistence/StoryBuilder.Persistence.csproj --configuration Release
dotnet build samples/StoryBuilder.Quickstart/StoryBuilder.Quickstart.csproj --configuration Release
dotnet test tests/StoryBuilder.Core.Tests/StoryBuilder.Core.Tests.csproj --configuration Release
```

The test suite covers low-friction character drafts, explicit synonym search with attribution, automatic re-indexing after edits, scene narration and reordering, workspace boundaries/identity, FSM_API lifecycle transitions, and FSM_COS composition of the StoryBuilder MicroBundle, and binary workspace round trips with malformed-payload rejection.

## Architecture guardrails

- Keep the domain core host-neutral and dependency-light.
- Keep implementation records separate from fictional canon.
- Preserve surface wording and attribution in vocabulary indexing.
- Do not turn suggested character traits into facts without author action.
- Keep scene narration editable and grounded in recorded beats.
- Integrate neighboring Workshop packages only after inspecting their current contracts.

## Publishing

No publishing or release workflow is enabled by this development guide. Package publication requires explicit authorization from the repository owner.
