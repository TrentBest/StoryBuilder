# PublishedBookEditionTests

These tests verify that publishing freezes the exact Markdown output, corrections create a new edition linked to the previous edition, scene provenance and content hashes survive binary restore, and workspace validation rejects altered published content.

The tests exercise `StoryBuilderSession.PublishManuscript`, `StoryWorkspaceValidator`, and `StoryWorkspaceBinaryDocument` format 11. They do not claim file-system export or real-host publishing UI is implemented.
