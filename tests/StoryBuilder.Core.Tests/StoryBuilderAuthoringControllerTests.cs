using StoryBuilder.Core.Models;
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
    [InlineData(StoryBuilderAuthoringCommands.CreateEvent, "The beacon is lit")]
    [InlineData(StoryBuilderAuthoringCommands.CreateManuscriptSection, "A strange signal")]
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
            + session.Workspace.Manuscripts.Count
            + session.Workspace.Events.Count
            + session.Workspace.Manuscripts.Sum(item => item.Sections.Count));
    }

    [Fact]
    public void EventAndPassageCommandsCreateStableLink()
    {
        var session = new StoryBuilder.Core.Services.StoryBuilderSession("Book workflow");
        var controller = new StoryBuilderAuthoringController(session);

        var eventResult = controller.Execute(StoryBuilderAuthoringCommands.CreateEvent, "The door opens");
        var passageResult = controller.Execute(StoryBuilderAuthoringCommands.CreateManuscriptSection, "The discovery");
        var linkResult = controller.Execute(StoryBuilderAuthoringCommands.LinkLatestEventToLatestSection);

        Assert.True(eventResult.Succeeded);
        Assert.True(passageResult.Succeeded);
        Assert.True(linkResult.Succeeded);
        var manuscript = Assert.Single(session.Workspace.Manuscripts);
        var passage = Assert.Single(manuscript.Sections);
        Assert.Equal(passageResult.RecordId, passage.Id);
        Assert.Contains(eventResult.RecordId!.Value, passage.EventIds);
        Assert.Equal("Main Timeline", Assert.Single(session.Workspace.Timelines).Name);
    }

    [Fact]
    public void LinkCommandFailsClosedWhenPrerequisitesAreMissing()
    {
        var session = new StoryBuilder.Core.Services.StoryBuilderSession("Book workflow");
        var controller = new StoryBuilderAuthoringController(session);

        var result = controller.Execute(StoryBuilderAuthoringCommands.LinkLatestEventToLatestSection);

        Assert.False(result.Succeeded);
        Assert.Null(result.RecordId);
        Assert.Empty(session.Workspace.Events);
        Assert.Empty(session.Workspace.Manuscripts);
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
        Assert.Empty(session.Workspace.Events);
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
