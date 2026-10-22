using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using StoryBuilder.Persistence;
using TheSingularityWorkshop.FSM_Serialization;
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
    public void PromotionCreatesOneProposedEventAndPreservesItsIdentityLink()
    {
        var session = new StoryBuilderSession();
        var timeline = session.CreateTimeline("Main");
        var point = session.CreatePlanningPoint(timeline.Id, "The reveal", "Near the midpoint", "Expose the hidden alliance.");
        point = session.SetPlanningPointAlternatives(point.Id, ["Public confession", "Secret bargain"]);

        var promoted = session.PromotePlanningPointToEvent(point.Id);
        var restoredPoint = Assert.Single(session.Workspace.PlanningPoints);
        var repeated = session.PromotePlanningPointToEvent(point.Id);

        Assert.Equal(point.Id, restoredPoint.Id);
        Assert.Equal(promoted.Id, restoredPoint.PromotedEventId);
        Assert.Equal(TimelinePlanningStatus.Superseded, restoredPoint.Status);
        Assert.Equal(CanonStatus.Proposed, promoted.Status);
        Assert.Equal("The reveal", promoted.Title);
        Assert.Equal("Expose the hidden alliance.", promoted.Summary);
        Assert.Equal(TimelinePlacementKind.Approximate, promoted.PlacementKind);
        Assert.Equal(promoted.Id, repeated.Id);
        Assert.Single(session.Workspace.Events);
        Assert.Empty(StoryWorkspaceValidator.Validate(session.Workspace));
    }

    [Fact]
    public void PromotionIdentitySurvivesFormatTwelvePersistence()
    {
        var session = new StoryBuilderSession();
        var timeline = session.CreateTimeline("Main");
        var point = session.CreatePlanningPoint(timeline.Id, "The reveal", intent: "Expose the hidden alliance.");
        var promoted = session.PromotePlanningPointToEvent(point.Id);

        using var output = new MemoryBinaryStream();
        StoryWorkspaceBinaryDocument.FromSession(session).Pack(output);
        using var input = new MemoryBinaryStream(output.ToArray());
        var restored = new StoryWorkspaceBinaryDocument();
        restored.Unpack(input);

        var restoredPoint = Assert.Single(restored.Workspace.PlanningPoints);
        Assert.Equal(TimelinePlanningStatus.Superseded, restoredPoint.Status);
        Assert.Equal(promoted.Id, restoredPoint.PromotedEventId);
        Assert.Equal(promoted.Id, Assert.Single(restored.Workspace.Events).Id);
        Assert.Empty(StoryWorkspaceValidator.Validate(restored.Workspace));
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
