# SceneRecord

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

SceneRecord stores a scene as small ordered beats. A beat can represent an action, reaction, dialogue line, transition, narration, or pose. Each beat may link to one or more canonical world-event IDs so the same event can be connected to the exact scene moment that portrays it without duplicating world truth.

## UML class card

See [SceneRecord UML](../../../docs/UML/SceneRecordUML.md) for the class diagram and use cases.

## Use cases

1. Create a scene with a title and optional location.
2. Capture only the beats the user knows: who acted, what changed, a line of dialogue, a pose, or a reaction.
3. Reorder beats and generate an editable narration draft from the recorded details.
4. Link a key event to a beat and inspect the link from the timeline map.

## Boundary

The current composer does not simulate physics, infer intent, or invent missing sensory detail. It formats captured material into a first-pass description. A future GUI/physics integration may supply richer pose and motion data without changing narrative record ownership.
