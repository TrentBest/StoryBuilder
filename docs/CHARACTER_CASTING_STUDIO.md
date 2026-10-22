# Character Casting Studio

## Purpose

Characters should feel like a cast the creator can discover, audition, revise, and revisit—not a form the creator must complete before writing. The Casting Studio organizes characters into **Hero**, **Neutral**, and **Antagonist** perspectives while allowing the creator to leave a role undecided or change it as the story evolves.

These are story-function groupings, not moral judgments. A hero can act badly; an antagonist can have a legitimate grievance; a neutral character can become central. Keep a character's current role, the author's interpretation, and other characters' opinions distinct.

## The casting-room metaphor

The creator enters a casting studio and can:
- Browse the cast by role, story, series, timeline, scene, or status.
- Create a character from a name, image/reference, a few words, a role, or a free-form description.
- Compare a character's current profile with proposed revisions.
- Explore how the character is described and where they appear.
- Open a private “changing booth” to revise appearance, costume, traits, voice, role, and other authored details.
- Return to the three-sided mirror to inspect the updated character in context.

The **three-sided mirror** is a visual design direction: one view presents the current character, another shows the relevant source text and appearances, and another compares the proposed revision with its prior state. The “booth” is an editing workspace, not a claim that photorealistic or 3D avatar rendering exists. A useful first implementation can use a profile card, source-context panel, and before/after comparison.

## Character identity and appearance

Keep stable identity separate from editable descriptions. A change to a name, pronoun, costume, or physical trait must not silently create a new character or rewrite every historical statement as if it had always been true.

Track authored facets independently where useful:
- Name and aliases.
- Current narrative role and role history.
- Pronouns and grammatical references, with context-sensitive interpretation.
- Traits, motivations, fears, skills, relationships, affiliations, and secrets.
- Physical description, costume, equipment, and appearance by era/scene.
- Voice, mannerisms, dialogue patterns, and point of view.
- Sources and attribution: narrator, self-description, another character, author note, or external reference.
- Canon status and revision history.

A character may have multiple outfits, forms, disguises, ages, or alternate-continuity appearances. The story should refer to the appropriate version in each scene without losing the character's stable identity.

## Word and reference observability

The author should be able to inspect every word associated with a character and the contexts in which those words occur. This includes:
- Direct name and alias mentions.
- Explicit descriptions and trait records.
- Paragraphs, dialogue, scene beats, notes, claims, and other indexed text.
- Pronouns and other references that may refer to the character.
- Synonyms and related concepts, without erasing the exact surface wording.
- Attribution: who used the words, where, and in what narrative context.

A raw count of “he,” “she,” “they,” or a name is not the same as a count of references to a specific character. Pronoun resolution is ambiguous: two characters may share a pronoun, a sentence may contain multiple possible antecedents, and a narrator may be unreliable. Therefore:

1. Preserve the original occurrence and its location.
2. Store a resolved character reference only when there is sufficient evidence or an explicit author decision.
3. Label automated resolution with confidence and method; unresolved references remain unresolved.
4. Allow the author to confirm or correct the antecedent.
5. Recompute counts and context links when source text, identity mappings, or synonym mappings change.
6. Distinguish confirmed mentions, likely references, and unresolved candidates in both counts and visualization.

Do not silently assign every pronoun to the nearest character. The goal is trustworthy observability, not a deceptively precise number.

## Revision and continuity safety

A character edit should be previewable before it changes accepted story material. Show:
- The current value and proposed value.
- Which scenes, paragraphs, relationships, and events may be affected.
- Whether the edit changes future depiction only, updates a canonical fact, or requests a controlled revision of existing prose.
- Any contradictions or ambiguous references introduced by the change.
- The ability to accept, revise, or discard the proposal.

A costume change should not rewrite personality. A pronoun change should update the character's authored reference preferences, but historical quoted speech and intentionally outdated narration may need to remain unchanged. A role change should not recategorize all past behavior without an explicit continuity decision.

Use the existing StoryBuilder review lifecycle where applicable: drafts and proposals are distinct from accepted canon. The UI must not bypass the domain's provenance or lifecycle rules.

## Living visual model

The mirror and cast board should be projections of actual StoryBuilder records:
- Hero / Neutral / Antagonist columns with an “Undecided” or “Other” lane.
- Character cards showing only known/accepted facets separately from proposed ones.
- A context view listing occurrences and opening the exact source paragraph or beat.
- Reference counts split into confirmed, likely, and unresolved.
- A relationship view that makes direction and source explicit.
- A revision comparison showing affected text and continuity warnings.

Do not present a static illustration as live character state. GUI.SVG may supply a living diagram where it adds value, while the host remains responsible for interactive controls and accessibility.

## Initial implementation sequence

1. Extend the existing Casting view with explicit narrative role groupings and a safe undecided state.
2. Make occurrence results navigable to their exact source text, including scene beats and paragraphs.
3. Separate exact lexical matches, explicit synonyms, and character-reference candidates in the vocabulary/index model.
4. Add an author-confirmed reference assignment and tests for ambiguous pronouns.
5. Build a three-panel “mirror” from the real profile, source context, and revision diff.
6. Add revision impact previews and continuity checks before applying changes to accepted canon.
7. Only then consider richer avatar rendering or a 3D booth, based on demonstrated value and available capabilities.

## Acceptance criteria

- Characters can be created at any time with minimal required input.
- Role groupings are editable, non-exclusive in interpretation, and never treated as moral labels.
- Every indexed occurrence preserves its exact text and source location.
- Pronoun counts distinguish lexical counts from references to a particular character.
- Ambiguous references are not silently resolved as facts.
- Character edits preview affected content and preserve revision history.
- The mirror reflects the real domain model and is accessible without a 3D renderer.
- Sensitive or sexualized appearance details are not required to create a compelling character; the experience keeps story purpose and user intent central.

## Status

This is a design specification for the next Casting evolution. It does not claim that pronoun coreference, impact previews, or the three-sided mirror are fully implemented.
