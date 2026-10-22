using System.Security.Cryptography;
using System.Text;
using StoryBuilder.Core.Models;

namespace StoryBuilder.Core.Services;

/// <summary>
/// Coordinates common authoring operations so edits and the vocabulary index stay synchronized.
/// The session owns in-memory domain state; host adapters may persist it without coupling storage to the core.
/// </summary>
public sealed class StoryBuilderSession
{
    public StoryBuilderSession(string? workspaceName = null)
    {
        Workspace = new StoryWorkspace
        {
            Name = string.IsNullOrWhiteSpace(workspaceName) ? "StoryBuilder" : workspaceName.Trim()
        };
    }

    public StoryWorkspace Workspace { get; private set; }
    public CharacterBuilder CharacterBuilder { get; } = new();
    public SceneComposer SceneComposer { get; } = new();
    public VocabularyIndex Vocabulary { get; } = new();

    public event Action? Changed;

    /// <summary>Creates a canonical timeline definition, optionally branching from a timeline in this workspace.</summary>
    public TimelineDefinition CreateTimeline(string? name = null, string? description = null, Guid? parentTimelineId = null)
    {
        var timeline = new TimelineDefinition
        {
            Name = string.IsNullOrWhiteSpace(name) ? "Untitled timeline" : name.Trim(),
            Description = NormalizeOptional(description) ?? string.Empty,
            ParentTimelineId = parentTimelineId
        };
        Workspace.AddTimeline(timeline);
        NotifyChanged();
        return timeline;
    }

