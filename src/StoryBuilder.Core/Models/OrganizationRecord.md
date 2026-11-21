# OrganizationRecord

## Purpose
Represents an organized actor in a story world: a house, faction, government, institution, corporation, guild, military body, or other collective. Organizations have stable identity and their own lifecycle status; they are not characters.

## Data
- Stable GUID identity.
- Name, descriptive kind, and description.
- Draft/proposed/accepted/rejected/deprecated status through `CanonStatus`.
- Creation and update timestamps.

## Relationships
`StoryEvent.OrganizationParticipantIds` references organizations independently from `StoryEvent.ParticipantIds`, which continues to reference characters. Historical claims can reference an organization through `RelatedEntityIds`.

## Authoring and persistence
`StoryBuilderSession.CreateOrganization` and `UpdateOrganization` maintain the vocabulary index. Workspace format 17 persists organization records and event organization participation; formats 1–16 restore with empty organization collections and empty organization-participant lists.
