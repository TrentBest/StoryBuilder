# StoryWorkspacePersistenceTests

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

These tests verify that the StoryBuilder-specific binary document preserves workspace identity, characters, scene beats, source provenance, campaigns, participant choices, and explicit synonym mappings. They also verify that unknown format versions and truncated payloads are rejected.

## Format boundary

The document is versioned and bounded. It uses the FSM_Serialization byte-stream contract, but StoryBuilder owns the semantic field order for its workspace records. Word occurrences are derived on restore from source records; explicit synonym mappings are persisted.

## Verification

Run the test project from the repository root using the Development guide.
