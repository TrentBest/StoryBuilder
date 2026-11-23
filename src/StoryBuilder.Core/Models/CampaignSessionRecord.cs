namespace StoryBuilder.Core.Models;

/// <summary>Lifecycle state of a scheduled or executed campaign session.</summary>
public enum CampaignSessionStatus
{
    Planned,
    Prepared,
    InProgress,
    Completed,
    Cancelled
}

/// <summary>
/// A single table session in a campaign. It records what actually happened without
/// conflating preparation notes, participant declarations, or accepted world canon.
/// </summary>
public sealed record CampaignSessionRecord
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid CampaignId { get; init; }
    public int Sequence { get; init; } = 1;
    public string Title { get; init; } = "Untitled session";
    public CampaignSessionStatus Status { get; init; } = CampaignSessionStatus.Planned;
    public DateTimeOffset? StartedAtUtc { get; init; }
    public DateTimeOffset? EndedAtUtc { get; init; }
    public string Summary { get; init; } = string.Empty;
    public string GameMasterNotes { get; init; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
}
