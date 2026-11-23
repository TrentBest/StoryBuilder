# NarrationDraftCoordinator

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

Coordinates the provider-neutral Narrator pipeline at the host/composition boundary: build a bounded request, invoke the configured optional provider, attribute its output, and validate the proposal before returning it for author review.

## UML class card

See [Narration Draft Request Flow UML](../../docs/UML/NarrationDraftRequestFlowUML.md).

## Behavior

- Builds requests through `NarrationDraftRequestBuilder`; the full workspace is never handed to the provider.
- Passes cancellation through to the provider and checks cancellation before request construction.
- Requires a stable provider ID and stamps it when the provider omits attribution; conflicting provider attribution is rejected.
- Validates request identity, complete text-range provenance, source beat/event links, and observer-permitted event claims.
- Returns a `NarrationDraftReview` containing the exact request and validated proposal.

## Non-goals

The coordinator does not create a provider, display a diff, persist a draft, or accept canon. The returned proposal remains non-canonical until an author-facing host presents it and the author explicitly accepts it through the existing manuscript save boundary.
