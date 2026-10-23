using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class StoryContinuityAnalyzerTests
{
    [Fact]
    public void ReportsCausalCycleWithoutChangingEvents()
    {
        var workspace = new StoryWorkspace();
        var first = new StoryEvent { Title = "The signal", CauseEventIds = [] };
        var second = new StoryEvent { Title = "The response", CauseEventIds = [first.Id] };
        first = first with { CauseEventIds = [second.Id] };
        workspace.AddEvent(first);
        workspace.AddEvent(second);

        var findings = StoryContinuityAnalyzer.Analyze(workspace);

        Assert.Contains(findings, finding => finding.Code == "event.causal-cycle");
        Assert.Contains(second.Id, first.CauseEventIds);
    }

    [Fact]
    public void ReportsCauseAndConsequenceLinksThatDisagree()
    {
        var workspace = new StoryWorkspace();
        var cause = new StoryEvent { Title = "The alarm" };
        var effect = new StoryEvent { Title = "The evacuation", CauseEventIds = [cause.Id] };
        workspace.AddEvent(cause);
        workspace.AddEvent(effect);

        var findings = StoryContinuityAnalyzer.Analyze(workspace);

        Assert.Contains(findings, finding => finding.Code == "event.cause-link-not-reciprocal");
    }

    [Fact]
    public void AcceptsValidTimelineBranchWithoutContinuityFindings()
    {
        var workspace = new StoryWorkspace();
        var root = new TimelineDefinition { Name = "Prime" };
        var branch = new TimelineDefinition { Name = "Branch", ParentTimelineId = root.Id };
        workspace.AddTimeline(root);
        workspace.AddTimeline(branch);

        Assert.DoesNotContain(
            StoryContinuityAnalyzer.Analyze(workspace),
            finding => finding.Code == "timeline.branch-cycle");
    }

    [Fact]
    public void ReportsPlaceContainmentCyclesWithoutMutatingPlaces()
    {
        var workspace = new StoryWorkspace();
        var first = new PlaceRecord { Name = "First" };
        var second = new PlaceRecord { Name = "Second", ParentPlaceId = first.Id };
        workspace.AddPlace(first);
        workspace.AddPlace(second);
        workspace.ReplacePlace(first with { ParentPlaceId = second.Id });

        var findings = StoryContinuityAnalyzer.Analyze(workspace);

        Assert.Contains(findings, finding => finding.Code == "place.containment-cycle");
        Assert.Equal(second.Id, Assert.Single(workspace.Places, place => place.Id == first.Id).ParentPlaceId);
    }

    [Fact]
    public void LeavesConsistentAcyclicHistoryWithoutFindings()
    {
        var workspace = new StoryWorkspace();
        var cause = new StoryEvent { Title = "The alarm" };
        var effect = new StoryEvent
        {
            Title = "The evacuation",
            CauseEventIds = [cause.Id]
        };
        cause = cause with { ConsequenceEventIds = [effect.Id] };
        workspace.AddEvent(cause);
        workspace.AddEvent(effect);

        Assert.Empty(StoryContinuityAnalyzer.Analyze(workspace));
    }
}
