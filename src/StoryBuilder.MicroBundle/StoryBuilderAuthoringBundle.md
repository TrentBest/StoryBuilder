# StoryBuilderAuthoringBundle

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

StoryBuilderAuthoringBundle exposes StoryBuilder as a MicroBundle capability so a host can compose it through FSM_COS. It depends on the neutral MicroBundleDomain contract, not on FSM_COS itself. The host discovers its declared workspace, Casting, Stage, vocabulary, history, and campaign providers and can request isolated StoryBuilderSession instances.

## UML class card

See [StoryBuilderAuthoringBundle UML](../../docs/UML/StoryBuilderAuthoringBundleUML.md) for the class diagram and use case.

## Use case

A host's catalog resolves the bundle by stable ID and version. FSM_COS loads and arbitrates the bundle. The host retrieves the composed bundle and calls CreateSession for an individual authoring workspace.

## Boundary

This adapter declares capabilities and creates sessions; it does not yet supply a UI, persistence provider, or campaign runtime. The MicroBundle instance is intentionally stateless so different runtime assemblies do not share one user's workspace.
