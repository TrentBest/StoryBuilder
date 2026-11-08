# Visual Asset Records

Visual assets are workspace records, not loose file annotations. The workspace owns identity, story relationships, source provenance, and review history; the actual media bytes remain in the repository or another durable asset store.

## Domain model

- `VisualAssetRecord` is the stable identity for a creative target, such as Lenny's character turnaround, a beach environment guide, or a page illustration.
- `VisualAssetRevision` is an immutable revision with its own identity, sequential revision number, durable asset location, media type, optional SHA-256, creation prompt, production notes, and review evidence.
- Asset relationships may point to a character, scene, manuscript section, and any number of source records.
- Review decisions are separate append-only records: `Approved`, `Rejected`, or `ChangesRequested`.

A revision is appended through `StoryWorkspace.AddVisualAssetRevision`. Existing revisions are not replaced when a new proposal is made. Reviews are appended through `StoryWorkspace.AddVisualAssetReview`; each points to one exact asset and revision and records reviewer attribution, timestamp, decision, and notes. A later revision does not inherit an earlier revision's approval. Review history is not overwritten when another decision is recorded.

## Provenance and canon

A generated image is a proposal until its exact revision is approved. Physical traits not established by the source manuscript must not be silently promoted into character canon. Keep prompts and production notes with the revision so an author can understand how it was made.

The asset record's source links establish which manuscript or reference material informed the work. The page/scene/character links establish where the asset belongs in the composition.

## Persistence and integrity

The binary workspace format is version 14. Earlier versions remain readable; versions 1–12 restore with no visual assets, and version 13 restores its assets with no review history. Version 14 serializes assets, immutable revisions, and append-only review decisions explicitly.

Workspace validation checks:
- workspace-wide identity uniqueness, including revision identities;
- character, scene, manuscript-section, and source references;
- sequential revision numbering;
- SHA-256 syntax when a content hash is supplied;
- valid review-to-asset and review-to-revision ownership links;
- reviewer attribution on every review record.

The workspace stores durable relative paths or URIs, never temporary machine-local paths. Media bytes are deliberately kept outside the binary workspace document.

## Lenny production sequence

1. Character turnarounds for Lenny, Dad, and the lead dolphin.
2. Beach, shoreline, open-water, ocean-floor, and reef environment guides.
3. A small set of representative page compositions.
4. Continuity and author review.
5. The remaining manuscript illustrations.
6. Motion studies and restrained animation tracks, with a static/reduced-motion reader fallback.

The manuscript remains authoritative. Visual asset approval is a separate creative decision and does not rewrite manuscript text.
