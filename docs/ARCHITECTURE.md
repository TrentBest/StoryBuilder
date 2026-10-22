# StoryBuilder Architecture

## Ownership

StoryBuilder owns the authoring domain: Experience design, character profiles, world claims and events, scenes, campaigns, participant actions, provenance, continuity, vocabulary, timeline definitions, manuscript structure, provisional planning points, timeline-assigned world events, event-to-section references, and observer knowledge. Immutable publication editions and reader-disclosure policy remain future capabilities.

It does not own host lifecycle, durable storage implementation, GUI rendering, physics simulation, or the general composition kernel.

## Boundary map

Author / participant
→ StoryBuilder authoring experience
→ Narrator: cooperating focused FSM_API state machines
→ StoryBuilder.MicroBundle
→ StoryBuilder.Core: host-neutral domain records and relationships
→ StoryBuilder.Persistence: bounded FSM_Serialization binary contract
→ Workshop composition/arbitration at the host boundary
→ versioned composition manifest + per-MicroBundle resolved configuration
→ AnyApp validates and consumes the captured package.

GUI.SVG, Renderer, Visualize, Gestures, and Physics are optional providers and projections. The MicroBundle adapter depends on the neutral MicroBundleDomain contract rather than FSM_COS itself. FSM_COS composes bundles at the host boundary. StoryBuilder.Core must not directly depend on FSM_COS or concrete renderer/physics providers.

## Core invariants

1. A sketch is useful. Require only what the current workflow truly needs.
2. Identity survives editing. Stable IDs connect scenes, relationships, sources, events, and manuscript references.
3. Wording survives normalization. Synonym search retains surface form and attribution.
4. Claims have provenance. A statement, its source, and the author's truth status are distinct.
5. Build record and fictional canon are separate.
6. Participant actions remain proposals until deliberately accepted.
7. Presentation is an adapter, not a domain dependency.
8. A timeline is composable execution through a host-compatible Process Group driver.
9. A planning point may remain unknown. Plot Twist can have stable identity and approximate placement before its meaning is decided; it is not accepted canon by default.
10. World truth, observer knowledge, and manuscript disclosure differ. Do not present inference as witnessed fact without attribution or explicit narration policy.
11. Published editions are immutable evidence. Corrections create a new version.
12. Resolved decisions are packaged. Workshop arbitration records its validated result; AnyApp consumes it rather than silently rerunning arbitration.

## Integration boundaries

- **FSM_API:** optional StoryBuilder.FSM adapter. Use it to coordinate focused Narrator FSMs and timeline Process Groups. Domain records remain the source of truth; Core stays independent of FSM_API.
- **MicroBundleDomain / FSM_COS:** the bundle implements the neutral contract; FSM_COS composes it at the host boundary.
- **FSM_Serialization:** bounded, versioned binary workspace persistence plus a binary package compiler/reader that emits manifest, configuration, and content payloads. Exact payload hashes are validated before workspace restoration.
- **GUI / GUI.SVG:** living projections derive from canonical domain data and link back to source records. The timeline renderer draws numbered event markers and distinct portrayal rails to the actual manuscript sections and scene beats that reference each event. Scene-beat rail and Stage controls are awaiting CI verification.
- **Renderer:** portrays the selected composition under explicit observer/disclosure constraints; it does not own knowledge rules.
- **Visualize / Gestures / Physics:** optional scene inspection, animation, and simulation providers.
- **WebApp / WebPage / AnyApp:** host authoring, preview, and consumption. The package reader validates a previously arbitrated result and reports incompatibility rather than improvising.

## Existing Workshop repositories and artifact flow

This project must use the existing Workshop repository boundaries instead of creating a parallel Experience or MicroBundle system:

