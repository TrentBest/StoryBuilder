# TimelineProcessGroup UML

```mermaid
classDiagram
    direction TB
    class TimelineDefinition {
        +Guid Id
        +string Name
        +string Description
        +Guid? ParentTimelineId
    }
    class IStoryProcessGroup {
        <<interface>>
        +Guid Id
        +string Name
        +long UpdateCount
        +Update()
    }
    class TimelineProcessGroup {
        +TimelineDefinition Definition
        +long RuntimeTickCount
        +Events IReadOnlyList
        +Update()
        +Dispose()
    }
    class TimelineEventRecord {
        +Guid Id
        +Guid TimelineId
        +long Sequence
        +string Kind
        +string Description
    }
    TimelineProcessGroup ..|> IStoryProcessGroup
    TimelineProcessGroup --> TimelineDefinition : executes projection of
    TimelineProcessGroup --> TimelineEventRecord : emits
```

## Use cases

- Construct one executable Process Group from a canonical timeline definition.
- Insert it into a driver and inspect its runtime event observations.
- Dispose the runtime adapter without deleting or mutating the canonical timeline definition.

## Boundary

The adapter is optional and FSM_API-backed. StoryBuilder.Core stays independent of the runtime package.
