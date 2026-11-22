# StoryWorkspaceBinaryDocument

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

StoryWorkspaceBinaryDocument defines a StoryBuilder-owned binary schema for workspace records and explicit synonym mappings. It uses FSM_Serialization's IBinarySerializable contract without adding a general-purpose serializer or JSON dependency.

## UML class card

See [StoryWorkspaceBinaryDocument UML](../../docs/UML/StoryWorkspaceBinaryDocumentUML.md).

## Safety and compatibility

The format starts with the SBWS magic and version 18. The schema evolves by appending version-gated fields and collections while preserving the interpretation of prior versions:

| Version | Added persisted data |
| --- | --- |
| 1 | Initial workspace and character records |
| 2 | Story premises |
| 3 | Ordered timeline definitions |
| 4 | Manuscripts and ordered manuscript sections |
| 5 | Timeline planning points |
| 6 | Timeline assignment and placement certainty for world events |
| 7 | Observer-knowledge records |
| 8 | Stable event links on scene beats |
| 9 | Original legacy project JSON |
| 10 | Canonical places and scene place IDs |
| 11 | Immutable published Markdown snapshots, SHA-256 integrity, provenance, and correction lineage |
| 12 | Durable promoted-event identity on planning points |
| 13 | Visual asset records and immutable revisions |
| 14 | Append-only visual asset review decisions |
| 15 | Page-composition layouts |
| 16 | Optional discovery-interaction settings |
| 17 | Organizations and organization participant IDs on events |
| 18 | Exact visual asset revision identity selected by each page composition |

Readers retain compatibility with formats 1–17. A composition restored from a format before 18 has no selected visual revision; the reader does not infer one from the asset's current revision list. Publication export requires an explicit selection and an unambiguous latest Approved review on that exact revision.

Strings and collection sizes are bounded; enum values, booleans, UTF-8, and end-of-payload are validated. Future incompatible schemas require a new format version and migration tests.

## Restore behavior

Records are restored in dependency order so references can be validated: timelines before planning points and timeline-assigned events; manuscripts before published editions; assets before reviews and page compositions; organizations before event organization references are validated. Published editions preserve correction lineage. Planning-point promotion identity is stored from version 12 onward. Observer knowledge preserves its observer, event, acquisition anchor, source, and claim framing. Visual asset revisions and review decisions remain append-only. Page compositions preserve their exact selected revision from version 18 onward. Runtime FSM handles and UI state are not serialized.

## Verification

Persistence and compatibility coverage is in `tests/StoryBuilder.Core.Tests/StoryWorkspacePersistenceTests.cs`. Organization authoring and event-participant coverage is in `tests/StoryBuilder.Core.Tests/OrganizationAuthoringTests.cs`.
