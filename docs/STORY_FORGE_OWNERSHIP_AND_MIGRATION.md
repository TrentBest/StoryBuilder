# Story Forge ownership and migration

## Decision

Story Forge is a StoryBuilder domain capability. The existing implementation was originally placed in the Experiences repository, before the Workshop ecosystem grew to its current multi-repository shape. That placement is historical, not the intended long-term boundary.

**Target ownership:**
- **StoryBuilder** owns story-domain semantics and tooling: events, named timelines and ranges, entity-state facts, provenance, temporal queries, publication coverage, immutable edition snapshots, manuscript relationships, and observer-constrained views.
- **Experiences** owns Experience and MicroBundle lifecycle/CRUD: definitions, identity, registration, discovery, versioning, manifests, and reusable bundle artifacts. It must remain independent of StoryBuilder.
- **FSM_COS** owns composition and arbitration, not story-domain semantics.
- **AnyApp** hosts the working authoring and runtime experience; it must consume StoryBuilder through stable contracts.
- **FSM_Serialization** remains the preferred existing serialization capability where its contract fits; do not invent a parallel serialization system.

## Migration approach

The implementation and tests have been relocated into `src/StoryBuilder.StoryForge` and `tests/StoryBuilder.StoryForge.Tests` on `development`. StoryBuilder CI has passed for the relocated project, including temporal model, query-engine, and canonical-workspace projection tests. Experiences no longer builds or tests Story Forge, and its former implementation/test files have been removed; `src/StoryForge/README.md` remains only as a migration pointer. The Experiences solution/CI decoupling runs passed.

1. [x] Preserve and relocate the temporal model, query engine, publication coverage, tests, and relevant documentation/assets.
2. [x] Establish StoryBuilder-owned project names, namespaces, solution/test integration, and package metadata. Experiences remains independent of StoryBuilder.
3. [x] Add an explicit projection from canonical StoryBuilder workspace events into the temporal query index, preserving stable IDs and source provenance without duplicating canonical records.
4. [ ] Add persistence round-trip coverage for story records, temporal links, manuscript order, and provenance.
5. [ ] Wire the expanded host-neutral event/passage command IDs into AnyApp, then demonstrate create → link → save → restart → verify end to end.
6. [x] Remove the old Story Forge project/test ownership from Experiences and update its solution, docs, and CI.

## Current readiness

- **Story Forge relocation:** 100% — source, tests, project/solution/CI ownership are moved and both repositories' relevant CI has passed. This percentage covers the relocation, not the product workflow.
- **Runnable book workflow:** still in progress — temporal projection and the host-neutral event-to-passage command contract exist; the native AnyApp dispatcher is not yet wired to the new commands, and save/reopen verification for the complete workflow is not yet proven.

## Migration safety

- Do not delete the existing implementation before the StoryBuilder-owned replacement and tests are green.
- Keep the change on `development`; do not create branches.
- Do not merge, publish, release, deploy, or delete branches without the owner's explicit authorization.
- Distinguish repository relocation from runtime integration: moving the code does not, by itself, prove that AnyApp can author, save, reopen, or run an Experience.

## First runnable acceptance slice

In AnyApp, create a character, scene, event, timeline placement, and manuscript passage; connect them by stable identity; query the event through Story Forge; save and reopen the workspace; and verify all links, chronology, provenance, and manuscript order survive. Then capture and run the Experience through the existing manifest/composition contracts. Report each proven step separately.
