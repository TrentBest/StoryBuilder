# TimelinePlanningPoint UML

TimelinePlanningPoint belongs to one TimelineDefinition and keeps an unresolved idea stable while the author develops it. It records approximate placement, author intent, candidate alternatives, and future links to scene/event records.

## Class card

- TimelinePlanningPoint: Id, TimelineId, Label, Intent, ApproximatePlacement, PlacementKind, Status, Alternatives, RelatedSceneIds, RelatedEventIds
- TimelineDefinition: stable identity and parent/branch lineage
- SceneRecord / StoryEvent: optional related records; the planning point does not replace them

## Use cases

- Add a provisional Plot Twist without inventing a precise date.
- Brainstorm alternatives without changing accepted world history.
- Connect a chosen explanation to event and scene records later.
- Show the point in the GUI.SVG timeline view with an explicit planning-point marker.
