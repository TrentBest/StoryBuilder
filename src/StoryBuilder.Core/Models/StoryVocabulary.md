# StoryVocabulary

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

These records describe vocabulary as a domain-wide resource. The index can treat words such as *brave* and *fearless* as aliases for a shared concept while retaining each original word, where it appeared, who used it, and the context.

## UML class card

See [VocabularyIndex UML](../../../docs/UML/VocabularyIndexUML.md) for the class diagram and use case.

## Use case

Record an occurrence against a character, scene, place, faction, or other story record. Add synonyms deliberately. Search by the canonical term or a known synonym to see occurrences across the indexed domain.

## Limitations

Synonym mapping is explicit and editable; the current core does not attempt automatic thesaurus generation, morphological analysis, or semantic inference. This keeps a word's original use and attribution auditable instead of silently rewriting the author's language.
