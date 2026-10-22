# Development Status

**Status snapshot:** 2026-10-21  
**Readiness estimate:** 20% — initial domain foundation only; not application-ready.

## Implemented in development source

- Dependency-free .NET 8 domain core.
- CharacterProfile and low-friction CharacterBuilder templates.
- SceneRecord and ordered SceneBeat capture modes, including PoseSequence and Claymation labels.
- SceneComposer that produces an editable narration draft from recorded beats.
- VocabularyIndex with explicit synonym mapping, surface-word preservation, usage counts, and attribution.
- StoryWorkspace records for the Experience definition, design decisions, sources, historical claims, events, campaigns, participant actions, and continuity issues.
- Initial unit tests for character drafts, synonym search, scene narration/reordering, and workspace identity/boundaries.
- Source companion Markdown pages and focused UML/use-case diagrams for the maintained core types.

## Not yet verified or implemented

- The .NET test suite has been authored but has not been executed in this environment; CI must provide the first build/test evidence.
- No clickable authoring UI, WebApp/WebPage integration, or GUI/GUI.SVG living view.
- No persistent repository, import/export adapter, or FSM_Serialization integration.
- No FSM_API workflow adapter or FSM_COS manifest/composition integration.
- No interactive fight simulation, live claymation playback, or physics integration.
- No automated adjective extraction or language-aware semantic analysis.
- No release or package publishing.

## Readiness rubric

The estimate reflects five equal workstreams: (1) domain contracts, (2) automated verification, (3) durable storage and provenance, (4) user-facing authoring, and (5) Workshop composition/visual integration. Only the initial portion of the first workstream is present; the remaining workstreams are open. The percentage is an engineering planning estimate, not a test or quality score.

## Immediate next steps

1. Get CI building and testing the current core.
2. Add a runnable quickstart sample and improve model validation based on test results.
3. Inspect current FSM_Serialization, FSM_API, GUI, and FSM_COS contracts before implementing adapters.
4. Deliver the first real Casting interface that creates and edits a CharacterProfile.
5. Connect vocabulary indexing to workspace edits and visualize actual usage data.
6. Add scene beat editing and a real pose-sequence view.
