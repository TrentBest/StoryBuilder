# Branch Reconciliation

**Policy:** permanent branches are exactly master and development. The repository owner performs manual deletion of surplus branches; no branch is deleted by this work.

## Current verified snapshot — 9 November 2026

The branch listing contains exactly:

| Branch | Head | Disposition |
|---|---|---|
| development | 661775060b7b1546afa8cdfbedf97e43bea0fb8c | Active integration branch; latest authored documentation commit is ead43f633d2ab86753caad31f9d5355468ed3b47. |
| master | 6f4100d04faed5d36c4e946ad4af4c6cc5a15d91 | Stable/default target; not updated by this work. |

PR [#1](https://github.com/TrentBest/StoryBuilder/pull/1) remains open and unmerged.

### Live graph comparison

The GitHub compare endpoint reports:
- development is **866 commits ahead** of master.
- development is **21 commits behind** master.
- Status: **diverged**.
- Shared merge base: 3eef29a7b6c8b016db02c6d1acd44170585211c5.

The 21 master-only commits were enumerated directly from the merge-base-to-master comparison. They are genuine master-side history, not merely a stale PR counter.

### Master-only commit inventory

The 21 commits fall into four coherent groups:

1. **Host decision (3 commits):** choose AnyApp Windows desktop as StoryBuilder's first runnable host, and align the proof target with it.
2. **Core domain and local persistence (7 commits):** add the host-neutral StoryBuilder.Core project, StoryProject, stable GUID identities for projects/events/passages, passage-linked timeline events, chronology ordering, schema-versioned JSON persistence, and atomic temporary-file replacement. Add tests for identity, passage linking, chronology, and persistence round-trip.
3. **Integration and CI evidence (3 commits):** document the verified AnyApp/FSM_COS/GUI boundary and its missing host registration extension point; add dedicated Core CI and pin its actions to immutable revisions.
4. **Presentation and branch policy (8 commits):** update the Gallery, add Sith badge assets/workflow/README evidence, and refresh policy evidence commits.

Master-only paths:
- .github/workflows/core-tests.yml, .github/workflows/sith-policy.yml
- src/StoryBuilder.Core/StoryProject.cs, LocalProjectStore.cs, and the Core project file
- tests/StoryBuilder.Core.Tests/*
- docs/DEPLOYMENT_DECISION.md, docs/INTEGRATION_GAPS.md, docs/Gallery.md
- README.md, assets/sith-policy.json, and assets/sith-policy.svg

### Architectural reconciliation

The master-side deployment decision is explicit: **AnyApp desktop is the first host; a hosted WebApp is not a prerequisite.** It also states that this is a target architecture, not evidence that the StoryBuilder runtime or authoring UI already exists. The integration-gap register says AnyApp currently launches known MicroBundles and does not yet expose a general registered Experience surface for StoryBuilder.

The current development branch contains a richer StoryBuilder domain/workspace, binary host-workspace persistence, FSM_API/FSM_COS composition adapters, MicroBundle integration, and a Blazor WebDemo authoring workbench. These are useful implementation assets, but the WebDemo must not silently become the product's required first host. The AnyApp-first decision and the current Blazor workbench can coexist: keep the workbench as a development/demo surface while the production vertical slice is proven through AnyApp.

The master-side StoryProject model is not safe to merge by copying its files over the richer development model. Its stable project/event/passage IDs, event timestamps, observer IDs, chronology semantics, and JSON schema-version behavior need an explicit compatibility decision. The right first step is to map those semantics to the canonical workspace and determine whether a one-way legacy JSON importer is warranted. Do not create two competing live authoring models or silently discard old project files.

The AnyApp gap is also concrete: AnyApp's current native catalog resolves known bundles, and its render path explicitly handles Moniker and Forge. StoryBuilder needs a deliberate registered Experience/semantic GUI extension point in AnyApp, implemented and tested against the real host contracts. Do not fake this capability inside StoryBuilder or add a direct WPF dependency to the domain.

### Reconciliation plan

1. **Do not merge PR #1 as-is.** The histories are diverged by 866/21 commits; the PR is not a safe automatic reconciliation.
2. Preserve the AnyApp-first product decision. Keep Blazor as a useful authoring/demo host without making it a deployment prerequisite.
3. Map the master Core domain semantics (stable IDs, passage anchors, chronology, timestamps, observer IDs, schema version) to the development-side canonical workspace. Decide explicitly whether a versioned JSON importer is needed; if implemented, preserve IDs/timestamps and reject unsupported schemas or malformed links without replacing data with an empty workspace.
4. Reconcile local project save/reopen guarantees with the current host workspace persistence. Avoid two independently editable project stores; define one canonical model and explicit import/export boundaries.
5. Reconcile the AnyApp integration-gap register against current AnyApp contracts; implement the missing host registration and semantic GUI path in the correct host repository, not by pretending an unregistered bundle can launch.
6. Reconcile Core CI and Sith policy workflows deliberately, with one authoritative implementation of each responsibility and immutable action revisions where appropriate.
7. Run fresh build/test and policy checks against the final proposed reconciliation head; historical green runs validate only the commits they tested.
8. Review the exact resolved diff and obtain owner authorization before any merge.
9. Verify the resulting master contains the intended combined implementation and passing checks.
10. Confirm the only permanent branches remain master and development; the owner performs any manual branch cleanup.

## Latest verification

- Sith Policy workflow passed for documentation commit ead43f633d2ab86753caad31f9d5355468ed3b47 (run [42557646073](https://github.com/TrentBest/StoryBuilder/actions/runs/42557646073)).
- The workflow's policy-evidence refresh produced current development head 661775060b7b1546afa8cdfbedf97e43bea0fb8c.
- This is a policy check, not a fresh full build/test of the development head.

## Safety

No merge, release, package publishing, deployment, or branch deletion was performed by this work. Green checks on development do not authorize merging PR #1.
