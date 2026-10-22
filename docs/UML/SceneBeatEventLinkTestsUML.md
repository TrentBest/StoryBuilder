# SceneBeatEventLinkTests UML

```mermaid
classDiagram
    class SceneBeatEventLinkTests {
        +LinkingIsIdempotentAndUnlinkingPreservesEventAndBeat()
        +LinkingRejectsUnknownSceneBeatOrEvent()
    }
    SceneBeatEventLinkTests ..> StoryBuilderSession : verifies links
    StoryBuilderSession ..> SceneBeat : updates EventIds
    StoryBuilderSession ..> StoryEvent : preserves canonical event
```

## Use cases

- Link an event to a scene beat without duplicating it.
- Remove a link without deleting the scene beat or world event.
- Reject missing scene, beat, or event identities.
