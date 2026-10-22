# NarratorAgent

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

Coordinates five independent FSM_API machines rather than one monolithic FSM: intent capture, timeline planning, observer knowledge, portrayal preparation, and draft review. Each machine has its own definition, processing group, context, and handle; StoryBuilderSession remains the canonical domain source.

## UML class card

See [NarratorAgent UML](../../docs/UML/NarratorAgentUML.md).

## Separate observer and portrayal responsibilities

The **Observer** FSM selects an observer and tracks whether a specific claim is allowed or blocked. The **Portrayal** FSM independently tracks projection preparation: Idle → Requested → Ready or NeedsReview. A valid observer can still have no accepted knowledge at a chosen event anchor, so an empty projection requests review without corrupting the observer-selection state.

The projection contains only accepted knowledge notes, attribution, source, confidence, and permitted narration modes. It deliberately excludes unrestricted world-event titles, summaries, and causal edges. Proposed or causally unavailable knowledge is omitted.

## Use cases

- Capture a premise without turning a rough idea into canon.
- Add a provisional Plot Twist to a timeline and continue planning.
- Select an observer and evaluate a specific claim.
- Prepare an observer-limited portrayal context using a dedicated FSM.
- Hold a draft for review when its declared claims exceed accepted observer knowledge.

## Boundary

The Narrator does not contain an LLM or renderer. Renderer chooses observer-relative visual/semantic representation; StoryBuilder decides what the observer can legitimately claim to know. These policies cooperate but are not interchangeable.
