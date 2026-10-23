using StoryBuilder.MicroBundle;
using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.MicroBundleDomain;

namespace StoryBuilder.FSM_COS;

/// <summary>
/// Creates, composes, and returns named StoryBuilder experiences through FSM_COS.
/// This is an in-process host adapter; durable artifact storage and remote delivery remain host responsibilities.
/// </summary>
public sealed class StoryBuilderExperienceHost
{
    private static long _nextRuntimeId = 4_700_000_000;

    public StoryBuilderExperienceHost()
    {
        AuthoringBundle = new StoryBuilderAuthoringBundle();
    }

    public StoryBuilderAuthoringBundle AuthoringBundle { get; }

    /// <summary>
    /// Creates a named experience MicroBundle, makes it resolvable, and composes it through FSM_COS.
    /// Repeated names of the same kind reuse the same bundle identity but receive a fresh isolated session.
    /// </summary>
    public StoryBuilderHostedExperience Create(string kind, string name)
    {
        var experience = new StoryBuilderExperienceMicroBundle(kind, name);
        var catalog = new ExperienceCatalog(AuthoringBundle, experience);
        var runtimeId = checked((ulong)Interlocked.Increment(ref _nextRuntimeId));
        var manifest = new RuntimeManifest(
            runtimeId,
            [new MicroBundleManifestEntry(experience.Id, experience.Descriptor.Version)]);

        var assembly = new FsmCos(catalog).Execute(manifest);
        if (!assembly.TryGetBundle<StoryBuilderExperienceMicroBundle>(experience.Id, out var loaded) ||
            loaded is null)
        {
            throw new InvalidOperationException(
                $"FSM_COS composed runtime {runtimeId} without the requested StoryBuilder experience bundle.");
        }

        if (!assembly.TryGetBundle<StoryBuilderAuthoringBundle>(
                StoryBuilderAuthoringBundle.BundleId, out _))
        {
            throw new InvalidOperationException(
                $"FSM_COS composed runtime {runtimeId} without StoryBuilder's authoring capability.");
        }

        return new StoryBuilderHostedExperience(assembly, loaded);
    }

    private sealed class ExperienceCatalog(
        StoryBuilderAuthoringBundle authoring,
        StoryBuilderExperienceMicroBundle experience) : IMicroBundleCatalog
    {
        public bool TryResolve(ulong bundleId, string version, out IMicroBundle? bundle)
        {
            if (bundleId == experience.Id &&
                string.Equals(version, experience.Descriptor.Version, StringComparison.Ordinal))
            {
                bundle = experience;
                return true;
            }

            if (bundleId == authoring.Id &&
                string.Equals(version, authoring.Descriptor.Version, StringComparison.Ordinal))
            {
                bundle = authoring;
                return true;
            }

            bundle = null;
            return false;
        }

        public bool TryResolve(ulong bundleId, out IMicroBundle? bundle)
        {
            if (bundleId == experience.Id)
            {
                bundle = experience;
                return true;
            }

            if (bundleId == authoring.Id)
            {
                bundle = authoring;
                return true;
            }

            bundle = null;
            return false;
        }
    }
}

/// <summary>A successfully composed StoryBuilder experience ready for the host to present.</summary>
public sealed record StoryBuilderHostedExperience(
    RuntimeAssembly Assembly,
    StoryBuilderExperienceMicroBundle Experience)
{
    public StoryBuilderAuthoringSurface Surface => Experience.Surface;
}
