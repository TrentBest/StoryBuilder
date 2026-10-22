# Development Status

**Status snapshot:** 2026-10-21  
**Readiness estimate:** 25% — tested domain foundation and first optional stack adapter; not application-ready.

## Implemented in development source

- Dependency-free .NET 8 domain core.
- CharacterProfile and low-friction CharacterBuilder templates.
- SceneRecord and ordered SceneBeat capture modes, including PoseSequence and Claymation labels.
- SceneComposer that produces an editable narration draft from recorded beats.
- VocabularyIndex with explicit synonym mapping, surface-word preservation, usage counts, and attribution.
- Vocabulary mappings reconcile existing occurrences when authors add synonyms later.
- StoryWorkspace records for the Experience definition, design decisions, sources, historical claims, events, campaigns, participant actions, and continuity issues.
- Optional StoryBuilder.FSM adapter using FSM_API for draft/proposed/accepted/rejected/deprecated review transitions.
- Runnable console quickstart demonstrating character, vocabulary, and scene workflows.
- Unit tests for character drafts, synonym search and later synonym edits, scene narration/reordering, workspace identity/boundaries, and FSM lifecycle transitions.
- Source companion Markdown pages and focused UML/use-case diagrams for maintained types.
- GitHub Actions builds the core, FSM_API adapter, and quickstart, then runs the test suite. Checkout action is pinned to an immutable commit SHA.

## Verification evidence

- [Workflow run 39697488409](https://github.com/TrentBest/StoryBuilder/actions/runs/39697488409) succeeded for the core, quickstart, and tests before the FSM adapter was added.
- The current branch triggers a fresh run for the adapter and lifecycle tests; check the latest [development workflow runs](https://github.com/TrentBest/StoryBuilder/actions/workflows/dotnet.yml?query=branch%3Adevelopment) before treating the newest commit as verified.
- No package has been published and no release has been created.

## Not yet implemented or verified

- No clickable authoring UI, WebApp/WebPage integration, or GUI/GUI.SVG living view.
- No persistent repository, import/export adapter, or FSM_Serialization integration.
- No FSM_COS composition adapter or MicroBundle packaging.
- No interactive fight simulation, live claymation playback, or physics integration.
- No automatic adjective extraction or language-aware semantic analysis.
- No full campaign runtime or source-to-event continuity reconciliation.

## Readiness rubric

The estimate reflects five equal workstreams: (1) domain contracts, (2) automated verification, (3) durable storage and provenance, (4) user-facing authoring, and (5) Workshop composition/visual integration. Domain contracts and the first verification path are underway; persistence and user-facing authoring remain open, and stack integration is only beginning. The percentage is an engineering planning estimate, not a test or quality score.

## Immediate next steps

1. Verify the FSM_API adapter and lifecycle tests in CI.
2. Inspect the current FSM_Serialization contracts and define an explicit portable workspace record contract before adding persistence.
3. Connect workspace edits to vocabulary indexing so counts update from real character and scene records.
4. Deliver a real Casting interface that creates and edits a CharacterProfile.
5. Add scene beat editing and a real pose-sequence view using the same domain data.
6. Compose the optional capabilities through FSM_COS only after the host/bundle contract is verified.
