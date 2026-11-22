# StoryBuilderAuthoringControllerTests

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

Verifies that supported host command IDs invoke existing StoryBuilder session operations, return created record identities, use a predictable default for blank names, and reject unknown commands without changing workspace state.

## UML

See [StoryBuilderAuthoringController UML](../../docs/UML/StoryBuilderAuthoringControllerUML.md).

## Verification

The test source is committed; passing status must be established by a CI run for the commit containing these tests.


## Publication authoring

The controller can create a stable publication outlet and issue an immutable in-world edition from a typed draft. It computes the content hash, preserves provenance references, and does not grant observer knowledge or imply delivery. Invalid outlet/edition lineage is rejected by the workspace domain boundary.
