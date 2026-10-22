# BookProductRendererTests UML

```mermaid
classDiagram
    class BookProductRendererTests {
        +RendersOrderedManuscriptAsDeterministicDigitalBook()
        +RejectsUnknownManuscript()
    }
    BookProductRendererTests ..> BookProductRenderer : verifies output
    BookProductRenderer ..> RenderedBookArtifact : creates
```

## Use cases

- Render a manuscript into stable Markdown with normalized line endings.
- Preserve event references as artifact metadata.
- Reject an unknown manuscript identity.
