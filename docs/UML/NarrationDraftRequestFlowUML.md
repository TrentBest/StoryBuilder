# Narration Draft Request Flow UML

```mermaid
classDiagram
    class StoryBuilderSession {
        +Workspace StoryWorkspace
        +SaveManuscriptDraft(manuscriptId, sectionId, title, body, assertedEventIds) ManuscriptEdition
    }
    class NarrationDraftRequestBuilder {
        +Build(session, sceneId, selectedBeatIds, manuscriptId, sectionId, perspective, voice, style, observerId, asOfEventId, allowOmniscientPerspective) NarrationDraftRequest
    }
    class NarrationDraftRequest {
        +Guid RequestId
        +Guid SceneId
        +Guid ManuscriptId
        +Guid SectionId
        +NarrationPerspective Perspective
        +ObserverKnowledgeProjection ObserverKnowledge
        +IReadOnlyList~NarrationSourceBeat~ SourceBeats
    }
    class NarrationDraftProposal {
        +Guid RequestId
        +string Text
        +string ProviderId
        +string ModelId
        +IReadOnlyList~NarrationDraftSegment~ Segments
        +IReadOnlyList~NarrationEventClaim~ Claims
    }
    class NarrationDraftProposalValidator {
        +Validate(request, proposal) void
    }
    class INarrationDraftProvider {
        +string ProviderId
        +DraftAsync(request, cancellationToken) Task~NarrationDraftProposal~
    }
    class NarrationDraftCoordinator {
        +ProposeAsync(sceneId, selectedBeatIds, manuscriptId, sectionId, perspective, voice, style, observerId, asOfEventId, allowOmniscientPerspective, cancellationToken) Task~NarrationDraftReview~
    }
    class NarrationDraftReview {
        +NarrationDraftRequest Request
        +NarrationDraftProposal Proposal
    }
    class ObserverKnowledgeProjectionService {
        +Project(workspace, observerId, asOfEventId) ObserverKnowledgeProjection
    }
    class ObserverKnowledgeProjection {
        +Guid ObserverId
        +Guid AsOfEventId
        +IReadOnlyList~ObserverKnownEvent~ Events
    }

    StoryBuilderSession --> NarrationDraftRequestBuilder : supplies workspace
    NarrationDraftRequestBuilder --> NarrationDraftRequest : snapshots selected records
    NarrationDraftRequestBuilder --> ObserverKnowledgeProjectionService : optional observer boundary
    ObserverKnowledgeProjectionService --> ObserverKnowledgeProjection : creates
    NarrationDraftRequest --> NarrationDraftProposalValidator : constrains
    NarrationDraftCoordinator --> NarrationDraftRequestBuilder : builds bounded request
    NarrationDraftCoordinator --> INarrationDraftProvider : invokes optional capability
    NarrationDraftCoordinator --> NarrationDraftProposalValidator : validates untrusted output
    NarrationDraftCoordinator --> NarrationDraftReview : returns request and proposal
    NarrationDraftProposal --> NarrationDraftProposalValidator : validated as untrusted output
    NarrationDraftProposalValidator ..> StoryBuilderSession : no direct write
    StoryBuilderSession --> NarrationDraftRequestBuilder : accepted text later saved through existing gate
```

## Use cases

- Build a drafting request from an explicitly selected scene, beat set, manuscript, and section.
- Constrain a request to an observer's accepted knowledge at a causal event horizon.
- Reject selected beats that are not grounded in the observer projection.
- Validate provider text ranges and event claims before showing the proposal for author review.
- Persist accepted text and event provenance only through the existing manuscript save operation.

## Non-goals

- The host may supply an optional provider through `INarrationDraftProvider`; no concrete provider is bundled or registered by the current WebDemo.
- This flow does not choose an LLM, generate creative prose by itself, or silently mutate canon.
- It does not expose the complete workspace to a provider.
- It does not replace the host's author-review UI or the atomic manuscript save gate.
