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
    public void DisposingTimelineDestroysOnlyItsOwnedHandleAndIsIdempotent()
    {
        using var first = new TimelineProcessGroup(new TimelineDefinition { Name = "Retired timeline" });
        using var second = new TimelineProcessGroup(new TimelineDefinition { Name = "Surviving timeline" });

        first.Dispose();
        first.Dispose();

        Assert.Throws<ObjectDisposedException>(() => first.Update());
        Assert.Equal(0L, first.RuntimeTickCount);
        Assert.Contains(first.Events, item => item.Kind == "ProcessGroupExited");

        second.Update();

        Assert.Equal(1L, second.RuntimeTickCount);
        Assert.Contains(second.Events, item => item.Kind == "RuntimeTick");
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
    public void DriverUpdatesSnapshotWhenGroupsChangeDuringPass()
    {
        var driver = new StoryProcessGroupDriver();
        var addedDuringPass = new RecordingProcessGroup();
        var removedDuringPass = new RecordingProcessGroup();
        var first = new RecordingProcessGroup(onUpdate: updateCount =>
        {
            if (updateCount != 1)
                return;

            Assert.True(driver.Remove(removedDuringPass.Id));
            driver.Insert(addedDuringPass);
        });

        driver.Insert(first);
        driver.Insert(removedDuringPass);

        driver.Update();

        Assert.Equal(1L, first.UpdateCount);
        Assert.Equal(1L, removedDuringPass.UpdateCount);
        Assert.Equal(0L, addedDuringPass.UpdateCount);
        Assert.DoesNotContain(driver.Groups, group => group.Id == removedDuringPass.Id);
        Assert.Contains(driver.Groups, group => group.Id == addedDuringPass.Id);

        driver.Update();

        Assert.Equal(2L, first.UpdateCount);
        Assert.Equal(1L, removedDuringPass.UpdateCount);
        Assert.Equal(1L, addedDuringPass.UpdateCount);
    }

    [Fact]
    public void DriverRemovalDoesNotDisposeDetachedGroup()
    {
        var group = new RecordingProcessGroup();
        var driver = new StoryProcessGroupDriver();
        driver.Insert(group);

        Assert.True(driver.Remove(group.Id));
        Assert.False(group.IsDisposed);
        Assert.Empty(driver.Groups);

        group.Dispose();

        Assert.True(group.IsDisposed);
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

    private sealed class RecordingProcessGroup(Action<long>? onUpdate = null) : IStoryProcessGroup, IDisposable
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name => "Recording test group";
        public long UpdateCount { get; private set; }
        public bool IsDisposed { get; private set; }

        public void Update()
        {
            ObjectDisposedException.ThrowIf(IsDisposed, this);
            UpdateCount++;
            onUpdate?.Invoke(UpdateCount);
        }

        public void Dispose() => IsDisposed = true;
    }
}
