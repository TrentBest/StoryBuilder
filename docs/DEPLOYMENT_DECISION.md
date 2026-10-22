# StoryBuilder Deployment Decision

**Decision:** StoryBuilder's first runnable host is [AnyApp](https://github.com/TrentBest/AnyApp), the Workshop's Windows desktop host. A hosted WebApp is not a prerequisite.

## Why

The first useful StoryBuilder should run on the creator's desktop without requiring a separately hosted platform. The application can still use the Workshop's composition architecture while keeping authored story data local by default.

## Boundary

```text
StoryBuilder domain and composition model
    |                       |
    | story records         | runtime capabilities
    v                       v
Local project storage     FSM_COS manifest
    |                       |
    +------> AnyApp <-------+
                 |
                 v
        GUI semantic surface
                 |
                 v
          GUI.WPF / desktop
```

This is a target architecture, not a claim that the StoryBuilder runtime or authoring UI already exists. AnyApp is the host; FSM_COS remains the composition boundary; StoryBuilder owns its domain meaning and workflows.

## Initial delivery rules

1. Launch locally as a Windows desktop application through AnyApp.
2. Keep the story/domain model independent of WPF and presentation details.
3. Store the first project locally; do not require Azure, cloud hosting, or a remote database for ordinary authoring.
4. Keep the manuscript, chronology, characters, and continuity views as projections of shared records rather than separate sources of truth.
5. Preserve stable IDs and provenance so passages, events, claims, and later views can cross-reference each other.
6. Separate actual Experience implementation evidence from fictional canon.
7. Add a browser host later only if it provides clear value; it must not become a prerequisite for the desktop workflow.

## First vertical slice

- Open StoryBuilder from AnyApp.
- Create and reopen a local project.
- Edit its title, premise, and manuscript.
- Add a stable-ID story event and associate it with a passage.
- View the same event in a basic chronology.
- Verify persistence after closing and reopening.

## Explicit non-goals for this slice

- No mandatory hosted service.
- No mandatory Azure dependency for local authoring.
- No multi-user synchronization.
- No autonomous narrator or multi-agent orchestration yet.
- No publishing to NuGet without explicit creator authorization.

## Compatibility note

AnyApp's current repository is a Windows WPF desktop host and uses versioned Workshop packages. The StoryBuilder integration must be built against the actual contracts and versions available in the repository, not against assumed APIs. If a required capability is missing, record the gap and implement the smallest justified adapter without changing package ownership boundaries.
