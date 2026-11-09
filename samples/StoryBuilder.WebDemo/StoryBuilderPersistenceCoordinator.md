# StoryBuilderPersistenceCoordinator

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

The scoped coordinator restores and autosaves a StoryBuilder workspace through `LocalStoryWorkspaceRepository`, which writes StoryBuilder's versioned FSM_Serialization binary document as an atomic host-local `.sbws` file. The default WebDemo location is `App_Data/workspaces`; `StoryBuilder:Workspaces:StoragePath` can override it.

## Lifecycle

The layout loads the host workspace when the interactive session starts. Session changes trigger serialized saves. If no host workspace exists, the coordinator checks for a legacy browser-local snapshot, imports it, writes the host copy, and clears the browser copy only after that write succeeds. An unreadable host or legacy snapshot is preserved and reported rather than silently overwritten; the user must explicitly choose to discard it and start fresh.

## Boundary

This is host-local persistence, not cloud synchronization, account isolation, backup, or multi-user collaboration. The demo currently uses one workspace slot, so it is intended for a single-author host. Configure a durable host volume and back up both workspace and visual-asset directories.
