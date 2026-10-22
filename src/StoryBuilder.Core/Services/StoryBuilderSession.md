# StoryBuilderSession

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

StoryBuilderSession coordinates common authoring operations. The Narrator can create and revise StoryPremise records; Casting and Stage edits update their workspace records; every edit refreshes the vocabulary index for that target, so searches reflect revisions rather than stale text. Timeline definitions can be created through the same session and optionally branch from an existing timeline in the workspace.

## UML class card

See [StoryBuilderSession UML](../../../docs/UML/StoryBuilderSessionUML.md) for the class diagram and use case.

## Use case

Create a session for one workspace. Use CreateTimeline to create a canonical timeline identity or a branch referencing an existing timeline; the Changed event notifies host adapters about the mutation. Use CreateCharacter or CreateScene when a user opens Casting or the Stage. UpdateCharacter and UpdateScene replace edited records and re-index their word occurrences. Historical claims, events, sources, continuity notes, campaigns, and participant actions can also be added through the session.

## Boundary

The session owns in-memory domain state; the WebDemo uses the versioned FSM_Serialization adapter to autosave that state to browser localStorage. Runtime execution is an optional adapter, separate from canonical timeline definitions. Cloud storage, concurrent multi-user editing, automatic adjective classification, and language-aware indexing are future work.
