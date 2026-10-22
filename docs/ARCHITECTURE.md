# StoryBuilder Architecture

## Ownership

StoryBuilder owns the authoring domain: Experience design records, character profiles, world claims, events, scenes, campaigns, participant actions, source provenance, continuity issues, and vocabulary usage.

It does not own host lifecycle, durable storage, GUI rendering, physics simulation, or the general composition kernel.

## Boundary map

```text
Author / participant
        |
        v
Interactive StoryBuilder WebDemo (local proving ground)
        |
        +---- future Workshop WebPage host
        |
        +---- future GUI / GUI.SVG living manifestations
        |
        v
StoryBuilder.MicroBundle
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
        +---- future serialization adapter
        |
        v
FSM_COS composes MicroBundles; the host owns persistence and presentation
```

The MicroBundle adapter depends on the neutral MicroBundleDomain contract rather than FSM_COS itself. The automated composition test resolves and loads StoryBuilderAuthoringBundle through FSM_COS. The local Blazor workbench creates its session through the same bundle factory, but is not yet hosted by WebPage.

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
- **MicroBundleDomain / FSM_COS:** StoryBuilder.MicroBundle implements the domain-owned MicroBundle contract and declares workspace, Casting, Stage, vocabulary, history, and campaign providers. An automated composition test uses FSM_COS to load it.
- **FSM_Serialization:** integrate after its current contracts are inspected and the workspace record shape is tested against them. The current core has no serialization package dependency.
- **GUI / GUI.SVG:** render semantic authoring controls and living diagrams from actual workspace state. The current domain core has no GUI dependency; GUI.Core's present semantic tree does not itself provide event callbacks.
- **WebApp / WebPage:** the local Blazor host proves authoring interactions; the next integration is to mount the StoryBuilder Experience in the Workshop host without duplicating domain logic.
- **Physics / Visualize / Renderer:** potential optional scene interaction and inspectable output layers, not required dependencies of the domain model.

## Current implementation boundary

The .NET 8 core provides character drafts and templates, structured scene beats, a narration draft, automatic indexing of character and scene words through StoryBuilderSession, explicit synonym mapping, usage search, and a workspace record model. Optional adapters demonstrate FSM_API lifecycle behavior and the FSM_COS MicroBundle contract. The local Blazor workbench has clickable Casting, Stage, Archive, and Campaign pages. The data is in-memory; persistence, Workshop-host integration, and living GUI.SVG views remain open.
