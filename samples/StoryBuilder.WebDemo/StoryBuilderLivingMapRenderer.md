# StoryBuilderLivingMapRenderer

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

This renderer maps actual StoryBuilderSession records into the platform-neutral GUI tree and asks GUI.SVG to render the immutable tree. Character names, roles, traits, review status, workspace counts, and top vocabulary usage come from current domain data.

## UML and use case

See [StoryBuilderWebDemo UML](../../docs/UML/StoryBuilderWebDemoUML.md). The Living Map page calls Render after session changes and refreshes the SVG manifestation.

## Boundary

The SVG is a compact view of the current workspace, not an exhaustive world atlas or a source of truth. The domain remains authoritative; the SVG is regenerated when records change.
