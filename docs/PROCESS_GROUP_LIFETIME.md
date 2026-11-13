# Process Group Lifetime and Ownership

## Scope

StoryBuilder currently has a small host-neutral stepping seam:

- `IStoryProcessGroup` describes a group that can be advanced.
- `StoryProcessGroupDriver` stores non-owning references and calls `Update` on a stable snapshot when its host invokes `Update`.
- `TimelineProcessGroup` is an optional FSM_API-backed implementation that owns one FSM_API instance/handle.

This is a local stepping utility and a deterministic test seam. It is not the production FSM_Foundry execution contract.

## Ownership rules

1. The component that creates a process group owns its lifetime unless an explicit ownership-transfer contract is introduced.
2. Inserting a group into StoryProcessGroupDriver does not transfer ownership.
3. Removing a group only detaches it from future driver snapshots. It does not call Dispose and does not cancel an update already captured in the current snapshot.
4. The owning host must dispose a removed disposable group when it is permanently retired.
5. TimelineProcessGroup owns the FSM_API handle it creates. Its host must call Dispose; disposal invalidates its context, destroys that exact handle, and allows FSM_API to invoke the current state's exit callback. Repeated disposal is supported.
6. A driver must not dispose arbitrary groups because it has no ownership-transfer contract and IStoryProcessGroup does not require IDisposable.

## Update-pass semantics

StoryProcessGroupDriver takes a snapshot of its currently inserted groups at the start of Update. Each group in that snapshot receives one Update call. A group inserted during the pass participates in the next pass. A group removed during the pass may still receive its already-snapshotted call.

The driver does not establish thread safety, prevent reentrant Update calls, schedule a clock, or provide cancellation/quiescence. The host must serialize access and must not treat removal as proof that an in-flight update has completed.

## FSM_API isolation

TimelineProcessGroup retains the canonical timeline ID for story attribution but assigns a fresh runtime-instance ID to its FSM_API processing group. This distinction is required because FSM_API updates every handle in a named group: two adapters may legitimately project the same canonical definition, and must still advance and dispose independently. The host must still dispose each adapter it owns. It must not remove a shared FSM_API processing group as a cleanup shortcut: group removal has broader handle-destruction semantics than disposing the adapter's exact handle.

## Foundry boundary

The current driver is not a StoryBuilder-to-Foundry adapter. Production execution must wait for the typed FSM_COS RuntimeAssembly handoff and lifecycle contract tracked upstream in [FSM_Foundry issue #23](https://github.com/TrentBest/TheSingularityWorkshop.FSM_Foundry/issues/23), including the related MicroBundleDomain capability/scheduling and FSM_COS scheduling-intent contracts. Do not infer Foundry execution, cancellation, or run evidence from this local driver.

## Verification scenarios

- Removing a group prevents it from appearing in subsequent driver update snapshots.
- Removing a group does not dispose it; the owning host can still inspect or explicitly dispose it.
- Disposing a TimelineProcessGroup destroys its owned FSM_API instance, invokes its current state's exit callback, and later Update calls fail with ObjectDisposedException.
- Two timeline adapters advance independently and retain timeline-scoped diagnostic observations, including when both adapters project the same canonical TimelineDefinition.
- Hosts serialize driver mutation and update calls; concurrent scheduling is outside this utility's contract.
