# NarrationDraftRequestBuilder

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

Builds a provider-facing narration request from explicit selections in the current `StoryBuilderSession`. The builder snapshots selected scene beats and manuscript context without passing the complete workspace to a drafting provider.

## UML class card

See [Narration Draft Request Flow UML](../../../docs/UML/NarrationDraftRequestFlowUML.md).

## Inputs and output

- Input: session, scene ID, selected beat IDs, manuscript ID, section ID, perspective, optional voice/style, optional observer ID and event horizon, and explicit omniscient permission.
- Output: immutable-value `NarrationDraftRequest` with selected beat snapshots, manuscript/section context, and an observer-safe projection when constrained.
- Beat snapshots are ordered by the scene's sequence, not caller enumeration order.
- Missing records, duplicate selections, invalid event links, and malformed identities fail before a request is returned.

## Observer boundary

When an observer is selected, every selected beat must link to at least one event in that observer's accepted knowledge projection, and every linked event must be present in that projection. Unlinked or partially unsupported beats are rejected instead of being sent to the provider.

The projection and request share the same event horizon. The complete workspace, unrestricted world-event summaries, and unrelated scenes are not copied into the request.

## Ownership and persistence

This service only builds a request. It does not invoke an LLM, persist provider output, or accept canon. The host may pass the request to an optional provider and present its proposal for author review. Accepted text and event links continue through `StoryBuilderSession.SaveManuscriptDraft`.
