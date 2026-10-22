# NarratorAgentTests

**User moniker:** Multi-FSM Narrator verification

**Purpose:** Verifies that the Narrator's focused FSM_API machines coordinate without merging their state or bypassing observer constraints.

## Coverage

- Intent, planning, observer, and draft/review states progress independently.
- A witnessed event can be linked into a saved draft.
- A reported event cannot be stated as directly witnessed by an observer.
- A blocked draft remains unchanged and enters NeedsReview.
- Separate agent instances do not share their intent state.
