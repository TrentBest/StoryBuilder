# CompositionPackageValidatorTests

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

Proves the host-side package validator accepts a compatible resolved package and rejects stale hashes/revisions, missing required capabilities, duplicate bundle identities, and configuration for a bundle not selected in the manifest.

## UML class card

See [CompositionPackageValidatorTests UML](../../docs/UML/CompositionPackageValidatorTestsUML.md).

## Verification boundary

These tests verify the contract validation logic only. They do not yet prove binary manifest/configuration serialization, production of content hashes from the FSM_Serialization payload, or loading by AnyApp.
