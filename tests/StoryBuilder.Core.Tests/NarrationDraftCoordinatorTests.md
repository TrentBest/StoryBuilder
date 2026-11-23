# NarrationDraftCoordinatorTests

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

Verifies the optional host provider boundary: bounded request delivery, provider attribution, request/proposal identity, cancellation, and non-persistence.

## UML class card

See [Narration Draft Request Flow UML](../../docs/UML/NarrationDraftRequestFlowUML.md).

## Acceptance evidence

- A valid provider proposal is attributed and returned with its exact request.
- Conflicting provider identity and mismatched request identity are rejected.
- Cancellation before invocation prevents provider execution.
- Proposal generation does not change the workspace or manuscript.
