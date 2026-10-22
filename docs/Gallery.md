# StoryBuilder Gallery

StoryBuilder captures the creation of an Experience and composes its characters, world history, timelines, scenes, and manuscript products. The local workbench uses canonical workspace records and GUI.SVG living views. Workshop WebPage hosting and external portrayal providers remain future integration work.

## Recipe 1: enter the workbench

Run the [interactive StoryBuilder WebDemo](../samples/StoryBuilder.WebDemo/README.md):

```sh
dotnet run --project samples/StoryBuilder.WebDemo/StoryBuilder.WebDemo.csproj --urls http://localhost:5080
```

Open http://localhost:5080.

- **Narrator:** capture an idea as an editable premise proposal. The page exposes the four cooperating FSM_API machines for intent, planning, observer constraints, and draft/review.
- **Places (Core API):** create canonical `PlaceRecord` entries, optionally nest them under parent places, and assign scenes to place IDs while retaining free-form location notes. This is available through `StoryBuilderSession`; it is not yet presented as a dedicated workbench screen.
- **Casting:** create and edit character records; assign Hero / Neutral / Antagonist / Undecided grouping; compare current profile, proposed revision, and indexed source context; add traits and synonyms.
- **The Stage:** capture ordered action, reaction, dialogue, and pose beats; reorder beats; link a world event to a specific beat; copy an editable narration draft.
- **World Archive:** record a historical claim and optionally attach a source record.
- **Campaign Room:** create a campaign and record a participant choice as a proposal.
- **Living Map:** see GUI.SVG render current characters, statuses, traits, and most-used vocabulary from StoryBuilderSession. The view updates when workspace records change.
- **Timelines:** create root histories and named child branches, record alternative outcomes for provisional Plot Twist points, link their affected scenes/events, explicitly promote a point to one proposed event while preserving its identity link, and record world events with approximate or unresolved placement. The core API also edits reciprocal cause/consequence links between events; Continuity Review flags causal cycles rather than rewriting the graph. GUI.SVG distinguishes planning points from actual events and draws separate event-to-book-section and event-to-scene-beat portrayal rails from canonical event IDs.
- **Book Preview:** create a working manuscript, add ordered chapters/scenes/passages, write actual text, link sections to canonical scene/event identities, and render deterministic Markdown. `BookProductRenderer.WriteMarkdown` writes the UTF-8 artifact to a caller-owned stream; the host still chooses the destination path.
- **Immutable book publications:** `PublishManuscript` freezes deterministic Markdown with SHA-256 integrity, section/scene/event provenance, and corrected-edition lineage; saved in workspace format 11.
- **Observer Lens:** record how a character witnessed, heard about, inferred, believed, disputed, or forgot an event. Accept knowledge explicitly, evaluate claims at a chosen event anchor, then use GUI.SVG to inspect only accepted, causally available knowledge.

The local Blazor host creates a session through StoryBuilderAuthoringBundle and uses the source-pinned GUI.SVG project to render living views. It autosaves a versioned binary workspace (currently format version 12) in browser localStorage and restores it after refresh. Storage is local to this browser; there is no cloud sync, account sharing, or WebPage deployment yet.

## Recipe 2: host-neutral first-book command slice 

`StoryBuilderAuthoringSurface` exposes explicit commands for creating a timeline event, creating a manuscript passage, and linking the latest event to the latest passage. Event creation creates a default `Main Timeline` if the workspace has none; passage creation creates an `Untitled Book` if necessary. The link command reports a safe failure if either side is missing. Automated tests cover these command-to-domain effects and the stable event ID stored by the passage. AnyApp now has dispatch and persistence code for this command slice; the remaining desktop gap is selecting/binding the StoryBuilder surface and refreshing the rendered tree after a command.\n\n## Recipe 3: run the compact domain example

```sh
dotnet run --project samples/StoryBuilder.Quickstart/StoryBuilder.Quickstart.csproj
```

This console sample creates an editable character, automatically indexes its traits, maps “brave” to a “courageous” vocabulary concept while preserving the word and who used it, and composes a scene from pose, dialogue, and action beats.

## Next living proof

Make each book or scene-beat rail row navigate directly to its source record. Connect the Narrator's constrained context to optional Renderer-backed visual/semantic representations through a host-neutral portrayal request and explicit result contract. The current event placement remains descriptive rather than a measured time axis; the evaluator fails closed when causal availability cannot be established. The binary package compiler/reader has passed CI; next, integrate its validated artifacts with the actual AnyApp host without rerunning Workshop arbitration. The broader contract is in [Timelines, Gestures, and Multiverse History](TIMELINES_GESTURES_MULTIVERSE.md).
