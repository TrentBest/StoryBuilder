namespace StoryBuilder.Core.Models;

/// <summary>
/// Immutable campaign attachment to one exact sourcebook edition. Entry IDs are stable identities
/// within the sourcebook; the edition and content hash pin the material used during play.
/// </summary>
public sealed record SourcebookAttachment
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid CampaignId { get; init; }
    public string SourcebookId { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Edition { get; init; } = string.Empty;
    public string ContentSha256 { get; init; } = string.Empty;
    public string SourceUri { get; init; } = string.Empty;
    public IReadOnlyList<string> EntryIds { get; init; } = Array.Empty<string>();
    public DateTimeOffset AttachedAtUtc { get; init; } = DateTimeOffset.UtcNow;
}
