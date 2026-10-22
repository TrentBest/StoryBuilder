# TimelineDefinition

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

TimelineDefinition is the canonical, host-neutral identity and descriptive metadata for one timeline. A runtime Process Group is an executable projection of this definition, not its replacement. ParentTimelineId records branch lineage without overwriting the source timeline.

## UML class card

See [TimelineDefinition UML](../../../docs/UML/TimelineDefinitionUML.md) for the focused class diagram and use cases.

## Use case

Create a definition, add it to a StoryWorkspace, and pass it to a runtime adapter when a host is ready to execute it. Branching creates a new timeline identity that points to its parent; it does not mutate the parent's identity.

## Boundary

This record does not implement event execution, storage, a timeline console, or publication locking. Those responsibilities belong to the workspace, runtime adapter, and future authoring/publishing services respectively.
