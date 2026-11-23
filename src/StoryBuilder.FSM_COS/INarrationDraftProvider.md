# INarrationDraftProvider

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

Defines the optional, host-supplied capability that turns a bounded Narrator request into an untrusted draft proposal. StoryBuilder does not select or depend on a concrete LLM, vendor SDK, or remote provider.

## Contract

- `ProviderId` is the stable provider attribution recorded with the proposal.
- `DraftAsync` receives only a `NarrationDraftRequest` and a cooperative cancellation token.
- The provider returns a `NarrationDraftProposal` tied to the request ID and carrying source-beat ranges and explicit event claims.
- Provider implementations must not write manuscripts, accept canon, or mutate the StoryBuilder workspace.

## Composition boundary

Hosts may register a provider implementation when one is available. No concrete provider is included or registered by the current WebDemo; provider invocation remains optional.
