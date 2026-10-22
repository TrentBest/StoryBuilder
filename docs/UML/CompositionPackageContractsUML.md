# Composition Package Contracts UML

```mermaid
classDiagram
    class CompositionPackageManifest {
        +int SchemaVersion
        +Guid PackageId
        +Guid WorkspaceId
        +string ContentRevision
        +string ContentSha256
        +string ConfigurationSha256
        +string OutputProfile
        +Guid ArbitrationRecordId
        +List~MicroBundleRequirement~ Bundles
    }
    class MicroBundleRequirement {
        +ulong BundleId
        +string Version
        +bool Required
        +List~string~ RequiredCapabilities
        +List~ulong~ Dependencies
    }
    class CompositionPackageConfiguration {
        +int SchemaVersion
        +Guid PackageId
        +Guid WorkspaceId
        +string ContentRevision
        +string OutputProfile
        +List~MicroBundleRuntimeConfiguration~ Bundles
    }
    class MicroBundleRuntimeConfiguration {
        +ulong BundleId
        +string BundleVersion
        +bool Enabled
        +List~MicroBundleSetting~ Settings
    }
    class MicroBundleSetting {
        +string Key
        +string ValueType
        +string Value
    }
    CompositionPackageManifest --> MicroBundleRequirement : selects
    CompositionPackageConfiguration --> MicroBundleRuntimeConfiguration : configures
    MicroBundleRuntimeConfiguration --> MicroBundleSetting : resolves
```

## Use cases

- Workshop records selected MicroBundles, versions, required capabilities, and the arbitration record in a manifest.
- Workshop stores resolved per-bundle settings in a configuration tied to the same package, workspace, and content revision.
- AnyApp validates the captured result and rejects incompatible packages without repeating arbitration.
- A new content revision or arbitration result creates a new package identity/revision rather than silently reusing stale configuration.
