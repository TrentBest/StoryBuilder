# Branch Reconciliation

**Policy:** permanent branches are exactly `master` and `development`. The repository owner performs manual deletion of surplus branches; no branch is deleted by this work.

## Current verified snapshot — 9 November 2026

The branch listing contains exactly:

| Branch | Head | Disposition |
|---|---|---|
| `development` | `d2499641f6ed6fc8324b61848fea2a9a3a61e03b` | Active integration branch; latest feature commit is `4bba6593edae15c5fd83e8d9d1fb6aefd8e6f2b1`. |
| `master` | `6f4100d04faed5d36c4e946ad4af4c6cc5a15d91` | Stable/default target; not updated by this work. |

PR [#1](https://github.com/TrentBest/StoryBuilder/pull/1) remains open and unmerged.

### Live graph comparison

The GitHub compare endpoint currently reports:

- `development` is **866 commits ahead** of `master`.
- `development` is **21 commits behind** `master`.
- Status: **diverged**.
- Shared merge base: `3eef29a7b6c8b016db02c6d1acd44170585211c5`.

This replaces the older 22 October snapshot of 457 ahead / 13 behind. The 21 master-only commits are real history, not just a stale PR counter.

### Master-only changes that must be preserved or explicitly dispositioned

A direct comparison of the merge base against `master` identifies these master-side paths:

- `.github/workflows/core-tests.yml`: dedicated Core CI workflow.
- `src/StoryBuilder.Core/StoryProject.cs`: stable-ID project, passage-anchor, and timeline-event model.
- `src/StoryBuilder.Core/LocalProjectStore.cs`: local JSON persistence with temporary-file replacement.
- `src/StoryBuilder.Core/StoryBuilder.Core.csproj` and `tests/StoryBuilder.Core.Tests/*`: Core project and tests.
- `docs/DEPLOYMENT_DECISION.md` and `docs/INTEGRATION_GAPS.md`: desktop-first decision and explicit host integration gaps.
- `README.md` and `docs/Gallery.md`: product framing and gallery changes.
- `.github/workflows/sith-policy.yml`, `assets/sith-policy.json`, and `assets/sith-policy.svg`: policy/evidence automation and assets.

The `development` branch already has a richer StoryBuilder.Core domain and its own binary workspace persistence, FSM_API/FSM_COS adapters, MicroBundle, Blazor authoring workbench, and tests. The master-side `StoryProject`/JSON store is therefore not safe to merge by blindly copying files: first establish whether its stable IDs, passage anchors, event timestamps, and JSON compatibility need an explicit importer into the richer workspace model. Preserve the host-neutral domain boundary and the useful deployment/integration-gap decisions. Reconcile CI responsibilities without duplicate or contradictory workflows. Policy files/evidence must be compared semantically before replacing either branch's version.

## Reconciliation plan

1. **Do not merge PR #1 as-is.** The histories are diverged by 866/21 commits; the PR is not a safe automatic reconciliation.
2. Inventory the 21 master-only commits and preserve their domain behavior, persistence guarantees, CI intent, and deployment/integration decisions.
3. Prefer a one-way, explicit legacy JSON importer into the canonical development-side workspace if the master-side schema still needs support. Preserve IDs and timestamps exactly; reject unsupported schemas and malformed links without falling back to an empty workspace. Do not create a second live authoring model or silently discard master-only data.
4. Reconcile workflows and Sith policy artifacts deliberately, with one authoritative implementation of each responsibility.
5. Run fresh build/test and policy checks against the final proposed reconciliation head; historical green runs validate only the commits they tested.
6. Review the exact resolved diff and obtain owner authorization before any merge.
7. Verify the resulting `master` contains the intended combined implementation and passing checks.
8. Confirm the only permanent branches remain `master` and `development`; the owner performs any manual branch cleanup.

## Safety

No merge, release, package publishing, deployment, or branch deletion was performed by this work. Green checks on `development` do not authorize merging PR #1.
