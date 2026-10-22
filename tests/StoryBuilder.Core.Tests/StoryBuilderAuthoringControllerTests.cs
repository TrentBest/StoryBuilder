using StoryBuilder.Core.Models;
using StoryBuilder.MicroBundle;
using StoryBuilder.Persistence;
using TheSingularityWorkshop.FSM_Serialization;
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
        var recordExists = commandId switch
        {
            StoryBuilderAuthoringCommands.CreateTimeline => session.Workspace.Timelines.Any(item => item.Id == result.RecordId),
            StoryBuilderAuthoringCommands.CreateCharacter => session.Workspace.Characters.Any(item => item.Id == result.RecordId),
            StoryBuilderAuthoringCommands.CreateScene => session.Workspace.Scenes.Any(item => item.Id == result.RecordId),
            StoryBuilderAuthoringCommands.CreateManuscript => session.Workspace.Manuscripts.Any(item => item.Id == result.RecordId),
            StoryBuilderAuthoringCommands.CreateEvent => session.Workspace.Events.Any(item => item.Id == result.RecordId),
            StoryBuilderAuthoringCommands.CreateManuscriptSection => session.Workspace.Manuscripts
                .SelectMany(item => item.Sections).Any(item => item.Id == result.RecordId),
            _ => false
        };
        Assert.True(recordExists);
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
    public void EventAndPassageCommandWorkflowSurvivesBinarySaveAndReopen()
    {
        var session = new StoryBuilder.Core.Services.StoryBuilderSession("Book workflow");
        var controller = new StoryBuilderAuthoringController(session);

        var eventResult = controller.Execute(StoryBuilderAuthoringCommands.CreateEvent, "The door opens");
        var passageResult = controller.Execute(StoryBuilderAuthoringCommands.CreateManuscriptSection, "The discovery");
        var linkResult = controller.Execute(StoryBuilderAuthoringCommands.LinkLatestEventToLatestSection);

        Assert.True(eventResult.Succeeded);
        Assert.True(passageResult.Succeeded);
        Assert.True(linkResult.Succeeded);

        using var output = new MemoryBinaryStream();
        StoryWorkspaceBinaryDocument.FromSession(session).Pack(output);
        using var input = new MemoryBinaryStream(output.ToArray());
        var restoredDocument = new StoryWorkspaceBinaryDocument();
        restoredDocument.Unpack(input);
        var reopened = new StoryBuilder.Core.Services.StoryBuilderSession("temporary");
        restoredDocument.RestoreInto(reopened);

        var restoredEvent = Assert.Single(reopened.Workspace.Events);
        var restoredManuscript = Assert.Single(reopened.Workspace.Manuscripts);
        var restoredPassage = Assert.Single(restoredManuscript.Sections);
        Assert.Equal(eventResult.RecordId, restoredEvent.Id);
        Assert.Equal(passageResult.RecordId, restoredPassage.Id);
        Assert.Contains(restoredEvent.Id, restoredPassage.EventIds);
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
