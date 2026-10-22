# StoryLifecycleTests

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

These tests verify the optional FSM_API adapter: acceptance requires a proposal, accepted records can be proposed again for revision, and deprecated records do not silently return to active workflow states.

See [StoryLifecycleSession UML](../../docs/UML/StoryLifecycleSessionUML.md) for the state and use-case diagram.

## Verification

Run the full suite using the repository's Development guide. The FSM adapter is part of the automated test graph and must pass CI before being treated as verified.
