# SceneRecord

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

SceneRecord stores a scene as small ordered beats. A beat can represent an action, reaction, dialogue line, transition, narration, or pose. This allows a user to play a scene, arrange it as a pose sequence, or capture it from freeform notes.

## UML class card

See [SceneRecord UML](../../../docs/UML/SceneRecordUML.md) for the class diagram and use case.

## Use case

1. Create a scene with a title and optional location.
2. Select Played, BeatByBeat, PoseSequence, or Claymation, or stay in freeform mode.
3. Capture only the beats the user knows: who acted, what changed, a line of dialogue, a pose, or a reaction.
4. Reorder beats and generate an editable narration draft from the recorded details.

## Boundary

The current composer does not simulate physics, infer intent, or invent missing sensory detail. It formats the captured material into a first-pass description. A future GUI/physics integration may supply richer pose and motion data without changing the narrative record's ownership.
