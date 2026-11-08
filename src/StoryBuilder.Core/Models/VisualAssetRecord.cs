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

/// <summary>The review state of one immutable visual asset revision.</summary>
public enum VisualAssetRevisionStatus
{
    Proposed,
    InReview,
    Approved,
    Rejected
}

/// <summary>A versioned visual artifact. Revisions are append-only; approval belongs to one exact revision.</summary>
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
    public VisualAssetRevisionStatus Status { get; init; } = VisualAssetRevisionStatus.Proposed;
    public string? ReviewedBy { get; init; }
    public DateTimeOffset? ReviewedAtUtc { get; init; }
    public string? ReviewNotes { get; init; }
}
