# Continuity Review page

**User moniker:** StoryBuilder author

## Purpose

The Continuity Review page runs `StoryContinuityAnalyzer` against the current in-memory workspace and displays advisory findings without changing author records.

## What authors see

- Counts for events, timelines, and findings.
- A clean result when the implemented structural checks find no conflicts.
- Finding code, explanation, primary record ID, and related record IDs when a check identifies a concern.
- A plain-language boundary describing what the checker does not assess.

## Use

Open **Continuity Review** from the main navigation. Fix records in their normal authoring pages and revisit the review. Findings are recomputed from the current workspace; this page does not silently repair or accept canon.

## Limitations

The checker does not validate narrative prose, date arithmetic, motive, character knowledge, or all possible continuity problems. A clean result means only that the implemented structural checks did not find a finding.
