# SceneComposer UML

## Class diagram

```mermaid
classDiagram
class SceneComposer {
  +Create(title, location, mode) SceneRecord
  +AddBeat(scene, beat) SceneRecord
  +ComposeNarration(scene) string
}
SceneComposer ..> SceneRecord
SceneComposer ..> SceneBeat
```

## Use case

After a player or author records beats, the composer produces an editable narration draft in sequence order. It does not silently fill gaps in the record.
