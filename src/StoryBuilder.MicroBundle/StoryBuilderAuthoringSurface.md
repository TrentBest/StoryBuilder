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

## Shared GUI.Core dependency

`StoryBuilder.MicroBundle.csproj` exposes the MSBuild property `WorkshopGuiCoreProject`. By default, it resolves to this repository's pinned `external/TheSingularityWorkshop.GUI` checkout. A source-mode host that already builds GUI.Core should override the property with the absolute path to that same `TheSingularityWorkshop.GUI.Core.csproj` file. This makes both projects reference one GUI.Core project identity rather than compiling parallel copies of the same semantic types.

This property only solves the project-reference identity; the host must still materialize StoryBuilder as a source dependency and pass the shared path consistently. AnyApp wiring and desktop behavior are not yet verified.

## UML class card

See [StoryBuilderAuthoringSurface UML](../../docs/UML/StoryBuilderAuthoringSurfaceUML.md) for the focused class diagram and use cases.
