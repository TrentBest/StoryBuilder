# StoryBuilderSession UML

## Class diagram

```mermaid
classDiagram
class StoryBuilderSession {
  +StoryWorkspace Workspace
  +CharacterBuilder CharacterBuilder
  +SceneComposer SceneComposer
  +VocabularyIndex Vocabulary
  +CreateCharacter(name, templateName) CharacterProfile
  +UpdateCharacter(character) CharacterProfile
  +CreateScene(title, location, mode) SceneRecord
  +UpdateScene(scene) SceneRecord
  +AddHistoricalClaim(claim) HistoricalClaim
  +AddEvent(storyEvent) StoryEvent
  -IndexCharacter(character)
  -IndexScene(scene)
}
StoryBuilderSession *-- StoryWorkspace
StoryBuilderSession *-- VocabularyIndex
StoryBuilderSession ..> CharacterBuilder
StoryBuilderSession ..> SceneComposer
```

## Use case

The host routes Casting and Stage edits through one workspace session. When a character trait or scene beat changes, the session removes that target's previous word occurrences and indexes the updated record. Searches therefore follow edits instead of preserving stale counts.
