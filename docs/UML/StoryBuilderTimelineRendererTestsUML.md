# StoryBuilderTimelineRendererTests UML

```mermaid
classDiagram
    class StoryBuilderTimelineRendererTests {
        +TimelineMapShowsEventToBookSectionPortrayalRail()
    }
    StoryBuilderTimelineRendererTests ..> StoryBuilderTimelineRenderer : verifies
    StoryBuilderTimelineRendererTests ..> StoryBuilderSession : constructs canonical data
```

## Use cases

- A world event is assigned to a timeline.
- A manuscript section references that event.
- The generated SVG shows the event and the section that portrays it.
