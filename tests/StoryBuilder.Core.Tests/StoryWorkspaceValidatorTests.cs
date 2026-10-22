using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class StoryWorkspaceValidatorTests
{
    [Fact]
    public void ValidateReportsDanglingReferencesWithoutChangingWorkspace()
    {
        var workspace = new StoryWorkspace();
        var timeline = new TimelineDefinition { Name = "Main" };
        workspace.AddTimeline(timeline);

        var missingCauseId = Guid.NewGuid();
        var missingSourceId = Guid.NewGuid();
        var storyEvent = new StoryEvent
        {
            Title = "Broken imported event",
            TimelineId = timeline.Id,
            CauseEventIds = [missingCauseId],
            SourceIds = [missingSourceId]
        };
        workspace.AddEvent(storyEvent);

        var missingEventId = Guid.NewGuid();
        var manuscript = new ManuscriptEdition
        {
            Title = "Draft",
            Sections =
            [
                new ManuscriptSection
                {
                    Title = "Chapter 1",
                    EventIds = [missingEventId]
                }
            ]
        };
        workspace.AddManuscript(manuscript);

        var issues = StoryWorkspaceValidator.Validate(workspace);

        Assert.Contains(issues, issue => issue.OwnerId == storyEvent.Id &&
            issue.Relationship == "cause event" && issue.MissingTargetId == missingCauseId);
        Assert.Contains(issues, issue => issue.OwnerId == storyEvent.Id &&
            issue.Relationship == "source" && issue.MissingTargetId == missingSourceId);
        Assert.Contains(issues, issue => issue.OwnerId == manuscript.Sections[0].Id &&
            issue.Relationship == "event" && issue.MissingTargetId == missingEventId);
        Assert.Single(workspace.Events);
        Assert.Equal(missingCauseId, Assert.Single(workspace.Events).CauseEventIds[0]);
    }

    [Fact]
    public void ValidateAcceptsLinkedAuthoringWorkflow()
    {
        var session = new StoryBuilderSession();
        var timeline = session.CreateTimeline("Main");
        var storyEvent = session.CreateTimelineEvent(timeline.Id, "The reveal");
        var manuscript = session.CreateManuscript("A book");
        var section = session.AddManuscriptSection(manuscript.Id, ManuscriptSectionKind.Chapter, "Chapter 1");
        session.LinkEventToManuscriptSection(manuscript.Id, section.Sections[0].Id, storyEvent.Id);

        Assert.Empty(StoryWorkspaceValidator.Validate(session.Workspace));
    }
}
