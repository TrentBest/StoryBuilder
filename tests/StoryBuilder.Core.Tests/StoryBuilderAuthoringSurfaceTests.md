# StoryBuilderAuthoringSurfaceTests

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

These tests verify the semantic authoring surface as an integration seam: stable presentation node IDs, explicit command metadata, updated projection after domain mutation, and rejection of unsupported commands without changing the workspace.

## Coverage

- `RootUsesStableNodeIdsAndExplicitCommandIds` checks the four supported action IDs.
- `ExecutingCommandUpdatesNextSemanticSnapshot` proves the next `Root` snapshot reflects a created timeline.
- `UnsupportedCommandLeavesProjectionUnchanged` verifies the safety boundary.

## UML class card

See [StoryBuilderAuthoringSurface UML](../../docs/UML/StoryBuilderAuthoringSurfaceUML.md) for the surface/controller/session relationship and command-refresh sequence.

## Verification status

The tests have been committed to `development`. No successful CI result is currently available for the latest commits; execution remains unverified.
