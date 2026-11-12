# StoryProcessGroupDriver

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

StoryProcessGroupDriver is a minimal, host-neutral driver that advances the process groups inserted into it. It does not own a clock, loop, thread, or rendering surface.

## UML class card

See [StoryProcessGroupDriver UML](../../../docs/UML/StoryProcessGroupDriverUML.md) for the focused class diagram and use cases.

## Use case

Insert one or more IStoryProcessGroup instances. Each driver Update advances each group in the snapshot taken at the beginning of that update pass once. A group inserted during a pass is first advanced on the next pass. A group removed during a pass may still receive the current pass's update because it was already in the snapshot.

Remove a group by stable identity when it should no longer be driven.

## Lifetime and ownership

The driver is a **non-owning** collection. Insert does not transfer ownership, and Remove only detaches the group; it does not call Dispose. The host that creates or owns a disposable group remains responsible for disposing it after removal. In particular, a TimelineProcessGroup owns an FSM_API handle and must be disposed by its host when permanently retired. See [Process Group Lifetime](../../../docs/PROCESS_GROUP_LIFETIME.md).

The host must serialize driver mutation and Update calls. The driver is not a thread-safe scheduler and does not provide cancellation, quiescence, or runtime cleanup.

## Boundary

Production hosts can provide their own driver or scheduling policy. This small driver exists to support deterministic local stepping and tests; it is not the production Foundry execution pipeline.
