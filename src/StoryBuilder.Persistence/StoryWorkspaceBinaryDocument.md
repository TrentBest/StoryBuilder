# StoryWorkspaceBinaryDocument

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

StoryWorkspaceBinaryDocument defines a StoryBuilder-owned binary schema for workspace records and explicit synonym mappings. It uses FSM_Serialization's IBinarySerializable contract without adding a general-purpose serializer or JSON dependency.

## UML class card

See [StoryWorkspaceBinaryDocument UML](../../docs/UML/StoryWorkspaceBinaryDocumentUML.md).

## Safety and compatibility

The format starts with the SBWS magic and version 2. It reads version 1 payloads by defaulting NarrativeRole to Undecided and initializing the new Premises collection empty. Strings and collection sizes are bounded; enum values, booleans, UTF-8, and end-of-payload are validated. Future incompatible schemas require a new format version and migration tests.

## Restore behavior

The workspace records and synonym mappings are restored. The vocabulary index's word occurrences are rebuilt from source records, avoiding stale duplicate index entries. The document does not persist runtime FSM handles or UI state.
