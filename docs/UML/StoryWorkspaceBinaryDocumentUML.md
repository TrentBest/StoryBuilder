# StoryWorkspaceBinaryDocument UML

## Class diagram

```mermaid
classDiagram
    class StoryWorkspaceBinaryDocument {
        +StoryWorkspace Workspace
        +List~VocabularyConcept~ Synonyms
        +FromSession(session) StoryWorkspaceBinaryDocument
        +RestoreInto(session)
        +Pack(stream)
        +Unpack(stream)
        -WriteWorkspace(stream, workspace)
        -ReadWorkspace(stream) StoryWorkspace
    }
    class PublicationOutlet {
        +Guid Id
        +string Name
        +string ArtifactType
        +string Description
    }
    class InWorldPublicationEdition {
        +Guid Id
        +Guid OutletId
        +int EditionNumber
        +string Content
        +string ContentSha256
        +string InWorldPublishedAt
        +Guid? SupersedesEditionId
    }
    StoryWorkspaceBinaryDocument ..|> IBinarySerializable
    StoryWorkspaceBinaryDocument ..> StoryWorkspace
    StoryWorkspaceBinaryDocument ..> SceneRecord
    StoryWorkspaceBinaryDocument ..> SceneBeat
    StoryWorkspaceBinaryDocument ..> PublicationOutlet
    StoryWorkspaceBinaryDocument ..> InWorldPublicationEdition
    StoryWorkspaceBinaryDocument ..> VocabularyConcept
    StoryWorkspaceBinaryDocument ..> BinaryCodec
    StoryWorkspaceBinaryDocument ..> StoryBuilderSession
    PublicationOutlet "1" --> "0..*" InWorldPublicationEdition : issues
    InWorldPublicationEdition "0..1" --> "0..1" InWorldPublicationEdition : supersedes
```

## Current schema

The payload begins with the `SBW1` marker and current schema version `1`. This is the complete current development shape, not a historical compatibility promise. The reader rejects other versions. Unpublished development snapshots may be regenerated when the model changes; the writer, reader, tests, fixtures, and consumers evolve together.

The schema persists the current workspace model, including manuscript publication snapshots, visual asset provenance and review decisions, exact page-composition revision selections, organizations, in-world publication outlets and editions, and explicit vocabulary synonyms. Runtime FSM handles and UI state are not serialized.
