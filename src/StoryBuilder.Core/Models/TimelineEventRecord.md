# TimelineEventRecord

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

TimelineEventRecord is an inspectable, stable-identity observation emitted while a timeline Process Group runs. It records which timeline emitted the observation and its sequence within that runtime group.

## UML class card

See [TimelineEventRecord UML](../../../docs/UML/TimelineEventRecordUML.md) for the focused class diagram and use cases.

## Use case

Inspect the event list from a running timeline adapter to verify which Process Group advanced and in what order. Event records are execution evidence; they do not silently promote a proposed world change into accepted canon.

## Boundary

This first runtime slice records start/update observations only. Causal event graphs, observer knowledge, scene/passages links, persistence, and immutable published editions remain subsequent work.
