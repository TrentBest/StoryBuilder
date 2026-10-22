# CompositionPackageBinaryArtifacts UML

```mermaid
classDiagram
    class CompositionPackageCompiler {
        +Compile(session, contentRevision, outputProfile, arbitrationRecordId, bundleRequirements, bundleConfigurations) CompositionPackageArtifact
    }
    class CompositionPackageReader {
        +ValidateAndRestore(artifact, availableBundleIds, availableCapabilities) CompositionPackageLoadResult
    }
    class CompositionPackageArtifact {
        +byte[] ManifestPayload
        +byte[] ConfigurationPayload
        +byte[] ContentPayload
    }
    class CompositionPackageLoadResult {
        +bool IsValid
        +IReadOnlyList~string~ Errors
        +StoryBuilderSession Session
    }
    CompositionPackageCompiler ..> StoryWorkspaceBinaryDocument : serializes content
    CompositionPackageCompiler ..> CompositionPackageArtifact : emits
    CompositionPackageReader ..> CompositionPackageValidator : validates
    CompositionPackageReader ..> CompositionPackageArtifact : consumes
```

## Use cases

- Workshop emits a content payload and resolved manifest/configuration after arbitration.
- AnyApp-side adapter validates the package against available bundle IDs and capabilities.
- A valid package restores the same workspace identity; invalid or tampered payloads fail before use.
