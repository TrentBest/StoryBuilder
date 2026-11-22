# RaWWar Empire Workspace

`RaWWarEmpireWorkspace` builds an inspectable StoryBuilder workspace from the living raWWar Empire history source.

## Source and provenance

The workspace links directly to [Empire History & Faction Origins](https://github.com/TrentBest/raWWar/blob/ddd4c0b5ce34d4a1291b76824b265701cb32e436/docs/EMPIRE_HISTORY_AND_FACTION_ORIGINS.md). Its source record describes the document as a living design model and preserves the source's explicit uncertainty around names, dates, rulers, and specific events.

## Records authored

- Thirteen first-class `OrganizationRecord` entries, one for each major House.
- Source-attributed `HistoricalClaim` records for the Houses and the shared historical model.
- One provisional historical timeline with undated `TimelinePlanningPoint` anchors for the source's named sections.
- Continuity issues that keep chronology, internal House variation, and interpretation open for review.

## Lifecycle rules

House organizations remain `Draft`. Claims and continuity issues remain `Proposed`. The timeline has planning points only: it does not invent dated events. House descriptions are concise source summaries, not universal descriptions of every member or sub-faction.

Run the Quickstart project to print the workspace's source, House, claim, outline, and event counts. The corresponding tests verify workspace integrity and binary persistence.

See the [workspace builder UML](../../docs/UML/RaWWarEmpireWorkspaceUML.md) for the composition and record relationships. The [Historical Discovery and Place-Based Lore](../../docs/HISTORICAL_DISCOVERY_AND_PLACE_BASED_LORE.md) guide describes how future locations can carry evidence, competing interpretations, and observer-specific discoveries without prematurely fixing canon.
