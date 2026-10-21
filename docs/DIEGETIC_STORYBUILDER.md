# StoryBuilder as a Diegetic Workshop Tool

## Purpose

StoryBuilder is both:

1. A real developer-facing tool for documenting how an Experience or Tool is conceived, assembled, and operated.
2. An in-universe story workbench that helps authors and campaign participants build a shared history, explore its consequences, and preserve continuity.

The tool must be useful before the fictional setting is complete. The first Experience StoryBuilder documents should be the creation of StoryBuilder itself.

## Diegetic principle

The interface should feel like an instrument that belongs in the world—not an ordinary editor wearing a science-fiction skin.

A developer might open a **World Archive**, inspect a **Chronology**, consult a **Continuity Desk**, or enter a **Campaign Room**. Those are proposed interface metaphors, not final names. Each must map to a real function:

- **Experience Blueprint:** what is being built, why it exists, and which components it uses.
- **World Archive:** accepted facts, open questions, sources, and historical records.
- **Chronology:** eras, nested periods, event sequences, and cause-and-effect relationships.
- **Campaign Room:** active participants, current situation, available choices, and resulting events.
- **Continuity Desk:** contradictions, unresolved claims, provenance, and canon decisions.
- **Living Display:** a visual projection of actual structured data—not a static mockup that pretends to be live.

## Two linked models, never silently merged

### Build record

Documents the real Experience or Tool: purpose, boundaries, components, dependencies, decisions, tests, implementation evidence, and readiness.

### Fictional canon

Documents the invented world: people, places, events, institutions, claims, traditions, and narrative consequences.

The models can reference each other, but their claims have different meanings. A real software dependency must not become a fictional historical fact merely because it appears in the same interface. A fictional technology must not be described as implemented software unless there is implementation evidence.

## Campaign participation

A campaign is not just a sequence of generated prose. It is a continuing situation in which participants make choices and those choices can change the shared record.

A minimal campaign loop:

1. Present a grounded situation and its known context.
2. Show what the participating character or group can plausibly know.
3. Offer meaningful choices, including room for participant-authored actions.
4. Record the choice and its consequences as a proposed event.
5. Evaluate the event against established canon and active campaign state.
6. Accept, revise, or reject continuity changes deliberately.
7. Preserve the result so the next session begins from the changed world.

The system should preserve participant agency. It can suggest, summarize, and surface consequences; it should not silently decide what a participant's character believes or chooses.

## Proposed data concepts

Keep the first model small and explicit:

- **Experience:** a tool or environment being built.
- **DesignDecision:** a recorded decision with rationale and status.
- **World:** a fictional continuity boundary.
- **Era:** a named span or historical transformation.
- **Place:** a world, system, settlement, habitat, or other location.
- **Entity:** a person, group, institution, culture, or other actor.
- **HistoricalClaim:** a statement about the past, including provenance and confidence.
- **Event:** something that happened or is proposed to have happened.
- **Campaign:** an ongoing participatory narrative context.
- **ParticipantAction:** an action submitted by a participant.
- **ContinuityIssue:** a conflict, gap, or unresolved dependency.
- **SourceRecord:** the evidence or originating perspective behind a claim.

These are conceptual vocabulary, not a commitment to a specific class hierarchy or serialized schema. Implementation should follow observed use cases and stay compatible with the Workshop's package boundaries.

## Stack alignment

- **FSM_API:** explicit, inspectable workflow/state behavior where a finite-state model is appropriate.
- **FSM_COS:** composition and coordination boundary; avoid making independent packages directly depend on each other without a deliberate architectural reason.
- **Serialization / bundle infrastructure:** preserve versioned records, stable identities, provenance, and portable Experience definitions.
- **GUI and GUI.SVG:** make the underlying model visible through living diagrams, chronologies, and campaign state displays that reflect real data.
- **WebApp / WebPage:** eventual accessible interface and showcase, after the domain model and first end-to-end workflow are proven.

Do not introduce dependencies merely to match this list. Use each component only where it provides a concrete capability.

## First vertical slice

Prove one complete path before building a giant lore database:

1. Create an Experience called StoryBuilder.
2. Record its purpose, boundary, and initial design decisions.
3. Create a World with one provisional era sequence.
4. Add a historical claim with source, confidence, and canon status.
5. Start a tiny campaign in one frontier location.
6. Record a participant choice and a resulting event.
7. Display the Experience structure, chronology, and campaign change using the same underlying data.

The slice is successful only when a change in the model is reflected in the visible output and can be traced to its source. No publishing or release is implied by this plan.

## Design constraint

StoryBuilder should help developers create Experiences inside the Workshop without requiring them to adopt the fictional setting. The science-fiction setting is a compelling first use case and a diegetic presentation layer—not a hard dependency of the tool itself.
