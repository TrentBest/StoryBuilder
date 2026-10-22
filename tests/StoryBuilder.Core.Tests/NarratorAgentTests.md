# NarratorAgentTests

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

Verifies that NarratorAgent coordinates five independent FSM_API machines for intent, planning, observer constraints, portrayal preparation, and draft review. Observer knowledge and portrayal readiness are separate responsibilities.

## UML class card

See [NarratorAgent UML](../../docs/UML/NarratorAgentUML.md).

## Acceptance evidence

Tests cover isolated agents, intent and timeline planning, draft review gates, accepted observer projections, and the portrayal FSM's NeedsReview state when no accepted knowledge is available at the chosen anchor.
