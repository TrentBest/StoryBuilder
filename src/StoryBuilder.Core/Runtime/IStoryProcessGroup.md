# IStoryProcessGroup

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

IStoryProcessGroup is the small runtime seam between StoryBuilder's domain and an execution engine. It gives a host a stable identity, a display name, an update count, and one operation to advance the group.

## UML class card

See [IStoryProcessGroup UML](../../../docs/UML/IStoryProcessGroupUML.md) for the focused class diagram and use cases.

## Use case

A host driver inserts compatible groups and calls Update on each group when its own schedule says to advance. StoryBuilder.Core does not need to know which engine implements the group.

## Boundary

The interface deliberately does not expose FSM_API, FSM_COS, rendering, scheduling policy, or publication authority.
