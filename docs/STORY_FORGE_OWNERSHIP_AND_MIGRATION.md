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

The existing code and tests currently live under `TheSingularityWorkshop.Experiences/src/StoryForge` and `tests/StoryForge.Tests`. Move their implementation and test ownership into this repository before removing the old project from Experiences.

1. Preserve and relocate the temporal model, query engine, publication coverage, tests, and relevant documentation/assets.
2. Establish StoryBuilder-owned project names, namespaces, solution/test integration, and package metadata. Avoid a StoryBuilder dependency from Experiences.
3. Integrate Story Forge with StoryBuilder's canonical workspace records through explicit stable IDs and references. Do not create duplicate canonical story facts.
4. Add persistence round-trip coverage for story records, temporal links, manuscript order, and provenance.
5. Connect the authoring views to AnyApp and demonstrate the first book workflow end to end.
6. Only after the new home builds and tests pass, remove the old Story Forge project/test ownership from Experiences and update its solution, docs, and CI.

## Migration safety

- Do not delete the existing implementation before the StoryBuilder-owned replacement and tests are green.
- Keep the change on `development`; do not create branches.
- Do not merge, publish, release, deploy, or delete branches without the owner's explicit authorization.
- Distinguish repository relocation from runtime integration: moving the code does not, by itself, prove that AnyApp can author, save, reopen, or run an Experience.

## First runnable acceptance slice

In AnyApp, create a character, scene, event, timeline placement, and manuscript passage; connect them by stable identity; query the event through Story Forge; save and reopen the workspace; and verify all links, chronology, provenance, and manuscript order survive. Then capture and run the Experience through the existing manifest/composition contracts. Report each proven step separately.
