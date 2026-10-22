namespace StoryBuilder.Core.Models;

/// <summary>Describes a Workshop-resolved package. Consumers validate and apply these decisions rather than rerunning arbitration.</summary>
public sealed record CompositionPackageManifest
{
    public const int CurrentSchemaVersion = 1;
    public int SchemaVersion { get; init; } = CurrentSchemaVersion;
    public Guid PackageId { get; init; } = Guid.NewGuid();
    public Guid WorkspaceId { get; init; }
    public string ContentRevision { get; init; } = string.Empty;
    public string ContentSha256 { get; init; } = string.Empty;
    public string ConfigurationSha256 { get; init; } = string.Empty;
    public string OutputProfile { get; init; } = "book";
    public Guid ArbitrationRecordId { get; init; }
    public List<MicroBundleRequirement> Bundles { get; init; } = [];
}

/// <summary>A selected MicroBundle and the capabilities its resolved package requires.</summary>
public sealed record MicroBundleRequirement
{
    public ulong BundleId { get; init; }
    public string Version { get; init; } = string.Empty;
    public bool Required { get; init; } = true;
    public List<string> RequiredCapabilities { get; init; } = [];
}

/// <summary>Resolved runtime configuration for one selected MicroBundle.</summary>
public sealed record MicroBundleRuntimeConfiguration
{
    public ulong BundleId { get; init; }
    public string BundleVersion { get; init; } = string.Empty;
    public bool Enabled { get; init; } = true;
    public List<MicroBundleSetting> Settings { get; init; } = [];
}

/// <summary>A provider-defined setting with a stable key and declared value type.</summary>
public sealed record MicroBundleSetting
{
    public string Key { get; init; } = string.Empty;
    public string ValueType { get; init; } = "string";
    public string Value { get; init; } = string.Empty;
}

/// <summary>Captured per-bundle decisions tied to one exact package and content revision.</summary>
public sealed record CompositionPackageConfiguration
{
    public const int CurrentSchemaVersion = 1;
    public int SchemaVersion { get; init; } = CurrentSchemaVersion;
    public Guid PackageId { get; init; }
    public Guid WorkspaceId { get; init; }
    public string ContentRevision { get; init; } = string.Empty;
    public string OutputProfile { get; init; } = "book";
    public List<MicroBundleRuntimeConfiguration> Bundles { get; init; } = [];
}
