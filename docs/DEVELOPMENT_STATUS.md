# Development Status

**Status snapshot:** 2026-10-22  
**Readiness estimate:** 56% — working local authoring prototype with binary browser persistence and a live GUI.SVG view; first executable timeline Process Group foundation committed, but its CI verification is pending. Not release-ready.

## Implemented in development source

- Dependency-free .NET 8 domain core.
- CharacterProfile and low-friction CharacterBuilder templates.
- Interactive **Casting** page: create, select, revise, and remove character traits; attribute each trait; explicitly map synonyms; search usage across the story domain.
- FSM_API-backed review actions for character drafts: propose, accept, reject, reopen for revision, and deprecate.
- SceneRecord and ordered SceneBeat capture modes, including Played, BeatByBeat, PoseSequence, and Claymation labels.
- Interactive **The Stage** page: capture action, reaction, dialogue, pose, tone, and notes; reorder/remove beats; generate and copy an editable narration draft.
- Interactive **World Archive** page: record historical claims with optional source provenance and explicit canon status.
- **Campaign Room:** create campaigns, record participant choices, and accept/reject/reopen/deprecate them through the FSM_API lifecycle.
- **Living Map:** the pinned GUI.SVG source project renders a live SVG manifestation of current character cards, review status, traits, workspace counts, and most-used vocabulary. Automated tests verify that the rendered output changes with real domain data.
- VocabularyIndex with explicit synonym mapping, surface-word preservation, counts, attribution, text tokenization, and re-indexing of existing occurrences when synonym mappings change.
- StoryBuilderSession coordinates workspace edits and refreshes indexed names, roles, descriptions, traits, relationships, notes, scene metadata, beat text, claims, events, sources, campaigns, participant actions, and continuity notes; it now also creates canonical timeline definitions and branch references.
- StoryWorkspace records for the Experience definition, timeline definitions, design decisions, sources, historical claims, events, campaigns, participant actions, and continuity issues.
- Host-neutral IStoryProcessGroup contract and StoryProcessGroupDriver: the driver updates each inserted group once per driver Update, without owning a clock or background loop.
- Optional FSM_API-backed TimelineProcessGroup with an isolated processing-group identity and inspectable ProcessGroup entry, driver-update, runtime-tick, and exit observations.
- Timeline definitions and branch lineage persisted in StoryWorkspaceBinaryDocument format version 3; readers retain compatibility with versions 1 and 2.
- Timeline Process Group tests cover independent event streams, driver updates, duplicate identity, workspace branch validation, and binary round-trip.
- Optional StoryBuilder.FSM adapter using FSM_API and a scoped StoryLifecycleRegistry.
- StoryBuilder.MicroBundle exposes workspace, Casting, Stage, vocabulary, history, campaign, and timeline authoring providers through MicroBundleDomain.
- StoryBuilder.Persistence implements a bounded, versioned binary workspace document using FSM_Serialization. Tests cover round-trip, synonym restoration, unknown versions, and truncated payloads.
- The WebDemo autosaves and restores that binary document through browser localStorage; a coordinator test verifies persistence across sessions.
- Automated test composes StoryBuilderAuthoringBundle through FSM_COS.
- Runnable console quickstart and interactive Blazor WebDemo.
- The GUI.SVG source is pinned as a submodule at a specific commit. CI initializes submodules and builds the actual source rather than depending on an unpublished NuGet package.
- Source companion Markdown pages and focused UML/use-case diagrams for maintained types.
- GitHub Actions builds the core, both adapters, persistence, both samples, and runs the tests. Checkout action is pinned to an immutable commit SHA.

## Verification evidence

- [Workflow run 39702171854](https://github.com/TrentBest/StoryBuilder/actions/runs/39702171854) succeeded for the domain core, FSM_API adapter, MicroBundle adapter, binary persistence adapter, console quickstart, interactive WebDemo, and tests including GUI.SVG rendering and browser-local persistence. That run used commit `ffa4dd67de94d09bea2d517458ac6cdcdcd6cabc`; it is a verified baseline, not verification of the new timeline code.
- The timeline foundation adds new build and test paths. Check the [latest development workflow runs](https://github.com/TrentBest/StoryBuilder/actions/workflows/dotnet.yml?query=branch%3Adevelopment) before treating this source as verified. At this status edit, the new commits' workflow runs have been queued; no pass/fail claim is made.
- No package has been published, no release has been created, and no deployment has occurred.

## Not yet implemented

- Narrator's full first-session conversation and proposal/revision loop.
- The full Character Casting Studio: Hero / Neutral / Antagonist / Undecided grouping, three-sided mirror, before/after impact preview, and navigable source context.
- Confirmed versus likely versus unresolved character-reference counts; ambiguous pronouns are not automatically resolved.
- Cloud/server storage, explicit import/export, backup, and migration from future binary schema versions.
- Integration into the Workshop's WebPage host.
- Broader source-derived GUI.SVG views for chronology, relationships, and scene continuity beyond the current living map.
- Browser-level automated tests for user interactions and accessibility.
- Physics-backed fight simulation or a visual pose/claymation timeline.
- Full canonical event timelines with world time distinct from reader order, causal graph editing, synchronized timeline comparison, observer knowledge, and explicit unrevealed events. The first Process Group execution seam exists, but it is not yet a temporal simulation.
- Keyboard-invoked timeline developer console, event-number/book color links, bidirectional scene-to-event navigation, and observer-aware recounting.
- Immutable published edition artifacts, explicit corrected-edition lineage, and publication-aware prequel/sequel continuity checks.
- Gestures-backed character animation and authorable gesture timing/preview.
- Visualize/Renderer provider discovery for interactive scene editing and multi-format portrayal; current GUI.SVG Living Map is not a full world-scene editor.
- Full campaign continuity resolution and source-to-event reconciliation.
- Automatic adjective classification, multilingual stemming, or semantic inference.
- Multi-user collaboration and concurrent editing.

## Readiness rubric

The estimate reflects five equal workstreams: (1) domain contracts, (2) automated verification, (3) durable storage and provenance, (4) user-facing authoring, and (5) Workshop composition/visual integration. The domain, CI, local authoring, binary round-trip, browser-local autosave, and a real GUI.SVG manifestation work together; the MicroBundle contract is tested through FSM_COS. The new timeline vertical slice establishes canonical timeline identities, driver composition, an FSM_API runtime adapter, and binary persistence, but its new tests still need a passing CI result. The Narrator-first workflow, Casting Studio's revision/reference observability, timeline console and recounting model, cloud storage, and embedding into the actual Workshop host remain absent. This is a useful prototype, not a finished or release-ready product. The percentage is an engineering planning estimate, not a test or quality score.

## Immediate next steps

1. **Verify the timeline vertical slice:** get a green workflow for the current branch head; resolve any compile, runtime, or persistence regressions before expanding the feature.
2. Build the keyboard-invoked timeline console over actual Process Group and domain records, with a visible accessible control as a fallback.
3. Add canonical event and causal identities plus bidirectional links between timeline events and book/chapter/scene/passage recountings, with colored relationship indicators and stable event IDs.
4. Add exact-repeat references and observer-constrained new recountings; flag knowledge violations and contradictions rather than silently rewriting canon.
5. Add immutable publication editions and explicit correction/alternate-continuity lineage.
6. Integrate optional Gestures, Visualize, and Renderer through host-discovered MicroBundle capabilities.
7. Continue the Narrator-first workflow and Casting Studio's source-aware revision loop.
8. Mount StoryBuilderAuthoringBundle in the Workshop WebPage host through the existing FSM_COS composition path.
