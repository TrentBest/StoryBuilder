namespace StoryBuilder.Core.Models;

/// <summary>A named place in the story world, with stable identity and optional containment.</summary>
public sealed record PlaceRecord
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = "Unnamed place";
    public string Description { get; init; } = string.Empty;
    public string Kind { get; init; } = "Place";
    public Guid? ParentPlaceId { get; init; }
    public CanonStatus Status { get; init; } = CanonStatus.Draft;
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
}
