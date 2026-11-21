# StoryBuilder.Quickstart

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

This sample runs the first authoring workflows against the same dependency-free domain core: create an editable character, record a vocabulary occurrence under a canonical synonym, and turn scene beats into an editable narration draft.

## Run

From the repository root:

```sh
dotnet run --project samples/StoryBuilder.Quickstart/StoryBuilder.Quickstart.csproj
```

The output shows the character, a vocabulary usage count, and a scene narration draft. This is a console proof of the domain API, not the final clickable Casting or claymation UI.


## raWWar Empire history workspace

The sample also builds a source-attributed StoryBuilder workspace from raWWar's living [Empire History & Faction Origins](https://github.com/TrentBest/raWWar/blob/development/docs/EMPIRE_HISTORY_AND_FACTION_ORIGINS.md) document. It creates thirteen provisional House organizations, proposed historical claims, and an undated outline of historical planning points. It does not invent dates or events, and it does not accept the claims as canon. See [RaWWar Empire Workspace](RaWWarEmpireWorkspace.md) for details.
