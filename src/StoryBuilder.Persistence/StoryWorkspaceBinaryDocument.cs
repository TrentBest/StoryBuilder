using System.Text;
using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using TheSingularityWorkshop.FSM_Serialization;

namespace StoryBuilder.Persistence;

/// <summary>
/// Versioned, domain-specific binary document for a StoryWorkspace and its explicit synonym mappings.
/// It uses FSM_Serialization's byte boundary; this is not a general-purpose object graph serializer.
/// </summary>
public sealed class StoryWorkspaceBinaryDocument : IBinarySerializable
{
    private const int FormatVersion = 12;
    private static readonly byte[] Magic = Encoding.ASCII.GetBytes("SBWS");

    public StoryWorkspaceBinaryDocument()
    {
    }

    public StoryWorkspaceBinaryDocument(StoryWorkspace workspace, IEnumerable<VocabularyConcept>? synonyms = null)
    {
        Workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
        Synonyms = synonyms?.ToList() ?? [];
    }

    public StoryWorkspace Workspace { get; private set; } = new();
    public List<VocabularyConcept> Synonyms { get; private set; } = [];

    public static StoryWorkspaceBinaryDocument FromSession(StoryBuilderSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        return new StoryWorkspaceBinaryDocument(session.Workspace, session.Vocabulary.ExportConcepts());
    }

    public void RestoreInto(StoryBuilderSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        session.RestoreFrom(Workspace, Synonyms);
    }

    public void Pack(IBinaryStream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (!stream.CanWrite)
            throw new ArgumentException("The binary stream is not writable.", nameof(stream));

        EnsureWorkspaceIntegrity(Workspace);
        stream.Write(Magic);
        BinaryCodec.WriteInt32(stream, FormatVersion);
        WriteWorkspace(stream, Workspace);
        BinaryCodec.WriteList(stream, Synonyms, WriteVocabularyConcept);
    }

    public void Unpack(IBinaryStream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (!stream.CanRead)
            throw new ArgumentException("The binary stream is not readable.", nameof(stream));

        Span<byte> magic = stackalloc byte[4];
        BinaryCodec.ReadExactly(stream, magic);
        if (!magic.SequenceEqual(Magic))
            throw new InvalidDataException("The payload is not a StoryBuilder workspace document.");

        var version = BinaryCodec.ReadInt32(stream);
        if (version is not 1 and not 2 and not 3 and not 4 and not 5 and not 6 and not 7 and not 8 and not 9 and not 10 and not 11 and not FormatVersion)
            throw new InvalidDataException($"Unsupported StoryBuilder workspace format version: {version}.");

        Workspace = ReadWorkspace(stream, version);
        Synonyms = BinaryCodec.ReadList(stream, ReadVocabularyConcept);
        EnsureWorkspaceIntegrity(Workspace);

        if (stream.Position != stream.Length)
            throw new InvalidDataException("The workspace payload contains trailing bytes.");
    }

    private static void EnsureWorkspaceIntegrity(StoryWorkspace workspace)
    {
        var issues = StoryWorkspaceValidator.Validate(workspace);
        if (issues.Count == 0)
            return;

        var preview = string.Join(Environment.NewLine, issues.Take(5).Select(issue => issue.Message));
        var remainder = issues.Count > 5
            ? $"{Environment.NewLine}...and {issues.Count - 5} more broken reference(s)."
            : string.Empty;
        throw new InvalidDataException(
            $"The StoryBuilder workspace contains {issues.Count} broken reference(s) and cannot be persisted or restored.{Environment.NewLine}{preview}{remainder}");
    }

    private static void WriteWorkspace(IBinaryStream stream, StoryWorkspace workspace)
    {
        BinaryCodec.WriteGuid(stream, workspace.Id);
        BinaryCodec.WriteString(stream, workspace.Name);
        WriteExperience(stream, workspace.Experience);
        BinaryCodec.WriteList(stream, workspace.Timelines, WriteTimeline);
        BinaryCodec.WriteList(stream, workspace.PlanningPoints, WritePlanningPoint);
        BinaryCodec.WriteList(stream, workspace.Manuscripts, WriteManuscript);
        BinaryCodec.WriteList(stream, workspace.Premises, WritePremise);
        BinaryCodec.WriteList(stream, workspace.Characters, WriteCharacter);
        BinaryCodec.WriteList(stream, workspace.Places, WritePlace);
        BinaryCodec.WriteList(stream, workspace.Scenes, WriteScene);
        BinaryCodec.WriteList(stream, workspace.HistoricalClaims, WriteHistoricalClaim);
        BinaryCodec.WriteList(stream, workspace.Events, WriteStoryEvent);
        BinaryCodec.WriteList(stream, workspace.ObserverKnowledge, WriteObserverKnowledge);
        BinaryCodec.WriteList(stream, workspace.Campaigns, WriteCampaign);
        BinaryCodec.WriteList(stream, workspace.ParticipantActions, WriteParticipantAction);
        BinaryCodec.WriteList(stream, workspace.Sources, WriteSource);
        BinaryCodec.WriteList(stream, workspace.ContinuityIssues, WriteContinuityIssue);
        BinaryCodec.WriteString(stream, workspace.LegacyProjectSourceJson);
        BinaryCodec.WriteList(stream, workspace.PublishedEditions, WritePublishedEdition);
    }

