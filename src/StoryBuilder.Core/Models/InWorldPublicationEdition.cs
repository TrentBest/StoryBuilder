namespace StoryBuilder.Core.Models;

/// <summary>
/// An immutable snapshot of one artifact issued within the story world. In-world publication
/// time is authored text/period, deliberately separate from event time and host UTC metadata.
/// The record contains no distribution, delivery, receipt, or presentation state.
/// </summary>
public sealed record InWorldPublicationEdition
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid OutletId { get; init; }
    public int EditionNumber { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string ContentFormat { get; init; } = "text/plain";
    public string ContentSha256 { get; init; } = string.Empty;
    public string InWorldPublishedAt { get; init; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public string IntendedAudience { get; init; } = string.Empty;
    public string AccessScope { get; init; } = string.Empty;
    public IReadOnlyList<Guid> ClaimIds { get; init; } = Array.Empty<Guid>();
    public IReadOnlyList<Guid> SourceIds { get; init; } = Array.Empty<Guid>();
    public IReadOnlyList<Guid> EventIds { get; init; } = Array.Empty<Guid>();
    public Guid? SupersedesEditionId { get; init; }
    public string? CorrectionNotes { get; init; }
}
