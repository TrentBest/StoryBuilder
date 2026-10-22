# StoryBuilderObserverProjectionRenderer

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

Renders an author-inspectable GUI.SVG view of an ObserverKnowledgeProjection. Only events present in the accepted-knowledge projection receive cards; the view shows each permitted claim mode and the accepted note that supports it.

## UML class card

See [StoryBuilderObserverProjectionRenderer UML](../../docs/UML/StoryBuilderObserverProjectionRendererUML.md).

## Use cases

- Choose an observer and an event anchor in the Observer Lens.
- Build the accepted-knowledge projection through NarratorAgent's observer FSM.
- Inspect known event identities, permitted framing modes, and accepted notes.
- Confirm unknown or causally unavailable events are not drawn in this view.

The SVG is an author-facing diagnostic projection. Its event labels are resolved locally for navigation; the projection object supplied to downstream providers deliberately omits unrestricted world-event titles and summaries.
