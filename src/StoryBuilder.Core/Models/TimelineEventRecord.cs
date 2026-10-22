namespace StoryBuilder.Core.Models;

/// <summary>
/// An inspectable runtime observation emitted by a timeline Process Group.
/// This is execution evidence, not an automatic declaration of accepted story canon.
/// </summary>
public sealed record TimelineEventRecord
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid TimelineId { get; init; }
    public long Sequence { get; init; }
    public string Kind { get; init; } = "Observation";
    public string Description { get; init; } = string.Empty;
    public DateTimeOffset RecordedAtUtc { get; init; } = DateTimeOffset.UtcNow;
}
