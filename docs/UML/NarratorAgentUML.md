# NarratorAgent UML

```mermaid
classDiagram
    class NarratorAgent {
        +IntentState
        +PlanningState
        +ObserverState
        +PortrayalState
        +DraftState
        +CaptureIntent(intent) StoryPremise
        +AddPlanningPoint(timelineId, label, placement) TimelinePlanningPoint
        +SelectObserver(observerId)
        +EvaluateObserverClaim(eventId, asOfEventId, mode) ObserverKnowledgeEvaluation
        +ProjectObserverKnowledge(asOfEventId) ObserverKnowledgeProjection
        +RequestDraft(manuscriptId, sectionId, observerId, allowOmniscient)
        +SubmitDraft(text, claimedEventIds, asOfEventId, mode) NarratorDraftSubmission
    }
    class StoryBuilderSession
    class ObserverKnowledgeEvaluator
    class ObserverKnowledgeProjectionService
    class NarratorFsmContext
    NarratorAgent --> StoryBuilderSession : edits canonical domain
    NarratorAgent ..> ObserverKnowledgeEvaluator : guards claims
    NarratorAgent ..> ObserverKnowledgeProjectionService : prepares portrayal context
    NarratorAgent *-- NarratorFsmContext : owns five isolated contexts
```

## Cooperating state machines

- **Intent:** Idle → Captured → Ready
- **Planning:** Empty → Anchored → Specified
- **Observer:** Unselected → Selected → Allowed / Blocked
- **Portrayal:** Idle → Requested → Ready / NeedsReview
- **Draft/review:** Idle → Requested → Ready / NeedsReview

## Use cases

- Capture a premise and add an unresolved timeline point.
- Evaluate a claim under an explicit observer and event anchor.
- Prepare a constrained portrayal context without changing observer-evaluation state.
- Request review when the observer has no accepted knowledge at the chosen anchor.
- Hold a draft for review if a declared claim is unsupported.
- Save approved draft text and link its declared event identities to the manuscript section.
