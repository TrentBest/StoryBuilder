# StoryContinuityAnalyzer

**Purpose:** Find reviewable continuity conflicts in a StoryBuilder workspace without changing the author's story.

## What it checks

- Events that directly or indirectly cause themselves.
- Cause links that are not reflected by the corresponding consequence link, and consequence links without the corresponding cause link.
- Timeline branches whose parent links form a cycle.

Each finding has a stable code, a primary record ID, a plain-language explanation, and the IDs of the related records. The analyzer does not choose canon, invent dates, repair links, or modify the workspace.

## Use cases

- Review causal history before treating an event sequence as established.
- Find timeline branches that cannot lead back to a root timeline.
- Present actionable continuity warnings in an authoring interface.
- Re-run the analysis after edits and verify that resolved findings disappear.

## Example

```csharp
var findings = StoryContinuityAnalyzer.Analyze(session.Workspace);
foreach (var finding in findings)
    Console.WriteLine($"{finding.Code}: {finding.Message}");
```

An empty result means none of these specific graph conflicts were found; it does not guarantee that all prose, dates, motives, or character knowledge are consistent.
