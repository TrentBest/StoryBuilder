# StoryWorkspace UML

## Class diagram

```mermaid
classDiagram
class StoryWorkspace {
  +Guid Id
  +string Name
  +ExperienceDefinition Experience
  +IReadOnlyList~StoryPremise~ Premises
  +IReadOnlyList~CharacterProfile~ Characters
  +IReadOnlyList~SceneRecord~ Scenes
  +IReadOnlyList~HistoricalClaim~ HistoricalClaims
  +IReadOnlyList~StoryEvent~ Events
  +IReadOnlyList~CampaignRecord~ Campaigns
  +IReadOnlyList~ParticipantAction~ ParticipantActions
  +IReadOnlyList~SourceRecord~ Sources
  +IReadOnlyList~ContinuityIssue~ ContinuityIssues
  +AddPremise(premise)
  +ReplacePremise(premise)
  +AddCharacter(character)
  +ReplaceCharacter(character)
  +AddScene(scene)
  +ReplaceScene(scene)
  +AddClaim(claim)
  +AddEvent(storyEvent)
  +AddCampaign(campaign)
  +AddParticipantAction(action)
  +AddSource(source)
  +AddContinuityIssue(issue)
}
StoryWorkspace o-- ExperienceDefinition
StoryWorkspace o-- StoryPremise
StoryWorkspace o-- CharacterProfile
StoryWorkspace o-- SceneRecord
StoryWorkspace o-- HistoricalClaim
StoryWorkspace o-- StoryEvent
StoryWorkspace o-- CampaignRecord
StoryWorkspace o-- ParticipantAction
StoryWorkspace o-- SourceRecord
StoryWorkspace o-- ContinuityIssue
```

## Use case

The host creates one workspace for an Experience, captures StoryPremise proposals through the Narrator, then adds fictional and campaign records without mixing them with the Experience's implementation record. The workspace exposes read-only collection views and controlled add/replace operations for character and scene edits. Storage and UI remain host or adapter responsibilities.
