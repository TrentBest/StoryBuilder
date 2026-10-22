# ObserverKnowledgeRecord

**User moniker:** Observer knowledge ledger

**Purpose:** Records how an observer knows, believes, infers, or disputes a world event. This is separate from objective world truth and separate from what the author has disclosed to the reader.

## UML class card

- Identity: Id, ObserverId, EventId
- Knowledge: Kind, AcquiredAtEventId, SourceId, Attribution, Note, Confidence
- Review: CanonStatus and CreatedAtUtc
- Evaluation result: allowed/denied/unresolved, required narration framing, explanation

## Use cases

- Record that a character witnessed an event.
- Record that a character learned an event from a source or inferred it later.
- Require author acceptance before a narration provider relies on the knowledge record.
- Reject direct-fact narration when the observer only has a report, inference, or belief.
- Keep Renderer optional: the core produces constraints; a portrayal provider consumes them.
