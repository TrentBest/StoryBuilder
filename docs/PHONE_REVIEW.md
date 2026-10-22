# StoryBuilder — phone review

**Snapshot:** 22 October 2026  
**Product workflow readiness:** approximately 70% (planning estimate, not a test score).  
**Story Forge relocation:** 100% complete.  
**Release status:** not release-ready; no package was published and no release or deployment was made.

## The product in one sentence

StoryBuilder is a book-first authoring workbench where characters, scenes, world events, timelines, observer knowledge, and manuscript passages are separate canonical records connected by stable identity—not duplicated as prose.

## What is real today

- A .NET 8 domain core and interactive Blazor WebDemo.
- Character casting, scene/beat authoring, vocabulary and source context, campaign choices, timeline branches, provisional planning points, and canonical world events.
- Manuscript sections with reading order and event IDs; deterministic Markdown book output.
- Observer-knowledge evaluation and a constrained projection for narration/portrayal.
- Versioned binary workspace persistence through FSM_Serialization; browser demo saves locally and restores after refresh.
- A GUI.SVG living map and timeline portrayal rails derived from live workspace records.
- Story Forge's temporal model and tests live in StoryBuilder, not Experiences.
- The composition package compiler/reader, manifest dependency checks, integrity validation, and package round-trip/tamper tests have passed earlier CI runs.

## Latest authoring increment

The host-neutral MicroBundle surface now exposes commands to:
1. Create a timeline event (creates `Main Timeline` if needed).
2. Create a manuscript passage (creates `Untitled Book` if needed).
3. Link the latest event to the latest passage by stable event ID.
4. Refuse the link safely if its prerequisites are missing.

Regression tests now exercise these effects and verify the passage stores the event's ID. The command/test increment passed CI on commit [`4416df3`](https://github.com/TrentBest/StoryBuilder/actions/runs/39726767850); the later documentation-only commits have separate CI runs in progress. These commands are not yet wired into the AnyApp desktop command dispatcher, so this is not yet proof of the full native desktop workflow.

## Open critical path

- [ ] Green CI for the latest documentation/UML commits (the command/test increment itself is green).
- [ ] Wire the new command IDs into AnyApp's desktop dispatcher.
- [ ] Prove create event → create passage → link → save → restart → reopen with the same IDs and links.
- [ ] Add/verify full persistence round-trip coverage for temporal links, manuscript order, and provenance.
- [ ] Exercise the validated composition package in the real AnyApp host without rerunning arbitration.
- [ ] Then consider WebPage hosting, Renderer portrayal, immutable publication editions/export, and broader observer-aware editing.

## Architecture boundaries

- **StoryBuilder:** story meaning, timelines, canonical events, manuscript composition, observer perspective, provenance, temporal queries.
- **Experiences:** reusable Experience/MicroBundle lifecycle, identity, discovery, manifests, versioning, and artifacts; no StoryBuilder dependency.
- **FSM_COS:** composition and arbitration.
- **AnyApp:** native authoring/runtime host.
- **FSM_Serialization:** preferred binary persistence capability.

## Review links

- [README / quick start](../README.md)
- [Documentation index](../DOCUMENTATION_INDEX.md)
- [Development status and historical CI evidence](DEVELOPMENT_STATUS.md)
- [Living demo gallery](Gallery.md)
- [Story Forge ownership and migration](STORY_FORGE_OWNERSHIP_AND_MIGRATION.md)
- [Branch reconciliation ledger](BRANCH_RECONCILIATION.md)
- [Current CI workflow](https://github.com/TrentBest/StoryBuilder/actions/workflows/dotnet.yml?query=branch%3Adevelopment)

## Safety / branch status

Work remains on `development`. PRs have not been merged, branches have not been deleted, packages have not been published, and no release/deployment has been made. The owner retains manual branch cleanup and release authority.
