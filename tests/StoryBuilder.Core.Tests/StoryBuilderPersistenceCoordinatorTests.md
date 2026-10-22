# StoryBuilderPersistenceCoordinatorTests

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

A fake JavaScript localStorage bridge verifies that the coordinator stores a versioned binary document and restores it into a fresh StoryBuilderSession. This checks the coordinator's load/save flow without requiring a real browser in CI.

## Boundary

The test does not claim cross-device sync or browser automation. It verifies the same binary codec used by the local WebDemo persistence path.
