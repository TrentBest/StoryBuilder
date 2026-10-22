# StoryWorkspaceBinaryDocument

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

StoryWorkspaceBinaryDocument defines a StoryBuilder-owned binary schema for workspace records and explicit synonym mappings. It uses FSM_Serialization's IBinarySerializable contract without adding a general-purpose serializer or JSON dependency.

## UML class card

See [StoryWorkspaceBinaryDocument UML](../../docs/UML/StoryWorkspaceBinaryDocumentUML.md).

## Safety and compatibility

The format starts with the SBWS magic and version 7. Version 1 payloads default NarrativeRole to Undecided and have no premises or timelines. Version 2 adds premises. Version 3 adds ordered timeline definitions. Version 4 adds manuscripts and ordered manuscript sections. Version 5 adds provisional timeline planning points. Version 6 adds timeline assignment and placement certainty to world events. Version 7 adds observer-knowledge records. Readers retain compatibility with versions 1 through 6. Strings and collection sizes are bounded; enum values, booleans, UTF-8, and end-of-payload are validated. Future incompatible schemas require a new format version and migration tests.

## Restore behavior

Timeline definitions are restored before planning points and timeline-assigned events so their references can be validated. Manuscript sections preserve authored text and stable event/scene references. Observer-knowledge records are restored only after the referenced world events, and the evaluator uses accepted records only. Branch records must appear after their parent in the workspace collection. The vocabulary index's word occurrences are rebuilt from source records, avoiding stale duplicate index entries. The document does not persist runtime FSM handles, Process Group update state, or UI state.
