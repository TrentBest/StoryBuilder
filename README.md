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

- **Casting:** create, select, revise, and add/remove character traits from editable drafts; record stable-ID relationships between characters.
- **Places:** create canonical place records, optionally nest places under parent places, and assign scenes to place IDs without losing free-form location notes.
- **The Stage:** capture ordered action, reaction, dialogue, and pose beats; reorder them and copy an editable narration draft.
- **Vocabulary:** index character and scene words, record explicit synonyms, and refresh counts when an author edits a record.
- **World Archive and Continuity Review:** record historical claims with optional source provenance and explicit canon status; open Continuity Review from the workbench navigation to inspect causal cycles, inconsistent cause/consequence links, and cyclic timeline branches. Findings are advisory and do not edit records.
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

- [Phone Review Guide](docs/PHONE_REVIEW.md) — concise readiness, evidence, and next steps for reviewing this work on a phone.
- [Documentation Index](DOCUMENTATION_INDEX.md) — choose a path through the repository.
- [Legacy Project Migration](docs/LEGACY_PROJECT_MIGRATION.md) — import the first-generation JSON schema while preserving the original source payload across binary saves.\n- [Story Domain Contract](docs/STORY_DOMAIN_CONTRACT.md) — implemented record meanings, canon and observer-knowledge rules, event-to-prose links, persistence boundaries, and Alpha limitations.
- [Runtime and Package Boundaries](docs/ARCHITECTURE.md) — what each package owns, how workspace operations and persistence behave, and current host limitations.
- [WebDemo Guide](samples/StoryBuilder.WebDemo/README.md) — how to run and explore the interactive prototype, including Continuity Review.
- [Continuity Review](samples/StoryBuilder.WebDemo/Components/Pages/Continuity.md) — what the live workspace checks report and what they do not establish.
- [Narrator Experience](docs/NARRATOR_EXPERIENCE.md) — low-friction, visual-first story creation; target output discovery; and the digital publication boundary.
- [Character Casting Studio](docs/CHARACTER_CASTING_STUDIO.md) — cast roles, the three-sided mirror concept, word occurrences, and careful pronoun-reference tracking.
- [Timelines, Gestures, and Multiverse History](docs/TIMELINES_GESTURES_MULTIVERSE.md) — world events distinct from prose, timeline branches and divergence, gesture-driven animation, and Visualize/Renderer capability boundaries.
- [Character Creation](docs/CHARACTER_CREATION.md) — defaults, prompts, templates, and editable character profiles.
- [Scene Capture](docs/SCENE_CAPTURE.md) — playing scenes, beat capture, pose sequences, and narration drafts.
- [Vocabulary and Provenance](docs/VOCABULARY_AND_PROVENANCE.md) — synonym usage, attribution, source records, and canon status.
- [Gallery](docs/Gallery.md) — runnable recipes and honest evidence of current behavior.
- [Development Status](docs/DEVELOPMENT_STATUS.md) — implemented scope and remaining operational gaps.
- [Alpha Release Checklist](docs/ALPHA_RELEASE_CHECKLIST.md) — evidence-based gates for the first public source Alpha.
- [Storycraft Workshop](docs/STORYCRAFT_WORKSHOP.md) — a practical writing course from premise and character through plot, scenes, prose, point of view, theme, revision, genre, and continuity.
- [Story Foundation](docs/STORY_FOUNDATION.md) and [Historical Scale](docs/HISTORICAL_SCALE.md) — the initial science-fiction setting and its nested chronology.
- [Diegetic StoryBuilder](docs/DIEGETIC_STORYBUILDER.md) — the in-world tool concept.

## Current boundary

The domain core, FSM_API lifecycle adapter, FSM_COS-composable MicroBundle, versioned binary persistence adapter, Narrator-first Blazor workbench, Casting Studio role lanes and three-sided mirror, GUI.SVG living map, and host-neutral event-to-manuscript authoring commands are implemented. The local workspace autosaves to browser localStorage through FSM_Serialization and restores across refreshes. Cloud/server sync, export/backup, embedding into the Workshop's WebPage, full campaign continuity, interactive editing and visualization of the full temporal/causal graph and timeline divergence, Gestures-backed character animation, physics-backed fight playback, and broader source-derived visualizations are not yet available. Alpha release preparation is underway; no release has been created and no package has been published. NuGet publishing remains gated on final package validation and explicit approval.

## License

StoryBuilder is licensed under the [MIT License](LICENSE). You may use, modify, distribute, and build on it under the license terms. The aim is to make adoption easy and encourage a community around the tools.
