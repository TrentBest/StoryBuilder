# WebDemo Browser Persistence Tests

## Purpose

These tests exercise the actual interactive server-side Blazor WebDemo in Chromium. They are distinct from the coordinator and repository unit tests: the browser drives the visible authoring controls, the host writes the versioned workspace file, and a separate host process restores that file.

## Covered acceptance path

`HostWorkspaceBrowserTests.ManuscriptSurvivesBrowserReloadAndWebHostRestart`:

1. Starts WebDemo with an isolated `StoryBuilder__Workspaces__StoragePath`.
2. Creates a manuscript and chapter through the `/book` UI.
3. Saves manuscript text through the visible editor.
4. Reloads the same browser page and verifies the manuscript, chapter, and text.
5. Stops the WebDemo process, starts a new process against the same host directory, and verifies the same content again.

The test uses a temporary workspace directory and removes it after the run. It does not test cloud storage, multiple users, backup/restore, or legacy browser-snapshot migration; those remain separate acceptance cases.

## Running locally

Build the WebDemo and browser test project in Release, install the Playwright Chromium browser, then run the test:

```powershell
dotnet build samples/StoryBuilder.WebDemo/StoryBuilder.WebDemo.csproj --configuration Release
dotnet build tests/StoryBuilder.WebDemo.BrowserTests/StoryBuilder.WebDemo.BrowserTests.csproj --configuration Release
pwsh tests/StoryBuilder.WebDemo.BrowserTests/bin/Release/net8.0/playwright.ps1 install chromium
dotnet test tests/StoryBuilder.WebDemo.BrowserTests/StoryBuilder.WebDemo.BrowserTests.csproj --configuration Release --no-build
```

CI installs Chromium with its Linux dependencies. The test reserves a loopback port and launches the already-built WebDemo DLL as a child process.
