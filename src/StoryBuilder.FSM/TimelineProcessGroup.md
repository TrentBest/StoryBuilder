# TimelineProcessGroup

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

TimelineProcessGroup is the optional FSM_API adapter that turns one canonical TimelineDefinition into an executable Process Group. Every instance uses a stable timeline ID and an isolated processing-group name. Its Events snapshot exposes update requests and runtime callbacks for inspection.

## UML class card

See [TimelineProcessGroup UML](../../docs/UML/TimelineProcessGroupUML.md) for the focused class diagram and use cases.

## Use case

Create one adapter per TimelineDefinition, insert it into a host's StoryProcessGroupDriver (or another host implementing the same contract), and let the host call Update. Each driver update reaches FSM_API for that timeline's processing group.

The adapter owns the FSM_API handle it creates. The host must call Dispose when the adapter is permanently retired. Removing it from StoryProcessGroupDriver only stops future driver passes; removal does not dispose the adapter. Dispose is idempotent and destroys the owned FSM_API instance.

## Runtime evidence

Events are diagnostic runtime observations (entry, driver-update request, runtime tick, and exit). They are not canonical world events, authored story facts, Foundry run evidence, or durable execution history.

## Boundary

This is a local stepping proof, not the production temporal simulation or FSM_Foundry integration. It records process-group entry, driver update requests, runtime ticks, and exit. Canonical event causality, observer knowledge, timeline branching behavior, narration links, durable binary persistence, and publication editions remain separate concerns. StoryBuilder.Core does not reference FSM_API or FSM_COS.
