# StoryWorkspaceBinaryDocument

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

StoryWorkspaceBinaryDocument defines a StoryBuilder-owned binary schema for workspace records and explicit synonym mappings. It uses FSM_Serialization's IBinarySerializable contract without adding a general-purpose serializer or JSON dependency.

## UML class card

See [StoryWorkspaceBinaryDocument UML](../../docs/UML/StoryWorkspaceBinaryDocumentUML.md).

## Safety and compatibility

The format starts with the SBWS magic and version 3. Version 1 payloads default NarrativeRole to Undecided and have no premises or timelines. Version 2 payloads preserve premises but have no timeline collection. Version 3 adds the ordered timeline-definition collection before premises. Readers retain compatibility with versions 1 and 2. Strings and collection sizes are bounded; enum values, booleans, UTF-8, and end-of-payload are validated. Future incompatible schemas require a new format version and migration tests.

## Restore behavior

Timeline definitions and their parent references are restored before later authoring work continues. Branch records must appear after their parent in the workspace collection. The vocabulary index's word occurrences are rebuilt from source records, avoiding stale duplicate index entries. The document does not persist runtime FSM handles, Process Group update state, or UI state.
