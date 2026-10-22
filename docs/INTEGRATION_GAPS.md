# AnyApp Integration Gap Register

**Status:** verified integration boundary; no host changes made.

## What exists today

AnyApp launches serialized `ExperienceManifest` records and composes them through FSM_COS. The current native host catalog is compiled into AnyApp and resolves the Moniker and Forge MicroBundles. `MainWindow.RenderRuntime` explicitly manifests those two known bundle types.

## Consequence for StoryBuilder

StoryBuilder cannot yet be launched merely by adding a MicroBundle in this repository. AnyApp needs a deliberate extension point for registered Experience surfaces, or a host-side StoryBuilder adapter and catalog registration. That integration should be implemented and tested in AnyApp against its actual GUI.WPF/FSM_COS contracts. This repo must not duplicate the host, pretend that an unregistered bundle can launch, or introduce a new direct dependency on WPF.

## First slice delivered here

The host-neutral Core project provides:
- a project identity and manuscript/premise fields;
- stable GUID identity for timeline events;
- optional event-to-passage and observer links;
- chronology ordering;
- local project save/reopen with a temporary-file replacement strategy and schema-version check.

## Next integration steps

1. Add tests for project creation, passage linking, chronology ordering, and persistence round-trip.
2. Introduce a real semantic GUI surface for manuscript and timeline editing using existing GUI builders.
3. Extend AnyApp's host registration/manifest path so StoryBuilder can be selected and rendered without hardcoding its entire domain into AnyApp.
4. Prove create/edit/save/close/reopen in a Windows desktop run.
5. Add observer-constrained event views and provenance states after the first round-trip is reliable.

No NuGet package has been published. No branch has been deleted.
