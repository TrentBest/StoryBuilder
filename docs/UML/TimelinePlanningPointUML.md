# TimelinePlanningPoint UML

TimelinePlanningPoint belongs to one TimelineDefinition and keeps an unresolved idea stable while the author develops it. It records approximate placement, author intent, candidate alternatives, affected scene/event links, and the stable identity of any event created through explicit promotion.

## Class card

- TimelinePlanningPoint: Id, TimelineId, Label, Intent, ApproximatePlacement, PlacementKind, Status, Alternatives, RelatedSceneIds, RelatedEventIds, PromotedEventId
- TimelineDefinition: stable identity and parent/branch lineage
- SceneRecord / StoryEvent: optional related records; the planning point does not replace them

## Use cases

- Add a provisional Plot Twist without inventing a precise date.
- Brainstorm alternatives without changing accepted world history.
- Link affected scenes and events while keeping the point provisional.
- Explicitly promote the point to one Proposed event; the point becomes Superseded and stores the event ID. Repeating promotion returns the same event.
- Show the point in the GUI.SVG timeline view with an explicit planning-point marker.
