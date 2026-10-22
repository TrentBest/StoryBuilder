# StoryBuilder Web Demo

This local Blazor host is the first clickable authoring surface for the StoryBuilder domain. It consumes a session created by StoryBuilderAuthoringBundle, which keeps the host on the same MicroBundle boundary exercised by the FSM_COS integration test.

## Run

Prerequisite: .NET 8 SDK.

From the repository root:

```sh
dotnet run --project samples/StoryBuilder.WebDemo/StoryBuilder.WebDemo.csproj --urls http://localhost:5080
```

Open http://localhost:5080.

## Doors

- **Casting:** create, select, revise, and add/remove character traits; attribute a description to the author, character, narrator, rival, or rumor; add synonym mappings; and use FSM_API-backed review controls. Search indexed words and inspect their attribution.
- **The Stage:** capture ordered action/reaction/dialogue/pose beats, reorder them, and copy an editable narration draft.
- **World Archive:** record historical claims and optional source provenance, keeping author status distinct from the claim itself.
- **Campaign Room:** create campaigns, record participant choices as proposals, and accept, reject, reopen, or deprecate choices through the FSM_API lifecycle.

## Current boundary

The workspace is scoped to the current browser and autosaves as a versioned binary document in localStorage. Refreshing the page restores characters, scenes, claims, sources, campaigns, participant actions, and synonym mappings. This demo is not yet integrated into the WebPage host, does not synchronize data across devices or users, and does not provide physics simulation or a fully visual claymation stage. Those are explicit next steps, not implied features.
