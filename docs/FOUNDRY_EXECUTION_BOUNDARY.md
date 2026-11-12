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

## Verified Foundry contract (development source)

The actual repository is [TheSingularityWorkshop.FSM_Foundry](https://github.com/TrentBest/TheSingularityWorkshop.FSM_Foundry), not a repository named simply `FSM_Foundry`. Its current `development` source exposes these distinct contracts:

- `WorkflowDefinition` is a versioned, data-only sequence of steps. Each step names a handler key; it does not contain executable code.
- `WorkflowHandlerRegistry` is host-owned. The host explicitly registers the handlers a workflow may invoke.
- `WorkflowRunner` validates the complete definition before side effects, then invokes registered handlers sequentially. Failure/cancellation stops later steps; results record succeeded, failed, cancelled, or skipped steps.
- `WorkflowExecutionContext` carries bounded JSON inputs/outputs for one run. Context data does not grant capabilities.
- `WorkflowExecutionCoordinator` accepts a definition, starts an accepted run once, exposes immutable in-memory snapshots, supports cooperative cancellation and optional cooperative timeout, and retains runs only for the coordinator lifetime.

This contract is real and useful, but it does **not** currently accept an FSM_COS `RuntimeAssembly`. Foundry's own [Hub integration boundary](https://github.com/TrentBest/TheSingularityWorkshop.FSM_Foundry/blob/development/docs/HUB_INTEGRATION_BOUNDARY.md) explicitly keeps a composed assembly distinct from a sequential workflow definition. Its [issue #23 — Define the FSM_COS RuntimeAssembly execution contract](https://github.com/TrentBest/TheSingularityWorkshop.FSM_Foundry/issues/23) owns the missing typed handoff, execution-provider/scheduling semantics, and end-to-end proof.

The current `Foundry.Workflow.csproj` is a plain .NET 8 library project without package metadata or a published consumer package contract. StoryBuilder must not invent a package reference, vendor a second copy of Foundry, or add a speculative adapter around an interface that Foundry has not defined. The upstream handoff contract must become concrete and consumable first.

The missing execution participant contract is not solely a Foundry implementation detail. The source audits recorded on Foundry issue #23 establish that the current MicroBundleDomain `IMicroBundle` contract is composition-only (`Descriptor`, `Dependencies`, `Load`, and `Arbitrate`); it has no general execute/start/stop/tick/dispose lifecycle. `MicroBundleDescriptor.Providers` contains provider IDs, not executable object references or a resolver. `RuntimeAssembly` retains the loaded bundle objects and composition metadata, but does not infer provider resolution, execution participants, FSM_API process groups, or runtime cleanup. The related upstream discovery work is tracked in [MicroBundleDomain #20](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain/issues/20) (effective capabilities/layers), [MicroBundleDomain #19](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain/issues/19) (scheduling declarations), and [FSM_COS #48](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/issues/48) (preserving scheduling intent through composition). Foundry #23 depends on those contracts becoming sufficiently concrete. This is a cross-repository dependency, not a reason for StoryBuilder to add its own provider registry or scheduler.

FSM_API adds an important isolation constraint: `FSM_API.Interaction.Update(processingGroup)` advances every handle in that named group, while removing a processing group destroys every handle in it. A runtime therefore cannot treat a shared group name as exclusive ownership. Any future Foundry adapter must prove per-runtime participant ownership, tick isolation, cancellation/quiescence, and exact-handle cleanup before StoryBuilder can rely on it.

## Execution overlap requiring containment

`StoryBuilder.Core.Runtime.IStoryProcessGroup` and `StoryProcessGroupDriver` define a host-neutral update abstraction. The driver stores process groups and calls each group's `Update` when its own `Update` method is invoked. `StoryBuilder.FSM.TimelineProcessGroup` adapts a timeline to FSM_API and responds to those calls.

This is a local stepping utility, not FSM_Foundry integration. It must not become the production scheduler or a second execution pipeline. Keep it only where needed for isolated tests, diagnostics, or a deliberately non-authoritative local simulation; label that use clearly. Once Foundry's RuntimeAssembly handoff is defined, replace or adapt this seam only where the production host needs authoritative execution.

Likewise, FSM_API-backed record review and NarratorAgent transitions are authoring/review coordination. They do not own the execution lifecycle of a composed Experience.

## Required handoff contract

The production integration should be implemented at the host/composition boundary, not in StoryBuilder.Core:

1. Host selects a StoryBuilder Experience and supplies its workspace/package identity.
2. StoryBuilder validates the workspace and the authored request against its domain rules and declared capability requirements.
3. FSM_COS composes the requested MicroBundles and returns the actual `RuntimeAssembly`.
4. The host passes that assembly through the typed handoff defined by Foundry's upstream issue #23. The handoff must preserve bundle identity, versions, effective capabilities, and required scheduling declarations; it must not flatten MicroBundle semantics into arbitrary workflow JSON or infer work from display labels.
5. FSM_Foundry owns authoritative scheduling/dispatch, execution lifecycle, cancellation, outcomes, and cleanup according to the contract it implements.
6. StoryBuilder consumes explicit/versioned execution evidence and maps relevant outcomes back into story records only through deliberate domain operations. Runtime observations do not silently become canon.
7. GUI may display execution evidence and submit further authoring commands. A headless host follows the same composition and execution path without GUI dependencies.

The existing sequential `WorkflowDefinition` path may remain useful for explicit host-registered workflows, but it is not a substitute for the missing RuntimeAssembly handoff. Do not force StoryBuilder Experience execution into a sequence of guessed handler keys.

## Verification gates

- Core, persistence, standalone, and headless consumers remain free of GUI and Foundry dependencies.
- FSM_COS composition tests prove bundle resolution only and do not claim workflow execution.
- Foundry issue #23 must define the typed RuntimeAssembly handoff and its lifecycle semantics before StoryBuilder implements a concrete adapter.
- A Foundry integration test composes a StoryBuilder Experience, hands the real assembly to Foundry, observes Foundry-owned run state/evidence, and proves StoryBuilder does not schedule or step the production workflow itself.
- Cancellation, failed validation, missing capabilities, incompatible versions, and rejected requests fail closed.
- Runtime observations remain distinct from accepted story canon.
- GUI-enabled and headless hosts execute the same request contract; GUI is not required for execution.
- Documentation and README claims distinguish implemented composition/authoring from the not-yet-implemented Foundry handoff.

## Current status

**Implemented:** host-neutral story authoring, optional FSM_API authoring/review adapters, neutral MicroBundle capability, FSM_COS composition adapter, bounded semantic authoring commands, and local workspace persistence.

**Verified upstream, not integrated:** Foundry's sequential workflow runner and in-memory run coordinator. Foundry issue #23 owns the missing typed RuntimeAssembly-to-execution handoff.

**Not implemented:** a verified StoryBuilder-to-Foundry RuntimeAssembly adapter, production execution handoff, Foundry-owned run evidence mapped back into StoryBuilder, AnyApp's complete StoryBuilder surface dispatch/refresh path, and GUI/headless end-to-end parity tests.

No Foundry integration should be inferred from an FSM_COS composition success, a sequential WorkflowDefinition test, or an FSM_API state-machine test.
