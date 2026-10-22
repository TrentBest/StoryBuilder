# StoryBuilder Gallery

StoryBuilder captures the creation of an Experience and composes its characters, world history, timelines, scenes, and manuscript products. The local workbench uses canonical workspace records and GUI.SVG living views. Workshop WebPage hosting and external portrayal providers remain future integration work.

## Recipe 1: enter the workbench

Run the [interactive StoryBuilder WebDemo](../samples/StoryBuilder.WebDemo/README.md):

```sh
dotnet run --project samples/StoryBuilder.WebDemo/StoryBuilder.WebDemo.csproj --urls http://localhost:5080
```

Open http://localhost:5080.

- **Narrator:** capture an idea as an editable premise proposal. The page exposes the four cooperating FSM_API machines for intent, planning, observer constraints, and draft/review.
- **Casting:** create and edit character records; assign Hero / Neutral / Antagonist / Undecided grouping; compare current profile, proposed revision, and indexed source context; add traits and synonyms.
- **The Stage:** capture ordered action, reaction, dialogue, and pose beats; reorder beats; copy an editable narration draft.
- **World Archive:** record a historical claim and optionally attach a source record.
- **Campaign Room:** create a campaign and record a participant choice as a proposal.
- **Living Map:** see GUI.SVG render current characters, statuses, traits, and most-used vocabulary from StoryBuilderSession. The view updates when workspace records change.
- **Timelines:** create root histories and named child branches, add provisional Plot Twist points, and record world events with approximate or unresolved placement. GUI.SVG distinguishes planning points from actual events and now draws an event-to-book portrayal rail from the same canonical event IDs used by the manuscript.
- **Book Preview:** create a working manuscript, add ordered chapters/scenes/passages, write actual text, and link sections to world-event identities.
- **Observer Lens:** record how a character witnessed, heard about, inferred, believed, disputed, or forgot an event. Accept knowledge explicitly, then evaluate whether a claim is allowed, blocked, or unresolved at a chosen event anchor.

The local Blazor host creates a session through StoryBuilderAuthoringBundle and uses the source-pinned GUI.SVG project to render living views. It autosaves a versioned binary workspace (currently format version 7) in browser localStorage and restores it after refresh. Storage is local to this browser; there is no cloud sync, account sharing, or WebPage deployment yet.

## Recipe 2: run the compact domain example

```sh
dotnet run --project samples/StoryBuilder.Quickstart/StoryBuilder.Quickstart.csproj
```

This console sample creates an editable character, automatically indexes its traits, maps “brave” to a “courageous” vocabulary concept while preserving the word and who used it, and composes a scene from pose, dialogue, and action beats.

## Next living proof

Expose the accepted observer-knowledge projection as a living GUI.SVG view at a chosen event anchor, then add scene/beat-level event links so a timeline event can point into the exact scene beats that portray it. Connect the Narrator's constrained context to optional Renderer-backed visual/semantic representations through a host-neutral portrayal request and explicit result contract. The current event placement remains descriptive rather than a measured time axis; the evaluator fails closed when causal availability cannot be established. The binary package compiler/reader has passed CI; next, integrate its validated artifacts with the actual AnyApp host without rerunning Workshop arbitration. The broader contract is in [Timelines, Gestures, and Multiverse History](TIMELINES_GESTURES_MULTIVERSE.md).
