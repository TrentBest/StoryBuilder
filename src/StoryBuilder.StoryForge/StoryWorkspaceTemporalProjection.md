# StoryWorkspaceTemporalProjection

**Purpose:** Build a read-only temporal query index from StoryBuilder's canonical workspace events.

**Ownership:** StoryBuilder.StoryForge consumes StoryBuilder.Core; it does not own a second canonical workspace or write projected records back into the workspace.

## Behavior

- Keeps stable canonical event, timeline, participant, and source IDs.
- Requires an explicit coordinate resolver because `InWorldDateOrPeriod` is human-authored text and may be approximate or unresolved.
- Skips events without an assigned existing timeline or without a resolved coordinate.
- Preserves summary text and source provenance.
- Maps uncertain/unrecognized canon statuses conservatively to `Proposed`; it never silently upgrades an event to confirmed canon.

## Use case

An author assigns an event to a timeline and later supplies a reliable coordinate mapping. A timeline, dossier, or observer view can query the projection without duplicating the event in persistent storage. Save/reopen continues to serialize the canonical `StoryWorkspace` through StoryBuilder.Persistence and FSM_Serialization.
