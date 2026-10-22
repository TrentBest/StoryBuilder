# CompositionPackageBinaryArtifactsTests UML

```mermaid
classDiagram
    class CompositionPackageBinaryArtifactsTests {
        +CompiledPackageValidatesAndRestoresTheExactWorkspace()
        +TamperedContentIsRejectedBeforeWorkspaceRestore()
        +MissingRequiredBundleIsRejectedBeforeRestore()
        +RepeatedPackageConsumptionProducesTheSameBookWithoutReArbitration()
    }
    CompositionPackageBinaryArtifactsTests ..> CompositionPackageCompiler : verifies
    CompositionPackageBinaryArtifactsTests ..> CompositionPackageReader : verifies
    CompositionPackageBinaryArtifactsTests ..> BookProductRenderer : compares repeated output
```

## Use cases

- Prove round-trip of a compiled book package.
- Reject content tampering before restoring authoring state.
- Reject unavailable required host capabilities without rerunning arbitration.
- Consume the same package twice and compare rendered book content and event-reference metadata.
