namespace StoryBuilder.Core.Models;

/// <summary>
/// An observer-safe set of accepted knowledge claims. It deliberately excludes world-event titles,
/// summaries, and causal edges so consumers cannot mistake world truth for the observer's account.
/// </summary>
public sealed record ObserverKnowledgeProjection(
    Guid ObserverId,
    Guid? AsOfEventId,
    IReadOnlyList<ObserverKnownEvent> Events);

/// <summary>A world-event identity exposed only because accepted observer knowledge permits a claim about it.</summary>
public sealed record ObserverKnownEvent(
    Guid EventId,
    Guid? TimelineId,
    IReadOnlyList<ObserverPermittedClaim> PermittedClaims);

/// <summary>The exact accepted knowledge record and narration mode that may support a portrayal.</summary>
public sealed record ObserverPermittedClaim(
    NarrationClaimMode Mode,
    Guid KnowledgeRecordId,
    ObserverKnowledgeKind Kind,
    string Attribution,
    string Note,
    Guid? SourceId,
    double? Confidence);
