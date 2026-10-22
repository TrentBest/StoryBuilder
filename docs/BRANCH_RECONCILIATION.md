# Branch Reconciliation

**Policy:** permanent branches are exactly `master` and `development`. The repository owner performs manual deletion of surplus branches; no branch was deleted by this work.

## Snapshot

The latest branch search found exactly these two branches:

| Branch | Observed head / disposition |
|---|---|
| `development` | Active integration branch. Contains the domain core, FSM_API lifecycle adapter, FSM_COS-composable MicroBundle, interactive Blazor workbench, console quickstart, tests, docs, and CI. |
| `master` | Default branch and stable reconciliation target. Pull request [#1](https://github.com/TrentBest/StoryBuilder/pull/1) proposes reconciling `development` into `master`; it remains open and unmerged. |

A previous snapshot in this ledger listed `main` at `9fabe16bcf7a50f3fbbdd9d6f4e2236ca5e79e06`. The latest branch search did not find a `main` branch, so it is not listed as a current branch or a pending deletion. This records observed branch state; it does not infer who removed it or when.

## Reconciliation plan

1. **Do not merge PR #1 as-is.** The live compare at the 22 October 2026 20:57 UTC review reports `development` as **455 commits ahead of `master` and 13 commits behind**. The histories remain diverged; re-check the compare before any merge decision.
2. The 13 commits on `master` are not merely administrative drift. They add a host-neutral `StoryBuilder.Core` project, stable-ID story project/timeline model, atomic local save/reopen, Core tests, a dedicated pinned-action CI workflow, and updated integration/deployment documentation. These must be inspected and reconciled with the richer `development` domain/adapters/workbench rather than overwritten or silently discarded.
3. Require fresh successful checks for the final reconciled head. Historical successful runs prove only the commits they tested.
4. Review the resolved diff and merge only after the owner authorizes reconciliation and current checks are green.
5. Verify `master` contains the intended combined implementation and passing checks.
6. Confirm the only permanent branches are `master` and `development`; the owner performs any manual branch cleanup.

## Safety

No merge, release, package publishing, deployment, or branch deletion was performed by this work.
