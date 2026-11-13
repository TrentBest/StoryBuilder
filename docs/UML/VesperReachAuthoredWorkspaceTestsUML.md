# VesperReachAuthoredWorkspaceTests UML

## Class diagram

```mermaid
classDiagram
    class VesperReachAuthoredWorkspaceTests {
        +FirstContactWorkspacePreservesDistinctObserverKnowledgeAndAuthoredLinksAcrossPersistence()
    }
    VesperReachAuthoredWorkspaceTests ..> StoryBuilderSession : authors
    VesperReachAuthoredWorkspaceTests ..> ObserverKnowledgeProjectionService : projects
    VesperReachAuthoredWorkspaceTests ..> StoryWorkspaceValidator : validates
    VesperReachAuthoredWorkspaceTests ..> StoryWorkspaceBinaryDocument : round-trips
    StoryBuilderSession ..> StoryWorkspace : owns authored records
    ObserverKnowledgeProjectionService ..> StoryWorkspace : reads
    StoryWorkspaceBinaryDocument ..> StoryWorkspace : serializes
```

## Scenario flow

```mermaid
flowchart TD
    A[Create Vesper Reach workspace] --> B[Author source-backed signal event]
    B --> C[Author bulletin and independent account]
    C --> D[Record distinct observer knowledge]
    D --> E[Project Ilyan, Mara, and Teren]
    E --> F{Knowledge framing correct?}
    F -->|Yes| G[Validate authored workspace]
    G --> H[Pack versioned binary document]
    H --> I[Restore workspace]
    I --> J[Verify identities and manuscript links]
    J --> K[Validate restored workspace]
```

## Use cases

- A direct witness can receive the primary signal as a direct fact.
- A courier who learned through the official bulletin receives only an attributed report, not direct-fact framing.
- An uninformed observer receives no signal event in their projection.
- An independent courier account remains distinct from the station bulletin.
- The scene, place assignment, event provenance, observer knowledge, and manuscript links survive the binary round trip with stable identifiers.
