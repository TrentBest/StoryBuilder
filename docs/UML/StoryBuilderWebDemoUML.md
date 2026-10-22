# StoryBuilder WebDemo UML

## Component diagram

```mermaid
flowchart TD
  Browser[Author / participant] --> Pages[Blazor pages: Casting, Stage, Archive, Campaigns]
  Pages --> Session[StoryBuilderSession]
  Host[Blazor Program] --> Bundle[StoryBuilderAuthoringBundle]
  Bundle --> Session
  Session --> Workspace[StoryWorkspace]
  Session --> Vocabulary[VocabularyIndex]
  Session --> Composer[SceneComposer]
  Bundle -. composed by host contract .-> COS[FSM_COS via MicroBundleDomain]
```

## Use case

The host creates a scoped StoryBuilderSession through the MicroBundle capability factory. Users create and revise characters, scenes, claims, and campaign choices. Session operations update the workspace and vocabulary index; scene beats generate an editable narration draft. Data is local to the current circuit and is not persisted.
