# StoryWorkspace

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

StoryWorkspace groups the implementation record for an Experience with creator-owned StoryPremise proposals and related fictional records: timelines, characters, canonical places, scenes, claims, events, campaigns, adventures, quests, quest objectives, participant actions, sources, continuity issues, immutable published-book snapshots, and pinned sourcebook attachments. The collections remain distinct so implementation facts do not silently become fictional canon. Timeline definitions are canonical domain records; executable Process Groups are optional runtime projections. Branch timelines must reference a parent already present in the same workspace.

## UML class card

See the focused [StoryWorkspace UML](../../../docs/UML/StoryWorkspaceUML.md) for the class diagram and use case.

## Boundary

This is an in-memory aggregate, not a database or serialization format. The host is responsible for durable storage and user interaction. The domain types provide stable identities and explicit status/provenance fields for those adapters. `PublishedEditions` are append-only through the public workspace API; each publication points to its source manuscript, stores exact Markdown plus SHA-256, and may link to a prior edition when correcting a publication. Timeline Process Group execution is supplied by the optional StoryBuilder.FSM adapter.

## Organizations

The workspace stores organized actors as first-class `OrganizationRecord` entries, separate from characters. Events retain `ParticipantIds` for characters and use `OrganizationParticipantIds` for houses, factions, governments, institutions, and other collectives. Historical claims can reference organizations through the generic `RelatedEntityIds` relationship. Organization lifecycle status remains explicit; imported/proposed material is not automatically accepted canon.

## Campaign relationship integrity

Campaigns may reference only characters, scenes, and events already present in the workspace. Participant actions must belong to an existing campaign and may reference only existing characters and resulting events. These relationships are checked when records are added, rather than waiting for a later persistence validation pass. Campaign relationship lists are copied and de-duplicated when the campaign is added so the workspace owns a stable set of references.

## Campaign planning hierarchy

Campaigns own adventures. Adventures group quests. Quests own ordered objectives, but objective order is authoring guidance rather than a mandatory execution sequence. Quests may also belong directly to a campaign without an adventure. Objectives can optionally reference an existing scene or target event. The workspace enforces campaign ownership at creation and update boundaries, and the validator repeats those checks after restore. These records describe intent; they do not dictate participant choices or automatically resolve SURPS rules.
