# The Narrator Experience

## Purpose

The Narrator is StoryBuilder's personable guide, creative collaborator, and voice. The central interaction is not a blank text box or a command console. It is a low-friction conversation that helps the creator express an idea, see it take shape, and revise the result without having to do all the writing.

A book is an experience. StoryBuilder therefore helps create the underlying story and its structure, not manufacture or physically distribute the finished object. It can prepare a publication-ready digital artifact with the chosen output format, page size, margins, and layout constraints. Printing, binding, inventory, shipping, and physical fulfillment belong to external providers. A future Workshop-mediated referral or transaction may connect users to those providers, but it must remain a separate, explicit service boundary.

## The Narrator's opening

> Hello! I'm your Narrator.
>
> If you'd like me to read your story aloud, I'm here for that. I can also help you create it. My job is to help you *see* your story, not make you do all the writing. I'll draft, organize, and illustrate the possibilities; you stay in control of what becomes yours.
>
> In computing, people call this WYSIWYG: “what you see is what you get.” Here, it's that idea for stories and experiences: what we change in the story model should become visible in the story we're shaping.
>
> Let's start with the broadest strokes. Do you already know the genre? Is this one story, a series, several connected series, or a larger universe with reboots, alternate continuities, and overlapping timelines?
>
> Or just tell me your vision in your own words. A giant peach? A city that remembers everyone who ever lived there? A detective who can only investigate events that haven't happened yet? You don't need the right terminology. We'll discover the shape together.
>
> And one more thing: how do you like to create? We can start with an outline or timeline, build a character, explore a setting, follow a single “what if?”, or simply talk until an idea emerges. I'll adapt to the way you work.

This copy is a product direction and may be revised through user testing. The Narrator should introduce the experience warmly without forcing the user through a long questionnaire.

## First-session conversation

The first session should gather only enough information to choose a useful next view. The Narrator may ask one question at a time and accept partial answers.

1. **Vision:** free-form description, image/voice input where supported, or a starter prompt.
2. **Scale:** standalone work, series, connected series, shared universe, or “not sure yet.”
3. **Genre and tone:** known choices, mixed genre, or “help me discover it.”
4. **Creation approach:** timeline/outline first, character first, setting first, central concept first, or conversational discovery.
5. **Primary target experience:** book, audio/story performance, film/screen, stage play, game, tabletop/board game, interactive web experience, or undecided.
6. **Next visible artifact:** a premise card, story map, timeline, cast board, setting view, or short narrated scene.

Do not require the user to settle every category before seeing something useful. Offer a sensible first artifact after the first meaningful idea, then ask what they want to change.

## See it, then shape it

The Narrator's main loop is:

1. Listen to the user's words and preserve the original intent.
2. Extract a small, inspectable set of proposed story elements.
3. Render those elements as a living visual manifestation of the actual workspace model.
4. Read back a short summary of what the Narrator believes the user means.
5. Invite a concrete correction, addition, deletion, or alternative.
6. Show the resulting change in both the model and its visual representation.
7. Keep proposed content distinct from accepted canon until the creator accepts it.

The creator can work by conversation, outline, chronology, cast, setting, scene, or direct editing. These are alternate doors into the same story model, not separate copies that drift apart. A generated paragraph is editable prose connected to structured story records, not the sole storage location for the story's meaning.

## Target platforms and output profiles

The creator's **primary target** helps StoryBuilder prioritize useful design questions; it must not lock the story into one medium. A single story model may support multiple output profiles when the relevant MicroBundles and transformation capabilities are available.

Candidate output profiles include:
- Book, novella, short story, and publication-ready page layout.
- Audiobook, narrated reading, and musical adaptation.
- Screenplay and screen adaptation.
- Stage play and performance script.
- Video game, branching interactive narrative, and other digital experiences.
- Tabletop role-playing game, board game, and campaign materials.
- Web-native or mixed-media experience.

These are discovery targets, not claims that every export pipeline already exists. Query the MicroBundle repository/catalog for the actual available output capabilities, versions, dependencies, and host requirements. Show only supported targets as ready; represent unavailable targets as future possibilities, not working buttons. Keep target discovery separate from commerce or promotional upselling.

## Publication boundary

StoryBuilder owns the authored story model and can prepare digital output to explicit constraints such as trim/page size, margins, pagination, typography, and selected export format, to the extent supported by available composition/export capabilities.

StoryBuilder does not own printing machinery, binding, physical stock, warehousing, or delivery. A user should receive the digital artifact needed by their chosen production provider. Any later facilitated printing purchase and revenue share must be an opt-in integration with clear pricing, provider identity, fulfillment responsibility, and transaction consent. Do not make physical fulfillment a prerequisite for creating or exporting a story.

## Workshop integration

- **StoryBuilder.Core** owns story meaning, stable identities, relationships, provenance, and authoring operations.
- **FSM_API** may govern explicit lifecycle transitions where a finite-state model fits; it does not own the story domain.
- **FSM_COS** composes declared capabilities and coordinates their composition boundary; it is not the UI or story engine.
- **MicroBundleDomain** defines the neutral capability contract; the MicroBundle repository/catalog is queried to discover available output and authoring capabilities.
- **GUI / GUI.SVG** provide interactive authoring views and living visualizations backed by actual workspace data, not static mockups.
- **WebApp / WebPage** are the intended browser-host integration; the local Blazor demo remains a proving ground until the shared host path is verified.
- **AnyApp** is the corresponding native application manifestation. The story domain must not depend on a particular host.
- **FSM_Serialization** remains the persistence contract boundary; persisted records need stable identities and explicit schema/version handling.

The core model must remain host-neutral. Keep adapters optional and dependencies deliberate. Do not introduce a direct StoryBuilder-to-FSM_COS dependency just to discover bundles; use the established host/composition contracts.

## Acceptance principles

- The user can begin with a fragment, not a completed brief.
- The Narrator asks only the next useful question and accepts “I don't know.”
- The user sees a visual result early and can revise it directly or conversationally.
- Generated prose and inferred facts remain proposals until accepted.
- Every visible cast, timeline, relationship, and usage count comes from the same underlying workspace records.
- The user can inspect and correct the Narrator's interpretation.
- A target output is labelled available only when its capability has been discovered and verified.
- Digital output preparation is in scope; physical production is not.
- No package publishing, release, deployment, or external purchase is authorized by this design document.

## First implementation slice

Build the first Narrator flow on top of the existing authoring session rather than inventing a parallel story store:

1. Capture one free-form concept.
2. Propose a premise and a small set of entities/events with stable IDs and provenance.
3. Display a living premise/story map.
4. Let the user accept or revise each proposal.
5. Ask whether to explore a character, setting, chronology, or target medium next.
6. Record the target as a preference and query the host-provided capability catalog for supported output profiles.

Voice playback, speech input, multi-output transformation pipelines, cloud collaboration, and commercial print-provider integrations are subsequent milestones. The first slice should work with keyboard interaction and the existing local host.

## Status

This document records intended product behavior and architecture. It is not evidence that voice, target discovery, export pipelines, or the integrated WebPage/AnyApp experience are already implemented.
