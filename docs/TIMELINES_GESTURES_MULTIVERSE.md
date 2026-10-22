# Timelines, Gestures, and Multiverse History

## Purpose

StoryBuilder must let an author build an Experience whose events can be explored, animated, revised, and revealed in different orders. A prose manuscript is one presentation of the world—not the world-history database itself.

This is an architecture and product contract. The current vertical slice persists timeline definitions, provisional planning points, timeline-assigned world events, manuscript sections, and event-to-section references. It does not yet provide full temporal/causal editing, observer-aware narration, character animation, physics-backed playback, or multiverse navigation.

## Core rule: one world model, several views

Keep the canonical authoring model independent of presentation and simulation. Visualize, Renderer, GUI.SVG, Gestures, and future physics or playback providers are optional capabilities composed by the host. StoryBuilder requests them through the neutral MicroBundleDomain/catalog contract; it must not take a direct dependency on FSM_COS, Visualize, Renderer, or a concrete renderer from its domain core.

- **StoryBuilder.Core** owns identities, events, temporal relationships, timeline branches, scene-to-event links, disclosure/reveal decisions, and author intent.
- **Gestures** can supply character movement/behavior definitions and animation capabilities.
- **Visualize** supplies an inspectable visual world/scene representation.
- **Renderer** supplies an appropriate portrayal for the consumer, including text-oriented representation where supported.
- **GUI / GUI.SVG** provide interactive, living authoring views derived from actual domain records.
- **Physics** may later supply optional simulation or consequence calculations; a simulation result is not silently canon until the author or a declared rule accepts it.
- **FSM_COS** composes available MicroBundles at the host boundary. The host discovers actual capabilities and versions; StoryBuilder must not pretend an unavailable provider exists.

## Events are not the same as scenes or prose

An event is a world-history record with a stable identity. A scene is an authored arrangement of material that may depict one or more events. A manuscript is a presentation that may reveal, conceal, summarize, or reorder events.

For example, an author may decide that lightning struck the lone hill's tree on a particular date but omit the strike from the first book. A sequel can later split flashbacks into before-strike and after-strike periods without inventing the event retroactively or relying on the author's memory.

Each event should be able to record, as the domain evolves:

- Stable event ID and concise author-facing label. Stable event identity and optional timeline assignment now exist.
- World/timeline identity and temporal placement, including approximate or unresolved dates. Timeline assignment and approximate/unresolved placement are persisted; full temporal semantics remain future work.
- Participants, affected entities, location, causes, consequences, and source/provenance links when known.
- Status such as idea, proposed, accepted canon, disputed, or superseded.
- One or more scene, beat, paragraph, or manuscript references that portray it. Manuscript sections can currently reference events by stable ID; scene/beat-level links remain future work.
- Disclosure state: author-only note, known to selected characters, hinted to readers, revealed, or not yet presented. Disclosure is separate from whether the event is canon.
- Revision history and links to the event or state it changes.

An omitted event is not an error. A gap can be deliberate, unknown, or unresolved; the UI should distinguish those states.

## Plural timelines and divergence

The system must support multiple timelines and shared-universe scale without requiring an author to simulate or enumerate every possible universe.

A timeline is a named, identifiable history branch with an origin and a divergence relationship. When a time-travel action changes history, model the operation as a branch from an explicit point, rather than overwriting the source history:

1. Record the time-travel action and its source timeline.
2. Identify the divergence point and the relevant state/snapshot boundary.
3. Create a related timeline branch with stable identity and parent/divergence references.
4. Preserve the source timeline and its events.
5. Record changes and consequences on the new branch, including events that were prevented, replaced, or newly introduced.
6. Present branches side by side, with divergence markers and links to comparable events.
7. Let the author explicitly decide whether a branch is canon, an alternate, a discarded draft, or unresolved.

The user's intended conceptual model is that time travel duplicates the universe at the moment of travel, then rolls back/changes the traveler’s history so the new branch can sit beside the original. The implementation should capture this semantic model without prematurely committing to a particular physics algorithm, memory-copy strategy, or simulation engine. Snapshot/delta storage, immutable event records, and efficient branch references are implementation options to evaluate later.

The engineering ceiling is **multiverse**, not an unbounded hierarchy of realities. Support many observers and timelines, but do not promise exhaustive traversal, full simulation, or enumeration of an arbitrarily large space. Use lazy discovery, focused subgraphs, summaries, indexed events, and branch comparison so users can navigate a comprehensible slice of history. A timeline is not required to be fully materialized merely because it is addressable.

## Time is plural, not merely a sorted list

Temporal data should distinguish:

- **World time:** when an event happened in its timeline.
- **Presentation order:** when the audience encounters it.
- **Character knowledge time:** when a character learned or remembered it.
- **Authorial disclosure:** when and how the author intends to reveal it.
- **Uncertainty:** exact, ranged, relative, inferred, disputed, or unknown placement.

Do not assume a single total order. Events can be simultaneous, causally ordered without known dates, or placed differently in alternate timelines. A timeline view can sort where possible while preserving uncertainty and partial ordering.

Useful views include one timeline, synchronized timelines, branch comparison, character-specific chronology, location history, event dependency/causality, and the order in which a reader encounters scenes. These are different projections of shared records, not separate competing sources of truth.

## Observers and event horizons

A shared universe may contain thousands or millions of unusually aware observers, while most non-player characters need only ordinary local behavior. Some events or “event horizons” may be organized around observers: their presence, awareness, choices, and access to information can shape which experiences matter or become reachable.

