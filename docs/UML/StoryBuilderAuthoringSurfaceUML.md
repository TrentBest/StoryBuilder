# StoryBuilderAuthoringSurface UML

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Class diagram

```mermaid
classDiagram
    class StoryBuilderAuthoringSurface {
        -StoryBuilderAuthoringController _controller
        +StoryBuilderSession Session
        +GuiNode Root
        +Execute(commandId, value) StoryBuilderCommandResult
    }
    class StoryBuilderAuthoringController {
        +StoryBuilderSession Session
        +Execute(commandId, value) StoryBuilderCommandResult
    }
    class StoryBuilderSession {
        +StoryWorkspace Workspace
        +CreateTimeline(name)
        +CreateCharacter(name)
        +CreateScene(name)
        +CreateManuscript(name)
    }
    class GuiBuilder {
        +Child(builder)
        +Property(name, value)
        +Build() GuiNode
    }
    class GuiNode
    StoryBuilderAuthoringSurface --> StoryBuilderAuthoringController : allow-listed commands
    StoryBuilderAuthoringSurface --> StoryBuilderSession : projects live workspace
    StoryBuilderAuthoringSurface ..> GuiBuilder : creates snapshot
    GuiBuilder --> GuiNode : builds
```

## Use case: create and refresh

```mermaid
sequenceDiagram
    actor Author
    participant Host
    participant Surface as StoryBuilderAuthoringSurface
    participant Controller as StoryBuilderAuthoringController
    participant Session as StoryBuilderSession
    Author->>Host: Activate create-timeline button
    Host->>Surface: Execute(storybuilder.timeline.create)
    Surface->>Controller: Execute(commandId)
    Controller->>Session: CreateTimeline(default name)
    Session-->>Controller: Timeline record
    Controller-->>Surface: Success result
    Host->>Surface: Read Root again
    Surface->>Session: Read current workspace counts
    Surface-->>Host: Refreshed GuiNode snapshot
```

## Responsibility boundary

- StoryBuilder owns records and the bounded command vocabulary.
- The surface projects current state into GUI.Core's semantic tree; it does not render native controls.
- The host binds command IDs to this surface and re-renders after execution.
- Unknown commands must remain rejected; display text must never be treated as an operation identifier.
