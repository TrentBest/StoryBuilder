# VocabularyIndex UML

## Class diagram

```mermaid
classDiagram
class VocabularyIndex {
  +AddSynonym(canonicalTerm, synonym, category) VocabularyConcept
  +Record(surfaceForm, category, targetId, targetLabel, field, context, attribution, sourceId, status) WordOccurrence
  +Search(query) IReadOnlyList~VocabularyUsage~
  +GetMostUsed(take) IReadOnlyList~VocabularyUsage~
}
class WordOccurrence {
  +string SurfaceForm
  +string CanonicalTerm
  +Guid TargetId
  +string Attribution
  +string? Context
}
class VocabularyConcept {
  +string CanonicalTerm
  +List~string~ Synonyms
}
VocabularyIndex o-- WordOccurrence
VocabularyIndex o-- VocabularyConcept
```

## Use case

A writer adds an adjective once, then discovers its uses across the story domain. Synonym search groups related terms while preserving the actual words and the speaker or source that used them.