    private static StoryWorkspace ReadWorkspace(IBinaryStream stream, int version)
    {
        var id = BinaryCodec.ReadGuid(stream);
        var name = BinaryCodec.ReadRequiredString(stream, nameof(StoryWorkspace.Name));
        var experience = ReadExperience(stream);
        var timelines = version >= 3 ? BinaryCodec.ReadList(stream, ReadTimeline) : [];
        var planningPoints = version >= 5 ? BinaryCodec.ReadList(stream, item => ReadPlanningPoint(item, version)) : [];
        var manuscripts = version >= 4 ? BinaryCodec.ReadList(stream, ReadManuscript) : [];
        var premises = version >= 2 ? BinaryCodec.ReadList(stream, ReadPremise) : [];
        var characters = BinaryCodec.ReadList(stream, item => ReadCharacter(item, version));
        var places = version >= 10 ? BinaryCodec.ReadList(stream, ReadPlace) : [];
        var scenes = BinaryCodec.ReadList(stream, item => ReadScene(item, version));
        var claims = BinaryCodec.ReadList(stream, ReadHistoricalClaim);
        var events = BinaryCodec.ReadList(stream, item => ReadStoryEvent(item, version));
        var observerKnowledge = version >= 7 ? BinaryCodec.ReadList(stream, ReadObserverKnowledge) : [];
        var campaigns = BinaryCodec.ReadList(stream, ReadCampaign);
        var actions = BinaryCodec.ReadList(stream, ReadParticipantAction);
        var sources = BinaryCodec.ReadList(stream, ReadSource);
        var issues = BinaryCodec.ReadList(stream, ReadContinuityIssue);
        var legacyProjectSourceJson = version >= 9 ? BinaryCodec.ReadString(stream) : null;
        var publishedEditions = version >= 11 ? BinaryCodec.ReadList(stream, ReadPublishedEdition) : [];

        var workspace = new StoryWorkspace { Id = id, Name = name, Experience = experience, LegacyProjectSourceJson = legacyProjectSourceJson };
        foreach (var item in timelines) workspace.AddTimeline(item);
        foreach (var item in planningPoints) workspace.AddPlanningPoint(item);
        foreach (var item in manuscripts) workspace.AddManuscript(item);
        // Persisted order preserves correction lineage: an earlier edition is added before its correction.
        foreach (var item in publishedEditions) workspace.AddPublishedEdition(item);
        foreach (var item in premises) workspace.AddPremise(item);
        foreach (var item in characters) workspace.AddCharacter(item);
        foreach (var item in places) workspace.AddPlace(item);
        foreach (var item in scenes) workspace.AddScene(item);
        foreach (var item in claims) workspace.AddClaim(item);
        foreach (var item in events) workspace.AddEvent(item);
        foreach (var item in observerKnowledge) workspace.AddObserverKnowledge(item);
        foreach (var item in campaigns) workspace.AddCampaign(item);
        foreach (var item in actions) workspace.AddParticipantAction(item);
        foreach (var item in sources) workspace.AddSource(item);
        foreach (var item in issues) workspace.AddContinuityIssue(item);
        return workspace;
    }

    private static void WriteExperience(IBinaryStream stream, ExperienceDefinition value)
    {
        BinaryCodec.WriteGuid(stream, value.Id);
        BinaryCodec.WriteString(stream, value.Name);
        BinaryCodec.WriteString(stream, value.Purpose);
        BinaryCodec.WriteString(stream, value.Audience);
        BinaryCodec.WriteStringList(stream, value.Capabilities);
        BinaryCodec.WriteStringList(stream, value.Constraints);
        BinaryCodec.WriteList(stream, value.Decisions, WriteDesignDecision);
    }

    private static ExperienceDefinition ReadExperience(IBinaryStream stream) => new()
    {
        Id = BinaryCodec.ReadGuid(stream),
        Name = BinaryCodec.ReadRequiredString(stream, nameof(ExperienceDefinition.Name)),
        Purpose = BinaryCodec.ReadRequiredString(stream, nameof(ExperienceDefinition.Purpose)),
        Audience = BinaryCodec.ReadRequiredString(stream, nameof(ExperienceDefinition.Audience)),
        Capabilities = BinaryCodec.ReadStringList(stream),
        Constraints = BinaryCodec.ReadStringList(stream),
        Decisions = BinaryCodec.ReadList(stream, ReadDesignDecision)
    };

    private static void WriteDesignDecision(IBinaryStream stream, DesignDecision value)
    {
        BinaryCodec.WriteGuid(stream, value.Id);
        BinaryCodec.WriteString(stream, value.Decision);
        BinaryCodec.WriteString(stream, value.Rationale);
        BinaryCodec.WriteEnum(stream, value.Status);
        BinaryCodec.WriteString(stream, value.EvidenceReference);
    }

    private static DesignDecision ReadDesignDecision(IBinaryStream stream) => new()
    {
        Id = BinaryCodec.ReadGuid(stream),
        Decision = BinaryCodec.ReadRequiredString(stream, nameof(DesignDecision.Decision)),
        Rationale = BinaryCodec.ReadRequiredString(stream, nameof(DesignDecision.Rationale)),
        Status = BinaryCodec.ReadEnum<CanonStatus>(stream),
        EvidenceReference = BinaryCodec.ReadString(stream)
    };

