# StoryWorkspaceBinaryDocument

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

StoryWorkspaceBinaryDocument defines the single current StoryBuilder workspace binary schema and explicit synonym mappings. It uses FSM_Serialization's IBinarySerializable contract without adding a general-purpose serializer or JSON dependency.

## Current development contract

The payload begins with the four-byte `SBW1` marker followed by schema version `5`. Version 5 adds author-accepted narration snapshots to manuscript sections, including the accepted text, request/scene identity, author and provider/model attribution, source-beat text ranges, and event claims. The reader also accepts version `4`; version-4 manuscripts restore with an empty narration-acceptance history.

The writer emits version 5. The reader accepts versions 4 and 5, with version-4 manuscript sections defaulting to no narration acceptance records. When the workspace model changes, update the writer, reader, fixtures, tests, and every in-repository consumer together.

This is an internal persistence contract, not a released interchange format. A future public release can establish its own compatibility policy based on actual published data and consumers.

## Persisted workspace shape

The current document includes:

- experience definition, timelines, planning points, manuscripts, append-only accepted narration provenance, premises, and characters;
- canonical places, scenes, claims, events, observer knowledge, campaigns, participant actions, sources, and continuity issues;
- legacy import source text retained as part of the current workspace model;
- immutable published book editions;
- visual assets, append-only reviews, page compositions, discovery settings, and exact selected visual revision IDs;
- organizations and organization participants on events;
- stable publication outlets and immutable in-world publication editions, including provenance and correction lineage;
- campaign sourcebook edition pins, adventures, quests, ordered quest objectives, and campaign sessions with lifecycle timestamps;
- explicit vocabulary synonym mappings.

## Restore behavior

Records are restored in dependency order so references can be validated: timelines before planning points and timeline-assigned events; manuscripts before published editions; assets before reviews and page compositions; organizations before event organization references are validated; publication outlets before in-world editions; and campaigns before their attached sourcebooks, adventures, quests, objectives, and sessions. Published editions and in-world editions preserve append-only correction lineage.

Strings and collection sizes are bounded; enum values, booleans, UTF-8, cross-record references, content hashes, and end-of-payload are validated. Runtime FSM handles and UI state are not serialized.

## Verification

Persistence coverage is in `tests/StoryBuilder.Core.Tests/StoryWorkspacePersistenceTests.cs`. In-world publication coverage is in `tests/StoryBuilder.Core.Tests/InWorldPublicationEditionTests.cs` and `tests/StoryBuilder.Core.Tests/VesperReachAuthoredWorkspaceTests.cs`.
