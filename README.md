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
- **Persistence:** StoryBuilder.Persistence uses FSM_Serialization's binary stream contract with a versioned StoryBuilder workspace schema. The WebDemo autosaves to browser localStorage; it does not use JSON.

The architecture follows the Workshop's boundaries: [FSM_API](https://github.com/TrentBest/FSM_API) governs explicit state behavior where appropriate; [FSM_COS](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS) composes capabilities at the host boundary; [GUI](https://github.com/TrentBest/TheSingularityWorkshop.GUI) and GUI.SVG provide the semantic model and living SVG manifestation; [WebApp](https://github.com/TrentBest/TheSingularityWorkshop.WebApp) and [WebPage](https://github.com/TrentBest/WebPage) are the intended Workshop hosts. The local Blazor workbench now includes a GUI.SVG map rendered from live StoryBuilderSession records; embedding the Experience into the Workshop's WebPage remains open.

## 🟢 04 See it in a minute

Prerequisite: .NET 8 SDK. Initialize the pinned GUI.SVG source submodule once with `git submodule update --init --recursive`, then run the interactive workbench from the repository root:

```sh
dotnet run --project samples/StoryBuilder.WebDemo/StoryBuilder.WebDemo.csproj --urls http://localhost:5080
```

Open http://localhost:5080. Narrator is the first stop: enter one idea and receive an editable premise proposal. Then try Casting to revise a character, The Stage to capture a scene, World Archive to record a sourced claim, Campaign Room to record a participant choice, or Living Map to see the current cast and vocabulary rendered by GUI.SVG.

The demo uses real StoryBuilder domain records and a session created through StoryBuilderAuthoringBundle. It autosaves a versioned binary workspace in this browser's localStorage and restores it after refresh. Saves are browser-local; there is no cloud sync or multi-user sharing. It is a local proving ground, not yet the integrated WebPage experience.

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
- [Narrator Experience](docs/NARRATOR_EXPERIENCE.md) — low-friction, visual-first story creation; target output discovery; and the digital publication boundary.
- [Character Casting Studio](docs/CHARACTER_CASTING_STUDIO.md) — cast roles, the three-sided mirror concept, word occurrences, and careful pronoun-reference tracking.
- [Timelines, Gestures, and Multiverse History](docs/TIMELINES_GESTURES_MULTIVERSE.md) — world events distinct from prose, timeline branches and divergence, gesture-driven animation, and Visualize/Renderer capability boundaries.
- [Narrator Experience](docs/NARRATOR_EXPERIENCE.md) — first-session premise capture and the future conversational guide.
- [Character Casting Studio](docs/CHARACTER_CASTING_STUDIO.md) — narrative-role lanes, revision mirror, and source-context roadmap.
- [Character Creation](docs/CHARACTER_CREATION.md) — defaults, prompts, templates, and editable character profiles.
- [Scene Capture](docs/SCENE_CAPTURE.md) — playing scenes, beat capture, pose sequences, and narration drafts.
- [Vocabulary and Provenance](docs/VOCABULARY_AND_PROVENANCE.md) — synonym usage, attribution, source records, and canon status.
- [Gallery](docs/Gallery.md) — runnable recipes and honest evidence of current behavior.
- [Development Status](docs/DEVELOPMENT_STATUS.md) — implemented scope, open work, and readiness estimate.
- [Branch Reconciliation](docs/BRANCH_RECONCILIATION.md) — the safe route to the canonical branch model.
- [Story Foundation](docs/STORY_FOUNDATION.md) and [Historical Scale](docs/HISTORICAL_SCALE.md) — the initial science-fiction setting and its nested chronology.
- [Diegetic StoryBuilder](docs/DIEGETIC_STORYBUILDER.md) — the in-world tool concept.

## Current boundary

The domain core, FSM_API lifecycle adapter, FSM_COS-composable MicroBundle, versioned binary persistence adapter, Narrator-first Blazor workbench, Casting Studio role lanes and three-sided mirror, and GUI.SVG living map are implemented. The local workspace autosaves to browser localStorage through FSM_Serialization and restores across refreshes. Cloud/server sync, export/backup, embedding into the Workshop's WebPage, full campaign continuity, authorable plural timelines and divergence views, Gestures-backed character animation, physics-backed fight playback, and broader source-derived visualizations remain open. No package publishing or release has been authorized.

## License

License selection is pending.
