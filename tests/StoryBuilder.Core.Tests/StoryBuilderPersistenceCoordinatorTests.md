# StoryBuilderPersistenceCoordinatorTests

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

These tests verify the coordinator's host-save, restore, and legacy-browser migration behavior using an in-memory JavaScript interop fake. They complement, but do not replace, the real Chromium/WebDemo persistence test in `tests/StoryBuilder.WebDemo.BrowserTests`.

## Failure and recovery coverage

- A host write failure does not clear the only recoverable legacy browser snapshot.
- Malformed Base64 and malformed legacy binary payloads are reported and retained.
- Browser storage denial is reported without creating or overwriting a host workspace.
- An unreadable host workspace file is left byte-for-byte intact until the explicit `ClearAndStartFreshAsync` action.
- Explicit fresh-start replaces the unreadable host workspace with a valid empty workspace and clears a recoverable legacy browser copy only after the host save succeeds.
- Missing legacy browser storage remains a valid first-run path.

## Boundary

The fake JavaScript runtime proves coordinator error handling, not browser permission behavior. Actual browser reload and separate WebDemo process restart are covered by the Playwright test. These tests do not claim cloud sync, account isolation, backup/restore, or multi-user storage.
