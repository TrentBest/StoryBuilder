# Development Status

**Status snapshot:** 2026-10-22  
**Readiness estimate:** 62% — verified local authoring prototype with versioned binary browser persistence, a persisted book preview, living GUI.SVG timeline markers, timeline-linked world events, event-to-manuscript links, a conservative observer-knowledge evaluator, and a four-FSM Narrator agent. Host-neutral package manifest/configuration contracts and a host capability validator have been added in the current increment; their CI verification is pending. Binary package production, AnyApp consumption, LLM-backed drafting, Renderer integration, reader disclosure, and publication remain incomplete. Not release-ready.

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
- NarratorAgent: four isolated FSM_API definitions and processing groups for intent, planning, observer constraints, and draft/review. The Home page displays their live states; premise capture uses the intent FSM, timeline planning uses the planning FSM, and draft submission is held for review when declared event claims exceed accepted observer knowledge. The agent records transitions and disposes its instances and definitions.
- Binary workspace format version 7 persists timeline definitions (v3), manuscripts (v4), provisional timeline planning points (v5), timeline assignment/placement for world events (v6), and observer-knowledge records (v7). Readers retain compatibility with versions 1 through 6.
- Host-neutral composition manifest/configuration contracts and CompositionPackageValidator: checks package/workspace identity, content revision, SHA-256 inputs, bundle configuration, required host capabilities, versions, and duplicate identifiers without rerunning arbitration.
- Optional StoryBuilder.FSM lifecycle adapter and MicroBundleDomain bundle.
- FSM_Serialization-backed bounded binary workspace persistence, browser-local autosave, and round-trip/error tests.
- Automated composition test loads StoryBuilderAuthoringBundle through FSM_COS.
- Runnable console quickstart and interactive Blazor WebDemo.
- GUI.SVG source pinned as a submodule; CI builds source rather than relying on an unpublished package.
- Companion Markdown pages and focused UML/use-case diagrams for maintained types.
- GitHub Actions builds projects and runs tests; checkout action is pinned to an immutable commit SHA.

## Architecture contracts added

- [Composition Package Contract](COMPOSITION_PACKAGE_CONTRACT.md): manifest/configuration data contracts and a host capability validator now exist; binary producer/consumer remains future work.
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
- No package has been published, no release has been created, and no deployment has occurred.

## Not yet implemented

- Book Preview supports manuscript metadata, ordered sections, and actual authored text. Export, immutable publication editions, and links from manuscript sections to scene/beat records remain incomplete.
- Richer Plot Twist brainstorming, alternatives editing, impact links, and promotion to events only by explicit author decision; the current increment creates a stable provisional point with approximate placement.
- Full reader disclosure policy, observer-facing UI, renderer integration, full causal graph editing, synchronized timeline comparison, and unrevealed-event handling remain incomplete.
- Keyboard timeline console, visual relationship rails from event markers to book sections, scene/beat navigation, and observer-aware recounting.
- LLM-backed drafting provider, visible observer knowledge editor, Renderer adapter, and broader continuity/composition/package FSM responsibilities beyond the initial four machines.
- Immutable published edition artifacts and corrected-edition lineage.
- Workshop arbitration output: binary manifest/configuration/content artifact production, exact serialized-content hashing, AnyApp package loading, and deterministic repeated-consumption proof.
- Cloud/server persistence, import/export, backup, and schema migration tooling.
- Workshop WebPage host integration and browser-level interaction/accessibility automation.
- Full Casting Studio reference observability and continuity impact preview.
- Gestures animation authoring/preview, optional Visualize/Renderer provider discovery, and Physics-backed simulation.
- Multi-user collaboration and concurrent editing.

## Readiness rubric

The estimate reflects five equal workstreams: domain contracts, automated verification, durable storage/provenance, user-facing authoring, and Workshop composition/visual integration. Verified work includes a persisted manuscript product, provisional planning points, timeline-assigned world events, event-to-section references, a tested observer-knowledge evaluator, and four cooperating FSM_API Narrator machines. Package contracts and validator are awaiting CI; binary package production, Renderer integration, and Workshop-host integration remain absent. This is a planning estimate, not a test or quality score.

## Immediate next steps

1. Verify the package contract and validator in CI; fix any failures.
2. Implement the binary manifest/configuration/content artifact producer using FSM_Serialization and hash the exact payload bytes.
3. Add a deterministic AnyApp-side package reader/validator proof that does not rerun arbitration.
4. Draw explicit GUI.SVG relationship rails from events to manuscript sections and add scene/beat links.
5. Connect the Narrator FSM workflow to the user's Renderer package through a host-neutral portrayal request and explicit result contract.
6. Add richer Plot Twist alternatives, consequence/impact links, and explicit promotion to world events.
7. Add immutable publication editions and corrected-edition lineage.
8. Integrate StoryBuilder into the Workshop WebPage host.
9. Continue optional Gestures, Visualize, and Physics integration through host-discovered capabilities.
