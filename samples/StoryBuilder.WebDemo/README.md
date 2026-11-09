# StoryBuilder Web Demo

This local Blazor host is the first clickable authoring surface for the StoryBuilder domain. It consumes a session created by StoryBuilderAuthoringBundle, which keeps the host on the same MicroBundle boundary exercised by the FSM_COS integration test. The demo autosaves its working workspace to a host-local versioned binary repository; existing browser-local snapshots are migrated on first load when no host workspace exists.

## Run

Prerequisite: .NET 8 SDK. The GUI.SVG dependency is source-pinned as a Git submodule; if you already cloned the repository, initialize it with `git submodule update --init --recursive`. A fresh clone should use `git clone --recurse-submodules`.

From the repository root:

```sh
dotnet run --project samples/StoryBuilder.WebDemo/StoryBuilder.WebDemo.csproj --urls http://localhost:5080
```

Open http://localhost:5080.

## Doors

- **Casting:** create, select, revise, and add/remove character traits; attribute a description to the author, character, narrator, rival, or rumor; add synonym mappings; and use FSM_API-backed review controls. Search indexed words and inspect their attribution.
- **The Stage:** capture ordered action/reaction/dialogue/pose beats, reorder them, link existing world events to specific beats, and copy an editable narration draft.
- **World Archive:** record historical claims and optional source provenance, keeping author status distinct from the claim itself.
- **Campaign Room:** create campaigns, record participant choices as proposals, and accept, reject, reopen, or deprecate choices through the FSM_API lifecycle.
- **Living Map:** view a live GUI.SVG manifestation of character cards, statuses, trait words, and the most-used vocabulary. It is generated from StoryBuilderSession data and refreshes as the workspace changes.
- **Timelines:** create root histories and branches, place provisional Plot Twist points, record timeline events, and see numbered event markers connected to the manuscript sections and scene beats that portray them.
- **Book Preview:** author actual manuscript text in ordered chapters, scenes, and passages, link sections to world-event identities, and render a deterministic Markdown digital-book artifact.
- **Observer Lens:** accept knowledge records, evaluate a claim at an event anchor, and render a GUI.SVG view containing only accepted, causally available observer knowledge.

## Visual asset storage

Uploaded PNG, JPEG, and WebP files are stored outside browser localStorage in the WebDemo host filesystem. The default directory is `App_Data/visual-assets` beneath the WebDemo content root and is served at `/visual-assets` for previews. Set `StoryBuilder:VisualAssets:StoragePath` in host configuration to select another directory (an absolute path is recommended for deployment). The workspace stores the asset URI and SHA-256 metadata; it does not contain the media bytes. Browser-local workspace data and filesystem media therefore need separate backup/restore handling. This is persistent host-local storage when the host volume is persistent, not cloud/object storage or multi-host shared storage.

## Workspace storage

Workspace snapshots are stored as versioned `.sbws` binary documents under `App_Data/workspaces` beneath the WebDemo content root. Set `StoryBuilder:Workspaces:StoragePath` to choose another host directory. The WebDemo uses a single workspace slot and does not yet provide account isolation, multi-user collaboration, cloud synchronization, or backup. A browser-local snapshot from an earlier build is imported only when no host snapshot exists; it is removed only after the host save succeeds. Uploaded media remains in the separate `App_Data/visual-assets` directory (configurable with `StoryBuilder:VisualAssets:StoragePath`), so both directories must be included in host backups.

This demo is not yet integrated into the WebPage host and does not provide physics simulation or a fully visual claymation stage. Those are explicit next steps, not implied features.
