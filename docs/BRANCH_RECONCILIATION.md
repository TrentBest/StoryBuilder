# Branch Reconciliation

## Policy

Permanent branches are exactly `master` and `development`.

- `development` is the active source and integration branch. Continue implementation and validation there; Sith branch policy is a **publication gate**, not a development/execution block.
- `master` is the stable/release-aligned branch. Do not update it as part of ordinary development.
- No automation or assistant action deletes branches. The repository owner authorizes any retirement batch.
- Never reconcile divergent histories by blindly merging, resetting, or force-pushing either canonical branch.

## Current verified snapshot — 12 November 2026

The live branch listing contains exactly:

| Branch | Head | Disposition |
|---|---|---|
| `development` | `297489051e949d8ec700f7d79af99d548bb47be9` | Active source and integration branch |
| `master` | `e228f62522ecbc812dd3b26f17de72b407ecc4be` | Stable/default branch; unchanged by this audit |

The Sith Policy workflow completed successfully on 12 November 2026: [run 43177940309](https://github.com/TrentBest/StoryBuilder/actions/runs/43177940309). Its evidence reports exactly two branches and `COMPLIANT`.

## Live graph comparison

GitHub compare `master...development` reports:

- **873 commits ahead** on `development`.
- **24 commits behind** on `development` (24 master-only commits).
- Status: `diverged`.
- Shared merge base: `3eef29a7b6c8b016db02c6d1acd44170585211c5`.

The current merge-base-to-`master` comparison identifies 13 changed paths:

- `.github/workflows/core-tests.yml`, `.github/workflows/sith-policy.yml`
- `README.md`, `assets/sith-policy.json`, `assets/sith-policy.svg`
- `docs/DEPLOYMENT_DECISION.md`, `docs/INTEGRATION_GAPS.md`, `docs/Gallery.md`
- `src/StoryBuilder.Core/StoryBuilder.Core.csproj`
- `src/StoryBuilder.Core/StoryProject.cs`, `src/StoryBuilder.Core/LocalProjectStore.cs`
- `tests/StoryBuilder.Core.Tests/StoryBuilder.Core.Tests.csproj`, `tests/StoryBuilder.Core.Tests/StoryProjectTests.cs`

The master-only history includes the explicit **AnyApp Windows desktop first-host decision**, an earlier host-neutral `StoryProject` / local JSON store with stable project/event/passage identities and chronology, dedicated Core tests/CI, integration-gap documentation, and Sith policy evidence. Later master-only commits also refresh the policy evidence. These changes are not safely reconciled by copying files over the richer development workspace model.

## Architectural reconciliation

The two sides express different generations of the product:

- `master` records the AnyApp desktop-first product decision and an earlier `StoryProject` persistence model.
- `development` contains the richer canonical StoryWorkspace domain, binary `FSM_Serialization` persistence, FSM_API/FSM_COS adapters, MicroBundle composition, and a Blazor workbench used as a development/demo surface.

These can coexist without making the WebDemo the required first product host. Preserve AnyApp desktop as the intended first host unless the owner explicitly changes that decision. The WebDemo remains useful for authoring and domain proof.

Do not create two competing editable project/workspace stores. Before adopting the master-side model, map its stable project/event/passage IDs, timestamps, observer IDs, chronology semantics, and JSON schema behavior into the canonical development workspace. If legacy JSON import is warranted, make it explicit and versioned, preserve identities/timestamps, and fail closed on malformed or unsupported input.

AnyApp's real host integration remains a separate requirement: verify the registered Experience/semantic GUI extension point against the current AnyApp contracts. Do not fake host registration inside StoryBuilder or add presentation dependencies to the domain core.

## Reconciliation plan

1. Keep development work moving on `development`; the satisfied branch-count policy must not block implementation or CI.
2. Preserve the AnyApp-first deployment decision while treating Blazor as a development/demo authoring surface.
3. Map the master-side `StoryProject` identity, chronology, observer, and JSON schema semantics to the canonical `StoryWorkspace`; decide whether a one-way legacy importer is needed.
4. Reconcile host-local persistence guarantees without introducing competing editable stores or silently replacing unreadable data.
5. Verify the StoryBuilder Experience through the actual AnyApp host and current FSM_COS composition contracts.
6. Retain one authoritative Core CI workflow and one Sith policy workflow, with immutable action revisions.
7. Run fresh build/test and policy checks on the exact proposed development head. Historical green runs validate only the commits they tested.
8. Review the exact resolved diff and obtain explicit owner authorization before updating `master`.
9. After any authorized reconciliation, verify the intended combined implementation and checks on `master`.
10. Keep exactly `master` and `development`; the owner performs any manual branch cleanup in one authorized batch.

## Current PR disposition

PR [#1](https://github.com/TrentBest/StoryBuilder/pull/1) remains open and unmerged. A fresh graph and policy audit was recorded in [PR comment 6818350425](https://github.com/TrentBest/StoryBuilder/pull/1#issuecomment-6818350425).

No review submissions or inline review threads were returned by the current inspection. The latest development head is a policy-evidence refresh commit; the inspected commit-status lookup returned no full build/test checks for that exact head. The Sith policy workflow passing does not substitute for build/test validation.

## Safety

No merge, release, package publication, deployment, force-push, `master` update, or branch deletion was performed by this audit.
