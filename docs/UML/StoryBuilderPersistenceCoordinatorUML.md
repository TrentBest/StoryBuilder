# StoryBuilderPersistenceCoordinator UML

## Class diagram

```mermaid
classDiagram
class StoryBuilderPersistenceCoordinator {
  +bool IsLoaded
  +string LoadError
  +string SaveError
  +DateTimeOffset LastSavedAtUtc
  +EnsureLoadedAsync() Task
  +SaveAsync() Task
  +ClearAndStartFreshAsync() Task
  -LoadCoreAsync() Task
  -OnSessionChanged()
}
StoryBuilderPersistenceCoordinator ..> StoryBuilderSession
StoryBuilderPersistenceCoordinator ..> StoryWorkspaceBinaryDocument
StoryBuilderPersistenceCoordinator ..> IJSRuntime
IJSRuntime ..> BrowserLocalStorage
```

## Use case

After the interactive browser connection is available, the layout loads a versioned binary workspace from localStorage. Subsequent StoryBuilderSession mutations trigger serialized saves. Corrupt or unreadable saved data is left untouched until the user explicitly chooses to reset it.
