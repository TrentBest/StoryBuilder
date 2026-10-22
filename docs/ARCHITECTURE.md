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
StoryBuilder domain core
  - character and scene authoring
  - provenance and canon status
  - vocabulary and synonym index
  - campaign and continuity records
        |
        +---- optional FSM_API workflow behavior
        |
        +---- optional serialization / bundle adapters
        |
        v
Host-owned persistence and runtime composition through FSM_COS
```

The diagram is a target responsibility map, not a claim that every adapter is integrated today.

## Core invariants

1. **A sketch is useful.** Required information is limited to what a workflow truly needs; defaults and templates are suggestions, not hidden facts.
2. **Identity survives editing.** Stable record IDs let scenes, relationships, sources, and campaigns refer to records while their descriptions change.
3. **Wording survives normalization.** Synonym search may group concepts, but occurrences retain their surface form and attribution.
4. **Claims have provenance.** A statement, the source that made it, and the truth status assigned by the author are distinct.
5. **Build record and fictional canon are separate.** An implementation dependency is not a world fact; a fictional technology is not an implemented capability.
6. **Participation is not predetermined.** Participant actions are recorded as proposals until deliberately accepted.
7. **Presentation is an adapter.** The domain does not depend on WPF, Blazor, SVG, a browser, or a game engine.

## Integration plan

- **FSM_API:** use for explicit workflow transitions when there is a verified lifecycle to model. The domain models must not become dependent on FSM_COS.
- **FSM_COS:** compose optional authoring, vocabulary, visualization, and campaign capabilities at the host boundary. StoryBuilder does not reimplement the composition kernel.
- **FSM_Serialization:** integrate only after its current contracts are inspected and the workspace record shape is tested against them. The first core currently has no serialization package dependency.
- **GUI / GUI.SVG:** render semantic authoring controls and living diagrams from actual workspace state. The current domain core has no GUI dependency.
- **WebApp / WebPage:** provide the user-facing Experience after the core authoring contract is verified.
- **Physics / Visualize / Renderer:** potential optional scene interaction and inspectable output layers, not required dependencies of the domain model.

## Current implementation boundary

The first C# core provides character drafts and templates, structured scene beats, a simple narration draft, explicit synonym mapping, usage search, and a workspace record model. It is currently in-memory. Persistence, a clickable GUI, campaign execution, source-derived live diagrams, and cross-package adapters are not yet proven.
