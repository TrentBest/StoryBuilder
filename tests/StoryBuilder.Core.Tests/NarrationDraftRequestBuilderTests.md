# NarrationDraftRequestBuilderTests

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

Verifies that workspace-backed narration requests contain only explicitly selected, correctly ordered scene beats and the requested manuscript context.

## UML class card

See [Narration Draft Request Flow UML](../../docs/UML/NarrationDraftRequestFlowUML.md).

## Acceptance evidence

- Selected beat IDs are snapshotted in scene sequence order.
- Scene title/location and manuscript/section identity and existing text are copied into the request.
- A beat from another scene and duplicate beat selections are rejected.
- Observer-constrained requests include the observer projection and reject unlinked beats that could leak ungrounded material.
- Voice/style hints are normalized without mutating the source records.