    private static void WriteTimeline(IBinaryStream stream, TimelineDefinition value)
    {
        BinaryCodec.WriteGuid(stream, value.Id);
        BinaryCodec.WriteString(stream, value.Name);
        BinaryCodec.WriteString(stream, value.Description);
        BinaryCodec.WriteNullableGuid(stream, value.ParentTimelineId);
        BinaryCodec.WriteDateTimeOffset(stream, value.CreatedAtUtc);
    }

    private static TimelineDefinition ReadTimeline(IBinaryStream stream) => new()
    {
        Id = BinaryCodec.ReadGuid(stream),
        Name = BinaryCodec.ReadRequiredString(stream, nameof(TimelineDefinition.Name)),
        Description = BinaryCodec.ReadRequiredString(stream, nameof(TimelineDefinition.Description)),
        ParentTimelineId = BinaryCodec.ReadNullableGuid(stream),
        CreatedAtUtc = BinaryCodec.ReadDateTimeOffset(stream)
    };

    private static void WritePlanningPoint(IBinaryStream stream, TimelinePlanningPoint value)
    {
        BinaryCodec.WriteGuid(stream, value.Id);
        BinaryCodec.WriteGuid(stream, value.TimelineId);
        BinaryCodec.WriteString(stream, value.Label);
        BinaryCodec.WriteString(stream, value.Intent);
        BinaryCodec.WriteString(stream, value.ApproximatePlacement);
        BinaryCodec.WriteEnum(stream, value.PlacementKind);
        BinaryCodec.WriteEnum(stream, value.Status);
        BinaryCodec.WriteStringList(stream, value.Alternatives);
        BinaryCodec.WriteGuidList(stream, value.RelatedSceneIds);
        BinaryCodec.WriteGuidList(stream, value.RelatedEventIds);
        BinaryCodec.WriteNullableGuid(stream, value.PromotedEventId);
        BinaryCodec.WriteDateTimeOffset(stream, value.CreatedAtUtc);
        BinaryCodec.WriteDateTimeOffset(stream, value.UpdatedAtUtc);
    }

    private static TimelinePlanningPoint ReadPlanningPoint(IBinaryStream stream, int version)
    {
        var id = BinaryCodec.ReadGuid(stream);
        var timelineId = BinaryCodec.ReadGuid(stream);
        var label = BinaryCodec.ReadRequiredString(stream, nameof(TimelinePlanningPoint.Label));
        var intent = BinaryCodec.ReadRequiredString(stream, nameof(TimelinePlanningPoint.Intent));
        var approximatePlacement = BinaryCodec.ReadRequiredString(stream, nameof(TimelinePlanningPoint.ApproximatePlacement));
        var placementKind = BinaryCodec.ReadEnum<TimelinePlacementKind>(stream);
        var status = BinaryCodec.ReadEnum<TimelinePlanningStatus>(stream);
        var alternatives = BinaryCodec.ReadStringList(stream);
        var relatedSceneIds = BinaryCodec.ReadGuidList(stream);
        var relatedEventIds = BinaryCodec.ReadGuidList(stream);
        var promotedEventId = version >= 12 ? BinaryCodec.ReadNullableGuid(stream) : null;
        return new TimelinePlanningPoint
        {
            Id = id,
            TimelineId = timelineId,
            Label = label,
            Intent = intent,
            ApproximatePlacement = approximatePlacement,
            PlacementKind = placementKind,
            Status = status,
            Alternatives = alternatives,
            RelatedSceneIds = relatedSceneIds,
            RelatedEventIds = relatedEventIds,
            PromotedEventId = promotedEventId,
            CreatedAtUtc = BinaryCodec.ReadDateTimeOffset(stream),
            UpdatedAtUtc = BinaryCodec.ReadDateTimeOffset(stream)
        };
    }

    private static void WriteManuscript(IBinaryStream stream, ManuscriptEdition value)
    {
        BinaryCodec.WriteGuid(stream, value.Id);
        BinaryCodec.WriteString(stream, value.Title);
        BinaryCodec.WriteString(stream, value.Subtitle);
        BinaryCodec.WriteString(stream, value.Author);
        BinaryCodec.WriteEnum(stream, value.Status);
        BinaryCodec.WriteList(stream, value.Sections, WriteManuscriptSection);
        BinaryCodec.WriteDateTimeOffset(stream, value.CreatedAtUtc);
        BinaryCodec.WriteDateTimeOffset(stream, value.UpdatedAtUtc);
    }

    private static ManuscriptEdition ReadManuscript(IBinaryStream stream) => new()
    {
        Id = BinaryCodec.ReadGuid(stream),
        Title = BinaryCodec.ReadRequiredString(stream, nameof(ManuscriptEdition.Title)),
        Subtitle = BinaryCodec.ReadRequiredString(stream, nameof(ManuscriptEdition.Subtitle)),
        Author = BinaryCodec.ReadRequiredString(stream, nameof(ManuscriptEdition.Author)),
        Status = BinaryCodec.ReadEnum<CanonStatus>(stream),
        Sections = BinaryCodec.ReadList(stream, ReadManuscriptSection),
        CreatedAtUtc = BinaryCodec.ReadDateTimeOffset(stream),
        UpdatedAtUtc = BinaryCodec.ReadDateTimeOffset(stream)
    };

