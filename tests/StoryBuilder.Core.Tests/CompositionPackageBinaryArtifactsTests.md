# CompositionPackageBinaryArtifactsTests

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

Verifies that a captured binary package validates and restores the exact workspace, including its manuscript; rejects tampered content and missing required bundles before restore; and can be consumed repeatedly to produce identical digital book output while preserving the same Workshop arbitration record ID.

## UML class card

See [CompositionPackageBinaryArtifactsTests UML](../../docs/UML/CompositionPackageBinaryArtifactsTestsUML.md).

## Acceptance evidence

The repeated-consumption test runs the same immutable artifact through the reader twice and passes both restored workspaces to BookProductRenderer. It compares rendered Markdown and referenced event IDs, and asserts that both loads retain the original arbitration record identity. The reader has no arbitration service dependency.
