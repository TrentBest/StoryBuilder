# Importing a legacy StoryBuilder project

The first-generation StoryBuilder JSON project format can be imported into the current workspace model with `LegacyStoryProjectImporter` from `StoryBuilder.Persistence`.

```csharp
using StoryBuilder.Persistence;

var json = await File.ReadAllTextAsync("my-story.storybuilder.json");
var workspace = LegacyStoryProjectImporter.Import(json);
var document = new StoryWorkspaceBinaryDocument(workspace);
// Pack the document through the application's configured FSM_Serialization stream.
```

## What is mapped

- The legacy project ID becomes the workspace ID.
- The title and premise become a workspace name and a story premise.
- The manuscript text is imported unchanged into a manuscript section when non-empty.
- Timeline event IDs, titles, descriptions, and timestamp offsets are represented in the workspace event records.
- A deterministic timeline and manuscript identity are generated from the source project ID so repeat imports are stable.

## Loss prevention

The original JSON is retained verbatim in `StoryWorkspace.LegacyProjectSourceJson` and persisted by binary workspace format version 9. This is an archival preservation field, not a second live model: edits made to the new workspace do not rewrite the archived JSON. Legacy passage anchors and string observer IDs are not yet first-class equivalents in the current model, so the archive remains the authoritative place to recover their exact original values. Do not discard the archive until those concepts have an explicit migration path.

Versions 1–8 of the binary workspace format remain readable. New saves use version 9. Unsupported legacy JSON schema versions fail with an explicit error instead of being guessed at.
