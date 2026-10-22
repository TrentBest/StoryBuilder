# StoryBuilder Architecture

## Ownership

StoryBuilder owns the authoring domain: Experience design records, character profiles, world claims, events, scenes, campaigns, participant actions, source provenance, continuity issues, and vocabulary usage.

It does not own host lifecycle, durable storage, GUI rendering, physics simulation, or the general composition kernel.

## Boundary map

```text
Author / participant
        |
        v
Interactive StoryBuilder WebDemo (local proving ground)
        |
        +---- future Workshop WebPage host
        |
        +---- future GUI / GUI.SVG living manifestations
        |
        v
StoryBuilder.MicroBundle
        |
        v
StoryBuilder.Core (host-neutral domain)
  - character and scene authoring
  - provenance and canon status
  - vocabulary and synonym index
  - campaign and continuity records
        |
        +---- StoryBuilder.FSM (optional FSM_API lifecycle adapter)
        |
        +---- future serialization adapter
        |
        v
FSM_COS composes MicroBundles; the host owns persistence and presentation
```

The MicroBundle adapter depends on the neutral MicroBundleDomain contract rather than FSM_COS itself. The automated composition test resolves and loads StoryBuilderAuthoringBundle through FSM_COS. The local Blazor workbench creates its session through the same bundle factory, but is not yet hosted by WebPage.

## Core invariants

1. **A sketch is useful.** Required information is limited to what a workflow truly needs; defaults and templates are suggestions, not hidden facts.
2. **Identity survives editing.** Stable record IDs let scenes, relationships, sources, and campaigns refer to records while their descriptions change.
3. **Wording survives normalization.** Synonym search may group concepts, but occurrences retain their surface form and attribution.
4. **Claims have provenance.** A statement, the source that made it, and the truth status assigned by the author are distinct.
5. **Build record and fictional canon are separate.** An implementation dependency is not a world fact; a fictional technology is not an implemented capability.
6. **Participation is not predetermined.** Participant actions are recorded as proposals until deliberately accepted.
7. **Presentation is an adapter.** The domain does not depend on WPF, Blazor, SVG, a browser, or a game engine.
8. **A timeline is composable execution.** A timeline has a stable identity and a semantic Process Group definition; a compatible host driver can insert and update it through an adapter without becoming coupled to StoryBuilder's domain.
9. **Published editions are immutable evidence.** Future stories may cite or retell their events, but cannot silently change a published content artifact in place.

## Integration plan

- **FSM_API:** implemented in the optional StoryBuilder.FSM adapter for the draft/proposed/accepted/rejected/deprecated review lifecycle. StoryBuilder.Core remains independent of FSM_API.
- **MicroBundleDomain / FSM_COS:** StoryBuilder.MicroBundle implements the domain-owned MicroBundle contract and declares workspace, Casting, Stage, vocabulary, history, and campaign providers. An automated composition test uses FSM_COS to load it.
- **FSM_Serialization:** implemented in StoryBuilder.Persistence as an explicit, bounded, versioned binary workspace document. The WebDemo stores that document in browser localStorage; StoryBuilder.Core remains independent of the serialization package.
- **GUI / GUI.SVG:** the WebDemo references the source-pinned GUI.SVG project and renders a live SVG manifestation of current StoryBuilderSession data. The domain core remains GUI-independent; GUI.SVG is an optional manifestation adapter.
- **WebApp / WebPage:** the local Blazor host proves authoring interactions; the next integration is to mount the StoryBuilder Experience in the Workshop host without duplicating domain logic.
- **Physics / Visualize / Renderer / Gestures:** optional providers for simulation, scene inspection, portrayal, and animation—not required dependencies of the domain model.

## Timeline-first architecture

Timelines are the first major next capability. Each timeline is a first-class semantic Process Group: it owns or references the events, temporal and causal relationships, observer knowledge, branch lineage, and recounting links belonging to that history. A runtime adapter maps this domain definition onto the available Process Group/driver contract; the host inserts the group and calls its update. StoryBuilder.Core must not take a direct dependency on FSM_COS or a concrete driver.

The timeline console should be summonable by a configurable keyboard shortcut, with an accessible visible control as a non-keyboard path. Timeline event numbers are stable within a view only; stable event IDs are the durable references. Colored boxes/rails associate events with books, chapters, scenes, passages, and publication editions, with bidirectional selection and highlighting.

One event can have exact repetition or multiple new recountings. An added camera/NPC observer can produce a new viewpoint draft while the event remains the same; the author must explicitly resolve impossible knowledge, contradictions, or proposed canon changes. A published edition locks its exact content artifact and provenance. Corrections create a new edition; sequels and prequels reference the immutable source and add new material rather than modifying it in place. The lock does not prevent new universe events or new linked interpretations.

A Process Group update must not implicitly rewrite canon or immutable published content. Runtime proposals, accepted world history, and published edition snapshots are distinct. See [Timeline Process Groups and Immutable Story Editions](TIMELINE_PROCESS_GROUPS.md) and [Timelines, Gestures, and Multiverse History](TIMELINES_GESTURES_MULTIVERSE.md) for requirements and acceptance scenarios.

## Provider and runtime boundary

Discover actual FSM/API Process Group support and optional Gestures, Visualize, Renderer, GUI.SVG, and future Physics capabilities through the host-provided neutral MicroBundle catalog. FSM_COS composes bundles at the host boundary; StoryBuilder domain code must not directly depend on FSM_COS or presentation/simulation packages. If a provider is absent, timeline records and authored relationships remain usable, with truthful fallbacks.

## Current implementation boundary

The .NET 8 core provides character drafts and templates, structured scene beats, a narration draft, automatic indexing of character and scene words through StoryBuilderSession, explicit synonym mapping, usage search, and a workspace record model. Optional adapters demonstrate FSM_API lifecycle behavior and the FSM_COS MicroBundle contract. The local Blazor workbench has clickable Casting, Stage, Archive, and Campaign pages. Browser-local binary persistence is implemented and round-trip tested. Timeline Process Groups, the timeline console, observer-aware recounting, immutable publication editions, cloud/server persistence, Workshop-host integration, export/backup and future schema migrations, and broader living GUI.SVG views remain open.
