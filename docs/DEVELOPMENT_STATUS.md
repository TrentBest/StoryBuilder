# Development Status

**Status snapshot:** 2026-10-22  
**Readiness estimate:** 60% — verified local authoring prototype with versioned binary browser persistence, living GUI.SVG views, a persisted book preview, provisional timeline planning points, timeline-assigned world events, event-to-manuscript links, and an observer-knowledge evaluator that fails closed when knowledge is unsupported. Renderer integration, full reader disclosure, package production/consumption, and publication remain incomplete. Not release-ready.

## Implemented in development source

- Dependency-free .NET 8 domain core.
- CharacterProfile and low-friction CharacterBuilder templates.
- Interactive Casting page for character drafts, traits, synonyms, and usage search.
- FSM_API-backed review actions for character drafts.
- SceneRecord and ordered SceneBeat capture modes.
- Interactive Stage page for action, reaction, dialogue, pose, tone, notes, and editable narration drafts.
- World Archive for historical claims, optional source provenance, and canon status.
- Campaign Room for participant choices and lifecycle review.
- Living Map: pinned GUI.SVG source renders live character cards, review status, traits, workspace counts, and vocabulary; tests verify changes from real domain data.
- Timeline map: root/branch creation plus GUI.SVG markers for provisional planning points and world events assigned to each timeline. Placement is currently a descriptive approximate/unresolved label, not a measured time axis or causal graph.
- VocabularyIndex with explicit synonym mapping, surface-word preservation, attribution, and re-indexing.
- StoryBuilderSession coordinates workspace edits and creates canonical timeline definitions and branch references.
- StoryWorkspace records for Experience, timelines, planning points, timeline-linked world events, manuscripts, event-to-section portrayal references, and observer-knowledge records, sources, claims, campaigns, participant actions, and continuity issues.
- Host-neutral IStoryProcessGroup and StoryProcessGroupDriver; each inserted group is updated once per driver Update.
- Optional FSM_API-backed TimelineProcessGroup with inspectable entry, driver-update, runtime-tick, and exit observations.
- Binary workspace format version 7 persists timeline definitions (v3), manuscripts (v4), provisional planning points (v5), timeline assignment/placement for world events (v6), and observer-knowledge records (v7). Readers retain compatibility with versions 1 through 6.
- Optional StoryBuilder.FSM lifecycle adapter and MicroBundleDomain bundle.
- FSM_Serialization-backed bounded binary workspace persistence, browser-local autosave, and round-trip/error tests.
- Automated composition test loads StoryBuilderAuthoringBundle through FSM_COS.
- Runnable console quickstart and interactive Blazor WebDemo.
- GUI.SVG source pinned as a submodule; CI builds source rather than relying on an unpublished package.
- Companion Markdown pages and focused UML/use-case diagrams for maintained types.
- GitHub Actions builds projects and runs tests; checkout action is pinned to an immutable commit SHA.

## Architecture contracts added

- [Composition Package Contract](COMPOSITION_PACKAGE_CONTRACT.md): intended manifest/configuration boundary between Workshop arbitration and AnyApp consumption; package compiler/consumer is not implemented yet.
- [Timeline Authoring and Observer Model](TIMELINE_AUTHORING_MODEL.md): provisional planning points, observer knowledge/disclosure, multi-FSM Narrator, and product-first views are specified as contracts, not claimed features.
- [Architecture](ARCHITECTURE.md) now makes book-first composition and the package boundary explicit.

## Verification evidence

- [Workflow run 39702171854](https://github.com/TrentBest/StoryBuilder/actions/runs/39702171854) succeeded for the established core, adapters, binary persistence, samples, and tests on commit ffa4dd67de94d09bea2d517458ac6cdcdcd6cabc; baseline only.
- [Timeline foundation workflow run 39705563323](https://github.com/TrentBest/StoryBuilder/actions/runs/39705563323) succeeded on commit abe19472f935508aa1a6b62f8f3eb13143341cbe.
- [GUI.SVG timeline map workflow run 39706155385](https://github.com/TrentBest/StoryBuilder/actions/runs/39706155385) succeeded on commit 41292fba32eb2238688508d7773004675a8d51d5.
- [Book preview and manuscript persistence workflow run 39707924820](https://github.com/TrentBest/StoryBuilder/actions/runs/39707924820) succeeded on commit c60224d09123e1db218ad8d7b43e31852b734c88.
- [Planning points workflow run 39708209120](https://github.com/TrentBest/StoryBuilder/actions/runs/39708209120) succeeded on commit fa23cff2b89fd1b3017974df1a707b4bf41c1534.
- [Timeline events and manuscript portrayal links workflow run 39708477845](https://github.com/TrentBest/StoryBuilder/actions/runs/39708477845) succeeded on commit fdde93994809ecffb6ebbb633a9eb72fcd20f4e0.
- [Observer knowledge constraint workflow run 39708952020](https://github.com/TrentBest/StoryBuilder/actions/runs/39708952020) succeeded on commit 93cc9a4ebc43a2727aa76a6eb935ae9cea106bfa.
- No package has been published, no release has been created, and no deployment has occurred.

## Not yet implemented

- Book Preview supports manuscript metadata, ordered sections, and actual authored text. Export, immutable publication editions, and links from manuscript sections to scene/beat records remain incomplete.
- Richer Plot Twist brainstorming, alternatives editing, impact links, and promotion to events only by explicit author decision; the current increment creates a stable provisional point with approximate placement.
- Full reader disclosure policy, observer-facing UI, renderer integration, full causal graph editing, synchronized timeline comparison, and unrevealed-event handling remain incomplete.
- Keyboard timeline console, visual relationship rails from event markers to book sections, scene/beat navigation, and observer-aware recounting.
- Narrator first-session workflow orchestrated by multiple focused FSM_API state machines.
- Immutable published edition artifacts and corrected-edition lineage.
- Workshop arbitration output: versioned composition manifest, per-MicroBundle configuration, validation, and AnyApp package consumption without repeat arbitration.
- Cloud/server persistence, import/export, backup, and schema migration tooling.
- Workshop WebPage host integration and browser-level interaction/accessibility automation.
- Full Casting Studio reference observability and continuity impact preview.
- Gestures animation authoring/preview, optional Visualize/Renderer provider discovery, and Physics-backed simulation.
- Multi-user collaboration and concurrent editing.

## Readiness rubric

The estimate reflects five equal workstreams: domain contracts, automated verification, durable storage/provenance, user-facing authoring, and Workshop composition/visual integration. Verified work now includes a persisted manuscript product, provisional planning points, timeline-assigned world events, event-to-section references, and a tested observer-knowledge evaluator. Multi-FSM Narrator orchestration, renderer integration, package compiler/consumer, and Workshop-host integration remain absent. This is a planning estimate, not a test or quality score.

## Immediate next steps

1. Verify timeline-linked events, manuscript portrayal links, and binary version 6 compatibility.
2. Add first-class provisional timeline planning points so Plot Twist can exist before its meaning is known.
3. Connect events to scene beats and manuscript passages through stable IDs; distinguish chronology, causality, portrayal, and disclosure in GUI.SVG.
4. Implement observer knowledge/disclosure contracts and tests that prevent unsupported narration.
5. Implement a small set of cooperating FSM_API Narrator state machines with explicit contracts.
6. Implement manifest and per-MicroBundle configuration production, then prove AnyApp consumption avoids repeated arbitration.
7. Add immutable publication editions and Workshop WebPage host integration.
8. Continue optional Gestures, Visualize, Renderer, and Physics integration through host-discovered capabilities.
