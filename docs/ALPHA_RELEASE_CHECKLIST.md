# StoryBuilder Alpha release checklist

**Target:** a clearly scoped public Alpha for the book-first authoring workbench.  
**Snapshot:** 22 October 2026.  
**Current status:** release candidate in preparation; not yet cleared for publication.

## Release gates

- [x] Domain core, MicroBundle, binary workspace persistence, WebDemo, quickstart, and test suites have green CI evidence on development.
- [x] Event-to-manuscript passage linkage is represented by canonical IDs and covered by domain persistence round-trip tests.
- [x] The command-level event → passage → link workflow survives a binary save/reopen test in [CI run 39821291048](https://github.com/TrentBest/StoryBuilder/actions/runs/39821291048).
- [x] The AnyApp dispatcher was updated to accept event creation, passage creation, and event-to-passage linking commands while preserving its save-after-success behavior.
- [x] AnyApp Windows Release build and test suite passed after the dispatcher change: [workflow run 39820766164](https://github.com/TrentBest/AnyApp/actions/runs/39820766164). This historical run does not verify the newer host-local binary persistence implementation.
- [x] Obtain green CI for the current AnyApp `development` source, including `StoryBuilderDesktopPersistenceStore` restore/save behavior and the expanded command dispatcher: [workflow run 39850248777](https://github.com/TrentBest/AnyApp/actions/runs/39850248777) completed successfully on commit `f780889a4c47a561d59b4f22e4d740edf4e6bacb` on 22 October 2026. The regression verifies manuscript-section event links survive store restore. The separate native process restart/reopen smoke test remains open; this CI result does not close that end-to-end gate.
- [x] Verify native process restart/reopen persistence: AnyApp workflow [39863022603](https://github.com/TrentBest/AnyApp/actions/runs/39863022603) runs the native app in a write process, exits, starts a separate verify process, reloads the workspace, and asserts the event-to-manuscript-section link survives. This closes the process-level persistence gate for the current probe path; interactive workflow coverage and real-host composition-package consumption remain separate.
- [x] Trent selected MIT to align with the Workshop's existing open-source direction; added the repository `LICENSE` and linked it from the README.
- [ ] Reconcile the open development → master PR. The live GitHub comparison confirms that `development` is ahead of and 13 commits behind `master` (reviewed 22 October 2026). The ahead count changes with new commits; recheck immediately before any reconciliation. Do not blindly merge.
- [ ] Decide the Alpha's exact version/tag, included projects, known limitations, and install/run instructions.
- [ ] Create the GitHub Alpha release only after the preceding gates are resolved and the final release commit is verified.

## Scope of the first Alpha

The defensible first milestone is a local, book-first authoring prototype: create characters and scenes, record timeline events, place events in manuscript passages, inspect the deterministic Markdown book output, and preserve a versioned binary workspace. The browser demo has browser-local persistence. The native AnyApp host is the intended desktop path; the automated two-process persistence probe now proves the saved workspace and event-to-section link survive a process restart. Real-host composition-package consumption remains to be validated.

Not promised for this Alpha: cloud sync, collaboration, LLM-backed drafting, immutable publication editions, complete Renderer integration, or Workshop WebPage hosting.

## Publishing boundary

No release or package has been created by this checklist. NuGet publishing is a separate action and remains gated on final package validation and explicit approval. A GitHub source Alpha must not imply that NuGet packages were published.

## Phone review

- [Development status and evidence](DEVELOPMENT_STATUS.md)
- [Phone review guide](PHONE_REVIEW.md)
- [Living demo gallery](Gallery.md)
- [Current AnyApp verification run](https://github.com/TrentBest/AnyApp/actions/runs/39820766164)
- [StoryBuilder development CI](https://github.com/TrentBest/StoryBuilder/actions/workflows/dotnet.yml?query=branch%3Adevelopment)
