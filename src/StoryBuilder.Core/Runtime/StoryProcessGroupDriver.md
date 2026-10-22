# StoryProcessGroupDriver

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

StoryProcessGroupDriver is a minimal, host-neutral proof of composition: inserted process groups are advanced by the driver's Update call. It does not own a clock, loop, thread, or rendering surface.

## UML class card

See [StoryProcessGroupDriver UML](../../../docs/UML/StoryProcessGroupDriverUML.md) for the focused class diagram and use cases.

## Use case

Insert one or more IStoryProcessGroup instances. Each driver Update calls Update on every inserted group once. Remove a group by stable identity when it should no longer be driven.

## Boundary

Production hosts can provide their own driver or scheduling policy. This small driver exists to prove the composition contract and support deterministic tests.
