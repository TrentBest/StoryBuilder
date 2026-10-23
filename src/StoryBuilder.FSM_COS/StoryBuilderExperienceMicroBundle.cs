using System.Security.Cryptography;
using System.Text;
using StoryBuilder.Core.Services;
using StoryBuilder.MicroBundle;
using TheSingularityWorkshop.MicroBundleDomain;

namespace StoryBuilder.FSM_COS;

/// <summary>
/// A host-created StoryBuilder experience with a stable identity derived from its kind and name.
/// The experience owns one authoring session; FSM_COS composes it with StoryBuilder's authoring capability.
/// </summary>
public sealed class StoryBuilderExperienceMicroBundle : IMicroBundle
{
    public const string CurrentVersion = "0.1.0";
    private const string ProviderPrefix = "storybuilder.experience.";

    public StoryBuilderExperienceMicroBundle(string kind, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(kind);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Kind = Normalize(kind);
        Name = name.Trim();
        Id = CreateStableId(Kind, Name);
        Session = new StoryBuilderSession($"{Kind}: {Name}");
        Surface = new StoryBuilderAuthoringSurface(Session);
        Descriptor = new MicroBundleDescriptor(
            Id,
            CurrentVersion,
            dependencies: [new MicroBundleDependency(StoryBuilderAuthoringBundle.BundleId)],
            providers: [new MicroBundleProvider(ProviderPrefix + Slug(Kind))]);
    }

    public string Kind { get; }
    public string Name { get; }
    public ulong Id { get; }
    public MicroBundleDescriptor Descriptor { get; }
    public IReadOnlyList<MicroBundleDependencyRequest> Dependencies { get; } =
        [MicroBundleDependencyRequest.Unconfigured(StoryBuilderAuthoringBundle.BundleId)];
    public StoryBuilderSession Session { get; }
    public StoryBuilderAuthoringSurface Surface { get; }

    public void Load(IMicroBundleLoadContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _ = context.RuntimeId;
        // The session is constructed before composition and remains owned by this bundle.
        // The host only exposes it after FSM_COS returns a successful RuntimeAssembly.
    }

    public bool Arbitrate(IMicroBundleArbitrationContext context, int roundIndex)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentOutOfRangeException.ThrowIfNegative(roundIndex);
        return false;
    }

    private static string Normalize(string value) => value.Trim().ToLowerInvariant();

    private static string Slug(string value)
    {
        var slug = new string(value.Select(character =>
            char.IsAsciiLetterOrDigit(character) ? char.ToLowerInvariant(character) : '-').ToArray()).Trim('-');
        return string.IsNullOrWhiteSpace(slug) ? "experience" : slug;
    }

    private static ulong CreateStableId(string kind, string name)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes($"{kind}\n{name.Trim().ToUpperInvariant()}"));
        var id = BitConverter.ToUInt64(bytes, 0) | (1UL << 63);
        return id == StoryBuilderAuthoringBundle.BundleId ? id - 1 : id;
    }
}
