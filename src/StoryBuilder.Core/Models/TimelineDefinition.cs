namespace StoryBuilder.Core.Models;

/// <summary>
/// Stable domain identity for one timeline. Runtime Process Groups and presentation numbers
/// are projections of this identity; neither becomes the source of truth.
/// </summary>
public sealed record TimelineDefinition
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = "Untitled timeline";
    public string Description { get; init; } = string.Empty;
    public Guid? ParentTimelineId { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public bool IsBranch => ParentTimelineId.HasValue;
}
