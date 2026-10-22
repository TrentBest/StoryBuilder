# Timeline Process Groups and Immutable Story Editions

## Status and intent

This is a design contract for the next critical StoryBuilder workstream. It does not claim timeline Process Groups, the developer-console UI, or publication locking are implemented yet. Timelines are the first major domain capability to design and prove.

## A timeline is an executable Process Group

Each timeline is a first-class, stable-identity Process Group. Everything that happens within that timeline is represented by, owned by, or explicitly referenced from that group: its event stream, temporal relations, participants and observers, branch relationships, and timeline-local process state.

The host should be able to insert a timeline Process Group into its driver and let the driver call that group's update. The timeline then advances according to its declared process behavior. This is the composability promise: a timeline can be hosted by a StoryBuilder session, a larger Experience, a simulation, a game, a test harness, or another compatible driver without requiring StoryBuilder to own the driver.

Keep the boundary clean:

- **StoryBuilder.Core** owns the semantic timeline definition, stable identities, event records, temporal/causal links, observer knowledge, branch lineage, narration links, and publication references.
- **The runtime adapter** maps a timeline definition to the available Process Group / driver contract. Use FSM/API capabilities through an adapter where appropriate; do not make the domain core depend directly on FSM_COS or a specific host.
- **The host/driver** decides when and how often to update the Process Group and supplies the execution environment.
- **MicroBundleDomain capability discovery** lets the host compose the actual runtime and optional providers. StoryBuilder must not assume a provider is present merely because a design document names it.

An update is not automatically permission to mutate published canon. Runtime progression, proposed world changes, accepted canon, and immutable published snapshots are distinct states. Process Group execution must be deterministic where its declared contract requires it, and observable enough to inspect and test.

## Timeline as a developer console

Timeline is not merely a page authors must navigate to. It is an always-available, developer-console-style overlay/workspace that can be summoned with a keyboard shortcut. The shortcut should be configurable and discoverable, and must not hijack typing when the author is editing manuscript text or using assistive technology. It should also have an ordinary visible control for touch/mobile and accessibility.

The console should let an author:

- Inspect the active timeline's Process Group identity, lifecycle/update state, current focus, and branch ancestry.
- View events, causal links, world-time placement, observer knowledge, manuscript presentation order, and reveal state together without conflating them.
- Step, pause, inspect, or compare timelines when the host's execution contract supports those actions.
- Jump from a timeline event to every linked scene, beat, paragraph, character, location, and publication.
- Reveal deliberately omitted events as author-only facts without accidentally exposing them in reader-facing output.
- Open synchronized timelines and see their shared history, divergence point, and branch-local consequences.
- Inspect which providers are available and what they actually support, with a graceful non-animated/non-visual fallback.

The console is an authoring and inspection surface, not a second source of truth. All meaningful edits go through domain commands and are persisted as domain records; the SVG, visualizer, renderer, or overlay is only a view of those records.

## Timeline numbers, colored boxes, and story recountings

Timeline event numbers should be stable references within the current view, not globally meaningful identities. Each event also has a stable event ID so renumbering, filtering, sorting, or changing a view cannot break references.

Colored boxes, rails, or brackets around timeline numbers can indicate the relationship between world events and one or more authored recountings: book editions, chapters, scenes, flashbacks, screenplay sequences, or other output forms. Color is a visual cue, not the underlying identity or only way to distinguish a relation; provide labels, accessible contrast, and a legend.

The UI should make these relations entangled in both directions:

- Select an event number and highlight the scenes, paragraphs, character histories, and published editions that recount it.
- Select a book passage and highlight the events it portrays, including events it compresses, combines, or deliberately leaves out.
- Select an observer/camera and show which facts that viewpoint could perceive or know at that moment.
- Select a branch and show inherited events, the divergence point, and branch-specific events.
- Select a publication and show the exact immutable edition that was released, plus any later work that references it.

One world event may be recounted in multiple books or scenes. One scene may portray several events. A recounting may be intentionally identical to an earlier telling, or newly written from another observer's viewpoint. These are explicit author choices, not automatic duplication.

