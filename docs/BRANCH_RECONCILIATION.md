# Branch Reconciliation

**Policy:** permanent branches are exactly master and development. The repository owner performs manual deletion of surplus branches; no branch was deleted by this work.

## Snapshot

| Branch | Observed head | Disposition |
|---|---|---|
| development | Active integration branch | Contains the domain core, tests, console quickstart, interactive WebDemo, documentation, CI, FSM_API lifecycle adapter, and FSM_COS-composable MicroBundle. |
| master | 3eef29a7b6c8b016db02c6d1acd44170585211c5 | Stable target; currently behind the implementation. Pull request [#1](https://github.com/TrentBest/StoryBuilder/pull/1) reconciles development into master. |
| main | 9fabe16bcf7a50f3fbbdd9d6f4e2236ca5e79e06 | Original repository seed commit. Development descends from this history, and master was created downstream of development. No unique work has been identified on main. No deletion performed. |

The repository's default branch is now **master**.

## Reconciliation plan

1. Require the latest development / PR #1 build and test checks to pass.
2. Merge PR #1 into master after review and green checks.
3. Verify master contains the full reconciled implementation and passing checks.
4. At that point, **main will be ready for the owner's manual deletion**: master is the default, development and master preserve the initial seed history, and no unique main work has been identified.
5. The owner performs the manual deletion and confirms only master and development remain.

## Safety

No release, package publishing, deployment, or branch deletion was performed. The extra main branch is not being deleted automatically; it is awaiting PR #1 reconciliation before being marked ready for manual deletion.
