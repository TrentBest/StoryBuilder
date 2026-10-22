namespace StoryBuilder.Core.Models;

/// <summary>How a scene's material was captured.</summary>
public enum SceneCaptureMode
{
    Freeform,
    Played,
    BeatByBeat,
    PoseSequence,
    Claymation
}

/// <summary>The lifecycle state of a scene.</summary>
public enum SceneStatus
{
    Sketch,
    InProgress,
    ReadyForReview,
    Accepted,
    Archived
}

/// <summary>A scene made from small, reorderable observable beats.</summary>
public sealed record SceneRecord
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; init; } = "Untitled scene";
    public string? Location { get; init; }
    public string? Purpose { get; init; }
    public SceneCaptureMode CaptureMode { get; init; } = SceneCaptureMode.Freeform;
    public SceneStatus Status { get; init; } = SceneStatus.Sketch;
    public List<SceneBeat> Beats { get; init; } = [];
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAtUtc { get; init; } = DateTimeOffset.UtcNow;

    public SceneRecord AddBeat(SceneBeat beat)
    {
        var nextSequence = Beats.Count == 0 ? 1 : Beats.Max(b => b.Sequence) + 1;
        var normalized = beat with { Sequence = beat.Sequence > 0 ? beat.Sequence : nextSequence };
        return this with { Beats = [.. Beats, normalized], Status = SceneStatus.InProgress, UpdatedAtUtc = DateTimeOffset.UtcNow };
    }

    public SceneRecord ReorderBeats(IEnumerable<Guid> beatIds)
    {
        var requested = beatIds.ToList();
        if (requested.Count != Beats.Count || requested.Distinct().Count() != Beats.Count ||
            requested.Any(id => Beats.All(b => b.Id != id)))
            throw new ArgumentException("The order must contain every scene beat exactly once.", nameof(beatIds));

        var byId = Beats.ToDictionary(b => b.Id);
        var reordered = requested.Select((id, index) => byId[id] with { Sequence = index + 1 }).ToList();
        return this with { Beats = reordered, UpdatedAtUtc = DateTimeOffset.UtcNow };
    }
}

/// <summary>A single action, reaction, line, transition, or pose in a scene.</summary>
public sealed record SceneBeat
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public int Sequence { get; init; }
    public SceneBeatKind Kind { get; init; } = SceneBeatKind.Action;
    public Guid? ActorId { get; init; }
    public string? ActorLabel { get; init; }
    public string? Action { get; init; }
    public string? Reaction { get; init; }
    public string? Dialogue { get; init; }
    public string? PoseOrPosition { get; init; }
    public string? EmotionalTone { get; init; }
    public string? CaptureNote { get; init; }
    public string Attribution { get; init; } = "Participant";
    public CanonStatus Status { get; init; } = CanonStatus.Proposed;
    public List<Guid> EventIds { get; init; } = [];
}

/// <summary>Meaningful beat categories used by the scene capture workflow.</summary>
public enum SceneBeatKind
{
    Action,
    Reaction,
    Dialogue,
    Pose,
    Transition,
    Narration
}
