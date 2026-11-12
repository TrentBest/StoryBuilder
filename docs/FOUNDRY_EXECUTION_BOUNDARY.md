# Foundry Execution and Optional Presentation Boundary

## Ownership contract

StoryBuilder owns the meaning and authoring of stories: workspace records, stable story identities, authoring operations, validation, immutable publication artifacts, and the authoring commands that express a user's intent.

FSM_COS owns MicroBundle composition. A host-facing StoryBuilder composition adapter may request that FSM_COS compose the authoring capability and a named StoryBuilder Experience. Composition proves that the requested bundles were resolved and loaded; it does not mean that a story workflow has been executed.

FSM_Foundry owns execution of composed workflows. StoryBuilder must submit validated authored intent through a narrow host/Foundry contract and consume execution evidence/results. StoryBuilder must not create a competing production scheduler, run loop, or authoritative execution lifecycle.

GUI.Core / GUI.WPF / Renderer / Visualization are optional presentation surfaces. They may project StoryBuilder data and issue explicit authoring commands, but they do not own story semantics, workflow execution, or authoritative run state. A headless host must be able to compose and execute the same authored Experience without a GUI.

## What the current source proves

- `StoryBuilder.Core` is a .NET 8 domain project with no project or package references. Its records and authoring services remain host-neutral.
- `StoryBuilder.MicroBundle` implements the neutral MicroBundleDomain contract and exposes the StoryBuilder authoring capability. Its GUI semantic projection uses GUI.Core, while the domain core does not.
- `StoryBuilder.FSM_COS` references FSM_COS and MicroBundleDomain. `StoryBuilderExperienceHost.Create` creates an Experience bundle, builds a runtime manifest, and calls `FsmCos.Execute` to compose it. It verifies that the Experience and authoring bundles were loaded. This is composition, not Foundry execution.
- `StoryBuilder.FSM` is an optional FSM_API adapter for record review, timeline process groups, and narrator authoring/review orchestration. It is not a reference to FSM_Foundry and does not establish the Foundry handoff contract.
- `StoryBuilderAuthoringSurface` projects a fresh GuiNode snapshot from the workspace. `StoryBuilderAuthoringController` accepts a bounded set of stable command IDs and rejects unknown commands without mutation. AnyApp dispatch and refresh are not completed by the existence of this surface.
- The WebDemo is a local authoring host, not proof of a production Foundry execution host.

## Execution overlap requiring containment

`StoryBuilder.Core.Runtime.IStoryProcessGroup` and `StoryProcessGroupDriver` define a host-neutral update abstraction. The driver stores process groups and calls each group's `Update` when its own `Update` method is invoked. `StoryBuilder.FSM.TimelineProcessGroup` adapts a timeline to FSM_API and responds to those calls.

This is a local stepping utility, not FSM_Foundry integration. It must not become the production scheduler or a second execution pipeline. Before a production host uses it to advance authoritative workflows, replace or adapt this seam to the actual FSM_Foundry contract. Keep it only where needed for isolated tests, diagnostics, or a deliberately non-authoritative local simulation; label that use clearly.

Likewise, FSM_API-backed record review and NarratorAgent transitions are authoring/review coordination. They do not own the execution lifecycle of a composed Experience.

## Required handoff contract

The production integration should be implemented at the host/composition boundary, not in StoryBuilder.Core:

1. Host selects a StoryBuilder Experience and supplies its workspace/package identity.
2. StoryBuilder validates the workspace and the authored request against its domain rules and declared capability requirements.
3. The host translates the validated request into the FSM_Foundry execution contract. The handoff carries stable Experience/workspace/request identities and versioned inputs; it does not pass GUI nodes or trust display labels as command IDs.
4. FSM_Foundry owns scheduling, stepping, cancellation, execution state, and run evidence.
5. StoryBuilder consumes immutable or explicitly versioned execution evidence and maps relevant outcomes back into story records only through deliberate domain operations. Runtime observations do not silently become canon.
6. A GUI host may display execution evidence and submit further authoring commands. A headless host follows the same composition and execution path without GUI dependencies.

No Foundry package/API is currently referenced by StoryBuilder source, and no real Foundry handoff or end-to-end execution test is present. Do not invent a placeholder interface and call it integration. First verify the current FSM_Foundry contract, then implement the adapter in the appropriate host/composition project.

## Verification gates

- Core, persistence, standalone, and headless consumers remain free of GUI and Foundry dependencies.
- FSM_COS composition tests prove bundle resolution only and do not claim workflow execution.
- A Foundry integration test composes a StoryBuilder Experience, submits a validated request, observes Foundry-owned run state/evidence, and proves StoryBuilder does not schedule or step the production workflow itself.
- Cancellation, failed validation, missing capabilities, incompatible versions, and rejected requests fail closed.
- Runtime observations remain distinct from accepted story canon.
- GUI-enabled and headless hosts execute the same request contract; GUI is not required for execution.
- Documentation and README claims distinguish implemented composition/authoring from the not-yet-implemented Foundry handoff.

## Current status

**Implemented:** host-neutral story authoring, optional FSM_API authoring/review adapters, neutral MicroBundle capability, FSM_COS composition adapter, bounded semantic authoring commands, and local workspace persistence.

**Not implemented:** a verified FSM_Foundry adapter, production execution handoff, Foundry-owned run evidence mapped back into StoryBuilder, AnyApp's complete StoryBuilder surface dispatch/refresh path, and GUI/headless end-to-end parity tests.

No Foundry integration should be inferred from an FSM_COS composition success or an FSM_API state-machine test.
