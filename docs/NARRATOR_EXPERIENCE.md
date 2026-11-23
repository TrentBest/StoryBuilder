# The Narrator Experience

## Purpose

The Narrator is StoryBuilder's personable guide, creative collaborator, and voice. The central interaction is not a blank text box or a command console. It is a low-friction conversation that helps the creator express an idea, see it take shape, and revise the result without having to do all the writing.

A book is an experience. StoryBuilder therefore helps create the underlying story and its structure, not manufacture or physically distribute the finished object. It can prepare a publication-ready digital artifact with the chosen output format, page size, margins, and layout constraints. Printing, binding, inventory, shipping, and physical fulfillment belong to external providers. A future Workshop-mediated referral or transaction may connect users to those providers, but it must remain a separate, explicit service boundary.

## Runtime agent boundary

The current WebDemo routes premise capture through NarratorAgent, which coordinates five independent FSM_API machines: intent, planning, observer constraint, portrayal preparation, and draft/review. The Home page exposes the intent, planning, observer, and draft/review states; portrayal preparation has its own state and is currently surfaced through the agent API and tests rather than the Home status strip. The timeline page uses the planning FSM when adding a provisional point. Draft submission accepts explicitly declared event claims, checks them against accepted observer knowledge, and holds unsupported claims for review.

This is orchestration infrastructure, not a claim that an LLM drafting provider or Renderer adapter is already connected. The domain records remain authoritative, and advancing a machine does not automatically accept canon.


## What narration means in the current build

There are three distinct capabilities, and they should not be conflated:

- **Scene assembly:** `SceneComposer` deterministically turns recorded `SceneBeat` fields into an editable first-pass text draft. It preserves the sequence and supplied details; it does not add motive, sensory detail, physical outcomes, or connective invention.
- **Narrative permission:** `NarratorAgent` can validate explicitly declared event claims against accepted observer knowledge, including the required framing for witnessed facts, reports, inference, and belief. This is a safety/continuity gate, not a prose-generation step.
- **Creative drafting and performance:** an LLM-backed drafting provider, revision dialogue, voice performance, and an author-facing narration workspace are not yet connected. The current Narrator captures and reviews ideas; it does not yet independently turn a story model into polished, stylistically controlled prose or read it aloud.

The next narration increment should preserve this separation: a provider-neutral request built from selected scene beats, manuscript context, author-selected voice/style/point of view, and an observer-safe knowledge projection; a provider returns a proposed draft with source-beat/event provenance; StoryBuilder validates declared claims and presents a diff for author review. Provider output must never write canon or overwrite manuscript text without explicit author action. StoryBuilder.Core should remain provider-independent, and the host should discover optional drafting and speech capabilities through the established composition boundary.

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


## Implementation checkpoint — 2026-10-22

The WebDemo now opens with a Narrator-first interaction. The creator can submit one free-form idea, optionally add scale/genre/approach/target hints, receive an editable StoryPremise proposal with a stable ID, revise it, and accept or reopen it through the FSM_API lifecycle. Premise words enter the shared vocabulary index and the binary persistence schema preserves them.

This is the first low-friction slice, not the full conversational Narrator described above. It does not yet generate an independent LLM interpretation, draft linked characters/events, read the story aloud, or produce publication-ready layouts. The app structures the creator's actual input rather than pretending a template is generative intelligence.


## Provider-neutral drafting contract — 2026-11-23

The Core model now defines a provider-neutral request/proposal boundary in `NarrationDraftContracts.cs`:

- A request carries the selected scene identity and beat snapshots, manuscript/section context, existing section text, author-selected perspective, voice/style hints, and (when constrained) an observer-safe knowledge projection with an explicit event horizon.
- A proposal carries text, provider/model attribution, source-linked text ranges, and explicit event claims. It is a proposal only; it does not save manuscript text or accept canon.
- The validator rejects mismatched request identities, empty proposals, out-of-range text spans, unknown beat references, event provenance not associated with the cited beats, claims outside the supplied event set, and claims not permitted by the supplied observer projection.
- Omniscient perspective requires explicit author permission. Observer-constrained requests must carry the matching observer projection and event horizon.

This is a contract and validation slice, not a connected concrete drafting provider, author-facing diff, or creative generation feature. The workspace-backed request builder and optional host provider coordinator are now implemented; the host still needs a concrete provider registration, proposal/diff presentation, and explicit author acceptance through the existing manuscript save gate. Tests have been added for provenance boundaries; execution is verified by CI below.


## Workspace-backed narration request builder — 2026-11-23

The Core service `NarrationDraftRequestBuilder` now constructs the provider-neutral request from the live `StoryBuilderSession` workspace:

- The caller explicitly selects the scene, beat IDs, manuscript, and section. The builder rejects missing or duplicate beat selections and unknown scene/manuscript/section identities.
- Selected beats are snapshotted in scene sequence order, regardless of the order in which their IDs were supplied. The request includes the section's existing text and the scene/manuscript labels needed for drafting context.
- Optional voice and style hints are normalized without changing the author's source records.
- When an observer is selected, the builder creates the matching observer-safe projection and refuses to expose a selected beat unless every event linked to that beat is present in the projection. Unlinked beats are rejected for observer-constrained requests rather than being passed through as potentially hidden information.
- The request carries the same event horizon as the projection. An event horizon without an observer is rejected, and omniscient perspective still requires explicit author permission.

