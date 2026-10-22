# VocabularyIndex

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

VocabularyIndex supports story-wide word usage analysis. It maps explicitly declared synonyms to a canonical concept, indexes occurrences, and reports counts and contexts without erasing the author's surface wording.

## UML class card

See [VocabularyIndex UML](../../../docs/UML/VocabularyIndexUML.md) for the class diagram and use case.

## Use case

Record a term directly, or pass text to RecordText to tokenize its words and preserve context, target, field, and attribution. Search by a canonical term or known synonym. RemoveTarget supports re-indexing one edited character or scene without retaining stale occurrences.

## Boundary

Synonym mapping is explicit and editable; the current core does not attempt automatic thesaurus generation, morphological analysis, or semantic inference. Text tokenization is intentionally simple, and it does not classify every prose word as an adjective. The index is in-memory and synchronous.
