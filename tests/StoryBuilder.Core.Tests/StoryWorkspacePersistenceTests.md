# StoryWorkspacePersistenceTests

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

Verifies that the StoryBuilder binary document preserves workspace identity, characters, scene beats, event-to-manuscript-section links, event-to-scene-beat links, author-accepted narration snapshots with beat-level text provenance and event claims, observer knowledge, source provenance, campaigns, participant choices, visual assets and review evidence, exact page-composition visual revision selections, discovery settings, and synonym mappings. It also verifies that invalid composition-to-revision ownership, unknown format versions, and truncated payloads are rejected.

## Format boundary

The document is versioned and bounded. It uses the FSM_Serialization byte-stream contract, while StoryBuilder owns the semantic field order. Format 5 persists accepted narration provenance in each manuscript section. Format 4 remains readable and restores with an empty narration acceptance history. Earlier visual-revision migration behavior is retained by the current reader.

## Verification

Run the test project from the repository root using the Development guide. The round-trip case asserts that a timeline event's identity survives links to both a manuscript section and a specific scene beat, and that accepted narration text, author/provider attribution, beat ranges, and event claims survive a binary round-trip. Visual composition tests assert that the selected revision survives a binary round-trip, and that adding or replacing a composition with a revision from another asset fails without mutating the workspace.


## Current development schema

The persistence tests exercise the `SBW1` workspace signature and current schema version 5, while the reader retains compatibility with schema version 4. They reject the retired `SBWS` development signature and unsupported schema versions.
