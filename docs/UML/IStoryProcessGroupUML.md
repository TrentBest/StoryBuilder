# IStoryProcessGroup UML

```mermaid
classDiagram
    direction TB
    class StoryProcessGroupDriver {
        +Groups IReadOnlyCollection
        +Insert(IStoryProcessGroup)
        +Remove(Guid) bool
        +Update()
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
        +Events IReadOnlyList
        +Update()
        +Dispose()
    }
    StoryProcessGroupDriver --> IStoryProcessGroup : updates inserted groups
    TimelineProcessGroup ..|> IStoryProcessGroup : implements
```

## Use cases

- Insert a compatible process group into a host-neutral driver.
- Let the driver advance each inserted group once per driver Update.
- Implement another adapter without adding its execution engine to StoryBuilder.Core.

## Boundary

The interface is a scheduling seam, not a scheduler or a promise that every runtime provider is installed.
