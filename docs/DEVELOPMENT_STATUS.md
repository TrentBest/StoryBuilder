# Development Status

**Status snapshot:** 2026-10-21  
**Readiness estimate:** 40% — working local authoring prototype with tested domain and composition contracts; not release-ready.

## Implemented in development source

- Dependency-free .NET 8 domain core.
- CharacterProfile and low-friction CharacterBuilder templates.
- Interactive **Casting** page: create, select, revise, and remove character traits; attribute each trait; explicitly map synonyms; search usage across the story domain.
- FSM_API-backed review actions for character drafts: propose, accept, reject, reopen for revision, and deprecate.
- SceneRecord and ordered SceneBeat capture modes, including Played, BeatByBeat, PoseSequence, and Claymation labels.
- Interactive **The Stage** page: capture action, reaction, dialogue, pose, tone, and notes; reorder/remove beats; generate and copy an editable narration draft.
- Interactive **World Archive** page: record historical claims with optional source provenance and explicit canon status.
- Interactive **Campaign Room**: create campaigns, record participant choices, and accept/reject/reopen/deprecate them through the FSM_API lifecycle.
- VocabularyIndex with explicit synonym mapping, surface-word preservation, counts, attribution, text tokenization, and re-indexing of existing occurrences when synonym mappings change.
- StoryBuilderSession coordinates workspace edits and refreshes indexed names, roles, descriptions, traits, relationships, notes, scene metadata, beat text, claims, events, sources, campaigns, participant actions, and continuity notes.
- StoryWorkspace records for the Experience definition, design decisions, sources, historical claims, events, campaigns, participant actions, and continuity issues.
- Optional StoryBuilder.FSM adapter using FSM_API and a scoped StoryLifecycleRegistry.
- StoryBuilder.MicroBundle exposes workspace, Casting, Stage, vocabulary, history, and campaign providers through MicroBundleDomain.
- Automated test composes StoryBuilderAuthoringBundle through FSM_COS.
- Runnable console quickstart and interactive Blazor WebDemo.
- Source companion Markdown pages and focused UML/use-case diagrams for maintained types.
- GitHub Actions builds the core, both adapters, both samples, and runs the tests. Checkout action is pinned to an immutable commit SHA.

## Verification evidence

- [Workflow run 39700210176](https://github.com/TrentBest/StoryBuilder/actions/runs/39700210176) succeeded for the domain core, FSM_API adapter, MicroBundle adapter, console quickstart, interactive WebDemo, and automated tests.
- That successful run used commit aaabdbe91e818ad766fb6773e8817726c7a2623a. Subsequent commits through this status update are documentation changes; see the latest [development workflow runs](https://github.com/TrentBest/StoryBuilder/actions/workflows/dotnet.yml?query=branch%3Adevelopment) for the latest branch-head result.
- No package has been published, no release has been created, and no deployment has occurred.

## Not yet implemented

- Durable storage, import/export, migration handling, or FSM_Serialization integration.
- Integration into the Workshop's WebPage host.
- Live GUI.SVG manifestations generated from the same workspace data.
- Browser-level automated tests for user interactions.
- Physics-backed fight simulation or a visual pose/claymation timeline.
- Full campaign continuity resolution and source-to-event reconciliation.
- Automatic adjective classification, multilingual stemming, or semantic inference.
- Multi-user collaboration and concurrent editing.

## Readiness rubric

The estimate reflects five equal workstreams: (1) domain contracts, (2) automated verification, (3) durable storage and provenance, (4) user-facing authoring, and (5) Workshop composition/visual integration. The domain, CI, and local authoring slices now work together, and the MicroBundle contract is tested through FSM_COS. Persistence and integration into the actual Workshop host remain absent; this is a useful prototype, not a finished or release-ready product. The percentage is an engineering planning estimate, not a test or quality score.

## Immediate next steps

1. Inspect FSM_Serialization's current contracts and add a round-trippable persistence adapter with migration tests.
2. Mount StoryBuilderAuthoringBundle in the Workshop WebPage host through the existing FSM_COS composition path.
3. Add GUI.SVG living views that reflect real characters, trait usage, scene beats, and historical chronology.
4. Add browser-level tests for create/edit/review flows and verify the UI is keyboard accessible.
5. Expand campaign actions into a continuity workflow that links participant choices to events and historical consequences.
