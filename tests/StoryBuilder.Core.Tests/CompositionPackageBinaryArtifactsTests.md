# CompositionPackageBinaryArtifactsTests

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

Exercises binary package compilation, integrity checking, host capability validation, and workspace restoration using the established FSM_Serialization-backed workspace document.

## UML class card

See [CompositionPackageBinaryArtifactsTests UML](../../docs/UML/CompositionPackageBinaryArtifactsTestsUML.md).

## Acceptance evidence

A compiled package restores the same workspace and authored book text; a modified content payload and a missing required MicroBundle are rejected before a session is returned.
