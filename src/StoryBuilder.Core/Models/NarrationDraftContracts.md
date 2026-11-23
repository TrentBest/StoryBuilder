# NarrationDraftContracts

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

Defines the provider-neutral request, source-beat snapshot, proposal, provenance segment, and explicit event-claim contracts used by Narrator drafting.

## UML class card

See [Narration Draft Request Flow UML](../../../docs/UML/NarrationDraftRequestFlowUML.md).

## Contract boundaries

- A request contains only the selected scene/beat material, manuscript context, author-selected presentation hints, and optional observer-safe knowledge.
- A proposal is untrusted, non-canonical provider output. It carries the exact request ID, text, provider/model attribution, source-linked text ranges, and explicit event claims.
- Every non-whitespace character in proposed text must be covered by at least one source-beat provenance segment.
- Every segment must cite request-supplied beats and may cite only events linked to those beats.
- Event claims must refer to supplied events. When observer-constrained, each claim mode must be permitted by the request's observer projection.
- Omniscient perspective requires explicit author permission.

## Persistence boundary

Validation does not save or accept the proposal. Host review and explicit author acceptance remain required; accepted text and event links use the existing atomic manuscript replacement path.