    /// <summary>Restores workspace records and explicit synonyms, rebuilding derived word occurrences.</summary>
    public void RestoreFrom(StoryWorkspace workspace, IEnumerable<VocabularyConcept>? concepts = null)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        Workspace = workspace;
        Vocabulary.Clear();
        RebuildVocabularyIndex();
        Vocabulary.ImportConcepts(concepts ?? Array.Empty<VocabularyConcept>());
    }

    public VocabularyConcept AddSynonym(string canonicalTerm, string synonym, VocabularyCategory category = VocabularyCategory.Adjective)
    {
        var concept = Vocabulary.AddSynonym(canonicalTerm, synonym, category);
        NotifyChanged();
        return concept;
    }

    /// <summary>Creates an intentionally incomplete planning point on an existing timeline.</summary>
    public TimelinePlanningPoint CreatePlanningPoint(
        Guid timelineId,
        string? label = null,
        string? approximatePlacement = null,
        string? intent = null)
    {
        if (!Workspace.Timelines.Any(timeline => timeline.Id == timelineId))
            throw new KeyNotFoundException($"Timeline '{timelineId}' does not exist in this workspace.");
        var point = new TimelinePlanningPoint
        {
            TimelineId = timelineId,
            Label = string.IsNullOrWhiteSpace(label) ? "Plot Twist" : label.Trim(),
            ApproximatePlacement = NormalizeOptional(approximatePlacement) ?? string.Empty,
            Intent = NormalizeOptional(intent) ?? string.Empty,
            PlacementKind = string.IsNullOrWhiteSpace(approximatePlacement)
                ? TimelinePlacementKind.Unresolved
                : TimelinePlacementKind.Approximate
        };
        Workspace.AddPlanningPoint(point);
        NotifyChanged();
        return point;
    }

    public TimelinePlanningPoint UpdatePlanningPoint(TimelinePlanningPoint point)
    {
        ArgumentNullException.ThrowIfNull(point);
        var updated = point with
        {
            Label = string.IsNullOrWhiteSpace(point.Label) ? "Untitled planning point" : point.Label.Trim(),
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };
        Workspace.ReplacePlanningPoint(updated);
        NotifyChanged();
        return updated;
    }

    /// <summary>Explicitly promotes a provisional planning point into a proposed canonical event.</summary>
    /// <remarks>The new event remains Proposed; promotion does not silently accept story canon.</remarks>
    public StoryEvent PromotePlanningPointToEvent(Guid planningPointId, string? title = null, string? summary = null)
    {
        var point = Workspace.PlanningPoints.FirstOrDefault(item => item.Id == planningPointId)
            ?? throw new KeyNotFoundException($"Planning point '{planningPointId}' does not exist in this workspace.");
        if (point.PromotedEventId is Guid existingId)
            return Workspace.Events.FirstOrDefault(item => item.Id == existingId)
                ?? throw new InvalidOperationException($"Planning point '{planningPointId}' references missing promoted event '{existingId}'.");
        if (!Workspace.Timelines.Any(item => item.Id == point.TimelineId))
            throw new KeyNotFoundException($"Timeline '{point.TimelineId}' does not exist in this workspace.");

        var storyEvent = new StoryEvent
        {
            TimelineId = point.TimelineId,
            Title = string.IsNullOrWhiteSpace(title) ? point.Label : title.Trim(),
            Summary = string.IsNullOrWhiteSpace(summary) ? point.Intent : summary.Trim(),
            InWorldDateOrPeriod = NormalizeOptional(point.ApproximatePlacement),
            PlacementKind = point.PlacementKind,
            Status = CanonStatus.Proposed
        };
        AddEvent(storyEvent);
        Workspace.ReplacePlanningPoint(point with
        {
            Status = TimelinePlanningStatus.Superseded,
            PromotedEventId = storyEvent.Id,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        });
        NotifyChanged();
        return storyEvent;
    }

    /// <summary>Replaces a planning point's alternative outcomes while preserving their author-entered order.</summary>
    public TimelinePlanningPoint SetPlanningPointAlternatives(Guid planningPointId, IEnumerable<string> alternatives)
    {
        ArgumentNullException.ThrowIfNull(alternatives);
        var point = Workspace.PlanningPoints.FirstOrDefault(item => item.Id == planningPointId)
            ?? throw new KeyNotFoundException($"Planning point '{planningPointId}' does not exist in this workspace.");
        var normalized = alternatives.Where(item => !string.IsNullOrWhiteSpace(item))
            .Select(item => item.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var updated = point with
        {
            Alternatives = normalized,
            Status = normalized.Count > 0 && point.Status == TimelinePlanningStatus.Placeholder
                ? TimelinePlanningStatus.Brainstorming
                : point.Status,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };
        Workspace.ReplacePlanningPoint(updated);
        NotifyChanged();
        return updated;
    }

    /// <summary>Links or unlinks a scene affected by a provisional planning point.</summary>
    public TimelinePlanningPoint LinkPlanningPointToScene(Guid planningPointId, Guid sceneId, bool linked = true)
    {
        if (!Workspace.Scenes.Any(item => item.Id == sceneId))
            throw new KeyNotFoundException($"Scene '{sceneId}' does not exist in this workspace.");
        var point = Workspace.PlanningPoints.FirstOrDefault(item => item.Id == planningPointId)
            ?? throw new KeyNotFoundException($"Planning point '{planningPointId}' does not exist in this workspace.");
        var ids = point.RelatedSceneIds.ToList();
        if (linked && !ids.Contains(sceneId)) ids.Add(sceneId);
        else if (!linked) ids.Remove(sceneId);
        var updated = point with { RelatedSceneIds = ids, UpdatedAtUtc = DateTimeOffset.UtcNow };
        Workspace.ReplacePlanningPoint(updated);
        NotifyChanged();
        return updated;
    }

    /// <summary>Links or unlinks an existing event affected by a provisional planning point.</summary>
    public TimelinePlanningPoint LinkPlanningPointToEvent(Guid planningPointId, Guid eventId, bool linked = true)
    {
        if (!Workspace.Events.Any(item => item.Id == eventId))
            throw new KeyNotFoundException($"Event '{eventId}' does not exist in this workspace.");
        var point = Workspace.PlanningPoints.FirstOrDefault(item => item.Id == planningPointId)
            ?? throw new KeyNotFoundException($"Planning point '{planningPointId}' does not exist in this workspace.");
        var ids = point.RelatedEventIds.ToList();
        if (linked && !ids.Contains(eventId)) ids.Add(eventId);
        else if (!linked) ids.Remove(eventId);
        var updated = point with { RelatedEventIds = ids, UpdatedAtUtc = DateTimeOffset.UtcNow };
        Workspace.ReplacePlanningPoint(updated);
        NotifyChanged();
        return updated;
    }

    /// <summary>Creates an editable manuscript product whose sections can be composed incrementally.</summary>
    public ManuscriptEdition CreateManuscript(string? title = null, string? subtitle = null, string? author = null)
    {
        var manuscript = new ManuscriptEdition
        {
            Title = string.IsNullOrWhiteSpace(title) ? "Untitled book" : title.Trim(),
            Subtitle = NormalizeOptional(subtitle) ?? string.Empty,
            Author = NormalizeOptional(author) ?? string.Empty
        };
        Workspace.AddManuscript(manuscript);
        NotifyChanged();
        return manuscript;
    }

    public ManuscriptEdition UpdateManuscript(ManuscriptEdition manuscript)
    {
        ArgumentNullException.ThrowIfNull(manuscript);
        var updated = manuscript with { UpdatedAtUtc = DateTimeOffset.UtcNow };
        Workspace.ReplaceManuscript(updated);
        NotifyChanged();
        return updated;
    }

    public ManuscriptEdition AddManuscriptSection(Guid manuscriptId, ManuscriptSectionKind kind, string? title = null)
    {
        var manuscript = Workspace.Manuscripts.FirstOrDefault(item => item.Id == manuscriptId)
            ?? throw new KeyNotFoundException($"No manuscript with ID '{manuscriptId}' exists in this workspace.");
        var section = new ManuscriptSection
        {
            Kind = kind,
            Title = string.IsNullOrWhiteSpace(title) ? DefaultSectionTitle(kind, manuscript.Sections.Count + 1) : title.Trim(),
            Sequence = manuscript.Sections.Count == 0 ? 1 : manuscript.Sections.Max(item => item.Sequence) + 1
        };
        return UpdateManuscript(manuscript.AddSection(section));
    }

    public ManuscriptEdition UpdateManuscriptSection(Guid manuscriptId, Guid sectionId, string title, string body)
    {
        var manuscript = Workspace.Manuscripts.FirstOrDefault(item => item.Id == manuscriptId)
            ?? throw new KeyNotFoundException($"No manuscript with ID '{manuscriptId}' exists in this workspace.");
        var section = manuscript.Sections.FirstOrDefault(item => item.Id == sectionId)
            ?? throw new KeyNotFoundException($"No section with ID '{sectionId}' exists in this manuscript.");
        return UpdateManuscript(manuscript.ReplaceSection(section with
        {
            Title = string.IsNullOrWhiteSpace(title) ? "Untitled section" : title.Trim(),
            Body = body ?? string.Empty
        }));
    }

    /// <summary>Freezes the current manuscript as a content-hashed publication snapshot.</summary>
    /// <remarks>Publishing a changed draft creates a new edition linked to the previous publication; existing snapshots are never modified.</remarks>
    public PublishedBookEdition PublishManuscript(Guid manuscriptId, string? correctionNotes = null)
    {
        var manuscript = Workspace.Manuscripts.FirstOrDefault(item => item.Id == manuscriptId)
            ?? throw new KeyNotFoundException($"No manuscript with ID '{manuscriptId}' exists in this workspace.");
        var artifact = new BookProductRenderer().Render(Workspace, manuscriptId);
        var previous = Workspace.PublishedEditions
            .Where(item => item.ManuscriptId == manuscriptId)
            .OrderByDescending(item => item.PublishedAtUtc)
            .FirstOrDefault();
        var edition = new PublishedBookEdition
        {
            ManuscriptId = manuscript.Id,
            Title = artifact.Title,
            Subtitle = artifact.Subtitle,
            Author = artifact.Author,
            Markdown = artifact.Markdown,
            ContentSha256 = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(artifact.Markdown))),
            PublishedAtUtc = DateTimeOffset.UtcNow,
            SourceUpdatedAtUtc = manuscript.UpdatedAtUtc,
            PreviousEditionId = previous?.Id,
            CorrectionNotes = string.IsNullOrWhiteSpace(correctionNotes) ? null : correctionNotes.Trim(),
            SectionIds = manuscript.Sections.OrderBy(section => section.Sequence).Select(section => section.Id).ToList(),
            SceneIds = manuscript.Sections.SelectMany(section => section.SceneIds).Distinct().ToList(),
            EventIds = artifact.ReferencedEventIds.ToList()
        };
        Workspace.AddPublishedEdition(edition);
        NotifyChanged();
        return edition;
    }

    private static string DefaultSectionTitle(ManuscriptSectionKind kind, int sequence) => kind switch
    {
        ManuscriptSectionKind.FrontMatter => "Front Matter",
        ManuscriptSectionKind.Chapter => $"Chapter {sequence}",
        ManuscriptSectionKind.Scene => $"Scene {sequence}",
        ManuscriptSectionKind.Passage => $"Passage {sequence}",
        ManuscriptSectionKind.BackMatter => "Back Matter",
        _ => $"Section {sequence}"
    };

    /// <summary>Creates an editable premise proposal from the creator's initial idea.</summary>
    public StoryPremise CreatePremise(
        string vision,
        string? title = null,
        string? scale = null,
        string? genre = null,
        string? creationApproach = null,
        string? primaryTarget = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(vision);
        var normalizedVision = vision.Trim();
        var premise = new StoryPremise
        {
            Title = string.IsNullOrWhiteSpace(title) ? SuggestPremiseTitle(normalizedVision) : title.Trim(),
            Vision = normalizedVision,
            Scale = NormalizeOptional(scale),
            Genre = NormalizeOptional(genre),
            CreationApproach = NormalizeOptional(creationApproach),
            PrimaryTarget = NormalizeOptional(primaryTarget),
            Status = CanonStatus.Proposed
        };
        Workspace.AddPremise(premise);
        IndexPremise(premise);
        NotifyChanged();
        return premise;
    }

    /// <summary>Revises a premise and replaces its vocabulary occurrences.</summary>
    public StoryPremise UpdatePremise(StoryPremise premise)
    {
        ArgumentNullException.ThrowIfNull(premise);
        Workspace.ReplacePremise(premise with { UpdatedAtUtc = DateTimeOffset.UtcNow });
        Vocabulary.RemoveTarget(premise.Id);
        var updated = Workspace.Premises.Single(item => item.Id == premise.Id);
        IndexPremise(updated);
        NotifyChanged();
        return updated;
    }

    /// <summary>Creates and indexes a new character draft immediately.</summary>
    public CharacterProfile CreateCharacter(string? name = null, string? templateName = null)
    {
        var character = CharacterBuilder.Create(name, templateName);
        Workspace.AddCharacter(character);
        IndexCharacter(character);
        NotifyChanged();
        return character;
    }

    /// <summary>Replaces a character draft and rebuilds its word occurrences after edits.</summary>
    public CharacterProfile UpdateCharacter(CharacterProfile character)
    {
        ArgumentNullException.ThrowIfNull(character);
        Workspace.ReplaceCharacter(character);
        Vocabulary.RemoveTarget(character.Id);
        IndexCharacter(character);
        NotifyChanged();
        return character;
    }

    /// <summary>Creates and stores an empty scene so the user can add beats incrementally.</summary>
    public PlaceRecord CreatePlace(string? name = null, string? description = null, string? kind = null, Guid? parentPlaceId = null)
    {
        var place = new PlaceRecord
        {
            Name = string.IsNullOrWhiteSpace(name) ? "Unnamed place" : name.Trim(),
            Description = NormalizeOptional(description) ?? string.Empty,
            Kind = string.IsNullOrWhiteSpace(kind) ? "Place" : kind.Trim(),
            ParentPlaceId = parentPlaceId,
            Status = CanonStatus.Draft
        };
        Workspace.AddPlace(place);
        IndexPlace(place);
        NotifyChanged();
        return place;
    }

    public PlaceRecord UpdatePlace(PlaceRecord place)
    {
        ArgumentNullException.ThrowIfNull(place);
        var updated = place with
        {
            Name = string.IsNullOrWhiteSpace(place.Name) ? "Unnamed place" : place.Name.Trim(),
            Description = place.Description?.Trim() ?? string.Empty,
            Kind = string.IsNullOrWhiteSpace(place.Kind) ? "Place" : place.Kind.Trim(),
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };
        Workspace.ReplacePlace(updated);
        Vocabulary.RemoveTarget(updated.Id);
        IndexPlace(updated);
        NotifyChanged();
        return updated;
    }

    public SceneRecord AssignSceneToPlace(Guid sceneId, Guid? placeId)
    {
        if (placeId is Guid targetId && !Workspace.Places.Any(place => place.Id == targetId))
            throw new KeyNotFoundException($"Place '{targetId}' does not exist in this workspace.");
        var scene = Workspace.Scenes.FirstOrDefault(item => item.Id == sceneId)
            ?? throw new KeyNotFoundException($"Scene '{sceneId}' does not exist in this workspace.");
        return UpdateScene(scene with { PlaceId = placeId });
    }

    public SceneRecord CreateScene(string? title = null, string? location = null, SceneCaptureMode mode = SceneCaptureMode.Freeform)
    {
        var scene = SceneComposer.Create(title, location, mode);
        Workspace.AddScene(scene);
        IndexScene(scene);
        NotifyChanged();
        return scene;
    }

    /// <summary>Replaces a scene and refreshes its indexed words after beat edits.</summary>
    public SceneRecord UpdateScene(SceneRecord scene)
    {
        ArgumentNullException.ThrowIfNull(scene);
        Workspace.ReplaceScene(scene);
        Vocabulary.RemoveTarget(scene.Id);
        IndexScene(scene);
        NotifyChanged();
        return scene;
    }

    /// <summary>Links a canonical world event to a scene beat without duplicating the event as new world truth.</summary>
    public SceneRecord LinkEventToSceneBeat(Guid sceneId, Guid beatId, Guid eventId, bool linked = true)
    {
        if (!Workspace.Events.Any(item => item.Id == eventId))
            throw new KeyNotFoundException($"World event '{eventId}' does not exist in this workspace.");
        var scene = Workspace.Scenes.FirstOrDefault(item => item.Id == sceneId)
            ?? throw new KeyNotFoundException($"Scene '{sceneId}' does not exist in this workspace.");
        var beat = scene.Beats.FirstOrDefault(item => item.Id == beatId)
            ?? throw new KeyNotFoundException($"Scene beat '{beatId}' does not exist in scene '{sceneId}'.");
        var eventIds = linked
            ? beat.EventIds.Append(eventId).Distinct().ToList()
            : beat.EventIds.Where(id => id != eventId).ToList();
        var updated = scene with
        {
            Beats = scene.Beats.Select(item => item.Id == beatId ? item with { EventIds = eventIds } : item).ToList(),
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };
        return UpdateScene(updated);
    }

    public HistoricalClaim AddHistoricalClaim(HistoricalClaim claim)
    {
        ArgumentNullException.ThrowIfNull(claim);
        Workspace.AddClaim(claim);
        var sourceId = claim.SourceIds.FirstOrDefault();
        Vocabulary.RecordText(claim.Statement, VocabularyCategory.Other, claim.Id, "Historical claim",
            nameof(HistoricalClaim.Statement), context: claim.Statement, attribution: claim.Attribution,
            sourceId: sourceId == Guid.Empty ? null : sourceId.ToString("D"),
            status: claim.Status);
        NotifyChanged();
        return claim;
    }

    public ObserverKnowledgeRecord UpdateObserverKnowledge(ObserverKnowledgeRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);
        Workspace.ReplaceObserverKnowledge(record);
        NotifyChanged();
        return record;
    }

    public ObserverKnowledgeRecord AddObserverKnowledge(ObserverKnowledgeRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);
        Workspace.AddObserverKnowledge(record);
        NotifyChanged();
        return record;
    }

    public StoryEvent CreateTimelineEvent(Guid timelineId, string? title = null, string? summary = null, string? approximatePlacement = null)
    {
        if (!Workspace.Timelines.Any(timeline => timeline.Id == timelineId))
            throw new KeyNotFoundException($"Timeline '{timelineId}' does not exist in this workspace.");
        return AddEvent(new StoryEvent
        {
            TimelineId = timelineId,
            Title = string.IsNullOrWhiteSpace(title) ? "Untitled event" : title.Trim(),
            Summary = NormalizeOptional(summary) ?? string.Empty,
            InWorldDateOrPeriod = NormalizeOptional(approximatePlacement),
            PlacementKind = string.IsNullOrWhiteSpace(approximatePlacement)
                ? TimelinePlacementKind.Unresolved
                : TimelinePlacementKind.Approximate
        });
    }

    /// <summary>Updates an existing canonical event while keeping the vocabulary index synchronized.</summary>
    public StoryEvent UpdateTimelineEvent(StoryEvent storyEvent)
    {
        ArgumentNullException.ThrowIfNull(storyEvent);
        var updated = storyEvent with
        {
            Title = string.IsNullOrWhiteSpace(storyEvent.Title) ? "Untitled event" : storyEvent.Title.Trim(),
            Summary = NormalizeOptional(storyEvent.Summary) ?? string.Empty
        };
        Workspace.ReplaceEvent(updated);
        Vocabulary.RemoveTarget(updated.Id);
        Vocabulary.RecordText(updated.Title, VocabularyCategory.Other, updated.Id, updated.Title,
            nameof(StoryEvent.Title), context: updated.Summary, status: updated.Status);
        Vocabulary.RecordText(updated.Summary, VocabularyCategory.Other, updated.Id, updated.Title,
            nameof(StoryEvent.Summary), context: updated.Summary, status: updated.Status);
        NotifyChanged();
        return updated;
    }

    /// <summary>Creates or removes a reciprocal cause-to-consequence edge between two distinct events.</summary>
    public void LinkEventCausality(Guid causeEventId, Guid consequenceEventId, bool linked = true)
    {
        if (causeEventId == consequenceEventId)
            throw new InvalidOperationException("An event cannot be its own cause or consequence.");
        var cause = Workspace.Events.FirstOrDefault(item => item.Id == causeEventId)
            ?? throw new KeyNotFoundException($"Cause event '{causeEventId}' does not exist in this workspace.");
        var consequence = Workspace.Events.FirstOrDefault(item => item.Id == consequenceEventId)
            ?? throw new KeyNotFoundException($"Consequence event '{consequenceEventId}' does not exist in this workspace.");

        var causeConsequences = cause.ConsequenceEventIds.Where(id => id != consequenceEventId).ToList();
        var consequenceCauses = consequence.CauseEventIds.Where(id => id != causeEventId).ToList();
        if (linked)
        {
            causeConsequences.Add(consequenceEventId);
            consequenceCauses.Add(causeEventId);
        }

        Workspace.ReplaceEvent(cause with { ConsequenceEventIds = causeConsequences });
        Workspace.ReplaceEvent(consequence with { CauseEventIds = consequenceCauses });
        NotifyChanged();
    }

    /// <summary>Links or unlinks a canonical scene from a manuscript section without copying scene data into prose.</summary>
    public ManuscriptEdition LinkSceneToManuscriptSection(Guid manuscriptId, Guid sectionId, Guid sceneId, bool linked = true)
    {
        if (!Workspace.Scenes.Any(item => item.Id == sceneId))
            throw new KeyNotFoundException($"Scene '{sceneId}' does not exist in this workspace.");
        var manuscript = Workspace.Manuscripts.FirstOrDefault(item => item.Id == manuscriptId)
            ?? throw new KeyNotFoundException($"No manuscript with ID '{manuscriptId}' exists in this workspace.");
        var section = manuscript.Sections.FirstOrDefault(item => item.Id == sectionId)
            ?? throw new KeyNotFoundException($"No section with ID '{sectionId}' exists in this manuscript.");
        var sceneIds = section.SceneIds.ToList();
        if (linked && !sceneIds.Contains(sceneId)) sceneIds.Add(sceneId);
        else if (!linked) sceneIds.Remove(sceneId);
        return UpdateManuscript(manuscript.ReplaceSection(section with { SceneIds = sceneIds }));
    }

    public ManuscriptEdition LinkEventToManuscriptSection(Guid manuscriptId, Guid sectionId, Guid eventId, bool linked = true)
    {
        if (!Workspace.Events.Any(item => item.Id == eventId))
            throw new KeyNotFoundException($"Event '{eventId}' does not exist in this workspace.");
        var manuscript = Workspace.Manuscripts.FirstOrDefault(item => item.Id == manuscriptId)
            ?? throw new KeyNotFoundException($"No manuscript with ID '{manuscriptId}' exists in this workspace.");
        var section = manuscript.Sections.FirstOrDefault(item => item.Id == sectionId)
            ?? throw new KeyNotFoundException($"No section with ID '{sectionId}' exists in this manuscript.");
        var eventIds = section.EventIds.ToList();
        if (linked && !eventIds.Contains(eventId))
            eventIds.Add(eventId);
        else if (!linked)
            eventIds.Remove(eventId);
        return UpdateManuscript(manuscript.ReplaceSection(section with { EventIds = eventIds }));
    }

    public StoryEvent AddEvent(StoryEvent storyEvent)
    {
        ArgumentNullException.ThrowIfNull(storyEvent);
        Workspace.AddEvent(storyEvent);
        Vocabulary.RecordText(storyEvent.Title, VocabularyCategory.Other, storyEvent.Id, storyEvent.Title,
            nameof(StoryEvent.Title), context: storyEvent.Summary, status: storyEvent.Status);
        Vocabulary.RecordText(storyEvent.Summary, VocabularyCategory.Other, storyEvent.Id, storyEvent.Title,
            nameof(StoryEvent.Summary), context: storyEvent.Summary, status: storyEvent.Status);
        NotifyChanged();
        return storyEvent;
    }

    public SourceRecord AddSource(SourceRecord source)
    {
        ArgumentNullException.ThrowIfNull(source);
        Workspace.AddSource(source);
        Vocabulary.RecordText(source.Title, VocabularyCategory.Other, source.Id, source.Title,
            nameof(SourceRecord.Title), context: source.Perspective, attribution: "Source");
        Vocabulary.RecordText(source.Perspective, VocabularyCategory.Other, source.Id, source.Title,
            nameof(SourceRecord.Perspective), context: source.ReliabilityNote, attribution: "Source");
        Vocabulary.RecordText(source.ReliabilityNote, VocabularyCategory.Other, source.Id, source.Title,
            nameof(SourceRecord.ReliabilityNote), context: source.ReliabilityNote, attribution: "Author");
        NotifyChanged();
        return source;
    }

    public ContinuityIssue AddContinuityIssue(ContinuityIssue issue)
    {
        ArgumentNullException.ThrowIfNull(issue);
        Workspace.AddContinuityIssue(issue);
        Vocabulary.RecordText(issue.Description, VocabularyCategory.Other, issue.Id, "Continuity issue",
            nameof(ContinuityIssue.Description), context: issue.Description, status: issue.Status);
        Vocabulary.RecordText(issue.ResolutionNotes, VocabularyCategory.Other, issue.Id, "Continuity issue",
            nameof(ContinuityIssue.ResolutionNotes), context: issue.ResolutionNotes, status: issue.Status);
        NotifyChanged();
        return issue;
    }

    public CampaignRecord AddCampaign(CampaignRecord campaign)
    {
        ArgumentNullException.ThrowIfNull(campaign);
        Workspace.AddCampaign(campaign);
        Vocabulary.RecordText(campaign.Name, VocabularyCategory.Other, campaign.Id, campaign.Name,
            nameof(CampaignRecord.Name), context: campaign.Description, status: campaign.Status);
        Vocabulary.RecordText(campaign.Description, VocabularyCategory.Other, campaign.Id, campaign.Name,
            nameof(CampaignRecord.Description), context: campaign.Description, status: campaign.Status);
        NotifyChanged();
        return campaign;
    }

    public ParticipantAction UpdateParticipantAction(ParticipantAction action)
    {
        ArgumentNullException.ThrowIfNull(action);
        Workspace.ReplaceParticipantAction(action);
        Vocabulary.RemoveTarget(action.Id);
        var campaignName = Workspace.Campaigns.FirstOrDefault(c => c.Id == action.CampaignId)?.Name ?? "Campaign action";
        Vocabulary.RecordText(action.Description, VocabularyCategory.Other, action.Id, campaignName,
            nameof(ParticipantAction.Description), context: action.Description, attribution: action.Attribution,
            sourceId: action.Id.ToString("D"), status: action.Status);
        NotifyChanged();
        return action;
    }

    public ParticipantAction AddParticipantAction(ParticipantAction action)
    {
        ArgumentNullException.ThrowIfNull(action);
        Workspace.AddParticipantAction(action);
        var campaignName = Workspace.Campaigns.FirstOrDefault(c => c.Id == action.CampaignId)?.Name ?? "Campaign action";
        Vocabulary.RecordText(action.Description, VocabularyCategory.Other, action.Id, campaignName,
            nameof(ParticipantAction.Description), context: action.Description, attribution: action.Attribution,
            sourceId: action.Id.ToString("D"), status: action.Status);
        NotifyChanged();
        return action;
    }

    private void RebuildVocabularyIndex()
    {
        foreach (var premise in Workspace.Premises) IndexPremise(premise);
        foreach (var character in Workspace.Characters) IndexCharacter(character);
        foreach (var place in Workspace.Places) IndexPlace(place);
        foreach (var scene in Workspace.Scenes) IndexScene(scene);
        foreach (var claim in Workspace.HistoricalClaims)
        {
            var sourceId = claim.SourceIds.FirstOrDefault();
            Vocabulary.RecordText(claim.Statement, VocabularyCategory.Other, claim.Id, "Historical claim",
                nameof(HistoricalClaim.Statement), context: claim.Statement, attribution: claim.Attribution,
                sourceId: sourceId == Guid.Empty ? null : sourceId.ToString("D"), status: claim.Status);
        }
        foreach (var storyEvent in Workspace.Events)
        {
            Vocabulary.RecordText(storyEvent.Title, VocabularyCategory.Other, storyEvent.Id, storyEvent.Title,
                nameof(StoryEvent.Title), context: storyEvent.Summary, status: storyEvent.Status);
            Vocabulary.RecordText(storyEvent.Summary, VocabularyCategory.Other, storyEvent.Id, storyEvent.Title,
                nameof(StoryEvent.Summary), context: storyEvent.Summary, status: storyEvent.Status);
        }
        foreach (var source in Workspace.Sources)
        {
            Vocabulary.RecordText(source.Title, VocabularyCategory.Other, source.Id, source.Title,
                nameof(SourceRecord.Title), context: source.Perspective, attribution: "Source");
            Vocabulary.RecordText(source.Perspective, VocabularyCategory.Other, source.Id, source.Title,
                nameof(SourceRecord.Perspective), context: source.ReliabilityNote, attribution: "Source");
            Vocabulary.RecordText(source.ReliabilityNote, VocabularyCategory.Other, source.Id, source.Title,
                nameof(SourceRecord.ReliabilityNote), context: source.ReliabilityNote, attribution: "Author");
        }
        foreach (var issue in Workspace.ContinuityIssues)
        {
            Vocabulary.RecordText(issue.Description, VocabularyCategory.Other, issue.Id, "Continuity issue",
                nameof(ContinuityIssue.Description), context: issue.Description, status: issue.Status);
            Vocabulary.RecordText(issue.ResolutionNotes, VocabularyCategory.Other, issue.Id, "Continuity issue",
                nameof(ContinuityIssue.ResolutionNotes), context: issue.ResolutionNotes, status: issue.Status);
        }
        foreach (var campaign in Workspace.Campaigns)
        {
            Vocabulary.RecordText(campaign.Name, VocabularyCategory.Other, campaign.Id, campaign.Name,
                nameof(CampaignRecord.Name), context: campaign.Description, status: campaign.Status);
            Vocabulary.RecordText(campaign.Description, VocabularyCategory.Other, campaign.Id, campaign.Name,
                nameof(CampaignRecord.Description), context: campaign.Description, status: campaign.Status);
        }
        foreach (var action in Workspace.ParticipantActions)
        {
            var campaignName = Workspace.Campaigns.FirstOrDefault(c => c.Id == action.CampaignId)?.Name ?? "Campaign action";
            Vocabulary.RecordText(action.Description, VocabularyCategory.Other, action.Id, campaignName,
                nameof(ParticipantAction.Description), context: action.Description, attribution: action.Attribution,
                sourceId: action.Id.ToString("D"), status: action.Status);
        }
    }

    private void NotifyChanged() => Changed?.Invoke();

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string SuggestPremiseTitle(string vision)
    {
        var firstSentence = vision.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault()?.Trim() ?? vision.Trim();
        var words = firstSentence.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        return words.Length <= 8 ? string.Join(' ', words) : string.Join(' ', words.Take(8)) + "…";
    }

    private void IndexPremise(StoryPremise premise)
    {
        Vocabulary.RecordText(premise.Title, VocabularyCategory.Other, premise.Id, premise.Title,
            nameof(StoryPremise.Title), context: premise.Vision, status: premise.Status);
        Vocabulary.RecordText(premise.Vision, VocabularyCategory.Other, premise.Id, premise.Title,
            nameof(StoryPremise.Vision), context: premise.Vision, status: premise.Status);
        Vocabulary.RecordText(premise.Scale, VocabularyCategory.Other, premise.Id, premise.Title,
            nameof(StoryPremise.Scale), context: premise.Vision, status: premise.Status);
        Vocabulary.RecordText(premise.Genre, VocabularyCategory.Other, premise.Id, premise.Title,
            nameof(StoryPremise.Genre), context: premise.Vision, status: premise.Status);
        Vocabulary.RecordText(premise.CreationApproach, VocabularyCategory.Other, premise.Id, premise.Title,
            nameof(StoryPremise.CreationApproach), context: premise.Vision, status: premise.Status);
        Vocabulary.RecordText(premise.PrimaryTarget, VocabularyCategory.Other, premise.Id, premise.Title,
            nameof(StoryPremise.PrimaryTarget), context: premise.Vision, status: premise.Status);
    }

    private void IndexPlace(PlaceRecord place)
    {
        Vocabulary.RecordText(place.Name, VocabularyCategory.Other, place.Id, place.Name,
            nameof(PlaceRecord.Name), context: place.Description, attribution: "Author", status: place.Status);
        Vocabulary.RecordText(place.Description, VocabularyCategory.Other, place.Id, place.Name,
            nameof(PlaceRecord.Description), context: place.Description, attribution: "Author", status: place.Status);
        Vocabulary.RecordText(place.Kind, VocabularyCategory.Other, place.Id, place.Name,
            nameof(PlaceRecord.Kind), context: place.Description, attribution: "Author", status: place.Status);
    }

    private void IndexCharacter(CharacterProfile character)
    {
        Vocabulary.RecordText(character.Name, VocabularyCategory.Other, character.Id, character.Name,
            nameof(CharacterProfile.Name), context: character.Name, attribution: "Author", status: character.Status);
        Vocabulary.RecordText(character.Role, VocabularyCategory.Other, character.Id, character.Name,
            nameof(CharacterProfile.Role), context: character.Role, attribution: "Author", status: character.Status);
        Vocabulary.RecordText(character.NarrativeRole.ToString(), VocabularyCategory.Other, character.Id, character.Name,
            nameof(CharacterProfile.NarrativeRole), context: character.Role, attribution: "Author", status: character.Status);
        Vocabulary.RecordText(character.Description, VocabularyCategory.Other, character.Id, character.Name,
            nameof(CharacterProfile.Description), context: character.Description, attribution: "Author",
            status: character.Status);
        Vocabulary.RecordText(character.Pronouns, VocabularyCategory.Other, character.Id, character.Name,
            nameof(CharacterProfile.Pronouns), context: character.Pronouns, attribution: "Author", status: character.Status);
        Vocabulary.RecordText(character.Species, VocabularyCategory.Other, character.Id, character.Name,
            nameof(CharacterProfile.Species), context: character.Species, attribution: "Author", status: character.Status);

        foreach (var note in character.Notes)
        {
            Vocabulary.RecordText(note.Key, VocabularyCategory.Other, character.Id, character.Name,
                $"Notes.{note.Key}.Key", context: note.Value, attribution: "Author", status: character.Status);
            Vocabulary.RecordText(note.Value, VocabularyCategory.Other, character.Id, character.Name,
                $"Notes.{note.Key}", context: note.Value, attribution: "Author", status: character.Status);
        }

        foreach (var relationship in character.Relationships)
        {
            Vocabulary.RecordText(relationship.Label, VocabularyCategory.Other, character.Id, character.Name,
                nameof(CharacterProfile.Relationships), context: relationship.Notes, attribution: "Author",
                status: relationship.Status);
            Vocabulary.RecordText(relationship.Notes, VocabularyCategory.Other, character.Id, character.Name,
                nameof(CharacterProfile.Relationships), context: relationship.Label, attribution: "Author",
                status: relationship.Status);
        }

        foreach (var trait in character.Traits)
        {
            Vocabulary.RecordText(trait.Term, VocabularyCategory.Adjective, character.Id, character.Name,
                nameof(CharacterProfile.Traits), context: trait.Notes ?? trait.Term, attribution: trait.Attribution,
                status: trait.Status);
            Vocabulary.RecordText(trait.Notes, VocabularyCategory.Other, character.Id, character.Name,
                nameof(CharacterProfile.Traits), context: trait.Term, attribution: trait.Attribution,
                status: trait.Status);
        }
    }

    private void IndexScene(SceneRecord scene)
    {
        Vocabulary.RecordText(scene.Title, VocabularyCategory.Other, scene.Id, scene.Title,
            nameof(SceneRecord.Title), context: scene.Purpose, status: CanonStatus.Draft);
        Vocabulary.RecordText(scene.Location, VocabularyCategory.Other, scene.Id, scene.Title,
            nameof(SceneRecord.Location), context: scene.Location, status: CanonStatus.Draft);
        Vocabulary.RecordText(scene.Purpose, VocabularyCategory.Other, scene.Id, scene.Title,
            nameof(SceneRecord.Purpose), context: scene.Purpose, status: CanonStatus.Draft);

        foreach (var beat in scene.Beats.OrderBy(b => b.Sequence))
        {
            IndexBeatText(scene, beat, beat.ActorLabel, nameof(SceneBeat.ActorLabel), VocabularyCategory.Other);
            IndexBeatText(scene, beat, beat.Action, nameof(SceneBeat.Action), VocabularyCategory.Other);
            IndexBeatText(scene, beat, beat.Reaction, nameof(SceneBeat.Reaction), VocabularyCategory.Other);
            IndexBeatText(scene, beat, beat.Dialogue, nameof(SceneBeat.Dialogue), VocabularyCategory.Other);
            IndexBeatText(scene, beat, beat.PoseOrPosition, nameof(SceneBeat.PoseOrPosition), VocabularyCategory.Other);
            IndexBeatText(scene, beat, beat.EmotionalTone, nameof(SceneBeat.EmotionalTone), VocabularyCategory.Adjective);
            IndexBeatText(scene, beat, beat.CaptureNote, nameof(SceneBeat.CaptureNote), VocabularyCategory.Other);
        }
    }

    private void IndexBeatText(SceneRecord scene, SceneBeat beat, string? text, string field, VocabularyCategory category)
    {
        Vocabulary.RecordText(text, category, scene.Id, scene.Title, field,
            context: text, attribution: beat.Attribution, sourceId: beat.Id.ToString("D"), status: beat.Status);
    }
}
