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
        +Changed
    }
    class StoryWorkspace {
        +Timelines
        +PlanningPoints
        +Events
        +Manuscripts
    }
    class TimelineDefinition {
        +Id
        +Name
        +Description
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
    ManuscriptEdition --> ManuscriptSection : contains
    ManuscriptSection --> StoryEvent : portrays by stable ID
    StoryBuilderTimelineRenderer ..> StoryBuilderSession : reads
    StoryBuilderTimelineRenderer ..> SvgGuiRenderer : renders through GUI.SVG
    StoryBuilderTimelineRenderer ..> TimelineDefinition : projects
    StoryBuilderTimelineRenderer ..> StoryEvent : marks
    StoryBuilderTimelineRenderer ..> ManuscriptSection : draws portrayal rail
```

## Use cases

- **Create a root:** the author creates a timeline with no parent and sees a root node.
- **Branch a history:** the author selects an existing parent, creates a child timeline, and sees the declared relationship.
- **Place an event:** the author adds a key event with approximate or unresolved placement; the numbered event marker is drawn on its timeline; the same view ordinal appears on its portrayal rail. This ordinal is a visual label, not a replacement for the event's stable GUID.
- **Link event to product:** a manuscript section references the event by stable ID and appears on the portrayal rail with the actual book and section titles.
- **Keep the model authoritative:** refresh or restore the workspace and rebuild the diagram from canonical records; SVG is not a persistence layer.

## Scope boundary

Parent connectors represent branch lineage. Portrayal connectors represent where a world event is recounted in manuscript order. Neither one implies causality or measured world chronology. Scene/beat-level links and observer-aware recountings remain future work.
