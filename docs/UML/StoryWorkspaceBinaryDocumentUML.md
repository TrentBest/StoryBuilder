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
        -ReadWorkspace(stream, version) StoryWorkspace
    }
    StoryWorkspaceBinaryDocument ..|> IBinarySerializable
    StoryWorkspaceBinaryDocument ..> StoryWorkspace
    StoryWorkspaceBinaryDocument ..> SceneRecord
    StoryWorkspaceBinaryDocument ..> SceneBeat
    StoryWorkspaceBinaryDocument ..> VocabularyConcept
    StoryWorkspaceBinaryDocument ..> BinaryCodec
    StoryWorkspaceBinaryDocument ..> StoryBuilderSession
```

## Use cases

The host captures a workspace and explicit synonym mappings as a versioned binary document. Version 8 persists event IDs on scene beats; versions 1 through 7 remain readable and restore those links as empty. Runtime FSM handles and UI state are not serialized.
