# StoryBuilderTimelineRenderer

**User moniker:** Living timeline map  
**Purpose:** Project the workspace's canonical timeline definitions and declared parent/branch relationships into an inspectable SVG view using GUI.SVG.

The renderer is a presentation adapter. It reads the current StoryBuilderSession and does not own or persist timeline records. Branch connectors mean “declared parent timeline”; they do not imply a divergence event, world-time scale, causal edge, or observer knowledge that the current domain has not recorded.

## UML class card

```text
StoryBuilderTimelineRenderer
  - SvgGuiRenderer renderer
  + Render(StoryBuilderSession) : string
  - Depth(TimelineDefinition, HashSet<Guid>) : int
  - Shorten(string, int) : string
```

## Use cases

- The Timelines page refreshes a living SVG when a timeline is created.
- An author visually inspects root timelines and their named branches.
- A host can reuse the renderer without moving canonical data into SVG.

See [Timeline map use cases](../../docs/UML/StoryBuilderTimelineRendererUML.md).
