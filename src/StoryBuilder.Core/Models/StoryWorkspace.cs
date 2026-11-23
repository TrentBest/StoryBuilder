namespace StoryBuilder.Core.Models;

/// <summary>The implementation record for an Experience or Tool being built.</summary>
public sealed record ExperienceDefinition
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = "Untitled Experience";
    public string Purpose { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public List<string> Capabilities { get; init; } = [];
    public List<string> Constraints { get; init; } = [];
    public List<DesignDecision> Decisions { get; init; } = [];
}

/// <summary>A design decision with a rationale and an explicit review status.</summary>
public sealed record DesignDecision
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Decision { get; init; } = string.Empty;
    public string Rationale { get; init; } = string.Empty;
    public CanonStatus Status { get; init; } = CanonStatus.Proposed;
    public string? EvidenceReference { get; init; }
}

/// <summary>A provenance record for a source, memory, testimony, artifact, or observation.</summary>
public sealed record SourceRecord
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; init; } = string.Empty;
    public string SourceType { get; init; } = "Author note";
    public string? LocationOrUri { get; init; }
    public string? Perspective { get; init; }
    public string? ReliabilityNote { get; init; }
}

/// <summary>A claim about a fictional world's past, distinct from the fact that a source made the claim.</summary>
public sealed record HistoricalClaim
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Statement { get; init; } = string.Empty;
    public string? InWorldDateOrPeriod { get; init; }
    public string Attribution { get; init; } = "Author";
    public double? Confidence { get; init; }
    public CanonStatus Status { get; init; } = CanonStatus.Proposed;
    public List<Guid> SourceIds { get; init; } = [];
    public List<Guid> RelatedEntityIds { get; init; } = [];
}

/// <summary>A recorded event or a proposed event that may change world or campaign state.</summary>
public sealed record StoryEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; init; } = "Untitled event";
    public string Summary { get; init; } = string.Empty;
    public string? InWorldDateOrPeriod { get; init; }
    public Guid? TimelineId { get; init; }
    public TimelinePlacementKind PlacementKind { get; init; } = TimelinePlacementKind.Unresolved;
    public List<Guid> ParticipantIds { get; init; } = [];
    public List<Guid> OrganizationParticipantIds { get; init; } = [];
    public List<Guid> CauseEventIds { get; init; } = [];
    public List<Guid> ConsequenceEventIds { get; init; } = [];
    public List<Guid> SourceIds { get; init; } = [];
    public CanonStatus Status { get; init; } = CanonStatus.Proposed;
}

/// <summary>A government, faction, house, institution, corporation, guild, or other organized actor.</summary>
public sealed record OrganizationRecord
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = "Unnamed organization";
    public string Kind { get; init; } = "Organization";
    public string Description { get; init; } = string.Empty;
    public CanonStatus Status { get; init; } = CanonStatus.Draft;
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
}

/// <summary>A continuing narrative context in which participants make consequential choices.</summary>
public sealed record CampaignRecord
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = "Untitled campaign";
    public string Description { get; init; } = string.Empty;
    public List<Guid> ParticipantCharacterIds { get; init; } = [];
    public List<Guid> SceneIds { get; init; } = [];
    public List<Guid> EventIds { get; init; } = [];
    public CanonStatus Status { get; init; } = CanonStatus.Draft;
}

/// <summary>An authored adventure within a campaign; it groups quests without prescribing outcomes.</summary>
public sealed record AdventureRecord
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid CampaignId { get; init; }
    public string Title { get; init; } = "Untitled adventure";
    public string Summary { get; init; } = string.Empty;
    public int Sequence { get; init; } = 1;
    public CanonStatus Status { get; init; } = CanonStatus.Draft;
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
}

/// <summary>A quest is playable intent; its objectives may be completed in any order or not at all.</summary>
public sealed record QuestRecord
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid CampaignId { get; init; }
    public Guid? AdventureId { get; init; }
    public string Title { get; init; } = "Untitled quest";
    public string Summary { get; init; } = string.Empty;
    public int Sequence { get; init; } = 1;
    public CanonStatus Status { get; init; } = CanonStatus.Draft;
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
}

/// <summary>A player-facing objective or line of inquiry within a quest.</summary>
public sealed record QuestObjectiveRecord
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid CampaignId { get; init; }
    public Guid QuestId { get; init; }
    public string Title { get; init; } = "Untitled objective";
    public string Description { get; init; } = string.Empty;
    public int Sequence { get; init; } = 1;
    public CanonStatus Status { get; init; } = CanonStatus.Draft;
    public Guid? RelatedSceneId { get; init; }
    public Guid? TargetEventId { get; init; }
}

/// <summary>A participant-authored choice; it does not become canon until deliberately accepted.</summary>
public sealed record ParticipantAction
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid CampaignId { get; init; }
    public Guid? CharacterId { get; init; }
    public string Description { get; init; } = string.Empty;
    public string Attribution { get; init; } = "Participant";
    public Guid? ResultingEventId { get; init; }
    public CanonStatus Status { get; init; } = CanonStatus.Proposed;
}

