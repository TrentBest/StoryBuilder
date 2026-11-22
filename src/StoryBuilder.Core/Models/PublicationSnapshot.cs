namespace StoryBuilder.Core.Models;

/// <summary>
/// A self-contained, immutable publication handoff. It contains no StoryBuilder runtime objects;
/// consumers can deserialize the document using its schema version and their own runtime model.
/// </summary>
public sealed record PublicationSnapshot
{
    public int SchemaVersion { get; init; } = 1;
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public string ContentSha256 { get; init; } = string.Empty;
    public PublicationSourceProvenance Source { get; init; } = new();
    public PublicationManuscript Manuscript { get; init; } = new();
    public IReadOnlyList<PublicationSection> Sections { get; init; } = Array.Empty<PublicationSection>();
}

public sealed record PublicationSourceProvenance
{
    public string ManifestPath { get; init; } = string.Empty;
    public string ManifestBlobSha { get; init; } = string.Empty;
    public IReadOnlyDictionary<string, Guid> SourceSectionIds { get; init; } =
        new Dictionary<string, Guid>(StringComparer.Ordinal);
}

public sealed record PublicationManuscript
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Subtitle { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
}

public sealed record PublicationSection
{
    public string SourceId { get; init; } = string.Empty;
    public Guid SectionId { get; init; }
    public int Sequence { get; init; }
    public string Kind { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public string BodySha256 { get; init; } = string.Empty;
    public PublicationComposition? Composition { get; init; }
}

public sealed record PublicationComposition
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Orientation { get; init; } = string.Empty;
    public string ImageFit { get; init; } = string.Empty;
    public PublicationRegion TextRegion { get; init; } = new();
    public PublicationRegion ImageRegion { get; init; } = new();
    public PublicationRegion? DiscoveryRegion { get; init; }
    public string? FocalPoint { get; init; }
    public string? DiscoveryPrompt { get; init; }
    public int DiscoveryHintDelaySeconds { get; init; }
    public int LayerOrder { get; init; }
    public PublicationVisualAsset VisualAsset { get; init; } = new();
}

public sealed record PublicationRegion
{
    public double X { get; init; }
    public double Y { get; init; }
    public double Width { get; init; }
    public double Height { get; init; }
}

public sealed record PublicationVisualAsset
{
    public Guid AssetId { get; init; }
    public Guid RevisionId { get; init; }
    public int RevisionNumber { get; init; }
    public string Location { get; init; } = string.Empty;
    public string MediaType { get; init; } = string.Empty;
    public string ContentSha256 { get; init; } = string.Empty;
    public PublicationApproval Approval { get; init; } = new();
}

public sealed record PublicationApproval
{
    public Guid ReviewId { get; init; }
    public string Decision { get; init; } = string.Empty;
    public string Reviewer { get; init; } = string.Empty;
    public DateTimeOffset ReviewedAtUtc { get; init; }
    public string? Notes { get; init; }
}
