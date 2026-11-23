# Campaign Builder and Live Game Master Workbench

## Purpose

StoryBuilder is the story that is happening to the player or players. It must support both sides of that responsibility:

- **Preparation:** author the campaign, establish its world context, assemble sourcebooks, design quests and adventures, prepare encounters and contingencies, and define what the GM needs at the table.
- **Play:** operate a live campaign, observe what is happening, make deliberate interventions, resolve choices and consequences, and preserve the resulting shared history.

The Campaign Builder is therefore not a separate product bolted onto the manuscript editor. It is the campaign-oriented authoring and operational surface over StoryBuilder's shared story domain.

The GM dashboard is the control room for a campaign. It should make the current state legible and make consequential actions explicit, while leaving world simulation and domain rules to their owning systems.

## Product model

Keep these concepts distinct:

| Concept | Responsibility | Lifetime |
| --- | --- | --- |
| Sourcebook | Reusable knowledge about a domain, setting, place, faction, creature, technology, culture, historical period, or other subject. | Reused across campaigns and editions. |
| Campaign | A continuing play experience with participants, scope, premise, starting conditions, and persistent world state. | Long-running; may span many sessions. |
| Adventure | A prepared playable situation or bounded arc within a campaign. | Reusable or campaign-specific; may span sessions. |
| Quest | An objective or connected set of objectives with discoverable state, prerequisites, and possible outcomes. | Can begin, change, branch, complete, fail, or become irrelevant during play. |
| Session | A bounded play meeting or execution window within a campaign. | Opened and closed; contributes to campaign history. |
| Scene | A focused situation in which participants act and the world responds. | Usually short-lived, but its consequences can persist. |
| Encounter | A structured interaction or challenge: social, exploratory, environmental, tactical, economic, or combat. | Prepared or emergent; resolved through the applicable domain systems. |
| Event | A durable statement that something happened in the world, with time, actors, causes, consequences, and status. | Persistent history. |
| Participant action | An attributed player decision or declared action. | Recorded first as an intent/proposal; resolved through the rules and GM authority. |
| GM intervention | A deliberate ruling, reveal, adjustment, override, or world-direction action. | Audited as part of the session record. |

A campaign is not a linear script. Adventures, quests, scenes, and encounters are tools for preparing possibilities. Players can ignore hooks, change objectives, create new factions, destroy important places, or solve problems in unexpected ways. The system records what actually happened rather than rewriting play to fit the preparation.

## Architectural boundaries

StoryBuilder owns the campaign's authored and recorded story state:

- campaign identity, premise, scope, participants, and campaign configuration;
- prepared adventures, quests, scenes, encounters, objectives, clues, and contingencies;
- references to reusable sourcebook content and selected versions;
- session boundaries, declared actions, GM interventions, rulings, and session notes;
- event history, causal links, timeline placement, observer knowledge, and portrayal;
- campaign-facing projections for the GM and participant experiences.

SURPS owns the reusable roleplaying ontology and behavioural/rules semantics: what entities are, what capabilities and constraints apply, how actions are evaluated, and what consequences follow. StoryBuilder should consume a formal SURPS contract, not duplicate or quietly redefine SURPS rules.

Execution belongs to the appropriate runtime systems. StoryBuilder may request a rules evaluation or simulation step and display its result; it should not become a second physics engine, combat engine, NPC brain, or world simulator. The GM remains able to make an explicit ruling where the campaign permits it, and that ruling must be distinguishable from a mechanically derived result.

Sourcebook repositories remain domain owners. A campaign can compose knowledge from multiple sourcebooks without copying their entire contents into its own identity. Pin the sourcebook edition/revision used for reproducibility; record deliberate campaign overrides as campaign-owned material rather than mutating the sourcebook.

## GM dashboard: operational areas

The dashboard should be organized around the GM's current task, not around internal package boundaries.

1. **Campaign Overview**
   - campaign premise, status, active participants, current session, current in-world time and location;
   - campaign health and outstanding decisions;
   - quick access to the active adventure and current objectives.

