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
  +int Sequence
  +SceneBeatKind Kind
  +string? ActorLabel
  +string? Action
  +string? Reaction
  +string? Dialogue
  +string? PoseOrPosition
  +string Attribution
}
SceneRecord o-- SceneBeat
```

## Use case

Capture a fight as ordered actions and reactions, play it out, or represent it as a pose sequence. The same ordered beats can become a first-pass narrative draft without claiming to simulate the fight.
