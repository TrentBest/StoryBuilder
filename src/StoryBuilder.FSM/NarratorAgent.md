# NarratorAgent

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

Coordinates four independent FSM_API machines for intent, timeline planning, observer constraints, and draft review. Each machine has its own definition, processing group, context, and handle; StoryBuilderSession remains the canonical domain source.

## UML class card

See [NarratorAgent UML](../../docs/UML/NarratorAgentUML.md).

## Observer projection

The observer FSM can now project accepted knowledge at a selected event anchor. The projection contains only event identities and accepted knowledge notes, attribution, source, confidence, and permitted narration modes. It deliberately excludes unrestricted world-event titles, summaries, and causal edges. Proposed or causally unavailable knowledge is omitted.

## Use cases

- Capture a premise without turning a rough idea into canon.
- Add a provisional Plot Twist to a timeline and continue planning.
- Select an observer and evaluate a specific claim.
- Project the observer's accepted knowledge for a downstream narrator or portrayal provider.
- Hold a draft for review when its declared claims exceed accepted observer knowledge.

## Boundary

The Narrator does not contain an LLM or renderer. Renderer chooses observer-relative visual/semantic representation; StoryBuilder decides what the observer can legitimately claim to know. These policies cooperate but are not interchangeable.
