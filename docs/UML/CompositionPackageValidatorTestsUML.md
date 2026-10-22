# CompositionPackageValidatorTests UML

```mermaid
classDiagram
    class CompositionPackageValidatorTests {
        +ValidResolvedPackagePassesWithoutArbitration()
        +RejectsHashRevisionAndRequiredCapabilityMismatch()
        +RejectsDuplicateBundleIdsAndUnselectedConfiguration()
    }
    CompositionPackageValidatorTests ..> CompositionPackageValidator : verifies
    CompositionPackageValidatorTests ..> CompositionPackageManifest : constructs
    CompositionPackageValidatorTests ..> CompositionPackageConfiguration : constructs
```

## Use cases

- Confirm the host can validate a package without rerunning Workshop arbitration.
- Exercise stale-content and missing-capability rejection paths.
- Protect the contract from duplicate or unselected bundle configuration.
