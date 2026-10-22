# CompositionPackageBinaryArtifacts

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

Compiles a resolved StoryBuilder workspace into three exact binary payloads: manifest, per-MicroBundle configuration, and workspace content. Uses the existing FSM_Serialization-backed workspace document and bounded binary codec; it does not use JSON or rerun Workshop arbitration.

## UML class card

See [CompositionPackageBinaryArtifacts UML](../../docs/UML/CompositionPackageBinaryArtifactsUML.md).

## Payload contract

- Manifest magic: SBPM; configuration magic: SBPC; each has an explicit binary format version.
- The manifest includes SHA-256 hashes over the exact content and configuration payload bytes.
- The configuration and manifest share package, workspace, content revision, and output-profile identity.
- The reader validates hashes and host capability availability before restoring the workspace.
- Failed validation returns diagnostics and no session. A successful read restores the original workspace ID and records.

The host receives byte arrays so its platform adapter can write or transport them using its own storage facilities. This class does not publish or deploy packages.
