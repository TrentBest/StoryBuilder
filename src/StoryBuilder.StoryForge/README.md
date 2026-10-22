# Story Forge — StoryBuilder temporal domain

Story Forge is the temporal domain engine owned by StoryBuilder. It models a story as connected, queryable records rather than making manuscript prose the only representation of canon.

## Responsibilities

- Named timelines and typed story-time coordinates/ranges.
- Events, participants, certainty, and provenance.
- Time-bounded entity-state assertions and temporal queries.
- Conflict reporting and optional proposal filtering.
- Publication coverage and immutable edition snapshots.

Story time is distinct from wall-clock time and publication order. A later-discovered event can occur earlier in the fictional world without silently mutating an already published edition.

## Boundaries

StoryBuilder owns story semantics and authoring. Experiences owns reusable Experience definitions, MicroBundle CRUD/lifecycle, manifests, and artifacts. FSM_COS composes capabilities at the host boundary; AnyApp presents them. Story Forge must not depend on Experiences, FSM_COS, GUI, or a host.

## Verification

The `StoryBuilder.StoryForge.Tests` project carries the temporal-model and query-engine tests migrated from the former Experiences placement. The source migration is additive at this stage: the original project remains in Experiences until the StoryBuilder-owned build/test path is green and downstream references have been reconciled.
