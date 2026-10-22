using StoryBuilder.Core.Models;

namespace StoryBuilder.Core.Services;

/// <summary>
/// Checks cross-record references after a workspace has been assembled or restored.
/// Validation is non-mutating: it reports broken links rather than guessing replacements.
/// </summary>
public static class StoryWorkspaceValidator
{
    public static IReadOnlyList<WorkspaceIntegrityIssue> Validate(StoryWorkspace workspace)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        var issues = new List<WorkspaceIntegrityIssue>();
        var timelines = workspace.Timelines.Select(x => x.Id).ToHashSet();
        var planningPoints = workspace.PlanningPoints.Select(x => x.Id).ToHashSet();
        var events = workspace.Events.Select(x => x.Id).ToHashSet();
        var characters = workspace.Characters.Select(x => x.Id).ToHashSet();
        var scenes = workspace.Scenes.Select(x => x.Id).ToHashSet();
        var manuscripts = workspace.Manuscripts.Select(x => x.Id).ToHashSet();
        var sources = workspace.Sources.Select(x => x.Id).ToHashSet();
        var campaigns = workspace.Campaigns.Select(x => x.Id).ToHashSet();
        var premises = workspace.Premises.Select(x => x.Id).ToHashSet();
        var claims = workspace.HistoricalClaims.Select(x => x.Id).ToHashSet();
        var knowledgeRecords = workspace.ObserverKnowledge.Select(x => x.Id).ToHashSet();
        var participantActions = workspace.ParticipantActions.Select(x => x.Id).ToHashSet();
        var continuityIssues = workspace.ContinuityIssues.Select(x => x.Id).ToHashSet();
        var knownRecords = timelines.Concat(planningPoints).Concat(events).Concat(characters)
            .Concat(scenes).Concat(manuscripts).Concat(sources).Concat(campaigns).Concat(premises)
            .Concat(claims).Concat(knowledgeRecords).Concat(participantActions).Concat(continuityIssues).ToHashSet();

        void Missing(string ownerType, Guid ownerId, string relationship, Guid targetId, HashSet<Guid> targetIds)
        {
            if (!targetIds.Contains(targetId))
                issues.Add(new WorkspaceIntegrityIssue(ownerType, ownerId, relationship, targetId,
                    $"'{ownerType}' record '{ownerId}' references missing {relationship} record '{targetId}'."));
        }

        foreach (var timeline in workspace.Timelines)
            if (timeline.ParentTimelineId is Guid parentId)
                Missing(nameof(TimelineDefinition), timeline.Id, "parent timeline", parentId, timelines);

        foreach (var point in workspace.PlanningPoints)
            Missing(nameof(TimelinePlanningPoint), point.Id, "timeline", point.TimelineId, timelines);

        foreach (var storyEvent in workspace.Events)
        {
            if (storyEvent.TimelineId is Guid timelineId)
                Missing(nameof(StoryEvent), storyEvent.Id, "timeline", timelineId, timelines);
            foreach (var id in storyEvent.ParticipantIds)
                Missing(nameof(StoryEvent), storyEvent.Id, "participant character", id, characters);
            foreach (var id in storyEvent.CauseEventIds)
                Missing(nameof(StoryEvent), storyEvent.Id, "cause event", id, events);
            foreach (var id in storyEvent.ConsequenceEventIds)
                Missing(nameof(StoryEvent), storyEvent.Id, "consequence event", id, events);
            foreach (var id in storyEvent.SourceIds)
                Missing(nameof(StoryEvent), storyEvent.Id, "source", id, sources);
        }

        foreach (var claim in workspace.HistoricalClaims)
        {
            foreach (var id in claim.SourceIds)
                Missing(nameof(HistoricalClaim), claim.Id, "source", id, sources);
            foreach (var id in claim.RelatedEntityIds)
                Missing(nameof(HistoricalClaim), claim.Id, "related record", id, knownRecords);
        }

        foreach (var manuscript in workspace.Manuscripts)
        foreach (var section in manuscript.Sections)
        {
            foreach (var id in section.SceneIds)
                Missing(nameof(ManuscriptSection), section.Id, "scene", id, scenes);
            foreach (var id in section.EventIds)
                Missing(nameof(ManuscriptSection), section.Id, "event", id, events);
        }

        foreach (var scene in workspace.Scenes)
        foreach (var beat in scene.Beats)
        {
            if (beat.ActorId is Guid actorId)
                Missing(nameof(SceneBeat), beat.Id, "actor character", actorId, characters);
            foreach (var id in beat.EventIds)
                Missing(nameof(SceneBeat), beat.Id, "event", id, events);
        }

        foreach (var knowledge in workspace.ObserverKnowledge)
        {
            Missing(nameof(ObserverKnowledgeRecord), knowledge.Id, "event", knowledge.EventId, events);
            if (knowledge.AcquiredAtEventId is Guid acquiredAtId)
                Missing(nameof(ObserverKnowledgeRecord), knowledge.Id, "acquisition event", acquiredAtId, events);
            if (knowledge.SourceId is Guid sourceId)
                Missing(nameof(ObserverKnowledgeRecord), knowledge.Id, "source", sourceId, sources);
        }

        foreach (var campaign in workspace.Campaigns)
        {
            foreach (var id in campaign.ParticipantCharacterIds)
                Missing(nameof(CampaignRecord), campaign.Id, "participant character", id, characters);
            foreach (var id in campaign.SceneIds)
                Missing(nameof(CampaignRecord), campaign.Id, "scene", id, scenes);
            foreach (var id in campaign.EventIds)
                Missing(nameof(CampaignRecord), campaign.Id, "event", id, events);
        }

        foreach (var action in workspace.ParticipantActions)
        {
            Missing(nameof(ParticipantAction), action.Id, "campaign", action.CampaignId, campaigns);
            if (action.CharacterId is Guid characterId)
                Missing(nameof(ParticipantAction), action.Id, "character", characterId, characters);
            if (action.ResultingEventId is Guid eventId)
                Missing(nameof(ParticipantAction), action.Id, "resulting event", eventId, events);
        }

        foreach (var issue in workspace.ContinuityIssues)
        foreach (var id in issue.RelatedRecordIds)
            Missing(nameof(ContinuityIssue), issue.Id, "related record", id, knownRecords);

        return issues;
    }
}

/// <summary>A single unresolved cross-record reference found by <see cref="StoryWorkspaceValidator"/>.</summary>
public sealed record WorkspaceIntegrityIssue(
    string OwnerType,
    Guid OwnerId,
    string Relationship,
    Guid MissingTargetId,
    string Message);
