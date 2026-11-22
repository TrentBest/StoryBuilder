# InWorldPublicationEditionTests

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

Exercises immutable in-world publication artifacts independently from the manuscript's published-book editions. The Vesper Reach scenario records a station bulletin, an independent courier account, and a later correction while retaining a shared underlying event and explicit provenance.

## Invariants under test

- An outlet has stable identity; an edition has an outlet-local positive number.
- Issued content and its SHA-256 hash remain unchanged when a later correction is appended.
- Corrections must reference an earlier edition from the same outlet and use a greater edition number.
- Claim, source, and event references must resolve, and provenance collections are snapshotted when added.
- Issuing an artifact does not grant observer knowledge or imply delivery.
- A genre-neutral archival catalog can use the same contract.

## Persistence

The publication records round-trip through the current StoryWorkspaceBinaryDocument schema. The test does not assert compatibility with unpublished historical binary snapshots.

## UML class card

See [StoryWorkspaceBinaryDocument UML](../../docs/UML/StoryWorkspaceBinaryDocumentUML.md).
