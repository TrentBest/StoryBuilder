# StoryBuilderAuthoringController

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

Provides a host-neutral, explicit command boundary between desktop controls and a single StoryBuilder authoring session. Stable command IDs create timelines, characters, scenes, manuscripts, timeline events, manuscript passages, and publication outlets through existing StoryBuilder domain operations. A typed `StoryBuilderPublicationEditionDraft` issues an immutable in-world artifact with explicit outlet identity, content, authored publication time, audience/access scope, and provenance references; the controller computes its SHA-256 hash. A bounded link command connects the latest event to the latest passage by canonical event ID.

## UML class card

See [StoryBuilderAuthoringController UML](../../docs/UML/StoryBuilderAuthoringControllerUML.md).

## Use case

A host binds a control to a documented command ID, calls Execute(commandId, optionalName), then refreshes its view from the session. Successful results include the created record identity. Unsupported command IDs return a rejected result and do not mutate the workspace.

## Safety and boundary

The controller does not use reflection, arbitrary method names, UI-framework types, or persistence. It is not itself a GUI renderer. Hosts must explicitly bind their controls to supported command IDs and display the returned outcome; rendering a button without such a binding is not a functional interaction.
