# TimelineProcessGroupTests

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

These tests exercise the first executable timeline contract: independent FSM_API-backed Process Groups can be inserted into a host-neutral driver, updated, and inspected without mixing their event streams. They also lock down the driver's snapshot-pass behavior and its non-owning lifetime semantics.

## UML class card

See [TimelineProcessGroup UML](../../docs/UML/TimelineProcessGroupUML.md), [StoryProcessGroupDriver UML](../../docs/UML/StoryProcessGroupDriverUML.md), and [TimelineProcessGroupTests UML](../../docs/UML/TimelineProcessGroupTestsUML.md).

## Use cases

- Prove driver updates reach each inserted group.
- Prove runtime observations remain associated with their originating timeline.
- Reject duplicate process-group identity in one driver.
- Prove insertions during an update pass begin on the next pass.
- Prove removals during a pass do not invalidate the stable snapshot already being updated.
- Prove Remove detaches a disposable group without disposing it; the host retains lifetime responsibility.
- Prove disposing a timeline adapter is idempotent, invokes its exit callback, rejects later updates, and leaves another timeline's FSM_API handle operational.
- Enforce workspace-local branch parent references.

## Verification boundary

The test source is committed. Its passing status must be established by an actual local test run or CI result; a commit alone is not test evidence. These tests do not establish production FSM_Foundry scheduling or execution integration.
