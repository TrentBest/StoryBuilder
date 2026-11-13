# In-World Publication Editions

## Purpose

An `InWorldPublicationEdition` is an immutable authored snapshot of an artifact issued inside a story world. It represents what a publication outlet issued, and when the fictional setting says it was issued.

This is distinct from:

- `SourceRecord`: provenance identity for a source or artifact.
- `HistoricalClaim`: a statement with attribution, confidence, and supporting sources.
- `StoryEvent`: what happened in the world.
- `PublishedBookEdition`: an immutable snapshot of a StoryBuilder manuscript.
- Runtime media: channels, schedules, geographic reach, distribution, delivery, receipt, and presentation.

A publication edition freezes the issued artifact; it does not imply that any character or player encountered it.

## Domain records

### PublicationOutlet

`PublicationOutlet` provides stable identity for an issuing organization, archive, journal, institution, individual publication source, or other outlet. Its `ArtifactType` is descriptive metadata, not a runtime channel type.

### InWorldPublicationEdition

Each edition stores:

- stable edition identity and its outlet identity;
- an outlet-local positive edition number;
- title, exact content snapshot, content format, and SHA-256 content hash;
- `InWorldPublishedAt`, an authored fictional timestamp or period;
- `CreatedAtUtc`, host-side record metadata, never a substitute for fictional publication time;
- intended audience and access scope as authored descriptions;
- explicit references to historical claims, source records, and story events;
- optional `SupersedesEditionId` and correction notes.

The content and metadata are init-only. When added to a workspace, provenance collections are copied into read-only snapshots. The workspace exposes append-only addition; there is no edit/replace operation for an issued edition.

## Example: Vesper Reach

The signal-detection event is authored once. Three editions can refer to that same event:

1. The station outlet issues a preliminary bulletin with a provisional direction estimate.
2. An independent courier outlet issues a separate account challenging that inference.
3. The station outlet issues a correction with a higher edition number and a `SupersedesEditionId` pointing to the original bulletin.

The original bulletin remains unchanged, including its content hash and in-world publication time. The correction does not rewrite the event or the independent account. Each edition keeps its own claims and source references.

Issuing these records does not create `ObserverKnowledgeRecord` entries. Character knowledge remains explicit and is evaluated through the observer knowledge services. A character with no acquisition record does not know an artifact merely because it exists in the workspace.

## Generic example: archival catalog

The same contract can represent a museum or research archive publishing a catalog entry for a restored field journal. No news, broadcast, science-fiction, or campaign concepts are required.

## Validation and persistence

`StoryWorkspaceValidator` checks:

- outlet and edition identity uniqueness, including workspace-wide identity collisions;
- required outlet and snapshot metadata;
- positive and unique edition numbers per outlet;
- outlet existence and same-outlet correction lineage;
- predecessor existence and strictly increasing edition order;
- claim, source, and event references;
- duplicate provenance references;
- content hash integrity.

`StoryWorkspaceBinaryDocument` writes publication outlets and editions in format version 17. Earlier workspace versions remain readable. Outlets are restored before editions, and edition insertion order preserves correction lineage.

## Ownership boundary

- **StoryBuilder** authors the outlet, immutable edition, provenance, intended audience/access descriptions, correction lineage, validation, and persistence.
- **Galactic War / Experiences** owns media channels, scheduling, geographic reach, distribution, observer delivery/receipt, player presentation, and campaign-specific behavior.
- **AnyApp / hosts** load and present validated StoryBuilder content through the established composition contracts.

Do not add delivery or receipt flags to these records. An issued artifact is not evidence of exposure.