/// <summary>An unresolved contradiction, gap, or dependency that needs a continuity decision.</summary>
public sealed record ContinuityIssue
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Description { get; init; } = string.Empty;
    public List<Guid> RelatedRecordIds { get; init; } = [];
    public string? ResolutionNotes { get; init; }
    public CanonStatus Status { get; init; } = CanonStatus.Proposed;
}

/// <summary>A creator-owned story idea, separate from the implementation record for the StoryBuilder tool itself.</summary>
public sealed record StoryPremise
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; init; } = "Untitled story idea";
    public string Vision { get; init; } = string.Empty;
    public string? Scale { get; init; }
    public string? Genre { get; init; }
    public string? CreationApproach { get; init; }
    public string? PrimaryTarget { get; init; }
    public CanonStatus Status { get; init; } = CanonStatus.Proposed;
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
}

/// <summary>
    /// An in-memory workspace boundary that groups the build record and fictional story records.
    /// Persistence and presentation are host responsibilities; the two record families remain distinct.
    /// </summary>
public sealed class StoryWorkspace
{
    private readonly List<TimelineDefinition> _timelines = [];
    private readonly List<TimelinePlanningPoint> _planningPoints = [];
    private readonly List<ObserverKnowledgeRecord> _observerKnowledge = [];
    private readonly List<ManuscriptEdition> _manuscripts = [];
    private readonly List<PublishedBookEdition> _publishedEditions = [];
    private readonly List<PublicationOutlet> _publicationOutlets = [];
    private readonly List<InWorldPublicationEdition> _inWorldPublicationEditions = [];
    private readonly List<StoryPremise> _premises = [];
    private readonly List<CharacterProfile> _characters = [];
    private readonly List<OrganizationRecord> _organizations = [];
    private readonly List<PlaceRecord> _places = [];
    private readonly List<SceneRecord> _scenes = [];
    private readonly List<HistoricalClaim> _historicalClaims = [];
    private readonly List<StoryEvent> _events = [];
    private readonly List<CampaignRecord> _campaigns = [];
    private readonly List<AdventureRecord> _adventures = [];
    private readonly List<QuestRecord> _quests = [];
    private readonly List<QuestObjectiveRecord> _questObjectives = [];
    private readonly List<CampaignSessionRecord> _campaignSessions = [];
    private readonly List<SourcebookAttachment> _sourcebookAttachments = [];
    private readonly List<ParticipantAction> _participantActions = [];
    private readonly List<SourceRecord> _sources = [];
    private readonly List<ContinuityIssue> _continuityIssues = [];
    private readonly List<VisualAssetRecord> _visualAssets = [];
    private readonly List<VisualAssetReview> _visualAssetReviews = [];
    private readonly List<PageComposition> _pageCompositions = [];

    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = "StoryBuilder";
    /// <summary>Exact original legacy project JSON retained during migration so unsupported legacy fields remain recoverable.</summary>
    public string? LegacyProjectSourceJson { get; set; }
    public ExperienceDefinition Experience { get; set; } = new();
    public IReadOnlyList<TimelineDefinition> Timelines => _timelines.AsReadOnly();
    public IReadOnlyList<TimelinePlanningPoint> PlanningPoints => _planningPoints.AsReadOnly();
    public IReadOnlyList<ObserverKnowledgeRecord> ObserverKnowledge => _observerKnowledge.AsReadOnly();
    public IReadOnlyList<ManuscriptEdition> Manuscripts => _manuscripts.AsReadOnly();
    public IReadOnlyList<PublishedBookEdition> PublishedEditions => _publishedEditions.AsReadOnly();
    public IReadOnlyList<PublicationOutlet> PublicationOutlets => _publicationOutlets.AsReadOnly();
    public IReadOnlyList<InWorldPublicationEdition> InWorldPublicationEditions => _inWorldPublicationEditions.AsReadOnly();
    public IReadOnlyList<StoryPremise> Premises => _premises.AsReadOnly();
    public IReadOnlyList<CharacterProfile> Characters => _characters.AsReadOnly();
    public IReadOnlyList<OrganizationRecord> Organizations => _organizations.AsReadOnly();
    public IReadOnlyList<PlaceRecord> Places => _places.AsReadOnly();
    public IReadOnlyList<SceneRecord> Scenes => _scenes.AsReadOnly();
    public IReadOnlyList<HistoricalClaim> HistoricalClaims => _historicalClaims.AsReadOnly();
    public IReadOnlyList<StoryEvent> Events => _events.AsReadOnly();
    public IReadOnlyList<CampaignRecord> Campaigns => _campaigns.AsReadOnly();
    public IReadOnlyList<AdventureRecord> Adventures => _adventures.AsReadOnly();
    public IReadOnlyList<QuestRecord> Quests => _quests.AsReadOnly();
    public IReadOnlyList<QuestObjectiveRecord> QuestObjectives => _questObjectives.AsReadOnly();
    public IReadOnlyList<CampaignSessionRecord> CampaignSessions => _campaignSessions.AsReadOnly();
    public IReadOnlyList<SourcebookAttachment> SourcebookAttachments => _sourcebookAttachments.AsReadOnly();
    public IReadOnlyList<ParticipantAction> ParticipantActions => _participantActions.AsReadOnly();
    public IReadOnlyList<SourceRecord> Sources => _sources.AsReadOnly();
    public IReadOnlyList<ContinuityIssue> ContinuityIssues => _continuityIssues.AsReadOnly();
    public IReadOnlyList<VisualAssetRecord> VisualAssets => _visualAssets.AsReadOnly();
    public IReadOnlyList<VisualAssetReview> VisualAssetReviews => _visualAssetReviews.AsReadOnly();
    public IReadOnlyList<PageComposition> PageCompositions => _pageCompositions.AsReadOnly();

