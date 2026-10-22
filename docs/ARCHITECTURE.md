# StoryBuilder Architecture

## Ownership

StoryBuilder owns the authoring domain: Experience design records, character profiles, world claims, events, scenes, campaigns, participant actions, source provenance, continuity issues, and vocabulary usage.

It does not own host lifecycle, durable storage, GUI rendering, physics simulation, or the general composition kernel.

## Boundary map

```text
Author / participant
        |
        v
Host experience (WebApp / WebPage / another host)
        |
        +---- GUI semantic model / GUI.SVG manifestations
        |
        v
StoryBuilder.Core (host-neutral domain)
  - character and scene authoring
  - provenance and canon status
  - vocabulary and synonym index
  - campaign and continuity records
        |
        +---- StoryBuilder.FSM (optional FSM_API lifecycle adapter)
        |
        +---- future serialization / bundle adapters
        |
        v
Host-owned persistence and runtime composition through FSM_COS
```

The diagram is a responsibility map. Only the domain core and optional FSM_API lifecycle adapter are currently implemented; UI, persistence, and FSM_COS composition adapters remain open.

## Core invariants

1. **A sketch is useful.** Required information is limited to what a workflow truly needs; defaults and templates are suggestions, not hidden facts.
2. **Identity survives editing.** Stable record IDs let scenes, relationships, sources, and campaigns refer to records while their descriptions change.
3. **Wording survives normalization.** Synonym search may group concepts, but occurrences retain their surface form and attribution.
4. **Claims have provenance.** A statement, the source that made it, and the truth status assigned by the author are distinct.
5. **Build record and fictional canon are separate.** An implementation dependency is not a world fact; a fictional technology is not an implemented capability.
6. **Participation is not predetermined.** Participant actions are recorded as proposals until deliberately accepted.
7. **Presentation is an adapter.** The domain does not depend on WPF, Blazor, SVG, a browser, or a game engine.

## Integration plan

- **FSM_API:** implemented in the optional StoryBuilder.FSM adapter for the draft/proposed/accepted/rejected/deprecated review lifecycle. StoryBuilder.Core remains independent of FSM_API.
- **FSM_COS:** compose optional authoring, vocabulary, visualization, and campaign capabilities at the host boundary. No StoryBuilder FSM_COS adapter exists yet.
- **FSM_Serialization:** integrate only after its current contracts are inspected and the workspace record shape is tested against them. The current core has no serialization package dependency.
- **GUI / GUI.SVG:** render semantic authoring controls and living diagrams from actual workspace state. The current domain core has no GUI dependency; GUI.Core's present semantic tree does not itself provide event callbacks.
- **WebApp / WebPage:** provide the user-facing Experience after the core authoring contract is verified.
- **Physics / Visualize / Renderer:** potential optional scene interaction and inspectable output layers, not required dependencies of the domain model.

## Current implementation boundary

The .NET 8 core provides character drafts and templates, structured scene beats, a narration draft, explicit synonym mapping, usage search, and a workspace record model. The optional FSM_API adapter demonstrates controlled review transitions. The core is in-memory. Persistence, a clickable authoring UI, campaign execution, source-derived live diagrams, and FSM_COS composition remain unproven.
