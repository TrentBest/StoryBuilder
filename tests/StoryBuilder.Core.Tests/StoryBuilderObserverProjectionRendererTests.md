# StoryBuilderObserverProjectionRendererTests

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

Verifies that the observer-lens SVG is generated from the observer-safe projection, shows accepted notes, and omits unknown event titles and unrestricted world summaries.

## UML class card

See [StoryBuilderObserverProjectionRendererTests UML](../../docs/UML/StoryBuilderObserverProjectionRendererTestsUML.md).

## Acceptance evidence

The test uses canonical workspace records and asserts the generated SVG includes only the accepted event and its permitted knowledge note.
