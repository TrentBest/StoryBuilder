# StoryWorkspaceBinaryDocument

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

StoryWorkspaceBinaryDocument defines the single current StoryBuilder workspace binary schema and explicit synonym mappings. It uses FSM_Serialization's IBinarySerializable contract without adding a general-purpose serializer or JSON dependency.

## Current development contract

The payload begins with the four-byte `SBW1` marker followed by schema version `3`. Version 3 is the current development shape and includes immutable campaign sourcebook attachments plus adventure, quest, and quest-objective records.

The reader accepts only the current schema version. It does not carry compatibility branches for unpublished development snapshots. When the workspace model changes, update the writer, reader, fixtures, tests, and every in-repository consumer together. Development workspaces may be regenerated when the schema changes.

This is an internal persistence contract, not a released interchange format. A future public release can establish its own compatibility policy based on actual published data and consumers.

## Persisted workspace shape

The current document includes:

- experience definition, timelines, planning points, manuscripts, premises, and characters;
- canonical places, scenes, claims, events, observer knowledge, campaigns, participant actions, sources, and continuity issues;
- legacy import source text retained as part of the current workspace model;
- immutable published book editions;
- visual assets, append-only reviews, page compositions, discovery settings, and exact selected visual revision IDs;
- organizations and organization participants on events;
- stable publication outlets and immutable in-world publication editions, including provenance and correction lineage;
- campaign sourcebook edition pins, adventures, quests, and ordered quest objectives;
- explicit vocabulary synonym mappings.

## Restore behavior

Records are restored in dependency order so references can be validated: timelines before planning points and timeline-assigned events; manuscripts before published editions; assets before reviews and page compositions; organizations before event organization references are validated; publication outlets before in-world editions; and campaigns before their attached sourcebooks, adventures, quests, and objectives. Published editions and in-world editions preserve append-only correction lineage.

Strings and collection sizes are bounded; enum values, booleans, UTF-8, cross-record references, content hashes, and end-of-payload are validated. Runtime FSM handles and UI state are not serialized.

## Verification

Persistence coverage is in `tests/StoryBuilder.Core.Tests/StoryWorkspacePersistenceTests.cs`. In-world publication coverage is in `tests/StoryBuilder.Core.Tests/InWorldPublicationEditionTests.cs` and `tests/StoryBuilder.Core.Tests/VesperReachAuthoredWorkspaceTests.cs`.
