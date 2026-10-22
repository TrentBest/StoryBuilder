# StoryBuilder Architecture

## Ownership

StoryBuilder owns the authoring domain: Experience design, character profiles, world claims and events, scenes, campaigns, participant actions, provenance, continuity, vocabulary, timeline definitions, manuscript structure, provisional planning points, timeline-assigned world events, and event-to-section references; observer knowledge and immutable publication editions remain future capabilities.

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
- **FSM_Serialization:** bounded, versioned binary workspace persistence. Package payloads should reuse this proven boundary where appropriate.
- **GUI / GUI.SVG:** living projections derive from canonical domain data and link back to source records.
- **Renderer:** portrays the selected composition under explicit observer/disclosure constraints; it does not own knowledge rules.
- **Visualize / Gestures / Physics:** optional scene inspection, animation, and simulation providers.
- **WebApp / WebPage / AnyApp:** host authoring, preview, and consumption. Package consumers validate a previously arbitrated result and report incompatibility rather than improvising.

## Product-first composition

The first end-to-end product is a book. It needs explicit manuscript composition—not just a bag of scenes—with edition identity, reading order, chapter/scene/passage placement, actual text, and stable links back to world events. The same source model should power book preview, event timeline, composition structure, observer analysis, continuity review, and optional visual/animation views.

Distinguish world chronology, causality, manuscript order, observer knowledge time, and reader disclosure order. A shared event may be portrayed in several places; each portrayal references the event rather than duplicating it as new world truth.

## Timeline and Narrator

Every timeline is intended to be a semantic Process Group advanced by a compatible host driver. Its update must not accept canon or mutate published content. NarratorAgent now coordinates four isolated FSM_API state machines for intent, planning, observer constraints, and draft/review. Its transitions are tested, its draft guard consumes ObserverKnowledgeEvaluator results, and it keeps canonical state in StoryBuilderSession. LLM-backed drafting, Renderer integration, and additional continuity/composition/package FSM responsibilities remain future work; expand the agent only through explicit contracts and tests.

The timeline console should have a configurable keyboard shortcut and accessible visible control. GUI.SVG can show event IDs and distinct relationship rails to books, chapters, scenes, and passages. Branch, causality, portrayal, and disclosure links must have different semantics.

See [Timeline Authoring and Observer Model](TIMELINE_AUTHORING_MODEL.md), [Timeline Process Groups](TIMELINE_PROCESS_GROUPS.md), and [Composition Package Contract](COMPOSITION_PACKAGE_CONTRACT.md).

## Current implementation boundary

The .NET 8 core currently provides character drafts, structured scene beats, a narration draft, vocabulary indexing, timeline definitions, provisional planning points, timeline-assigned world events, manuscript sections, event-to-section references, observer-knowledge records, and a host-neutral Process Group driver contract. ObserverKnowledgeEvaluator conservatively checks whether an observer can make a requested claim at an explicit event anchor. Optional adapters demonstrate FSM_API lifecycle behavior and the MicroBundleDomain contract. The local Blazor workbench includes Casting, Stage, Archive, Campaign, Living Map, and Timeline views. Versioned binary workspace persistence and browser-local autosave are implemented. The latest GUI.SVG timeline map CI is green.

Still planned: scene/beat-to-event links, book export, full temporal/causal graph editing, reader-disclosure policy, LLM-backed drafting and Renderer integration, immutable publication editions, package manifest/configuration production and consumption, cloud persistence, and Workshop-host integration.
