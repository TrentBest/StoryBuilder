<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" />
  </a>
</p>

# ✳️ 00 StoryBuilder

[![Build and test](https://img.shields.io/github/actions/workflow/status/TrentBest/StoryBuilder/dotnet.yml?branch=development&style=flat-square&logo=github)](https://github.com/TrentBest/StoryBuilder/actions/workflows/dotnet.yml)

**Build the Experience. Compose its history. Let the campaign make it live.**

## 🟦 01 What

StoryBuilder is a workbench for **creating the tools that capture stories**. It records how an Experience or Tool is built, gives authors low-friction ways to define characters and worlds, and helps campaigns preserve choices, events, and continuity.

The tool is genre-neutral. Our far-future science-fiction setting is its first demanding demonstration—not a dependency of the product.

## 🟣 02 Why

A story tool should not make someone write a dissertation before they can create a character. Start with a name, a role, a phrase, a few adjectives, or a rough action. Defaults and prompts help when wanted; synonyms help users find related language; every record can be revised later.

StoryBuilder also needs to understand how words are used across the story domain. “Brave” and “fearless” may map to one concept, but the tool must preserve which word was used, where it appeared, and who said it. A character's self-description is not the same as a rival's accusation.

## 🩵 03 How

The initial .NET 8 core includes:

- **Casting:** create editable character drafts from optional templates.
- **The Stage:** capture ordered action, reaction, dialogue, and pose beats; produce an editable narration draft.
- **Vocabulary:** record surface words and explicit synonyms, then search usage across indexed records.
- **Workspace:** separate the Experience build record from fictional claims, events, campaigns, participant actions, sources, and continuity issues.
- **Review lifecycle:** the optional StoryBuilder.FSM adapter uses FSM_API to govern draft, proposed, accepted, rejected, and deprecated transitions. The domain core remains independent of FSM_API.

The architecture follows the Workshop's boundaries: [FSM_API](https://github.com/TrentBest/FSM_API) governs explicit state behavior where appropriate; [FSM_COS](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS) composes capabilities at the host boundary; [GUI](https://github.com/TrentBest/TheSingularityWorkshop.GUI) and GUI.SVG are the intended semantic authoring and living-documentation layers; [WebApp](https://github.com/TrentBest/TheSingularityWorkshop.WebApp) and [WebPage](https://github.com/TrentBest/WebPage) are potential hosts. These are planned integration points, not claims that all adapters are already connected.

## 🟢 04 See it in a minute

Prerequisite: .NET 8 SDK. From the repository root, run:

```sh
dotnet run --project samples/StoryBuilder.Quickstart/StoryBuilder.Quickstart.csproj
```

The sample creates a character draft, records a word under a synonym concept, and composes a short scene from a pose, dialogue line, and action. It demonstrates real domain behavior in a console host; it is not yet the final clickable Casting or claymation interface.

To run the automated tests:

```sh
dotnet test tests/StoryBuilder.Core.Tests/StoryBuilder.Core.Tests.csproj --configuration Release
```

## 🟪 05 Available documentation and theory

- [Documentation Index](DOCUMENTATION_INDEX.md) — choose a path through the repository.
- [Architecture](docs/ARCHITECTURE.md) — package ownership and the Workshop integration plan.
- [Character Creation](docs/CHARACTER_CREATION.md) — defaults, prompts, templates, and editable character profiles.
- [Scene Capture](docs/SCENE_CAPTURE.md) — playing scenes, beat capture, pose sequences, and narration drafts.
- [Vocabulary and Provenance](docs/VOCABULARY_AND_PROVENANCE.md) — synonym usage, attribution, source records, and canon status.
- [Gallery](docs/Gallery.md) — runnable recipes and honest evidence of current behavior.
- [Development Status](docs/DEVELOPMENT_STATUS.md) — implemented scope, open work, and readiness estimate.
- [Branch Reconciliation](docs/BRANCH_RECONCILIATION.md) — the safe route to the canonical branch model.
- [Story Foundation](docs/STORY_FOUNDATION.md) and [Historical Scale](docs/HISTORICAL_SCALE.md) — the initial science-fiction setting and its nested chronology.
- [Diegetic StoryBuilder](docs/DIEGETIC_STORYBUILDER.md) — the in-world interface concept.

## Current boundary

The domain core and optional FSM_API lifecycle adapter are initial implementations. Data is currently in memory; persistent storage, clickable authoring screens, campaign runtime, live GUI.SVG views, and verified FSM_COS/serialization adapters remain open work. The .NET workflow builds the core, adapter, and quickstart and runs tests; consult the Actions run for actual execution evidence.

## License

License selection is pending. No package publishing or release is authorized by this README.
