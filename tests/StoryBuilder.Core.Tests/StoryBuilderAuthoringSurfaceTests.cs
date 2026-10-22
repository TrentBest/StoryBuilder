using StoryBuilder.Core.Services;
using StoryBuilder.MicroBundle;
using TheSingularityWorkshop.Workshop.Gui;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class StoryBuilderAuthoringSurfaceTests
{
    [Fact]
    public void RootUsesStableNodeIdsAndExplicitCommandIds()
    {
        var surface = new StoryBuilderAuthoringSurface(new StoryBuilderSession("Surface test"));

        var root = surface.Root;

        Assert.Equal("storybuilder-root", root.Id);
        Assert.Equal("Column", root.Kind);
        Assert.Equal(
            StoryBuilderAuthoringCommands.CreateTimeline,
            Assert.IsType<GuiNode>(root.Children.Single(node => node.Id == "storybuilder-create-timeline"))
                .Properties["onClickCommand"]);
        Assert.Equal(
            StoryBuilderAuthoringCommands.CreateCharacter,
            root.Children.Single(node => node.Id == "storybuilder-create-character").Properties["onClickCommand"]);
        Assert.Equal(
            StoryBuilderAuthoringCommands.CreateScene,
            root.Children.Single(node => node.Id == "storybuilder-create-scene").Properties["onClickCommand"]);
        Assert.Equal(
            StoryBuilderAuthoringCommands.CreateManuscript,
            root.Children.Single(node => node.Id == "storybuilder-create-manuscript").Properties["onClickCommand"]);
    }

    [Fact]
    public void ExecutingCommandUpdatesNextSemanticSnapshot()
    {
        var surface = new StoryBuilderAuthoringSurface(new StoryBuilderSession("Refresh test"));

        var result = surface.Execute(StoryBuilderAuthoringCommands.CreateTimeline, "Branch history");

        Assert.True(result.Succeeded);
        Assert.Contains(
            "Timelines: 1",
            surface.Root.Children.Single(node => node.Id == "storybuilder-counts").Text ?? string.Empty);
    }

    [Fact]
    public void UnsupportedCommandLeavesProjectionUnchanged()
    {
        var surface = new StoryBuilderAuthoringSurface(new StoryBuilderSession("Safe surface"));
        var before = surface.Root.Children.Single(node => node.Id == "storybuilder-counts").Text;

        var result = surface.Execute("storybuilder.workspace.delete");

        Assert.False(result.Succeeded);
        Assert.Equal(before, surface.Root.Children.Single(node => node.Id == "storybuilder-counts").Text);
    }
}
