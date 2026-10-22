# NarratorAgent UML

```mermaid
classDiagram
    class NarratorAgent {
        +IntentState
        +PlanningState
        +ObserverState
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
    NarratorAgent ..> ObserverKnowledgeProjectionService : builds observer context
    NarratorAgent *-- NarratorFsmContext : owns four isolated contexts
```

## Cooperating state machines

- **Intent:** Idle → Captured → Ready
- **Planning:** Empty → Anchored → Specified
- **Observer:** Unselected → Selected → Allowed / Blocked / Projected
- **Draft/review:** Idle → Requested → Ready / NeedsReview

## Use cases

- Capture a premise and add an unresolved timeline point.
- Evaluate a claim under an explicit observer and event anchor.
- Project accepted observer knowledge without exposing unrestricted world-event summaries.
- Hold a draft for review if a declared claim is unsupported.
- Save approved draft text and link its declared event identities to the manuscript section.
