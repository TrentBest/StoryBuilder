# StoryWorkspace UML

## Class diagram

```mermaid
classDiagram
class StoryWorkspace {
  +Guid Id
  +string Name
  +ExperienceDefinition Experience
  +IReadOnlyList~TimelineDefinition~ Timelines
  +IReadOnlyList~TimelinePlanningPoint~ PlanningPoints
  +IReadOnlyList~ManuscriptEdition~ Manuscripts
  +IReadOnlyList~PublishedBookEdition~ PublishedEditions
  +IReadOnlyList~StoryPremise~ Premises
  +IReadOnlyList~PlaceRecord~ Places
  +IReadOnlyList~CharacterProfile~ Characters
  +IReadOnlyList~OrganizationRecord~ Organizations
  +IReadOnlyList~SceneRecord~ Scenes
  +IReadOnlyList~HistoricalClaim~ HistoricalClaims
  +IReadOnlyList~StoryEvent~ Events
  +IReadOnlyList~CampaignRecord~ Campaigns
  +IReadOnlyList~SourcebookAttachment~ SourcebookAttachments
  +IReadOnlyList~AdventureRecord~ Adventures
  +IReadOnlyList~QuestRecord~ Quests
  +IReadOnlyList~QuestObjectiveRecord~ QuestObjectives
  +IReadOnlyList~ParticipantAction~ ParticipantActions
  +IReadOnlyList~SourceRecord~ Sources
  +IReadOnlyList~ContinuityIssue~ ContinuityIssues
  +AddTimeline(timeline)
  +AddPublishedEdition(edition)
  +AddPremise(premise)
  +ReplacePremise(premise)
  +AddCharacter(character)
  +AddOrganization(organization)
  +ReplaceOrganization(organization)
  +ReplaceCharacter(character)
  +AddScene(scene)
  +ReplaceScene(scene)
  +AddClaim(claim)
  +AddEvent(storyEvent)
  +AddCampaign(campaign)
  +AddSourcebookAttachment(attachment)
  +AddAdventure(adventure)
  +ReplaceAdventure(adventure)
  +AddQuest(quest)
  +ReplaceQuest(quest)
  +AddQuestObjective(objective)
  +ReplaceQuestObjective(objective)
  +AddParticipantAction(action)
  +AddSource(source)
  +AddContinuityIssue(issue)
}
StoryWorkspace o-- ExperienceDefinition
StoryWorkspace o-- TimelineDefinition
StoryWorkspace o-- TimelinePlanningPoint
StoryWorkspace o-- ManuscriptEdition
StoryWorkspace o-- PublishedBookEdition
StoryWorkspace o-- StoryPremise
StoryWorkspace o-- PlaceRecord
PublishedBookEdition --> ManuscriptEdition : source manuscript
PublishedBookEdition --> PublishedBookEdition : corrected from
StoryWorkspace o-- CharacterProfile
StoryWorkspace o-- OrganizationRecord
StoryWorkspace o-- SceneRecord
StoryWorkspace o-- HistoricalClaim
StoryWorkspace o-- StoryEvent
StoryEvent --> CharacterProfile : ParticipantIds
StoryEvent --> OrganizationRecord : OrganizationParticipantIds
StoryWorkspace o-- CampaignRecord
StoryWorkspace o-- SourcebookAttachment
StoryWorkspace o-- AdventureRecord
StoryWorkspace o-- QuestRecord
StoryWorkspace o-- QuestObjectiveRecord
AdventureRecord --> CampaignRecord : owned by
QuestRecord --> CampaignRecord : owned by
QuestRecord --> AdventureRecord : optional parent
QuestObjectiveRecord --> QuestRecord : belongs to
QuestObjectiveRecord --> SceneRecord : optional scene context
QuestObjectiveRecord --> StoryEvent : optional target event
SourcebookAttachment --> CampaignRecord : pins exact edition for
StoryWorkspace o-- ParticipantAction
StoryWorkspace o-- SourceRecord
StoryWorkspace o-- ContinuityIssue
TimelineDefinition "0..1" --> "0..*" TimelineDefinition : branch lineage
```

## Use case

The host creates one workspace for an Experience, captures StoryPremise proposals through the Narrator, and registers timeline definitions before executing them through an optional runtime adapter. A branch must point to a parent already registered in that workspace. The host then adds fictional and campaign records without mixing them with the Experience's implementation record. Campaigns can pin exact sourcebook editions, group adventures, and organize quests into ordered objectives. These are preparation structures, not a forced plot path; objectives may be pursued in any order and participant choices remain independent records. The workspace exposes read-only collection views; storage and UI remain host or adapter responsibilities. Published editions are append-only snapshots with content hashes and correction lineage, separate from mutable manuscript drafts.
