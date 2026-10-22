# PublishedBookEdition

**Owner:** StoryBuilder Core domain

`PublishedBookEdition` is the frozen output of an explicit manuscript publication action. It stores the exact Markdown output, SHA-256 content hash, source manuscript identity and update timestamp, ordered section, scene, and event references, publication time, and optional link to the previous edition with correction notes.

Use `StoryBuilderSession.PublishManuscript` to create a snapshot. Editing the working manuscript afterward does not change an existing publication. Publishing a correction creates a new edition that points to the prior edition. Workspace validation checks the source manuscript, lineage, section/event references, and content hash; binary workspace format 11 persists the snapshots while readers continue to accept formats 1–10.

## Use case

1. Finish the working manuscript and its section/event links.
2. Call `PublishManuscript(manuscriptId)` to freeze the current deterministic Markdown output.
3. Continue editing the working draft independently.
4. Call `PublishManuscript(manuscriptId, correctionNotes)` to publish a correction; inspect `PreviousEditionId` to follow the lineage.
5. Save and restore the workspace; each published artifact retains its original content and hash.
