using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class TimelinePlanningPointAuthoringTests
{
    [Fact]
    public void AlternativesAndImpactLinksRemainProvisionalAndAreEditable()
    {
        var session = new StoryBuilderSession();
        var timeline = session.CreateTimeline("Main");
        var point = session.CreatePlanningPoint(timeline.Id, "The reveal", intent: "Change the alliance");
        var scene = session.CreateScene("Council", "The high chamber");
        var storyEvent = session.CreateTimelineEvent(timeline.Id, "Alliance changes");

        point = session.SetPlanningPointAlternatives(point.Id, [" A public confession ", "Secret bargain", "a PUBLIC CONFESSION", " "]);
        point = session.LinkPlanningPointToScene(point.Id, scene.Id);
        point = session.LinkPlanningPointToEvent(point.Id, storyEvent.Id);

        Assert.Equal(["A public confession", "Secret bargain"], point.Alternatives);
        Assert.Equal(TimelinePlanningStatus.Brainstorming, point.Status);
        Assert.Contains(scene.Id, point.RelatedSceneIds);
        Assert.Contains(storyEvent.Id, point.RelatedEventIds);
        Assert.Equal(CanonStatus.Proposed, storyEvent.Status);
        Assert.Empty(StoryWorkspaceValidator.Validate(session.Workspace));

        point = session.LinkPlanningPointToScene(point.Id, scene.Id, linked: false);
        point = session.LinkPlanningPointToEvent(point.Id, storyEvent.Id, linked: false);
        Assert.Empty(point.RelatedSceneIds);
        Assert.Empty(point.RelatedEventIds);
    }

    [Fact]
    public void ValidatorReportsMissingPlanningPointImpactTargets()
    {
        var workspace = new StoryWorkspace();
        var timeline = new TimelineDefinition { Name = "Main" };
        workspace.AddTimeline(timeline);
        var missingSceneId = Guid.NewGuid();
        var missingEventId = Guid.NewGuid();
        var point = new TimelinePlanningPoint
        {
            TimelineId = timeline.Id,
            RelatedSceneIds = [missingSceneId],
            RelatedEventIds = [missingEventId]
        };
        workspace.AddPlanningPoint(point);

        var issues = StoryWorkspaceValidator.Validate(workspace);

        Assert.Contains(issues, issue => issue.OwnerId == point.Id && issue.Relationship == "related scene" && issue.MissingTargetId == missingSceneId);
        Assert.Contains(issues, issue => issue.OwnerId == point.Id && issue.Relationship == "related event" && issue.MissingTargetId == missingEventId);
    }
}
