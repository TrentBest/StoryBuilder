using StoryBuilder.Core.Services;
using TheSingularityWorkshop.Workshop.Gui;

namespace StoryBuilder.MicroBundle;

/// <summary>
/// Projects one StoryBuilder session into the shared, host-neutral semantic GUI tree and
/// executes only the explicit authoring command vocabulary. A host re-reads Root after
/// Execute to render the updated session projection.
/// </summary>
public sealed class StoryBuilderAuthoringSurface
{
    private readonly StoryBuilderAuthoringController _controller;

    public StoryBuilderAuthoringSurface(StoryBuilderSession session)
    {
        Session = session ?? throw new ArgumentNullException(nameof(session));
        _controller = new StoryBuilderAuthoringController(session);
    }

    /// <summary>The domain session projected by this surface.</summary>
    public StoryBuilderSession Session { get; }

    /// <summary>Builds a fresh GUI snapshot from the current workspace records.</summary>
    public GuiNode Root
    {
        get
        {
            var workspace = Session.Workspace;
            var root = GuiBuilders.Column("storybuilder-root")
                .Property("padding", "24")
                .Property("margin", "12")
                .Child(GuiBuilders.Text("storybuilder-title", "STORYBUILDER").Property("fontSize", "28"))
                .Child(GuiBuilders.Text("storybuilder-subtitle", "Compose a story from timelines, characters, scenes, and manuscripts."))
                .Child(GuiBuilders.Text(
                    "storybuilder-workspace",
                    $"Workspace: {workspace.Name}"))
                .Child(GuiBuilders.Text(
                    "storybuilder-counts",
                    $"Timelines: {workspace.Timelines.Count}  |  Characters: {workspace.Characters.Count}  |  Scenes: {workspace.Scenes.Count}  |  Manuscripts: {workspace.Manuscripts.Count}"))
                .Child(GuiBuilders.Button("storybuilder-create-timeline", "＋ Create timeline")
                    .Property("onClickCommand", StoryBuilderAuthoringCommands.CreateTimeline))
                .Child(GuiBuilders.Button("storybuilder-create-character", "＋ Create character")
                    .Property("onClickCommand", StoryBuilderAuthoringCommands.CreateCharacter))
                .Child(GuiBuilders.Button("storybuilder-create-scene", "＋ Create scene")
                    .Property("onClickCommand", StoryBuilderAuthoringCommands.CreateScene))
                .Child(GuiBuilders.Button("storybuilder-create-manuscript", "＋ Create manuscript")
                    .Property("onClickCommand", StoryBuilderAuthoringCommands.CreateManuscript));

            return root.Build();
        }
    }

    /// <summary>Dispatches a stable StoryBuilder command ID; unknown IDs are rejected without mutation.</summary>
    public StoryBuilderCommandResult Execute(string commandId, string? value = null) =>
        _controller.Execute(commandId, value);
}
