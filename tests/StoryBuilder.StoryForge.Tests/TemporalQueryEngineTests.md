# TemporalQueryEngineTests

Regression tests for exact-coordinate event selection, state-fact range queries, entity focus, proposal filtering, and conflict behavior.

## Use cases

- Ensure temporal queries return only records from the requested time domain.
- Verify state transitions at adjacent range boundaries.
- Prevent proposed assertions from silently becoming accepted canon.
