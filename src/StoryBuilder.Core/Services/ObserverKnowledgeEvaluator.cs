using StoryBuilder.Core.Models;

namespace StoryBuilder.Core.Services;

/// <summary>
/// Conservatively checks whether an observer may make a requested claim about an event.
/// It has no Renderer dependency; portrayal providers consume the result as an authoring constraint.
/// </summary>
public sealed class ObserverKnowledgeEvaluator
{
    public ObserverKnowledgeEvaluation Evaluate(
        StoryWorkspace workspace,
        Guid observerId,
        Guid eventId,
        Guid? asOfEventId,
        NarrationClaimMode requestedMode)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        var storyEvent = workspace.Events.FirstOrDefault(item => item.Id == eventId);
        if (storyEvent is null)
            return Deny("The referenced world event does not exist in this workspace.");

        var records = workspace.ObserverKnowledge
            .Where(item => item.ObserverId == observerId && item.EventId == eventId)
            .ToArray();
        return EvaluateRecords(records, requestedMode,
            record => IsAvailableAt(workspace, record.AcquiredAtEventId, asOfEventId));
    }

    /// <summary>
    /// Evaluates records already indexed for one observer/event against a precomputed causal history.
    /// Used by batch projections to avoid rescanning the full workspace for each event.
    /// </summary>
    internal ObserverKnowledgeEvaluation EvaluateForProjection(
        IReadOnlyList<ObserverKnowledgeRecord> records,
        Guid? asOfEventId,
        NarrationClaimMode requestedMode,
        IReadOnlySet<Guid> causalHistory)
    {
        ArgumentNullException.ThrowIfNull(records);
        ArgumentNullException.ThrowIfNull(causalHistory);
        return EvaluateRecords(records, requestedMode,
            record => IsAvailableAt(record.AcquiredAtEventId, asOfEventId, causalHistory));
    }

    private static ObserverKnowledgeEvaluation EvaluateRecords(
        IReadOnlyList<ObserverKnowledgeRecord> records,
        NarrationClaimMode requestedMode,
        Func<ObserverKnowledgeRecord, bool> isAvailable)
    {
        if (records.Count == 0)
            return Deny("No observer-knowledge record establishes that this observer knows the event.");

        var accepted = records.Where(item => item.Status == CanonStatus.Accepted).ToList();
        if (accepted.Count == 0)
            return new ObserverKnowledgeEvaluation
            {
                IsUnresolved = true,
                Explanation = "Observer knowledge is only proposed; an author must accept it before narration relies on it."
            };

        var available = accepted.Where(isAvailable).ToList();
        if (available.Count == 0)
            return Deny("The event is not established as known at this narration point.");

        if (available.Any(item => item.Kind == ObserverKnowledgeKind.Forgotten))
            return Deny("The accepted observer record marks this event as forgotten at this narration point.");

        foreach (var record in available.OrderByDescending(item => item.CreatedAtUtc))
        {
            if (record.Kind == ObserverKnowledgeKind.Told && record.SourceId is null && string.IsNullOrWhiteSpace(record.Attribution))
                continue;
            var required = RequiredMode(record.Kind);
            if (required is not null && ModeSatisfies(requestedMode, required.Value))
                return new ObserverKnowledgeEvaluation
                {
                    IsAllowed = true,
                    RequiredMode = required,
                    KnowledgeRecordId = record.Id,
                    Explanation = ExplanationFor(record.Kind, required.Value)
                };
        }

        var strongest = available.OrderBy(item => RequiredMode(item.Kind) is null ? int.MaxValue : (int)RequiredMode(item.Kind)!.Value).First();
        var requiredMode = RequiredMode(strongest.Kind);
        return new ObserverKnowledgeEvaluation
        {
            IsAllowed = false,
            RequiredMode = requiredMode,
            KnowledgeRecordId = strongest.Id,
            Explanation = requiredMode is null
                ? "This knowledge state does not support a narration claim."
                : $"This observer can support the claim only when framed as {requiredMode.Value}."
        };
    }

    private static NarrationClaimMode? RequiredMode(ObserverKnowledgeKind kind) => kind switch
    {
        ObserverKnowledgeKind.Witnessed => NarrationClaimMode.DirectFact,
        ObserverKnowledgeKind.Told => NarrationClaimMode.AttributedReport,
        ObserverKnowledgeKind.Inferred => NarrationClaimMode.Inference,
        ObserverKnowledgeKind.Believes or ObserverKnowledgeKind.Disputed => NarrationClaimMode.Belief,
        ObserverKnowledgeKind.Forgotten => null,
        _ => null
    };

    private static bool ModeSatisfies(NarrationClaimMode requested, NarrationClaimMode required) => required switch
    {
        NarrationClaimMode.DirectFact => requested == NarrationClaimMode.DirectFact,
        NarrationClaimMode.AttributedReport => requested == NarrationClaimMode.AttributedReport,
        NarrationClaimMode.Inference => requested is NarrationClaimMode.Inference or NarrationClaimMode.Belief,
        NarrationClaimMode.Belief => requested == NarrationClaimMode.Belief,
        _ => false
    };

    private static bool IsAvailableAt(StoryWorkspace workspace, Guid? acquiredAtEventId, Guid? asOfEventId)
    {
        if (acquiredAtEventId is null)
            return true;
        if (asOfEventId is null)
            return false;
        if (acquiredAtEventId == asOfEventId)
            return true;

        var visited = new HashSet<Guid>();
        var pending = new Stack<Guid>();
        pending.Push(asOfEventId.Value);
        while (pending.Count > 0)
        {
            var currentId = pending.Pop();
            if (!visited.Add(currentId))
                continue;
            var current = workspace.Events.FirstOrDefault(item => item.Id == currentId);
            if (current is null)
                continue;
            foreach (var causeId in current.CauseEventIds)
            {
                if (causeId == acquiredAtEventId.Value)
                    return true;
                pending.Push(causeId);
            }
        }

        return false;
    }

    private static bool IsAvailableAt(Guid? acquiredAtEventId, Guid? asOfEventId, IReadOnlySet<Guid> causalHistory)
    {
        if (acquiredAtEventId is null)
            return true;
        if (asOfEventId is null)
            return false;
        return causalHistory.Contains(acquiredAtEventId.Value);
    }

    private static string ExplanationFor(ObserverKnowledgeKind kind, NarrationClaimMode required) => kind switch
    {
        ObserverKnowledgeKind.Witnessed => "The accepted record says the observer witnessed the event.",
        ObserverKnowledgeKind.Told => "The accepted record permits the event only as an attributed report.",
        ObserverKnowledgeKind.Inferred => "The accepted record permits the event only as an inference.",
        ObserverKnowledgeKind.Believes => "The accepted record supports a belief, not objective fact.",
        ObserverKnowledgeKind.Disputed => "The accepted record marks this as disputed knowledge.",
        _ => $"The requested claim must be framed as {required}."
    };

    private static ObserverKnowledgeEvaluation Deny(string explanation) => new()
    {
        IsAllowed = false,
        Explanation = explanation
    };
}
