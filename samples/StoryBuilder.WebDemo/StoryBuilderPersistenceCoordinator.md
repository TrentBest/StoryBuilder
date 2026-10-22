# StoryBuilderPersistenceCoordinator

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

The scoped coordinator restores and autosaves a StoryBuilder workspace to this browser's localStorage. It stores a Base64 envelope around StoryBuilder's versioned binary document from FSM_Serialization; the payload is not JSON and is not sent to a server.

## Lifecycle

The layout loads storage after the interactive browser connection is available. Session changes trigger serialized saves. An unreadable saved value is preserved and reported rather than silently overwritten; the user must explicitly choose to discard it and start fresh.

## Boundary

This provides browser-local persistence for the demo, not cloud synchronization, account sharing, backup, or multi-user collaboration. Local browser storage can be cleared by the user or browser.
