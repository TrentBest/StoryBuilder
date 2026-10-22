# PublishedBookEdition UML

```mermaid
classDiagram
    class StoryBuilderSession {
      +PublishManuscript(Guid, string) PublishedBookEdition
    }
    class PublishedBookEdition {
      +Guid Id
      +Guid ManuscriptId
      +string Title
      +string Markdown
      +string ContentSha256
      +DateTimeOffset PublishedAtUtc
      +DateTimeOffset SourceUpdatedAtUtc
      +Guid? PreviousEditionId
      +string? CorrectionNotes
      +List~Guid~ SectionIds
      +List~Guid~ SceneIds
      +List~Guid~ EventIds
    }
    class ManuscriptEdition
    class StoryWorkspace {
      +PublishedEditions
      +AddPublishedEdition(PublishedBookEdition)
    }
    StoryBuilderSession --> ManuscriptEdition : snapshots
    StoryBuilderSession --> PublishedBookEdition : publishes
    StoryWorkspace --> PublishedBookEdition : stores
    PublishedBookEdition --> ManuscriptEdition : source identity
    PublishedBookEdition --> PublishedBookEdition : previous edition
```

## Use case

An author publishes a working book, makes a correction later, and publishes again. The original Markdown remains unchanged; the correction is a new hashed artifact linked to its predecessor.
