# ObserverKnowledgeProjection

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

Represents the accepted knowledge an observer can legitimately use at a chosen event anchor. It intentionally omits world-event titles, summaries, and causal edges; a portrayal provider receives the observer's accepted notes and permitted narration modes, not the unrestricted world-event record.

## UML class card

See [ObserverKnowledgeProjection UML](../../../docs/UML/ObserverKnowledgeProjectionUML.md).

## Safety boundary

Only accepted knowledge records that pass ObserverKnowledgeEvaluator are included. Direct fact, attributed report, inference, and belief remain distinct modes. Proposed, unavailable, forgotten, and unsupported knowledge is omitted. This is an authoring/portrayal constraint, not a security boundary around the entire workspace.
