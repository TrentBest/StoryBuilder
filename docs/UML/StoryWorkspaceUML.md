# StoryWorkspace UML

## Class diagram

```mermaid
classDiagram
class StoryWorkspace {
  +Guid Id
  +string Name
  +ExperienceDefinition Experience
  +List~CharacterProfile~ Characters
  +List~SceneRecord~ Scenes
  +List~HistoricalClaim~ HistoricalClaims
  +List~StoryEvent~ Events
  +List~CampaignRecord~ Campaigns
  +List~ParticipantAction~ ParticipantActions
  +List~SourceRecord~ Sources
  +List~ContinuityIssue~ ContinuityIssues
  +AddCharacter(character)
  +AddScene(scene)
  +AddClaim(claim)
  +AddEvent(storyEvent)
  +AddCampaign(campaign)
}
StoryWorkspace o-- ExperienceDefinition
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

The host creates one workspace for an Experience, then adds fictional and campaign records without mixing them with the Experience's implementation record. Storage and UI are intentionally owned by the host or an adapter.
