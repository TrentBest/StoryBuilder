# StoryBuilder Gallery

StoryBuilder is the tooling that captures the creation of an Experience and composes its characters, world history, scenes, and campaigns. The first local web workbench now exercises real domain behavior; integration with the Workshop's WebPage and living GUI.SVG views remains future work.

## Recipe 1: enter the workbench

Run the [interactive StoryBuilder WebDemo](../samples/StoryBuilder.WebDemo/README.md):

```sh
dotnet run --project samples/StoryBuilder.WebDemo/StoryBuilder.WebDemo.csproj --urls http://localhost:5080
```

Open http://localhost:5080.

- **Casting:** create and edit character records; add or remove traits; search word occurrences with attribution.
- **The Stage:** capture ordered action, reaction, dialogue, and pose beats; reorder beats; copy an editable narration draft.
- **World Archive:** record a historical claim and optionally attach a source record.
- **Campaign Room:** create a campaign and record a participant choice as a proposal.
- **Living Map:** see GUI.SVG render the current characters, statuses, traits, and most-used vocabulary from StoryBuilderSession. The view updates when workspace records change.

This is an interactive local Blazor host using a session created through StoryBuilderAuthoringBundle. It uses the source-pinned GUI.SVG project to render a living workspace view and autosaves a versioned binary workspace in browser localStorage, restoring it after refresh. Storage is local to this browser; there is no cloud sync, account sharing, or WebPage deployment yet.

## Recipe 2: run the compact domain example

```sh
dotnet run --project samples/StoryBuilder.Quickstart/StoryBuilder.Quickstart.csproj
```

This console sample creates an editable character, automatically indexes its traits, maps “brave” to a “courageous” vocabulary concept while preserving the word and who used it, and composes a scene from pose, dialogue, and action beats.

## Next living proof

The next end-to-end Experience should show the same character and vocabulary state through GUI.SVG, generated from the actual StoryBuilderSession rather than a static mockup. After that, integrate the StoryBuilder MicroBundle into the Workshop host, add export/backup and schema migration, then extend The Stage with pose visualization and optional physics data. The timeline/gesture/multiverse design contract is in [Timelines, Gestures, and Multiverse History](TIMELINES_GESTURES_MULTIVERSE.md); it describes intended behavior, not shipped functionality.
