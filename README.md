# StoryBuilder

**Build the Experience. Compose its history. Let the campaign make it live.**

StoryBuilder is a creative workbench for documenting the creation of an Experience or Tool and composing the backstory that gives it meaning. It supports worlds, characters, factions, places, timelines, lore, narrative rules, and campaigns whose participants contribute choices and consequences.

StoryBuilder is both:
- **An Experience/Tool design record** — what is being built, why it exists, how it behaves, and how its parts fit together.
- **A narrative composition system** — the fictional world, history, relationships, events, and continuity that make the resulting Experience meaningful.

These are related but distinct models. Implementation facts must not silently become fictional facts, and fictional claims must not be mistaken for verified implementation behavior.

## Vision

A participant should be able to enter a campaign with a coherent world and meaningful context, take actions, and help shape what happens next. StoryBuilder should preserve continuity and provenance as the story evolves.

## The first world

The initial setting begins close enough to our own time for its fractures to be recognizable, then follows their consequences across recovery, a new global order, Solar System settlement, interstellar expansion, and a far-future civilization of more worlds than any one book could list. Earth remains humanity's remembered origin, but its location is unknown in the era of the main story.

This is speculative alternate history, not a prediction. The premise and its current boundaries are recorded in [Story Foundation](docs/STORY_FOUNDATION.md); the nested chronology is outlined in [Historical Scale](docs/HISTORICAL_SCALE.md).

## Fit within The Singularity Workshop

StoryBuilder should reuse the Workshop's existing technology rather than create a parallel platform:

- **FSM_API** — model deterministic behavior and state transitions.
- **FSM_COS** — compose capabilities and bundles at the system boundary, preserving package independence.
- **GUI / GUI.SVG** — provide interactive authoring and living visual documentation.
- **WebApp / WebPage** — deliver the browser-based Experience when the core authoring flow is ready.
- **Serialization and bundle infrastructure** — preserve and exchange authored artifacts when their contracts are verified.

Dependencies should remain intentional. FSM_COS is the composition boundary; StoryBuilder should not tightly couple otherwise independent Workshop packages.

## Initial Experience

1. **Define an Experience** — name, purpose, audience, capabilities, constraints, and success criteria.
2. **Document its construction** — components, decisions, dependencies, behavior, and verification evidence.
3. **Create its world bible** — premise, setting, timeline, characters, factions, locations, rules, and unresolved mysteries.
4. **Compose a campaign** — objectives, scenes, events, participant roles, choices, and consequences.
5. **Run and record participation** — preserve events and decisions as campaign history.
6. **Reconcile continuity** — distinguish established canon, proposals, contradictions, and open questions.
7. **Visualize the result** — connect the written model to useful, living diagrams and interactive views.

The diegetic-tool concept is described in [StoryBuilder as a Diegetic Workshop Tool](docs/DIEGETIC_STORYBUILDER.md).

## Core principles

- Separate the **build record** from the **fictional canon**, while linking related facts explicitly.
- Track provenance: who or what introduced a fact, why it exists, and which events depend on it.
- Distinguish proposed content from accepted canon and observed campaign events.
- Preserve participant agency; authored backstory provides context, not a predetermined outcome for every choice.
- Keep the initial implementation small, testable, and dependency-conscious.
- Demonstrate real behavior in examples and documentation; do not substitute static mockups for working Experiences.

## Initial scope

Start with the domain model and one end-to-end authoring slice: create an Experience, document its purpose and components, compose a small world and campaign, and record a participant choice with its resulting event. Keep hosting, large-scale collaboration, and automated narrative generation behind explicit future milestones until the core model is proven.

## Development status

This repository is being established. The initial charter is a direction, not a claim that these capabilities are implemented. No package publishing or release is authorized by this document.

## License

License selection is pending.
