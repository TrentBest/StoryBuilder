# StoryBuilder Gallery

StoryBuilder is the tooling that captures the creation of an Experience and composes its characters, world history, scenes, and campaigns. The local web workbench exercises real domain behavior and uses GUI.SVG for living workspace and timeline views. Integration with the Workshop's WebPage host remains future work.

## Recipe 1: enter the workbench

Run the [interactive StoryBuilder WebDemo](../samples/StoryBuilder.WebDemo/README.md):

```sh
dotnet run --project samples/StoryBuilder.WebDemo/StoryBuilder.WebDemo.csproj --urls http://localhost:5080
```

Open http://localhost:5080.

- **Narrator:** enter one free-form idea, optionally add scale/genre/approach/target hints, and get an editable premise proposal with stable identity and review status.
- **Casting:** create and edit character records; assign Hero / Neutral / Antagonist / Undecided grouping; compare current profile, proposed revision, and indexed source context; add traits and synonyms.
- **The Stage:** capture ordered action, reaction, dialogue, and pose beats; reorder beats; copy an editable narration draft.
- **World Archive:** record a historical claim and optionally attach a source record.
- **Campaign Room:** create a campaign and record a participant choice as a proposal.
- **Living Map:** see GUI.SVG render current characters, statuses, traits, and most-used vocabulary from StoryBuilderSession. The view updates when workspace records change.
- **Timelines:** create root histories and named child branches. GUI.SVG draws the declared parent relationships from the canonical timeline definitions; connectors are not yet event-causality or world-time lines.

This is an interactive local Blazor host using a session created through StoryBuilderAuthoringBundle. It uses the source-pinned GUI.SVG project to render a living workspace view and autosaves a versioned binary workspace in browser localStorage, restoring it after refresh. Storage is local to this browser; there is no cloud sync, account sharing, or WebPage deployment yet.

## Recipe 2: run the compact domain example

```sh
dotnet run --project samples/StoryBuilder.Quickstart/StoryBuilder.Quickstart.csproj
```

This console sample creates an editable character, automatically indexes its traits, maps “brave” to a “courageous” vocabulary concept while preserving the word and who used it, and composes a scene from pose, dialogue, and action beats.

## Next living proof

Extend the GUI.SVG projections from timeline definitions to canonical event identities, explicit divergence points, scene-to-event links, and observer-aware disclosure—only as each relationship becomes real domain data. The current timeline view shows declared parent/branch relationships, not temporal simulation. After that, integrate the StoryBuilder MicroBundle into the Workshop host, add export/backup and schema migration, then extend The Stage with gesture/pose visualization and optional physics data. The broader design contract is in [Timelines, Gestures, and Multiverse History](TIMELINES_GESTURES_MULTIVERSE.md); it describes intended behavior, not shipped functionality.
