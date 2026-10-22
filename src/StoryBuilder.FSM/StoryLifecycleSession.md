# StoryLifecycleSession

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

StoryLifecycleSession is an optional adapter that uses FSM_API to govern deliberate review transitions for story records. The base StoryBuilder.Core package does not depend on FSM_API.

## UML class card

See [StoryLifecycleSession UML](../../docs/UML/StoryLifecycleSessionUML.md) for the class diagram and use case.

## Use case

Create a session for a record, then request Proposed, Accepted, Rejected, Draft, or Deprecated as appropriate. The FSM controls which transitions are possible. A request that has no transition from the current state leaves the state unchanged.

## Boundary

This is a lifecycle demonstration, not yet a universal persistence workflow or an FSM_COS MicroBundle. The host remains responsible for saving the resulting status to the associated domain record and for disposing or scheduling FSM instances appropriately.
