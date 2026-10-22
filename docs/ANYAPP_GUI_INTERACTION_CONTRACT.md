# AnyApp GUI Interaction Contract

## Purpose

This note records the verified seam between StoryBuilder's host-neutral authoring model, the Workshop semantic GUI tree, and AnyApp's native WPF host. It is an implementation contract for the next integration increment, not a claim that desktop integration is already complete.

## Verified boundaries

- `StoryBuilder.Core` owns authoring records and operations. Its domain state must not depend on WPF or AnyApp.
- `StoryBuilder.MicroBundle` implements `IMicroBundle` and declares StoryBuilder capabilities. It currently does not expose a semantic `Root` surface.
- `TheSingularityWorkshop.GUI.Core` represents a GUI as a recursive `GuiNode` tree built with `GuiBuilders`. The tree can describe Buttons and TextBoxes but does not itself execute their actions.
- AnyApp's `MicroBundleGuiSurface.TryGetRoot` discovers a public `Root` property whose type is `GuiNode`.
- AnyApp renders the tree through `WpfGuiRenderer.Render`. The verified renderer creates WPF Buttons and TextBoxes, but it does not wire button clicks to domain commands or propagate edited text back to the domain.
- AnyApp's current Moniker/Forge interactions include host-specific wiring. Those paths are not a general interaction contract for arbitrary MicroBundles.

## Required interaction path

1. StoryBuilder produces a semantic view from real StoryBuilder session/workspace data.
2. Each actionable node carries a stable, host-neutral command identifier; editable nodes carry stable field identifiers. Display labels must not double as command identities.
3. The host maps supported command identifiers to explicit StoryBuilder operations and maps field edits back to session operations. Unknown commands are rejected or left inert with a visible explanation; they must never be interpreted as arbitrary method names.
4. Domain changes trigger a refreshed semantic projection so counts, status, and linked views reflect the current workspace.
5. The GUI.SVG documentation view derives from the same domain records/projection, rather than a separately maintained mockup.
6. Automated tests cover node identity, command-to-operation mapping, validation failures, refresh after edits, and the host's handling of unsupported commands.

## Integration choices

- Keep the MicroBundleDomain contract and StoryBuilder.Core free of AnyApp/WPF dependencies.
- Reuse GUI.Core and GUI.WPF rather than introducing a second UI tree or a new manifest system.
- Prefer a small, explicit command contract over reflection-based invocation or arbitrary command strings.
- Keep AnyApp's repository-source integration conditional. A development integration must not make a new NuGet package dependency mandatory for ordinary builds before a release is authorized.
- A rendered control is not considered functional until its interaction is wired and verified.

## Current status

The semantic rendering path is verified in AnyApp source, but the StoryBuilder surface and generic interaction bridge are not yet implemented. This is an acknowledged integration gap; StoryBuilder is not release-ready. No NuGet package was published, no PR was merged, no branch was deleted, and no deployment was performed.
