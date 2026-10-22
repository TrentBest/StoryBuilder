# StoryBuilderAuthoringSurface

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

Projects one live `StoryBuilderSession` into the shared, platform-neutral `GuiNode` model. Its `Root` getter builds a fresh snapshot from current workspace counts, and its `Execute` method routes only the four stable authoring command IDs supported by `StoryBuilderAuthoringController`.

## Example

```csharp
var session = new StoryBuilderSession("My story");
var surface = new StoryBuilderAuthoringSurface(session);

// Render surface.Root with the host's GUI adapter.
// On activation, dispatch the node's onClickCommand through surface.Execute(...).
var result = surface.Execute(StoryBuilderAuthoringCommands.CreateTimeline);
var refreshedRoot = surface.Root;
```

## Interaction boundary

- The surface does not reference WPF or AnyApp.
- Node IDs identify stable presentation elements; labels are not command IDs.
- Command IDs are explicit and unknown commands are rejected without mutation.
- Hosts own rendering, authorization, user input, and re-rendering after a successful command.
- The semantic snapshot is regenerated from the domain session; it is not a second source of truth.

## UML class card

See [StoryBuilderAuthoringSurface UML](../../docs/UML/StoryBuilderAuthoringSurfaceUML.md) for the focused class diagram and use cases.
