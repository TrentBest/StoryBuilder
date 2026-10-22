namespace StoryBuilder.Core.Models;

/// <summary>An explicit record of what an observer knows about a world event and how that knowledge was acquired.</summary>
public sealed record ObserverKnowledgeRecord
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid ObserverId { get; init; }
    public Guid EventId { get; init; }
    public ObserverKnowledgeKind Kind { get; init; } = ObserverKnowledgeKind.Believes;
    public Guid? AcquiredAtEventId { get; init; }
    public Guid? SourceId { get; init; }
    public string Attribution { get; init; } = string.Empty;
    public string Note { get; init; } = string.Empty;
    public double? Confidence { get; init; }
    public CanonStatus Status { get; init; } = CanonStatus.Proposed;
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
}

/// <summary>How the observer relates to the event, distinct from whether the event is objectively true.</summary>
public enum ObserverKnowledgeKind
{
    Witnessed,
    Told,
    Inferred,
    Believes,
    Disputed,
    Forgotten
}

/// <summary>How a requested narration sentence frames an event claim.</summary>
public enum NarrationClaimMode
{
    DirectFact,
    AttributedReport,
    Inference,
    Belief
}

/// <summary>Result of checking a narration claim against an observer's accepted knowledge.</summary>
public sealed record ObserverKnowledgeEvaluation
{
    public bool IsAllowed { get; init; }
    public bool IsUnresolved { get; init; }
    public NarrationClaimMode? RequiredMode { get; init; }
    public string Explanation { get; init; } = string.Empty;
    public Guid? KnowledgeRecordId { get; init; }
}
