# VesperReachAuthoredWorkspaceTests

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

Exercises a complete authored first-contact workspace through the StoryBuilder domain. The scenario keeps canonical events, attributed sources, historical claims, observer-specific knowledge, scene/place links, and manuscript-section links distinct while proving that a binary persistence round trip preserves the workspace's key identities and relationships.

## Observer-knowledge contract

- Ilyan directly examines the primary instrument record and may receive the signal as a direct fact.
- Mara learns of the signal through the station's later public bulletin and may receive it only as an attributed report.
- Teren receives neither account and must not receive the signal in the projection.
- The courier's independent account remains a separate attributed event and does not rewrite the original bulletin.
- A proposed historical claim remains proposed; acceptance of related events does not silently promote it.

## Persistence contract

The test creates a scene with an authored beat, assigns the scene to Vesper Reach, and links the scene and three distinct events into a manuscript section. It then packs and restores the workspace through StoryWorkspaceBinaryDocument, checks stable event and manuscript-link identities, and validates both the original and restored workspaces.

## UML class card

See [VesperReachAuthoredWorkspaceTests UML](../../docs/UML/VesperReachAuthoredWorkspaceTestsUML.md).

## Verification boundary

This is an integrated domain/persistence test, not a production authentication, publication authorization, Foundry execution, or browser-host integration test. Observer knowledge is a domain disclosure projection; it does not grant workspace access.
