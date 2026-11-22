# StoryWorkspacePersistenceTests

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

Verifies that the StoryBuilder binary document preserves workspace identity, characters, scene beats, event-to-manuscript-section links, event-to-scene-beat links, observer knowledge, source provenance, campaigns, participant choices, visual assets and review evidence, exact page-composition visual revision selections, discovery settings, and synonym mappings. It also verifies that invalid composition-to-revision ownership, unknown format versions, and truncated payloads are rejected.

## Format boundary

The document is versioned and bounded. It uses the FSM_Serialization byte-stream contract, while StoryBuilder owns the semantic field order. Format 18 persists the exact visual revision selected by a page composition. Older workspaces remain readable; pre-format-18 compositions restore with no selected visual revision and cannot be exported as approved artwork until an author makes that selection.

## Verification

Run the test project from the repository root using the Development guide. The round-trip case asserts that a timeline event's identity survives links to both a manuscript section and a specific scene beat. Visual composition tests assert that the selected revision survives a binary round-trip, and that adding or replacing a composition with a revision from another asset fails without mutating the workspace.
