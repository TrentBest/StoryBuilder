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

This is an interactive local Blazor host using a session created through StoryBuilderAuthoringBundle. Data is in-memory and scoped to the browser circuit; refresh starts a new workspace. It is not yet deployed or embedded into WebPage.

## Recipe 2: run the compact domain example

```sh
dotnet run --project samples/StoryBuilder.Quickstart/StoryBuilder.Quickstart.csproj
```

This console sample creates an editable character, automatically indexes its traits, maps “brave” to a “courageous” vocabulary concept while preserving the word and who used it, and composes a scene from pose, dialogue, and action beats.

## Next living proof

The next end-to-end Experience should show the same character and vocabulary state through GUI.SVG, generated from the actual StoryBuilderSession rather than a static mockup. After that, integrate the StoryBuilder MicroBundle into the Workshop host and add persistent storage, then extend The Stage with pose visualization and optional physics data.