2. **World and Sourcebooks**
   - browse and attach sourcebooks by domain, region, period, and level of detail;
   - inspect the exact entry and revision being used;
   - see missing knowledge or unresolved references;
   - add campaign-specific facts and overrides without silently changing shared source material.

3. **Adventure and Quest Builder**
   - create an adventure from a blank outline, a sourcebook, or an existing template;
   - define hooks, objectives, prerequisites, clues, locations, factions, and likely consequences;
   - model branches and alternative approaches as possibilities, not mandatory story rails;
   - connect quests and objectives to events and world entities by stable IDs.

4. **Cast and Factions**
   - player characters, non-player characters, organizations, groups, and relationships;
   - motives, knowledge, loyalties, capabilities, and current disposition;
   - separate what the GM knows from what each character or player knows.

5. **Locations and Situation**
   - current scene, map or spatial view where available, occupants, active hazards, resources, and nearby points of interest;
   - inspect the situation without turning a visualization into an independent source of truth.

6. **Encounter and Resolution**
   - prepare or initiate an encounter;
   - declare actions, select relevant participants and targets, request SURPS evaluation, and inspect the result;
   - expose applicable constraints, costs, risks, and consequences;
   - record a GM ruling separately when the GM changes or overrides the proposed result.

7. **Live Session Console**
   - turn/beat or time progression where the selected ruleset supports it;
   - participant declarations, initiative/order when applicable, active effects, pending checks, and unresolved decisions;
   - pause, resume, advance, rewind only through explicit compensating actions, or branch a what-if simulation without silently rewriting canonical history.

8. **Events and Continuity**
   - chronological and causal history;
   - proposed versus accepted events;
   - unresolved contradictions, dangling references, and timeline uncertainty;
   - inspect which scenes, session records, and manuscript sections portray each event.

9. **Session Journal and Aftermath**
   - summarize the session from actual recorded events;
   - preserve player-facing discoveries separately from GM-only notes;
   - record rewards, injuries, changes of allegiance, world-state changes, and open threads;
   - prepare the next session from the actual campaign state.

10. **Campaign Administration**
    - participants, permissions, safety/consent settings, campaign configuration, save/restore, export, and audit history;
    - host-owned authentication and authorization; StoryBuilder workspace data is not itself proof of identity or permission.

## Preparation-to-play lifecycle

A campaign should move through explicit, inspectable phases:

1. **Draft:** define the premise, campaign scope, starting context, and intended experience.
2. **Assemble:** attach sourcebook editions and define campaign-specific adaptations.
3. **Prepare:** create adventures, quests, NPC intentions, locations, clues, encounters, and contingency material.
4. **Ready:** validate required references, unresolved preparation decisions, permissions, and execution capabilities.
5. **Play:** open a session and record participant declarations, GM actions, evaluations, and world events.
6. **Reconcile:** resolve pending proposals, review consequences, update campaign state, and inspect continuity.
7. **Close session:** freeze the session record as an immutable snapshot while leaving the campaign active.
8. **Continue or conclude:** prepare another session, pause the campaign, or record its conclusion.

Do not let a status label imply that a campaign is mechanically ready. Readiness should be a validation result with actionable findings.

## Live play and truth boundaries

The current Campaign Room already creates campaigns and records participant choices as proposals. That is a useful seed, but it is not yet a live GM console.

Build toward a command-driven session model:

- A participant declaration is an intent, not an accomplished world event.
- A SURPS evaluation is a rules result, not automatically a canonical event.
- A GM ruling is an attributed intervention, not a hidden mutation.
- A resolved action may produce one or more proposed world events and state changes.
- Accepted outcomes update the campaign's authoritative state and event history.
- A scene or manuscript passage portrays events; it does not replace them.
- Observer knowledge is updated according to the campaign's disclosure rules, not because the GM dashboard can see everything.

