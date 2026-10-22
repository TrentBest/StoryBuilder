# RenderedBookArtifact UML

```mermaid
classDiagram
    class RenderedBookArtifact {
        +Guid ManuscriptId
        +string Title
        +string Subtitle
        +string Author
        +string Markdown
        +int SectionCount
        +IReadOnlyList~Guid~ ReferencedEventIds
    }
    class BookProductRenderer {
        +Render(workspace, manuscriptId) RenderedBookArtifact
    }
    BookProductRenderer ..> RenderedBookArtifact : creates
    BookProductRenderer ..> ManuscriptEdition : reads
    BookProductRenderer ..> ManuscriptSection : orders and renders
```

## Use cases

- Produce a deterministic digital book from the current manuscript.
- Preserve stable event references in product metadata for timeline and observer views.
- Consume the same manuscript after binary package restoration without repeating arbitration.