- [TheSingularityWorkshop.Experiences](https://github.com/TrentBest/TheSingularityWorkshop.Experiences) owns independently buildable, host-independent Experience/MicroBundle projects. Its current publication guidance stages the compiled MicroBundle assembly as the artifact; it explicitly says the host is not the source of truth for Forge or Moniker.
- [TheSingularityWorkshop.MicroBundleDomain](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain) supplies the neutral `IMicroBundle`, descriptor, provider, dependency, and lifecycle contracts. StoryBuilder's `StoryBuilderAuthoringBundle` currently implements this contract directly and must remain independent of the composition kernel.
- [TheSingularityWorkshop.MicroBundleRepository](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleRepository) owns durable artifact delivery and verification. MicroBundle artifact identity is `BundleId + Version + SHA-256`; Experience artifacts have their own `ExperienceId + Version + SHA-256` identity. The repository stores opaque bytes and does not compose or arbitrate them.
- [TheSingularityWorkshop.FSM_COS](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS) owns dependency closure, composition, arbitration, and the resulting RuntimeAssembly.
- [AnyApp](https://github.com/TrentBest/AnyApp) is the desktop manifestation/host. Its current development branch already references the Experiences repository projects and MicroBundle Repository REST project when `WorkshopUseRepositorySources=true`, and has a published-Experience catalog client/launch path. Its native MicroBundle catalog and presentation code still contain explicit registrations and must be reconciled with artifact materialization and the actual supported surface contract.

Keep three artifacts distinct:

1. **Experience manifest:** the host-facing launch/composition request, including the immutable identities and configuration needed for its MicroBundle closure.
2. **MicroBundle artifact:** the independently built executable bundle bytes addressed by bundle ID, version, and content hash.
3. **StoryBuilder workspace/composition package:** mutable authoring data versus a versioned, validated capture of content and resolved decisions. Neither is a replacement for the host's Experience manifest or the repository's immutable artifact identity.

The intended path is: build/publish the StoryBuilder MicroBundle from its independent project; reference its immutable artifact identity from a StoryBuilder Experience manifest; resolve and verify bytes through the existing repository/cache path; materialize the neutral MicroBundle contract; compose through FSM_COS; then expose the resulting experience through AnyApp's existing GUI/WPF surface contract. Do not add StoryBuilder-specific host logic or a competing manifest directory before the current Experience and MicroBundle contracts have been checked end-to-end.

**Current integration gap:** StoryBuilder's bundle project currently references `MicroBundleDomain` and StoryBuilder.Core, while AnyApp's catalog is still a finite compiled catalog of known bundles. The StoryBuilder bundle declares capabilities but does not yet implement AnyApp's `IAnyAppSurface` or another verified presentation adapter. Registering its bundle ID alone is therefore not a complete desktop integration. First validate the real artifact materializer, catalog extension point, configuration contract, and surface/renderer contract against the current AnyApp development branch. No NuGet publishing is required or authorized for this work.

## Product-first composition

The first end-to-end product is a book. It needs explicit manuscript composition—not just a bag of scenes—with edition identity, reading order, chapter/scene/passage placement, actual text, and stable links back to world events. The same source model should power book preview, event timeline, composition structure, observer analysis, continuity review, and optional visual/animation views.

Distinguish world chronology, causality, manuscript order, observer knowledge time, and reader disclosure order. A shared event may be portrayed in several places; each portrayal references the event rather than duplicating it as new world truth.

## Timeline and Narrator

Every timeline is intended to be a semantic Process Group advanced by a compatible host driver. Its update must not accept canon or mutate published content. NarratorAgent coordinates five isolated FSM_API state machines for intent, planning, observer constraints, portrayal preparation, and draft/review. Its transitions are tested, its draft guard consumes ObserverKnowledgeEvaluator results, and it keeps canonical state in StoryBuilderSession. LLM-backed drafting, Renderer integration, and additional continuity/composition/package FSM responsibilities remain future work; expand the agent only through explicit contracts and tests.

The timeline console should have a configurable keyboard shortcut and accessible visible control. GUI.SVG can show event IDs and distinct relationship rails to books, chapters, scenes, and passages. Branch, causality, portrayal, and disclosure links must have different semantics.

See [Timeline Authoring and Observer Model](TIMELINE_AUTHORING_MODEL.md), [Timeline Process Groups](TIMELINE_PROCESS_GROUPS.md), and [Composition Package Contract](COMPOSITION_PACKAGE_CONTRACT.md).

## Current implementation boundary

The .NET 8 core currently provides character drafts, structured scene beats, a narration draft, vocabulary indexing, timeline definitions, provisional planning points, timeline-assigned world events, manuscript sections, event-to-section and event-to-scene-beat references, observer-knowledge records, and a host-neutral Process Group driver contract. ObserverKnowledgeEvaluator conservatively checks whether an observer can make a requested claim at an explicit event anchor. Optional adapters demonstrate FSM_API lifecycle behavior and the MicroBundleDomain contract. The local Blazor workbench includes Casting, Stage, Archive, Campaign, Living Map, Timeline, Book Preview, and Observer Lens views. Stage links world events to exact beats; GUI.SVG timeline rails show both manuscript-section and scene-beat portrayals; Observer Lens visualizes accepted knowledge. Version 8 binary workspace persistence stores event links on scene beats while preserving readers for versions 1 through 7; browser-local autosave is implemented. BookProductRenderer emits deterministic Markdown and stable referenced-event metadata from a working manuscript; repeated package consumption produces the same book and preserves the captured arbitration-record identity. The binary package compiler/reader and manifest dependency-closure validation have passed CI.

Actual AnyApp host integration, repeated-consumption proof against the real host, direct rail-row navigation, full temporal/causal graph editing, reader-disclosure policy, LLM-backed drafting and Renderer integration, immutable publication editions, cloud persistence, and Workshop-host integration remain planned.
