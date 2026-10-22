# ObserverKnowledgeProjection UML

```mermaid
classDiagram
    class ObserverKnowledgeProjectionService {
        +Project(workspace, observerId, asOfEventId) ObserverKnowledgeProjection
    }
    class ObserverKnowledgeProjection {
        +Guid ObserverId
        +Guid AsOfEventId
        +IReadOnlyList~ObserverKnownEvent~ Events
    }
    class ObserverKnownEvent {
        +Guid EventId
        +Guid TimelineId
        +IReadOnlyList~ObserverPermittedClaim~ PermittedClaims
    }
    class ObserverPermittedClaim {
        +NarrationClaimMode Mode
        +Guid KnowledgeRecordId
        +ObserverKnowledgeKind Kind
        +string Attribution
        +string Note
        +Guid SourceId
        +double Confidence
    }
    ObserverKnowledgeProjectionService ..> ObserverKnowledgeEvaluator : checks
    ObserverKnowledgeProjectionService ..> ObserverKnowledgeProjection : creates
    ObserverKnowledgeProjection --> ObserverKnownEvent : includes permitted events
    ObserverKnownEvent --> ObserverPermittedClaim : carries accepted claims
```

## Use cases

- Build an observer-specific view of accepted knowledge at a causal event anchor.
- Give downstream narration/portrayal providers only the accepted notes and modes that may support claims.
- Keep world truth, observer knowledge, and representation detail as separate layers.
