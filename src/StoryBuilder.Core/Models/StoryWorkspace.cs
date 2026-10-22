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
    public List<Guid> CauseEventIds { get; init; } = [];
    public List<Guid> ConsequenceEventIds { get; init; } = [];
    public List<Guid> SourceIds { get; init; } = [];
    public CanonStatus Status { get; init; } = CanonStatus.Proposed;
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
    private readonly List<StoryPremise> _premises = [];
    private readonly List<CharacterProfile> _characters = [];
    private readonly List<PlaceRecord> _places = [];
    private readonly List<SceneRecord> _scenes = [];
    private readonly List<HistoricalClaim> _historicalClaims = [];
    private readonly List<StoryEvent> _events = [];
    private readonly List<CampaignRecord> _campaigns = [];
    private readonly List<ParticipantAction> _participantActions = [];
    private readonly List<SourceRecord> _sources = [];
    private readonly List<ContinuityIssue> _continuityIssues = [];

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
    public IReadOnlyList<StoryPremise> Premises => _premises.AsReadOnly();
    public IReadOnlyList<CharacterProfile> Characters => _characters.AsReadOnly();
    public IReadOnlyList<PlaceRecord> Places => _places.AsReadOnly();
    public IReadOnlyList<SceneRecord> Scenes => _scenes.AsReadOnly();
    public IReadOnlyList<HistoricalClaim> HistoricalClaims => _historicalClaims.AsReadOnly();
    public IReadOnlyList<StoryEvent> Events => _events.AsReadOnly();
    public IReadOnlyList<CampaignRecord> Campaigns => _campaigns.AsReadOnly();
    public IReadOnlyList<ParticipantAction> ParticipantActions => _participantActions.AsReadOnly();
    public IReadOnlyList<SourceRecord> Sources => _sources.AsReadOnly();
    public IReadOnlyList<ContinuityIssue> ContinuityIssues => _continuityIssues.AsReadOnly();

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
        if (edition.PreviousEditionId is Guid previousId && !_publishedEditions.Any(item => item.Id == previousId))
            throw new KeyNotFoundException($"Previous publication edition '{previousId}' must exist before a corrected edition can be added.");
        _publishedEditions.Add(edition);
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
        _campaigns.Add(campaign);
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
        SceneRecord scene => scene.Id,
        ParticipantAction action => action.Id,
        _ => throw new InvalidOperationException($"ReplaceById does not support record type {typeof(T).Name}.")
    };

    private static void EnsureUnique(IEnumerable<Guid> existingIds, Guid id, string parameterName)
    {
        if (existingIds.Contains(id))
            throw new InvalidOperationException($"A record with ID '{id}' already exists in this collection ({parameterName}).");
    }
}
