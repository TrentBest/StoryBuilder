# StoryBuilder Runtime and Package Boundaries

This page describes the code that ships in the repository and how its pieces behave when used. The domain meanings and authoring rules are defined in [Story Domain Contract](STORY_DOMAIN_CONTRACT.md).

## Package responsibilities

- **StoryBuilder.Core** owns host-neutral records and in-memory authoring operations: premises, characters and relationships, canonical places, scenes and beats, timelines and planning points (including alternatives, impact links, and explicit promotion), world events and reciprocal causal links, working manuscripts, immutable published book editions, sources and historical claims, campaigns, participant actions, observer knowledge, continuity issues, and vocabulary indexing.
- **StoryBuilder.Persistence** encodes and restores a workspace through the bounded FSM_Serialization binary stream contract. Workspace format 12 includes canonical places and scene-to-place links, immutable Markdown publications with SHA-256 integrity and correction lineage, and durable planning-point promotion links; readers for formats 1–11 remain supported.
- **StoryBuilder.FSM** supplies optional FSM_API lifecycle integration.
- **StoryBuilder.MicroBundle** exposes the authoring bundle through the neutral MicroBundleDomain contract. It does not depend directly on FSM_COS.
- **StoryBuilder.WebDemo** hosts the local Blazor workbench and its browser-local autosave.
- **StoryBuilder.Quickstart** demonstrates Core authoring from a console application.

The core records do not depend on GUI, renderer, physics, or host implementations. Hosts choose how to present and persist the workspace.

## Working with a workspace

Create a `StoryBuilderSession` to author records and keep the derived vocabulary index synchronized. Use session methods for creating and revising characters, places, scenes, timelines, events, and manuscript sections; edit cause/consequence edges reciprocally; record provisional alternatives and impact links; explicitly promote a planning point into one proposed event; link sections to scenes/events; and publish immutable Markdown snapshots. `StoryWorkspace` owns the records; IDs are the links between them.

For a canonical location, create a `PlaceRecord` and assign its ID to a scene with `StoryBuilderSession.AssignSceneToPlace`. The scene's free-form `Location` text remains available for older data and additional notes. Parent places must be created before child places.

Use `StoryWorkspaceValidator.Validate` after assembling or restoring a workspace. It reports duplicate identities, dangling references, and invalid links without mutating the workspace or inventing replacements. Resolve reported issues explicitly when a consistent workspace is required.

## Story meaning and portrayal

- A `StoryEvent` is the stable identity for something that happens in the story world. Its status, timeline, participants, causes, consequences, and sources are separate from prose that portrays it.
- A `SceneBeat` captures an ordered action, reaction, dialogue line, pose, transition, or narration beat. It can reference the event it portrays.
- A `ManuscriptSection` stores reading-order text and may reference scenes and events. Multiple sections or beats may portray the same event without creating duplicate world events.
- A `HistoricalClaim` records an assertion about the world's past; `SourceRecord` records its provenance. A source making a claim does not by itself establish that claim as canon.
- `ObserverKnowledgeRecord` records what an observer knows, the basis for that knowledge, and its acquisition point. `ObserverKnowledgeEvaluator` denies unsupported claims, distinguishes witnessed facts from reports and inference, and checks event-scoped knowledge against causal history.
- `ParticipantAction` is an attributed proposal. It does not become an accepted campaign consequence merely because it exists.
- `TimelinePlanningPoint` supports intentionally incomplete planning. Its presence is not a confirmed event or an exact date.

These distinctions let one world history support multiple portrayals and perspectives without treating manuscript order, chronology, causal order, and observer knowledge as interchangeable.

## Persistence and presentation

The binary workspace document validates references at the persistence boundary and preserves supported records and IDs through save/restore. Format 10 adds canonical places and scene-to-place references; format 11 adds immutable published Markdown snapshots with content hashes and correction lineage; format 12 adds scene provenance to publications and durable planning-point promotion links. Readers for formats 1–11 remain supported, with newer optional links defaulting empty when absent. Older free-form scene location text remains intact. Legacy JSON import stores the original source payload alongside the mapped records so legacy fields without direct equivalents remain recoverable.

The WebDemo autosaves to the current browser's local storage and restores after refresh. That is browser-local persistence, not cloud sync or cross-device collaboration. A successful binary round-trip proves the persistence adapter's behavior; it does not prove that a separate desktop host can exit and reopen a workspace.

The workbench includes Casting, Stage, Archive, Campaign, Living Map, Timeline, Book Preview, and Observer Lens views. Book output is deterministic Markdown. GUI.SVG views are projections of live workspace data; they are not an independent source of canon.

## Current host boundary

The Core, persistence adapter, local workbench, FSM_API adapter, and neutral MicroBundle adapter are implemented. The bundle declares its capabilities but is not, by that fact alone, a complete AnyApp presentation surface. Native AnyApp artifact materialization, configuration delivery, surface registration, and the complete create-link-save-exit-reopen flow must be verified against the actual host contract before claiming native end-to-end integration.

Cloud persistence, simultaneous collaboration, LLM-backed drafting, full reader-disclosure policy, direct Renderer-backed narration, and dedicated authoring screens for Plot Twist alternatives/promotion are not provided by the current package. Markdown can be written to a caller-owned stream; choosing a filesystem destination remains a host responsibility.
