# SceneComposer

## Purpose

`SceneComposer` converts explicit scene-beat fields into deterministic, editable narration text. It does not infer missing actions, motives, outcomes, or sensory detail.

## Operations

- `Create` constructs a scene shell with a title, optional location, and capture mode.
- `AddBeat` appends a recorded beat to a scene.
- `ComposeNarrationBeat` produces the first-pass line for exactly one beat. This supports authoring surfaces that need a one-to-one text-to-beat provenance mapping.
- `ComposeNarration` composes the ordered beat lines and optional location heading for whole-scene preview/copy.

## Provenance boundary

The composer is a text transformation, not a canon acceptance service. The Stage can use `ComposeNarrationBeat` to seed independently editable text blocks, then construct a `NarrationDraftProposal` whose text ranges cite only the corresponding source beat and its linked event IDs. The proposal validator and `StoryBuilderSession.AcceptNarrationDraft` remain responsible for validation, stale-source rejection, and explicit acceptance.

## Example

```csharp
var composer = new SceneComposer();
var line = composer.ComposeNarrationBeat(scene.Beats[0]);
var fullDraft = composer.ComposeNarration(scene);
```

The two methods use the same per-beat formatter, so whole-scene preview and individually mapped blocks do not drift in sentence formatting.
