# StoryBuilderAuthoringController UML

```mermaid
classDiagram
    class StoryBuilderSession {
        +StoryWorkspace Workspace
        +CreateTimeline(name, description, parentTimelineId)
        +CreateCharacter(name, templateName)
        +CreateScene(title, location, mode)
        +CreateManuscript(title, subtitle, author)
    }
    class StoryBuilderAuthoringController {
        +StoryBuilderSession Session
        +Execute(commandId, value) StoryBuilderCommandResult
    }
    class StoryBuilderCommandResult {
        +bool Succeeded
        +string CommandId
        +string Message
        +Guid? RecordId
    }
    class StoryBuilderAuthoringCommands {
        <<static>>
        +CreateTimeline
        +CreateCharacter
        +CreateScene
        +CreateManuscript
    }
    StoryBuilderAuthoringController --> StoryBuilderSession : delegates to
    StoryBuilderAuthoringController ..> StoryBuilderCommandResult : returns
    StoryBuilderAuthoringController ..> StoryBuilderAuthoringCommands : accepts stable IDs
```

## Use cases

- Create a timeline, character, scene, or manuscript using an existing domain operation.
- Reject unsupported commands without mutating the session.
- Return a stable record ID and human-readable outcome to a host.
- Let AnyApp bind native controls explicitly without adding WPF dependencies to StoryBuilder.
