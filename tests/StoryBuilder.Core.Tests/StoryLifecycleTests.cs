using StoryBuilder.Core.Models;
using StoryBuilder.FSM;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class StoryLifecycleTests
{
    [Fact]
    public void LifecycleRequiresProposalBeforeAcceptance()
    {
        using var lifecycle = new StoryLifecycleSession("Character review");

        Assert.Equal(CanonStatus.Draft, lifecycle.CurrentStatus);
        Assert.Equal(CanonStatus.Draft, lifecycle.Request(CanonStatus.Accepted));
        Assert.Equal(CanonStatus.Proposed, lifecycle.Request(CanonStatus.Proposed));
        Assert.Equal(CanonStatus.Accepted, lifecycle.Request(CanonStatus.Accepted));
    }

    [Fact]
    public void RegistryReconstructsTheCurrentReviewStatusForAnExistingRecord()
    {
        using var registry = new StoryLifecycleRegistry();
        var recordId = Guid.NewGuid();

        var lifecycle = registry.GetSession(recordId, CanonStatus.Accepted, "Restored record");

        Assert.Equal(CanonStatus.Accepted, lifecycle.CurrentStatus);
        Assert.Same(lifecycle, registry.GetSession(recordId, CanonStatus.Accepted, "Ignored later name"));
    }

    [Fact]
    public void AcceptedRecordCanBeReproposedForRevisionThenDeprecated()
    {
        using var lifecycle = new StoryLifecycleSession("Event review");
        lifecycle.Request(CanonStatus.Proposed);
        lifecycle.Request(CanonStatus.Accepted);

        Assert.Equal(CanonStatus.Proposed, lifecycle.Request(CanonStatus.Proposed));
        Assert.Equal(CanonStatus.Accepted, lifecycle.Request(CanonStatus.Accepted));
        Assert.Equal(CanonStatus.Deprecated, lifecycle.Request(CanonStatus.Deprecated));
        Assert.Equal(CanonStatus.Deprecated, lifecycle.Request(CanonStatus.Proposed));
    }
}
