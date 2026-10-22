# NarratorAgent

**User moniker:** The Narrator's cooperating FSM agent

**Purpose:** Coordinates separate FSM_API state machines for author intent, provisional planning, observer knowledge constraints, and draft/review. It does not replace the canonical domain records or pretend to generate prose without a drafting provider.

## UML class card

- Intent FSM: Idle → Captured → Ready
- Planning FSM: Empty → Anchored → Specified
- Observer FSM: Unselected → Selected → Allowed / Blocked
- Draft/review FSM: Idle → Requested → Ready / NeedsReview
- Inputs/outputs: StoryBuilderSession records, ObserverKnowledgeEvaluation, manuscript sections, event trace
- Runtime: four isolated FSM_API processing groups per agent instance

## Use cases

- Capture the creator's idea as a premise proposal.
- Add an unresolved Plot Twist to a timeline.
- Select an observer and evaluate whether an event claim is supported.
- Hold a draft for review when declared claims exceed observer knowledge.
- Save accepted draft text and link its declared world-event IDs to the manuscript section.
- Inspect each FSM's state transitions without a monolithic agent state hiding the workflow.
