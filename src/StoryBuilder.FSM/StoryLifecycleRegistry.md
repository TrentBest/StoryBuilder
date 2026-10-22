# StoryLifecycleRegistry

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

StoryLifecycleRegistry keeps one FSM_API-backed lifecycle session per stable record ID for the lifetime of a host scope. When a record is first encountered, it reconstructs the supported path from Draft to the record's current status.

## UML class card

See [StoryLifecycleRegistry UML](../../docs/UML/StoryLifecycleRegistryUML.md) for the class diagram and use case.

## Use case

A UI asks the registry for a character or participant-action lifecycle using the record ID and current status. Subsequent edits reuse that lifecycle; the host disposes the registry when its workspace/circuit ends.

## Boundary

The registry is optional and belongs to the FSM adapter, not StoryBuilder.Core. Persisted workflow history and multi-process recovery remain future work; the current registry reconstructs state from the current status only.
