# Timeline Process Groups and Immutable Story Editions

## Status and intent

The executable foundation is verified by CI: StoryWorkspace owns TimelineDefinition records, the host-neutral StoryProcessGroupDriver advances inserted IStoryProcessGroup instances, and the optional FSM_API-backed TimelineProcessGroup emits inspectable runtime observations. The binary workspace schema is version 6 and preserves timeline definitions, manuscripts, provisional planning points, and timeline assignment for world events. The WebDemo can create planning points and world events, and manuscript sections can reference events by stable ID.

The keyboard-invoked developer console, drawn event-to-book relationship rails, observer-aware recounting, full temporal/causal model, and immutable publication editions remain future work. Event-to-section references are persisted, but the SVG does not yet draw the cross-view links.

## A timeline is an executable Process Group

Each timeline is a first-class, stable-identity Process Group. Everything that happens within that timeline is represented by, owned by, or explicitly referenced from that group: its event stream, temporal relations, participants and observers, branch relationships, and timeline-local process state.

The host should be able to insert a timeline Process Group into its driver and let the driver call that group's update. The first implementation proves this composition seam: the host-neutral driver invokes Update on each inserted group, and the FSM_API adapter advances its isolated processing group and exposes event observations. This is a foundation, not yet the complete temporal simulation.

Keep the boundary clean:

- **StoryBuilder.Core** owns the semantic timeline definition, stable identities, event records, temporal/causal links, observer knowledge, branch lineage, narration links, and publication references.
- **The runtime adapter** maps a timeline definition to the available Process Group / driver contract. Use FSM/API capabilities through an adapter where appropriate; do not make the domain core depend directly on FSM_COS or a specific host.
- **The host/driver** decides when and how often to update the Process Group and supplies the execution environment.
- **MicroBundleDomain capability discovery** lets the host compose the actual runtime and optional providers. StoryBuilder must not assume a provider is present merely because a design document names it.

An update is not automatically permission to mutate published canon. Runtime progression, proposed world changes, accepted canon, and immutable published snapshots are distinct states. Process Group execution must be deterministic where its declared contract requires it, and observable enough to inspect and test.

## Timeline as a developer console

Timeline is not merely a page authors must navigate to. It is an always-available, developer-console-style overlay/workspace that can be summoned with a keyboard shortcut. The shortcut should be configurable and discoverable, and must not hijack ordinary text entry or assistive technology. A visible, accessible control is required as a fallback on mobile and for users who cannot use the shortcut.

The console should inspect real timeline Process Group state, events, causal links, world time, observer knowledge, presentation/reveal state, branches, and links to scenes, paragraphs, characters, locations, and publications. It should display provider availability based on actual host discovery.

## Stable event identity and book relationships

Timeline event IDs are stable semantic identities. Displayed timeline numbers are view references and may change with sorting, filtering, or layout. Colored boxes or rails should connect events to the books, chapters, scenes, and passages that recount them, with a legend and non-color cues.

The relationship is bidirectional: selecting an event highlights every recounting; selecting a passage highlights its source event(s). Selecting an observer exposes what that observer could perceive or know at the relevant point. Selecting a branch shows shared history and divergence. Selecting a publication opens the exact immutable edition and its provenance.

## Retelling the same event

An event can be referenced by multiple recountings without duplicating its underlying identity. An author may choose to:
- reuse an exact recounting when that is intentional;
- create a new recounting constrained by a different camera, character, NPC, or other observer's knowledge;
- present an explicitly marked interpretation or contradiction;
- reveal another slice of the same event.

A sequel may point back to the original event while narrating it from a new viewpoint. The authoring system should preserve relationships and continuity instead of forcing copy/paste or silently overwriting earlier prose.

## Publication immutability

A published edition is an immutable artifact: its exact content, identity, and provenance remain fixed. Corrections create a new edition with explicit lineage; they do not rewrite the published edition. Prequels and sequels may cite, quote, echo, or retell source events, but the original publication remains unchanged. Publication locking freezes the edition, not the entire fictional universe.

## Acceptance scenarios

1. Two timelines can be inserted into a driver, advanced independently, and inspected without mixing their event streams.
2. Branch creation preserves the parent timeline identity and shared-history relationship.
3. A console can be opened without leaving the current authoring task and can be used without a keyboard.
4. One event can have multiple scene/passage recountings, including an exact reuse and an observer-constrained new retelling.
5. Publishing an edition freezes its content and provenance; corrections create a new linked edition.
6. Missing optional providers are clearly reported and do not prevent basic domain authoring.

## Delivery sequence

1. Define timeline/event/branch identities and the adapter boundary; persist timeline definitions in the versioned binary document.
2. Prove that two timeline Process Groups can be inserted into a driver, updated, and inspected.
3. Build the keyboard-invoked console over actual Process Group and domain records.
4. Draw bidirectional event-to-passage links and colored relationship indicators; stable event-to-section references now exist.
5. Add observer-aware retelling and exact-repeat references.
6. Add immutable publication editions and correction lineage.
7. Integrate optional Visualize, Renderer, and Gestures providers through host-discovered capabilities.
8. Optimize for many thousands of observers and large event horizons after correctness and provenance are covered.
