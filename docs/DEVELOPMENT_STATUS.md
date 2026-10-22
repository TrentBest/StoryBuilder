# Development Status

**Status snapshot:** 2026-10-22  
**Readiness estimate:** 70% — verified local authoring prototype with versioned binary browser persistence, a persisted book preview, living GUI.SVG timeline markers, timeline-linked world events, event-to-manuscript links, a conservative observer-knowledge evaluator, a five-FSM Narrator agent, and an accepted-knowledge projection for downstream portrayal. BookProductRenderer now produces deterministic Markdown book output from the canonical manuscript; repeated package consumption preserves the same output and arbitration-record identity. Scene beats can reference canonical world-event IDs; Stage exposes event-link controls and the timeline renderer draws a separate event-to-scene-beat rail. The persistence, renderer, and Stage increment passed CI. Manifest/configuration contracts, host-capability validation, binary package compilation/reading, integrity checks, and package round-trip/tamper tests have passed CI. The GUI.SVG timeline now draws numbered event markers and a portrayal rail linking each event to the actual manuscript sections that reference it; this data-derived view passed CI. Actual AnyApp host integration, LLM-backed drafting, Renderer integration, reader disclosure, and publication remain incomplete. Not release-ready.

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
- Timeline map: root/branch creation plus GUI.SVG markers for provisional planning points and world events assigned to each timeline. A new portrayal rail maps linked events to manuscript/book sections; chronology remains approximate/unresolved until a richer temporal model is implemented.
- VocabularyIndex with explicit synonym mapping, surface-form preservation, attribution, and re-indexing.
- StoryBuilderSession coordinates workspace edits and creates canonical timeline definitions and branch references.
- StoryWorkspace records for Experience, timelines, planning points, timeline-linked world events, manuscripts, event-to-section and event-to-scene-beat portrayal references, observer-knowledge records, sources, claims, campaigns, participant actions, and continuity issues.
- Host-neutral IStoryProcessGroup and StoryProcessGroupDriver; each inserted group is updated once per driver Update.
- Optional FSM_API-backed TimelineProcessGroup with inspectable entry, driver-update, runtime-tick, and exit observations.
- NarratorAgent: five isolated FSM_API definitions and processing groups for intent, planning, observer constraints, portrayal preparation, and draft/review. Portrayal preparation has its own Requested → Ready / NeedsReview lifecycle; an empty observer projection does not overwrite observer-selection state. Its observer FSM exposes an accepted-knowledge projection that omits unrestricted world-event titles, summaries, and causal edges; the projection service indexes knowledge by event and traverses causal history once per projection. The Home page displays their live states; premise capture uses the intent FSM, timeline planning uses the planning FSM, and draft submission is held for review when declared event claims exceed accepted observer knowledge. The agent records transitions and disposes its instances and definitions.
- Binary workspace format version 8 persists timeline definitions (v3), manuscripts (v4), provisional timeline planning points (v5), timeline assignment/placement for world events (v6), observer-knowledge records (v7), and scene-beat event links (v8). Readers retain compatibility with versions 1 through 7.
- Host-neutral composition manifest/configuration contracts and CompositionPackageValidator: checks package/workspace identity, content revision, SHA-256 inputs, bundle configuration, required host capabilities, versions, and duplicate identifiers without rerunning arbitration.
- CompositionPackageCompiler and CompositionPackageReader: emits binary manifest, per-MicroBundle configuration, and content payloads; hashes exact content/configuration bytes and rejects invalid packages before restoring a workspace. Manifest schema version 2 captures explicit MicroBundle dependencies; dependency-closure, round-trip, tamper, and missing-required-bundle tests passed CI.
- Optional StoryBuilder.FSM lifecycle adapter and MicroBundleDomain bundle.
- StoryBuilderAuthoringController provides explicit, host-neutral command IDs for creating timelines, characters, scenes, and manuscripts. Unsupported commands are rejected without mutation; AnyApp control binding and end-to-end desktop verification remain incomplete.
- FSM_Serialization-backed bounded binary workspace persistence, browser-local autosave, and round-trip/error tests.
- Automated composition test loads StoryBuilderAuthoringBundle through FSM_COS.
- BookProductRenderer emits a deterministic Markdown digital-book artifact with ordered sections, normalized line endings, and stable referenced-event metadata. The Book Preview exposes this output directly.
- Runnable console quickstart and interactive Blazor WebDemo.
- GUI.SVG source pinned as a submodule; CI builds source rather than relying on an unpublished package.
- Companion Markdown pages and focused UML/use-case diagrams for maintained types.
- GitHub Actions builds projects and tests; checkout action is pinned to an immutable commit SHA.

