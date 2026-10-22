using StoryBuilder.Core.Models;

namespace StoryBuilder.Core.Services;

/// <summary>
/// Projects only accepted, causally available observer knowledge for downstream narration or portrayal.
/// Uses per-event indexes and one causal-history traversal per projection.
/// </summary>
public sealed class ObserverKnowledgeProjectionService
{
    private static readonly NarrationClaimMode[] ClaimModes =
    [
        NarrationClaimMode.DirectFact,
        NarrationClaimMode.AttributedReport,
        NarrationClaimMode.Inference,
        NarrationClaimMode.Belief
    ];

    private readonly ObserverKnowledgeEvaluator _evaluator = new();

    public ObserverKnowledgeProjection Project(StoryWorkspace workspace, Guid observerId, Guid? asOfEventId)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        if (observerId == Guid.Empty || !workspace.Characters.Any(character => character.Id == observerId))
            throw new KeyNotFoundException($"Observer character '{observerId}' does not exist in this workspace.");

        var eventsById = workspace.Events.ToDictionary(storyEvent => storyEvent.Id);
        if (asOfEventId is Guid anchor && !eventsById.ContainsKey(anchor))
            throw new KeyNotFoundException($"Narration event anchor '{anchor}' does not exist in this workspace.");

        var recordsByEvent = workspace.ObserverKnowledge
            .Where(record => record.ObserverId == observerId)
            .GroupBy(record => record.EventId)
            .ToDictionary(group => group.Key, group => (IReadOnlyList<ObserverKnowledgeRecord>)group.ToArray());
        var causalHistory = BuildCausalHistory(eventsById, asOfEventId);
        var projectedEvents = new List<ObserverKnownEvent>();

        foreach (var storyEvent in workspace.Events)
        {
            if (!recordsByEvent.TryGetValue(storyEvent.Id, out var records))
                continue;

            var permittedClaims = new List<ObserverPermittedClaim>();
            foreach (var mode in ClaimModes)
            {
                var evaluation = _evaluator.EvaluateForProjection(records, asOfEventId, mode, causalHistory);
                if (!evaluation.IsAllowed || evaluation.KnowledgeRecordId is not Guid knowledgeId)
                    continue;

                var record = records.FirstOrDefault(item => item.Id == knowledgeId);
                if (record is null || record.Status != CanonStatus.Accepted)
                    continue;

                permittedClaims.Add(new ObserverPermittedClaim(
                    mode,
                    record.Id,
                    record.Kind,
                    record.Attribution,
                    record.Note,
                    record.SourceId,
                    record.Confidence));
            }

            if (permittedClaims.Count > 0)
                projectedEvents.Add(new ObserverKnownEvent(storyEvent.Id, storyEvent.TimelineId, permittedClaims));
        }

        return new ObserverKnowledgeProjection(observerId, asOfEventId, projectedEvents);
    }

    private static HashSet<Guid> BuildCausalHistory(
        IReadOnlyDictionary<Guid, StoryEvent> eventsById,
        Guid? asOfEventId)
    {
        var history = new HashSet<Guid>();
        if (asOfEventId is not Guid anchor)
            return history;

        var pending = new Stack<Guid>();
        pending.Push(anchor);
        while (pending.Count > 0)
        {
            var currentId = pending.Pop();
            if (!history.Add(currentId) || !eventsById.TryGetValue(currentId, out var current))
                continue;
            foreach (var causeId in current.CauseEventIds)
                pending.Push(causeId);
        }

        return history;
    }
}
