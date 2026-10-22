# StoryBuilderSession UML

```mermaid
classDiagram
    class StoryBuilderSession {
        +Workspace
        +CreateTimeline(name, description, parentTimelineId) TimelineDefinition
        +CreatePlanningPoint(timelineId, label, placement, intent) TimelinePlanningPoint
        +CreateTimelineEvent(timelineId, title, summary, placement) StoryEvent
        +UpdateTimelineEvent(event) StoryEvent
        +LinkEventCausality(causeId, consequenceId, linked)
        +SetPlanningPointAlternatives(pointId, alternatives) TimelinePlanningPoint
        +LinkPlanningPointToScene(pointId, sceneId, linked) TimelinePlanningPoint
        +LinkPlanningPointToEvent(pointId, eventId, linked) TimelinePlanningPoint
        +PublishManuscript(manuscriptId, correctionNotes) PublishedBookEdition
        +LinkEventToManuscriptSection(manuscriptId, sectionId, eventId, linked)
        +LinkEventToSceneBeat(sceneId, beatId, eventId, linked) SceneRecord
        +CreateManuscript(title, subtitle, author) ManuscriptEdition
        +Changed
    }
    class StoryWorkspace
    class SceneRecord
    class SceneBeat
    class TimelinePlanningPoint
    class PublishedBookEdition
    class StoryEvent
    StoryBuilderSession --> StoryWorkspace : owns canonical records
    StoryBuilderSession ..> TimelinePlanningPoint : edits alternatives and impact links
    StoryBuilderSession ..> PublishedBookEdition : freezes Markdown snapshots
    StoryBuilderSession ..> SceneRecord : updates scene beat links
    SceneRecord o-- SceneBeat : contains
    SceneBeat --> StoryEvent : references by stable ID
```

## Use cases

- Coordinate timeline, manuscript, and scene-beat authoring without coupling the core to a host.
- Keep event-to-manuscript and event-to-beat links as relationships to one stable world-event identity.
- Edit cause/consequence edges reciprocally and inspect cycles without silent repairs.
- Explore provisional alternatives without promoting them automatically to story canon.
- Freeze a manuscript publication and link corrected editions without rewriting prior output.
- Notify living views when records change.
