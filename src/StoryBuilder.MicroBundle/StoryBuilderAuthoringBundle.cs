using TheSingularityWorkshop.MicroBundleDomain;
using StoryBuilder.Core.Services;

namespace StoryBuilder.MicroBundle;

/// <summary>
/// A host-composable authoring capability. FSM_COS composes this bundle; the bundle itself
/// depends only on the neutral MicroBundleDomain contract, not on the composition kernel.
/// </summary>
#if WORKSHOP_USE_REPOSITORY_SOURCES
public sealed class StoryBuilderAuthoringBundle : IMicroBundle, IMicroBundleResolvedConfiguration
#else
public sealed class StoryBuilderAuthoringBundle : IMicroBundle
#endif
{
    public const ulong BundleId = 0x53544F5259425549UL;
    public const string BundleVersion = "0.1.0";

    public MicroBundleDescriptor Descriptor { get; } = new(
        BundleId,
        BundleVersion,
        providers:
        [
            new MicroBundleProvider("storybuilder.workspace"),
            new MicroBundleProvider("storybuilder.casting"),
            new MicroBundleProvider("storybuilder.stage"),
            new MicroBundleProvider("storybuilder.vocabulary"),
            new MicroBundleProvider("storybuilder.history"),
            new MicroBundleProvider("storybuilder.campaign"),
            new MicroBundleProvider("storybuilder.timeline")
        ]);

    public IReadOnlyList<MicroBundleDependencyRequest> Dependencies { get; } =
        Array.Empty<MicroBundleDependencyRequest>();

    /// <summary>Creates an isolated authoring session for one user/workspace.</summary>
    public StoryBuilderSession CreateSession(string? workspaceName = null) =>
        new(workspaceName);

    public void Load(IMicroBundleLoadContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        // The bundle is a stateless capability factory; user workspaces are created per session.
        _ = context.RuntimeId;
    }

#if WORKSHOP_USE_REPOSITORY_SOURCES
    /// <summary>
    /// StoryBuilder's bundle is a stateless capability factory with no configuration overrides.
    /// </summary>
    public ReadOnlyMemory<byte> ExportResolvedConfiguration() => ReadOnlyMemory<byte>.Empty;
#endif

    public bool Arbitrate(IMicroBundleArbitrationContext context, int roundIndex)
    {
        ArgumentNullException.ThrowIfNull(context);
        if (roundIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(roundIndex));
        // StoryBuilder declares its capabilities and does not mutate the host's composition graph.
        return false;
    }
}
