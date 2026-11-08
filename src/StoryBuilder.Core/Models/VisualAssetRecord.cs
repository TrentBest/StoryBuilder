namespace StoryBuilder.Core.Models;

/// <summary>What a visual asset contributes to a story composition.</summary>
public enum VisualAssetKind
{
    CharacterDesign,
    EnvironmentDesign,
    PropDesign,
    PageIllustration,
    MotionStudy
}

/// <summary>The decision recorded by an author or designated reviewer for one exact asset revision.</summary>
public enum VisualAssetReviewDecision
{
    Approved,
    Rejected,
    ChangesRequested
}

/// <summary>A versioned visual artifact. Revisions are append-only; review decisions are separate records.</summary>
public sealed record VisualAssetRecord
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; init; } = string.Empty;
    public VisualAssetKind Kind { get; init; }
    public Guid? CharacterId { get; init; }
    public Guid? SceneId { get; init; }
    public Guid? ManuscriptSectionId { get; init; }
    public List<Guid> SourceIds { get; init; } = [];
    public List<VisualAssetRevision> Revisions { get; init; } = [];
}

/// <summary>A specific immutable file/prompt/provenance revision of a visual asset.</summary>
public sealed record VisualAssetRevision
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public int RevisionNumber { get; init; } = 1;
    /// <summary>Repository-relative path or durable URI; never an ephemeral local path.</summary>
    public string AssetLocation { get; init; } = string.Empty;
    public string MediaType { get; init; } = "image/png";
    public string? ContentSha256 { get; init; }
    public string? CreationPrompt { get; init; }
    public string? ProductionNotes { get; init; }
}

/// <summary>An append-only review decision tied to one exact asset revision.</summary>
public sealed record VisualAssetReview
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid AssetId { get; init; }
    public Guid RevisionId { get; init; }
    public VisualAssetReviewDecision Decision { get; init; }
    public string Reviewer { get; init; } = string.Empty;
    public DateTimeOffset ReviewedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public string? Notes { get; init; }
}