The builder passes only selected beat snapshots and the observer projection to the provider boundary; it does not pass the whole workspace, invoke a provider, persist a proposal, or accept manuscript changes. The existing `SaveManuscriptDraft` remains the author-acceptance write path.

The proposal validator also now fails closed on malformed deserialized provenance collections, empty identities, and invalid claim modes. Every non-whitespace character in a proposal must be covered by a source-beat provenance segment; providers cannot submit ungrounded prose by omitting segments. Tests cover selected-beat ordering, manuscript context, foreign-beat rejection, observer-projection gating, unlinked-beat rejection, and duplicate selections. Test execution remains unverified until CI or a local .NET run reports a result.


## Optional provider host boundary — 2026-11-23

The composition/host adapter now defines `INarrationDraftProvider` and `NarrationDraftCoordinator`:

- A host may supply an implementation without adding a concrete provider dependency to StoryBuilder.Core.
- The coordinator builds the bounded request, passes cancellation to the provider, stamps or verifies provider attribution, and validates the returned proposal against the exact request.
- The result is a `NarrationDraftReview` containing the request and validated proposal. It remains untrusted, non-canonical content.
- The coordinator never writes to the workspace. A provider cannot accept canon by returning text.

No concrete provider is currently registered by the WebDemo, and no proposal diff/acceptance UI is connected yet. The next slice is to register a real optional provider and present its validated proposal for comparison and author editing, while preserving the existing explicit save gate.



## Author acceptance and persisted narration provenance — 2026-11-23

Validated proposals now have an explicit acceptance operation: `StoryBuilderSession.AcceptNarrationDraft(request, proposal, author)`.

- Acceptance re-runs proposal validation and verifies the manuscript, section, scene, referenced events, and current source-beat snapshots before mutation. If selected beat content, attribution, identity, sequence, or event links changed after drafting, the proposal is rejected as stale.
- A successful acceptance updates section text, adds referenced event links, and appends one immutable `NarrationAcceptanceRecord` in the same manuscript/workspace replacement.
- Each record preserves exact accepted text, request/scene IDs, author, provider/model attribution, acceptance timestamp, source-beat text ranges, and event claims. Later edits to the section body do not erase earlier acceptance records.
- Provider proposals remain non-canonical until this operation is called. The coordinator still does not accept or persist anything automatically.

This is persistence at the StoryBuilder domain-model/workspace serialization boundary. It does not claim a database transaction, concurrent-write isolation, or a connected durable database adapter. No concrete provider is registered in WebDemo. The Stage page now exposes a manual author-authored acceptance path that calls this operation; provider-generated proposal comparison and fine-grained editing remain future work.


## Verification — 2026-11-23

The previous verified baseline is GitHub Actions run [#1234](https://github.com/TrentBest/StoryBuilder/actions/runs/45272789664), which passed 180 Core tests, 21 Story Forge tests, 2 WebDemo browser tests, and all configured project builds. The acceptance/provenance changes above are newer and still require a fresh CI result.


## WebDemo author acceptance surface — 2026-11-23

The Stage page now uses the workspace-backed `NarrationDraftRequestBuilder` when preparing a manuscript passage. It snapshots the selected scene's current beats in sequence order and applies observer projection checks before the editable draft is presented. The page no longer routes this passage through the older `NarratorAgent.SubmitDraft` save path.

The author can edit the prepared text and explicitly choose **Accept this author-written passage**. The UI constructs a proposal with the exact request ID, identifies the source beats and linked events for the passage, and calls `StoryBuilderSession.AcceptNarrationDraft`. That operation revalidates provenance and staleness before updating the manuscript and appending the acceptance record. A stale or invalid proposal remains unaccepted and the UI reports the reason.

This first UI slice uses a single coarse provenance segment for the entire passage, linked to all selected beats and their event IDs. It deliberately records no semantic event claims because the WebDemo does not infer claims from prose. Fine-grained sentence-to-beat mapping and a concrete optional creative provider remain future work. The acceptance record identifies this as author-authored text (`author` / `manual-authoring`), not AI output.

The UI is therefore an explicit manual authoring/acceptance path; it does not imply that a model provider is configured or that generated prose is available.


## Fine-grained author provenance editor — 2026-11-23

The Stage's manual authoring surface now prepares one editable text block per selected source beat. The deterministic first-pass line for each beat comes from `SceneComposer.ComposeNarrationBeat`, the same operation used by whole-scene narration assembly.

When the author accepts the passage, the host joins non-empty blocks with paragraph separators and creates one provenance segment per block. Each segment cites exactly its own beat and only the world-event IDs linked to that beat. Empty blocks are omitted; a passage with no non-empty blocks cannot be accepted. The existing proposal validator still checks full non-whitespace text coverage, beat/event association, observer permissions for explicit claims, and request identity. The session still rechecks the source snapshot before its single workspace replacement.

This improves attribution granularity without pretending to infer sentence-level semantics: an author can edit the text inside a beat's block, but the UI does not automatically decide which clause asserts which event. It remains manual authoring (`author` / `manual-authoring`), with no concrete creative provider registered.
