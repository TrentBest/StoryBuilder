# AnyApp GUI Interaction Contract

## Purpose

This note records the verified seam between StoryBuilder's host-neutral authoring model, the Workshop semantic GUI tree, and AnyApp's native WPF host. It is an implementation contract; StoryBuilder's semantic surface exists, but the end-to-end desktop integration is not yet complete.

## Verified boundaries

- `StoryBuilder.Core` owns authoring records and operations. Its domain state must not depend on WPF or AnyApp.
- `StoryBuilder.MicroBundle` implements `IMicroBundle` and declares StoryBuilder capabilities. `StoryBuilderAuthoringSurface` now projects a session into a fresh `GuiNode` snapshot and routes the explicit authoring command vocabulary through `StoryBuilderAuthoringController`.
- `TheSingularityWorkshop.GUI.Core` represents a GUI as a recursive `GuiNode` tree built with `GuiBuilders`. The tree describes controls and stable command metadata; it does not execute domain actions itself.
- AnyApp's `MicroBundleGuiSurface.TryGetRoot` discovers a public `Root` property whose type is `GuiNode`. The StoryBuilder surface is a separate adapter and has not yet been selected and bound by AnyApp.
- The current GUI development branch has `WpfGuiRenderer.Render(node, commandDispatcher)` for host-supplied command dispatch. AnyApp still calls the non-dispatching overload in its generic Experience path.
- AnyApp's current Moniker/Forge interactions include host-specific wiring. Those paths are not a general interaction contract for arbitrary MicroBundles.

## Required interaction path

1. StoryBuilder produces a semantic view from real StoryBuilder session/workspace data. **Implemented in `StoryBuilderAuthoringSurface`.**
2. Each actionable node carries a stable, host-neutral command identifier; editable nodes carry stable field identifiers. Display labels must not double as command identities. **The four create buttons now carry stable `onClickCommand` IDs; editable fields are not yet implemented.**
3. The host maps supported command identifiers to explicit StoryBuilder operations. Unknown commands are rejected without mutation by the StoryBuilder controller. AnyApp's dispatcher integration remains outstanding.
4. Domain changes trigger a refreshed semantic projection. The surface's `Root` getter rebuilds its snapshot from the current workspace; the host must request and render that snapshot after execution.
5. The GUI.SVG documentation view should derive from the same domain records/projection, rather than a separately maintained mockup. The new surface has not yet been wired to the existing SVG projection.
6. Automated tests cover stable node IDs, command metadata, command dispatch, updated counts after mutation, and unsupported-command safety. AnyApp renderer/host integration tests remain outstanding.

## Integration choices

- Keep the MicroBundleDomain contract and StoryBuilder.Core free of AnyApp/WPF dependencies.
- Reuse GUI.Core and GUI.WPF rather than introducing a second UI tree or a new manifest system.
- Prefer a small, explicit command contract over reflection-based invocation or arbitrary command strings.
- Keep AnyApp's repository-source integration conditional. A development integration must not make a new NuGet package dependency mandatory for ordinary builds before a release is authorized.
- A rendered control is not considered end-to-end functional until the host dispatches the action, the domain changes, and the refreshed projection is visible.

## Current status

The StoryBuilder semantic projection and bounded command adapter are implemented on StoryBuilder `development`; tests and UML/use-case documentation have been added but have not yet been verified by CI. AnyApp is not yet wired to create the surface for a composed StoryBuilder bundle, route button events to it, and refresh the rendered tree. The GUI repository's new WPF dispatcher overload is committed on its `development` branch, but StoryBuilder's GUI submodule pin predates that change. StoryBuilder is therefore **not release-ready** on the strength of this increment alone.

No NuGet package was published, no PR was merged, no branch was deleted, and no deployment was performed.
