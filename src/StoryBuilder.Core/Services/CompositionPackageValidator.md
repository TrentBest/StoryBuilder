# CompositionPackageValidator

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

Validates an already-resolved package for schema compatibility, package/content identity, SHA-256 integrity, selected bundle configuration, and host capabilities. It does not arbitrate or substitute provider choices.

## UML class card

See [CompositionPackageValidator UML](../../../docs/UML/CompositionPackageValidatorUML.md).

## Failure behavior

Validation returns explicit diagnostics for mismatched revisions, invalid hashes, missing required bundles/capabilities, duplicate IDs/settings, missing required configuration, disabled required bundles, and version mismatch. Optional unavailable bundles are not promoted to required. This is a host-neutral validation seam; binary artifact production and the AnyApp loader remain future work.
