namespace StoryBuilder.Core.Models;

/// <summary>
/// An intentionally incomplete authoring anchor on a timeline. It is not a world event or accepted canon by itself.
/// </summary>
public sealed record TimelinePlanningPoint
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid TimelineId { get; init; }
    public string Label { get; init; } = "Plot Twist";
    public string Intent { get; init; } = string.Empty;
    public string ApproximatePlacement { get; init; } = string.Empty;
    public TimelinePlacementKind PlacementKind { get; init; } = TimelinePlacementKind.Unresolved;
    public TimelinePlanningStatus Status { get; init; } = TimelinePlanningStatus.Placeholder;
    public List<string> Alternatives { get; init; } = [];
    public List<Guid> RelatedSceneIds { get; init; } = [];
    public List<Guid> RelatedEventIds { get; init; } = [];
    /// <summary>The canonical event created when the author explicitly promotes this planning point.</summary>
    public Guid? PromotedEventId { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
}

/// <summary>How precisely a planning point's place in a timeline is known.</summary>
public enum TimelinePlacementKind
{
    Unresolved,
    Approximate,
    Relative,
    Range
}

/// <summary>Authoring progress for a planning point; acceptance does not happen implicitly.</summary>
public enum TimelinePlanningStatus
{
    Placeholder,
    Brainstorming,
    Specified,
    ReadyForReview,
    Accepted,
    Superseded
}
