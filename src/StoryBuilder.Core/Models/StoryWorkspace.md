# StoryWorkspace

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

StoryWorkspace groups the implementation record for an Experience with creator-owned StoryPremise proposals and related fictional records: timelines, characters, canonical places, scenes, claims, events, campaigns, participant actions, sources, continuity issues, and immutable published-book snapshots. The collections remain distinct so implementation facts do not silently become fictional canon. Timeline definitions are canonical domain records; executable Process Groups are optional runtime projections. Branch timelines must reference a parent already present in the same workspace.

## UML class card

See the focused [StoryWorkspace UML](../../../docs/UML/StoryWorkspaceUML.md) for the class diagram and use case.

## Boundary

This is an in-memory aggregate, not a database or serialization format. The host is responsible for durable storage and user interaction. The domain types provide stable identities and explicit status/provenance fields for those adapters. `PublishedEditions` are append-only through the public workspace API; each publication points to its source manuscript, stores exact Markdown plus SHA-256, and may link to a prior edition when correcting a publication. Timeline Process Group execution is supplied by the optional StoryBuilder.FSM adapter.
