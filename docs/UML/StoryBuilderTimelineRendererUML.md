# StoryBuilderTimelineRenderer UML

## Class diagram

```mermaid
classDiagram
    class StoryBuilderSession {
        +Workspace
        +CreateTimeline(name, description, parentTimelineId)
        +CreatePlanningPoint(timelineId, label, approximatePlacement, intent)
        +CreateTimelineEvent(timelineId, title, summary, approximatePlacement)
        +LinkEventToManuscriptSection(manuscriptId, sectionId, eventId, linked)
        +LinkEventToSceneBeat(sceneId, beatId, eventId, linked)
        +Changed
    }
    class StoryWorkspace {
        +Timelines
        +PlanningPoints
        +Events
        +Manuscripts
        +Scenes
    }
    class TimelineDefinition {
        +Id
        +Name
        +ParentTimelineId
        +IsBranch
    }
    class StoryEvent {
        +Id
        +Title
        +TimelineId
        +InWorldDateOrPeriod
    }
    class ManuscriptEdition {
        +Id
        +Title
        +Sections
    }
    class ManuscriptSection {
        +Id
        +Sequence
        +Kind
        +Title
        +EventIds
    }
    class SceneRecord {
        +Id
        +Title
        +Beats
    }
    class SceneBeat {
        +Id
        +Sequence
        +Kind
        +EventIds
    }
    class StoryBuilderTimelineRenderer {
        +Render(session) string
        +Derived per-timeline event ordinals
    }
    class SvgGuiRenderer {
        +Render(model, width, height) string
    }
    StoryBuilderSession --> StoryWorkspace : owns
    StoryWorkspace --> TimelineDefinition : stores
    StoryWorkspace --> StoryEvent : stores
    StoryWorkspace --> ManuscriptEdition : stores
    StoryWorkspace --> SceneRecord : stores
    ManuscriptEdition --> ManuscriptSection : contains
    ManuscriptSection --> StoryEvent : portrays in book
    SceneRecord --> SceneBeat : contains
    SceneBeat --> StoryEvent : portrays in scene
    StoryBuilderTimelineRenderer ..> StoryBuilderSession : reads
    StoryBuilderTimelineRenderer ..> SvgGuiRenderer : renders through GUI.SVG
    StoryBuilderTimelineRenderer ..> TimelineDefinition : projects
    StoryBuilderTimelineRenderer ..> StoryEvent : marks
    StoryBuilderTimelineRenderer ..> ManuscriptSection : draws book rail
    StoryBuilderTimelineRenderer ..> SceneBeat : draws scene-beat rail
```

## Use cases

- **Create a root:** the author creates a timeline with no parent and sees a root node.
- **Branch a history:** the author selects an existing parent, creates a child timeline, and sees the declared relationship.
- **Place an event:** the author adds a key event with approximate or unresolved placement; a numbered event marker is drawn on its timeline.
- **Link event to product:** a manuscript section references the event by stable ID and appears on the book portrayal rail.
- **Link event to scene beat:** a beat references the same event ID and appears on the scene-beat rail.
- **Keep the model authoritative:** refresh or restore the workspace and rebuild the diagram from canonical records; SVG is not a persistence layer.

## Scope boundary

Parent connectors represent branch lineage. Book portrayal connectors represent where a world event is recounted in manuscript order. Scene-beat connectors represent where the event is enacted or observed in the scene record. None of these relationships alone implies causality or measured world chronology.
