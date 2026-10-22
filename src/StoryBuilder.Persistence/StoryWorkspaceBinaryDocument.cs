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
    private const int FormatVersion = 3;
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
        if (version is not 1 and not 2 and not FormatVersion)
            throw new InvalidDataException($"Unsupported StoryBuilder workspace format version: {version}.");

        Workspace = ReadWorkspace(stream, version);
        Synonyms = BinaryCodec.ReadList(stream, ReadVocabularyConcept);

        if (stream.Position != stream.Length)
            throw new InvalidDataException("The workspace payload contains trailing bytes.");
    }

    private static void WriteWorkspace(IBinaryStream stream, StoryWorkspace workspace)
    {
        BinaryCodec.WriteGuid(stream, workspace.Id);
        BinaryCodec.WriteString(stream, workspace.Name);
        WriteExperience(stream, workspace.Experience);
        BinaryCodec.WriteList(stream, workspace.Timelines, WriteTimeline);
        BinaryCodec.WriteList(stream, workspace.Premises, WritePremise);
        BinaryCodec.WriteList(stream, workspace.Characters, WriteCharacter);
        BinaryCodec.WriteList(stream, workspace.Scenes, WriteScene);
        BinaryCodec.WriteList(stream, workspace.HistoricalClaims, WriteHistoricalClaim);
        BinaryCodec.WriteList(stream, workspace.Events, WriteStoryEvent);
        BinaryCodec.WriteList(stream, workspace.Campaigns, WriteCampaign);
        BinaryCodec.WriteList(stream, workspace.ParticipantActions, WriteParticipantAction);
        BinaryCodec.WriteList(stream, workspace.Sources, WriteSource);
        BinaryCodec.WriteList(stream, workspace.ContinuityIssues, WriteContinuityIssue);
    }

    private static StoryWorkspace ReadWorkspace(IBinaryStream stream, int version)
    {
        var id = BinaryCodec.ReadGuid(stream);
        var name = BinaryCodec.ReadRequiredString(stream, nameof(StoryWorkspace.Name));
        var experience = ReadExperience(stream);
        var timelines = version >= 3 ? BinaryCodec.ReadList(stream, ReadTimeline) : [];
        var premises = version >= 2 ? BinaryCodec.ReadList(stream, ReadPremise) : [];
        var characters = BinaryCodec.ReadList(stream, item => ReadCharacter(item, version));
        var scenes = BinaryCodec.ReadList(stream, ReadScene);
        var claims = BinaryCodec.ReadList(stream, ReadHistoricalClaim);
        var events = BinaryCodec.ReadList(stream, ReadStoryEvent);
        var campaigns = BinaryCodec.ReadList(stream, ReadCampaign);
        var actions = BinaryCodec.ReadList(stream, ReadParticipantAction);
        var sources = BinaryCodec.ReadList(stream, ReadSource);
        var issues = BinaryCodec.ReadList(stream, ReadContinuityIssue);

        var workspace = new StoryWorkspace { Id = id, Name = name, Experience = experience };
        foreach (var item in timelines) workspace.AddTimeline(item);
        foreach (var item in premises) workspace.AddPremise(item);
        foreach (var item in characters) workspace.AddCharacter(item);
        foreach (var item in scenes) workspace.AddScene(item);
        foreach (var item in claims) workspace.AddClaim(item);
        foreach (var item in events) workspace.AddEvent(item);
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

    private static void WriteScene(IBinaryStream stream, SceneRecord value)
    {
        BinaryCodec.WriteGuid(stream, value.Id);
        BinaryCodec.WriteString(stream, value.Title);
        BinaryCodec.WriteString(stream, value.Location);
        BinaryCodec.WriteString(stream, value.Purpose);
        BinaryCodec.WriteEnum(stream, value.CaptureMode);
        BinaryCodec.WriteEnum(stream, value.Status);
        BinaryCodec.WriteList(stream, value.Beats, WriteSceneBeat);
        BinaryCodec.WriteDateTimeOffset(stream, value.CreatedAtUtc);
        BinaryCodec.WriteDateTimeOffset(stream, value.UpdatedAtUtc);
    }

    private static SceneRecord ReadScene(IBinaryStream stream) => new()
    {
        Id = BinaryCodec.ReadGuid(stream),
        Title = BinaryCodec.ReadRequiredString(stream, nameof(SceneRecord.Title)),
        Location = BinaryCodec.ReadString(stream),
        Purpose = BinaryCodec.ReadString(stream),
        CaptureMode = BinaryCodec.ReadEnum<SceneCaptureMode>(stream),
        Status = BinaryCodec.ReadEnum<SceneStatus>(stream),
        Beats = BinaryCodec.ReadList(stream, ReadSceneBeat),
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
    }

    private static SceneBeat ReadSceneBeat(IBinaryStream stream) => new()
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
        BinaryCodec.WriteGuidList(stream, value.ParticipantIds);
        BinaryCodec.WriteGuidList(stream, value.CauseEventIds);
        BinaryCodec.WriteGuidList(stream, value.ConsequenceEventIds);
        BinaryCodec.WriteGuidList(stream, value.SourceIds);
        BinaryCodec.WriteEnum(stream, value.Status);
    }

    private static StoryEvent ReadStoryEvent(IBinaryStream stream) => new()
    {
        Id = BinaryCodec.ReadGuid(stream),
        Title = BinaryCodec.ReadRequiredString(stream, nameof(StoryEvent.Title)),
        Summary = BinaryCodec.ReadRequiredString(stream, nameof(StoryEvent.Summary)),
        InWorldDateOrPeriod = BinaryCodec.ReadString(stream),
        ParticipantIds = BinaryCodec.ReadGuidList(stream),
        CauseEventIds = BinaryCodec.ReadGuidList(stream),
        ConsequenceEventIds = BinaryCodec.ReadGuidList(stream),
        SourceIds = BinaryCodec.ReadGuidList(stream),
        Status = BinaryCodec.ReadEnum<CanonStatus>(stream)
    };

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
