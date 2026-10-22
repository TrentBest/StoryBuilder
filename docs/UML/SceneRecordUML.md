# SceneRecord UML

## Class diagram

```mermaid
classDiagram
    class SceneRecord {
        +Guid Id
        +string Title
        +SceneCaptureMode CaptureMode
        +SceneStatus Status
        +List~SceneBeat~ Beats
        +AddBeat(beat) SceneRecord
        +ReorderBeats(beatIds) SceneRecord
    }
    class SceneBeat {
        +Guid Id
        +int Sequence
        +SceneBeatKind Kind
        +string? ActorLabel
        +string? Action
        +string? Reaction
        +string? Dialogue
        +string Attribution
        +List~Guid~ EventIds
    }
    class StoryEvent {
        +Guid Id
        +string Title
    }
    SceneRecord o-- SceneBeat : contains
    SceneBeat --> StoryEvent : references by stable ID
```

## Use cases

- Capture a scene as ordered actions and reactions.
- Link an event to the exact beat that portrays it.
- Reorder beats without duplicating or mutating the referenced world event.
