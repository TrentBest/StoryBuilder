using StoryBuilder.FSM_COS;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class StoryBuilderExperienceHostTests
{
    [Fact]
    public void Create_composes_named_experience_through_fsm_cos()
    {
        var host = new StoryBuilderExperienceHost();

        var hosted = host.Create("city", "Tacoma");

        Assert.Equal("city", hosted.Experience.Kind);
        Assert.Equal("Tacoma", hosted.Experience.Name);
        Assert.Equal("city: Tacoma", hosted.Experience.Session.Workspace.Name);
        Assert.Contains(hosted.Assembly.Bundles, bundle =>
            bundle.Id == hosted.Experience.Id);
        Assert.Contains(hosted.Assembly.Bundles, bundle =>
            bundle.Id == StoryBuilder.MicroBundle.StoryBuilderAuthoringBundle.BundleId);
        Assert.NotNull(hosted.Surface.Root);
    }

    [Fact]
    public void Create_uses_a_stable_bundle_identity_for_same_named_experience()
    {
        var host = new StoryBuilderExperienceHost();

        var first = host.Create("city", "Tacoma");
        var second = host.Create("city", "Tacoma");

        Assert.Equal(first.Experience.Id, second.Experience.Id);
        Assert.NotSame(first.Experience.Session, second.Experience.Session);
    }
}
