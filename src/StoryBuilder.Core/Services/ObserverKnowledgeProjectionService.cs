using StoryBuilder.Core.Models;

namespace StoryBuilder.Core.Services;

/// <summary>
/// Projects only accepted, causally available observer knowledge for downstream narration or portrayal.
/// World-event titles and summaries are intentionally not copied into the projection.
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
        if (asOfEventId is Guid anchor && !workspace.Events.Any(storyEvent => storyEvent.Id == anchor))
            throw new KeyNotFoundException($"Narration event anchor '{anchor}' does not exist in this workspace.");

        var projectedEvents = new List<ObserverKnownEvent>();
        foreach (var storyEvent in workspace.Events)
        {
            var permittedClaims = new List<ObserverPermittedClaim>();
            foreach (var mode in ClaimModes)
            {
                var evaluation = _evaluator.Evaluate(workspace, observerId, storyEvent.Id, asOfEventId, mode);
                if (!evaluation.IsAllowed || evaluation.KnowledgeRecordId is not Guid knowledgeId)
                    continue;

                var record = workspace.ObserverKnowledge.FirstOrDefault(item => item.Id == knowledgeId);
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
}
