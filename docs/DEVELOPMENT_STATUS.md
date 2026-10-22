# Development Status

**Status snapshot:** 2026-10-21  
**Readiness estimate:** 30% — tested domain and composition foundation; not application-ready.

## Implemented in development source

- Dependency-free .NET 8 domain core.
- CharacterProfile and low-friction CharacterBuilder templates.
- SceneRecord and ordered SceneBeat capture modes, including PoseSequence and Claymation labels.
- SceneComposer that produces an editable narration draft from recorded beats.
- VocabularyIndex with explicit synonym mapping, surface-word preservation, usage counts, attribution, text tokenization, and re-indexing of existing occurrences when synonym mappings change.
- StoryBuilderSession coordinates workspace edits and refreshes indexed names, roles, descriptions, traits, scene metadata, and beat text when characters or scenes are edited.
- StoryWorkspace records for the Experience definition, design decisions, sources, historical claims, events, campaigns, participant actions, and continuity issues.
- Optional StoryBuilder.FSM adapter using FSM_API for draft/proposed/accepted/rejected/deprecated review transitions.
- StoryBuilder.MicroBundle exposes workspace, Casting, Stage, vocabulary, history, and campaign providers through the neutral MicroBundleDomain contract.
- Runnable console quickstart demonstrating character, vocabulary, and scene workflows.
- Unit tests for character drafts, synonym search and later synonym edits, session-driven re-indexing, scene narration/reordering, workspace identity/boundaries, FSM lifecycle transitions, and FSM_COS composition.
- Source companion Markdown pages and focused UML/use-case diagrams for maintained types.
- GitHub Actions builds the core, both adapters, and quickstart, then runs the test suite. Checkout action is pinned to an immutable commit SHA.

## Verification evidence

- [Workflow run 39698910732](https://github.com/TrentBest/StoryBuilder/actions/runs/39698910732) succeeded: domain core build, FSM_API adapter build, MicroBundle adapter build, quickstart build, and automated tests including the FSM_COS composition test.
- That successful run used commit `48a8798351f092a679a2e4cfb089575db455aafb`. Subsequent commits through this status update are documentation changes; check the latest [development workflow runs](https://github.com/TrentBest/StoryBuilder/actions/workflows/dotnet.yml?query=branch%3Adevelopment) for the latest branch-head run.
- No package has been published and no release has been created.

## Not yet implemented

- No clickable authoring UI, WebApp/WebPage integration, or GUI/GUI.SVG living view.
- No persistent repository, import/export adapter, or FSM_Serialization integration.
- No complete campaign runtime or source-to-event continuity reconciliation.
- No interactive fight simulation or live claymation playback.
- No automatic adjective classification, multilingual stemming, or semantic inference.
- No full source-derived living diagram pipeline.

## Readiness rubric

The estimate reflects five equal workstreams: (1) domain contracts, (2) automated verification, (3) durable storage and provenance, (4) user-facing authoring, and (5) Workshop composition/visual integration. The first two have a passing vertical slice; the MicroBundle composition boundary is also verified. Persistence and the user-facing Experience are still absent, so this is foundation progress rather than application readiness. The percentage is an engineering planning estimate, not a test or quality score.

## Immediate next steps

1. Build the first clickable Casting interface and bind it to StoryBuilderSession.
2. Add durable storage through the inspected FSM_Serialization contract, with round-trip and migration tests.
3. Make vocabulary indexing incremental and explicit about word category, attribution, and source.
4. Build a real Stage beat editor and pose-sequence view; keep generated narration editable and traceable to beats.
5. Integrate the StoryBuilder MicroBundle into the Workshop host and generate living GUI.SVG views from the same domain data.
6. Add campaign participant actions and continuity resolution as a tested vertical slice.
