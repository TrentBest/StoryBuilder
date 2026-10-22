# StoryBuilder Gallery

StoryBuilder is the tooling that captures the creation of an Experience and composes its characters, world history, scenes, and campaigns. The first proof is a real domain workflow; living GUI views will follow when the same records drive an actual host.

## Recipe 1: create a character, index a word, compose a scene

Run the [StoryBuilder Quickstart](../samples/StoryBuilder.Quickstart/Program.cs) with:

```sh
dotnet run --project samples/StoryBuilder.Quickstart/StoryBuilder.Quickstart.csproj
```

The sample uses the actual C# core to:
1. Create an editable Investigator character draft.
2. Add a trait without requiring a full biography.
3. Map “brave” to a “courageous” vocabulary concept and record its usage.
4. Capture a pose, dialogue line, and action as ordered scene beats.
5. Compose an editable narration draft from those beats.

This is a runnable console proof, not a claim that the final clickable Casting UI or claymation view already exists.

## Recipe 2: from a historical claim to a campaign

Read [Story Foundation](STORY_FOUNDATION.md) for the setting, [Historical Scale](HISTORICAL_SCALE.md) for the millennia-to-event hierarchy, and [Diegetic StoryBuilder](DIEGETIC_STORYBUILDER.md) for the in-world tool concept. The workspace model now includes source records, historical claims, events, campaigns, participant actions, and continuity issues; a complete persisted campaign workflow remains future work.

## Next living proof

The next end-to-end Experience should let a user create a character through Casting, revise a trait, and immediately see the changed vocabulary counts and character view. The visible result must be generated from the same underlying records—not a static mockup. Later, the Stage should let the user play or pose a scene and preserve each beat's provenance.
