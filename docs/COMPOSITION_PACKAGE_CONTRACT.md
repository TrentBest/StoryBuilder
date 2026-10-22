# Composition Package Contract

## Purpose

Workshop arbitration must produce a reusable composition package that AnyApp can consume without repeating the expensive decisions that produced it. StoryBuilder's first demanding proof is a real book that can also be inspected through timeline, composition, continuity, and observer views.

The manifest/configuration contracts, validator, binary artifact compiler, and reader are implemented and passed CI. Actual integration with the AnyApp host, provider activation, deployment packaging, and the complete repeated-consumption proof remain future work.

## Three responsibilities

1. **Composition model — what exists.** Stable identities and relationships for an Experience, timelines, events, characters, scenes, beats, manuscript structure, observer knowledge, disclosure intent, and publication editions.
2. **Arbitrated manifest — what is included.** Identifies package/schema, workspace and content revision, selected MicroBundles, required capabilities and versions, output profile, integrity hashes, and the Workshop arbitration record.
3. **Per-MicroBundle configuration — how each selected bundle runs.** Captures resolved options and decisions, including provider selections, output profile, and applicable observer/disclosure policy. Settings have stable keys and explicit value types.

The manifest is not the story database. Configuration is not a replacement for live state. Both identify the exact content revision they apply to.

## Workshop to AnyApp

Authoring model + constraints + available capability catalog
→ Workshop arbitration
→ validated resolved decisions
→ versioned binary manifest + per-MicroBundle configuration + content payload
→ AnyApp validates and consumes the captured package.

Workshop arbitration is the honing step. It resolves choices and records the result. AnyApp validates and consumes that result; ordinary startup must not silently repeat arbitration. An incompatible or incomplete package must fail clearly or use a declared fallback, not improvise a different composition.

## Current contract implementation

- `CompositionPackageManifest` records package/workspace identities, content revision and SHA-256, configuration SHA-256, output profile, arbitration record identity, selected bundle requirements, explicit dependency closure, and required capabilities.
- `CompositionPackageConfiguration` records the same package/workspace/revision/output-profile scope and per-bundle resolved settings.
- `CompositionPackageValidator` checks schema versions, identity and revision alignment, payload hashes, selected bundle configuration, required host bundles/capabilities, versions, and duplicate IDs/settings.
- `CompositionPackageCompiler` serializes workspace content through the existing FSM_Serialization-backed workspace document and emits three separate binary payloads: manifest, configuration, and content. The manifest hashes the exact configuration and content bytes.
- `CompositionPackageReader` validates the manifest, configuration, payload hashes, and host capability availability before restoring content. Failed validation returns diagnostics and no session.
- The manifest and configuration have explicit binary format versions and magic identifiers; settings are typed textual values under an explicit value-type identifier. No JSON or general-purpose object-graph serializer is introduced.
- CI passed for contract validation, binary compilation/reading, round-trip book restoration, tamper rejection, and missing-required-bundle rejection. The manifest contract now also records explicit MicroBundle dependencies. AnyApp host integration and deterministic repeated-consumption proof remain future work.

These contracts are host-neutral and do not introduce a StoryBuilder.Core dependency on FSM_COS, AnyApp, GUI.SVG, or Renderer.

## Capture decisions once; keep live state live

Persist decisions whose inputs and scope are captured by the package: provider selection, compatible capability versions, output profile, resolved composition options, and other deterministic arbitration results.

Do not freeze intentionally dynamic state such as current author edits, participant actions, changing simulation results, or an observer's knowledge after new events occur. Represent those as runtime state or a new package/configuration revision. When an input changes, explicitly reuse a still-valid decision, invalidate and recompute it in the Workshop, or reject the package as incompatible.

## Format and compatibility

- StoryBuilder.Core remains independent of FSM_COS and concrete providers.
- Reuse the established FSM_Serialization binary contract for bounded domain/package payloads; do not introduce an ad hoc general-purpose object-graph serializer.
- Manifest, configuration, and content have explicit versioned binary boundaries, stable identities, declared dependencies, and integrity checks.
- Provide inspectable diagnostics without making a diagnostic format the canonical persisted contract by accident.
- Validate missing capabilities, incompatible versions, invalid references, unknown required fields, and content/configuration mismatch before execution.
- A corrected published book or changed arbitration result creates a new version rather than mutating an immutable artifact.
- No package publication, release, or deployment without explicit owner authorization.

## First complete proof

1. Workspace records produce an explicit manuscript/book composition.
2. Workshop resolves the requested output profile and provider choices.
3. The package compiler serializes workspace content and hashes the exact bytes.
4. Manifest and per-bundle configuration are emitted as versioned artifacts tied to that content hash/revision.
5. AnyApp validates the captured result and produces the same book without rerunning arbitration.
6. A deterministic test compares output across repeated package consumption.
7. Changed inputs produce an explicit new revision or invalidation result.

## Acceptance checks

- Repeated consumption of an unchanged package does not repeat Workshop arbitration.
- Missing required providers are reported before execution; optional providers have declared fallbacks.
- Configuration cannot accidentally apply to different content.
- Runtime observer state is not mistaken for immutable configuration.
- Package version, selected capabilities, and integrity information are inspectable.
- Book preview, timeline map, and observer analysis project the same canonical records.

## Related documents

- [Architecture](ARCHITECTURE.md)
- [Timeline Authoring and Observer Model](TIMELINE_AUTHORING_MODEL.md)
- [Timelines, Gestures, and Multiverse History](TIMELINES_GESTURES_MULTIVERSE.md)
- [Development Status](DEVELOPMENT_STATUS.md)
