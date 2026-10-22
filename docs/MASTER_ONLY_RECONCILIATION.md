# Master-Only Reconciliation Ledger

**Snapshot:** 22 October 2026  
**Repository:** [TrentBest/StoryBuilder](https://github.com/TrentBest/StoryBuilder)  
**Purpose:** preserve the work that exists only on `master` before deciding how to reconcile the diverged histories.

## Verified graph facts

The GitHub compare endpoint reports that `master` is **13 commits ahead of the shared merge base** `3eef29a7b6c8b016db02c6d1acd44170585211c5`. The live comparison of `master` against that merge base lists the following 10 changed paths. The latest direct comparison of `master` (base) against `development` (head), checked 22 October 2026, reports **452 commits ahead and 13 behind**, status `diverged`; the merge base remains `3eef29a7b6c8b016db02c6d1acd44170585211c5`. Counts are a dated snapshot and change as commits land. PR [#1](https://github.com/TrentBest/StoryBuilder/pull/1) is still open and GitHub currently reports it as not mergeable. The reconciliation-ledger update at [`74597dd`](https://github.com/TrentBest/StoryBuilder/commit/74597dd3320ef1e5d2d41e8683c96dadf562024f) passed the `dotnet` workflow [run 39851609177](https://github.com/TrentBest/StoryBuilder/actions/runs/39851609177) on 22 October 2026; this validates the current development test/build workflow, not the unimplemented master-JSON import.

This ledger is a file/behavior inventory, not a claim that the histories are safe to merge automatically. No merge or deletion is authorized by this document.

## Master-only file inventory and disposition

| Path | Observed purpose / behavior | Reconciliation decision |
|---|---|---|
| `.github/workflows/core-tests.yml` | Dedicated Core workflow; compare reports 24 added lines. | **Preserve the coverage intent.** Compare its triggers, pinned action SHAs, SDK setup, and test commands with the active `development` workflow. Merge workflow responsibilities only after checking for duplicate or contradictory CI. |
| `src/StoryBuilder.Core/StoryBuilder.Core.csproj` | Separate Core project definition; compare reports 11 added lines. | **Preserve the host-neutral boundary.** Reconcile target framework, package references, nullable/compiler settings, and project identity with the current domain model. Avoid creating a second competing Core assembly. |
| `src/StoryBuilder.Core/StoryProject.cs` | Stable project GUID and schema version; title, premise, manuscript; passage anchors; timeline events with stable GUIDs, optional passage/observer links, event creation, and deterministic chronology ordering. | **Preserve the domain behaviors, not necessarily the exact type shape.** Map these concepts into the current authoring model, identify any duplicate event/passage types, and define explicit compatibility/migration behavior before replacing or adapting types. |
| `src/StoryBuilder.Core/LocalProjectStore.cs` | JSON local save/load, temporary-file write then replacement, cancellation support, and schema-version validation. | **Preserve local-first persistence guarantees.** Compare against current binary workspace format v8 and command-level persistence. Choose a canonical storage format and provide explicit import/export or migration if both formats are needed; do not silently fork the source of truth. Review crash durability and replacement semantics on supported platforms. |
| `tests/StoryBuilder.Core.Tests/StoryBuilder.Core.Tests.csproj` | Dedicated Core test project; compare reports 20 added lines. | **Preserve tests in the final test graph.** Avoid duplicate project names or test discovery gaps. |
| `tests/StoryBuilder.Core.Tests/StoryProjectTests.cs` | Tests reusable passage identity, rejects links to text outside the manuscript, orders dated events before undated events, and round-trips project/event/passage IDs through local storage. | **Port or adapt these assertions** to the chosen canonical domain and persistence model. Add coverage for duplicate passage text, malformed/unsupported schema versions, cancellation/failure cleanup, and atomic replacement where applicable. |
| `docs/DEPLOYMENT_DECISION.md` | Declares AnyApp as the first local Windows desktop host and says WebApp hosting is not a prerequisite; explicitly labels the architecture as a target, not an implemented runtime. | **Retain the desktop-first decision if still current.** Align it with actual AnyApp integration evidence and avoid describing target architecture as delivered functionality. |
| `docs/INTEGRATION_GAPS.md` | Records that AnyApp currently has a fixed native host catalog and that StoryBuilder cannot launch merely by adding a MicroBundle; identifies host registration/semantic surface and desktop round-trip as gaps. | **Keep as an evidence-driven gap register**, updating it against the current AnyApp source and CI. This is a key guard against overstating end-to-end integration. |
| `README.md` | Revises product framing around both Experience/Tool design records and narrative composition, links the desktop-first decision, and describes Workshop package boundaries and later visualization goals. | **Reconcile prose with the current README rather than overwrite wholesale.** Preserve the product distinction between implementation facts and fictional canon; ensure all links exist and all claims match shipped code. |
| `docs/Gallery.md` | Compare reports a 20-addition/10-deletion documentation change. | **Preserve useful recipes, but verify every demonstration.** Label implemented behavior, planned work, and screenshots/diagrams accurately. |

The compare endpoint reported the following aggregate change counts for the 10 paths: 404 additions and 17 deletions. This is the endpoint's aggregate for the files listed above; it is not a test result.

## Canonical model mapping proposal (not yet implemented)

The two models are not equivalent in scope. Treat `StoryProject` as a compact local-first project envelope and `StoryWorkspace` as the richer authoring/canon workspace; do not flatten the richer records just to make a merge easy.

| Master `StoryProject` field/behavior | Development-side candidate | Required decision / invariant |
|---|---|---|
| `ProjectId` | `StoryWorkspace.Id` (verify exact workspace identity semantics before coding) | Preserve the original GUID through conversion; never generate a replacement during import/export. |
| `Title` | `StoryWorkspace.Name` or a distinct project title | Confirm whether the workspace name and story/product title mean the same thing. If not, add/retain a distinct field instead of silently conflating them. |
| `Premise` | `StoryWorkspace.Premises` / `StoryPremise` (`Title`, `Vision`, optional genre/scale/target) | The master field is a single string while the development record is structured. Define whether the string becomes `Vision`, a title plus vision, or an import note; do not invent a premise identity when none exists. |
| `Manuscript` | `StoryWorkspace.Manuscripts` / `ManuscriptEdition` and its ordered sections | The master field is a single body string, not a structured edition. Import it losslessly into a documented default edition/section; preserve exact text and define deterministic reconstruction. Do not assume one master passage anchor equals a whole section. |
| `PassageAnchor.Id` / `Text` | Candidate manuscript section ID/body, or an explicit anchor record if section semantics differ | Preserve the GUID and exact text. A passage anchor may be a substring rather than a section, so inspect the master invariants and represent it explicitly if section mapping loses meaning. Define repeated-text and post-edit behavior. |
| `TimelineEvent.Id` | `StoryWorkspace.Events` / `StoryEvent.Id` | Preserve event GUIDs exactly. Map title to `Title` and description to `Summary` only after checking field semantics; do not map links by title, list position, or regenerated IDs. |
| `OccurredAt` | Closest candidates are `StoryEvent.InWorldDateOrPeriod`, `TimelineId`, and `PlacementKind` | No direct `OccurredAt` timestamp field is visible on `StoryEvent`. Preserve the original timestamp exactly in a defined extension/compatibility field or record; do not silently convert it to an approximate fictional date/placement. |
| `PassageId` | Manuscript section's `EventIds` (inverse relationship) | On conversion, preserve the relationship in both directions or derive one direction from a single canonical edge list; validate dangling IDs. |
| `ObserverId` | `ObserverKnowledgeRecord.ObserverId` (with its `EventId`, knowledge kind/status and acquisition anchor) | Observer identity is distinct from provenance: `SourceRecord` is not a substitute. Preserve the ID even if the observer is not a `CharacterProfile`; document validation or external-reference policy. |
| `SchemaVersion` | `StoryWorkspaceBinaryDocument` format version (currently v8) | Keep file-format version separate from domain schema version. Provide explicit versioned import/migration and reject unsupported versions without destructive fallback. |
| JSON atomic-save guarantees | Binary document + AnyApp store | Preserve local-first save semantics and stable IDs. Test replacement/failure cleanup and real process restart/reopen on the supported host. |

This mapping remains a proposal, not proof that an adapter or migration exists. Source inspection confirms that `StoryWorkspace` keeps premises, manuscript editions, timelines, planning points, world events, observer-knowledge records, and source/provenance records in distinct collections. `StoryWorkspaceBinaryDocument` uses `SBWS` format v8 and explicitly reads versions 1–8; its tests cover stable workspace/manuscript/event/observer IDs, section-to-event links, synonym mappings, a v1 migration, unknown-version rejection, and truncated payload rejection. These tests validate the development-side binary format, not conversion from master’s `StoryProject` JSON schema. Before implementation, add explicit import/conversion tests for identity, links, timestamp preservation, ambiguous passage anchors, unsupported JSON schema versions, and deterministic round-trips. The first release should have one canonical mutable model and one canonical persistence route; optional JSON import/export may be a boundary adapter if it earns its maintenance cost.

### Import implementation sequence and acceptance contract

The safest first implementation is a one-way, explicit legacy JSON importer into `StoryWorkspace`, not a second live `StoryProject` authoring model. Keep the legacy JSON DTO/schema isolated at the import boundary and let the existing binary workspace document remain the canonical save format. Do not add an export path until a concrete compatibility need requires it.

1. **Parse and validate:** deserialize the documented master schema into an isolated legacy DTO; reject null payloads, unsupported `SchemaVersion`, malformed GUIDs/fields, and invalid duplicate IDs with actionable errors. Never fall back to an empty workspace after a parse or validation failure.
2. **Build records in dependency order:** create workspace identity/name; map the single premise to one clearly documented `StoryPremise` representation without inventing an identity; create a manuscript edition and a lossless default section for the full manuscript; import timeline events with their original GUIDs and titles/descriptions; then resolve passage anchors and observer links after all target records exist.
3. **Preserve information that does not fit:** retain each original `OccurredAt` timestamp exactly in an explicit compatibility extension/record until the domain supports an equivalent field. A passage anchor remains a separate stable identity and exact substring unless it can be represented as a section without changing its semantics. Preserve `ObserverId` even when no corresponding character record exists.
4. **Resolve links explicitly:** map `PassageId` to a stable manuscript section/event link only when the mapping is semantically exact. If a substring anchor cannot be a section, use an explicit anchor model rather than attaching the event to the entire manuscript section. Reject or report dangling links; do not repair them by matching titles or list positions.
5. **Persist only after successful conversion:** validate all IDs and relationships in memory, then save through the existing binary store. A failed import must not overwrite an existing project file. Record the importer/schema version in the migration boundary so later changes are diagnosable.

Acceptance tests must cover: (a) a representative master JSON fixture; (b) exact preservation of project/event/passage GUIDs and manuscript text; (c) two events sharing one passage anchor; (d) a passage that is only a substring of the manuscript; (e) dated and undated events retaining their exact timestamp semantics; (f) observer ID retained without a matching character; (g) unsupported schema and malformed input rejected; (h) unresolved IDs reported without silent reassignment; (i) conversion → binary save → binary load retains every mapped field and relationship; and (j) failed conversion leaves an existing destination byte-for-byte unchanged. The master tests currently cover passage reuse, out-of-manuscript rejection, chronology ordering, and JSON store round-trip; the development binary tests do not yet exercise this importer.

**Status of this sequence:** design contract only. No importer or conversion tests are claimed to exist yet.

## Required behavior-level reconciliation

1. **One canonical story model.** Decide whether the stable-ID `StoryProject` / `TimelineEvent` / `PassageAnchor` model is merged into the current domain, adapted through a mapper, or imported. Document the mapping for IDs, passage links, observer IDs, event timestamps, provenance, and schema version.
2. **One canonical persistence path.** The `master` implementation serializes `StoryProject` to JSON, while `development` has a binary workspace format v8 and command-level save/reopen behavior. Do not maintain two unrelated mutable sources of truth. Specify conversion and round-trip guarantees.
3. **Retain local-first desktop intent without claiming completion.** AnyApp is the intended first host, but a real native-host `create → link → save → restart → reopen` test remains a release gate. Existing component-level or command-level tests are not equivalent to this host smoke test.
4. **Merge CI by responsibility.** Compare `.github/workflows/core-tests.yml` with `.github/workflows/dotnet.yml`; ensure the final workflows test the actual final head, pin third-party actions to immutable SHAs, and do not introduce publishing or deployment side effects.
5. **Resolve docs against source and evidence.** Keep the integration gap register and deployment decision, but update them only when source or a passing workflow supports the change. Link the reconciliation ledger from the main branch ledger.
6. **Validate the combined result.** Require clean restore/build/test for every retained project, inspect the full resolved diff, and run the native AnyApp smoke test on Windows. Only then is a merge decision appropriate.

## Release and branch safety

- Permanent branches remain `master` and `development`; branch cleanup remains the repository owner's manual decision.
- PR [#1](https://github.com/TrentBest/StoryBuilder/pull/1) remains a review surface, not permission to merge.
- No NuGet publish, GitHub release, deployment, or branch deletion is authorized by this ledger.
- StoryBuilder readiness remains **70% (planning estimate, not test-derived)** until a fresh review establishes a better-supported figure.
