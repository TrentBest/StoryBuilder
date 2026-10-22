using StoryBuilder.MicroBundle;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class StoryBuilderAuthoringControllerTests
{
    [Theory]
    [InlineData(StoryBuilderAuthoringCommands.CreateTimeline, "Branch history")]
    [InlineData(StoryBuilderAuthoringCommands.CreateCharacter, "Mara")]
    [InlineData(StoryBuilderAuthoringCommands.CreateScene, "The North Gate")]
    [InlineData(StoryBuilderAuthoringCommands.CreateManuscript, "The Lost Cartographer")]
    public void SupportedCommandCreatesRecordInSession(string commandId, string name)
    {
        var session = new StoryBuilder.Core.Services.StoryBuilderSession("Command test");
        var controller = new StoryBuilderAuthoringController(session);

        var result = controller.Execute(commandId, name);

        Assert.True(result.Succeeded);
        Assert.Equal(commandId, result.CommandId);
        Assert.NotNull(result.RecordId);
        Assert.Contains(name, result.Message);
        Assert.Equal(1, session.Workspace.Timelines.Count
            + session.Workspace.Characters.Count
            + session.Workspace.Scenes.Count
            + session.Workspace.Manuscripts.Count);
    }

    [Fact]
    public void UnsupportedCommandIsRejectedWithoutMutation()
    {
        var session = new StoryBuilder.Core.Services.StoryBuilderSession("Command test");
        var controller = new StoryBuilderAuthoringController(session);

        var result = controller.Execute("storybuilder.workspace.delete", "Everything");

        Assert.False(result.Succeeded);
        Assert.Null(result.RecordId);
        Assert.Contains("Unsupported", result.Message);
        Assert.Empty(session.Workspace.Timelines);
        Assert.Empty(session.Workspace.Characters);
        Assert.Empty(session.Workspace.Scenes);
        Assert.Empty(session.Workspace.Manuscripts);
    }

    [Fact]
    public void BlankNameUsesPredictableDefault()
    {
        var session = new StoryBuilder.Core.Services.StoryBuilderSession("Command test");
        var controller = new StoryBuilderAuthoringController(session);

        var result = controller.Execute(StoryBuilderAuthoringCommands.CreateTimeline, "   ");

        Assert.True(result.Succeeded);
        Assert.Equal("Timeline 1", Assert.Single(session.Workspace.Timelines).Name);
    }
}
