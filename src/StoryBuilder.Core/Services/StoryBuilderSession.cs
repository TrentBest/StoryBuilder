using StoryBuilder.Core.Models;

namespace StoryBuilder.Core.Services;

/// <summary>
/// Coordinates common authoring operations so edits and the vocabulary index stay synchronized.
/// This session is in-memory; the host is responsible for persistence.
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
        return character;
    }

    /// <summary>Creates and stores an empty scene so the user can add beats incrementally.</summary>
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
        return scene;
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
        foreach (var character in Workspace.Characters) IndexCharacter(character);
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

    private void IndexCharacter(CharacterProfile character)
    {
        Vocabulary.RecordText(character.Name, VocabularyCategory.Other, character.Id, character.Name,
            nameof(CharacterProfile.Name), context: character.Name, attribution: "Author", status: character.Status);
        Vocabulary.RecordText(character.Role, VocabularyCategory.Other, character.Id, character.Name,
            nameof(CharacterProfile.Role), context: character.Role, attribution: "Author", status: character.Status);
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
