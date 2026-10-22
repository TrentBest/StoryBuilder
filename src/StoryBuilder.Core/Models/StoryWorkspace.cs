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

/// <summary>
/// An in-memory workspace boundary that groups the build record and fictional story records.
/// Persistence and presentation are host responsibilities; the two record families remain distinct.
/// </summary>
public sealed class StoryWorkspace
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = "StoryBuilder";
    public ExperienceDefinition Experience { get; set; } = new();
    public List<CharacterProfile> Characters { get; } = [];
    public List<SceneRecord> Scenes { get; } = [];
    public List<HistoricalClaim> HistoricalClaims { get; } = [];
    public List<StoryEvent> Events { get; } = [];
    public List<CampaignRecord> Campaigns { get; } = [];
    public List<ParticipantAction> ParticipantActions { get; } = [];
    public List<SourceRecord> Sources { get; } = [];
    public List<ContinuityIssue> ContinuityIssues { get; } = [];

    public void AddCharacter(CharacterProfile character)
    {
        ArgumentNullException.ThrowIfNull(character);
        EnsureUnique(Characters.Select(x => x.Id), character.Id, nameof(character));
        Characters.Add(character);
    }

    public void AddScene(SceneRecord scene)
    {
        ArgumentNullException.ThrowIfNull(scene);
        EnsureUnique(Scenes.Select(x => x.Id), scene.Id, nameof(scene));
        Scenes.Add(scene);
    }

    public void AddClaim(HistoricalClaim claim)
    {
        ArgumentNullException.ThrowIfNull(claim);
        EnsureUnique(HistoricalClaims.Select(x => x.Id), claim.Id, nameof(claim));
        HistoricalClaims.Add(claim);
    }

    public void AddEvent(StoryEvent storyEvent)
    {
        ArgumentNullException.ThrowIfNull(storyEvent);
        EnsureUnique(Events.Select(x => x.Id), storyEvent.Id, nameof(storyEvent));
        Events.Add(storyEvent);
    }

    public void AddCampaign(CampaignRecord campaign)
    {
        ArgumentNullException.ThrowIfNull(campaign);
        EnsureUnique(Campaigns.Select(x => x.Id), campaign.Id, nameof(campaign));
        Campaigns.Add(campaign);
    }

    private static void EnsureUnique(IEnumerable<Guid> existingIds, Guid id, string parameterName)
    {
        if (existingIds.Contains(id))
            throw new InvalidOperationException($"A record with ID '{id}' already exists in this collection ({parameterName}).");
    }
}
