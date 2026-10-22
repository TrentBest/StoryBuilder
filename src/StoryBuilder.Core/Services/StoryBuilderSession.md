# StoryBuilderSession

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

Coordinates canonical workspace edits, vocabulary indexing, timeline planning, manuscript composition, world-event links, and observer-knowledge records. It keeps host and persistence concerns outside the domain core.

## Use cases

- Capture and revise a premise.
- Create timelines, provisional planning points, and timeline-assigned events.
- Set de-duplicated Plot Twist alternatives and link/unlink the scenes and events each provisional point may affect.
- Revise existing world events with `UpdateTimelineEvent`; title and summary edits rebuild their vocabulary occurrences.
- Add or remove reciprocal cause/consequence edges with `LinkEventCausality`; self-causation and missing endpoints are rejected.
- Create a working manuscript and link its sections to world events.
- Publish a manuscript with `PublishManuscript`; the returned immutable snapshot contains deterministic Markdown and a SHA-256 content hash.
- Publish a correction as a new edition with optional correction notes; `PreviousEditionId` preserves the lineage and does not rewrite earlier output.
- Link a world event to a specific scene beat with `LinkEventToSceneBeat`; removing a link does not delete either record.
- Add or revise observer-knowledge records with explicit status transitions.
- Notify the host when canonical records change so live views can refresh.

## UML class card

See [StoryBuilderSession UML](../../../docs/UML/StoryBuilderSessionUML.md).
