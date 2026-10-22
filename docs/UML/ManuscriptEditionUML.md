# ManuscriptEdition UML

```mermaid
classDiagram
    class ManuscriptEdition {
        +Guid Id
        +string Title
        +string Subtitle
        +string Author
        +CanonStatus Status
        +List~ManuscriptSection~ Sections
        +AddSection(section)
        +ReplaceSection(section)
    }
    class ManuscriptSection {
        +Guid Id
        +int Sequence
        +ManuscriptSectionKind Kind
        +string Title
        +string Body
        +List~Guid~ SceneIds
        +List~Guid~ EventIds
    }
    StoryWorkspace "1" o-- "*" ManuscriptEdition
    ManuscriptEdition "1" o-- "*" ManuscriptSection
    StoryBuilderSession --> StoryWorkspace : edits
    BookPreview --> StoryBuilderSession : reads
```

## Use cases

- Compose a working book from ordered sections.
- Revise section text and refresh the actual product preview.
- Preserve section links to scenes and events for future timeline navigation.
- Keep a working manuscript distinct from an immutable published edition.
