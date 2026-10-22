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

- **AnyApp** — first host: launch StoryBuilder as a local Windows desktop Experience without requiring a hosted platform.
- **FSM_API** — model deterministic state transitions and workflow behavior where appropriate.
- **FSM_COS** — compose capabilities at the system boundary, preserving package independence.
- **MicroBundle infrastructure** — define, discover, and compose capabilities using verified contracts.
- **GUI / GUI.WPF / GUI.SVG** — provide authoring surfaces and living visual documentation.
- **Renderer / Visualize** — later portray the same structured story data textually and visually.
- **Serialization** — use existing representation contracts where they fit; keep local project persistence explicit and testable.

The desktop deployment decision is documented in [Deployment Decision](docs/DEPLOYMENT_DECISION.md). A browser host can be considered later, but hosted WebApp deployment is not a prerequisite for the initial authoring workflow.

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
- Prefer local-first authoring; a network service must not be required to write or reopen a project.

## Initial scope

The first runnable slice should launch through AnyApp, create and reopen a local project, edit its title/premise/manuscript, record a stable-ID event linked to a passage, and show that event in a chronology view. This proves the shared model and persistence before adding larger collaboration features or automated narrative generation.

## Development status

The repository is being established. The charter and deployment decision define direction, not a claim that the application, timeline engine, campaign runtime, or living visualizations have been implemented. No package publishing or release is authorized.

See [Gallery](docs/Gallery.md) for the current proof target and implementation evidence.

## License

License selection is pending.