Model observers as explicit entities/roles with links to what they can perceive, know, influence, and remember. Do not equate an observer's knowledge with objective world truth. Do not require every NPC to be a fully simulated observer. Detailed observer-driven world evolution is a future capability that must be supplied by an actual provider or rule system, not implied by a diagram.

## Gestures and character animation

StoryBuilder should allow an author to choose a character, select or compose a gesture, place it on a scene/beat/timeline, adjust its timing and context, and preview the result when a compatible provider is available.

The authoring model should preserve:

- Character and gesture identity.
- Intent/emotion/context, when provided by the author.
- Start time, duration, ordering, and links to scene beats or world events.
- Target entities and relevant spatial anchors.
- The provider/capability used to portray the gesture, including version where needed.
- Whether the motion is a hand-authored direction, a suggested performance, or a simulation result.
- Review state and provenance for generated or simulated outcomes.

Keep gesture intent separate from its manifestation. One character gesture might be portrayed through a visual animation, summarized by Renderer, or described in prose. An animation preview should not silently rewrite the manuscript; an accepted world action may be linked to the prose draft as a proposed passage.

Initial interaction should be practical and modest: select character, add a gesture/action beat, place or reorder it, preview if available, and generate an editable narration draft. Rich skeletal animation, crowd simulation, and physics-driven choreography are later capability slices.

## Visualizing and rendering a scene

When an author sees a scene, the author should be able to select an entity or location—such as the lone tree on a hill—and add an event or edit its temporal placement. That action should create or update a StoryBuilder domain record, then refresh all relevant projections:

- scene view / living visual;
- event and world timeline;
- related timeline branch comparisons;
- character and location histories;
- continuity warnings and affected manuscript passages;
- Renderer-produced narration or other portrayal, if a compatible output provider is available.

Visualize and Renderer are capability providers, not the canonical database. If the scene provider can display a tree but cannot author a persistent event, StoryBuilder must still record the event in its own model and send a view update through the supported contract. Never make an event exist only inside a transient renderer scene.

## Continuity and revision safety

When a historical event changes, show the author the impact before accepting a consequential rewrite. Potential impacts include date conflicts, character knowledge that arrives too early, flashbacks now crossing a divergence, scene references to an event that no longer exists on that branch, and descriptions that imply the wrong historical state.

Offer explicit choices: revise only the event record, update linked scene drafts, propagate a change to selected branches, mark a contradiction for later, or cancel. Preserve prior history and provenance. Do not automatically rewrite accepted prose, erase an original timeline, or infer that a branch is canon because it is currently selected.

## Capability discovery and composition

Before enabling animation, visualization, text rendering, timeline interaction, or simulation, query the host-provided MicroBundle catalog for actual available capabilities, compatible versions, dependencies, and host requirements. If a capability is unavailable, provide a truthful fallback (for example, a textual scene description or event list) and keep the domain model usable.

The StoryBuilder bundle depends on the neutral MicroBundleDomain contract. FSM_COS composes it in the host. No direct StoryBuilder.Core dependency on FSM_COS, Visualize, Renderer, Gestures, GUI.SVG, or a physics engine.

## Delivery sequence

1. **Historical event records:** stable event identity, uncertain temporal placement, canon/revision state, and links to existing claims/scenes.
2. **Author-facing chronology:** list and timeline views with world time distinct from presentation order; create an unshown event such as the lightning strike.
3. **Scene-to-event links:** link beats and draft passages to events; mark events as unrevealed or author-only.
4. **Branch model:** parent timeline, divergence point, branch-local changes, side-by-side comparison, and explicit canon/alternate status.
5. **Gesture authoring:** attach gestures to characters and beats; timing/reorder controls; narration fallback.
6. **Provider-backed scene visualization:** discover Visualize/Renderer/Gestures capabilities through the host catalog and update living views from real domain records.
7. **Continuity impact preview:** surface affected dates, character knowledge, scene references, and branch assumptions before revision.
8. **Scale and optimization:** observer-centered navigation, indexed history, lazy branch expansion, and snapshot/delta strategy informed by measured needs.

## Acceptance examples

- An author records the tree's lightning strike as accepted world history but does not show it in Book One.
- A sequel can create “before the strike” and “after the strike” flashback groupings and show which events each scene depicts.
- A time-travel action creates a sibling timeline at a visible divergence point while leaving the original branch intact.
- A branch comparison highlights added, removed, altered, and unchanged events without flattening them into one timeline.
- A character gesture can be placed on a scene beat; if animation capability is unavailable, the beat and editable narration remain usable.
- A visual change refreshes from canonical StoryBuilder records; no event is stored only in a transient visual scene.
- Ambiguous dates, character knowledge, and unresolved causality remain explicitly uncertain rather than being guessed.
- A host without Visualize or Renderer can still load, edit, persist, and inspect the same underlying timeline data.

## UML and use cases

Conceptual entities for the future domain model:

```text
Experience
  ├── World / Universe
  │    ├── Timeline (parent, divergence point)
  │    │    └── WorldEvent (time, causality, status, disclosure)
  │    ├── Observer (knowledge, perception, influence)
  │    └── Entity / Location
  ├── Character ── GesturePerformance ── SceneBeat
  ├── Scene ── portrays ── WorldEvent
  └── ManuscriptPresentation (reader order, reveal plan)
```

These are conceptual relationships, not a claim that all corresponding classes exist. Source-derived UML should be generated only after the actual contracts and types are implemented.

## Related documentation

- [Narrator Experience](NARRATOR_EXPERIENCE.md)
- [Character Casting Studio](CHARACTER_CASTING_STUDIO.md)
- [Architecture](ARCHITECTURE.md)
- [Scene Capture](SCENE_CAPTURE.md)
- [Development Status](DEVELOPMENT_STATUS.md)
