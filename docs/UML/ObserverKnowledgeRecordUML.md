# ObserverKnowledgeRecord and Evaluator UML

## Class card

- ObserverKnowledgeRecord: ObserverId, EventId, Kind, AcquiredAtEventId, SourceId, Attribution, Note, Confidence, Status
- ObserverKnowledgeEvaluator: evaluates a requested narration claim against the workspace's accepted observer knowledge
- ObserverKnowledgeEvaluation: IsAllowed, IsUnresolved, RequiredMode, Explanation, KnowledgeRecordId
- StoryEvent: objective event record; observer knowledge does not change the event itself
- StoryBuilder.Core has no Renderer dependency; a renderer adapter can consume the evaluation result

## Use cases

- Permit direct factual narration when the observer has an accepted witnessed record.
- Require attributed-report framing for information received from another source.
- Require inference or belief framing for non-witness knowledge.
- Keep proposed knowledge unresolved and deny claims whose causal availability cannot be established.
