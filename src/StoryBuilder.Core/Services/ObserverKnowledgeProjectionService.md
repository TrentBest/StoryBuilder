# ObserverKnowledgeProjectionService

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

Builds a downstream portrayal context from accepted observer-knowledge records and the existing conservative evaluator. The batch path indexes records by event and computes causal ancestry once, rather than rescanning the entire workspace for each event. The batch path indexes records by event and computes causal ancestry once, rather than rescanning the entire workspace for each event. It does not copy unrestricted world-event text into the observer projection.

## UML class card

See [ObserverKnowledgeProjection UML](../../../docs/UML/ObserverKnowledgeProjectionUML.md).

## Behavior

- Requires a known observer character and, when supplied, a valid narration event anchor.
- Evaluates direct fact, attributed report, inference, and belief independently.
- Includes only accepted records available at the chosen causal anchor.
- Returns event identities plus the exact accepted notes, attribution, source, confidence, and allowed modes.
- Omits blocked, unresolved, proposed, forgotten, and otherwise unavailable events.

Renderer may use this context alongside its own observer-relative representation policy. Renderer detail selection does not grant narrative knowledge, and this projection does not select a graphics representation.
