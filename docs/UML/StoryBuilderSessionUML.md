# StoryBuilderSession UML

## Class diagram

```mermaid
classDiagram
class StoryBuilderSession {
  +StoryWorkspace Workspace\n  +event Changed
  +CharacterBuilder CharacterBuilder
  +SceneComposer SceneComposer
  +VocabularyIndex Vocabulary
  +CreateCharacter(name, templateName) CharacterProfile
  +UpdateCharacter(character) CharacterProfile
  +CreateScene(title, location, mode) SceneRecord
  +UpdateScene(scene) SceneRecord
  +AddHistoricalClaim(claim) HistoricalClaim
  +AddEvent(storyEvent) StoryEvent\n  +AddSource(source) SourceRecord\n  +AddContinuityIssue(issue) ContinuityIssue\n  +AddCampaign(campaign) CampaignRecord\n  +AddParticipantAction(action) ParticipantAction\n  +UpdateParticipantAction(action) ParticipantAction
  -IndexPremise(premise)\n  -IndexCharacter(character)
  -IndexScene(scene)
}
StoryBuilderSession *-- StoryWorkspace
StoryBuilderSession *-- VocabularyIndex
StoryBuilderSession ..> CharacterBuilder
StoryBuilderSession ..> SceneComposer
```

## Use case

The Narrator, Casting, and Stage route edits through one workspace session. When a character trait or scene beat changes, the session removes that target's previous word occurrences and indexes the updated record. Searches therefore follow edits instead of preserving stale counts.
