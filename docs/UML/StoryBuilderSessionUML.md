# StoryBuilderSession UML

## Class diagram

```mermaid
classDiagram
class StoryBuilderSession {
  +StoryWorkspace Workspace
  +event Changed
  +CharacterBuilder CharacterBuilder
  +SceneComposer SceneComposer
  +VocabularyIndex Vocabulary
  +CreateTimeline(name, description, parentTimelineId) TimelineDefinition
  +CreatePremise(vision, options) StoryPremise
  +UpdatePremise(premise) StoryPremise
  +CreateCharacter(name, templateName) CharacterProfile
  +UpdateCharacter(character) CharacterProfile
  +CreateScene(title, location, mode) SceneRecord
  +UpdateScene(scene) SceneRecord
  +AddHistoricalClaim(claim) HistoricalClaim
  +AddEvent(storyEvent) StoryEvent
  +AddSource(source) SourceRecord
  +AddContinuityIssue(issue) ContinuityIssue
  +AddCampaign(campaign) CampaignRecord
  +AddParticipantAction(action) ParticipantAction
  +UpdateParticipantAction(action) ParticipantAction
  -IndexPremise(premise)
  -IndexCharacter(character)
  -IndexScene(scene)
}
StoryBuilderSession *-- StoryWorkspace
StoryBuilderSession *-- VocabularyIndex
StoryBuilderSession ..> TimelineDefinition : creates
StoryBuilderSession ..> CharacterBuilder
StoryBuilderSession ..> SceneComposer
```

## Use case

The Narrator, timeline authoring, Casting, and Stage route edits through one workspace session. CreateTimeline registers canonical timeline definitions and supports branch lineage without replacing the parent. Premise, character, and scene edits refresh the corresponding vocabulary occurrences. Host adapters observe Changed to autosave the workspace without coupling the domain to a storage technology.
