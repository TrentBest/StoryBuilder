# Master-Only Reconciliation Ledger

**Snapshot:** 22 October 2026  
**Repository:** [TrentBest/StoryBuilder](https://github.com/TrentBest/StoryBuilder)  
**Purpose:** preserve the work that exists only on `master` before deciding how to reconcile the diverged histories.

## Verified graph facts

The GitHub compare endpoint reports that `master` is **13 commits ahead of the shared merge base** `3eef29a7b6c8b016db02c6d1acd44170585211c5`. The live comparison of `master` against that merge base lists the following 10 changed paths. The broader branch comparison has also reported `development` ahead of `master` and 13 commits behind; exact ahead counts on `development` change when commits are added and should be treated as dated snapshots, not invariants.

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

The compare endpoint reported the following aggregate change counts for the 10 paths: 334 additions and 17 deletions. This is the endpoint's aggregate for the files listed above; it is not a test result.

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
