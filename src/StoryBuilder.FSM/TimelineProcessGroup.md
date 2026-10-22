# TimelineProcessGroup

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

TimelineProcessGroup is the optional FSM_API adapter that turns one canonical TimelineDefinition into an executable Process Group. Every instance uses a stable timeline ID and an isolated processing-group name. Its Events snapshot exposes update requests and runtime callbacks for inspection.

## UML class card

See [TimelineProcessGroup UML](../../docs/UML/TimelineProcessGroupUML.md) for the focused class diagram and use cases.

## Use case

Create one adapter per TimelineDefinition, insert it into a host's StoryProcessGroupDriver (or another host implementing the same contract), and let the host call Update. Each driver update reaches FSM_API for that timeline's processing group. Dispose the adapter when the host removes it permanently.

## Boundary

This is the first execution proof, not yet the complete temporal simulation. It records process-group entry, driver update requests, runtime ticks, and exit. Canonical event causality, observer knowledge, timeline branching behavior, narration links, durable binary persistence, and publication editions remain separate follow-on work. StoryBuilder.Core does not reference FSM_API or FSM_COS.