Every consequential operation should have a stable ID, actor/authority, timestamp or story-time coordinate, input references, result, and provenance. Prefer append-only decisions and compensating corrections over destructive edits to an already-played session.

For uncertain or random outcomes, persist the inputs and enough evaluation provenance to explain or reproduce the result where the underlying rules system supports it. Never claim deterministic replay if the execution system cannot guarantee it.

## GM authority, automation, and AI

The GM dashboard may automate preparation, offer suggestions, surface likely consequences, and request NPC or world evaluations. Automation must remain visible and bounded.

- Suggestions do not become campaign facts by being generated.
- An automated agent's declaration is not a player decision or GM ruling.
- Host authorization controls who may issue commands.
- High-impact actions should require an explicit confirmation or configured GM authority.
- The system should make it clear whether an outcome came from SURPS, a simulation subsystem, a GM ruling, or an external agent.
- AI is an optional host capability. StoryBuilder Core must remain usable without an LLM or AI package dependency.

## First vertical slice

Do not attempt a complete virtual tabletop or every game mechanic at once. Prove the campaign loop with a small end-to-end vertical slice:

1. Create a campaign and attach a versioned sourcebook reference.
2. Create one adventure containing two quests, a small cast, and a few locations.
3. Open a session and establish its starting situation.
4. Record a player declaration.
5. Submit a typed resolution request through an adapter boundary.
6. Receive a result and present it to the GM for acceptance, modification, or rejection.
7. Record the resulting event(s), causal links, and observer-knowledge changes.
8. Show the updated campaign dashboard and a session journal derived from the recorded history.
9. Save, close, restore, and verify that the same campaign and session history are recovered.

The first proof can use a deterministic test resolver behind an interface. That validates StoryBuilder's orchestration and audit model without pretending the SURPS contract or full runtime integration already exists.

## Current implementation versus target

Implemented today: campaign records and participant-action proposals in StoryBuilder.Core; a simple Campaign Room for creating campaigns, recording choices, and requesting lifecycle transitions; broader story events, timelines, causality, observer knowledge, scenes, places, and persistence.

Not yet implemented as a complete product: adventure and quest domain models, sourcebook attachment/version pinning, a live GM dashboard, session lifecycle and immutable session snapshots, typed rules-evaluation requests/results, encounter orchestration, campaign permissions, participant-facing projections, and end-to-end save/restore of a running campaign.

This document defines a direction and proposed boundaries. It does not claim that SURPS integration, campaign execution, or the dashboard is already implemented.

## First sourcebook integration pilot: FSM_Dungeon Fantasy Library

The initial cross-repository pilot is the original [FSM_Dungeon Fantasy Library](https://github.com/TrentBest/FSM_Dungeon/tree/development/docs/books). It currently includes Fantasy Foundations, The Living Bestiary, Elves: Peoples of the Long Memory, Dragons and Wyrms: Ecology of the Elder Sky, Ruins and Lost Places, and The Broken Star campaign seed.

Use this pilot to prove the sourcebook boundary before attempting a broad marketplace or universal catalogue:
- browse a domain-owned book without copying its full contents into the campaign;
- reference a stable book and entry identity;
- pin the exact revision used by a campaign;
- attach elf, bestiary, draconic ecology, and ruin-site knowledge to an adventure;
- use The Broken Star as a premise proposal, not a forced plot;
- compose named ruins, creature populations, and draconic lineages as reusable sourcebook references rather than copying their content into campaign canon;
- keep campaign-specific discoveries and adaptations in StoryBuilder;
- preserve player decisions and accepted consequences as the campaign's actual history.

The Dungeon sourcebook repository owns its setting content and future revisions. StoryBuilder owns the campaign's selected references, preparation, session records, and evolving story. SURPS remains the authority for roleplaying ontology and resolution. The integration work is tracked in [StoryBuilder issue #15](https://github.com/TrentBest/StoryBuilder/issues/15).

This is an authoring/content pilot. It does not imply that sourcebook pinning, adventure/quest models, or the complete GM dashboard are already implemented.

