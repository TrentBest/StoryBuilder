# StoryBuilderObserverProjectionRenderer UML

```mermaid
classDiagram
    class StoryBuilderObserverProjectionRenderer {
        +Render(workspace, projection) string
    }
    class ObserverKnowledgeProjection {
        +ObserverId
        +AsOfEventId
        +Events
    }
    class ObserverKnownEvent {
        +EventId
        +PermittedClaims
    }
    class StoryWorkspace {
        +Events
    }
    class SvgGuiRenderer {
        +Render(model, width, height) string
    }
    StoryBuilderObserverProjectionRenderer ..> ObserverKnowledgeProjection : draws only projected events
    StoryBuilderObserverProjectionRenderer ..> StoryWorkspace : resolves author-facing labels
    StoryBuilderObserverProjectionRenderer ..> SvgGuiRenderer : renders through GUI.SVG
    ObserverKnowledgeProjection --> ObserverKnownEvent : contains
```

## Use cases

- Show the author which accepted claims are available to a selected observer at a chosen event anchor.
- Display the allowed narration mode and accepted note for each projected event.
- Omit unknown events from the SVG rather than rendering the whole world and styling hidden items afterward.
