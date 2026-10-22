# Story domain contract

This document describes the domain StoryBuilder implements today. It is a guide to interpreting and consuming the records, not a proposal for a future architecture.

## The author's working loop

1. Create or restore a `StoryWorkspace` through `StoryBuilderSession`.
2. Create characters and scenes; capture a scene as ordered `SceneBeat` records.
3. Create a `TimelineDefinition` and record world events as `StoryEvent` records. Events may remain proposed or temporally unresolved while the author works.
4. Create a `ManuscriptEdition` and ordered `ManuscriptSection` records. Link sections and scene beats to existing event IDs when they portray those events.
5. Review claims, sources, observer knowledge, and continuity issues; accept only what should be treated as established.
6. Render the manuscript as deterministic Markdown and persist the workspace through the host's versioned binary persistence adapter.

The same world event can be referenced from more than one scene beat or manuscript section. Those links express portrayals of one event; they do not create duplicate world events or copy the event's truth into the prose.

## Records that must not be confused

| Record | Meaning | What it does not mean |
| --- | --- | --- |
| `TimelinePlanningPoint` | An intentionally incomplete point on a timeline, useful while chronology is being worked out. | A confirmed event or exact date. |
| `StoryEvent` | The author's canonical identity for a world event, including status, timeline placement, participants, causes, consequences, and source references. | Proof that the event has been accepted, or proof that a character knows about it. |
| `TimelineEventRecord` | An inspectable runtime observation emitted by a timeline Process Group. | An automatic declaration of story canon. |
| `SceneBeat` | An ordered captured action, reaction, dialogue, pose, or related scene element. | The world event itself. |
| `ManuscriptSection` | A reading-order unit containing editable prose and references to scenes/events it portrays. | A second source of truth for the linked event. |
| `HistoricalClaim` / `SourceRecord` | A claim about the world's past and provenance for an account or artifact. | The fact that a source made a statement is not, by itself, proof that the statement is canon. |
| `ObserverKnowledgeRecord` | What a particular observer is established to know, how they know it, and when they acquired it. | Objective world state or unrestricted narrator access. |
| `CharacterRelationship` | A character's attributed relationship to another character, with its own label, notes, and review status. | A free-text name; `TargetCharacterId` must resolve to a character in the same workspace. |
| `PlaceRecord` | A named world location with stable identity, descriptive type, status, and optional parent place. | A scene's free-form location note; scenes can retain that note and separately reference a canonical place ID. |
| `ParticipantAction` | A participant's attributed proposed choice. | An accepted campaign consequence until deliberately resolved. |

These distinctions are enforced in different places: record identity and basic workspace membership in the domain models/session; author-review status in the record lifecycle; narration constraints in `ObserverKnowledgeEvaluator` and the accepted-knowledge projection; and storage compatibility in the versioned persistence layer. A record's existence alone must not be presented as acceptance. `StoryWorkspaceValidator.Validate` also checks cross-record references after assembly or restore, including character-to-character relationships, character participation in events, event causes and consequences, source attribution, scene-beat actors, and manuscript portrayals. It detects duplicate IDs within record collections and IDs reused across different record types. It reports integrity issues without repairing or deleting data; callers should resolve the reported IDs explicitly before persisting a workspace that must be internally consistent.

## Canon, attribution, and point of view

StoryBuilder keeps objective event records separate from the information available to a narrator or character. The observer-knowledge evaluator is conservative:

- no matching knowledge record means the requested claim is denied;
- knowledge records that are only proposed leave the result unresolved;
- accepted witnessed knowledge can support a direct-fact claim;
- accepted reports require attributed-report framing;
- inference supports inference or belief framing;
- belief or disputed knowledge supports belief framing;
- accepted forgotten knowledge does not support a claim at that narration point.

Knowledge acquired at a particular event is available later only when that acquisition event is in the causal history of the narration point. Knowledge with no acquisition event is treated as already available; if a narration point is supplied, event-scoped acquisition must be reachable through the event's cause links. This is a constraint for drafting and portrayal, not an automatic prose generator or a guarantee that every possible continuity contradiction is detected.

## Identity and persistence

Relationships are stored as stable record IDs, not as copied prose. A section's `EventIds` and a beat's event references let tools inspect where an event is portrayed without making the manuscript the authority on world history. Timeline IDs likewise identify timeline records; presentation order and approximate placement should not be mistaken for a precise global chronology.

The versioned binary workspace format preserves domain records and their links across supported schema versions. Hosts are responsible for using the persistence adapter and verifying their own save/restore lifecycle. A successful serializer round-trip or store-level restore test proves data preservation at that layer; it does **not** by itself prove that a desktop process can exit and later reopen the same workspace.

## What the Alpha does and does not promise

The current Alpha scope is a local, book-first authoring prototype: create characters/scenes/events, connect events to manuscript passages, inspect Markdown book output, and preserve a versioned workspace. The browser demo stores data in that browser's local storage. Native desktop process-restart behavior remains an explicit end-to-end verification gate until the real host workflow is exercised.

The Alpha does not promise cloud synchronization, simultaneous collaboration, LLM-backed drafting, immutable publication editions, complete Renderer integration, or hosted Workshop WebPage deployment. These are limitations of the current release, not hidden capabilities.

## Practical rule

When reading or extending StoryBuilder, ask three separate questions:

1. **What happened in the world?** Inspect the canonical event and its status.
2. **Who knows what, and when?** Inspect accepted observer knowledge and its acquisition history.
3. **Where is it portrayed?** Follow the event's scene-beat and manuscript-section references.

Keeping those questions separate is the core domain contract that makes multiple perspectives, disputed accounts, and multiple portrayals possible without silently rewriting canon.
