# StoryBuilderSession

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

StoryBuilderSession coordinates common authoring operations. Creating or editing a character or scene updates the workspace and refreshes the vocabulary index for that record, so searches reflect revisions rather than stale text.

## UML class card

See [StoryBuilderSession UML](../../../docs/UML/StoryBuilderSessionUML.md) for the class diagram and use case.

## Use case

Create a session for one workspace. Use CreateCharacter or CreateScene when a user opens Casting or the Stage. UpdateCharacter and UpdateScene replace edited records and re-index their word occurrences. Historical claims, events, sources, continuity notes, campaigns, and participant actions can also be added through the session. The vocabulary index retains context and attribution for those records too.

## Boundary

This first session is in-memory and local to one workspace. Durable storage, concurrent multi-user editing, automatic adjective classification, and language-aware indexing are future work.
