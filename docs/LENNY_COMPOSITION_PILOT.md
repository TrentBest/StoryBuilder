# Little Lenny — StoryBuilder Composition Pilot

**Purpose:** make *Little Lenny and the Big Ocean Problem* the first complete, author-respecting composition-to-publication pilot for StoryBuilder.

This is an implementation plan grounded in the current Lenny manuscript and the current StoryBuilder domain. It is not a claim that the illustration, animation, or GitHub Pages pipeline is already complete.

## Product boundary

- **The author owns the story.** The supplied manuscript, dedication, closing statement, and page order are source material. StoryBuilder may organize and present them; it must not silently rewrite them.
- **StoryBuilder owns composition.** It holds the structured book, cast, scenes, visual decisions, provenance, and publication editions.
- **The illustration process creates assets.** Generated artwork is a proposal until reviewed and accepted. The author remains the authority on whether a composition represents her intent.
- **The Experiences repository owns the reader.** The published Lenny Experience remains independently runnable and must not require StoryBuilder at runtime.
- **GitHub Pages hosts the public reading edition.** The deployment consumes a frozen, validated publication artifact; it does not read a mutable authoring workspace or expose private drafts.

## Existing StoryBuilder capabilities to reuse

The current development branch already provides the important foundation:

- **StoryWorkspace** and **StoryBuilderSession** as the canonical authoring workspace and command surface.
- Stable character, scene, beat, timeline, manuscript, source, and publication identities.
- Casting and attributed trait/word records.
- Scene capture as ordered action, reaction, dialogue, and pose beats.
- Canonical events distinct from scene beats and manuscript portrayals.
- **ManuscriptEdition** with ordered sections and explicit links to scene/event identities.
- Deterministic Markdown book rendering.
- Immutable **PublishedBookEdition** snapshots with SHA-256 integrity and correction lineage.
- Versioned binary workspace persistence and a local Blazor authoring workbench.
- GUI.SVG living views over real workspace records.

Do not reintroduce the earlier minimal StoryProject model as a second active authoring model. The current workspace/session contract is the source of truth. The legacy JSON importer is a migration bridge, not the future live format.

## Lenny workspace mapping

Use stable IDs and links rather than copying story facts into disconnected documents.

| Lenny source concept | StoryBuilder representation | Rule |
| --- | --- | --- |
| Lenny, Dad, lead dolphin, dolphin pod, sea turtles, fish, beachgoers | Character records | Preserve the manuscript's relationships and roles. Unspecified appearance remains a proposal. |
| Beach, shoreline, open water, ocean floor, coral reef | Place records | Keep physical places distinct; link scenes to canonical place IDs. |
| Each of the 33 printed story pages | Ordered manuscript sections | Preserve page number, exact source wording, and reading order. |
| Page-specific visual action | Scene records and ordered SceneBeat records | Break a page into compositional beats only when useful; do not create new plot events. |
| Story events that recur across pages | Canonical timeline events | One event may be portrayed by multiple scene beats and manuscript sections. |
| Author manuscript and author-approved visual references | Source records / provenance | Record source text, author feedback, asset revision, and which approved reference supports each design decision. Exclude the informal concept illustrations. |
| Character turnaround, environment guide, page illustration | Visual asset records (to be added) | Stable asset ID, revision, role, source, approval state, and file path. |
| Camera, movement, expression, transition, timing | Animation/gesture track (to be added) | Keep animation as portrayal of an existing scene, not a new story event. |
| Public reading edition | Immutable publication edition | Publish only the reviewed snapshot and its referenced assets. |

### Canon and visual interpretation

Keep three explicit states visible in the authoring UI:

1. **Manuscript fact** — directly established by the source text or author-approved reference.
2. **Composition proposal** — framing, pose, expression, palette, visual metaphor, or generated image offered to communicate a manuscript fact.
3. **Author-approved design** — a proposal the author has accepted for reuse as continuity guidance.

For example, the manuscript establishes that Lenny is a small, young boy who carries a bucket and shovel. It does not establish his hair, eyes, skin tone, exact age, or clothes. An image generator may need a consistent provisional design to make a coherent turnaround, but StoryBuilder must retain that design as provisional until the author approves it.

Approval should be attached to a specific asset revision and its source/provenance. Replacing an image creates a new revision; it must not erase the previous approved publication snapshot.

## Visual production pipeline

The first art pass should be deliberately small:

1. **Character sheet:** Lenny, Dad, and the lead dolphin, with front/side/three-quarter views and a compact expression set.
2. **Environment guide:** beach, waterline/open water, and underwater reef/ocean floor.
3. **Composition tests:** a small set of representative pages covering curiosity (1–3), discouragement (9–10 or 16–18), dolphin reveal (21), underwater scale (26–30), and hopeful resolution (31–33).
4. **Continuity review:** compare proportions, props, shoreline geography, light, palette, and emotional staging across the tests.
5. **Author review:** accept, revise, or reject each design proposal before it becomes the reusable visual reference.
6. **Full illustration pass:** create the remaining page artwork from the approved character/environment references.
7. **Animation pass:** add restrained motion to selected layers and scene beats only after the still compositions work.

