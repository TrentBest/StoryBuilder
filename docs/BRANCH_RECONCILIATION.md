# Branch Reconciliation

**Policy:** permanent branches are exactly master and development. The repository owner performs manual deletion of surplus branches; no branch was deleted by this work.

## Snapshot

| Branch | Observed head | Disposition |
|---|---|---|
| development | Active integration branch | Contains the initial design plus domain core, tests, quickstart, documentation, CI, and optional FSM_API adapter. |
| master | 3eef29a7b6c8b016db02c6d1acd44170585211c5 | Created from development after the initial documentation pass; currently behind the new implementation. Must be updated through a reviewed development-to-master pull request. |
| main | 9fabe16bcf7a50f3fbbdd9d6f4e2236ca5e79e06 | Original repository seed commit. Development was created from this history and master was created downstream of development. No unique work has been identified on main; no deletion performed. |

## Reconciliation plan

1. Wait for the current development build/test workflow covering StoryBuilder.Core, StoryBuilder.FSM, the quickstart, and the test suite.
2. Open a pull request from development to master and require its checks to pass before merging.
3. Verify master contains the complete reconciled history and the same passing checks.
4. The default branch is still main at this snapshot. Change the repository default to master before considering main ready for owner-performed manual deletion.
5. Confirm only master and development remain after the owner completes the manual cleanup.

## Safety

No release, package publishing, deployment, or branch deletion was performed. The extra main branch is **not yet ready for manual deletion** because master integration and the default-branch change remain outstanding.