    private static void WriteManuscriptSection(IBinaryStream stream, ManuscriptSection value)
    {
        BinaryCodec.WriteGuid(stream, value.Id);
        BinaryCodec.WriteInt32(stream, value.Sequence);
        BinaryCodec.WriteEnum(stream, value.Kind);
        BinaryCodec.WriteString(stream, value.Title);
        BinaryCodec.WriteString(stream, value.Body);
        BinaryCodec.WriteGuidList(stream, value.SceneIds);
        BinaryCodec.WriteGuidList(stream, value.EventIds);
    }

    private static ManuscriptSection ReadManuscriptSection(IBinaryStream stream) => new()
    {
        Id = BinaryCodec.ReadGuid(stream),
        Sequence = BinaryCodec.ReadInt32(stream),
        Kind = BinaryCodec.ReadEnum<ManuscriptSectionKind>(stream),
        Title = BinaryCodec.ReadRequiredString(stream, nameof(ManuscriptSection.Title)),
        Body = BinaryCodec.ReadRequiredString(stream, nameof(ManuscriptSection.Body)),
        SceneIds = BinaryCodec.ReadGuidList(stream),
        EventIds = BinaryCodec.ReadGuidList(stream)
    };

    private static void WritePublishedEdition(IBinaryStream stream, PublishedBookEdition value)
    {
        BinaryCodec.WriteGuid(stream, value.Id);
        BinaryCodec.WriteGuid(stream, value.ManuscriptId);
        BinaryCodec.WriteString(stream, value.Title);
        BinaryCodec.WriteString(stream, value.Subtitle);
        BinaryCodec.WriteString(stream, value.Author);
        BinaryCodec.WriteString(stream, value.Markdown);
        BinaryCodec.WriteString(stream, value.ContentSha256);
        BinaryCodec.WriteDateTimeOffset(stream, value.PublishedAtUtc);
        BinaryCodec.WriteDateTimeOffset(stream, value.SourceUpdatedAtUtc);
        BinaryCodec.WriteNullableGuid(stream, value.PreviousEditionId);
        BinaryCodec.WriteString(stream, value.CorrectionNotes);
        BinaryCodec.WriteGuidList(stream, value.SectionIds);
        BinaryCodec.WriteGuidList(stream, value.SceneIds);
        BinaryCodec.WriteGuidList(stream, value.EventIds);
    }

    private static PublishedBookEdition ReadPublishedEdition(IBinaryStream stream) => new()
    {
        Id = BinaryCodec.ReadGuid(stream),
        ManuscriptId = BinaryCodec.ReadGuid(stream),
        Title = BinaryCodec.ReadRequiredString(stream, nameof(PublishedBookEdition.Title)),
        Subtitle = BinaryCodec.ReadRequiredString(stream, nameof(PublishedBookEdition.Subtitle)),
        Author = BinaryCodec.ReadRequiredString(stream, nameof(PublishedBookEdition.Author)),
        Markdown = BinaryCodec.ReadRequiredString(stream, nameof(PublishedBookEdition.Markdown)),
        ContentSha256 = BinaryCodec.ReadRequiredString(stream, nameof(PublishedBookEdition.ContentSha256)),
        PublishedAtUtc = BinaryCodec.ReadDateTimeOffset(stream),
        SourceUpdatedAtUtc = BinaryCodec.ReadDateTimeOffset(stream),
        PreviousEditionId = BinaryCodec.ReadNullableGuid(stream),
        CorrectionNotes = BinaryCodec.ReadString(stream),
        SectionIds = BinaryCodec.ReadGuidList(stream),
        SceneIds = BinaryCodec.ReadGuidList(stream),
        EventIds = BinaryCodec.ReadGuidList(stream)
    };

    private static void WritePremise(IBinaryStream stream, StoryPremise value)
    {
        BinaryCodec.WriteGuid(stream, value.Id);
        BinaryCodec.WriteString(stream, value.Title);
        BinaryCodec.WriteString(stream, value.Vision);
        BinaryCodec.WriteString(stream, value.Scale);
        BinaryCodec.WriteString(stream, value.Genre);
        BinaryCodec.WriteString(stream, value.CreationApproach);
        BinaryCodec.WriteString(stream, value.PrimaryTarget);
        BinaryCodec.WriteEnum(stream, value.Status);
        BinaryCodec.WriteDateTimeOffset(stream, value.CreatedAtUtc);
        BinaryCodec.WriteDateTimeOffset(stream, value.UpdatedAtUtc);
    }

    private static StoryPremise ReadPremise(IBinaryStream stream) => new()
    {
        Id = BinaryCodec.ReadGuid(stream),
        Title = BinaryCodec.ReadRequiredString(stream, nameof(StoryPremise.Title)),
        Vision = BinaryCodec.ReadRequiredString(stream, nameof(StoryPremise.Vision)),
        Scale = BinaryCodec.ReadString(stream),
        Genre = BinaryCodec.ReadString(stream),
        CreationApproach = BinaryCodec.ReadString(stream),
        PrimaryTarget = BinaryCodec.ReadString(stream),
        Status = BinaryCodec.ReadEnum<CanonStatus>(stream),
        CreatedAtUtc = BinaryCodec.ReadDateTimeOffset(stream),
        UpdatedAtUtc = BinaryCodec.ReadDateTimeOffset(stream)
    };

