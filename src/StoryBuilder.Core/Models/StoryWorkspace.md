# StoryWorkspace

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

StoryWorkspace groups the implementation record for an Experience with creator-owned StoryPremise proposals and related fictional records: characters, scenes, claims, events, campaigns, participant actions, sources, and continuity issues. The collections remain distinct so implementation facts do not silently become fictional canon. Premises preserve the creator's first idea as an editable proposal rather than turning it into an accepted world fact.

## UML class card

See the focused [StoryWorkspace UML](../../../docs/UML/StoryWorkspaceUML.md) for the class diagram and use case.

## Boundary

This is an in-memory aggregate, not a database or serialization format. The host is responsible for durable storage and user interaction. The domain types provide stable identities and explicit status/provenance fields for those adapters.
