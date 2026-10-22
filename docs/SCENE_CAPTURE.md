# Scene Capture: The Stage

## Goal

Let a person describe what they see or do without requiring them to first become a novelist. A scene is a sequence of observable beats, not one giant prose field.

## Capture modes

- **Freeform:** capture notes in whatever order is comfortable.
- **Played:** record actions and reactions as participants perform them.
- **BeatByBeat:** build the sequence explicitly.
- **PoseSequence / Claymation:** arrange named participants in poses or positions, advance to the next pose, and preserve the sequence.
- **Narration:** use the recorded beats to produce an editable prose draft.

These modes are labels in the current domain model. A live interactive playback or claymation surface is not yet implemented.

## A beat can record

- actor identity or a temporary actor label;
- action or change;
- reaction;
- dialogue;
- pose or position;
- emotional tone;
- capture note;
- attribution and canon status.

Do not require all fields for every beat. A quick action such as “Mara ducks” is a valid start; a detailed choreography can be added later.

## Event links

A scene beat can reference a canonical world event by stable ID. Use Stage's Timeline connections controls to link an event to a specific beat. The timeline map draws a separate scene-beat rail for these relationships. Removing a link does not delete the event or beat, and an event linked to a beat is not duplicated as new world truth.

## Narration rule

The current SceneComposer formats only recorded information into a first-pass draft. It does not infer motive, physics, injury, winner, or sensory details that the user did not supply. Generated wording remains editable and should retain links to the beats it summarizes.

## Future visual integration

GUI/GUI.SVG can display the same ordered beat model; Physics may contribute optional motion or contact information when the user wants it. StoryBuilder's narrative domain remains independent of any one renderer or physics implementation.
