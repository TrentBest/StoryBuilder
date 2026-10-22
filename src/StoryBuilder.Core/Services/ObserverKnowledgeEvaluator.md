# ObserverKnowledgeEvaluator

**User moniker:** Observer lens / narration guard

**Purpose:** Evaluates whether a requested narration claim is supported by an observer's accepted knowledge records at a given event anchor. If temporal availability cannot be proven from the explicit causal graph, it fails closed rather than guessing.

## UML class card

- Input: StoryWorkspace, ObserverId, EventId, AsOfEventId, NarrationClaimMode
- Output: ObserverKnowledgeEvaluation
- Decisions: allowed, denied, or unresolved; required framing; explanation; evidence record ID
- Boundary: no dependency on Renderer or any concrete output provider

## Use cases

- Allow a witnessed event to be stated as a direct fact.
- Require attribution when the observer learned the event from another source.
- Require inference or belief framing when the observer did not witness the event.
- Deny unsupported knowledge and leave proposed knowledge unresolved until accepted.