## Same event, different camera, fresh recounting

A new camera, NPC, or other observer can be introduced to witness a scene that already exists. StoryBuilder should let the author reuse the underlying event and scene facts, add the observer's perspective and knowledge constraints, and draft a new recounting for a sequel or alternate presentation.

The author chooses the desired operation:

1. **Repeat exactly:** reference the prior passage or approved passage snapshot without rewriting it.
2. **Retell from another viewpoint:** preserve the event identity and established facts, but generate a new draft constrained by the new observer's location, senses, knowledge, and voice.
3. **Reinterpret or contradict:** mark the difference explicitly as a mistaken witness, unreliable narration, disputed account, branch difference, or proposed canon change. Do not silently rewrite history.
4. **Show a different slice:** focus on a different part of the same event while linking it to the same event record.

Narrator-generated prose is always a proposal until the author accepts it. A camera/NPC is not omniscient by default. It cannot narrate facts it could not observe or know unless the author deliberately chooses an omniscient or otherwise justified narrative mode.

## Publication creates immutable source material

When an edition is published, its published content and publication identity become immutable source material for future prequels, sequels, and shared-universe work. The author can continue writing in the same universe, but cannot quietly edit the historical published edition in place.

A publication record should identify at least:

- Stable publication and edition IDs.
- The exact immutable manuscript/content artifact and its content hash.
- The release/version identity and publication timestamp or declared in-world/out-of-world context as applicable.
- The canon and timeline references used for that edition.
- Links from passages to the events and sources they portray.
- The edition's status and any superseding/corrected edition relationship.

Corrections produce a new edition with explicit lineage; they do not mutate the old edition. A sequel can cite, quote, echo, or intentionally retell an earlier scene. The prior publication stays unchanged while the sequel's own passage is new content linked to the same events. If the author wants a new continuity that diverges from a published story, create an explicit alternate branch or continuity; never silently alter the locked edition.

Publication locking is about the artifact and its recorded provenance. It does not freeze the entire universe forever: new events can occur before, during, or after the published story, and new observer accounts can be written, provided they preserve traceability and surface contradictions for author review.

## Acceptance scenarios

1. A timeline Process Group can be inserted into a compatible driver; the driver calls update and the group's declared behavior runs without the driver needing StoryBuilder-specific knowledge.
2. Two timeline groups can share a declared origin and then diverge without overwriting source events.
3. The author summons the timeline console with a shortcut, inspects an event, and returns to the manuscript without losing selection or editing state.
4. Selecting a colored event-number box highlights every linked recounting; selecting a passage highlights its source events.
5. The author chooses exact repetition and the original published passage remains byte-for-byte unchanged.
6. The author adds an NPC observer and drafts a sequel recounting of the same event; the new text is distinct, linked to the same event, and constrained by the observer's knowledge.
7. An impossible-to-know detail is flagged unless the chosen narrative mode permits it.
8. A published edition cannot be mutated in place. A correction creates a new edition; a sequel may reference the old edition while adding new material.
9. Removing or replacing an optional visualization/renderer provider does not delete or change canonical timeline records.
10. A keyboard shortcut does not fire while it would disrupt text entry, and an accessible visible control remains available.

## Delivery order

1. Define and test the domain timeline/event/branch identities and the Process Group adapter boundary.
2. Prove one timeline Process Group can be inserted into a driver and updated.
3. Build the keyboard-invoked timeline console over real domain records.
4. Add bidirectional event-to-recounting links and colored relationship indicators.
5. Add observer-constrained retelling and exact-repeat references.
6. Add immutable publication editions and explicit correction/alternate-continuity lineage.
7. Discover and integrate optional Visualize, Renderer, Gestures, and GUI.SVG capabilities through the host's MicroBundle catalog.
8. Optimize indexed history and focused branch views for large observer/timeline counts, retaining multiverse as the engineering ceiling.

The first milestone is the timeline's domain contract and executable Process Group proof—not a polished console or a speculative simulation engine.
