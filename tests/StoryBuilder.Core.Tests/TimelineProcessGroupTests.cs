using StoryBuilder.Core.Models;
using StoryBuilder.Core.Runtime;
using StoryBuilder.Core.Services;
using StoryBuilder.FSM;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class TimelineProcessGroupTests
{
    [Fact]
    public void DriverAdvancesTwoIndependentTimelineProcessGroups()
    {
        using var first = new TimelineProcessGroup(new TimelineDefinition { Name = "Original history" });
        using var second = new TimelineProcessGroup(new TimelineDefinition { Name = "Branch history" });
        var driver = new StoryProcessGroupDriver();

        driver.Insert(first);
        driver.Insert(second);
        driver.Update();
        driver.Update();

        Assert.Equal(2L, first.UpdateCount);
        Assert.Equal(2L, second.UpdateCount);
        Assert.Contains(first.Events, item => item.Kind == "RuntimeTick");
        Assert.Contains(second.Events, item => item.Kind == "RuntimeTick");
        Assert.All(first.Events, item => Assert.Equal(first.Id, item.TimelineId));
        Assert.All(second.Events, item => Assert.Equal(second.Id, item.TimelineId));
        Assert.NotEqual(first.Id, second.Id);
    }

    [Fact]
    public void SessionCreatesCanonicalTimelineAndBranch()
    {
        var session = new StoryBuilderSession("Timeline authoring");
        var original = session.CreateTimeline("Original");
        var branch = session.CreateTimeline("Changed history", parentTimelineId: original.Id);

        Assert.Equal(2, session.Workspace.Timelines.Count);
        Assert.Equal(original.Id, branch.ParentTimelineId);
        Assert.True(branch.IsBranch);
    }

    [Fact]
    public void DriverRejectsDuplicateProcessGroupIdentity()
    {
        using var group = new TimelineProcessGroup(new TimelineDefinition { Name = "One timeline" });
        var driver = new StoryProcessGroupDriver();

        driver.Insert(group);

        Assert.Throws<InvalidOperationException>(() => driver.Insert(group));
    }

    [Fact]
    public void WorkspaceRequiresBranchParentToExistBeforeBranchIsAdded()
    {
        var workspace = new StoryWorkspace();
        var parent = new TimelineDefinition { Name = "Original" };
        var branch = new TimelineDefinition { Name = "Changed history", ParentTimelineId = parent.Id };

        Assert.Throws<KeyNotFoundException>(() => workspace.AddTimeline(branch));

        workspace.AddTimeline(parent);
        workspace.AddTimeline(branch);

        Assert.Equal(2, workspace.Timelines.Count);
        Assert.True(workspace.Timelines[1].IsBranch);
        Assert.Equal(parent.Id, workspace.Timelines[1].ParentTimelineId);
    }
}
