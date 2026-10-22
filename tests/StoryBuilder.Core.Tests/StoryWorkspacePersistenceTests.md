# StoryWorkspacePersistenceTests

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

Verifies that the StoryBuilder binary document preserves workspace identity, characters, scene beats, event-to-manuscript-section links, event-to-scene-beat links, observer knowledge, source provenance, campaigns, participant choices, and synonym mappings. It also verifies that unknown format versions and truncated payloads are rejected.

## Format boundary

The document is versioned and bounded. It uses the FSM_Serialization byte-stream contract, while StoryBuilder owns the semantic field order. Older versions remain readable; scene beats from versions before 8 receive no event links.

## Verification

Run the test project from the repository root using the Development guide. The round-trip case asserts that a timeline event's identity survives links to both a manuscript section and a specific scene beat.
