# StoryBuilderSession UML

```mermaid
classDiagram
    class StoryBuilderSession {
        +Workspace
        +CreateTimeline(name, description, parentTimelineId) TimelineDefinition
        +CreatePlanningPoint(timelineId, label, placement, intent) TimelinePlanningPoint
        +CreateTimelineEvent(timelineId, title, summary, placement) StoryEvent
        +LinkEventToManuscriptSection(manuscriptId, sectionId, eventId, linked)
        +LinkEventToSceneBeat(sceneId, beatId, eventId, linked) SceneRecord
        +CreateManuscript(title, subtitle, author) ManuscriptEdition
        +Changed
    }
    class StoryWorkspace
    class ObserverKnowledgeProjectionService
    StoryBuilderSession --> StoryWorkspace : owns canonical records
    StoryBuilderSession ..> ObserverKnowledgeProjectionService : observer projection service is called by NarratorAgent
```

## Use cases

- Coordinate timeline, manuscript, and scene-beat authoring without coupling the core to a host.
- Keep event-to-manuscript and event-to-beat links as relationships to one stable world-event identity.
- Notify living views when records change.
