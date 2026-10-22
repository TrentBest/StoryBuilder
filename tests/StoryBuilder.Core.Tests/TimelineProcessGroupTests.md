# TimelineProcessGroupTests

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

These tests exercise the first executable timeline contract: two independent FSM_API-backed Process Groups can be inserted into a host-neutral driver, updated, and inspected without mixing their event streams.

## UML class card

See [TimelineProcessGroup UML](../../docs/UML/TimelineProcessGroupUML.md) and [StoryProcessGroupDriver UML](../../docs/UML/StoryProcessGroupDriverUML.md).

## Use cases

- Prove driver updates reach each inserted group.
- Prove runtime observations remain associated with their originating timeline.
- Reject duplicate process-group identity in one driver.
- Enforce workspace-local branch parent references.

## Verification boundary

The test source is committed. Its passing status must be established by an actual local test run or CI result; a commit alone is not test evidence.
