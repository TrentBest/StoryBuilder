# StoryWorkspaceTemporalProjectionTests

**Purpose:** Verify the read-only bridge from StoryBuilder.Core's canonical workspace records into Story Forge's temporal query index.

## Covered behavior

- Canonical event/timeline/participant/source IDs remain stable in the projection.
- Source provenance and event summary survive projection.
- The coordinate resolver is explicit; events without a usable coordinate are omitted rather than assigned invented dates.
- Events without an assigned timeline are not made queryable.
