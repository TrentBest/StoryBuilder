# StoryBuilderTimelineRendererTests UML

```mermaid
classDiagram
    class StoryBuilderTimelineRendererTests {
        +TimelineMapShowsEventToBookSectionPortrayalRail()
        +TimelineMapShowsEventToSceneBeatPortrayalRail()
    }
    StoryBuilderTimelineRendererTests ..> StoryBuilderTimelineRenderer : verifies
    StoryBuilderTimelineRendererTests ..> StoryBuilderSession : constructs canonical data
```

## Use cases

- A manuscript section references an event and appears on the book portrayal rail.
- A scene beat references the same event and appears on the scene-beat rail.
