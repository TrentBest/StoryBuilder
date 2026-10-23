using System.Security.Cryptography;
using System.Text;
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
        var places = workspace.Places.Select(x => x.Id).ToHashSet();
        var scenes = workspace.Scenes.Select(x => x.Id).ToHashSet();
        var manuscripts = workspace.Manuscripts.Select(x => x.Id).ToHashSet();
        var publishedEditions = workspace.PublishedEditions.Select(x => x.Id).ToHashSet();
        var sources = workspace.Sources.Select(x => x.Id).ToHashSet();
        var campaigns = workspace.Campaigns.Select(x => x.Id).ToHashSet();
        var premises = workspace.Premises.Select(x => x.Id).ToHashSet();
        var claims = workspace.HistoricalClaims.Select(x => x.Id).ToHashSet();
        var knowledgeRecords = workspace.ObserverKnowledge.Select(x => x.Id).ToHashSet();
        var participantActions = workspace.ParticipantActions.Select(x => x.Id).ToHashSet();
        var continuityIssues = workspace.ContinuityIssues.Select(x => x.Id).ToHashSet();
        var knownRecords = timelines.Concat(planningPoints).Concat(events).Concat(characters)
            .Concat(scenes).Concat(manuscripts).Concat(publishedEditions).Concat(sources).Concat(campaigns).Concat(premises)
            .Concat(claims).Concat(knowledgeRecords).Concat(participantActions).Concat(continuityIssues).Concat(places)
            .Concat(workspace.Manuscripts.SelectMany(item => item.Sections).Select(item => item.Id))
            .Concat(workspace.Scenes.SelectMany(item => item.Beats).Select(item => item.Id))
            .Append(workspace.Experience.Id)
            .Concat(workspace.Experience.Decisions.Select(item => item.Id)).ToHashSet();

        void Missing(string ownerType, Guid ownerId, string relationship, Guid targetId, HashSet<Guid> targetIds)
        {
            if (!targetIds.Contains(targetId))
                issues.Add(new WorkspaceIntegrityIssue(ownerType, ownerId, relationship, targetId,
                    $"'{ownerType}' record '{ownerId}' references missing {relationship} record '{targetId}'."));
        }

        // IDs are the identity boundary used by every relationship in a workspace.
        // A HashSet alone would silently collapse duplicate IDs and let ambiguous records persist.
        void DuplicateIds<T>(IEnumerable<T> records, Func<T, Guid> getId, string recordType)
        {
            foreach (var duplicate in records.GroupBy(getId).Where(group => group.Count() > 1))
                issues.Add(new WorkspaceIntegrityIssue(recordType, duplicate.Key, "duplicate identity",
                    duplicate.Key, $"'{recordType}' contains {duplicate.Count()} records with duplicate ID '{duplicate.Key}'."));
        }

        DuplicateIds(new[] { workspace.Experience }, item => item.Id, nameof(ExperienceDefinition));
        DuplicateIds(workspace.Experience.Decisions, item => item.Id, nameof(DesignDecision));
        DuplicateIds(workspace.Timelines, item => item.Id, nameof(TimelineDefinition));
        DuplicateIds(workspace.PlanningPoints, item => item.Id, nameof(TimelinePlanningPoint));
        DuplicateIds(workspace.Events, item => item.Id, nameof(StoryEvent));
        DuplicateIds(workspace.Characters, item => item.Id, nameof(CharacterProfile));
        DuplicateIds(workspace.Places, item => item.Id, nameof(PlaceRecord));
        DuplicateIds(workspace.Scenes, item => item.Id, nameof(SceneRecord));
        DuplicateIds(workspace.Manuscripts, item => item.Id, nameof(ManuscriptEdition));
        DuplicateIds(workspace.PublishedEditions, item => item.Id, nameof(PublishedBookEdition));
        DuplicateIds(workspace.Premises, item => item.Id, nameof(StoryPremise));
        DuplicateIds(workspace.Sources, item => item.Id, nameof(SourceRecord));
        DuplicateIds(workspace.Campaigns, item => item.Id, nameof(CampaignRecord));
        DuplicateIds(workspace.HistoricalClaims, item => item.Id, nameof(HistoricalClaim));
        DuplicateIds(workspace.ObserverKnowledge, item => item.Id, nameof(ObserverKnowledgeRecord));
        DuplicateIds(workspace.ParticipantActions, item => item.Id, nameof(ParticipantAction));
        DuplicateIds(workspace.ContinuityIssues, item => item.Id, nameof(ContinuityIssue));
        DuplicateIds(workspace.Manuscripts.SelectMany(item => item.Sections), item => item.Id, nameof(ManuscriptSection));
        DuplicateIds(workspace.Scenes.SelectMany(item => item.Beats), item => item.Id, nameof(SceneBeat));

        // IDs are workspace-wide identities. Per-collection checks above catch duplicates
        // within a type; this catches a GUID reused by different record types, which would
        // make generic related-record links ambiguous.
        var typedIds = new List<(Guid Id, string Type)>();
        typedIds.Add((workspace.Experience.Id, nameof(ExperienceDefinition)));
        typedIds.AddRange(workspace.Experience.Decisions.Select(item => (item.Id, nameof(DesignDecision))));
        typedIds.AddRange(workspace.Timelines.Select(item => (item.Id, nameof(TimelineDefinition))));
        typedIds.AddRange(workspace.PlanningPoints.Select(item => (item.Id, nameof(TimelinePlanningPoint))));
        typedIds.AddRange(workspace.Events.Select(item => (item.Id, nameof(StoryEvent))));
        typedIds.AddRange(workspace.Characters.Select(item => (item.Id, nameof(CharacterProfile))));
        typedIds.AddRange(workspace.Places.Select(item => (item.Id, nameof(PlaceRecord))));
        typedIds.AddRange(workspace.Scenes.Select(item => (item.Id, nameof(SceneRecord))));
        typedIds.AddRange(workspace.Scenes.SelectMany(item => item.Beats).Select(item => (item.Id, nameof(SceneBeat))));
        typedIds.AddRange(workspace.Manuscripts.Select(item => (item.Id, nameof(ManuscriptEdition))));
        typedIds.AddRange(workspace.PublishedEditions.Select(item => (item.Id, nameof(PublishedBookEdition))));
        typedIds.AddRange(workspace.Manuscripts.SelectMany(item => item.Sections).Select(item => (item.Id, nameof(ManuscriptSection))));
        typedIds.AddRange(workspace.Sources.Select(item => (item.Id, nameof(SourceRecord))));
        typedIds.AddRange(workspace.Campaigns.Select(item => (item.Id, nameof(CampaignRecord))));
        typedIds.AddRange(workspace.Premises.Select(item => (item.Id, nameof(StoryPremise))));
        typedIds.AddRange(workspace.HistoricalClaims.Select(item => (item.Id, nameof(HistoricalClaim))));
        typedIds.AddRange(workspace.ObserverKnowledge.Select(item => (item.Id, nameof(ObserverKnowledgeRecord))));
        typedIds.AddRange(workspace.ParticipantActions.Select(item => (item.Id, nameof(ParticipantAction))));
        typedIds.AddRange(workspace.ContinuityIssues.Select(item => (item.Id, nameof(ContinuityIssue))));

        foreach (var collision in typedIds.GroupBy(item => item.Id)
            .Where(group => group.Select(item => item.Type).Distinct().Count() > 1))
        {
            var types = string.Join(", ", collision.Select(item => item.Type).Distinct().OrderBy(type => type));
            issues.Add(new WorkspaceIntegrityIssue("Workspace", collision.Key, "cross-type duplicate identity",
                collision.Key, $"Workspace ID '{collision.Key}' is reused across record types: {types}."));
        }

        foreach (var timeline in workspace.Timelines)
            if (timeline.ParentTimelineId is Guid parentId)
                Missing(nameof(TimelineDefinition), timeline.Id, "parent timeline", parentId, timelines);

        foreach (var place in workspace.Places)
            if (place.ParentPlaceId is Guid parentPlaceId)
                Missing(nameof(PlaceRecord), place.Id, "parent place", parentPlaceId, places);

        foreach (var point in workspace.PlanningPoints)
            Missing(nameof(TimelinePlanningPoint), point.Id, "timeline", point.TimelineId, timelines);

        foreach (var point in workspace.PlanningPoints)
        {
            foreach (var sceneId in point.RelatedSceneIds)
                Missing(nameof(TimelinePlanningPoint), point.Id, "related scene", sceneId, scenes);
            foreach (var eventId in point.RelatedEventIds)
                Missing(nameof(TimelinePlanningPoint), point.Id, "related event", eventId, events);
            if (point.PromotedEventId is Guid promotedEventId)
            {
                Missing(nameof(TimelinePlanningPoint), point.Id, "promoted event", promotedEventId, events);
                if (point.Status != TimelinePlanningStatus.Superseded)
                    issues.Add(new WorkspaceIntegrityIssue(nameof(TimelinePlanningPoint), point.Id, "promotion status", promotedEventId,
                        $"Planning point '{point.Id}' has a promoted event but is not marked Superseded."));
            }
        }

        // Character relationships are domain links, not merely vocabulary text.
        // Validate them against the workspace so imported/restored profiles cannot silently
        // point at characters that do not exist.
        foreach (var character in workspace.Characters)
        {
            foreach (var relationship in character.Relationships)
                Missing(nameof(CharacterProfile), character.Id, "related character",
                    relationship.TargetCharacterId, characters);
        }

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

        foreach (var edition in workspace.PublishedEditions)
        {
            Missing(nameof(PublishedBookEdition), edition.Id, "manuscript", edition.ManuscriptId, manuscripts);
            if (edition.PreviousEditionId is Guid previousId)
            {
                Missing(nameof(PublishedBookEdition), edition.Id, "previous publication edition", previousId, publishedEditions);
                var previous = workspace.PublishedEditions.FirstOrDefault(item => item.Id == previousId);
                if (previous is not null && previous.ManuscriptId != edition.ManuscriptId)
                    issues.Add(new WorkspaceIntegrityIssue(nameof(PublishedBookEdition), edition.Id, "publication lineage manuscript", previousId,
                        $"Published edition '{edition.Id}' links to an edition of a different manuscript."));
            }
            foreach (var sectionId in edition.SectionIds)
            {
                if (!workspace.Manuscripts.Any(item => item.Id == edition.ManuscriptId && item.Sections.Any(section => section.Id == sectionId)))
                    issues.Add(new WorkspaceIntegrityIssue(nameof(PublishedBookEdition), edition.Id, "published section", sectionId,
                        $"Published edition '{edition.Id}' references section '{sectionId}' that is not in its source manuscript."));
            }
            foreach (var sceneId in edition.SceneIds)
                Missing(nameof(PublishedBookEdition), edition.Id, "published scene", sceneId, scenes);
            foreach (var eventId in edition.EventIds)
                Missing(nameof(PublishedBookEdition), edition.Id, "published event", eventId, events);

            var expectedHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(edition.Markdown)));
            if (!string.Equals(expectedHash, edition.ContentSha256, StringComparison.OrdinalIgnoreCase))
                issues.Add(new WorkspaceIntegrityIssue(nameof(PublishedBookEdition), edition.Id, "content hash", edition.Id,
                    $"Published edition '{edition.Id}' does not match its stored SHA-256 content hash."));
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
        {
            if (scene.PlaceId is Guid placeId)
                Missing(nameof(SceneRecord), scene.Id, "place", placeId, places);

            foreach (var beat in scene.Beats)
            {
                if (beat.ActorId is Guid actorId)
                    Missing(nameof(SceneBeat), beat.Id, "actor character", actorId, characters);
                foreach (var id in beat.EventIds)
                    Missing(nameof(SceneBeat), beat.Id, "event", id, events);
            }
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
