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

Vocabulary is a shared story-domain service. The first implementation supports explicit synonym mappings and preserves the exact surface word at each occurrence. Automatic language inference and thesaurus generation are future capabilities, not current claims.

## Game-system boundary

D&D/GURPS-style attributes, skill systems, point budgets, species lists, and combat rules should be optional rule sets layered over the genre-neutral character profile. The core should not force one game system onto every Experience.

## First implementation

- CharacterProfile: editable record with stable identity, role, description, traits, relationships, notes, and status.
- CharacterBuilder: low-friction draft creation and optional templates.
- VocabularyIndex: synonym-aware search over explicitly recorded occurrences.

The domain types are implemented in the C# core; a clickable Casting interface and persistent storage remain future integration work.
