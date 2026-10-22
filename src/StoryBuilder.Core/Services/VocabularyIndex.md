# VocabularyIndex

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

VocabularyIndex supports story-wide word usage analysis. It maps explicitly declared synonyms to a canonical concept, indexes occurrences, and reports counts and contexts without erasing the author's surface wording.

## UML class card

See [VocabularyIndex UML](../../../docs/UML/VocabularyIndexUML.md) for the class diagram and use case.

## Use case

Index every word occurrence when a record changes, or record it as the user authors. Search by a canonical term or known synonym. Use Attribution and Context to distinguish a self-description from an insult, rumor, narrator statement, or author note.

## Boundary

This first version is in-memory and synchronous. Durable storage, incremental reindexing, language-aware synonym data, and UI visualization are separate integration milestones.
