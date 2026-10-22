# VocabularySpanTests

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

Verifies that each indexed word retains its original surface form plus exact start offset and length in the authored source text. This makes usage searchable and navigable without rewriting the source text or guessing where a word came from.

## Boundary

The index records exact lexical spans. Pronoun coreference and alias resolution remain separate, explicit workflows; ambiguous references are not automatically assigned to a character.
