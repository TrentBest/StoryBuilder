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

The .NET 8 domain core includes:

- **Casting:** create, select, revise, and add/remove character traits from editable drafts.
- **The Stage:** capture ordered action, reaction, dialogue, and pose beats; reorder them and copy an editable narration draft.
- **Vocabulary:** index character and scene words, record explicit synonyms, and refresh counts when an author edits a record.
- **World Archive:** record historical claims with optional source provenance and explicit canon status.
- **Campaign Room:** create campaigns and record participant choices as proposals until deliberately accepted.
- **Review lifecycle:** the optional StoryBuilder.FSM adapter uses FSM_API to govern draft, proposed, accepted, rejected, and deprecated transitions.
- **Composition contract:** StoryBuilder.MicroBundle exposes authoring capabilities through MicroBundleDomain. An automated test composes it through FSM_COS; the bundle itself does not depend on the composition kernel.

The architecture follows the Workshop's boundaries: [FSM_API](https://github.com/TrentBest/FSM_API) governs explicit state behavior where appropriate; [FSM_COS](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS) composes capabilities at the host boundary; [GUI](https://github.com/TrentBest/TheSingularityWorkshop.GUI) and GUI.SVG are the intended semantic authoring and living-documentation layers; [WebApp](https://github.com/TrentBest/TheSingularityWorkshop.WebApp) and [WebPage](https://github.com/TrentBest/WebPage) are potential hosts. The local Blazor workbench is interactive; integration into the Workshop's WebPage and living GUI.SVG views is still open.

## 🟢 04 See it in a minute

Prerequisite: .NET 8 SDK. From the repository root, run the interactive workbench:

```sh
dotnet run --project samples/StoryBuilder.WebDemo/StoryBuilder.WebDemo.csproj --urls http://localhost:5080
```

Open http://localhost:5080. Try Casting to create and revise a character, The Stage to add and reorder beats, World Archive to record a sourced claim, or Campaign Room to record a participant choice.

The demo uses real StoryBuilder domain records and a session created through StoryBuilderAuthoringBundle. Data is in memory for the current browser circuit; refreshing starts a new workspace. It is a local proving ground, not yet the integrated WebPage experience.

For the smaller console recipe:

```sh
dotnet run --project samples/StoryBuilder.Quickstart/StoryBuilder.Quickstart.csproj
```

To run the automated tests:

```sh
dotnet test tests/StoryBuilder.Core.Tests/StoryBuilder.Core.Tests.csproj --configuration Release
```

## 🟪 05 Available documentation and theory

- [Documentation Index](DOCUMENTATION_INDEX.md) — choose a path through the repository.
- [Architecture](docs/ARCHITECTURE.md) — package ownership and the Workshop integration plan.
- [WebDemo Guide](samples/StoryBuilder.WebDemo/README.md) — how to run and explore the interactive prototype.
- [Character Creation](docs/CHARACTER_CREATION.md) — defaults, prompts, templates, and editable character profiles.
- [Scene Capture](docs/SCENE_CAPTURE.md) — playing scenes, beat capture, pose sequences, and narration drafts.
- [Vocabulary and Provenance](docs/VOCABULARY_AND_PROVENANCE.md) — synonym usage, attribution, source records, and canon status.
- [Gallery](docs/Gallery.md) — runnable recipes and honest evidence of current behavior.
- [Development Status](docs/DEVELOPMENT_STATUS.md) — implemented scope, open work, and readiness estimate.
- [Branch Reconciliation](docs/BRANCH_RECONCILIATION.md) — the safe route to the canonical branch model.
- [Story Foundation](docs/STORY_FOUNDATION.md) and [Historical Scale](docs/HISTORICAL_SCALE.md) — the initial science-fiction setting and its nested chronology.
- [Diegetic StoryBuilder](docs/DIEGETIC_STORYBUILDER.md) — the in-world tool concept.

## Current boundary

The domain core, FSM_API lifecycle adapter, and FSM_COS-composable MicroBundle contract are implemented and tested. The interactive Blazor workbench has a successful build/test run at commit aaabdbe91e818ad766fb6773e8817726c7a2623a; subsequent documentation commits trigger fresh checks. Data is currently in memory; persistent storage, full campaign runtime, physics-backed fight playback, and live GUI.SVG views remain open work. No package publishing or release has been authorized.

## License

License selection is pending.
