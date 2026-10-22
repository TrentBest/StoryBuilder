# NarratorAgent UML

## Cooperating state machines

- Intent FSM: Idle, Captured, Ready
- Planning FSM: Empty, Anchored, Specified
- Observer FSM: Unselected, Selected, Allowed, Blocked
- Draft/review FSM: Idle, Requested, Ready, NeedsReview

## Collaborators

- StoryBuilderSession owns canonical premise, timeline, planning point, event, observer-knowledge, and manuscript records.
- ObserverKnowledgeEvaluator checks claims against accepted observer knowledge.
- NarratorAgent coordinates FSM_API instances and records transitions.
- Renderer remains an optional portrayal provider; this class does not embed an LLM or a renderer.

## Use cases

- Capture a premise and add an unresolved timeline point.
- Evaluate a claim under an explicit observer and event anchor.
- Hold a draft for review if a declared claim is unsupported.
- Save approved draft text and link its declared event identities to the manuscript section.
