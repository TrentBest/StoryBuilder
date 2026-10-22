# CompositionPackageContracts

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

Defines the host-neutral manifest and per-MicroBundle resolved configuration consumed by an AnyApp host. The manifest identifies exact content and selected bundle requirements; the configuration records the Workshop's resolved settings. Neither contract contains a renderer dependency or reruns arbitration.

## UML class card

See [CompositionPackageContracts UML](../../../docs/UML/CompositionPackageContractsUML.md).

## Contract boundary

The records are versioned and identify a package, workspace, content revision, output profile, arbitration evidence, bundle IDs and versions, explicit MicroBundle dependencies, required capabilities, and provider-owned settings. Settings carry a declared value type; this is not a JSON object-graph format. The current implementation defines the contract only. Binary package production, hashing serialized content/configuration, and AnyApp consumption are not yet implemented.
