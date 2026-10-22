# StoryBuilderObserverProjectionRendererTests UML

```mermaid
classDiagram
    class StoryBuilderObserverProjectionRendererTests {
        +SvgShowsAcceptedKnowledgeAndOmitsUnknownEvents()
    }
    StoryBuilderObserverProjectionRendererTests ..> ObserverKnowledgeProjectionService : obtains projection
    StoryBuilderObserverProjectionRendererTests ..> StoryBuilderObserverProjectionRenderer : verifies SVG
```

## Use cases

- Accepted observer knowledge appears in the generated view.
- Unknown event titles and unrestricted world summaries do not appear in the view.
