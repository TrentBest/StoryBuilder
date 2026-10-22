# Character Creation: Casting

## Goal

Make a new character feel like opening a door, not filling out a tax return. The first useful state is an editable draft. Biography, game statistics, and detailed backstory remain optional.

## Proposed user flow

1. The user opens **Casting**.
2. They may choose a starting pattern such as Unspecified, Adventurer, Investigator, Antagonist, or Witness.
3. The tool creates a draft immediately. It may offer prompts and trait suggestions without applying them automatically.
4. The user can enter a name, a phrase, a few adjectives, or a complete biography.
5. Every field can be revised later. Stable identity is separate from the current wording.
6. Traits may record intensity, context, status, and attribution (self-description, another character's opinion, narrator, or author).

## Defaults and synonyms

Defaults are useful only when they reduce work without smuggling in assumptions. A template provides optional questions and suggestions; it does not silently decide personality or canon. The user can accept, ignore, rename, or replace every suggestion.

StoryBuilderSession keeps the vocabulary index synchronized when characters are created or edited. The exact wording is preserved as an occurrence; explicit synonyms make related terms searchable without silently replacing the author's language.

## Game-system boundary

D&D/GURPS-style attributes, skill systems, point budgets, species lists, and combat rules should be optional rule sets layered over the genre-neutral character profile. The core should not force one game system onto every Experience.

## First implementation

- CharacterProfile: editable record with stable identity, role, description, traits, relationships, notes, and status.
- CharacterBuilder: low-friction draft creation and optional templates.
- StoryBuilderSession: coordinates workspace edits and vocabulary re-indexing.
- VocabularyIndex: synonym-aware search over explicitly recorded and session-indexed occurrences.

The domain types are implemented in the C# core; a clickable Casting interface and persistent storage remain future integration work.