Generated images should use an original visual direction, not imitate a living artist or a named copyrighted style. Prompts and asset metadata should record the intended scene, source page, character IDs, visual decisions, and revision. Avoid embedding story text inside generated art; the reader should render the author's actual manuscript text as accessible HTML.

## Animation model: living picture, not noisy slideshow

The goal is an animated children's book, not a game and not motion for motion's sake.

The first animation vocabulary can remain small and declarative:

- **Ambient:** slow wave movement, drifting bubbles, soft underwater particles, gentle light.
- **Character gesture:** a glance, a turn, a small step, a dolphin bob, a fin wave.
- **Prop action:** water sloshing in the bucket, litter landing in the bag.
- **Transition:** a restrained page-to-page dissolve or camera move when it supports the story.

Each track should identify the page/scene, target asset or layer, gesture/action, start time, duration, and reduced-motion fallback. Motion must not obscure text, imply events absent from the manuscript, or make the reader difficult to use. The first public edition should remain beautiful and complete with motion disabled.

StoryBuilder's current SceneBeat pose/action vocabulary is a useful authoring source. A real Gestures adapter and renderer-backed animation playback are future integrations; do not claim that the current WebDemo already provides a claymation stage or full animation export.

## Reader and GitHub Pages publication

The Lenny Experience in TheSingularityWorkshop.Experiences is the reader product. It should consume a versioned, static publication bundle with:

- Exact manuscript text and stable page/section IDs.
- Page illustration references and descriptive alt text.
- Optional animation/gesture tracks with reduced-motion behavior.
- A publication edition ID, content hash, source/revision metadata, and asset manifest.
- A clear reading order, keyboard/touch navigation, accessible text, and responsive layout.

GitHub Pages should publish from GitHub Actions using the repository's supported static-site workflow. The workflow should build from a selected branch/ref, validate that every referenced asset exists, validate page count/order and required alt text, then upload only the public reader bundle. It must not publish StoryBuilder workspace files, local storage snapshots, prompts marked private, rejected art, or unreviewed drafts.

**Publication gate:** an author-approved immutable edition is the only input eligible for public deployment. A push to an authoring branch must not automatically turn unreviewed work into the public book. Deployment configuration can be prepared and tested as a preview artifact before enabling a public production release.

## Implementation sequence

### Slice 1 — Composition bridge (next)

- Create a Lenny StoryBuilder workspace from the current manuscript and visual plan.
- Preserve source manuscript and page numbering.
- Create the initial cast and place records with stable IDs.
- Create one manuscript section per printed story page, linked to its scene/beat records.
- Add source/provenance links to the author's manuscript transcription. Explicitly mark the earlier PDF illustrations as excluded concept material; do not index or use them as visual references or production assets.
- Verify save/restore, deterministic book output, and immutable publication hashing.

### Slice 2 — Visual asset provenance

- Add first-class visual asset and asset-revision records to StoryBuilder.
- Record prompt/source provenance, page/scene links, approval state, and continuity roles.
- Provide a review surface for accepting/rejecting a revision without losing history.
- Keep actual image bytes in the Experience's static asset tree or an explicit asset store; the workspace stores stable references and metadata.

### Slice 3 — Living page composition

- Add page-level composition data: text region, image region, focal point, crop/fit, and optional layered assets.
- Preview the same page model in the authoring surface and public reader.
- Validate contrast, text readability, alt text, and mobile layouts.

### Slice 4 — Restrained animation

- Define a host-neutral animation/gesture track contract.
- Connect to Gestures/Renderer only through discovered and verified capabilities.
- Implement reduced-motion and static fallbacks before enabling motion by default.
- Animate a few representative pages first; review before scaling to all 33.

### Slice 5 — Public edition

- Freeze an author-approved edition.
- Export a deterministic static bundle.
- Run link, asset, accessibility, content-integrity, and page-sequence checks.
- Publish the reader to GitHub Pages only after the author approves the production edition.

## Acceptance criteria

- The author's text and all 33 story pages remain in their original order.
- Every visual proposal is traceable to a page/scene and has an explicit review state.
- Unknown character traits are not silently promoted to canon.
- Revisions do not mutate prior immutable publication editions.
- The reader works without animation and with reduced motion enabled.
- Text remains selectable, accessible, and responsive; it is not baked into images.
- Public deployment contains only the approved edition and its required assets.
- StoryBuilder is not a runtime dependency of the Lenny reader.
- No NuGet publication, repository release, or production deployment occurs without its separate explicit authorization.

## Current status

The Lenny Experience has a 33-page structured manuscript, a working text-only reader, and an original visual plan. The earlier concept illustrations are explicitly excluded. New character and environment designs must be developed and approved through StoryBuilder. StoryBuilder's current development branch already has casting, scene capture, manuscript composition, deterministic book output, immutable publication snapshots, persistence, and a local authoring workbench.

The next engineering work is to create the Lenny workspace through those current APIs, then add the missing first-class visual asset/revision model. Animation and Pages publication follow after those foundations are verifiable.
