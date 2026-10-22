# StoryWorkspaceBinaryDocument

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

StoryWorkspaceBinaryDocument defines a StoryBuilder-owned binary schema for workspace records and explicit synonym mappings. It uses FSM_Serialization's IBinarySerializable contract without adding a general-purpose serializer or JSON dependency.

## UML class card

See [StoryWorkspaceBinaryDocument UML](../../docs/UML/StoryWorkspaceBinaryDocumentUML.md).

## Safety and compatibility

The format starts with the SBWS magic and version 8. Version 1 payloads default NarrativeRole to Undecided and have no premises or timelines. Version 2 adds premises. Version 3 adds ordered timeline definitions. Version 4 adds manuscripts and ordered manuscript sections. Version 5 adds provisional timeline planning points. Version 6 adds timeline assignment and placement certainty to world events. Version 7 adds observer-knowledge records. Version 8 adds stable event links on scene beats. Readers retain compatibility with versions 1 through 7. Strings and collection sizes are bounded; enum values, booleans, UTF-8, and end-of-payload are validated. Future incompatible schemas require a new format version and migration tests.

## Restore behavior

Timeline definitions are restored before planning points and timeline-assigned events so their references can be validated. Manuscript sections preserve authored text and event references. Scene beats preserve event IDs in version 8; older payloads restore with empty beat event links. Observer knowledge preserves its observer, event, acquisition anchor, source, and claim framing. Runtime FSM handles and UI state are not serialized.