    public void AddTimeline(TimelineDefinition timeline)
    {
        ArgumentNullException.ThrowIfNull(timeline);
        EnsureUnique(_timelines.Select(x => x.Id), timeline.Id, nameof(timeline));
        if (timeline.ParentTimelineId is Guid parentId && !_timelines.Any(x => x.Id == parentId))
            throw new KeyNotFoundException($"Parent timeline '{parentId}' must exist in this workspace before its branch is added.");
        _timelines.Add(timeline);
    }

    public void AddPlanningPoint(TimelinePlanningPoint point)
    {
        ArgumentNullException.ThrowIfNull(point);
        EnsureUnique(_planningPoints.Select(x => x.Id), point.Id, nameof(point));
        if (!_timelines.Any(timeline => timeline.Id == point.TimelineId))
            throw new KeyNotFoundException($"Timeline '{point.TimelineId}' must exist before a planning point can be added.");
        _planningPoints.Add(point);
    }

    public void ReplacePlanningPoint(TimelinePlanningPoint point)
    {
        ArgumentNullException.ThrowIfNull(point);
        if (!_timelines.Any(timeline => timeline.Id == point.TimelineId))
            throw new KeyNotFoundException($"Timeline '{point.TimelineId}' must exist before a planning point can be updated.");
        ReplaceById(_planningPoints, point.Id, point, nameof(point));
    }

