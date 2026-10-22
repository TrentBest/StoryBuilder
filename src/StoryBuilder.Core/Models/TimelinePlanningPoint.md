# TimelinePlanningPoint

**User moniker:** Plot anchor / unresolved timeline point

**Purpose:** Preserves a provisional story idea at a chosen timeline and approximate location before the author knows its meaning. A planning point is not automatically a world event or accepted canon.

## UML class card

- Identity: Id and TimelineId
- Intent: Label and optional Intent
- Placement: ApproximatePlacement and TimelinePlacementKind
- Progress: TimelinePlanningStatus
- Exploration: Alternatives
- Future traceability: RelatedSceneIds and RelatedEventIds
- History: CreatedAtUtc and UpdatedAtUtc

## Use cases

- Add Plot Twist near the midpoint while its meaning is unknown.
- Preserve several candidate explanations without rewriting canon.
- Progress from placeholder to brainstorming, specification, review, and an explicit decision.
- Link a resolved point to event and scene records while retaining its identity.