## Architecture contracts added

- [Composition Package Contract](COMPOSITION_PACKAGE_CONTRACT.md): manifest/configuration records, capability validator, binary artifact compiler/reader, and integrity boundary are implemented; actual AnyApp host integration remains future work.
- [Timeline Authoring and Observer Model](TIMELINE_AUTHORING_MODEL.md): provisional planning points, observer knowledge/disclosure, multi-FSM Narrator, and product-first views are specified as contracts, not claimed features.
- [Architecture](ARCHITECTURE.md) makes book-first composition and the package boundary explicit.

## Verification evidence

- [Workflow run 39702171854](https://github.com/TrentBest/StoryBuilder/actions/runs/39702171854) succeeded for the established core, adapters, binary persistence, samples, and tests on commit ffa4dd67de94d09bea2d517458ac6cdcdcd6cabc; baseline only.
- [Timeline foundation workflow run 39705563323](https://github.com/TrentBest/StoryBuilder/actions/runs/39705563323) succeeded on commit abe19472f935508aa1a6b62f8f3eb13143341cbe.
- [GUI.SVG timeline map workflow run 39706155385](https://github.com/TrentBest/StoryBuilder/actions/runs/39706155385) succeeded on commit 41292fba32eb2238688508d7773004675a8d51d5.
- [Book preview and manuscript persistence workflow run 39707924820](https://github.com/TrentBest/StoryBuilder/actions/runs/39707924820) succeeded on commit c60224d09123e1db218ad8d7b43e31852b734c88.
- [Planning points workflow run 39708209120](https://github.com/TrentBest/StoryBuilder/actions/runs/39708209120) succeeded on commit fa23cff2b89fd1b3017974df1a707b4bf41c1534.
- [Timeline events and manuscript portrayal links workflow run 39708477845](https://github.com/TrentBest/StoryBuilder/actions/runs/39708477845) succeeded on commit fdde93994809ecffb6ebbb633a9eb72fcd20f4e0.
- [Observer knowledge constraint workflow run 39708952020](https://github.com/TrentBest/StoryBuilder/actions/runs/39708952020) succeeded on commit 93cc9a4ebc43a2727aa76a6eb935ae9cea106bfa.
- [Multi-FSM Narrator workflow run 39709886977](https://github.com/TrentBest/StoryBuilder/actions/runs/39709886977) succeeded on commit 5ae7c1c461d0fe445e989d1bc6382cb39b9fdaf1.
- [Package contract/validator workflow run 39710671114](https://github.com/TrentBest/StoryBuilder/actions/runs/39710671114) succeeded on commit b8b6da7970bb4cad47e6379f302b5cbbdd939a00.
- [Binary package compiler/reader workflow run 39710865443](https://github.com/TrentBest/StoryBuilder/actions/runs/39710865443) succeeded on commit e0cc03494cc6418370f61f9f16d2c81842219e5f.
- [Manifest dependency-closure workflow run 39711546281](https://github.com/TrentBest/StoryBuilder/actions/runs/39711546281) succeeded on commit f3c2b0a20946cf5b8cb2d273f2941b72c9574f80.
- [Numbered timeline portrayal rail workflow run 39711644001](https://github.com/TrentBest/StoryBuilder/actions/runs/39711644001) succeeded on commit 266dd4a35d2e0088b0430834f327bc99b9a5592b.
- [Observer projection and Narrator FSM workflow run 39712401525](https://github.com/TrentBest/StoryBuilder/actions/runs/39712401525) succeeded on commit 6e2df6c274c9789f5495e14b1cec10f21fe6372.
- [Separate portrayal FSM workflow run 39715158063](https://github.com/TrentBest/StoryBuilder/actions/runs/39715158063) succeeded on commit bbe2cdb0eddb6a1f4467a9473ee9eb814193b812.
- [Indexed observer projection workflow run 39712572636](https://github.com/TrentBest/StoryBuilder/actions/runs/39712572636) succeeded on commit 3465455479df9eb95e419a5124b6efbfbfc3901c.
- [GUI.SVG observer-lens view workflow run 39712919016](https://github.com/TrentBest/StoryBuilder/actions/runs/39712919016) succeeded on commit 5ecbb45f5b1533f4266d66e2494775453c7e2cdb.
- [Scene-beat event-link persistence workflow run 39713533768](https://github.com/TrentBest/StoryBuilder/actions/runs/39713533768) succeeded on commit d001104fcbfd7248e8f28566c5648366018dec30.
- [Scene-beat portrayal rail and Stage controls workflow run 39713997378](https://github.com/TrentBest/StoryBuilder/actions/runs/39713997378) succeeded on commit 17d8b58dea20ad1efc6d6214d06ec8fab4542923.
- [Deterministic book artifact and repeated package-consumption workflow run 39714658993](https://github.com/TrentBest/StoryBuilder/actions/runs/39714658993) succeeded on commit a507f4bc8c8201c76a147d101072cebdb513db2b.
- No package has been published, no release has been created, and no deployment has occurred.

## Not yet implemented

- Book Preview supports manuscript metadata, ordered sections, and actual authored text. Export, immutable publication editions, and links from manuscript sections to scene/beat records remain incomplete.
- Richer Plot Twist brainstorming, alternatives editing, impact links, and promotion to events only by explicit author decision; the current increment creates a stable provisional point with approximate placement.
- Full reader disclosure policy, renderer integration, full causal graph editing, synchronized timeline comparison, and unrevealed-event handling remain incomplete.
- Keyboard timeline console, scene/beat navigation, and observer-aware recounting.
- LLM-backed drafting provider, Renderer adapter, and broader continuity/composition/package FSM responsibilities beyond the initial five machines.
- Immutable published edition artifacts and corrected-edition lineage.
- End-to-end AnyApp integration and deterministic repeated-consumption proof against the real host.
- Cloud/server persistence, import/export, backup, and schema migration tooling.
- Workshop WebPage host integration and browser-level interaction/accessibility automation.
- Full Casting Studio reference observability and continuity impact preview.
- Gestures animation authoring/preview, optional Visualize/Renderer provider discovery, and Physics-backed simulation.
- Multi-user collaboration and concurrent editing.

## Readiness rubric

The estimate reflects five equal workstreams: domain contracts, automated verification, durable storage/provenance, user-facing authoring, and Workshop composition/visual integration. Verified work includes a persisted manuscript product, provisional planning points, timeline-assigned world events, event-to-section references, a tested observer-knowledge evaluator, five cooperating FSM_API Narrator machines, the observer-safe projection, deterministic digital book output, repeated package-consumption test, and the binary package compiler/reader with integrity tests. Actual AnyApp integration, Renderer integration, and Workshop-host integration remain absent. The numbered book portrayal rail, manifest dependency closure, scene-beat event-link persistence, scene-beat rail, and Stage controls are verified in CI. This is a planning estimate, not a test or quality score.

## Immediate next steps

1. Expose the observer-safe projection in the WebDemo and use GUI.SVG to inspect which accepted claims are available at a chosen anchor.
2. Add scene/beat-level event links so timeline events can navigate deeper than a manuscript section.
3. Add a host adapter proving AnyApp validates and consumes the package without rerunning arbitration.
4. Connect StoryBuilder's accepted-knowledge constraints with Renderer-backed representations through a host-neutral portrayal contract; keep narrative knowledge separate from visual/computational detail selection.
5. Add richer Plot Twist alternatives, consequence/impact links, and explicit promotion to world events.
6. Add immutable publication editions and corrected-edition lineage.
7. Integrate StoryBuilder into the Workshop WebPage host.
8. Continue optional Gestures, Visualize, and Physics integration through host-discovered capabilities.
