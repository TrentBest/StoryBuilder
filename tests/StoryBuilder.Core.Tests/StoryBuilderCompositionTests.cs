using StoryBuilder.Core.Models;
using StoryBuilder.MicroBundle;
using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.MicroBundleDomain;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class StoryBuilderCompositionTests
{
    [Fact]
    public void FsmCosLoadsStoryBuilderAndHostCanCreateAnIsolatedSession()
    {
        var bundle = new StoryBuilderAuthoringBundle();
        var cos = new FsmCos(new SingleBundleCatalog(bundle));
        var manifest = new RuntimeManifest(
            42,
            [new MicroBundleManifestEntry(StoryBuilderAuthoringBundle.BundleId, StoryBuilderAuthoringBundle.BundleVersion)]);

        var assembly = cos.Execute(manifest);

        Assert.Equal(42UL, assembly.RuntimeId);
        Assert.True(assembly.TryGetBundle<StoryBuilderAuthoringBundle>(StoryBuilderAuthoringBundle.BundleId, out var loaded));
        Assert.NotNull(loaded);
        Assert.Equal(StoryBuilderAuthoringBundle.BundleId, loaded!.Descriptor.Id);
        Assert.Contains(loaded.Descriptor.Providers, provider => provider.Id == "storybuilder.casting");
        Assert.Contains(loaded.Descriptor.Providers, provider => provider.Id == "storybuilder.timeline");

        var session = loaded.CreateSession("A campaign");
        var character = session.CreateCharacter("Mara");
        Assert.Equal("Mara", Assert.Single(session.Workspace.Characters).Name);
    }

    private sealed class SingleBundleCatalog(IMicroBundle bundle) : IMicroBundleCatalog
    {
        public bool TryResolve(ulong bundleId, string version, out IMicroBundle? resolved)
        {
            resolved = bundle.Id == bundleId && bundle.Descriptor.Version == version ? bundle : null;
            return resolved is not null;
        }

        public bool TryResolve(ulong bundleId, out IMicroBundle? resolved)
        {
            resolved = bundle.Id == bundleId ? bundle : null;
            return resolved is not null;
        }
    }
}
