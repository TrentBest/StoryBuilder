# StoryBuilder Gallery

StoryBuilder documents the creation of an Experience or Tool, then composes the world history and campaign context that make that Experience meaningful. Its first living example is its own origin story: a far-future human civilization built from recognizable history, hard-won recovery, interstellar expansion, and an Earth whose location has been lost.

## First recipe: from present fracture to a universe of worlds

Read [Story Foundation](STORY_FOUNDATION.md) for the premise and tonal rules, [Historical Scale](HISTORICAL_SCALE.md) for the millennia-to-event hierarchy, [Diegetic StoryBuilder](DIEGETIC_STORYBUILDER.md) for the tool's role inside The Singularity Workshop, and [Deployment Decision](DEPLOYMENT_DECISION.md) for the local desktop direction.

## First runnable proof: AnyApp desktop

The first runnable proof targets [AnyApp](https://github.com/TrentBest/AnyApp), the Workshop's Windows desktop host. StoryBuilder should not require a hosted WebApp, Azure account, remote database, or network connection for ordinary local writing.

The intended end-to-end desktop proof is:
1. Launch StoryBuilder through AnyApp.
2. Create and reopen a local project.
3. Edit the manuscript and project metadata.
4. Add a stable-ID event linked to a passage.
5. See the same event in a chronology view after reopening the project.

## Current implementation evidence

The host-neutral Core project now exists under `src/StoryBuilder.Core`:
- `StoryProject` stores project identity, title, premise, manuscript, passage anchors, and timeline events.
- Timeline events have stable GUID identity and can link to a reusable passage anchor.
- Chronology orders dated events first and leaves undated events after them.
- `LocalProjectStore` saves and reopens a schema-versioned local project file.
- `tests/StoryBuilder.Core.Tests` covers passage reuse, invalid passage links, chronology ordering, and persistence round-trip.
- [Integration Gap Register](INTEGRATION_GAPS.md) records why this Core slice is not yet launchable through the current AnyApp host.

The Core and test workflow have been committed, but no CI run has yet been independently confirmed. This is not yet a claim of a completed desktop application or living GUI.
