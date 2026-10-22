# StoryBuilder WebDemo UML

## Component diagram

```mermaid
flowchart TD
  Browser[Author / participant] --> Pages[Blazor pages: Narrator, Casting, Stage, Archive, Campaigns, Living Map]
  Pages --> Session[StoryBuilderSession]
  Host[Blazor Program] --> Bundle[StoryBuilderAuthoringBundle]
  Bundle --> Session
  Session --> Workspace[StoryWorkspace]
  Session --> Vocabulary[VocabularyIndex]
  Session --> Composer[SceneComposer]
  Session --> Persistence[StoryBuilderPersistenceCoordinator]
  Persistence --> Binary[StoryWorkspaceBinaryDocument]
  Persistence --> LocalStorage[Browser localStorage]
  Session --> SvgRenderer[GUI.SVG living map]
  Bundle -. composed by host contract .-> COS[FSM_COS via MicroBundleDomain]
```

## Use case

The host creates a scoped StoryBuilderSession through the MicroBundle capability factory. The Narrator captures editable premise proposals; Casting manages character profiles and story-function roles; the Living Map renders the current records through GUI.SVG. Session changes trigger a versioned binary autosave to browser localStorage. Data remains local to this browser and is not cloud-synchronized.
