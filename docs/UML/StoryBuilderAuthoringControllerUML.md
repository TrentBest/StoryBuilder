# StoryBuilderAuthoringController UML

```mermaid
classDiagram
    class StoryBuilderSession {
        +StoryWorkspace Workspace
        +CreateTimeline(name, description, parentTimelineId)
        +CreateCharacter(name, templateName)
        +CreateScene(title, location, mode)
        +CreateManuscript(title, subtitle, author)
        +CreateTimelineEvent(timelineId, title, summary, approximatePlacement)
        +AddManuscriptSection(manuscriptId, kind, title)
        +LinkEventToManuscriptSection(manuscriptId, sectionId, eventId, linked)
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
        +CreateEvent
        +CreateManuscriptSection
        +LinkLatestEventToLatestSection
    }
    StoryBuilderAuthoringController --> StoryBuilderSession : delegates to
    StoryBuilderAuthoringController ..> StoryBuilderCommandResult : returns
    StoryBuilderAuthoringController ..> StoryBuilderAuthoringCommands : accepts stable IDs
```

## Use cases

- Create a timeline, character, scene, or manuscript using an existing domain operation.
- Create a canonical event on the first timeline, creating a default timeline if necessary.
- Create a manuscript passage, creating a default manuscript if necessary.
- Link the latest event to the latest passage by stable identity; reject the link without mutation when prerequisites are absent.
- Reject unsupported commands without mutating the session.
- Return a stable record ID and human-readable outcome to a host.
- Let AnyApp bind native controls explicitly without adding WPF dependencies to StoryBuilder.