    private static void WriteCharacter(IBinaryStream stream, CharacterProfile value)
    {
        BinaryCodec.WriteGuid(stream, value.Id);
        BinaryCodec.WriteString(stream, value.Name);
        BinaryCodec.WriteString(stream, value.Role);
        BinaryCodec.WriteString(stream, value.Description);
        BinaryCodec.WriteString(stream, value.Pronouns);
        BinaryCodec.WriteString(stream, value.Species);
        BinaryCodec.WriteEnum(stream, value.NarrativeRole);
        BinaryCodec.WriteEnum(stream, value.Status);
        BinaryCodec.WriteList(stream, value.Traits, WriteCharacterTrait);
        BinaryCodec.WriteList(stream, value.Relationships, WriteCharacterRelationship);
        BinaryCodec.WriteCount(stream, value.Notes.Count);
        foreach (var note in value.Notes.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase))
        {
            BinaryCodec.WriteString(stream, note.Key);
            BinaryCodec.WriteString(stream, note.Value);
        }
        BinaryCodec.WriteDateTimeOffset(stream, value.CreatedAtUtc);
        BinaryCodec.WriteDateTimeOffset(stream, value.UpdatedAtUtc);
    }

    private static CharacterProfile ReadCharacter(IBinaryStream stream, int version)
    {
        var id = BinaryCodec.ReadGuid(stream);
        var name = BinaryCodec.ReadRequiredString(stream, nameof(CharacterProfile.Name));
        var role = BinaryCodec.ReadRequiredString(stream, nameof(CharacterProfile.Role));
        var description = BinaryCodec.ReadRequiredString(stream, nameof(CharacterProfile.Description));
        var pronouns = BinaryCodec.ReadString(stream);
        var species = BinaryCodec.ReadString(stream);
        var narrativeRole = version >= 2
            ? BinaryCodec.ReadEnum<CharacterNarrativeRole>(stream)
            : CharacterNarrativeRole.Undecided;
        var status = BinaryCodec.ReadEnum<CanonStatus>(stream);
        var traits = BinaryCodec.ReadList(stream, ReadCharacterTrait);
        var relationships = BinaryCodec.ReadList(stream, ReadCharacterRelationship);
        var notes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var noteCount = BinaryCodec.ReadCount(stream);
        for (var i = 0; i < noteCount; i++)
            notes.Add(BinaryCodec.ReadRequiredString(stream, "note key"), BinaryCodec.ReadRequiredString(stream, "note value"));

        return new CharacterProfile
        {
            Id = id,
            Name = name,
            Role = role,
            Description = description,
            Pronouns = pronouns,
            Species = species,
            NarrativeRole = narrativeRole,
            Status = status,
            Traits = traits,
            Relationships = relationships,
            Notes = notes,
            CreatedAtUtc = BinaryCodec.ReadDateTimeOffset(stream),
            UpdatedAtUtc = BinaryCodec.ReadDateTimeOffset(stream)
        };
    }

    private static void WriteCharacterTrait(IBinaryStream stream, CharacterTrait value)
    {
        BinaryCodec.WriteGuid(stream, value.Id);
        BinaryCodec.WriteString(stream, value.Term);
        BinaryCodec.WriteString(stream, value.Category);
        BinaryCodec.WriteDouble(stream, value.Intensity);
        BinaryCodec.WriteString(stream, value.Notes);
        BinaryCodec.WriteString(stream, value.Attribution);
        BinaryCodec.WriteEnum(stream, value.Status);
    }

    private static CharacterTrait ReadCharacterTrait(IBinaryStream stream) => new()
    {
        Id = BinaryCodec.ReadGuid(stream),
        Term = BinaryCodec.ReadRequiredString(stream, nameof(CharacterTrait.Term)),
        Category = BinaryCodec.ReadRequiredString(stream, nameof(CharacterTrait.Category)),
        Intensity = BinaryCodec.ReadDouble(stream),
        Notes = BinaryCodec.ReadString(stream),
        Attribution = BinaryCodec.ReadRequiredString(stream, nameof(CharacterTrait.Attribution)),
        Status = BinaryCodec.ReadEnum<CanonStatus>(stream)
    };

    private static void WriteCharacterRelationship(IBinaryStream stream, CharacterRelationship value)
    {
        BinaryCodec.WriteGuid(stream, value.TargetCharacterId);
        BinaryCodec.WriteString(stream, value.Label);
        BinaryCodec.WriteString(stream, value.Notes);
        BinaryCodec.WriteEnum(stream, value.Status);
    }

    private static CharacterRelationship ReadCharacterRelationship(IBinaryStream stream) => new()
    {
        TargetCharacterId = BinaryCodec.ReadGuid(stream),
        Label = BinaryCodec.ReadRequiredString(stream, nameof(CharacterRelationship.Label)),
        Notes = BinaryCodec.ReadString(stream),
        Status = BinaryCodec.ReadEnum<CanonStatus>(stream)
    };

    private static void WritePlace(IBinaryStream stream, PlaceRecord value)
    {
        BinaryCodec.WriteGuid(stream, value.Id);
        BinaryCodec.WriteString(stream, value.Name);
        BinaryCodec.WriteString(stream, value.Description);
        BinaryCodec.WriteString(stream, value.Kind);
        BinaryCodec.WriteNullableGuid(stream, value.ParentPlaceId);
        BinaryCodec.WriteEnum(stream, value.Status);
        BinaryCodec.WriteDateTimeOffset(stream, value.CreatedAtUtc);
        BinaryCodec.WriteDateTimeOffset(stream, value.UpdatedAtUtc);
    }

    private static PlaceRecord ReadPlace(IBinaryStream stream) => new()
    {
        Id = BinaryCodec.ReadGuid(stream),
        Name = BinaryCodec.ReadRequiredString(stream, nameof(PlaceRecord.Name)),
        Description = BinaryCodec.ReadRequiredString(stream, nameof(PlaceRecord.Description)),
        Kind = BinaryCodec.ReadRequiredString(stream, nameof(PlaceRecord.Kind)),
        ParentPlaceId = BinaryCodec.ReadNullableGuid(stream),
        Status = BinaryCodec.ReadEnum<CanonStatus>(stream),
        CreatedAtUtc = BinaryCodec.ReadDateTimeOffset(stream),
        UpdatedAtUtc = BinaryCodec.ReadDateTimeOffset(stream)
    };

    private static void WriteScene(IBinaryStream stream, SceneRecord value)
    {
        BinaryCodec.WriteGuid(stream, value.Id);
        BinaryCodec.WriteString(stream, value.Title);
        BinaryCodec.WriteString(stream, value.Location);
        BinaryCodec.WriteNullableGuid(stream, value.PlaceId);
        BinaryCodec.WriteString(stream, value.Purpose);
        BinaryCodec.WriteEnum(stream, value.CaptureMode);
        BinaryCodec.WriteEnum(stream, value.Status);
        BinaryCodec.WriteList(stream, value.Beats, WriteSceneBeat);
        BinaryCodec.WriteDateTimeOffset(stream, value.CreatedAtUtc);
        BinaryCodec.WriteDateTimeOffset(stream, value.UpdatedAtUtc);
    }

    private static SceneRecord ReadScene(IBinaryStream stream, int version) => new()
    {
        Id = BinaryCodec.ReadGuid(stream),
        Title = BinaryCodec.ReadRequiredString(stream, nameof(SceneRecord.Title)),
        Location = BinaryCodec.ReadString(stream),
        PlaceId = version >= 10 ? BinaryCodec.ReadNullableGuid(stream) : null,
        Purpose = BinaryCodec.ReadString(stream),
        CaptureMode = BinaryCodec.ReadEnum<SceneCaptureMode>(stream),
        Status = BinaryCodec.ReadEnum<SceneStatus>(stream),
        Beats = BinaryCodec.ReadList(stream, item => ReadSceneBeat(item, version)),
        CreatedAtUtc = BinaryCodec.ReadDateTimeOffset(stream),
        UpdatedAtUtc = BinaryCodec.ReadDateTimeOffset(stream)
    };

    private static void WriteSceneBeat(IBinaryStream stream, SceneBeat value)
    {
        BinaryCodec.WriteGuid(stream, value.Id);
        BinaryCodec.WriteInt32(stream, value.Sequence);
        BinaryCodec.WriteEnum(stream, value.Kind);
        BinaryCodec.WriteNullableGuid(stream, value.ActorId);
        BinaryCodec.WriteString(stream, value.ActorLabel);
        BinaryCodec.WriteString(stream, value.Action);
        BinaryCodec.WriteString(stream, value.Reaction);
        BinaryCodec.WriteString(stream, value.Dialogue);
        BinaryCodec.WriteString(stream, value.PoseOrPosition);
        BinaryCodec.WriteString(stream, value.EmotionalTone);
        BinaryCodec.WriteString(stream, value.CaptureNote);
        BinaryCodec.WriteString(stream, value.Attribution);
        BinaryCodec.WriteEnum(stream, value.Status);
        BinaryCodec.WriteList(stream, value.EventIds, BinaryCodec.WriteGuid);
    }

    private static SceneBeat ReadSceneBeat(IBinaryStream stream, int version)
    {
        var beat = new SceneBeat
    {
        Id = BinaryCodec.ReadGuid(stream),
        Sequence = BinaryCodec.ReadInt32(stream),
        Kind = BinaryCodec.ReadEnum<SceneBeatKind>(stream),
        ActorId = BinaryCodec.ReadNullableGuid(stream),
        ActorLabel = BinaryCodec.ReadString(stream),
        Action = BinaryCodec.ReadString(stream),
        Reaction = BinaryCodec.ReadString(stream),
        Dialogue = BinaryCodec.ReadString(stream),
        PoseOrPosition = BinaryCodec.ReadString(stream),
        EmotionalTone = BinaryCodec.ReadString(stream),
        CaptureNote = BinaryCodec.ReadString(stream),
        Attribution = BinaryCodec.ReadRequiredString(stream, nameof(SceneBeat.Attribution)),
        Status = BinaryCodec.ReadEnum<CanonStatus>(stream)
        };
        return version >= 8 ? beat with { EventIds = BinaryCodec.ReadList(stream, BinaryCodec.ReadGuid) } : beat;
    }

    private static void WriteHistoricalClaim(IBinaryStream stream, HistoricalClaim value)
    {
        BinaryCodec.WriteGuid(stream, value.Id);
        BinaryCodec.WriteString(stream, value.Statement);
        BinaryCodec.WriteString(stream, value.InWorldDateOrPeriod);
        BinaryCodec.WriteString(stream, value.Attribution);
        BinaryCodec.WriteBoolean(stream, value.Confidence.HasValue);
        if (value.Confidence.HasValue) BinaryCodec.WriteDouble(stream, value.Confidence.Value);
        BinaryCodec.WriteEnum(stream, value.Status);
        BinaryCodec.WriteGuidList(stream, value.SourceIds);
        BinaryCodec.WriteGuidList(stream, value.RelatedEntityIds);
    }

    private static HistoricalClaim ReadHistoricalClaim(IBinaryStream stream)
    {
        var id = BinaryCodec.ReadGuid(stream);
        var statement = BinaryCodec.ReadRequiredString(stream, nameof(HistoricalClaim.Statement));
        var period = BinaryCodec.ReadString(stream);
        var attribution = BinaryCodec.ReadRequiredString(stream, nameof(HistoricalClaim.Attribution));
        double? confidence = BinaryCodec.ReadBoolean(stream) ? BinaryCodec.ReadDouble(stream) : null;
        return new HistoricalClaim
        {
            Id = id,
            Statement = statement,
            InWorldDateOrPeriod = period,
            Attribution = attribution,
            Confidence = confidence,
            Status = BinaryCodec.ReadEnum<CanonStatus>(stream),
            SourceIds = BinaryCodec.ReadGuidList(stream),
            RelatedEntityIds = BinaryCodec.ReadGuidList(stream)
        };
    }

    private static void WriteStoryEvent(IBinaryStream stream, StoryEvent value)
    {
        BinaryCodec.WriteGuid(stream, value.Id);
        BinaryCodec.WriteString(stream, value.Title);
        BinaryCodec.WriteString(stream, value.Summary);
        BinaryCodec.WriteString(stream, value.InWorldDateOrPeriod);
        BinaryCodec.WriteNullableGuid(stream, value.TimelineId);
        BinaryCodec.WriteEnum(stream, value.PlacementKind);
        BinaryCodec.WriteGuidList(stream, value.ParticipantIds);
        BinaryCodec.WriteGuidList(stream, value.CauseEventIds);
        BinaryCodec.WriteGuidList(stream, value.ConsequenceEventIds);
        BinaryCodec.WriteGuidList(stream, value.SourceIds);
        BinaryCodec.WriteEnum(stream, value.Status);
    }

    private static StoryEvent ReadStoryEvent(IBinaryStream stream, int version)
    {
        var id = BinaryCodec.ReadGuid(stream);
        var title = BinaryCodec.ReadRequiredString(stream, nameof(StoryEvent.Title));
        var summary = BinaryCodec.ReadRequiredString(stream, nameof(StoryEvent.Summary));
        var period = BinaryCodec.ReadString(stream);
        var timelineId = version >= 6 ? BinaryCodec.ReadNullableGuid(stream) : null;
        var placementKind = version >= 6 ? BinaryCodec.ReadEnum<TimelinePlacementKind>(stream) : TimelinePlacementKind.Unresolved;
        return new StoryEvent
        {
            Id = id,
            Title = title,
            Summary = summary,
            InWorldDateOrPeriod = period,
            TimelineId = timelineId,
            PlacementKind = placementKind,
            ParticipantIds = BinaryCodec.ReadGuidList(stream),
            CauseEventIds = BinaryCodec.ReadGuidList(stream),
            ConsequenceEventIds = BinaryCodec.ReadGuidList(stream),
            SourceIds = BinaryCodec.ReadGuidList(stream),
            Status = BinaryCodec.ReadEnum<CanonStatus>(stream)
        };
    }

    private static void WriteObserverKnowledge(IBinaryStream stream, ObserverKnowledgeRecord value)
    {
        BinaryCodec.WriteGuid(stream, value.Id);
        BinaryCodec.WriteGuid(stream, value.ObserverId);
        BinaryCodec.WriteGuid(stream, value.EventId);
        BinaryCodec.WriteEnum(stream, value.Kind);
        BinaryCodec.WriteNullableGuid(stream, value.AcquiredAtEventId);
        BinaryCodec.WriteNullableGuid(stream, value.SourceId);
        BinaryCodec.WriteString(stream, value.Attribution);
        BinaryCodec.WriteString(stream, value.Note);
        BinaryCodec.WriteBoolean(stream, value.Confidence.HasValue);
        if (value.Confidence.HasValue) BinaryCodec.WriteDouble(stream, value.Confidence.Value);
        BinaryCodec.WriteEnum(stream, value.Status);
        BinaryCodec.WriteDateTimeOffset(stream, value.CreatedAtUtc);
    }

    private static ObserverKnowledgeRecord ReadObserverKnowledge(IBinaryStream stream)
    {
        var id = BinaryCodec.ReadGuid(stream);
        var observerId = BinaryCodec.ReadGuid(stream);
        var eventId = BinaryCodec.ReadGuid(stream);
        var kind = BinaryCodec.ReadEnum<ObserverKnowledgeKind>(stream);
        var acquiredAtEventId = BinaryCodec.ReadNullableGuid(stream);
        var sourceId = BinaryCodec.ReadNullableGuid(stream);
        var attribution = BinaryCodec.ReadRequiredString(stream, nameof(ObserverKnowledgeRecord.Attribution));
        var note = BinaryCodec.ReadRequiredString(stream, nameof(ObserverKnowledgeRecord.Note));
        double? confidence = BinaryCodec.ReadBoolean(stream) ? BinaryCodec.ReadDouble(stream) : null;
        return new ObserverKnowledgeRecord
        {
            Id = id,
            ObserverId = observerId,
            EventId = eventId,
            Kind = kind,
            AcquiredAtEventId = acquiredAtEventId,
            SourceId = sourceId,
            Attribution = attribution,
            Note = note,
            Confidence = confidence,
            Status = BinaryCodec.ReadEnum<CanonStatus>(stream),
            CreatedAtUtc = BinaryCodec.ReadDateTimeOffset(stream)
        };
    }

    private static void WriteCampaign(IBinaryStream stream, CampaignRecord value)
    {
        BinaryCodec.WriteGuid(stream, value.Id);
        BinaryCodec.WriteString(stream, value.Name);
        BinaryCodec.WriteString(stream, value.Description);
        BinaryCodec.WriteGuidList(stream, value.ParticipantCharacterIds);
        BinaryCodec.WriteGuidList(stream, value.SceneIds);
        BinaryCodec.WriteGuidList(stream, value.EventIds);
        BinaryCodec.WriteEnum(stream, value.Status);
    }

    private static CampaignRecord ReadCampaign(IBinaryStream stream) => new()
    {
        Id = BinaryCodec.ReadGuid(stream),
        Name = BinaryCodec.ReadRequiredString(stream, nameof(CampaignRecord.Name)),
        Description = BinaryCodec.ReadRequiredString(stream, nameof(CampaignRecord.Description)),
        ParticipantCharacterIds = BinaryCodec.ReadGuidList(stream),
        SceneIds = BinaryCodec.ReadGuidList(stream),
        EventIds = BinaryCodec.ReadGuidList(stream),
        Status = BinaryCodec.ReadEnum<CanonStatus>(stream)
    };

    private static void WriteParticipantAction(IBinaryStream stream, ParticipantAction value)
    {
        BinaryCodec.WriteGuid(stream, value.Id);
        BinaryCodec.WriteGuid(stream, value.CampaignId);
        BinaryCodec.WriteNullableGuid(stream, value.CharacterId);
        BinaryCodec.WriteString(stream, value.Description);
        BinaryCodec.WriteString(stream, value.Attribution);
        BinaryCodec.WriteNullableGuid(stream, value.ResultingEventId);
        BinaryCodec.WriteEnum(stream, value.Status);
    }

    private static ParticipantAction ReadParticipantAction(IBinaryStream stream) => new()
    {
        Id = BinaryCodec.ReadGuid(stream),
        CampaignId = BinaryCodec.ReadGuid(stream),
        CharacterId = BinaryCodec.ReadNullableGuid(stream),
        Description = BinaryCodec.ReadRequiredString(stream, nameof(ParticipantAction.Description)),
        Attribution = BinaryCodec.ReadRequiredString(stream, nameof(ParticipantAction.Attribution)),
        ResultingEventId = BinaryCodec.ReadNullableGuid(stream),
        Status = BinaryCodec.ReadEnum<CanonStatus>(stream)
    };

    private static void WriteSource(IBinaryStream stream, SourceRecord value)
    {
        BinaryCodec.WriteGuid(stream, value.Id);
        BinaryCodec.WriteString(stream, value.Title);
        BinaryCodec.WriteString(stream, value.SourceType);
        BinaryCodec.WriteString(stream, value.LocationOrUri);
        BinaryCodec.WriteString(stream, value.Perspective);
        BinaryCodec.WriteString(stream, value.ReliabilityNote);
    }

    private static SourceRecord ReadSource(IBinaryStream stream) => new()
    {
        Id = BinaryCodec.ReadGuid(stream),
        Title = BinaryCodec.ReadRequiredString(stream, nameof(SourceRecord.Title)),
        SourceType = BinaryCodec.ReadRequiredString(stream, nameof(SourceRecord.SourceType)),
        LocationOrUri = BinaryCodec.ReadString(stream),
        Perspective = BinaryCodec.ReadString(stream),
        ReliabilityNote = BinaryCodec.ReadString(stream)
    };

    private static void WriteContinuityIssue(IBinaryStream stream, ContinuityIssue value)
    {
        BinaryCodec.WriteGuid(stream, value.Id);
        BinaryCodec.WriteString(stream, value.Description);
        BinaryCodec.WriteGuidList(stream, value.RelatedRecordIds);
        BinaryCodec.WriteString(stream, value.ResolutionNotes);
        BinaryCodec.WriteEnum(stream, value.Status);
    }

    private static ContinuityIssue ReadContinuityIssue(IBinaryStream stream) => new()
    {
        Id = BinaryCodec.ReadGuid(stream),
        Description = BinaryCodec.ReadRequiredString(stream, nameof(ContinuityIssue.Description)),
        RelatedRecordIds = BinaryCodec.ReadGuidList(stream),
        ResolutionNotes = BinaryCodec.ReadString(stream),
        Status = BinaryCodec.ReadEnum<CanonStatus>(stream)
    };

    private static void WriteVocabularyConcept(IBinaryStream stream, VocabularyConcept value)
    {
        BinaryCodec.WriteString(stream, value.CanonicalTerm);
        BinaryCodec.WriteEnum(stream, value.Category);
        BinaryCodec.WriteStringList(stream, value.Synonyms);
    }

    private static VocabularyConcept ReadVocabularyConcept(IBinaryStream stream) => new()
    {
        CanonicalTerm = BinaryCodec.ReadRequiredString(stream, nameof(VocabularyConcept.CanonicalTerm)),
        Category = BinaryCodec.ReadEnum<VocabularyCategory>(stream),
        Synonyms = BinaryCodec.ReadStringList(stream)
    };
}
