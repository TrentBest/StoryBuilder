# BookProductRenderer UML

```mermaid
classDiagram
    class BookProductRenderer {
        +Render(workspace, manuscriptId) RenderedBookArtifact
    }
    class StoryWorkspace {
        +Manuscripts
    }
    class ManuscriptEdition {
        +Id
        +Title
        +Subtitle
        +Author
        +Sections
    }
    class ManuscriptSection {
        +Sequence
        +Kind
        +Title
        +Body
        +EventIds
    }
    class RenderedBookArtifact {
        +Markdown
        +SectionCount
        +ReferencedEventIds
    }
    StoryWorkspace --> ManuscriptEdition : stores
    ManuscriptEdition --> ManuscriptSection : contains
    BookProductRenderer ..> StoryWorkspace : reads
    BookProductRenderer ..> RenderedBookArtifact : creates
```

## Use cases

- Render the current manuscript as deterministic Markdown.
- Use the exact same output renderer after package validation and workspace restoration.
- Keep physical printing and immutable publication editions outside this initial output service.
