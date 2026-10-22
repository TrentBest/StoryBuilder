# Timeline Authoring and Observer Model

## Purpose

The timeline organizes world history and acts as an executable authoring Process Group. Authors need to place ideas before they know what those ideas mean, then develop them into events, scenes, passages, and a finished product without losing the original intent.

This is a domain contract, not a claim that all described workflows are implemented. The current verified vertical slice includes timeline-assigned world events, stable event-to-manuscript-section references, and version 8 scene-beat event links. The scene-beat rail and Stage linking controls passed CI. ObserverKnowledgeRecord, ObserverKnowledgeEvaluator, and an indexed ObserverKnowledgeProjectionService are implemented and tested. NarratorAgent exposes the accepted-knowledge projection through its observer FSM, and the Observer Lens renders that projection through GUI.SVG. NarratorAgent now coordinates separate FSM_API state machines for intent, planning, observer constraints, and draft/review. The evaluator accepts only author-accepted knowledge, requires appropriate claim framing, and fails closed when causal availability cannot be established. Renderer integration and reader-disclosure policy are not yet implemented.

## A provisional point is a real object

An author must be able to add **Plot Twist** at an approximate position and continue authoring around it. The point has a stable identity even while its explanation is unknown. Uncertainty is meaningful data, not a validation failure.

A planning point should support a stable ID and timeline, label and optional question, author notes, approximate/relative/ranged/unresolved placement, planning state, alternative explanations, possible consequences, affected characters, links to events and manuscript locations, and change history. Unknown information stays unknown.

A placeholder is not automatically a world event or accepted canon. When the author understands the twist, the system can explicitly promote or link the planning point to events and scene drafts. Brainstorming must not silently rewrite canon.

## Keep four orders distinct

1. **World chronology:** when an event happens in a timeline; dates may be approximate or unknown.
2. **Causality:** which events influence other events even when dates are unknown.
3. **Manuscript order:** where the reader encounters an event through chapter, scene, passage, flashback, or recap.
4. **Knowledge and disclosure:** when an observer learns or remembers something, and when the author intends the reader to learn it.

These orders may differ without contradiction. GUI.SVG connectors must distinguish branch lineage, causality, portrayal, and disclosure rather than using one ambiguous line for every relationship.

## Observer constraints

An observer-constrained account should distinguish what the observer directly perceived; what they were told and by whom; what they inferred or believe; what they remember at the account's point in time; what they cannot know or have forgotten; and what the author has disclosed to the reader.

A text-generation provider may consume this constrained model to draft language. The current Renderer package selects observer-relative visual/semantic representations and computational detail; it is not itself a text-generation provider. Renderer is not the authority that decides what the observer knows. StoryBuilder records and declared policies supply the constraints. An omniscient narrator is an explicit policy, not an accidental bypass.

When a passage exceeds an observer's knowledge, flag it and offer grounded alternatives: omit it, frame it as inference, attribute it to a source, change observer, or explicitly change narration policy. Do not silently invent knowledge.

## Renderer and narrative observer are complementary

Renderer's observer model governs observer-relative representation: semantic detail, visibility, interaction, spatial context, and computational detail across Event Horizons. StoryBuilder's observer-knowledge model governs what a character or narrator may claim to know, and how that claim must be framed. These are connected layers, not interchangeable policies.

For example, StoryBuilder may permit an observer to report a rumor only as an attributed report. Renderer can then choose the appropriate visual or semantic representation for that observer and presentation context. A visual representation decision must never grant the character new narrative knowledge, and an observer-knowledge decision must not force a particular graphics backend. The integration belongs behind host-neutral portrayal requests and optional MicroBundle selection; StoryBuilder.Core must not acquire a direct dependency on Renderer or FSM_COS. The projection includes only event identities and accepted knowledge claims, not unrestricted world-event titles, summaries, or causal edges.

## Multi-FSM Narrator

The Narrator is a coordinated agent of focused FSMs, not a monolithic state machine. Use FSM_API for lifecycle and transitions. Candidate responsibilities include:

- Intent: preserve the author's goal and unresolved questions.
- Planning: maintain provisional points, alternatives, and dependencies.
- Continuity: check identities, chronology, causality, branch lineage, and contradictions.
- Observer/disclosure: determine what an observer and reader can know at a given point.
- Composition: map events into chapters, scenes, beats, and passages.
- Drafting: produce or revise a portrayal under explicit constraints.
- Review: present issues and proposed changes for author approval.
- Package: validate and capture resolved configuration and package revision.

The current implementation instantiates four of these responsibilities—intent, planning, observer constraint, and draft/review—as isolated FSM_API definitions and processing groups. Continuity, composition, and package responsibilities remain candidate future machines. A coordinator sequences them; canonical story truth stays in domain records, not duplicated as competing FSM-local canon.

FSMs communicate through explicit proposals, events, and stable IDs. A transition does not itself accept canon or mutate an immutable published edition. Expensive arbitration results are captured in package configuration once their inputs are known.

## Timeline execution and product views

A timeline Process Group is an execution seam, not permission to rewrite the world. Runtime observations, authored planning points, canonical world events, and published artifacts are different record categories.

The keyboard-invoked author console should have an accessible visible fallback. Authors should add/select points, inspect connected scenes, and navigate to source records. GUI.SVG is a living projection of the same data; Renderer and other views must not maintain separate story databases.

Books are the first output. Views should include a real manuscript/book with edition, reading order, chapters, scenes, passages and text; world chronology and causality; composition structure; observer knowledge; continuity; and optional provider-backed scene/gesture views. Every view links back to stable source records.

## Acceptance scenarios

- Add a provisional Plot Twist between known events without specifying its meaning or exact date.
- Brainstorm alternatives without changing accepted canon.
- Select a candidate and link consequences and scene drafts while preserving the original point ID.
- Show one world event in several scenes or recountings without duplicating its event identity.
- An observer who did not witness an event must attribute a source or present the information as inference, not direct knowledge.
- Select an event in GUI.SVG and navigate to the chapters/scenes/passages that portray it.
- Consume an arbitrated book package in AnyApp without repeating arbitration.
- Add an optional renderer without introducing a hard dependency into StoryBuilder.Core.

## Related documents

- [Timeline Process Groups](TIMELINE_PROCESS_GROUPS.md)
- [Timelines, Gestures, and Multiverse History](TIMELINES_GESTURES_MULTIVERSE.md)
- [Composition Package Contract](COMPOSITION_PACKAGE_CONTRACT.md)
- [Narrator Experience](NARRATOR_EXPERIENCE.md)
- [Architecture](ARCHITECTURE.md)