    public void AddObserverKnowledge(ObserverKnowledgeRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);
        EnsureUnique(_observerKnowledge.Select(x => x.Id), record.Id, nameof(record));
        if (!_events.Any(storyEvent => storyEvent.Id == record.EventId))
            throw new KeyNotFoundException($"Event '{record.EventId}' must exist before observer knowledge can reference it.");
        _observerKnowledge.Add(record);
    }

    public void ReplaceObserverKnowledge(ObserverKnowledgeRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);
        if (!_events.Any(storyEvent => storyEvent.Id == record.EventId))
            throw new KeyNotFoundException($"Event '{record.EventId}' must exist before observer knowledge can reference it.");
        ReplaceById(_observerKnowledge, record.Id, record, nameof(record));
    }

    public void AddManuscript(ManuscriptEdition manuscript)
    {
        ArgumentNullException.ThrowIfNull(manuscript);
        EnsureUnique(_manuscripts.Select(x => x.Id), manuscript.Id, nameof(manuscript));
        _manuscripts.Add(manuscript);
    }

    public void ReplaceManuscript(ManuscriptEdition manuscript)
    {
        ArgumentNullException.ThrowIfNull(manuscript);
        ReplaceById(_manuscripts, manuscript.Id, manuscript, nameof(manuscript));
    }

    public void AddPublishedEdition(PublishedBookEdition edition)
    {
        ArgumentNullException.ThrowIfNull(edition);
        EnsureUnique(_publishedEditions.Select(x => x.Id), edition.Id, nameof(edition));
        if (!_manuscripts.Any(item => item.Id == edition.ManuscriptId))
            throw new KeyNotFoundException($"Manuscript '{edition.ManuscriptId}' must exist before a publication edition can be added.");
        if (edition.PreviousEditionId is Guid previousId)
        {
            var previous = _publishedEditions.FirstOrDefault(item => item.Id == previousId)
                ?? throw new KeyNotFoundException($"Previous publication edition '{previousId}' must exist before a corrected edition can be added.");
            if (previous.ManuscriptId != edition.ManuscriptId)
                throw new InvalidOperationException("A corrected publication must refer to a previous edition of the same manuscript.");
        }
        _publishedEditions.Add(edition);
    }

    public void AddPublicationOutlet(PublicationOutlet outlet)
    {
        ArgumentNullException.ThrowIfNull(outlet);
        EnsureUnique(_publicationOutlets.Select(item => item.Id), outlet.Id, nameof(outlet));
        _publicationOutlets.Add(outlet);
    }

    /// <summary>Appends an issued artifact; existing editions are never edited or replaced.</summary>
    public void AddInWorldPublicationEdition(InWorldPublicationEdition edition)
    {
        ArgumentNullException.ThrowIfNull(edition);
        EnsureUnique(_inWorldPublicationEditions.Select(item => item.Id), edition.Id, nameof(edition));
        if (!_publicationOutlets.Any(item => item.Id == edition.OutletId))
            throw new KeyNotFoundException($"Publication outlet '{edition.OutletId}' must exist before an edition can be added.");
        if (edition.EditionNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(edition), "Publication edition numbers must be positive.");
        if (_inWorldPublicationEditions.Any(item => item.OutletId == edition.OutletId && item.EditionNumber == edition.EditionNumber))
            throw new InvalidOperationException($"Outlet '{edition.OutletId}' already has edition number {edition.EditionNumber}.");
        foreach (var id in edition.ClaimIds)
            if (!_historicalClaims.Any(item => item.Id == id))
                throw new KeyNotFoundException($"Historical claim '{id}' must exist before an edition can reference it.");
        foreach (var id in edition.SourceIds)
            if (!_sources.Any(item => item.Id == id))
                throw new KeyNotFoundException($"Source '{id}' must exist before an edition can reference it.");
        foreach (var id in edition.EventIds)
            if (!_events.Any(item => item.Id == id))
                throw new KeyNotFoundException($"Story event '{id}' must exist before an edition can reference it.");
        if (edition.SupersedesEditionId is Guid previousId)
        {
            var previous = _inWorldPublicationEditions.FirstOrDefault(item => item.Id == previousId)
                ?? throw new KeyNotFoundException($"Superseded edition '{previousId}' must exist before its correction can be added.");
            if (previous.OutletId != edition.OutletId)
                throw new InvalidOperationException("A correction must belong to the same publication outlet as the edition it supersedes.");
            if (previous.EditionNumber >= edition.EditionNumber)
                throw new InvalidOperationException("A correction must have a greater edition number than the edition it supersedes.");
        }
        edition = edition with
        {
            ClaimIds = Array.AsReadOnly(edition.ClaimIds.ToArray()),
            SourceIds = Array.AsReadOnly(edition.SourceIds.ToArray()),
            EventIds = Array.AsReadOnly(edition.EventIds.ToArray())
        };
        _inWorldPublicationEditions.Add(edition);
    }

    public void AddPremise(StoryPremise premise)
    {
        ArgumentNullException.ThrowIfNull(premise);
        EnsureUnique(_premises.Select(x => x.Id), premise.Id, nameof(premise));
        _premises.Add(premise);
    }

    public void ReplacePremise(StoryPremise premise)
    {
        ArgumentNullException.ThrowIfNull(premise);
        ReplaceById(_premises, premise.Id, premise, nameof(premise));
    }

    public void AddCharacter(CharacterProfile character)
    {
        ArgumentNullException.ThrowIfNull(character);
        EnsureUnique(_characters.Select(x => x.Id), character.Id, nameof(character));
        _characters.Add(character);
    }

    public void AddOrganization(OrganizationRecord organization)
    {
        ArgumentNullException.ThrowIfNull(organization);
        EnsureUnique(_organizations.Select(x => x.Id), organization.Id, nameof(organization));
        _organizations.Add(organization);
    }

    public void ReplaceOrganization(OrganizationRecord organization)
    {
        ArgumentNullException.ThrowIfNull(organization);
        ReplaceById(_organizations, organization.Id, organization, nameof(organization));
    }

    public void AddPlace(PlaceRecord place)
    {
        ArgumentNullException.ThrowIfNull(place);
        EnsureUnique(_places.Select(x => x.Id), place.Id, nameof(place));
        if (place.ParentPlaceId is Guid parentId && !_places.Any(existing => existing.Id == parentId))
            throw new KeyNotFoundException($"Parent place '{parentId}' must exist before its child place is added.");
        _places.Add(place);
    }

    public void ReplacePlace(PlaceRecord place)
    {
        ArgumentNullException.ThrowIfNull(place);
        if (place.ParentPlaceId is Guid parentId && !_places.Any(existing => existing.Id == parentId))
            throw new KeyNotFoundException($"Parent place '{parentId}' does not exist in this workspace.");
        if (place.ParentPlaceId == place.Id)
            throw new InvalidOperationException("A place cannot contain itself.");
        ReplaceById(_places, place.Id, place, nameof(place));
    }

    public void AddScene(SceneRecord scene)
    {
        ArgumentNullException.ThrowIfNull(scene);
        EnsureUnique(_scenes.Select(x => x.Id), scene.Id, nameof(scene));
        _scenes.Add(scene);
    }

    public void ReplaceCharacter(CharacterProfile character)
    {
        ArgumentNullException.ThrowIfNull(character);
        ReplaceById(_characters, character.Id, character, nameof(character));
    }

    public void ReplaceScene(SceneRecord scene)
    {
        ArgumentNullException.ThrowIfNull(scene);
        ReplaceById(_scenes, scene.Id, scene, nameof(scene));
    }

    public void AddClaim(HistoricalClaim claim)
    {
        ArgumentNullException.ThrowIfNull(claim);
        EnsureUnique(_historicalClaims.Select(x => x.Id), claim.Id, nameof(claim));
        _historicalClaims.Add(claim);
    }

    public void AddEvent(StoryEvent storyEvent)
    {
        ArgumentNullException.ThrowIfNull(storyEvent);
        EnsureUnique(_events.Select(x => x.Id), storyEvent.Id, nameof(storyEvent));
        if (storyEvent.TimelineId is Guid timelineId && !_timelines.Any(timeline => timeline.Id == timelineId))
            throw new KeyNotFoundException($"Timeline '{timelineId}' must exist before an event can be assigned to it.");
        _events.Add(storyEvent);
    }

    public void ReplaceEvent(StoryEvent storyEvent)
    {
        ArgumentNullException.ThrowIfNull(storyEvent);
        if (storyEvent.TimelineId is Guid timelineId && !_timelines.Any(timeline => timeline.Id == timelineId))
            throw new KeyNotFoundException($"Timeline '{timelineId}' must exist before an event can be assigned to it.");
        ReplaceById(_events, storyEvent.Id, storyEvent, nameof(storyEvent));
    }

    public void AddCampaign(CampaignRecord campaign)
    {
        ArgumentNullException.ThrowIfNull(campaign);
        EnsureUnique(_campaigns.Select(x => x.Id), campaign.Id, nameof(campaign));

        foreach (var id in campaign.ParticipantCharacterIds)
            if (!_characters.Any(item => item.Id == id))
                throw new KeyNotFoundException($"Participant character '{id}' must exist before a campaign can reference it.");
        foreach (var id in campaign.SceneIds)
            if (!_scenes.Any(item => item.Id == id))
                throw new KeyNotFoundException($"Scene '{id}' must exist before a campaign can reference it.");
        foreach (var id in campaign.EventIds)
            if (!_events.Any(item => item.Id == id))
                throw new KeyNotFoundException($"Event '{id}' must exist before a campaign can reference it.");

        _campaigns.Add(campaign with
        {
            ParticipantCharacterIds = campaign.ParticipantCharacterIds.Distinct().ToList(),
            SceneIds = campaign.SceneIds.Distinct().ToList(),
            EventIds = campaign.EventIds.Distinct().ToList()
        });
    }

    public void AddAdventure(AdventureRecord item)
    {
        ArgumentNullException.ThrowIfNull(item);
        EnsureUnique(_adventures.Select(x => x.Id), item.Id, nameof(item));
        if (!_campaigns.Any(x => x.Id == item.CampaignId)) throw new KeyNotFoundException($"Campaign '{item.CampaignId}' must exist before an adventure can be added.");
        if (item.Sequence < 1) throw new ArgumentOutOfRangeException(nameof(item));
        _adventures.Add(item);
    }

    public void ReplaceAdventure(AdventureRecord item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (!_campaigns.Any(x => x.Id == item.CampaignId)) throw new KeyNotFoundException($"Campaign '{item.CampaignId}' does not exist.");
        if (item.Sequence < 1) throw new ArgumentOutOfRangeException(nameof(item));
        if (_quests.Any(x => x.AdventureId == item.Id && x.CampaignId != item.CampaignId)) throw new InvalidOperationException("Adventure campaign ownership cannot change while quests reference it.");
        ReplaceById(_adventures, item.Id, item, nameof(item));
    }

    public void AddQuest(QuestRecord item)
    {
        ArgumentNullException.ThrowIfNull(item);
        EnsureUnique(_quests.Select(x => x.Id), item.Id, nameof(item));
        if (!_campaigns.Any(x => x.Id == item.CampaignId)) throw new KeyNotFoundException($"Campaign '{item.CampaignId}' must exist before a quest can be added.");
        if (item.AdventureId is Guid adventureId && !_adventures.Any(x => x.Id == adventureId && x.CampaignId == item.CampaignId))
            throw new KeyNotFoundException($"Adventure '{adventureId}' must belong to campaign '{item.CampaignId}'.");
        if (item.Sequence < 1) throw new ArgumentOutOfRangeException(nameof(item));
        _quests.Add(item);
    }

    public void ReplaceQuest(QuestRecord item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (!_campaigns.Any(x => x.Id == item.CampaignId)) throw new KeyNotFoundException($"Campaign '{item.CampaignId}' does not exist.");
        if (item.AdventureId is Guid adventureId && !_adventures.Any(x => x.Id == adventureId && x.CampaignId == item.CampaignId))
            throw new KeyNotFoundException($"Adventure '{adventureId}' must belong to campaign '{item.CampaignId}'.");
        if (item.Sequence < 1) throw new ArgumentOutOfRangeException(nameof(item));
        if (_questObjectives.Any(x => x.QuestId == item.Id && x.CampaignId != item.CampaignId)) throw new InvalidOperationException("Quest campaign ownership cannot change while objectives reference it.");
        ReplaceById(_quests, item.Id, item, nameof(item));
    }

    public void AddCampaignSession(CampaignSessionRecord item)
    {
        ArgumentNullException.ThrowIfNull(item);
        EnsureUnique(_campaignSessions.Select(x => x.Id), item.Id, nameof(item));
        if (!_campaigns.Any(x => x.Id == item.CampaignId))
            throw new KeyNotFoundException($"Campaign '{item.CampaignId}' must exist before a session can be added.");
        ValidateCampaignSession(item);
        _campaignSessions.Add(item);
    }

    public void ReplaceCampaignSession(CampaignSessionRecord item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (!_campaigns.Any(x => x.Id == item.CampaignId))
            throw new KeyNotFoundException($"Campaign '{item.CampaignId}' must exist before a session can be updated.");
        ValidateCampaignSession(item);
        ReplaceById(_campaignSessions, item.Id, item, nameof(item));
    }

    private static void ValidateCampaignSession(CampaignSessionRecord item)
    {
        if (item.Sequence < 1)
            throw new ArgumentOutOfRangeException(nameof(item), "Session sequence must be positive.");
        if ((item.Status is CampaignSessionStatus.Planned or CampaignSessionStatus.Prepared) &&
            (item.StartedAtUtc is not null || item.EndedAtUtc is not null))
            throw new InvalidOperationException("A planned or prepared session cannot have execution timestamps.");
        if (item.Status is CampaignSessionStatus.InProgress or CampaignSessionStatus.Completed &&
            item.StartedAtUtc is null)
            throw new InvalidOperationException("An in-progress or completed session must have a start timestamp.");
        if (item.Status == CampaignSessionStatus.InProgress && item.EndedAtUtc is not null)
            throw new InvalidOperationException("An in-progress session cannot have an end timestamp.");
        if (item.Status == CampaignSessionStatus.Completed && item.EndedAtUtc is null)
            throw new InvalidOperationException("A completed session must have an end timestamp.");
        if (item.EndedAtUtc is not null &&
            (item.StartedAtUtc is null || item.EndedAtUtc < item.StartedAtUtc))
            throw new InvalidOperationException("A session end timestamp must not precede its start timestamp.");
    }

    public void AddQuestObjective(QuestObjectiveRecord item)
    {
        ArgumentNullException.ThrowIfNull(item);
        EnsureUnique(_questObjectives.Select(x => x.Id), item.Id, nameof(item));
        if (!_quests.Any(x => x.Id == item.QuestId && x.CampaignId == item.CampaignId)) throw new KeyNotFoundException($"Quest '{item.QuestId}' must belong to campaign '{item.CampaignId}'.");
        if (item.Sequence < 1) throw new ArgumentOutOfRangeException(nameof(item));
        if (item.RelatedSceneId is Guid sceneId && !_scenes.Any(x => x.Id == sceneId)) throw new KeyNotFoundException($"Scene '{sceneId}' does not exist.");
        if (item.TargetEventId is Guid eventId && !_events.Any(x => x.Id == eventId)) throw new KeyNotFoundException($"Event '{eventId}' does not exist.");
        _questObjectives.Add(item);
    }

    public void ReplaceQuestObjective(QuestObjectiveRecord item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (!_quests.Any(x => x.Id == item.QuestId && x.CampaignId == item.CampaignId)) throw new KeyNotFoundException($"Quest '{item.QuestId}' must belong to campaign '{item.CampaignId}'.");
        if (item.Sequence < 1) throw new ArgumentOutOfRangeException(nameof(item));
        if (item.RelatedSceneId is Guid sceneId && !_scenes.Any(x => x.Id == sceneId)) throw new KeyNotFoundException($"Scene '{sceneId}' does not exist.");
        if (item.TargetEventId is Guid eventId && !_events.Any(x => x.Id == eventId)) throw new KeyNotFoundException($"Event '{eventId}' does not exist.");
        ReplaceById(_questObjectives, item.Id, item, nameof(item));
    }

    /// <summary>Attaches an exact sourcebook edition to a campaign without copying or mutating its content.</summary>
    public void AddSourcebookAttachment(SourcebookAttachment attachment)
    {
        ArgumentNullException.ThrowIfNull(attachment);
        EnsureUnique(_sourcebookAttachments.Select(item => item.Id), attachment.Id, nameof(attachment));
        if (!_campaigns.Any(item => item.Id == attachment.CampaignId))
            throw new KeyNotFoundException($"Campaign '{attachment.CampaignId}' must exist before a sourcebook can be attached.");
        if (string.IsNullOrWhiteSpace(attachment.SourcebookId) ||
            string.IsNullOrWhiteSpace(attachment.Title) ||
            string.IsNullOrWhiteSpace(attachment.Edition) ||
            string.IsNullOrWhiteSpace(attachment.ContentSha256) ||
            string.IsNullOrWhiteSpace(attachment.SourceUri))
            throw new ArgumentException("A sourcebook attachment requires stable identity, title, edition, content hash, and source URI.", nameof(attachment));
        if (_sourcebookAttachments.Any(item =>
            item.CampaignId == attachment.CampaignId &&
            item.SourcebookId == attachment.SourcebookId &&
            item.Edition == attachment.Edition))
            throw new InvalidOperationException("That exact sourcebook edition is already attached to this campaign.");
        if (attachment.EntryIds.Any(string.IsNullOrWhiteSpace) ||
            attachment.EntryIds.Distinct(StringComparer.Ordinal).Count() != attachment.EntryIds.Count)
            throw new ArgumentException("Sourcebook entry IDs must be non-empty and unique.", nameof(attachment));
        _sourcebookAttachments.Add(attachment with
        {
            EntryIds = Array.AsReadOnly(attachment.EntryIds.ToArray())
        });
    }

    public void ReplaceParticipantAction(ParticipantAction action)
    {
        ArgumentNullException.ThrowIfNull(action);
        ReplaceById(_participantActions, action.Id, action, nameof(action));
    }

    public void AddParticipantAction(ParticipantAction action)
    {
        ArgumentNullException.ThrowIfNull(action);
        EnsureUnique(_participantActions.Select(x => x.Id), action.Id, nameof(action));
        if (!_campaigns.Any(item => item.Id == action.CampaignId))
            throw new KeyNotFoundException($"Campaign '{action.CampaignId}' must exist before a participant action can be recorded.");
        if (action.CharacterId is Guid characterId && !_characters.Any(item => item.Id == characterId))
            throw new KeyNotFoundException($"Character '{characterId}' must exist before a participant action can reference it.");
        if (action.ResultingEventId is Guid eventId && !_events.Any(item => item.Id == eventId))
            throw new KeyNotFoundException($"Event '{eventId}' must exist before a participant action can reference it.");
        _participantActions.Add(action);
    }

    public void AddSource(SourceRecord source)
    {
        ArgumentNullException.ThrowIfNull(source);
        EnsureUnique(_sources.Select(x => x.Id), source.Id, nameof(source));
        _sources.Add(source);
    }

    public void AddContinuityIssue(ContinuityIssue issue)
    {
        ArgumentNullException.ThrowIfNull(issue);
        EnsureUnique(_continuityIssues.Select(x => x.Id), issue.Id, nameof(issue));
        _continuityIssues.Add(issue);
    }

    public void AddVisualAsset(VisualAssetRecord asset)
    {
        ArgumentNullException.ThrowIfNull(asset);
        EnsureUnique(_visualAssets.Select(x => x.Id), asset.Id, nameof(asset));
        if (asset.Revisions.Select(item => item.RevisionNumber).Distinct().Count() != asset.Revisions.Count || asset.Revisions.Any(item => item.RevisionNumber < 1))
            throw new InvalidOperationException("Visual asset revision numbers must be unique positive integers.");
        if (asset.Revisions.Select(item => item.Id).Distinct().Count() != asset.Revisions.Count)
            throw new InvalidOperationException("Visual asset revision identities must be unique.");
        _visualAssets.Add(asset);
    }

    public void ReplacePageComposition(PageComposition composition)
    {
        ArgumentNullException.ThrowIfNull(composition);
        if (!_manuscripts.SelectMany(item => item.Sections).Any(item => item.Id == composition.ManuscriptSectionId))
            throw new KeyNotFoundException($"Manuscript section '{composition.ManuscriptSectionId}' does not exist.");
        if (composition.VisualAssetId is Guid assetId && !_visualAssets.Any(item => item.Id == assetId))
            throw new KeyNotFoundException($"Visual asset '{assetId}' does not exist.");
        if (composition.VisualAssetRevisionId is Guid revisionId &&
            (composition.VisualAssetId is not Guid selectedAssetId ||
             !_visualAssets.Any(item => item.Id == selectedAssetId && item.Revisions.Any(revision => revision.Id == revisionId))))
            throw new KeyNotFoundException($"Visual revision '{revisionId}' does not belong to the selected visual asset.");
        if (!IsNormalized(composition.TextRegion) || !IsNormalized(composition.ImageRegion) ||
            (composition.DiscoveryRegion is not null && !IsNormalized(composition.DiscoveryRegion)))
            throw new InvalidOperationException("Page composition regions must fit within normalized page bounds (0..1).");
        if (composition.DiscoveryHintDelaySeconds is < 0 or > 120)
            throw new InvalidOperationException("Discovery hint delay must be between 0 and 120 seconds.");
        ReplaceById(_pageCompositions, composition.Id, composition, nameof(composition));
    }

    public void AddPageComposition(PageComposition composition)
    {
        ArgumentNullException.ThrowIfNull(composition);
        EnsureUnique(_pageCompositions.Select(item => item.Id), composition.Id, nameof(composition));
        if (!_manuscripts.SelectMany(item => item.Sections).Any(item => item.Id == composition.ManuscriptSectionId))
            throw new KeyNotFoundException($"Manuscript section '{composition.ManuscriptSectionId}' does not exist.");
        if (composition.VisualAssetId is Guid assetId && !_visualAssets.Any(item => item.Id == assetId))
            throw new KeyNotFoundException($"Visual asset '{assetId}' does not exist.");
        if (composition.VisualAssetRevisionId is Guid revisionId &&
            (composition.VisualAssetId is not Guid selectedAssetId ||
             !_visualAssets.Any(item => item.Id == selectedAssetId && item.Revisions.Any(revision => revision.Id == revisionId))))
            throw new KeyNotFoundException($"Visual revision '{revisionId}' does not belong to the selected visual asset.");
        if (!IsNormalized(composition.TextRegion) || !IsNormalized(composition.ImageRegion) ||
            (composition.DiscoveryRegion is not null && !IsNormalized(composition.DiscoveryRegion)))
            throw new InvalidOperationException("Page composition regions must fit within normalized page bounds (0..1).");
        if (composition.DiscoveryHintDelaySeconds is < 0 or > 120)
            throw new InvalidOperationException("Discovery hint delay must be between 0 and 120 seconds.");
        _pageCompositions.Add(composition);
    }

    private static bool IsNormalized(PageCompositionRegion region) =>
        double.IsFinite(region.X) && double.IsFinite(region.Y) &&
        double.IsFinite(region.Width) && double.IsFinite(region.Height) &&
        region.X >= 0 && region.Y >= 0 && region.Width > 0 && region.Height > 0 &&
        region.X + region.Width <= 1 && region.Y + region.Height <= 1;

    public void AddVisualAssetReview(VisualAssetReview review)
    {
        ArgumentNullException.ThrowIfNull(review);
        EnsureUnique(_visualAssetReviews.Select(x => x.Id), review.Id, nameof(review));
        var asset = _visualAssets.FirstOrDefault(item => item.Id == review.AssetId)
            ?? throw new KeyNotFoundException($"Visual asset '{review.AssetId}' does not exist.");
        if (!asset.Revisions.Any(item => item.Id == review.RevisionId))
            throw new KeyNotFoundException($"Visual asset revision '{review.RevisionId}' does not exist on asset '{review.AssetId}'.");
        _visualAssetReviews.Add(review);
    }

    /// <summary>Appends a new revision; existing revisions and their review history remain unchanged.</summary>
    public void AddVisualAssetRevision(Guid assetId, VisualAssetRevision revision)
    {
        ArgumentNullException.ThrowIfNull(revision);
        var index = _visualAssets.FindIndex(item => item.Id == assetId);
        if (index < 0) throw new KeyNotFoundException($"Visual asset '{assetId}' does not exist.");
        var asset = _visualAssets[index];
        if (revision.RevisionNumber != asset.Revisions.Count + 1)
            throw new InvalidOperationException("A visual asset revision must be appended using the next sequential revision number.");
        if (asset.Revisions.Any(item => item.Id == revision.Id))
            throw new InvalidOperationException($"Visual asset revision '{revision.Id}' already exists.");
        _visualAssets[index] = asset with { Revisions = [.. asset.Revisions, revision] };
    }

    private static void ReplaceById<T>(List<T> records, Guid id, T replacement, string parameterName)
        where T : notnull
    {
        var index = records.FindIndex(record => GetId(record) == id);
        if (index < 0)
            throw new KeyNotFoundException($"No record with ID '{id}' exists in this collection ({parameterName}).");
        records[index] = replacement;
    }

    private static Guid GetId<T>(T record) where T : notnull => record switch
    {
        StoryEvent storyEvent => storyEvent.Id,
        StoryPremise premise => premise.Id,
        PlaceRecord place => place.Id,
        ManuscriptEdition manuscript => manuscript.Id,
        TimelinePlanningPoint point => point.Id,
        ObserverKnowledgeRecord observerRecord => observerRecord.Id,
        CharacterProfile character => character.Id,
        OrganizationRecord organization => organization.Id,
        SceneRecord scene => scene.Id,
        PageComposition composition => composition.Id,
        ParticipantAction action => action.Id,
        AdventureRecord adventure => adventure.Id,
        QuestRecord quest => quest.Id,
        QuestObjectiveRecord objective => objective.Id,
        CampaignSessionRecord campaignSession => campaignSession.Id,
        _ => throw new InvalidOperationException($"ReplaceById does not support record type {typeof(T).Name}.")
    };

    private static void EnsureUnique(IEnumerable<Guid> existingIds, Guid id, string parameterName)
    {
        if (existingIds.Contains(id))
            throw new InvalidOperationException($"A record with ID '{id}' already exists in this collection ({parameterName}).");
    }
}
